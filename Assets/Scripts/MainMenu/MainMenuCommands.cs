using QFramework;

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
