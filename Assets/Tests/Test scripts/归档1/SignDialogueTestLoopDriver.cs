using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

namespace ITC.Dialogue.Testing
{
    [DisallowMultipleComponent]
    public sealed class SignDialogueTestLoopDriver : MonoBehaviour
    {
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private string startNode = "Start";
        [SerializeField] private float initialStartDelaySeconds = 0.2f;
        [SerializeField] private bool autoRestartOnComplete = true;
        [SerializeField] private float restartDelaySeconds = 0.6f;
        [SerializeField] private bool allowManualRestart = true;
        [SerializeField] private Key manualRestartKey = Key.R;

        private bool runtimeInitialized;
        private bool wasDialogueRunning;
        private bool pendingAutoStart;
        private float nextAutoStartTime;
        private float startCooldownUntil;

        private void Awake()
        {
            if (dialogueRunner == null)
            {
                dialogueRunner = GetComponent<DialogueRunner>();
            }

            if (dialogueRunner == null)
            {
                dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            }
        }

        private void InitializeRuntimeState()
        {
            runtimeInitialized = true;
            wasDialogueRunning = dialogueRunner != null && dialogueRunner.IsDialogueRunning;
            startCooldownUntil = 0f;
            ScheduleAutoStart(initialStartDelaySeconds);
        }

        private void OnEnable()
        {
            if (dialogueRunner != null)
            {
                dialogueRunner.onDialogueComplete.AddListener(HandleDialogueComplete);
            }

            runtimeInitialized = false;
        }

        private void OnDisable()
        {
            if (dialogueRunner != null)
            {
                dialogueRunner.onDialogueComplete.RemoveListener(HandleDialogueComplete);
            }

            runtimeInitialized = false;
            pendingAutoStart = false;
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (!runtimeInitialized)
            {
                if (dialogueRunner == null)
                {
                    dialogueRunner = GetComponent<DialogueRunner>() ?? FindFirstObjectByType<DialogueRunner>();
                }

                InitializeRuntimeState();
            }

            if (dialogueRunner == null)
            {
                return;
            }

            var isRunning = dialogueRunner.IsDialogueRunning;
            if (isRunning)
            {
                wasDialogueRunning = true;
                pendingAutoStart = false;
            }
            else
            {
                if (wasDialogueRunning && autoRestartOnComplete)
                {
                    ScheduleAutoStart(restartDelaySeconds);
                }

                wasDialogueRunning = false;

                if (pendingAutoStart && Time.unscaledTime >= nextAutoStartTime)
                {
                    pendingAutoStart = false;
                    StartDialogue();
                }
            }

            if (!allowManualRestart || isRunning)
            {
                return;
            }

            if (Keyboard.current != null && Keyboard.current[manualRestartKey].wasPressedThisFrame)
            {
                StartDialogue(force: true);
            }
        }

        private void HandleDialogueComplete()
        {
            if (!autoRestartOnComplete || dialogueRunner == null || !isActiveAndEnabled)
            {
                return;
            }

            ScheduleAutoStart(restartDelaySeconds);
        }

        private void ScheduleAutoStart(float delaySeconds)
        {
            if (!autoRestartOnComplete)
            {
                pendingAutoStart = false;
                return;
            }

            pendingAutoStart = true;
            nextAutoStartTime = Time.unscaledTime + Mathf.Max(0f, delaySeconds);
        }

        private void StartDialogue(bool force = false)
        {
            if (dialogueRunner == null || dialogueRunner.IsDialogueRunning)
            {
                return;
            }

            if (!force && Time.unscaledTime < startCooldownUntil)
            {
                return;
            }

            var node = string.IsNullOrWhiteSpace(startNode) ? "Start" : startNode.Trim();
            _ = dialogueRunner.StartDialogue(node);
            startCooldownUntil = Time.unscaledTime + 0.2f;
        }
    }
}
