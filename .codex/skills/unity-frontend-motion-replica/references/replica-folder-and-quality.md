# Replica Folder Naming And Acceptance

## Folder Naming

Use lowercase kebab-case.
Keep names short and behavior-oriented.

Pattern:
- `<domain>-<key-motion>-<style>`

Examples:
- `card-hover-tilt-glass`
- `pricing-toggle-elastic`
- `hero-banner-parallax-modern`
- `menu-reveal-stagger-minimal`

Avoid:
- generic names like `test1`, `demo`, `new-folder`
- mixed Chinese/English punctuation in folder names
- overly long names above 48 characters

## Scene Selection Rule

- Use `UGUIReplicaScene` for UGUI-based requests.
- Use `UIToolkitReplicaScene` for UI Toolkit-based requests.
- If user asks for both, implement in separate effect folders and keep each scene independently runnable.

## Asset Placement Rule

Store replica-specific files under:
- `Assets/Tests/Replica/<effect-folder>/`

Allow scene files in:
- `Assets/Scenes/UGUIReplicaScene.unity`
- `Assets/Scenes/UIToolkitReplicaScene.unity`

Avoid scattering one replica across unrelated directories.

## Playability Acceptance Checklist

- Pressing Play shows the effect within 3 seconds.
- No compile errors in Console related to new files.
- No missing script/material/font references.
- Motion has easing and at least one secondary visual response.
- UI state has visible idle, interaction, and transition distinction.

## Handoff Notes To User

When MCP path is used:
- Confirm scene path and effect folder path.
- Tell user to open the scene and press Play.

When fallback path is used:
- Tell user to run `Tools/Replica/Deploy Pending Replica Assets`.
- Tell user which scene will be generated/configured by the deploy tool.
