# CurvySplineSegment (Control Point)

Location: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/Splines/CurvySplineSegment_private.cs
Defaults: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/Splines/CurvySplineSegmentDefaultValues.cs

Purpose
- Defines a control point and the segment starting at it.
- Holds per-point interpolation overrides and connection data.

Serialized Fields and Meaning
General
- AutoBakeOrientation (m_AutoBakeOrientation): Apply spline orientation to this transform.
- OrientationAnchor (m_OrientationAnchor): Use this CP rotation as an orientation anchor.
- Swirl (m_Swirl, default None): Swirl mode applied to orientation.
- SwirlTurns (m_SwirlTurns): Number of swirl turns.

Bezier Options (only when spline Interpolation = Bezier)
- AutoHandles (m_AutoHandles, default true): Auto-calc handles.
- AutoHandleDistance (m_AutoHandleDistance, default 0.39): Handle length ratio.
- HandleIn (m_HandleIn, default -1,0,0)
- HandleOut (m_HandleOut, default 1,0,0)

TCB Options (only when spline Interpolation = TCB)
- OverrideGlobalTension (m_OverrideGlobalTension)
- OverrideGlobalContinuity (m_OverrideGlobalContinuity)
- OverrideGlobalBias (m_OverrideGlobalBias)
- SynchronizeTCB (m_SynchronizeTCB, default true)
- StartTension / EndTension
- StartContinuity / EndContinuity
- StartBias / EndBias

Connections (stored on CP)
- FollowUp (m_FollowUp): Next CP when crossing a connection.
- FollowUpHeading (m_FollowUpHeading, default Auto): Heading for follow-up.
- ConnectionSyncPosition (m_ConnectionSyncPosition)
- ConnectionSyncRotation (m_ConnectionSyncRotation)
- Connection (m_Connection)

Operational Notes
- OrientationAnchor and Swirl only apply when the spline uses Dynamic orientation and the CP is visible.
- Connection sync options are enforced by CurvyConnection when syncing transforms.
- AutoHandles updates handles based on spline settings; disable to edit handles manually.
