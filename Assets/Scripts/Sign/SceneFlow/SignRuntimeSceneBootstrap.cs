using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ITC.Sign
{
    public sealed class SignRuntimeSceneBootstrap : MonoBehaviour
    {
        private const string BootstrapObjectName = "[SignRuntimeSceneBootstrap]";
        private const string BaseSceneName = "SignScene";
        private const string DialogueSceneName = "DialogueScene";
        private const string MiniGameOverlaySceneName = "SignMiniGameOverlayScene";

        private static SignRuntimeSceneBootstrap instance;

        private Coroutine ensureRoutine;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            EnsureInstance();
            SceneManager.sceneLoaded -= HandleSceneLoaded;
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (scene.name != BaseSceneName && scene.name != DialogueSceneName)
            {
                return;
            }

            EnsureInstance().QueueEnsureScenesLoaded();
        }

        private static SignRuntimeSceneBootstrap EnsureInstance()
        {
            if (instance != null)
            {
                return instance;
            }

            var bootstrapObject = new GameObject(BootstrapObjectName);
            DontDestroyOnLoad(bootstrapObject);
            instance = bootstrapObject.AddComponent<SignRuntimeSceneBootstrap>();
            return instance;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            QueueEnsureScenesLoaded();
        }

        private void QueueEnsureScenesLoaded()
        {
            if (!Application.isPlaying || ensureRoutine != null)
            {
                return;
            }

            ensureRoutine = StartCoroutine(EnsureScenesLoadedRoutine());
        }

        private IEnumerator EnsureScenesLoadedRoutine()
        {
            yield return null;

            if (!SignSceneOverlayLoader.IsSceneLoaded(BaseSceneName))
            {
                ensureRoutine = null;
                yield break;
            }

            yield return EnsureSceneLoaded(DialogueSceneName);
            yield return EnsureSceneLoaded(MiniGameOverlaySceneName);

            var baseScene = SceneManager.GetSceneByName(BaseSceneName);
            if (baseScene.IsValid() && baseScene.isLoaded)
            {
                SceneManager.SetActiveScene(baseScene);
            }

            ensureRoutine = null;
        }

        private static IEnumerator EnsureSceneLoaded(string sceneReference)
        {
            if (SignSceneOverlayLoader.IsSceneLoaded(sceneReference))
            {
                yield break;
            }

            var asyncOp = SignSceneOverlayLoader.LoadAdditiveScene(sceneReference);
            if (asyncOp == null)
            {
                Debug.LogWarning($"[SignRuntimeSceneBootstrap] Failed to resolve additive scene '{sceneReference}'.");
                yield break;
            }

            while (!asyncOp.isDone)
            {
                yield return null;
            }
        }
    }
}
