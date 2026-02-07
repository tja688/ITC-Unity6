# Code Templates

Use these as copy-ready starters and adapt naming to feature context.

## 1) Root Architecture (`GameRootApp` style)

```csharp
using QFramework;
public sealed class GameRootApp : Architecture<GameRootApp>
{
    protected override void Init()
    {
        this.RegisterModel<IGameModel>(new GameModel());
        this.RegisterSystem<IGameSystem>(new GameSystem());
        this.RegisterUtility<IGameStorage>(new GameStorage());
    }
}
```

## 2) Controller Entry

```csharp
using QFramework;
using UnityEngine;
public sealed class GamePanelController : MonoBehaviour, IController
{
    public IArchitecture GetArchitecture() => GameRootApp.Interface;
    private IGameModel mGameModel;
    private void OnEnable()
    {
        mGameModel = this.GetModel<IGameModel>();
        mGameModel.Score.RegisterWithInitValue(OnScoreChanged).UnRegisterWhenDisabled(gameObject);
        this.RegisterEvent<RefreshHUDRequestEvent>(OnRefreshHUDRequest).UnRegisterWhenDisabled(gameObject);
    }
    public void OnClickAddScore() => this.SendCommand(new AddScoreCommand(10));
    private void OnRefreshHUDRequest(RefreshHUDRequestEvent e) => this.SendCommand(new RefreshHUDCommand());
    private void OnScoreChanged(int score) => LogKit.I($"Score -> {score}");
}
```

## 3) Command (void)

```csharp
using QFramework;
public sealed class AddScoreCommand : AbstractCommand
{
    private readonly int mDelta;
    public AddScoreCommand(int delta) => mDelta = delta;
    protected override void OnExecute()
    {
        var model = this.GetModel<IGameModel>();
        model.AddScore(mDelta);
        this.SendEvent(new ScoreChangedEvent { NewScore = model.Score.Value });
    }
}
```

## 4) Command (with return)

```csharp
using QFramework;
public sealed class PurchaseItemCommand : AbstractCommand<bool>
{
    private readonly string mItemId;
    public PurchaseItemCommand(string itemId) => mItemId = itemId;
    protected override bool OnExecute()
    {
        var model = this.GetModel<IGameModel>();
        if (!model.CanAfford(mItemId)) return false;
        model.Purchase(mItemId);
        this.SendEvent(new InventoryChangedEvent());
        return true;
    }
}
```

## 5) Query

```csharp
using QFramework;
public sealed class GetBattlePowerQuery : AbstractQuery<int>
{
    protected override int OnDo()
    {
        var model = this.GetModel<IGameModel>();
        var system = this.GetSystem<IGameSystem>();
        return system.CalculateBattlePower(model.Level.Value, model.EquippedItems);
    }
}
```

## 6) Model with `BindableProperty`

```csharp
using System.Collections.Generic;
using QFramework;
public interface IGameModel : IModel
{
    BindableProperty<int> Score { get; }
    BindableProperty<int> Level { get; }
    IReadOnlyList<string> EquippedItems { get; }
    void AddScore(int delta);
    bool CanAfford(string itemId);
    void Purchase(string itemId);
}
public sealed class GameModel : AbstractModel, IGameModel
{
    public BindableProperty<int> Score { get; } = new BindableProperty<int>(0);
    public BindableProperty<int> Level { get; } = new BindableProperty<int>(1);
    private readonly List<string> mEquippedItems = new List<string>();
    public IReadOnlyList<string> EquippedItems => mEquippedItems;
    protected override void OnInit()
    {
        Score.SetValueWithoutEvent(0);
        Level.SetValueWithoutEvent(1);
    }
    public void AddScore(int delta) => Score.Value += delta;
    public bool CanAfford(string itemId) => itemId.IsNotNullAndEmpty() && Score.Value >= 10;
    public void Purchase(string itemId)
    {
        Score.Value -= 10;
        mEquippedItems.Add(itemId);
    }
}
```

## 7) System with Event Orchestration

```csharp
using QFramework;
public interface IGameSystem : ISystem
{
    int CalculateBattlePower(int level, System.Collections.Generic.IReadOnlyList<string> equippedItems);
}
public sealed class GameSystem : AbstractSystem, IGameSystem
{
    protected override void OnInit() => this.RegisterEvent<BattleEndedEvent>(OnBattleEnded);
    private void OnBattleEnded(BattleEndedEvent e) => this.SendEvent(new RefreshHUDRequestEvent());
    public int CalculateBattlePower(int level, System.Collections.Generic.IReadOnlyList<string> equippedItems)
        => level * 100 + equippedItems.Count * 25;
}
```

## 8) Optional Utility Skeleton

```csharp
using QFramework;
public interface IGameStorage : IUtility
{
    void SaveInt(string key, int value);
    int LoadInt(string key, int defaultValue = 0);
}
public sealed class GameStorage : IGameStorage
{
    public void SaveInt(string key, int value) => UnityEngine.PlayerPrefs.SetInt(key, value);
    public int LoadInt(string key, int defaultValue = 0) => UnityEngine.PlayerPrefs.GetInt(key, defaultValue);
}
```
