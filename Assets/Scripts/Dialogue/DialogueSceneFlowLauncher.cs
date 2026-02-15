using System.Collections;
using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

namespace ITC.Dialogue
{
    [DefaultExecutionOrder(-300)]
    [DisallowMultipleComponent]
    public sealed class DialogueSceneFlowLauncher : MonoBehaviour, IController
    {
        private bool bootstrapped;
        private GameObject runtimeDialogueSystem;
        private DialogueRunner runtimeDialogueRunner;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void AutoBootstrap()
        {
            var activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || activeScene.name != ITCDialogueResPaths.DialogueSceneName)
            {
                return;
            }

            if (Object.FindFirstObjectByType<DialogueSceneFlowLauncher>() != null)
            {
                return;
            }

            var launcherGo = new GameObject(nameof(DialogueSceneFlowLauncher));
            launcherGo.AddComponent<DialogueSceneFlowLauncher>();
            LogKit.I("[DialogueSceneFlowLauncher] Auto-bootstrap GameObject created.");
        }

        private IEnumerator Start()
        {
            if (bootstrapped)
            {
                yield break;
            }

            bootstrapped = true;

            var activeScene = SceneManager.GetActiveScene();
            if (!activeScene.IsValid() || activeScene.name != ITCDialogueResPaths.DialogueSceneName)
            {
                LogKit.W("[DialogueSceneFlowLauncher] Start skipped because active scene is not DialogueScene.");
                yield break;
            }

            LogKit.I("[DialogueSceneFlowLauncher] Bootstrapping DialogueScene runtime.");

            yield return ResKit.InitAsync();
            this.SendCommand<MarkDialogueResReadyCommand>();
            LogKit.I("[DialogueSceneFlowLauncher] ResKit.InitAsync completed.");

            var loader = ResLoader.Allocate();
            var dialoguePrefab = TryLoadDialogueSystemPrefab(loader, out var usedFallback);
            if (dialoguePrefab == null)
            {
                LogKit.E("[DialogueSceneFlowLauncher] DialogueSystem prefab load failed.");
                loader.ReleaseAllRes();
                loader.Recycle2Cache();
                yield break;
            }

            runtimeDialogueSystem = Instantiate(dialoguePrefab);
            runtimeDialogueSystem.name = "ITC DialogueSystem(Runtime)";
            runtimeDialogueSystem.SetActive(false);
            this.SendCommand(new MarkDialogueSystemSpawnedCommand(usedFallback));

            LogKit.I(
                $"[DialogueSceneFlowLauncher] DialogueSystem instantiated. fallbackUsed={usedFallback}.");

            runtimeDialogueRunner = runtimeDialogueSystem.GetComponentInChildren<DialogueRunner>(true);
            if (runtimeDialogueRunner == null)
            {
                LogKit.E("[DialogueSceneFlowLauncher] DialogueRunner component was not found.");
                loader.ReleaseAllRes();
                loader.Recycle2Cache();
                yield break;
            }

            runtimeDialogueRunner.onDialogueStart.AddListener(OnDialogueStarted);

            var visualRuntime = runtimeDialogueSystem.GetComponent<ITCDialogueVisualRuntime>();
            if (visualRuntime == null)
            {
                visualRuntime = runtimeDialogueSystem.AddComponent<ITCDialogueVisualRuntime>();
            }

            visualRuntime.Initialize(runtimeDialogueRunner);

            runtimeDialogueSystem.SetActive(true);
            LogKit.I("[DialogueSceneFlowLauncher] DialogueSystem activated.");

            loader.ReleaseAllRes();
            loader.Recycle2Cache();
        }

        public IArchitecture GetArchitecture()
        {
            return DialogueSceneApp.Interface;
        }

        private static GameObject TryLoadDialogueSystemPrefab(ResLoader loader, out bool usedFallback)
        {
            usedFallback = false;
            GameObject prefab = null;

            if (ITCDialogueResPaths.HasAssetEntry(
                    ITCDialogueResPaths.DialogueSystemBundle,
                    ITCDialogueResPaths.DialogueSystemPrefabName))
            {
                prefab = loader.LoadSync<GameObject>(
                    ITCDialogueResPaths.DialogueSystemBundle,
                    ITCDialogueResPaths.DialogueSystemPrefabName);
            }

            if (prefab != null)
            {
                return prefab;
            }

            usedFallback = true;
            LogKit.W(
                $"[DialogueSceneFlowLauncher] Bundle entry missing for prefab '{ITCDialogueResPaths.DialogueSystemPrefabName}', fallback to ownerless lookup.");

            if (ITCDialogueResPaths.HasAssetEntry(string.Empty, ITCDialogueResPaths.DialogueSystemPrefabName))
            {
                prefab = loader.LoadSync<GameObject>(ITCDialogueResPaths.DialogueSystemPrefabName);
            }

            return prefab;
        }

        private void OnDialogueStarted()
        {
            this.SendCommand<MarkDialogueRunningCommand>();
            LogKit.I("[DialogueSceneFlowLauncher] DialogueRunner started.");
        }

        private void OnDestroy()
        {
            if (runtimeDialogueRunner != null)
            {
                runtimeDialogueRunner.onDialogueStart.RemoveListener(OnDialogueStarted);
                runtimeDialogueRunner = null;
            }
        }
    }
}
