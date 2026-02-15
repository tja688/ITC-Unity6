using QFramework;
using ITC.Dialogue;

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

        UIKit.OpenPanelAsync<MainMenuPanel>(
                UILevel.Common,
                assetBundleName: "menu_core",
                prefabName: nameof(MainMenuPanel))
            .ToAction()
            .StartGlobal(() => this.SendCommand<MarkMainMenuPanelOpenedCommand>());
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
    protected override void OnExecute()
    {
        var model = this.GetModel<MainMenuStateModel>();
        if (!model.ResKitReady.Value || model.DialoguePanelOpenRequested.Value)
        {
            return;
        }

        model.DialoguePanelOpenRequested.Value = true;

        UIKit.OpenPanelAsync<ITCDialoguePanel>(
                UILevel.Common,
                new ITCDialoguePanelData
                {
                    StartNode = "ITC_Start",
                    AutoStartOnOpen = true
                },
                assetBundleName: "dialogue_ui",
                prefabName: "ITC DialogueSystem")
            .ToAction()
            .StartGlobal(() =>
            {
                UIKit.ClosePanel<MainMenuPanel>();
                this.SendCommand<MarkDialoguePanelOpenedCommand>();
            });
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
