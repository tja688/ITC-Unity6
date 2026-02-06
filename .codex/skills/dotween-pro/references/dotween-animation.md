# DOTweenAnimation Field Map

Source: Assets/Plugins/Demigiant/DOTweenPro/DOTweenAnimation.cs and DOTweenPro Examples.

## Core fields
- targetIsSelf (bool): use the same GameObject as the animated target.
- targetGO (GameObject): target when targetIsSelf is false.
- tweenTargetIsTargetGO (bool): SetTarget uses targetGO when true.
- delay (float)
- duration (float)
- easeType (Ease enum)
- easeCurve (AnimationCurve) used only when easeType is INTERNAL_Custom.
- loopType (LoopType enum)
- loops (int)
- id (string)
- isRelative (bool)
- isFrom (bool)
- isIndependentUpdate (bool)
- autoKill (bool)
- autoGenerate (bool)
- autoPlay (bool)
- isActive (bool)
- isValid (bool)
- animationType (AnimationType enum)
- targetType (TargetType enum)
- forcedTargetType (TargetType enum)
- useTargetAsV3 (bool)
- endValueFloat (float)
- endValueV3 (Vector3)
- endValueV2 (Vector2)
- endValueColor (Color)
- endValueString (string)
- endValueRect (Rect)
- endValueTransform (Transform)
- optionalBool0, optionalBool1 (bool)
- optionalFloat0 (float)
- optionalInt0 (int)
- optionalRotationMode (RotateMode enum)
- optionalScrambleMode (ScrambleMode enum)
- optionalShakeRandomnessMode (ShakeRandomnessMode enum)
- optionalString (string)

## Base animation component fields
- updateType (UpdateType enum)
- isSpeedBased (bool)
- hasOnStart, hasOnPlay, hasOnUpdate, hasOnStepComplete, hasOnComplete, hasOnTweenCreated, hasOnRewind
- onStart, onPlay, onUpdate, onStepComplete, onComplete, onTweenCreated, onRewind (UnityEvent)

## AnimationType values
None, Move, LocalMove, Rotate, LocalRotate, Scale, Color, Fade, Text,
PunchPosition, PunchRotation, PunchScale, ShakePosition, ShakeRotation, ShakeScale,
CameraAspect, CameraBackgroundColor, CameraFieldOfView, CameraOrthoSize, CameraPixelRect, CameraRect,
UIWidthHeight, FillAmount

## TargetType values
Unset, Camera, CanvasGroup, Image, Light, RectTransform, Renderer, SpriteRenderer,
Rigidbody, Rigidbody2D, Text, Transform, tk2dBaseSprite, tk2dTextMesh, TextMeshPro, TextMeshProUGUI

## AnimationType supported targets (inspector mapping)
- Move: Rigidbody, Rigidbody2D, RectTransform, Transform
- Rotate: Rigidbody, Rigidbody2D, Transform
- LocalMove: Transform
- LocalRotate: Transform
- Scale: Transform
- Color: Light, SpriteRenderer, Image, Text, RawImage, Graphic, Renderer
- Fade: Light, SpriteRenderer, Image, Text, CanvasGroup, RawImage, Graphic, Renderer
- Text: Text (and TMP types when module is enabled)
- PunchPosition: RectTransform, Transform
- PunchRotation: Transform
- PunchScale: Transform
- ShakePosition: RectTransform, Transform
- ShakeRotation: Transform
- ShakeScale: Transform
- CameraAspect, CameraBackgroundColor, CameraFieldOfView, CameraOrthoSize, CameraPixelRect, CameraRect: Camera
- UIWidthHeight: RectTransform
- FillAmount: Image

## Required values and option mapping by AnimationType
- Move: endValueV3, optionalBool0 = snapping. If useTargetAsV3 true, set endValueTransform instead of endValueV3.
- LocalMove: endValueV3, optionalBool0 = snapping.
- Rotate: endValueV3 (Transform or Rigidbody) or endValueFloat (Rigidbody2D), optionalRotationMode.
- LocalRotate: endValueV3, optionalRotationMode.
- Scale: endValueV3. Use endValueFloat when optionalBool0 is true (uniform scale).
- UIWidthHeight: endValueV2. Use endValueFloat when optionalBool0 is true (uniform width and height).
- Color: endValueColor.
- Fade: endValueFloat.
- FillAmount: endValueFloat.
- Text: endValueString, optionalBool0 = richTextEnabled, optionalScrambleMode, optionalString = scrambleChars.
- PunchPosition: endValueV3 (punch), optionalInt0 = vibrato, optionalFloat0 = elasticity, optionalBool0 = snapping.
- PunchRotation: endValueV3, optionalInt0 = vibrato, optionalFloat0 = elasticity.
- PunchScale: endValueV3, optionalInt0 = vibrato, optionalFloat0 = elasticity.
- ShakePosition: endValueV3 (strength), optionalInt0 = vibrato, optionalFloat0 = randomness,
  optionalBool0 = snapping, optionalBool1 = fadeOut, optionalShakeRandomnessMode.
- ShakeRotation: endValueV3, optionalInt0 = vibrato, optionalFloat0 = randomness,
  optionalBool1 = fadeOut, optionalShakeRandomnessMode.
- ShakeScale: endValueV3, optionalInt0 = vibrato, optionalFloat0 = randomness,
  optionalBool1 = fadeOut, optionalShakeRandomnessMode.
- CameraAspect: endValueFloat.
- CameraBackgroundColor: endValueColor.
- CameraFieldOfView: endValueFloat.
- CameraOrthoSize: endValueFloat.
- CameraPixelRect: endValueRect.
- CameraRect: endValueRect.

## Defaults (use when user does not specify)
- duration = 1
- easeType = Ease.OutQuad
- loops = 1
- loopType = LoopType.Restart
- autoKill = true
- autoGenerate = true
- autoPlay = true

## Notes
- When Fade is used on an object with both CanvasGroup and Image, set forcedTargetType to pick the correct target.
- TargetType is chosen from the actual component on the target GameObject.
- Some TargetTypes require DOTween modules to be enabled (TextMeshPro, tk2d). Use DOTween Utility Panel to enable modules.
