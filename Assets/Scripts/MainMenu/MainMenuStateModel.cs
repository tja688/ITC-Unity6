using QFramework;

public sealed class MainMenuStateModel : AbstractModel
{
    public readonly BindableProperty<bool> ResKitReady = new(false);
    public readonly BindableProperty<bool> PanelOpenRequested = new(false);
    public readonly BindableProperty<bool> PanelOpened = new(false);

    protected override void OnInit()
    {
    }
}
