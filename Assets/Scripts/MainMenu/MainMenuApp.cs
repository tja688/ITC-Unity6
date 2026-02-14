using QFramework;

public sealed class MainMenuApp : Architecture<MainMenuApp>
{
    protected override void Init()
    {
        RegisterModel(new MainMenuStateModel());
    }
}
