# DOTweenTimeline Slider Preview - Feasibility Test

## Changes
- Added `DOTweenTimelineSliderPreview` to connect a UI Slider to a `DOTweenTimeline` preview.
- No changes made to `Assets/Plugins/DOTweenTimeline`.

## Implementation Notes
- The script listens to `Slider.onValueChanged` and drives timeline time by calling:
  - `GotoPercentageAndPause(value)` when using normalized slider values (0-1).
  - `GotoAndPause(seconds)` when using time values (seconds).
- `pauseBeforePreview` stops any active playback before seeking, ensuring the slider acts as a scrubber.
- Timeline is auto-resolved from the parent if not assigned.

## Setup Steps
1. Create or reuse a `DOTweenTimeline` GameObject in the scene.
2. Create a UI `Slider` in the scene.
3. Add `DOTweenTimelineSliderPreview` to the Slider GameObject.
4. Assign the target `DOTweenTimeline` reference (or keep it empty to auto-find in parent).
5. For basic preview:
   - Keep `useNormalizedValue = true`.
   - Ensure Slider min = 0 and max = 1 (script enforces this at runtime).
6. Enter Play Mode and drag the Slider to preview the timeline.
