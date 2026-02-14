using System.Collections;
using QFramework;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MainMenuFlowLauncher : MonoBehaviour, IController
{
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
    }

    public IArchitecture GetArchitecture()
    {
        return MainMenuApp.Interface;
    }
}
