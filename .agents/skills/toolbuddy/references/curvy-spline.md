# CurvySpline

Location: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/Splines/CurvySpline_private.cs
Defaults: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/Splines/CurvySplineDefaultValues.cs

Purpose
- Defines a spline, stores control points, and computes cached sampling.
- Refreshes when dirty, and exposes events for tooling and runtime updates.

Serialized Fields and Meaning
General
- Interpolation (m_Interpolation): CurvyInterpolation. Default: CurvyGlobalManager.DefaultInterpolation.
- RestrictTo2D (m_RestrictTo2D): If true, control points are constrained to a plane.
- restricted2DPlane: CurvyPlane (default XY) used when RestrictTo2D is true.
- Closed (m_Closed): Loop the spline.
- AutoEndTangents (m_AutoEndTangents, default true): Handles end control points automatically.
- Orientation (m_Orientation, default Dynamic): Orientation flow along spline.

Bezier Options (only when Interpolation = Bezier)
- AutoHandleDistance (m_AutoHandleDistance, default 0.39): Default handle length ratio.

TCB Options (only when Interpolation = TCB)
- Tension (m_Tension)
- Continuity (m_Continuity)
- Bias (m_Bias)

B-Spline Options (only when Interpolation = BSpline)
- bSplineDegree (default 2): Range is [2, controlPointsCount - 1].
- isBSplineClamped (default true): When true, curve passes through first/last control points.

Advanced Settings
- GizmoColor (m_GizmoColor): Default CurvyGlobalManager.DefaultGizmoColor.
- GizmoSelectionColor (m_GizmoSelectionColor): Default CurvyGlobalManager.DefaultGizmoSelectionColor.
- CacheDensity (m_CacheDensity, default 50): 1..100, relative density vs MaxPointsPerUnit.
- MaxPointsPerUnit (m_MaxPointsPerUnit, default 8): Sampling cap per world unit.
- UsePooling (m_UsePooling, default true): Uses CurvyGlobalManager control point pool at runtime.
- UseThreading (m_UseThreading): Threaded updates when supported (not WebGL/UWP).
- CheckTransform (m_CheckTransform, default true): Auto refresh on CP transform changes.
- UpdateIn (m_UpdateIn, default Update): Update / LateUpdate / FixedUpdate.

Events
- onInitialized
- OnRefresh
- OnAfterControlPointChanges
- OnBeforeControlPointAdd
- OnAfterControlPointAdd
- OnBeforeControlPointDelete

Operational Notes
- If UsePooling is true, CurvyGlobalManager is instantiated in Awake.
- Initialization happens in Start/OnEnable; Refresh is triggered when dirty.
- CheckTransform watches CP transforms in editor and runtime; disable if you manage refresh manually.
