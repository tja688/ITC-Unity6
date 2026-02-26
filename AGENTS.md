# AGENTS.md — Unity Base (Safe-API-first)

## 0. Scope

A compact operating guide for an AI agent working in a Unity project.
Goals: **safe changes, visual/functional verification, runnable project**.

Core philosophy: **Operate strictly through Unity APIs to maintain project integrity.**

---

## 1. Core Rules

* Prefer **small, reversible** changes.
* **Show evidence** whenever anything is ambiguous: Console Logs / Screenshots (where allowed) / Status.
* Do not claim actions you did not actually perform.
* If in doubt, **skip and report** rather than force-resolve.

---

## 2. Unity Modification Constraints

* **Strict "No External Writing"**: AI/Automation MUST NOT directly write to `Assets/*.unity`, `.prefab`, `.asset`, or `.meta` files using file-write tools.
* **API-Only Modification**: All "Scene/Prefab/Asset" changes MUST be performed through the Unity API via MCP commands (e.g., `create_gameobject`, `add_component`, `set_property`, `manage_prefabs`, etc.).
* **YAML Modification Exception**: Direct text-based editing of YAML files (.unity, .prefab, etc.) is ONLY permitted if:
  1. The user explicitly requests it.
  2. The Unity Editor is closed OR the target scene/asset is NOT currently open.
  3. After modification, Unity must be allowed to re-import the asset before it is accessed again.
* **MCP Resilience & Blocking Dialogs**: If MCP hangs, loses connection, or fails to drive Unity (e.g., infinite waiting), investigate for blocking modal dialogs in the Editor. If the blockage coincides with a direct file modification attempt:
  - **Immediately pause the task.**
  - **Do not continue** with repeated automated attempts.
  - **Throw a request** to the user and wait for manual intervention.

---

## 3. Unity Workflow

### 3.1 Code vs Editor Changes

* **Code-only tasks**: Edit scripts using file manipulation tools.
* **Scene/Prefab/Asset wiring**:
  1. **Mandatory**: Use **Unity MCP** commands (`manage_gameobject`, `manage_components`, etc.).
  2. If MCP tools are insufficient: Create a **Unity Editor script/tool** to perform the action via `UnityEditor` API.
  3. Last resort: Provide **human steps** for the user to follow in the UI.

### 3.2 Safety Defaults

* Avoid mass reimports / GUID churn.
* Don't rename/move assets unless required.
* Prefer additive changes over destructive ones.

---

## 4. QFramework Governance (Mandatory in This Project)

### 4.1 Scripts Business Code Must Use QF Architecture

* All business code development under `Assets/Scripts/**` must follow QFramework architecture.
* For implementation details, always read and follow: `.agent/skills/qframework-architecture/SKILL.md`

### 4.2 Three Key Scenarios Must Use Corresponding QF Skills

* `res` scenario -> `.agent/skills/qframework-reskit/SKILL.md`
* `audio` scenario -> `.agent/skills/qframework-audiokit/SKILL.md`
* `ui` scenario -> `.agent/skills/qframework-uikit/SKILL.md`

---

## 5. Parallel Development Constraints

* **Multi-AI Parallel Development**: When the user specifies that multiple AI agents are developing in parallel, **prohibit** the use of MCP runtime/monitoring tools that interact with the active Unity "Play" or "Editor" state (e.g., screenshots or frequent scene polling) to prevent undefined errors or crashes caused by background script compilation and hot-reloading during code modifications.

---

## 6. Verification & Reporting

* **Verification**: Use `read_console` to verify that no errors related to your changes appear in the Unity console.
* **Reporting**: When finished, provide a clear summary of changes including modified assets and any manual steps required.
