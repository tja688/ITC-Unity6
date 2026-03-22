using QFramework;
using ITC.SignMiniGame;

public sealed class MainMenuApp : Architecture<MainMenuApp>
{
    protected override void Init()
    {
        RegisterModel(new MainMenuStateModel());
        RegisterModel(new SignMiniGameFlowStateModel());
        RegisterModel(new SignMiniGameClientConfigModel());
    }
}
