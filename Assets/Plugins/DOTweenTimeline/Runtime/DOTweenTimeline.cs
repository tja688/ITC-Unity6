using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;

namespace Dott
{
    [AddComponentMenu("DOTween/DOTween Timeline")]
    public class DOTweenTimeline : MonoBehaviour
    {
        [CanBeNull] public Sequence Sequence { get; private set; }

        #region Properties


        /// <summary>是否正在倒播</summary>
        public bool IsPlayingBackwards => Sequence != null && Sequence.IsActive() && Sequence.IsBackwards();


        /// <summary>是否正在播放（正向或倒播）</summary>
        public bool IsPlaying => Sequence != null && Sequence.IsActive() && Sequence.IsPlaying();


        /// <summary>当前播放进度时间</summary>
        public float ElapsedTime => Sequence?.Elapsed(includeLoops: false) ?? 0f;


        /// <summary>时间轴总时长</summary>
        public float Duration => Sequence?.Duration(includeLoops: false) ?? 0f;


        /// <summary>播放进度百分比 (0-1)</summary>
        public float ElapsedPercentage => Sequence?.ElapsedPercentage(includeLoops: false) ?? 0f;

        #endregion

        #region Playback Control


        /// <summary>正向播放时间轴</summary>
        public Sequence Play()
        {
            TryGenerateSequence();
            return Sequence.Play();
        }

        /// <summary>Wrapper for UnityEvent (requires void return type)</summary>
        public void DOPlay() => Play();

        /// <summary>倒播时间轴（从当前位置向起点播放）</summary>
        public Sequence PlayBackwards()
        {
            TryGenerateSequence();
            Sequence.PlayBackwards();
            return Sequence;
        }


        /// <summary>Wrapper for UnityEvent</summary>
        public void DOPlayBackwards() => PlayBackwards();

        /// <summary>正向播放时间轴（从当前位置向终点播放）</summary>
        public Sequence PlayForward()
        {
            TryGenerateSequence();
            Sequence.PlayForward();
            return Sequence;
        }


        /// <summary>Wrapper for UnityEvent</summary>
        public void DOPlayForward() => PlayForward();

        /// <summary>翻转播放方向（正变倒、倒变正）</summary>
        public Sequence Flip()
        {
            TryGenerateSequence();
            Sequence.Flip();
            return Sequence;
        }


        /// <summary>Wrapper for UnityEvent</summary>
        public void DOFlip() => Flip();

        /// <summary>暂停播放</summary>
        public Sequence Pause()
        {
            if (Sequence != null && Sequence.IsActive())
            {
                Sequence.Pause();
            }
            return Sequence;
        }


        /// <summary>Wrapper for UnityEvent</summary>
        public void DOPause() => Pause();

        /// <summary>重新开始播放（从起点正向）</summary>
        public Sequence Restart()
        {
            TryGenerateSequence();
            Sequence.Restart();
            return Sequence;
        }


        /// <summary>Wrapper for UnityEvent</summary>
        public void DORestart() => Restart();

        /// <summary>倒带到起点并暂停</summary>
        public Sequence Rewind()
        {
            if (Sequence != null && Sequence.IsActive())
            {
                Sequence.Rewind();
            }
            return Sequence;
        }


        /// <summary>Wrapper for UnityEvent</summary>
        public void DORewind() => Rewind();

        /// <summary>平滑倒带到起点（保持动画流畅）</summary>
        public Sequence SmoothRewind()
        {
            TryGenerateSequence();
            Sequence.SmoothRewind();
            return Sequence;
        }


        /// <summary>Wrapper for UnityEvent</summary>
        public void DOSmoothRewind() => SmoothRewind();

        /// <summary>快进到终点</summary>
        public Sequence Complete()
        {
            if (Sequence != null && Sequence.IsActive())
            {
                Sequence.Complete();
            }
            return Sequence;
        }


        /// <summary>Wrapper for UnityEvent</summary>
        public void DOComplete() => Complete();

        /// <summary>停止并销毁时间轴</summary>
        public void Kill()
        {
            if (Sequence != null && Sequence.IsActive())
            {
                Sequence.Kill();
            }
            Sequence = null;
        }


        /// <summary>Wrapper for UnityEvent</summary>
        public void DOKill() => Kill();

        #endregion

        #region Goto Methods


        /// <summary>跳转到指定时间点并暂停</summary>
        /// <param name="time">目标时间（秒）</param>
        public Sequence GotoAndPause(float time)
        {
            TryGenerateSequence();
            Sequence.Goto(time, andPlay: false);
            return Sequence;
        }

        /// <summary>跳转到指定时间点并播放</summary>
        /// <param name="time">目标时间（秒）</param>
        public Sequence GotoAndPlay(float time)
        {
            TryGenerateSequence();
            Sequence.Goto(time, andPlay: true);
            return Sequence;
        }

        /// <summary>跳转到指定进度（0-1）并暂停</summary>
        /// <param name="percentage">进度百分比 (0-1)</param>
        public Sequence GotoPercentageAndPause(float percentage)
        {
            TryGenerateSequence();
            var time = Sequence.Duration(includeLoops: false) * Mathf.Clamp01(percentage);
            Sequence.Goto(time, andPlay: false);
            return Sequence;
        }

        /// <summary>跳转到指定进度（0-1）并播放</summary>
        /// <param name="percentage">进度百分比 (0-1)</param>
        public Sequence GotoPercentageAndPlay(float percentage)
        {
            TryGenerateSequence();
            var time = Sequence.Duration(includeLoops: false) * Mathf.Clamp01(percentage);
            Sequence.Goto(time, andPlay: true);
            return Sequence;
        }

        #endregion

        #region Internal

        private void TryGenerateSequence()
        {
            if (Sequence != null) { return; }

            Sequence = DOTween.Sequence();
            Sequence.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            Sequence.OnKill(() => Sequence = null);
            var components = GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                switch (component)
                {
                    case DOTweenAnimation animation:
                        if (!animation.isValid || !animation.isActive) continue;

                        animation.CreateTween(regenerateIfExists: true);
                        Sequence.Insert(0, animation.tween);
                        break;

                    case IDOTweenAnimation animation:
                        if (!animation.IsValid || !animation.IsActive) continue;

                        var tween = animation.CreateTween(regenerateIfExists: true);
                        Sequence.Insert(0, tween);
                        break;
                }
            }
        }

        private void OnDestroy()
        {
            // Already handled by SetLink, but needed to avoid warnings from children DOTweenAnimation.OnDestroy
            Sequence?.Kill();
        }

        public void OnValidate()
        {
            foreach (var doTweenAnimation in GetComponents<DOTweenAnimation>())
            {
                doTweenAnimation.autoGenerate = false;
            }
        }

        #endregion
    }
}