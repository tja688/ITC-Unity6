using System.Collections;
using QFramework;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MainMenuFlowLauncher : MonoBehaviour, IController
{
    [Header("Dialogue Demo Flow")]
    [SerializeField] private bool autoRunDialogueDemoFlow = false;
    [SerializeField] private float autoRunDelaySeconds = 0.8f;

    private bool bootstrapped;

    private IEnumerator Start()
    {
        if (bootstrapped)
        {
            yield break;
        }

        bootstrapped = true;
        this.SendCommand<MarkMainMenuResReadyCommand>();
        this.SendCommand<OpenMainMenuPanelCommand>();

        if (autoRunDialogueDemoFlow)
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, autoRunDelaySeconds));
            this.SendCommand<RequestOpenDialoguePanelCommand>();
        }
    }

    public IArchitecture GetArchitecture()
    {
        return MainMenuApp.Interface;
    }
}
