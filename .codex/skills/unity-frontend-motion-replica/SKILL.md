---
name: unity-frontend-motion-replica
description: Recreate frontend UI animations and interaction effects inside Unity with a strict MCP-first workflow and a no-MCP fallback. Use when users ask to replicate web/frontend motion behavior into Unity (UGUI or UI Toolkit), require playable test scenes, or need an editor one-click deployment path when Unity MCP is unavailable.
---

# Unity Frontend Motion Replica Workflow

Execute this workflow whenever the user asks to reproduce a frontend effect in Unity.
Keep output directly playable from the Editor Play button.

## Execution Contract

Follow the five-step gate in this exact order.
Do not skip validation gates.
Use MCP operations only when both Unity and Unity MCP are available.

1. Check whether Unity is open.
2. Check whether Unity MCP is available.
3. Ensure target folders under `Assets/Tests/Replica/<effect-folder>/`.
4. Build complete replication artifacts and playable replica scene (MCP path).
5. If step 1 or step 2 fails, avoid MCP calls and generate file-only assets plus one-click Editor deploy tool.

## Step 1: Check Unity Open State

Attempt a lightweight Unity MCP read call first:
- `mcp__unityMCP__manage_scene` with `action=get_active`

Interpretation:
- If it returns scene information, treat Unity as open.
- If connection/tool errors indicate no active editor instance, treat Unity as not open and jump to Step 5.

## Step 2: Check Unity MCP Availability

Run a second MCP capability check:
- `mcp__unityMCP__manage_editor` with `action=telemetry_ping`

Interpretation:
- If successful, continue to Step 3 and use MCP operations.
- If unavailable or failing due to MCP transport/tool state, jump to Step 5.

## Step 3: Prepare Replica Folder Structure

Create and use these directories:
- Base test folder: `Assets/Tests/`
- Replica root: `Assets/Tests/Replica/`
- Effect folder: `Assets/Tests/Replica/<effect-folder>/`

Derive `<effect-folder>` from the requested effect summary.
Apply naming rules in `references/replica-folder-and-quality.md`.
Store all task assets inside that effect folder:
- scripts (`*.cs`)
- shaders (`*.shader`, Shader Graph assets)
- materials/prefabs/data assets

## Step 4: MCP Build Path (Unity + MCP Available)

Implement the replica based on user scope (`UGUI` or `UI Toolkit`):

1. Ensure scene exists:
- `Assets/Scenes/UGUIReplicaScene.unity` for UGUI requests.
- `Assets/Scenes/UIToolkitReplicaScene.unity` for UI Toolkit requests.
- Create if missing; then load and configure.

2. Configure scene for immediate preview:
- camera and background styling
- root canvas/document setup
- base typography, spacing, and color contrast
- clear visual hierarchy so effect is not flat or overly minimal

3. Create and attach required assets in the effect folder:
- controller scripts
- optional shader/material assets
- optional helper prefabs and ScriptableObjects

4. Wire runtime behavior:
- bind events, timelines, tweens, and state transitions
- make Play mode entry immediately demonstrate the effect without manual setup

5. Verify before handoff:
- check Unity console for compile/runtime errors
- ensure no missing references
- confirm first 3 seconds in Play mode clearly present the intended motion

## Step 5: No-MCP Fallback Path (Unity Closed or MCP Unavailable)

When Step 1 or Step 2 fails:
- Do not run MCP operations.
- Create all needed files directly in the project tree, especially inside `Assets/Tests/Replica/<effect-folder>/`.

Create an Editor one-click deployment tool so user can complete setup inside Unity:
- target path: `Assets/Editor/Replica/ReplicaOfflineDeployTool.cs`
- use this skill script:
  - `scripts/install_offline_replica_tool.py --project-root <unity-project-root>`
- template source:
  - `assets/ReplicaOfflineDeployTool.cs.txt`

After file generation, provide user operation steps:
1. Open Unity project.
2. Wait for compile.
3. Click menu `Tools/Replica/Deploy Pending Replica Assets`.
4. Open `UGUIReplicaScene` or `UIToolkitReplicaScene`.
5. Press Play to preview.

## Quality Baseline

Always enforce these minimum quality checks:
- visual design has at least one layered background treatment (gradient, panel depth, or texture)
- animation includes easing/stagger/transition intent, not only linear movement
- interaction has clear hover/press or focus-state feedback
- scene starts in a presentable state with no manual camera/UI adjustment
- all replica files stay scoped to the designated effect folder and related scene

## Resources

- Naming and acceptance rules:
  - `references/replica-folder-and-quality.md`
- Offline one-click deploy installer:
  - `scripts/install_offline_replica_tool.py`
- Editor deploy tool template:
  - `assets/ReplicaOfflineDeployTool.cs.txt`
