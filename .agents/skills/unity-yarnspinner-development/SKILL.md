---
name: unity-yarnspinner-development
description: Develop, extend, and debug the Yarn Spinner for Unity integration. Use when modifying or understanding Yarn Spinner Unity runtime/editor code, command and function registration, localization and line providers, variable storage, dialogue presentation, or import pipeline behavior.
---

# Unity Yarn Spinner Development

## Overview

Use this skill to develop, customize, and debug the Yarn Spinner for Unity plugin itself (runtime and editor integration). Keep .yarn authoring and project-specific markup rules out of scope.

## Scope and guardrails

- Do not use this skill for authoring .yarn dialogue content; use `itc-yarn-authoring` instead.
- Avoid editing `Library/PackageCache` directly; embed the package under `Packages/` before changing Yarn Spinner source.
- If a request is about Text Animator markup or project dialogue formatting rules, switch to `itc-yarn-authoring` or `febucci-text-animator`.

## Quick start: pick the extension point

- Dialogue flow and orchestration: `DialogueRunner` and its action registration.
- UI presentation: `DialoguePresenterBase` and built-in presenters.
- Localization and line retrieval: `LineProviderBehaviour`, `Localization`, Unity Localization provider.
- Commands and functions: `YarnCommandAttribute`, `YarnFunctionAttribute`, `Actions`.
- Variables and persistence: `VariableStorageBehaviour` and storage implementations.
- Importers and editor UX: Yarn importers, editors, settings providers.

Use `references/source-map.md` to jump to the exact files.

## Common customization tasks

### Build a custom dialogue presenter

1. Subclass `DialoguePresenterBase` and implement `RunLineAsync`, `RunOptionsAsync`, `OnDialogueStartedAsync`, and `OnDialogueCompleteAsync`.
2. Honor `LineCancellationToken` (stop on `NextContentToken`, speed up on `HurryUpToken`).
3. Add the component to a scene object and register it on the `DialogueRunner`.
4. Return immediately for content types you do not handle.

### Add or override commands and functions

1. Use `[YarnCommand]` on `MonoBehaviour` methods or call `DialogueRunner.AddCommandHandler`.
2. Use `[YarnFunction]` or `DialogueRunner.AddFunction` for Yarn functions.
3. On Unity 2021.1 or earlier, run Yarn Spinner -> Update Yarn Commands after adding attributes.
4. Prefer `YarnTask`, `Task`, `IEnumerator`, or `Coroutine` for async commands; validate parameter types.

### Customize line providers and localization

1. Subclass `LineProviderBehaviour` and implement `GetLocalizedLineAsync`, `LocaleCode`, and marker processor registration.
2. For built-in localization, review `BuiltinLocalisedLineProvider` and `Localization` assets.
3. For Unity Localization, use `UnityLocalisedLineProvider` and configure string/asset tables.
4. Register custom markup processors via `RegisterMarkerProcessor`.

### Customize variable storage and persistence

1. Subclass `VariableStorageBehaviour` to back variables with your save system or game state.
2. Ensure `Program` is assigned so variable kinds and defaults resolve correctly.
3. Use `AddChangeListener` for reactive updates.

### Modify import pipeline or editor tools

1. Inspect `YarnImporter` and `YarnProjectImporter` for compilation and asset generation.
2. Adjust editor UX in `DialogueRunnerEditor`, `YarnProjectEditor`, and settings providers.
3. Use `CommandsWindow` when validating registered commands/functions.
4. Embed the package before editing any importer/editor code.

## Debugging checklist

- If `YarnProject.Program` is null, resolve importer/compiler errors first.
- If lines are missing, verify `LineProvider` locale and localization tables.
- Turn on `DialogueRunner.verboseLogging` for runtime tracing.
- Confirm presenters are enabled and registered in `DialogueRunner`.

## References

- `references/source-map.md`
