using QFramework;

public sealed class MainMenuStateModel : AbstractModel
{
    public readonly BindableProperty<bool> ResKitReady = new(false);
    public readonly BindableProperty<bool> PanelOpenRequested = new(false);
    public readonly BindableProperty<bool> PanelOpened = new(false);
    public readonly BindableProperty<bool> SceneTransitionInProgress = new(false);
    public readonly BindableProperty<string> SceneTransitionTarget = new(string.Empty);
    public readonly BindableProperty<bool> SceneTransitionFallbackUsed = new(false);

    protected override void OnInit()
    {
    }
}
