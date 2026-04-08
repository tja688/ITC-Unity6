using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ITC.Sign
{
    [DefaultExecutionOrder(-900)]
    [DisallowMultipleComponent]
    public sealed class SignSceneOverlayLoader : MonoBehaviour
    {
        [Header("Overlay Scenes")]
        [SerializeField] private bool loadOnStart = true;
        [SerializeField] private float startupDelaySeconds = 0.1f;
        [SerializeField] private string[] additiveSceneNames =
        {
            "DialogueScene",
            "SignMiniGameOverlayScene"
        };

        [SerializeField] private bool setBaseSceneActiveAfterLoad = true;

        private Coroutine loadRoutine;
        private bool loadingRequested;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void RegisterRuntimeHooks()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!Application.isPlaying || !scene.IsValid() || !scene.isLoaded)
            {
                return;
            }

            foreach (var root in scene.GetRootGameObjects())
            {
                if (root == null)
                {
                    continue;
                }

                var loaders = root.GetComponentsInChildren<SignSceneOverlayLoader>(true);
                foreach (var loader in loaders)
                {
                    loader.TryBeginLoad();
                }
            }
        }

        private void Awake()
        {
            TryBeginLoad();
        }

        private void OnEnable()
        {
            TryBeginLoad();
        }

        private IEnumerator Start()
        {
            TryBeginLoad();
            yield break;
        }

        public IEnumerator LoadOverlayScenesRoutine()
        {
            if (loadingRequested)
            {
                yield break;
            }

            loadingRequested = true;

            if (startupDelaySeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(startupDelaySeconds);
            }

            var baseScene = gameObject.scene;
            foreach (var sceneName in additiveSceneNames)
            {
                var sceneReference = sceneName?.Trim();
                if (string.IsNullOrWhiteSpace(sceneReference) || IsSceneLoaded(sceneReference))
                {
                    continue;
                }

                var asyncOp = LoadAdditiveScene(sceneReference);
                if (asyncOp == null)
                {
                    Debug.LogWarning($"[SignSceneOverlayLoader] Failed to resolve additive scene '{sceneReference}'.");
                    continue;
                }

                while (!asyncOp.isDone)
                {
                    yield return null;
                }
            }

            if (setBaseSceneActiveAfterLoad && baseScene.IsValid())
            {
                SceneManager.SetActiveScene(baseScene);
            }

            loadRoutine = null;
        }

        internal static bool IsSceneLoaded(string sceneName)
        {
            var safeSceneName = sceneName?.Trim();
            if (string.IsNullOrEmpty(safeSceneName))
            {
                return false;
            }

            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded)
                {
                    continue;
                }

                if (scene.name == safeSceneName || scene.path == safeSceneName)
                {
                    return true;
                }

                if (safeSceneName.EndsWith(".unity") && scene.name == Path.GetFileNameWithoutExtension(safeSceneName))
                {
                    return true;
                }
            }

            return false;
        }

        private void TryBeginLoad()
        {
            if (!Application.isPlaying || !isActiveAndEnabled || !loadOnStart || loadRoutine != null || loadingRequested)
            {
                return;
            }

            loadRoutine = StartCoroutine(LoadOverlayScenesRoutine());
        }

        internal static AsyncOperation LoadAdditiveScene(string sceneReference)
        {
            var buildIndex = ResolveBuildIndex(sceneReference);
            if (buildIndex >= 0)
            {
                return SceneManager.LoadSceneAsync(buildIndex, LoadSceneMode.Additive);
            }

            return SceneManager.LoadSceneAsync(sceneReference, LoadSceneMode.Additive);
        }

        private static int ResolveBuildIndex(string sceneReference)
        {
            if (string.IsNullOrWhiteSpace(sceneReference))
            {
                return -1;
            }

            var trimmed = sceneReference.Trim();
            if (trimmed.EndsWith(".unity"))
            {
                var directIndex = SceneUtility.GetBuildIndexByScenePath(trimmed);
                if (directIndex >= 0)
                {
                    return directIndex;
                }
            }

            var assetsScenePath = $"Assets/Scenes/{trimmed}";
            if (!assetsScenePath.EndsWith(".unity"))
            {
                assetsScenePath += ".unity";
            }

            return SceneUtility.GetBuildIndexByScenePath(assetsScenePath);
        }
    }
}
