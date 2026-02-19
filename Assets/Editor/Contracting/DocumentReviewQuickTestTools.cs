#if UNITY_EDITOR
using ITC.Contracting;
using UnityEditor;
using UnityEngine;

namespace ITC.EditorTools.Contracting
{
    [InitializeOnLoad]
    public static class DocumentReviewQuickTestTools
    {
        private const string PendingRunKey = "ITC.DocumentReviewQuickTestTools.PendingRun";

        static DocumentReviewQuickTestTools()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        [MenuItem("ITC/Contracting/一键运行 01文书审核全流程测试")]
        public static void RunDocumentReviewQuickTest()
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogWarning("[文书审核快测] 当前已在运行中。");
                return;
            }

            SessionState.SetBool(PendingRunKey, true);
            Debug.Log("[文书审核快测] 已请求启动，进入 Play 后将自动创建临时启动器并跑完整流程。");
            EditorApplication.isPlaying = true;
        }

        [MenuItem("ITC/Contracting/一键停止 01文书审核全流程测试")]
        public static void StopDocumentReviewQuickTest()
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
                newBaselineSatisfaction: 3f,
                newBaselineSignMistake: 0,
                newMaxRoundSeconds: 180f,
                newAllowHotkeyReopen: true,
                newReopenKey: KeyCode.R,
                newOpenOnStart: true);
            Debug.Log("[文书审核快测] 运行时临时启动器已准备，自动开局中。");
        }

        private static void EnsureMainCameraRuntime()
        {
            var camera = Object.FindAnyObjectByType<Camera>();
            if (camera != null)
            {
                return;
            }

            var cameraObj = new GameObject("Main Camera (QuickTest)", typeof(Camera), typeof(AudioListener));
            var createdCamera = cameraObj.GetComponent<Camera>();
            if (createdCamera != null)
            {
                createdCamera.clearFlags = CameraClearFlags.SolidColor;
                createdCamera.backgroundColor = new Color(0.08f, 0.08f, 0.08f, 1f);
            }
        }

        private static DocumentReviewQuickTestLauncher EnsureRuntimeLauncher()
        {
            var launcher = Object.FindAnyObjectByType<DocumentReviewQuickTestLauncher>();
            if (launcher != null)
            {
                return launcher;
            }

            var launcherObj = new GameObject("DocumentReviewQuickTestLauncher_Runtime");
            return launcherObj.AddComponent<DocumentReviewQuickTestLauncher>();
        }
    }
}
#endif
