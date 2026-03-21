---
name: qframework-audiokit
description: Operate QFramework AudioKit safely and consistently in Unity projects, including BGM/voice/SFX playback APIs, AudioKit.Settings bindable toggles and volume control, PlaySoundModes anti-spam policy, and loader-pool integration through AudioKit.Config.AudioLoaderPool (including ResKit-backed loaders). Use when implementing, refactoring, or reviewing QFramework audio flows. Use together with qframework-architecture in QFramework projects, and with qframework-reskit when audio assets are loaded via ResKit.
---

# QFramework AudioKit

## Overview

Use this as a pluggable toolkit skill under `qframework-architecture`. Keep architecture ownership first, then apply AudioKit playback, settings, and loader-pool rules.

## Workflow

1. Confirm architecture context and boundaries from `qframework-architecture`.
2. Choose playback intent (`PlayMusic`, `PlayVoice`, `PlaySound`) and lifecycle behavior.
3. Choose loading strategy via `AudioKit.Config.AudioLoaderPool` (default or ResKit-backed).
4. Enforce teardown and state consistency (`Stop*`, `StopAllSound`, loader `Unload` path).
5. Run the hard self-check gate before final output.

## Hard Rules

- Use AudioKit public APIs for runtime playback (`PlayMusic`, `PlayVoice`, `PlaySound`) instead of ad-hoc `AudioSource` orchestration in business code.
- Treat `AudioKit.PlaySound(...)` as nullable in practice (can return `null` when sound is off or dedup policy blocks playback).
- Use `AudioKit.Settings` bindable properties (`IsSoundOn`, `IsMusicOn`, `IsVoiceOn`, and volume properties) for user-facing audio state.
- Do not bypass AudioKit settings with direct PlayerPrefs writes in feature logic.
- Configure anti-spam sound policy intentionally through `AudioKit.PlaySoundMode`, `SoundFrameCountForIgnoreSameSound`, and `GlobalFrameCountForIgnoreSameSound`.
- When using custom loaders, wire through `AudioKit.Config.AudioLoaderPool`; ensure loader `Unload()` is deterministic and recycles underlying resources.
- When integrating ResKit-backed loaders, follow `AudioKitWithResKitInit` pattern and ensure `ResLoader` lifecycle is symmetric (`Allocate` -> `Recycle2Cache`).
- Treat QFramework toolkit source code as read-only unless explicitly asked to edit framework internals.

## Architecture Coordination

- Keep CQRS ownership from `qframework-architecture`: audio playback is side-effect IO, not model state mutation authority.
- Route user setting changes through architecture-approved write paths (typically commands/model methods), then let AudioKit react through settings bindables.
- Keep event and bindable lifecycle hygiene; avoid unmanaged long-lived callbacks in controllers/systems.

## Integration Hooks

- Existing ResKit-backed audio loader adapter:
  - `Assets/QFramework/Toolkits/SupportOldQF/Scripts/AudioKitWithResKitInit.cs`
- Related UI-side ResKit adapter for cross-toolkit consistency:
  - `Assets/QFramework/Toolkits/SupportOldQF/Scripts/UIKitWithResKitInit.cs`
- Treat this skill as defining AudioKit-side requirements. Loader integration with ResKit should also follow `qframework-reskit`.

## Reference Loading Map

- Read `references/audiokit-recipes.md` for canonical playback/settings/integration snippets and anti-patterns.

## Hard Self-Check Gate

Answer every item before final output:

1. `Playback Intent`: Is the API choice correct for BGM vs voice vs SFX?
2. `Null Safety`: If using `PlaySound`, is `null` return handled safely?
3. `Settings Path`: Are toggles/volumes using `AudioKit.Settings` instead of ad-hoc persistence?
4. `Spam Policy`: Is `PlaySoundMode` configured intentionally for repeated SFX scenarios?
5. `Loader Path`: If custom or ResKit loader pool is used, is `Unload` and recycle path symmetric?
6. `Teardown`: Are stop/release paths explicit where scene/feature lifecycle requires cleanup?
7. `Boundary`: Are CQRS and layer boundaries from `qframework-architecture` preserved?

If any answer is `No`, revise before presenting code.
