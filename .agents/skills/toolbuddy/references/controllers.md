# Controllers

Base: CurvyController
Location: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/Controllers/CurvyController.cs

Purpose
- Moves a target component along a spline/path/volume with speed, clamping, and orientation.

Serialized Fields (CurvyController)
General
- UpdateIn: When the controller updates (Update/LateUpdate/FixedUpdate).
- TargetComponent: Transform, KinematicRigidbody, or KinematicRigidbody2D.

Position
- PositionMode (m_PositionMode): Relative or WorldUnits.
- Position (m_Position): Current position along source (type depends on PositionMode).

Motion
- MoveMode (m_MoveMode): Relative or AbsolutePrecise.
- Speed (m_Speed): Positive speed, direction handled separately.
- Direction (m_Direction): Forward or Backward.
- Clamping (m_Clamping): Loop, Clamp, PingPong.
- MotionConstraints: Freeze axis/rotation.
- PlayAutomatically: Auto play on entering play mode.

Orientation
- OrientationMode (m_OrientationMode): None, Orientation, Tangent, etc.
- LockRotation (m_LockRotation): Lock rotation when OrientationMode = None.
- OrientationAxis (m_OrientationAxis): Axis used for orientation.
- IgnoreDirection (m_IgnoreDirection): Ignore movement direction.
- DampingDirection (m_DampingDirection): Direction damping time.
- DampingUp (m_DampingUp): Up damping time.

Offset
- OffsetAngle (m_OffsetAngle): Degrees around tangent.
- OffsetRadius (m_OffsetRadius): Lateral offset.
- OffsetCompensation (m_OffsetCompensation): Adjust speed for offset path.

Events
- onInitialized

Editor Only
- ForceFrequentUpdates (m_ForceFrequentUpdates)

SplineController
Location: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/Controllers/SplineController.cs

Extra Serialized Fields
- Spline (m_Spline): Source CurvySpline.
- UseCache (m_UseCache): Use spline cache for performance.

Connections Handling
- ConnectionBehavior: CurrentSpline, FollowUpSpline, RandomSpline, FollowUpOtherwiseRandom, Custom.
- AllowDirectionChange: Adjust direction to match connection.
- RejectCurrentSpline: Exclude current spline when random.
- RejectTooDivergentSplines: Filter by angle.
- MaxAllowedDivergenceAngle: Threshold in degrees.
- ConnectionCustomSelector: Custom selector script.

Events
- OnPositionReachedList
- OnControlPointReached
- OnEndReached
- OnSwitch

PathController
Location: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/Controllers/PathController.cs

Extra Serialized Fields
- Path (m_Path): CGDataReference to a CGPath slot.

VolumeController
Location: Assets/Plugins/ToolBuddy/Assets/Curvy/Scripts/Controllers/VolumeController.cs

Extra Serialized Fields
- Volume (m_Volume): CGDataReference to a CGVolume slot.
- CrossRange (m_CrossRange): From/To range, -0.5..0.5.
- CrossRelativePosition (crossRelativePosition): Lateral position within range.
- CrossClamping (m_CrossClamping): Clamp/Loop/PingPong for cross movement.

Operational Notes
- Controllers are initialized when source is ready (Spline initialized or CG data present).
- Speed is always positive; set Direction to control movement.
- OffsetCompensation corrects speed when offset radius is non-zero.
