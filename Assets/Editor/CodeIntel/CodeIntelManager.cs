using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace UnityCodeIntel.Editor
{
    [InitializeOnLoad]
    public static class CodeIntelManager
    {
        public static OmniSharpProcess OmniSharp { get; private set; }
        public static BridgeServer Bridge { get; private set; }
        public static BridgeConfig Config { get; private set; }
        public static bool IsDegraded { get; private set; }
        public static int RestartsInLast10Min => _restartTimestamps.Count;
        public static string LastRestartReason => _lastRestartReason;
        public static string LastFatalCode { get; private set; } = "";
        public static string BridgeBaseUrl => Bridge != null && Bridge.IsRunning && Config != null
            ? $"http://{Config.bindAddress}:{Bridge.Port}/"
            : "";
        public static string RuntimeStatePath => GetRuntimeStatePath();

        private static string _projectRoot;
        private const string PID_KEY = "CodeIntel_OmniSharp_PID";
        private const string RUNTIME_STATE_FILENAME = "codeintel-endpoints.json";

        private static double _lastHeartbeatTime;
        private static bool _heartbeatCheckInProgress;
        private static int _consecutiveHeartbeatFails;
        private static double _lastSuccessfulHeartbeatAt = -1;
        private static double _degradedSince = -1;
        private static readonly List<double> _restartTimestamps = new List<double>();
        private static bool _startRequested;
        private static bool _restartRequested;
        private static double _restartRequestedAt;
        private static double _nextRestartAllowedAt;
        private static string _pendingRestartReason = "";
        private static string _lastRestartReason = "";
        private static double _restartCircuitOpenUntil;
        private static double _lastCircuitBreakLogAt = -9999.0;
        private static bool _isShuttingDown;
        private static bool _compileGracePending;
        private static bool _compileGraceCheckInProgress;
        private static double _compileGraceUntil = -1;
        private static double _lastCompileFinishedAt = -1;
        private static ServiceStatus _lastObservedOmniStatus = ServiceStatus.Stopped;
        private static long _lastObservedOmniOkTimestamp;

        static CodeIntelManager()
        {
            _projectRoot = Directory.GetCurrentDirectory();
            Config = BridgeConfig.Load(Path.Combine(_projectRoot, "Tools/CodeIntel/bridge-config.json"));

            CleanupZombieProcess();

            OmniSharp = new OmniSharpProcess();
            OmniSharp.UnexpectedExited += OnOmniSharpUnexpectedExited;
            Bridge = new BridgeServer(OmniSharp);

            BridgeServer.SetRestartReason("");
            SyncHealthSignalsToBridge(forceRuntimeState: true);

            EditorApplication.quitting += Shutdown;
            AssemblyReloadEvents.beforeAssemblyReload += BeforeAssemblyReload;
            EditorApplication.update += OnUpdate;
            CompilationPipeline.compilationFinished += OnCompilationFinished;

            if (Config.autoStartOnEditorLaunch)
            {
                EditorApplication.delayCall += RequestStartServices;
            }
        }

        public static void ReloadConfig()
        {
            Config = BridgeConfig.Load(Path.Combine(_projectRoot, "Tools/CodeIntel/bridge-config.json"));
            SyncHealthSignalsToBridge(forceRuntimeState: true);
        }

        public static void StartServices()
        {
            RequestStartServices();
        }

        public static void StopServices()
        {
            StopServicesInternal(clearHealthState: true);
        }

        public static void BatchSmoke_StartServicesAndQuit()
        {
            StartServices();
            double startedAt = EditorApplication.timeSinceStartup;
            EditorApplication.CallbackFunction tick = null;
            tick = () =>
            {
                if (EditorApplication.timeSinceStartup - startedAt < 10.0) return;
                EditorApplication.update -= tick;
                StopServices();
                EditorApplication.Exit(0);
            };
            EditorApplication.update += tick;
        }

        private static string GetRuntimeStatePath()
        {
            string projectRoot = _projectRoot;
            string logDirRel = string.IsNullOrEmpty(Config?.logDir) ? "Library/CodeIntelLogs" : Config.logDir;
            return Path.Combine(projectRoot, logDirRel, RUNTIME_STATE_FILENAME);
        }

        private static void Shutdown()
        {
            _isShuttingDown = true;
            EditorApplication.update -= OnUpdate;
            CompilationPipeline.compilationFinished -= OnCompilationFinished;
            AssemblyReloadEvents.beforeAssemblyReload -= BeforeAssemblyReload;
            EditorApplication.quitting -= Shutdown;
            StopServicesInternal(clearHealthState: true);
        }

        private static void BeforeAssemblyReload()
        {
            _isShuttingDown = true;
            StopServicesInternal(clearHealthState: true);
        }

        private static void OnUpdate()
        {
            double now = EditorApplication.timeSinceStartup;

            ObserveOmniRuntimeState();

            if (_restartRequested && now >= _nextRestartAllowedAt)
            {
                double debounceSeconds = Math.Max(0.0, (Config?.restartDebounceMs ?? 0) / 1000.0);
                if (now - _restartRequestedAt >= debounceSeconds)
                {
                    PerformRestart();
                }
            }

            if (_startRequested)
            {
                TryStartServicesNow();
            }

            if (_compileGracePending && now >= _compileGraceUntil && !_compileGraceCheckInProgress)
            {
                _compileGracePending = false;
                _compileGraceCheckInProgress = true;
                _ = EvaluatePostCompileHealthAsync();
            }

            if (!_heartbeatCheckInProgress && now - _lastHeartbeatTime > 30.0)
            {
                _lastHeartbeatTime = now;
                CheckHeartbeat();
            }
        }

        private static async void CheckHeartbeat()
        {
            if (_isShuttingDown || _heartbeatCheckInProgress) return;
            if (OmniSharp.Status != ServiceStatus.Running) return;

            _heartbeatCheckInProgress = true;
            try
            {
                bool alive = await OmniSharp.CheckHealthAsync();
                if (_isShuttingDown) return;

                if (alive)
                {
                    HandleHeartbeatSuccess();
                    return;
                }

                MarkHeartbeatFailure();
                int quickDelayMs = Math.Max(250, Config?.heartbeatQuickRetryDelayMs ?? 1500);
                try
                {
                    await Task.Delay(quickDelayMs);
                }
                catch
                {
                    return;
                }

                if (_isShuttingDown) return;

                bool recovered = await OmniSharp.CheckHealthAsync(force: true, timeoutMs: 4000);
                if (_isShuttingDown) return;

                if (recovered)
                {
                    HandleHeartbeatSuccess();
                    return;
                }

                MarkHeartbeatFailure();
                int threshold = Math.Max(2, Config?.heartbeatFailThreshold ?? 2);
                if (_consecutiveHeartbeatFails >= threshold)
                {
                    OmniSharp.MarkAsUnhealthy();
                    RequestRestart("heartbeat_unhealthy");
                }
            }
            finally
            {
                _heartbeatCheckInProgress = false;
            }
        }

        private static async Task EvaluatePostCompileHealthAsync()
        {
            try
            {
                if (_isShuttingDown) return;

                bool healthy = false;
                if (OmniSharp.Status == ServiceStatus.Running)
                {
                    int timeoutMs = Math.Max(1000, Config?.startupHealthTimeoutMs ?? 5000);
                    healthy = await OmniSharp.CheckHealthAsync(force: true, timeoutMs: timeoutMs);
                }

                if (_isShuttingDown) return;

                if (healthy)
                {
                    HandleHeartbeatSuccess();
                }
                else
                {
                    RequestRestart("post_compile_unhealthy");
                }
            }
            finally
            {
                _compileGraceCheckInProgress = false;
            }
        }

        private static void HandleHeartbeatSuccess()
        {
            _consecutiveHeartbeatFails = 0;
            _lastSuccessfulHeartbeatAt = EditorApplication.timeSinceStartup;

            if (IsDegraded)
            {
                IsDegraded = false;
                _degradedSince = -1;
                Debug.Log("[CodeIntel] OmniSharp heartbeat recovered.");
            }

            SyncHealthSignalsToBridge(forceRuntimeState: true);
        }

        private static void MarkHeartbeatFailure()
        {
            _consecutiveHeartbeatFails++;
            if (!IsDegraded)
            {
                IsDegraded = true;
                _degradedSince = EditorApplication.timeSinceStartup;
            }

            SyncHealthSignalsToBridge(forceRuntimeState: true);
        }

        private static void OnCompilationFinished(object context)
        {
            if (_isShuttingDown) return;
            if (Config == null) ReloadConfig();
            if (Config == null || !Config.autoRestartOnCompile) return;
            if (EditorUtility.scriptCompilationFailed) return;

            _lastCompileFinishedAt = EditorApplication.timeSinceStartup;
            _compileGraceUntil = _lastCompileFinishedAt + Math.Max(1.0, (Config.compileGraceMs / 1000.0));
            _compileGracePending = true;
            _compileGraceCheckInProgress = false;
        }

        private static void OnOmniSharpUnexpectedExited(int exitCode, string logTail)
        {
            if (_isShuttingDown) return;

            if (exitCode != -10002)
            {
                LastFatalCode = $"EXIT_{exitCode}";
            }

            string reason = exitCode == -10002 ? "startup_timeout_recoverable" : $"process_exit_{exitCode}";
            EditorApplication.delayCall += () => RequestRestart(reason);
        }

        private static void RequestStartServices()
        {
            if (_isShuttingDown) return;
            if (Config == null) ReloadConfig();

            _startRequested = true;
            TryStartServicesNow();
        }

        private static void TryStartServicesNow()
        {
            if (_isShuttingDown) return;
            if (Config == null) ReloadConfig();
            if (Config == null) return;

            if (EditorApplication.isCompiling || EditorApplication.isUpdating || UnityCompilationWatcher.IsCompiling)
            {
                return;
            }

            if (!AreProjectFilesStable(_projectRoot))
            {
                return;
            }

            _startRequested = false;

            if (OmniSharp.Status != ServiceStatus.Running && OmniSharp.Status != ServiceStatus.Starting)
            {
                OmniSharp.Start(_projectRoot, Config);
                if (OmniSharp.Pid > 0)
                {
                    EditorPrefs.SetInt(PID_KEY, OmniSharp.Pid);
                }
            }

            if (!Bridge.IsRunning)
            {
                Bridge.Start(Config);
            }

            SyncHealthSignalsToBridge(forceRuntimeState: true);
        }

        private static bool AreProjectFilesStable(string projectRoot)
        {
            try
            {
                var slnFiles = Directory.GetFiles(projectRoot, "*.sln", SearchOption.TopDirectoryOnly);
                if (slnFiles.Length == 0) return false;

                var candidates = new List<string>(slnFiles);
                candidates.AddRange(Directory.GetFiles(projectRoot, "*.csproj", SearchOption.TopDirectoryOnly));

                DateTime newestWriteUtc = DateTime.MinValue;
                foreach (var path in candidates)
                {
                    if (!File.Exists(path)) continue;
                    var fi = new FileInfo(path);
                    if (fi.Length == 0) return false;
                    if (fi.LastWriteTimeUtc > newestWriteUtc) newestWriteUtc = fi.LastWriteTimeUtc;
                }

                if (newestWriteUtc == DateTime.MinValue) return false;
                return (DateTime.UtcNow - newestWriteUtc).TotalSeconds >= 2.0;
            }
            catch
            {
                return false;
            }
        }

        private static void RequestRestart(string reason)
        {
            if (_isShuttingDown) return;
            if (Config == null) ReloadConfig();
            if (Config == null) return;

            _pendingRestartReason = string.IsNullOrEmpty(reason) ? "unspecified" : reason;
            _restartRequested = true;
            _restartRequestedAt = EditorApplication.timeSinceStartup;

            if (!IsDegraded)
            {
                IsDegraded = true;
                _degradedSince = _restartRequestedAt;
            }

            SyncHealthSignalsToBridge(forceRuntimeState: true);
        }

        private static void PerformRestart()
        {
            if (_isShuttingDown) return;
            if (Config == null) ReloadConfig();
            if (Config == null) return;

            double now = EditorApplication.timeSinceStartup;
            if (now < _restartCircuitOpenUntil)
            {
                _restartRequested = false;
                return;
            }

            _restartTimestamps.RemoveAll(t => now - t > 600.0);

            if (_restartTimestamps.Count >= Config.maxRestartsPer10Min)
            {
                _restartRequested = false;
                LastFatalCode = "RESTART_LIMIT";
                IsDegraded = true;
                _degradedSince = _degradedSince < 0 ? now : _degradedSince;
                _restartCircuitOpenUntil = now + Math.Max(30.0, (Config?.fatalCooldownMs ?? 120000) / 1000.0);

                if (now - _lastCircuitBreakLogAt >= 60.0)
                {
                    _lastCircuitBreakLogAt = now;
                    Debug.LogError("[CodeIntel] Max restart limit reached. Entering cooldown before next retry.");
                }

                SyncHealthSignalsToBridge(forceRuntimeState: true);
                return;
            }

            _restartRequested = false;
            _nextRestartAllowedAt = now + Math.Max(0.0, Config.restartCooldownMs / 1000.0);
            _restartTimestamps.Add(now);
            LastFatalCode = "";

            _lastRestartReason = string.IsNullOrEmpty(_pendingRestartReason) ? "unspecified" : _pendingRestartReason;
            _pendingRestartReason = "";
            BridgeServer.SetRestartReason(_lastRestartReason);
            BridgeServer.IncrementRestartSequence();

            Debug.Log($"[CodeIntel] Attempting auto-restart... Reason: {_lastRestartReason}");
            StopServicesInternal(clearHealthState: false);
            RequestStartServices();
            SyncHealthSignalsToBridge(forceRuntimeState: true);
        }

        private static void ObserveOmniRuntimeState()
        {
            if (OmniSharp == null || Bridge == null) return;

            bool changed = false;
            if (_lastObservedOmniStatus != OmniSharp.Status)
            {
                _lastObservedOmniStatus = OmniSharp.Status;
                changed = true;
            }

            if (_lastObservedOmniOkTimestamp != OmniSharp.LastOkTimestamp)
            {
                _lastObservedOmniOkTimestamp = OmniSharp.LastOkTimestamp;
                changed = true;
            }

            if (changed)
            {
                Bridge.UpdateRuntimeState();
            }
        }

        private static void StopServicesInternal(bool clearHealthState)
        {
            _startRequested = false;
            _restartRequested = false;
            _heartbeatCheckInProgress = false;
            _compileGracePending = false;
            _compileGraceCheckInProgress = false;
            _compileGraceUntil = -1;
            _pendingRestartReason = "";

            Bridge?.Stop();
            OmniSharp?.Stop();
            EditorPrefs.DeleteKey(PID_KEY);

            if (clearHealthState)
            {
                IsDegraded = false;
                _degradedSince = -1;
                _consecutiveHeartbeatFails = 0;
                _lastSuccessfulHeartbeatAt = -1;
                LastFatalCode = "";
            }

            SyncHealthSignalsToBridge(forceRuntimeState: true);
        }

        private static void SyncHealthSignalsToBridge(bool forceRuntimeState = false)
        {
            double now = EditorApplication.timeSinceStartup;
            _restartTimestamps.RemoveAll(t => now - t > 600.0);
            BridgeServer.SetHealthSignals(IsDegraded, _restartTimestamps.Count, LastFatalCode);
            Bridge?.UpdateRuntimeState(forceRuntimeState);
        }

        private static void CleanupZombieProcess()
        {
            if (EditorPrefs.HasKey(PID_KEY))
            {
                int pid = EditorPrefs.GetInt(PID_KEY);
                if (pid > 0)
                {
                    try
                    {
                        var proc = System.Diagnostics.Process.GetProcessById(pid);
                        if (!proc.HasExited)
                        {
                            proc.Kill();
                            Debug.Log($"[CodeIntel] Cleaned up zombie OmniSharp process (PID: {pid})");
                        }
                    }
                    catch
                    {
                        // Process already gone or access denied
                    }
                }
                EditorPrefs.DeleteKey(PID_KEY);
            }
        }
    }
}
