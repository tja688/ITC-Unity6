# Core Toolkits

Use these toolkit rules to keep business code consistent with QFramework architecture.

## 1) BindableProperty

Use when UI or orchestration needs reactive state updates.

Rules:

- Define bindables in `Model`.
- Mutate bindable values through command-driven model methods.
- Use `RegisterWithInitValue` for UI that needs initial render and live updates.
- Always bind unregister to lifecycle.

Example:

```csharp
// Model
public BindableProperty<int> HP { get; } = new BindableProperty<int>(100);

// Controller
private void OnEnable()
{
    var model = this.GetModel<ICombatModel>();
    model.HP.RegisterWithInitValue(UpdateHPBar)
        .UnRegisterWhenDisabled(gameObject);
}
```

## 2) Event Strategy (EventKit in Architecture Context)

Default to typed architecture events:

- publish: `this.SendEvent(new SomeEvent { ... })`
- subscribe: `this.RegisterEvent<SomeEvent>(OnSomeEvent)`

Use `TypeEventSystem` style typed events as default path for cross-layer communication.

Example:

```csharp
public struct RequestSaveEvent
{
    public string SlotId;
}

// System publishes
this.SendEvent(new RequestSaveEvent { SlotId = "auto" });

// Controller subscribes
this.RegisterEvent<RequestSaveEvent>(OnRequestSave)
    .UnRegisterWhenDisabled(gameObject);
```

Guideline:

- Keep event payload focused and serializable.
- Do not use events to hide direct writes that should be explicit commands.

## 3) ActionKit

Use for temporal orchestration instead of ad-hoc delay/coroutine sprawl.

Recommended primitives:

- `ActionKit.Delay(...)`
- `ActionKit.Sequence()`
- `ActionKit.Parallel()`
- `ActionKit.Repeat(...)`

Start modes:

- `Start(this)` for component-bound lifecycle.
- `StartCurrentScene(...)` for scene-lifetime orchestration.
- `StartGlobal(...)` for global runner semantics.

Example:

```csharp
ActionKit.Sequence()
    .Callback(() => this.SendCommand(new LockInputCommand()))
    .Delay(0.2f)
    .Parallel(p =>
    {
        p.Delay(0.2f, () => this.SendEvent(new PlayFxEvent()));
        p.Delay(0.2f, () => this.SendCommand(new ResolveHitCommand()));
    })
    .Callback(() => this.SendCommand(new UnlockInputCommand()))
    .Start(this);
```

Use `IgnoreTimeScale()` on the returned controller only when UI/flow must run under paused time scale.

## 4) FluentAPI

Use FluentAPI as a readability enhancer, not as a default style mandate.

Use when:

- the chain represents one coherent intent
- each chained call is obvious and short

Avoid when:

- chain length hides branch conditions
- debugging breakpoints become unclear
- non-fluent code is more explicit

Good:

```csharp
target.Parent(parent).LocalIdentity().Show();
```

Avoid:

```csharp
// Too long for business flow readability:
// create -> lookup -> branch -> side effects -> analytics in one chain
```

Boundary rule:

- If a chain exceeds the local readability threshold, break into named intermediate steps.
