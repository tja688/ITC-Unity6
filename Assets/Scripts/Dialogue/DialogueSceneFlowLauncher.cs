using System.Collections;
using UnityEngine;
using Yarn.Unity;

[DisallowMultipleComponent]
public sealed class DialogueSceneFlowLauncher : MonoBehaviour
{
    [Header("Dialogue Startup")]
    [SerializeField] private bool autoOpenDialogueOnStart = true;
    [SerializeField] private float startupDelaySeconds = 0.2f;
    [SerializeField] private string startNode = "ITC_Start";
    [SerializeField] private DialogueRunner dialogueRunner;

    private bool bootstrapped;

    private IEnumerator Start()
    {
        if (bootstrapped)
        {
            yield break;
        }

        bootstrapped = true;

        dialogueRunner ??= FindFirstObjectByType<DialogueRunner>(FindObjectsInactive.Include);
        if (dialogueRunner == null)
        {
            yield break;
        }

        dialogueRunner.autoStart = false;

        if (!autoOpenDialogueOnStart)
        {
            yield break;
        }

        yield return new WaitForSecondsRealtime(Mathf.Max(0f, startupDelaySeconds));
        if (dialogueRunner.IsDialogueRunning)
        {
            yield break;
        }

        _ = dialogueRunner.StartDialogue(string.IsNullOrWhiteSpace(startNode) ? "ITC_Start" : startNode.Trim());
    }
}
