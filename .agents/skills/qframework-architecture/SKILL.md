---
name: qframework-architecture
description: Architecture-first QFramework development for Unity with strict CQRS and layer boundaries. Use when implementing, refactoring, or reviewing QFramework code that must enforce Controller/System/Model/Utility responsibilities, Command-Query separation, event and bindable state updates, ActionKit temporal orchestration, and FluentAPI readability rules.
---

# QFramework Architecture

## Overview

Produce high-quality QFramework code by enforcing architecture invariants first, then selecting core toolkit patterns that keep business code readable and consistent.

Use bundled references in this skill as the primary source. Load only the minimum file needed for the current task.

`AudioKit`, `ResKit`, and `UIKit` are pluggable choices and are intentionally out of scope for this core architecture skill.

## Core Workflow

1. Classify the task as `feature`, `refactor`, or `review`.
2. Map each responsibility to QF layers before writing code.
3. Enforce CQRS paths:
 - state changes through `Command`
 - read paths through direct query or `Query` when composition is complex
 - upward notification through events or bindables
4. Select toolkit usage by priority:
 - state/UI sync: `BindableProperty` + typed architecture events
 - temporal orchestration: `ActionKit`
 - fluent chaining: `FluentAPI` only when readability improves
5. Run the hard self-check gate and answer each item before producing final code.

## Hard Rules

- Keep a single root architecture entry (one root app per runtime domain).
- Keep dependency direction one-way: upper layer may call lower layer; lower layer must not reference upper layer.
- Route all state mutation through `Command`; do not mutate model state directly in `Controller` or `System`.
- Keep `Command` and `Query` stateless in behavior:
 - allow immutable constructor input for per-request data
 - forbid mutable cross-frame state, cached controller references, or static runtime context
- Notify upward changes via typed events or bindables.
- Unregister every event subscription with lifecycle-safe patterns.
- Use `LogKit` for logs in generated examples; do not use `UnityEngine.Debug.*`.
- Treat QFramework vendor sources as read-only unless user explicitly asks to modify framework internals.

## Toolkit Priority Policy

- Default to `BindableProperty` when UI reacts to model state and needs immediate initial sync via `RegisterWithInitValue`.
- Default to architecture typed events for cross-layer notifications and request handoff (`this.SendEvent`, `this.RegisterEvent`).
- Default to `ActionKit` for multi-step temporal flows (delay, sequence, parallel, repeat), rather than ad-hoc orchestration.
- Use `FluentAPI` conditionally:
 - keep chains short
 - preserve intent clarity
 - stop chaining when debugging or branching readability degrades

## Output Contract

When implementing or reviewing, include:

1. Architecture map (`type -> layer`).
2. Write/read path verification (who writes, who reads, where side effects happen).
3. Event lifecycle hygiene (`register -> unregister` symmetry).
4. Toolkit rationale (why bindable/event/action/fluent choices fit this case).
5. Risks and fixes if architecture drift exists.

## Reference Loading Map

- Read `references/architecture-rules.md` when deciding layer ownership, CQRS legality, or event lifecycle handling.
- Read `references/code-templates.md` when authoring new files or refactoring to canonical QF skeletons.
- Read `references/core-toolkits.md` when choosing `BindableProperty`, event strategy, `ActionKit`, or `FluentAPI`.
- Read `references/advanced-patterns.md` when implementing command interception, fallback event buses, or drift remediation.

## Hard Self-Check Gate

Answer every item before final output:

1. `Layering`: Is every class assigned to exactly one QF layer with legal dependencies?
2. `Writes`: Are all model state mutations triggered through commands?
3. `Reads`: Are read paths side-effect free, and are complex compositions in queries when needed?
4. `Events`: Does every subscription have deterministic unregistration?
5. `Bindables`: Are bindable updates owned by model/command paths, not direct controller/system writes?
6. `Action Flow`: If temporal logic exists, is `ActionKit` used with clear lifecycle start mode?
7. `Fluent Scope`: Are fluent chains short and clearer than non-fluent alternatives?
8. `Logging`: Are examples using `LogKit` instead of `UnityEngine.Debug.*`?
9. `Scope`: Did you avoid introducing non-core module requirements?

If any answer is `No`, revise the design before presenting code.
