---
name: qframework-uikit
description: Operate QFramework UIKit safely and consistently in Unity projects, including panel lifecycle (Open/Show/Hide/Close), typed IUIData flow, UI layering and stack behavior, and loader-pool integration through UIKit.Config.PanelLoaderPool (default Resources loader or ResKit-backed loader). Use when implementing, refactoring, or reviewing QFramework panel/UI runtime code. Use together with qframework-architecture in QFramework projects, and with qframework-reskit when UIKit panel assets are loaded via ResKit.
---

# QFramework UIKit

## Overview

Use this as a pluggable toolkit skill under `qframework-architecture`. Prioritize runtime panel behavior and architecture-safe UI flows. Treat editor/codegen internals as secondary unless explicitly requested.

## AI Coding Scope

Focus by default on:

- Runtime panel APIs in `UIKit.cs`, `UIManager.cs`, `UIPanel.cs`, `UIKitConfig.cs`.
- Panel open/close/show/hide/get flows.
- Typed `IUIData` handoff and panel lifecycle (`OnInit`, `OnOpen`, `OnClose`).
- Loader pool wiring (`IPanelLoader`, `IPanelLoaderPool`) and release symmetry.
- UI level/layer placement (`UILevel`, `UIRoot.SetLevelOfPanel`).

De-prioritize unless user explicitly asks:

- Editor code generation internals under `Assets/QFramework/Toolkits/UIKit/Editor/CodeGen`.
- Menu tooling, serializer/bind collector internals, and template pipeline changes.
- Generated designer files and template-level conventions.

## Workflow

1. Confirm architecture context from `qframework-architecture`.
2. Select panel operation path (`OpenPanel`, `OpenPanelAsync`, `ClosePanel`, `ShowPanel`, `HidePanel`, `GetPanel`).
3. Model UI input/output with typed `IUIData` and panel lifecycle methods.
4. Choose loader strategy (default `Resources` loader vs custom/ResKit loader pool).
5. Validate lifecycle and teardown (`Close` -> loader `Unload` -> pool recycle) before output.

## Hard Rules

- Prefer `UIKit` public APIs over direct `UIManager`/table manipulation in feature code.
- Use typed `IUIData` objects for panel input; avoid untyped payload patterns.
- Implement panel behavior through `OnInit`, `OnOpen`, `OnClose`; do not bypass panel lifecycle.
- Do not manually control panel loader lifetime from business code; let `UIPanel.Close` and `UIKit.Config.PanelLoaderPool` own loader recycle.
- Keep loader implementations symmetric: allocate/use/unload/recycle without leaks.
- If using async panel loading, model control flow accordingly (`IEnumerator` path), then fetch panel via `GetPanel<T>()` if needed.
- Use `UIKit.Stack`/`UIKit.Back` only when the push-pop navigation model is intentional.
- Treat QFramework UIKit source as read-only unless the user explicitly requests framework-internal edits.
- Treat generated `*.Designer.cs` as generated artifacts; do not hand-edit unless explicitly required.

## Architecture Coordination

- Keep CQRS boundaries from `qframework-architecture`: UIKit calls are UI-side effects, not state mutation authority.
- Keep business state writes in commands/model paths, then render state through panel open/update flows.
- Keep event/bindable registration lifecycle-safe in controllers and panels.

## Integration Hooks

- Existing ResKit-backed UIKit loader adapter:
  - `Assets/QFramework/Toolkits/SupportOldQF/Scripts/UIKitWithResKitInit.cs`
- Companion loader adapter in audio path:
  - `Assets/QFramework/Toolkits/SupportOldQF/Scripts/AudioKitWithResKitInit.cs`
- Use `qframework-reskit` when panel prefab loading is routed through ResKit.
- Use `qframework-audiokit` when panel interactions include AudioKit playback policy.

## Reference Loading Map

- Read `references/uikit-recipes.md` for runtime recipes and explicit boundary between AI-critical and external/codegen-heavy workflows.

## Hard Self-Check Gate

Answer every item before final output:

1. `API Path`: Are panel operations done via `UIKit` public APIs?
2. `Lifecycle`: Are `OnInit`/`OnOpen`/`OnClose` responsibilities clear and valid?
3. `IUIData`: Is panel data typed and passed through `IUIData` flow?
4. `Loader Symmetry`: If custom loader pool is involved, is `Unload` and recycle path deterministic?
5. `Async Flow`: If `OpenPanelAsync` is used, is follow-up flow safe and explicit?
6. `Navigation Model`: If using stack/back, is push-pop intent explicit and not accidental?
7. `Boundary`: Are CQRS and layer boundaries from `qframework-architecture` preserved?
8. `Scope`: Did you avoid unnecessary edits to codegen/editor internals and generated designer files?

If any answer is `No`, revise before presenting code.
