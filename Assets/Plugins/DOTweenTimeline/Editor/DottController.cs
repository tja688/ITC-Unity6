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
        private double lastUpdateTime;
        private IDOTweenAnimation[] currentPlayAnimations;
        private readonly DottDrivenProperties drivenProperties;
        private PlaybackDirection playbackDirection = PlaybackDirection.Forward;
        private float playFromTime;


        /// <summary>
        /// 倒播时的当前动画时间（用于虚拟拖进度条模式）
        /// </summary>
        private float backwardsAnimationTime;

        public bool IsPlaying => DottEditorPreview.IsPlaying || isPlayingBackwardsVirtual;
        public bool IsPlayingBackwards => playbackDirection == PlaybackDirection.Backward;


        /// <summary>
        /// 是否正在使用"虚拟拖进度条"模式倒播
        /// </summary>
        private bool isPlayingBackwardsVirtual;


        public float ElapsedTime
        {
            get
            {
                // 倒播时使用独立追踪的动画时间
                if (isPlayingBackwardsVirtual)
                {
                    return backwardsAnimationTime;
                }


                if (!DottEditorPreview.IsPlaying)
                {
                    return (float)DottEditorPreview.CurrentTime;
                }

                var time = (float)(DottEditorPreview.CurrentTime - startTime);
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
            EditorApplication.update += OnEditorUpdate;
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

            if (direction == PlaybackDirection.Backward)
            {
                // 倒播使用"虚拟拖进度条"模式
                // 先跳到起始位置，然后每帧递减时间并调用 GoTo
                isPlayingBackwardsVirtual = true;
                backwardsAnimationTime = shift;
                lastUpdateTime = EditorApplication.timeSinceStartup;

                // 初始跳转到当前位置

                GoTo(animations, shift);
                Paused = false;
            }
            else
            {
                // 正播使用原始的 ManualUpdate 模式
                isPlayingBackwardsVirtual = false;
                GoTo(animations, shift);
                DottEditorPreview.Start();
                startTime = DottEditorPreview.CurrentTime - shift;
                Paused = false;
            }
        }


        /// <summary>
        /// 编辑器更新回调，用于处理"虚拟拖进度条"模式的倒播
        /// </summary>
        private void OnEditorUpdate()
        {
            if (!isPlayingBackwardsVirtual || Paused)
            {
                return;
            }


            var currentTime = EditorApplication.timeSinceStartup;
            var delta = (float)(currentTime - lastUpdateTime);
            lastUpdateTime = currentTime;

            // 递减动画时间

            backwardsAnimationTime -= delta;


            if (backwardsAnimationTime <= 0)
            {
                backwardsAnimationTime = 0;

                // 跳到终点后检查是否需要循环

                if (currentPlayAnimations != null)
                {
                    GoTo(currentPlayAnimations, 0);


                    if (Loop)
                    {
                        // 循环模式：重新从起始位置开始倒播
                        backwardsAnimationTime = playFromTime;
                    }
                    else
                    {
                        // 非循环模式：停止倒播
                        isPlayingBackwardsVirtual = false;
                        Paused = true;
                    }
                }
            }
            else if (currentPlayAnimations != null)
            {
                // 每帧调用 GoTo，模拟"拖动进度条"
                GoTo(currentPlayAnimations, backwardsAnimationTime);
            }
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
            isPlayingBackwardsVirtual = false;
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

            // 循环播放
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
            EditorApplication.update -= OnEditorUpdate;
            drivenProperties.Dispose();
            DottEditorPreview.Completed -= DottEditorPreviewOnCompleted;
        }
    }
}
