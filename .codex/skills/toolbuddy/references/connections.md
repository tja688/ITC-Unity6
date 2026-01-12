# CurvyConnection

Location: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/Splines/Connections/CurvyConnection.cs

Purpose
- Connects multiple control points and optionally synchronizes their transforms.
- Used by SplineController connection handling.

Key Concepts
- Connection holds a list of CurvySplineSegment control points.
- Sync behavior is stored per control point (ConnectionSyncPosition/Rotation).

Operational Notes
- Create connections with CurvyConnection.Create(params CurvySplineSegment[]).
- Add/remove control points via AddControlPoints/RemoveControlPoint.
- SetSynchronisationPositionAndRotation applies a reference transform to connected CPs.
- SetSynchronizationOptions syncs position/rotation flags on all connected CPs.
- When two CPs are connected with sync position, AutoSetFollowUp may set FollowUp.

Related Types
- ConnectionHeadingEnum (Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/Splines/Connections/ConnectionHeadingEnum.cs)
- CurvySplineSegment fields: FollowUp, FollowUpHeading, ConnectionSyncPosition, ConnectionSyncRotation, Connection.
