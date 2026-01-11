---
name: febucci-text-animator
description: Use when working with the Febucci Text Animator for Unity plugin in this repo (Assets/Plugins/Febucci/Text Animator for Unity), including adding animated text to TextMeshPro or UI Toolkit, applying effect tags/modifiers, configuring animator/typewriter settings, creating custom effects/actions, or troubleshooting plugin behavior.
---

# Febucci Text Animator

## Overview

Integrate Febucci Text Animator in this Unity project, with guidance for TMP and UI Toolkit setups, effect tags/modifiers, typewriter control, and settings/customizations.

## Quick Start

1. Pick the UI stack:
   - **TextMeshPro**: Add `TextAnimator_TMP` to the same GameObject as `TMP_Text`.
   - **UI Toolkit**: Use `Febucci.TextAnimatorForUnity.AnimatedLabel` in UXML or instantiate it via code.
2. Set text through the animator or typewriter APIs rather than directly on the text component.
3. Add tags for effects and modifiers (see “Effects & Tags”).

## Effects & Tags

- Use rich-text tags to scope effects:
  - **Persistent**: `<tagID>...</tagID>`
  - **Appearance**: `{tagID}...{/tagID}`
  - **Disappearance**: `{#tagID}...{/#tagID}`
- Stack effects by nesting tags (e.g., `<shake><size>Text</>`).
- Close all open tags with `</>`, `{/}`, or `{/#}` when helpful.
- Use modifiers like `<wave s=2>` or `<shake a=5>` when the effect supports them.

## Typewriter Control

- For TMP, add a `TypewriterComponent` alongside `TextAnimator_TMP` when you need per-letter reveal or actions.
- Call `ShowText`, `StartShowingText`, `SkipTypewriter`, and related methods from `ITypewriterProvider` as needed.
- Use `TypewriterSettingsScriptable`, `TypingDelaysByCharacter`, or `TypingDelaysByWord` assets for reusable timing presets.

## Settings & Defaults

- Prefer shared ScriptableObject settings for consistency across scenes.
- Animator defaults live in `AnimatorSettingsScriptable`; typewriter defaults in `TypewriterSettingsScriptable`.
- Check global settings and built-in databases before creating new assets.

## Custom Effects & Actions

- Create custom effects or actions by following the existing base classes in the plugin’s `Scripts/Runtime` folders.
- Register custom typewriter actions in the action database or local components for the target text.

## Limits & Gotchas

- Underlines/strikethroughs are intentionally not animated.
- Removing the plugin requires manually cleaning tags from text content.
- Backspace/partial erase inside a running typewriter is not supported; replace the full text instead.

## References

- See `references/locations.md` for doc paths and fast search hints.
