using System;
using System.IO;
using UnityEngine;

namespace UnityCodeIntel.Editor
{
    [Serializable]
    public class BridgeConfig
    {
        public int bridgePort = 0;
        public string bindAddress = "127.0.0.1";
        public string token = "";
        public int omnisharpPort = 0;
        public string omnisharpExePath = "Tools/CodeIntel/omnisharp/OmniSharp.exe";
        public string omnisharpJsonPath = "Tools/CodeIntel/omnisharp.json";
        public string solutionPath = "";
        public bool autoStartOnEditorLaunch = true;
        public bool autoRestartOnCompile = true;
        public int restartDebounceMs = 8000;
        public int restartCooldownMs = 15000;
        public int maxRestartsPer10Min = 20;
        public string logDir = "Library/CodeIntelLogs";
        public int heartbeatFailThreshold = 2;
        public int heartbeatQuickRetryDelayMs = 1500;
        public int startupHealthTimeoutMs = 5000;
        public int compileGraceMs = 12000;
        public int logDedupWindowMs = 60000;
        public int fatalCooldownMs = 120000;

        public static BridgeConfig Load(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    return JsonUtility.FromJson<BridgeConfig>(File.ReadAllText(path));
                }
                catch (Exception e)
                {
                    CILogger.LogError($"[CodeIntel] Failed to load config: {e.Message}");
                }
            }
            return new BridgeConfig();
        }
    }
}
