using QFramework;
using UnityEngine.SceneManagement;
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

public sealed class RequestEnterDialogueSceneCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        var model = this.GetModel<MainMenuStateModel>();

        if (!model.ResKitReady.Value)
        {
            LogKit.W("[MainMenu] Scene transition rejected because ResKit is not ready.");
            return;
        }

        if (model.SceneTransitionInProgress.Value)
        {
            LogKit.W("[MainMenu] Scene transition rejected because another transition is in progress.");
            return;
        }

        model.SceneTransitionInProgress.Value = true;
        model.SceneTransitionFallbackUsed.Value = false;
        model.SceneTransitionTarget.Value = ITCDialogueResPaths.DialogueSceneName;

        var resLoader = ResLoader.Allocate();
        var canUseResKitScenePath = ITCDialogueResPaths.HasSceneAssetEntry(
            ITCDialogueResPaths.DialogueSceneBundle,
            ITCDialogueResPaths.DialogueSceneName);

        if (canUseResKitScenePath)
        {
            LogKit.I(
                $"[MainMenu] Loading scene via ResKit: bundle={ITCDialogueResPaths.DialogueSceneBundle}, scene={ITCDialogueResPaths.DialogueSceneName}.");

            resLoader.LoadSceneAsync(
                ITCDialogueResPaths.DialogueSceneBundle,
                ITCDialogueResPaths.DialogueSceneName,
                LoadSceneMode.Single,
                LocalPhysicsMode.None,
                operation =>
                {
                    if (operation == null)
                    {
                        LogKit.E("[MainMenu] ResKit scene load returned null AsyncOperation.");
                        model.SceneTransitionInProgress.Value = false;
                        resLoader.ReleaseAllRes();
                        resLoader.Recycle2Cache();
                        return;
                    }

                    operation.completed += _ =>
                    {
                        LogKit.I("[MainMenu] DialogueScene loaded via ResKit.");
                        resLoader.ReleaseAllRes();
                        resLoader.Recycle2Cache();
                    };
                });

            return;
        }

        model.SceneTransitionFallbackUsed.Value = true;
        LogKit.W(
            $"[MainMenu] ResKit scene entry not found for '{ITCDialogueResPaths.DialogueSceneName}', fallback to SceneManager.");

        var fallbackOperation = SceneManager.LoadSceneAsync(ITCDialogueResPaths.DialogueSceneName, LoadSceneMode.Single);
        if (fallbackOperation == null)
        {
            LogKit.E("[MainMenu] SceneManager fallback returned null AsyncOperation.");
            model.SceneTransitionInProgress.Value = false;
            resLoader.ReleaseAllRes();
            resLoader.Recycle2Cache();
            return;
        }

        fallbackOperation.completed += _ =>
        {
            LogKit.I("[MainMenu] DialogueScene loaded via SceneManager fallback.");
            resLoader.ReleaseAllRes();
            resLoader.Recycle2Cache();
        };
    }
}
