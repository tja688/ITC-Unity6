using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEditor;

namespace Dott.Editor
{
    public class DottController : IDisposable
    {
        private enum PlaybackDirection
        {
            Forward,
            Backward
        }

        private double startTime;
        private IDOTweenAnimation[] currentPlayAnimations;
        private readonly DottDrivenProperties drivenProperties;
        private PlaybackDirection playbackDirection = PlaybackDirection.Forward;
        private float playFromTime;

        public bool IsPlaying => DottEditorPreview.IsPlaying;
        public bool IsPlayingBackwards => IsPlaying && playbackDirection == PlaybackDirection.Backward;
        public float ElapsedTime
        {
            get
            {
                if (!IsPlaying)
                {
                    return (float)DottEditorPreview.CurrentTime;
                }

                var time = playbackDirection == PlaybackDirection.Backward
                    ? (float)(startTime - DottEditorPreview.CurrentTime)
                    : (float)(DottEditorPreview.CurrentTime - startTime);
                return Math.Max(0f, time);
            }
        }
        public bool Paused { get; private set; }

        public bool Loop
        {
            get => EditorPrefs.GetBool("Dott.Loop", false);
            set => EditorPrefs.SetBool("Dott.Loop", value);
        }

        public DottController()
        {
            DottEditorPreview.Completed += DottEditorPreviewOnCompleted;
            drivenProperties = new DottDrivenProperties();
        }

        public void Play(IDOTweenAnimation[] animations) =>
            PlayInternal(animations, PlaybackDirection.Forward, null);

        public void PlayBackwards(IDOTweenAnimation[] animations) =>
            PlayInternal(animations, PlaybackDirection.Backward, null);

        private void PlayInternal(IDOTweenAnimation[] animations, PlaybackDirection direction, float? startFromTime)
        {
            var shift = startFromTime ?? (IsPlaying ? ElapsedTime : (float)DottEditorPreview.CurrentTime);
            currentPlayAnimations = animations;
            playbackDirection = direction;
            playFromTime = shift;

            GoTo(animations, shift);
            DottEditorPreview.SetPlaybackDirection(direction == PlaybackDirection.Backward);
            DottEditorPreview.Start();
            startTime = direction == PlaybackDirection.Backward
                ? DottEditorPreview.CurrentTime + shift
                : DottEditorPreview.CurrentTime - shift;
            Paused = false;
        }

        public void GoTo(IDOTweenAnimation[] animations, in float time)
        {
            DottEditorPreview.Stop();

            drivenProperties.Register(animations);
            Sort(animations).ForEach(PreviewTween);
            DottEditorPreview.GoTo(time);
            startTime = 0;
        }

        public void Stop()
        {
            currentPlayAnimations = null;
            Paused = false;
            playbackDirection = PlaybackDirection.Forward;
            playFromTime = 0f;
            DottEditorPreview.Stop();
            drivenProperties.Unregister();
        }

        public void Pause()
        {
            Paused = true;
        }

        [CanBeNull]
        private static Tween PreviewTween(IDOTweenAnimation animation)
        {
            if (!animation.IsValid || !animation.IsActive) { return null; }

            var tween = animation.CreateEditorPreview();
            if (tween == null) { return null; }

            DottEditorPreview.Add(tween, animation.IsFrom, animation.AllowEditorCallbacks);
            return tween;
        }

        private static IEnumerable<IDOTweenAnimation> Sort(IDOTweenAnimation[] animations)
        {
            return animations.OrderBy(animation => animation.Delay);
        }

        private void DottEditorPreviewOnCompleted()
        {
            // Hotfix to prevent exception when two Inspector tabs with a Timeline component are open
            if (currentPlayAnimations == null) { return; }

            if (!Loop)
            {
                Stop();
                return;
            }

            DottEditorPreview.Stop();
            if (playbackDirection == PlaybackDirection.Backward)
            {
                PlayInternal(currentPlayAnimations, PlaybackDirection.Backward, playFromTime);
            }
            else
            {
                Play(currentPlayAnimations);
            }
        }

        public void Dispose()
        {
            Stop();
            drivenProperties.Dispose();
            DottEditorPreview.Completed -= DottEditorPreviewOnCompleted;
        }
    }
}
