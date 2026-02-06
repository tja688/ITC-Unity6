# DOTweenPath Field Map

Source: DOTweenPro Examples/DOTweenPath.unity and DOTween XML docs.

## Core fields
- updateType (UpdateType enum)
- isSpeedBased (bool)
- delay (float)
- duration (float)
- easeType (Ease enum)
- easeCurve (AnimationCurve)
- loops (int)
- loopType (LoopType enum)
- id (string)
- autoPlay (bool)
- autoKill (bool)
- relative (bool)
- isLocal (bool)
- isClosedPath (bool)
- pathResolution (int)
- pathType (PathType enum)
- pathMode (PathMode enum)
- tweenRigidbody (bool)

## Orientation
- orientType (enum)
- lookAtTransform (Transform)
- lookAtPosition (Vector3)
- lookAhead (float)
- lockRotation (bool)
- assignForwardAndUp (bool)
- forwardDirection (Vector3)
- upDirection (Vector3)

## Waypoints
- wps (Vector3 list): main waypoint list.
- fullWps (Vector3 list): used for CubicBezier control points.
- path (Path data): generated data, usually do not edit directly.

## Editor only fields (safe to ignore)
- inspectorMode, handlesType, livePreview, handlesDrawMode, perspectiveHandleSize
- showIndexes, showWpLength, pathColor, lastSrcPosition, lastSrcRotation
- wpsDropdown, dropToFloorOffset

## PathType and PathMode
- PathType: Linear, CatmullRom, CubicBezier (experimental, needs extra control points).
- PathMode: Ignore, Full3D, TopDown2D, Sidescroller2D.

## Waypoint rules
- Use wps in world coordinates when isLocal is false.
- Use local coordinates when isLocal is true (relative to the DOTweenPath GameObject).
- If the user requests Bezier but only provides anchor points, prefer CatmullRom or ask for control points.
- For CubicBezier, fullWps must include control points (two per segment). Do not guess unless the user accepts a heuristic.

## MCP usage notes
- Read existing DOTweenPath component fields with component_getComponents before setting.
- Set wps and pathType first, then set other options.
- Let DOTweenPath regenerate the internal path; avoid writing the path field directly.
