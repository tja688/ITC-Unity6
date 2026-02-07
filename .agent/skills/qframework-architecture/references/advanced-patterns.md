# Advanced Patterns

Use these patterns only when baseline architecture rules are already satisfied.

## 1) Optional Command Interception

Use interception for command logging, audit, middleware-like concerns, or deterministic replay.

Pattern:

```csharp
using QFramework;

public sealed class GameRootApp : Architecture<GameRootApp>
{
    protected override void Init()
    {
        this.RegisterModel<IGameModel>(new GameModel());
        this.RegisterSystem<IGameSystem>(new GameSystem());
    }

    protected override void ExecuteCommand(ICommand command)
    {
        LogKit.I($"Before Command: {command.GetType().Name}");
        base.ExecuteCommand(command);
        LogKit.I($"After Command: {command.GetType().Name}");
    }
}
```

Guidelines:

- Keep interception side effects lightweight.
- Never mutate unrelated model state in interceptor.
- Preserve command determinism and order.

## 2) Fallback Event Buses

Default remains typed architecture events. Use fallback buses only with explicit reason.

Use `EnumEventSystem` when:

- integrating with message-id protocols
- interoperating with long-link/network message enums

Use `StringEventSystem` when:

- bridging to external script layers
- message keys are naturally string-based and integration-driven

Do not select fallback buses for convenience if typed architecture events already fit.

## 3) Architecture Drift Anti-Patterns and Fixes

| Drift Pattern | Risk | Fix |
|---|---|---|
| Controller directly sets model bindable | bypasses write path, hidden coupling | move mutation to command; controller sends command |
| System holds controller reference | inverted dependency, test fragility | remove controller reference; send typed event upward |
| Query performs writes | read path side effects, nondeterminism | split write into command; keep query pure |
| Event subscription without unregister | leaks and ghost callbacks | bind unregister to disable/destroy lifecycle |
| Over-chained FluentAPI in business flow | low debuggability | split into named intermediate steps |
| Temporal flow scattered across coroutines | ordering bugs | consolidate with `ActionKit` sequence/parallel |

## 4) Review Checklist for Drift Repair

1. Identify illegal write paths first.
2. Introduce command boundaries before any micro-refactor.
3. Rewire upward notifications via typed events/bindables.
4. Add lifecycle-safe unregister to every listener.
5. Re-check toolkit choices after architecture is clean.
