# Intent to Parameter Cues

Use these cues only when the user explicitly implies them. Otherwise keep defaults.

## Motion style to easeType
- bounce, spring, elastic, overshoot: Ease.OutBack or Ease.OutElastic
- snappy, quick: Ease.OutQuad or Ease.OutCubic
- smooth, soft: Ease.InOutSine or Ease.InOutQuad
- linear, constant: Ease.Linear

## Looping
- loop, repeat N times: loops = N, loopType = Restart unless yoyo is stated.
- ping pong, yoyo, back and forth: loopType = Yoyo.
- forever, infinite: loops = -1.

## Timing
- delay, wait, after X seconds: delay = X.
- slow, leisurely: longer duration.
- quick, fast: shorter duration.

## Strength and amplitude
- slight, subtle: reduce endValue magnitude, punch strength, or shake strength.
- strong, dramatic: increase endValue magnitude or punch strength.

## Effects
- shake, jitter: use ShakePosition or ShakeRotation.
- punch, impact: use PunchPosition or PunchScale.
- fade in, fade out: use Fade with endValueFloat 1 or 0.
- pop in, pop out: use Scale with overshoot (or PunchScale).
