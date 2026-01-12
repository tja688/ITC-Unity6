# CurvyGlobalManager

Location: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/Other Components/CurvyGlobalManager.cs

Purpose
- Scene-wide singleton that owns pooling, gizmo defaults, and Curvy preferences.
- Auto-created when a CurvySpline uses pooling or when the component is added.
- GameObject name is forced to "_CurvyGlobal_".

Required Components
- PoolManager (FluffyUnderware.Curvy.Pools)
- ArrayPoolsSettings (FluffyUnderware.Curvy.Pools)

Key Static Settings (persisted via PlayerPrefs)
- SaveGeneratorOutputs (default true): If false, generator outputs are not saved in scenes.
- SceneViewResolution (default 0.5): Spline drawing resolution in Scene view.
- DefaultGizmoColor (default 0.71,0.71,0.71)
- DefaultGizmoSelectionColor (default 1,0.4,0)
- DefaultInterpolation (default CatmullRom): Used for new splines.
- GizmoControlPointSize (default 0.15)
- GizmoOrientationLength (default 1.0)
- GizmoOrientationColor (default 0.75,0.75,0.4)
- Gizmos (default Curve | Connections): Bitfield for what gets drawn.
- SplineLayer (default 0): Layer used for new spline GOs.
- HideManager: If true, hide _CurvyGlobal_ from hierarchy.

Gizmos Bitfield Helpers
- ShowCurveGizmo
- ShowConnectionsGizmo
- ShowApproximationGizmo
- ShowTangentsGizmo
- ShowOrientationGizmo
- ShowTFsGizmo
- ShowRelativeDistancesGizmo
- ShowLabelsGizmo
- ShowMetadataGizmo
- ShowBoundsGizmo
- ShowOrientationAnchorsGizmo

Runtime Behavior Notes
- Awake creates a ComponentPool<CurvySplineSegment> and caches ArrayPoolsSettings.
- SaveRuntimeSettings writes prefs for all fields above.
- LoadRuntimeSettings pulls values from prefs at startup.
- If multiple _CurvyGlobal_ instances load, MergeDoubleLoaded moves connections under the surviving instance.
