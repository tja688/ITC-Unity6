using System.Collections;
using QFramework;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MainMenuFlowLauncher : MonoBehaviour, IController
{
    [Header("Dialogue Demo Flow")]
    [SerializeField] private bool autoRunDialogueDemoFlow = false;
    [SerializeField] private float autoRunDelaySeconds = 0.8f;
    [SerializeField] private float waitPanelOpenTimeoutSeconds = 8f;

    private bool bootstrapped;

    private void Awake()
    {
        UIKit.Config = new MainMenuUIKitConfig();
    }

    private IEnumerator Start()
    {
        if (bootstrapped)
        {
            yield break;
        }

        bootstrapped = true;

        yield return ResKit.InitAsync();

        this.SendCommand<MarkMainMenuResReadyCommand>();
        this.SendCommand<OpenMainMenuPanelCommand>();

        if (autoRunDialogueDemoFlow)
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0f, autoRunDelaySeconds));
            yield return WaitMainMenuPanelOpened();
            this.SendCommand<RequestOpenDialoguePanelCommand>();
        }
    }

    private IEnumerator WaitMainMenuPanelOpened()
    {
        var model = this.GetModel<MainMenuStateModel>();
        var timeout = Mathf.Max(0.5f, waitPanelOpenTimeoutSeconds);
        var elapsed = 0f;
        while (!model.PanelOpened.Value && elapsed < timeout)
        {
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    public IArchitecture GetArchitecture()
    {
        return MainMenuApp.Interface;
    }
}
