using QFramework;
using ITC.Contracting;
using ITC.Dialogue.Sign;

public sealed class MainMenuApp : Architecture<MainMenuApp>
{
    protected override void Init()
    {
        RegisterModel(new MainMenuStateModel());
        RegisterModel(new ContractFlowStateModel());
        RegisterModel(new ContractClientConfigModel());
        RegisterModel(new SignScrollCardRuntimeModel());
    }
}
