---
name: dotween-pro
description: Configure and operate DOTween Pro components in Unity (DOTweenAnimation, DOTweenPath, DOTweenVisualManager, Pro shortcuts, TMP or tk2d modules). Use when mapping user intent to DOTweenAnimation parameters, building DOTweenPath motion, or editing DOTween Pro components via MCP.
---

# DOTween Pro

## Overview
Enable consistent setup of DOTween Pro components with a focus on DOTweenAnimation parameter mapping and MCP driven edits.

## Workflow
1. Parse the user intent into concrete motion goals, timing, and target objects.
2. Choose the component type: DOTweenAnimation for most tweens, DOTweenPath for path motion.
3. Map intent to parameters using references; leave unspecified values at component defaults.
4. Use MCP to add components and set properties. Read existing components first to confirm field names.
5. Validate in scene, then create a prefab only if the user asks.

## MCP execution pattern
- Read the scene hierarchy before changes to avoid blind edits.
- Find or create target GameObjects.
- Add DOTweenAnimation or DOTweenPath components as needed.
- Call component_getComponents to confirm the component exists and to inspect fields.
- Set properties via component_setComponentProperties using field names from the references.
- For UI fades, prefer CanvasGroup when fading a whole UI group; otherwise use Image or Text and set forcedTargetType.

## Sequencing and multi stage effects
- If the user asks for a multi stage effect (for example, shrink then overshoot), create multiple DOTweenAnimation components on the same object.
- Keep AutoPlay off on later stages and chain via OnComplete or DOPlayNext only if the user needs sequencing.
- Keep each component focused on a single property change.

## Defaults rule
- Do not invent values. If the request does not specify a parameter, leave the component default in place.

## References
- DOTweenAnimation fields and parameter mapping: references/dotween-animation.md
- DOTweenPath fields and waypoint rules: references/dotween-path.md
- Intent to parameter cues: references/intent-to-params.md
- Game animation principles summary: references/game-animation-principles.md
- DOTween Pro component overview and module notes: references/dotween-pro-components.md

## Prefab handling
- Create or apply prefab changes only when the user explicitly asks.
