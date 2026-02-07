# Architecture Rules

## Layer Capability Matrix

| Type | Can Access | Can Send | Must Not Do |
|---|---|---|---|
| `IController` | `System`, `Model`, `Utility` | `Command`, `Query`, register events | Directly mutate model state |
| `ISystem` | `System`, `Model`, `Utility` | typed events | Reference controller or UI objects |
| `IModel` | `Utility` | typed events | Depend on controller/system implementations |
| `IUtility` | internal infra dependencies | none by default | Become global business service locator |
| `ICommand` | `System`, `Model`, `Utility` | events, nested command/query | Hold mutable runtime state across frames |
| `IQuery<TResult>` | `System`, `Model` | query only | Perform writes or side effects |

Apply strict direction: upper layer may call lower layer, lower layer signals upward via events/bindables.

## CQRS Invariants

1. Route all writes through `Command`.
2. Keep `Query` read-only and deterministic.
3. Keep `Controller` as interaction adapter, not state writer.
4. Keep `System` as orchestration and policy, not UI or direct view owner.
5. Keep `Model` as state source and state transition host.

Forbidden write paths:

- `Controller -> Model.BindableProperty.Value = ...`
- `System -> Model.BindableProperty.Value = ...` (outside model-owned transition methods called by command path)
- `Query` that mutates any state
- event callbacks that bypass command path for writes

## Event Lifecycle Rules

Every subscription must have a deterministic unregister path.

Preferred patterns:

```csharp
this.RegisterEvent<SomeEvent>(OnSomeEvent)
    .UnRegisterWhenDisabled(gameObject);
```

```csharp
model.Count.RegisterWithInitValue(OnCountChanged)
    .UnRegisterWhenGameObjectDestroyed(gameObject);
```

Fallback explicit pattern:

```csharp
private IUnRegister mUnregister;

private void OnEnable()
{
    mUnregister = this.RegisterEvent<SomeEvent>(OnSomeEvent);
}

private void OnDisable()
{
    mUnregister?.UnRegister();
    mUnregister = null;
}
```

Avoid short-lived listeners on long-lived buses without lifecycle binding.

## Stateless Definition for Command and Query

Allowed:

- immutable constructor input that represents per-request parameters
- local variables inside `OnExecute` or `OnDo`

Not allowed:

- mutable fields that persist across frames
- caches of controller/view/gameobject references
- static mutable context as implicit input/output channel
- reusing a command/query instance as a stateful object

Target shape:

```csharp
public sealed class AddScoreCommand : AbstractCommand
{
    private readonly int mDelta;
    public AddScoreCommand(int delta) => mDelta = delta;

    protected override void OnExecute()
    {
        var model = this.GetModel<IGameModel>();
        model.AddScore(mDelta);
    }
}
```

## Single Root Principle

- Keep one root architecture for the runtime domain.
- Edit root registration only for global model/system/utility registration and deterministic startup order.
- Do not place feature business logic in root/bootstrap files.
