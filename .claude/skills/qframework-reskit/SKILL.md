---
name: qframework-reskit
description: Operate QFramework ResKit safely and consistently in Unity projects, including choosing init mode (SimulationMode vs device mode), using ResLoader lifecycle correctly (Allocate/Recycle2Cache with ReleaseAllRes path), and implementing sync/async asset and scene loading with IResLoaderExtensions. Use when implementing, refactoring, or reviewing resource loading flows, or wiring ResKit-backed loader pools for AudioKit/UIKit adapters. Use together with qframework-architecture in QFramework projects.
---

# QFramework ResKit

## Overview

Use this as a pluggable toolkit skill under `qframework-architecture`. Enforce architecture ownership first, then apply ResKit loading and lifecycle rules.

## Workflow

1. Confirm architecture context and ownership boundaries from `qframework-architecture`.
2. Choose init strategy by runtime mode and platform.
3. Choose loading strategy (`LoadSync<T>`, `Add2Load<T> + LoadAsync`, or scene APIs).
4. Enforce loader lifecycle and deterministic release paths.
5. Run the hard self-check gate before final output.

## Hard Rules

- Never instantiate `ResLoader` via constructor. Always use `ResLoader.Allocate()`.
- Always recycle loader instances with `Recycle2Cache()` and ensure a `ReleaseAllRes` path through disposal/teardown lifecycle.
- In device mode, call `ResKit.Init()` or `ResKit.InitAsync()` after startup before using runtime AB loading.
- For WebGL, prefer async initialization via `ResKit.InitAsync()`.
- Prefer `IResLoaderExtensions` APIs: `LoadSync<T>`, `Add2Load<T>`, `LoadAsync`, `LoadSceneSync`, `LoadSceneAsync`.
- Respect asset/bundle naming and typed load semantics (asset name, optional owner bundle, typed generic load).
- Treat QFramework toolkit source code as read-only unless the user explicitly requests framework-internal edits.

## Architecture Coordination

- Keep CQRS ownership from `qframework-architecture`: loading assets is IO/toolkit work, not a replacement for command-driven state mutation.
- Keep business state mutation inside commands/model methods; use ResKit to fetch runtime resources needed by that flow.
- Do not use resource-loading callbacks to bypass architecture write/read boundaries.

## Integration Hooks

- Existing ResKit-backed adapters:
  - `Assets/QFramework/Toolkits/SupportOldQF/Scripts/AudioKitWithResKitInit.cs`
  - `Assets/QFramework/Toolkits/SupportOldQF/Scripts/UIKitWithResKitInit.cs`
- Treat this skill as defining ResKit-side requirements. AudioKit/UIKit integrations should follow these lifecycle and init constraints.

## Reference Loading Map

- Read `references/reskit-recipes.md` for minimal init matrix, canonical snippets, and anti-patterns.

## Hard Self-Check Gate

Answer every item before final output:

1. `Init Mode`: Is init strategy correct for SimulationMode vs device mode?
2. `WebGL`: If WebGL is involved, is `ResKit.InitAsync()` selected?
3. `Loader Lifecycle`: Is `ResLoader.Allocate()` paired with teardown `Recycle2Cache()`?
4. `Release Path`: Is there a deterministic `ReleaseAllRes` path through lifecycle/dispose?
5. `Async Safety`: Do async callbacks avoid hidden state writes and handle success/failure safely?
6. `Load API`: Are `IResLoaderExtensions` APIs used consistently for sync/async/scene operations?
7. `Boundary`: Are CQRS and layer boundaries from `qframework-architecture` preserved?

If any answer is `No`, revise before presenting code.
