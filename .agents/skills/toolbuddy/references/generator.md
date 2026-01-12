# CurvyGenerator

Location: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/CG/CurvyGenerator.cs

Purpose
- Hosts a Curvy Generator graph composed of CG modules.
- Handles module ordering, refresh, and output resource management.

Serialized Fields
- AutoRefresh (m_AutoRefresh, default true): Auto-refresh when dirty.
- RefreshDelay (m_RefreshDelay): Min delay between refresh in play mode (ms).
- RefreshDelayEditor (m_RefreshDelayEditor, default 10): Min delay in edit mode (ms).
- OnRefresh (m_OnRefresh): Event after refresh.
- ForceFrequentUpdates (editor only): Update as often as play mode.
- Modules: List of CGModule instances (children of generator).

Operational Notes
- Initialize happens in Update; modules are sorted before refresh.
- Only dirty modules are refreshed unless forceUpdate is true.
- Modules can be added via AddModule<T>() or manually as children.
- SaveGeneratorOutputs (CurvyGlobalManager) controls output persistence in scenes.

Module System Overview
- Module classes live in `Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/CG Modules`.
- Core patterns:
  - [ModuleInfo] describes module name/category.
  - [InputSlot] and [OutputSlot] define graph connections.
  - [SerializeField] defines inspector parameters.
  - [Section], [Group], [FieldCondition] define inspector layout and behavior.

When to Open Generator Modules
- For any module parameter or behavior, open the module class file and read its serialized fields and Refresh/Process logic.
- Use references/generator-modules.md to find module file names quickly.
