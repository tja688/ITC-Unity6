using System.Collections;
using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

namespace ITC.Sign
{
    [DisallowMultipleComponent]
    public sealed class SignSceneDialogueLauncher : MonoBehaviour
    {
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private YarnProject yarnProject;
        [SerializeField] private bool autoStartOnEnable = true;
        [SerializeField] private float startupDelaySeconds = 0.2f;
        [SerializeField] private string startNode = "Sign_Day1_Start";
        [SerializeField] private float startupRetryWindowSeconds = 5f;
        [SerializeField] private string[] requiredOverlaySceneNames =
        {
            "SignMiniGameOverlayScene"
        };

        private bool startupRequestedThisSession;
        private float sessionStartRealtime;
        private Coroutine startupRoutine;

        public void Configure(DialogueRunner runner, YarnProject project, bool autoStart, float delaySeconds, string nodeName)
        {
            dialogueRunner = runner;
            yarnProject = project;
            autoStartOnEnable = autoStart;
            startupDelaySeconds = delaySeconds;
            startNode = nodeName;
        }

        private void Awake()
        {
            if (dialogueRunner == null)
            {
                dialogueRunner = GetComponentInChildren<DialogueRunner>(true);
            }

            if (dialogueRunner != null)
            {
                dialogueRunner.autoStart = false;
            }
        }

        private void OnEnable()
        {
            startupRequestedThisSession = false;
            sessionStartRealtime = Time.realtimeSinceStartup;
        }

        private void OnDisable()
        {
            startupRequestedThisSession = false;
            sessionStartRealtime = 0f;

            if (startupRoutine != null)
            {
                StopCoroutine(startupRoutine);
                startupRoutine = null;
            }
        }

        private IEnumerator Start()
        {
            if (!autoStartOnEnable)
            {
                yield break;
            }

            if (startupRoutine == null)
            {
                startupRoutine = StartCoroutine(StartDialogueRoutine());
            }

            yield return startupRoutine;
        }

        private void Update()
        {
            if (!Application.isPlaying || !autoStartOnEnable)
            {
                return;
            }

            // Scene/domain reload may be disabled in the editor. Use the first frames of each play
            // session to re-arm a single startup attempt so editor testing matches build behavior.
            if (Time.frameCount <= 1)
            {
                startupRequestedThisSession = false;
                sessionStartRealtime = Time.realtimeSinceStartup;
            }

            if (dialogueRunner == null || dialogueRunner.IsDialogueRunning)
            {
                return;
            }

            if (startupRoutine != null)
            {
                return;
            }

            if (Time.realtimeSinceStartup - sessionStartRealtime > startupRetryWindowSeconds)
            {
                return;
            }

            if (!startupRequestedThisSession || Time.frameCount <= 10)
            {
                startupRoutine = StartCoroutine(StartDialogueRoutine());
            }
        }

        public IEnumerator StartDialogueRoutine()
        {
            startupRequestedThisSession = true;

            if (dialogueRunner == null)
            {
                LogKit.E("[SignSceneDialogueLauncher] DialogueRunner is missing.");
                startupRoutine = null;
                yield break;
            }

            if (dialogueRunner.IsDialogueRunning)
            {
                startupRoutine = null;
                yield break;
            }

            yield return EnsureRequiredOverlayScenesLoaded();

            if (yarnProject != null)
            {
                dialogueRunner.SetProject(yarnProject);
            }
            else
            {
                LogKit.W("[SignSceneDialogueLauncher] YarnProject is not assigned. Runner will use its existing project.");
            }

            dialogueRunner.startNode = string.IsNullOrWhiteSpace(startNode) ? "Sign_Day1_Start" : startNode.Trim();

            if (startupDelaySeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(startupDelaySeconds);
            }

            if (!dialogueRunner.IsDialogueRunning)
            {
                _ = dialogueRunner.StartDialogue(dialogueRunner.startNode);
                yield return null;
            }

            if (!dialogueRunner.IsDialogueRunning)
            {
                startupRequestedThisSession = false;
            }

            startupRoutine = null;
        }

        private IEnumerator EnsureRequiredOverlayScenesLoaded()
        {
            if (requiredOverlaySceneNames == null || requiredOverlaySceneNames.Length == 0)
            {
                yield break;
            }

            foreach (var sceneReference in requiredOverlaySceneNames)
            {
                var trimmedReference = sceneReference?.Trim();
                if (string.IsNullOrWhiteSpace(trimmedReference) || SignSceneOverlayLoader.IsSceneLoaded(trimmedReference))
                {
                    continue;
                }

                var asyncOp = SignSceneOverlayLoader.LoadAdditiveScene(trimmedReference);
                if (asyncOp == null)
                {
                    LogKit.W($"[SignSceneDialogueLauncher] Failed to resolve overlay scene '{trimmedReference}'.");
                    continue;
                }

                while (!asyncOp.isDone)
                {
                    yield return null;
                }
            }

            if (gameObject.scene.IsValid())
            {
                SceneManager.SetActiveScene(gameObject.scene);
            }
        }
    }
}
