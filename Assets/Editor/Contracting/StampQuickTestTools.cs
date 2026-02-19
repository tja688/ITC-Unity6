#if UNITY_EDITOR
using ITC.Contracting;
using UnityEditor;
using UnityEngine;

namespace ITC.EditorTools.Contracting
{
    [InitializeOnLoad]
    public static class StampQuickTestTools
    {
        private const string PendingRunKey = "ITC.StampQuickTestTools.PendingRun";

        static StampQuickTestTools()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [MenuItem("ITC/Contracting/一键运行 04印章盖印全流程测试")]
        public static void RunStampQuickTest()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("[印章盖印快测] 当前已在运行中。");
                return;
            }

            SessionState.SetBool(PendingRunKey, true);
            Debug.Log("[印章盖印快测] 已请求启动，进入 Play 后将自动创建临时启动器并跑完整流程。");
            EditorApplication.isPlaying = true;
        }

        [MenuItem("ITC/Contracting/一键停止 04印章盖印全流程测试")]
        public static void StopStampQuickTest()
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
                newTutorialMode: false,
                newHasVerifyDebuff: false,
                newBaselineSatisfaction: 3f,
                newBaselineSignMistake: 0,
                newMaxRoundSeconds: 120f,
                newAllowHotkeyReopen: true,
                newReopenKey: KeyCode.T,
                newOpenOnStart: true);
            Debug.Log("[印章盖印快测] 运行时临时启动器已准备，自动开局中。");
        }

        private static void EnsureMainCameraRuntime()
        {
            var camera = Object.FindAnyObjectByType<Camera>();
            if (camera != null)
            {
                return;
            }

            var cameraObj = new GameObject("Main Camera (StampQuickTest)", typeof(Camera), typeof(AudioListener));
            var createdCamera = cameraObj.GetComponent<Camera>();
            if (createdCamera != null)
            {
                createdCamera.clearFlags = CameraClearFlags.SolidColor;
                createdCamera.backgroundColor = new Color(0.08f, 0.08f, 0.08f, 1f);
            }
        }

        private static StampQuickTestLauncher EnsureRuntimeLauncher()
        {
            var launcher = Object.FindAnyObjectByType<StampQuickTestLauncher>();
            if (launcher != null)
            {
                return launcher;
            }

            var launcherObj = new GameObject("StampQuickTestLauncher_Runtime");
            return launcherObj.AddComponent<StampQuickTestLauncher>();
        }
    }
}
#endif

