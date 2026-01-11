# Game Animation Principles Summary

Source: .agents/skills/game-development/SKILL.md

## Principles (apply to DOTween choices)
- Squash and Stretch: use scale or punch for impact and weight, keep volume consistent.
- Anticipation: add small pre motion before main action.
- Staging: keep focus clear, avoid UI noise during key motion.
- Pose to Pose: stage key poses with simple tweens, add secondary motion later.
- Follow Through and Overlap: add small trailing motion or settle after main move.
- Slow In and Slow Out: prefer eased curves over linear motion.
- Arc: prefer curved paths for natural motion instead of straight lines.
- Secondary Action: support with particles, audio, or slight shake when asked.
- Timing: tune duration and delay to feel responsive; keep input response under 100ms when asked.
- Exaggeration: push overshoot or amplitude for readability when requested.
- Solid Drawing: maintain readable silhouettes and spacing.
- Appeal: keep motion satisfying and clean.

## Timing reference (60fps)
- Light action: startup 3 to 6 frames, recovery 8 to 12 frames.
- Heavy action: startup 12 to 20 frames, recovery 16 to 24 frames.
- Hit stop: 2 to 5 frames when explicitly requested.
