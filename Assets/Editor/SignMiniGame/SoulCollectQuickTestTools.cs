#if UNITY_EDITOR
using ITC.SignMiniGame;
using UnityEditor;
using UnityEngine;

namespace ITC.EditorTools.SignMiniGame
{
    [InitializeOnLoad]
    public static class SoulCollectQuickTestTools
    {
        private const string PendingRunKey = "ITC.SoulCollectQuickTestTools.PendingRun";

        static SoulCollectQuickTestTools()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [MenuItem("ITC/SignMiniGame/一键运行 05灵魂收取全流程测试")]
        public static void RunSoulCollectQuickTest()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("[灵魂收取快测] 当前已在运行中。");
                return;
            }

            SessionState.SetBool(PendingRunKey, true);
            Debug.Log("[灵魂收取快测] 已请求启动，进入 Play 后将自动创建临时启动器并跑完整流程。");
            EditorApplication.isPlaying = true;
        }

        [MenuItem("ITC/SignMiniGame/一键停止 05灵魂收取全流程测试")]
        public static void StopSoulCollectQuickTest()
        {
            SessionState.SetBool(PendingRunKey, false);
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
            {
                SessionState.SetBool(PendingRunKey, false);
                return;
            }

            if (state != PlayModeStateChange.EnteredPlayMode || !SessionState.GetBool(PendingRunKey, false))
            {
                return;
            }

            SessionState.SetBool(PendingRunKey, false);
            EnsureMainCameraRuntime();
            var launcher = EnsureRuntimeLauncher();
            launcher.ConfigureForQuickTest(
                newClientId: 1,
                newTargetPercent: 45,
                newTutorialMode: false,
                newBaselineSatisfaction: 3f,
                newBaselineSignMistake: 0,
                newMaxRoundSeconds: 120f,
                newAllowHotkeyReopen: true,
                newReopenKey: KeyCode.Y,
                newOpenOnStart: true);
            Debug.Log("[灵魂收取快测] 运行时临时启动器已准备，自动开局中。");
        }

        private static void EnsureMainCameraRuntime()
        {
            var camera = Object.FindAnyObjectByType<Camera>();
            if (camera != null)
            {
                return;
            }

            var cameraObj = new GameObject("Main Camera (SoulCollect QuickTest)", typeof(Camera), typeof(AudioListener));
            var createdCamera = cameraObj.GetComponent<Camera>();
            if (createdCamera != null)
            {
                createdCamera.clearFlags = CameraClearFlags.SolidColor;
                createdCamera.backgroundColor = new Color(0.08f, 0.08f, 0.08f, 1f);
            }
        }

        private static SoulCollectQuickTestLauncher EnsureRuntimeLauncher()
        {
            var launcher = Object.FindAnyObjectByType<SoulCollectQuickTestLauncher>();
            if (launcher != null)
            {
                return launcher;
            }

            var launcherObj = new GameObject("SoulCollectQuickTestLauncher_Runtime");
            return launcherObj.AddComponent<SoulCollectQuickTestLauncher>();
        }
    }
}
#endif
