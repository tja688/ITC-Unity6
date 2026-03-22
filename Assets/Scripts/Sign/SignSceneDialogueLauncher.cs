using System.Collections;
using QFramework;
using UnityEngine;
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

        private bool startupRequestedThisSession;

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

        private IEnumerator Start()
        {
            if (!autoStartOnEnable)
            {
                yield break;
            }

            startupRequestedThisSession = true;
            yield return StartDialogueRoutine();
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
            }

            if (startupRequestedThisSession || Time.frameCount > 10)
            {
                return;
            }

            startupRequestedThisSession = true;
            StartCoroutine(StartDialogueRoutine());
        }

        public IEnumerator StartDialogueRoutine()
        {
            if (dialogueRunner == null)
            {
                LogKit.E("[SignSceneDialogueLauncher] DialogueRunner is missing.");
                yield break;
            }

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
            }
        }
    }
}
