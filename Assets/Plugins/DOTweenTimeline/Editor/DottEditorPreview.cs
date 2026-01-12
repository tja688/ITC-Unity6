using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace Dott.Editor
{
    public static class DottEditorPreview
    {
        private struct TweenData
        {
            public readonly Tween Tween;
            public readonly bool IsFrom;

            public TweenData(Tween tween, bool isFrom)
            {
                Tween = tween;
                IsFrom = isFrom;
            }
        }

        private static readonly List<TweenData> Tweens = new();

        public static bool IsPlaying { get; private set; }

        /// <summary>
        /// 当前动画时间（用于 UI 显示）
        /// </summary>
        public static double CurrentTime { get; private set; }

        /// <summary>
        /// 上一帧的编辑器时间（用于计算 delta）
        /// </summary>
        private static double lastEditorTime;

        /// <summary>
        /// 当前是否在倒播
        /// </summary>
        private static bool isPlayingBackwards;

        public static event Action Completed;

        static DottEditorPreview()
        {
            if (!Application.isPlaying)
            {
                DOTween.useSafeMode = false;
            }
        }

        public static void Start()
        {
            if (IsPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            IsPlaying = true;
            lastEditorTime = EditorApplication.timeSinceStartup;
            EditorApplication.update += Update;
        }

        public static void Stop()
        {
            IsPlaying = false;
            isPlayingBackwards = false;
            EditorApplication.update -= Update;
            CurrentTime = 0;

            for (var i = Tweens.Count - 1; i >= 0; i--)
            {
                var tweenData = Tweens[i];
                var tween = tweenData.Tween;

                if (tweenData.IsFrom)
                {
                    // Yes, this is a hack to rewind multiple "from" tweens for the same target
                    tween.Rewind();
                    tween.Complete();
                }
                else
                {
                    tween.Rewind();
                }

                tween.Kill();
            }

            Tweens.Clear();

            QueuePlayerLoopUpdate();
        }

        /// <summary>
        /// 跳转到指定时间点。
        /// 注意：这个方法依赖于 tween 刚被创建（从0开始），
        /// 通过 ManualUpdate(absoluteTime) 让 tween 从0前进到指定位置。
        /// </summary>
        public static void GoTo(float time)
        {
            CurrentTime = time;
            // 这里传入 time 作为 delta，因为 tween 刚被创建（内部时间为0）
            // ManualUpdate 会让 tween 前进 time 秒，从而到达正确位置
            DOTween.ManualUpdate(time, time);
            QueuePlayerLoopUpdate();
        }

        public static void SetPlaybackDirection(bool playBackwards)
        {
            isPlayingBackwards = playBackwards;

            for (var i = 0; i < Tweens.Count; i++)
            {
                var tween = Tweens[i].Tween;
                if (tween == null || !tween.active)
                {
                    continue;
                }

                if (playBackwards)
                {
                    tween.PlayBackwards();
                }
                else
                {
                    tween.PlayForward();
                }
            }
        }

        public static void Add([NotNull] Tween tween, bool isFrom, bool allowCallbacks)
        {
            Tweens.Add(new TweenData(tween, isFrom));
            tween.SetUpdate(UpdateType.Manual);
            tween.SetAutoKill(false);
            if (!allowCallbacks)
            {
                tween.OnComplete(null).OnStart(null).OnPlay(null).OnPause(null).OnUpdate(null).OnWaypointChange(null).OnStepComplete(null).OnRewind(null).OnKill(null);
            }

            tween.Play();
        }

        public static void QueuePlayerLoopUpdate()
        {
            EditorApplication.QueuePlayerLoopUpdate();
        }

        private static void Update()
        {
            var currentEditorTime = EditorApplication.timeSinceStartup;
            var delta = currentEditorTime - lastEditorTime;
            lastEditorTime = currentEditorTime;

            // 更新 CurrentTime（用于 UI 显示 playhead）
            if (isPlayingBackwards)
            {
                CurrentTime -= delta;
                if (CurrentTime < 0)
                {
                    CurrentTime = 0;
                }
            }
            else
            {
                CurrentTime += delta;
            }

            // ManualUpdate 使用正的 delta
            // DOTween 内部会根据每个 tween 的播放方向（PlayBackwards/PlayForward）
            // 来决定是让 tween 时间前进还是后退
            DOTween.ManualUpdate((float)delta, (float)delta);
            QueuePlayerLoopUpdate();

            // 检查是否还有活跃的 tween
            var activeTweens = Tweens.Any(tweenData => tweenData.Tween.IsPlaying());
            if (!activeTweens)
            {
                Completed?.Invoke();
            }
        }
    }
}
