using QFramework;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MarkMainMenuResReadyCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<MainMenuStateModel>();
        model.ResKitReady.Value = true;
    }
}

public sealed class OpenMainMenuPanelCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<MainMenuStateModel>();
        if (!model.ResKitReady.Value || model.PanelOpenRequested.Value)
        {
            return;
        }

        model.PanelOpenRequested.Value = true;
        var panel = Object.FindFirstObjectByType<MainMenuPanel>(FindObjectsInactive.Include);
        if (panel == null)
        {
            LogKit.E("[MainMenuCommands] MainMenuPanel is missing from scene.");
            return;
        }

        panel.gameObject.SetActive(true);
        this.SendCommand<MarkMainMenuPanelOpenedCommand>();
    }
}

public sealed class MarkMainMenuPanelOpenedCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<MainMenuStateModel>();
        model.PanelOpened.Value = true;
    }
}

public sealed class RequestOpenDialoguePanelCommand : AbstractCommand
{
    private const string TargetGameplaySceneName = "SignScene";

    protected override void OnExecute()
    {
        var model = this.GetModel<MainMenuStateModel>();
        if (!model.ResKitReady.Value || model.DialoguePanelOpenRequested.Value)
        {
            return;
        }

        model.DialoguePanelOpenRequested.Value = true;
        SceneManager.LoadScene(TargetGameplaySceneName, LoadSceneMode.Single);
        this.SendCommand<MarkDialoguePanelOpenedCommand>();
    }
}

public sealed class MarkDialoguePanelOpenedCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<MainMenuStateModel>();
        model.DialoguePanelOpened.Value = true;
    }
}

public sealed class MarkDialoguePanelClosedCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<MainMenuStateModel>();
        model.DialoguePanelOpenRequested.Value = false;
        model.DialoguePanelOpened.Value = false;
    }
}
