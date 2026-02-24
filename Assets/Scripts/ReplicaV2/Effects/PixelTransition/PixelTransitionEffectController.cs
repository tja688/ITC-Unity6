using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class PixelTransitionEffectController : IReplicaEffect<PixelTransitionEffectConfig, PixelTransitionEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();
    private readonly List<IReplicaTweenHandle> mPixelTweens = new List<IReplicaTweenHandle>();
    private readonly List<int> mPixelOrder = new List<int>();

    private ReplicaHostContext mContext;
    private PixelTransitionEffectConfig mConfig;
    private PixelTransitionEffectView mView;
    private bool mIsActive;
    private bool mAnimating;
    private bool mInitialized;

    public string EffectId => "pixeltransition-v2";

    public void Initialize(ReplicaHostContext context, PixelTransitionEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<PixelTransitionEffectConfig>();
        mView = PixelTransitionEffectViewBuilder.Build(mContext.MountRoot, mConfig, this, PixelTransitionEffectModel.CreateDefault());
        SetActiveLayer(false);
        HideAllPixels();
        mInitialized = true;
    }

    public void SetModel(PixelTransitionEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        PixelTransitionEffectViewBuilder.ApplyModel(mView, model != null ? model : PixelTransitionEffectModel.CreateDefault());
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();

        var duration = ResolveDuration(transition, 0.5f);
        var offset = EnterOffset(transition.EnterDirection, mConfig.EnterOffset);

        mView.Card.anchoredPosition = offset;
        mView.Group.alpha = 0f;

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Card, Vector2.zero, duration, mView.Card)
            .SetEase(transition.Ease));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Card, transition.Ease));
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        if (!mInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        KillTransitionTweens();

        var duration = ResolveDuration(transition, 0.4f);
        var offset = ExitOffset(transition.ExitDirection, mConfig.ExitOffset);

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Card, offset, duration, mView.Card)
            .SetEase(transition.Ease)
            .OnComplete(onComplete));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 0f, duration, mView.Card, transition.Ease));
    }

    public void Tick(float deltaTime, float unscaledDeltaTime) { }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        KillPixelTweens();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mAnimating || mIsActive)
        {
            return;
        }

        AnimatePixels(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mAnimating || !mIsActive)
        {
            return;
        }

        if (mConfig.Once)
        {
            return;
        }

        AnimatePixels(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (mAnimating)
        {
            return;
        }

        if (!mIsActive)
        {
            AnimatePixels(true);
            return;
        }

        if (!mConfig.Once)
        {
            AnimatePixels(false);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (mAnimating || mIsActive)
        {
            return;
        }

        AnimatePixels(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (mAnimating || !mIsActive || mConfig.Once)
        {
            return;
        }

        AnimatePixels(false);
    }

    private void AnimatePixels(bool activate)
    {
        if (!mInitialized)
        {
            return;
        }

        mIsActive = activate;
        mAnimating = true;

        KillPixelTweens();
        HideAllPixels();

        var pixels = mView.Pixels;
        var total = pixels != null ? pixels.Count : 0;
        if (total <= 0)
        {
            SetActiveLayer(activate);
            mAnimating = false;
            return;
        }

        var stepDuration = Mathf.Max(0.01f, mConfig.AnimationStepDuration);
        var stagger = stepDuration / total;

        EnsureOrder(total);
        ShuffleOrder();

        for (var i = 0; i < total; i++)
        {
            var index = mPixelOrder[i];
            var delay = i * stagger;
            var handle = mContext.Tweens.DelayedCall(delay, () =>
            {
                if (index >= 0 && index < pixels.Count)
                {
                    pixels[index].gameObject.SetActive(true);
                }
            }, mView.Card);
            mPixelTweens.Add(handle);
        }

        mPixelTweens.Add(mContext.Tweens.DelayedCall(stepDuration, () =>
        {
            SetActiveLayer(activate);
        }, mView.Card));

        for (var i = 0; i < total; i++)
        {
            var index = mPixelOrder[i];
            var delay = stepDuration + (i * stagger);
            var handle = mContext.Tweens.DelayedCall(delay, () =>
            {
                if (index >= 0 && index < pixels.Count)
                {
                    pixels[index].gameObject.SetActive(false);
                }
            }, mView.Card);
            mPixelTweens.Add(handle);
        }

        mPixelTweens.Add(mContext.Tweens.DelayedCall(stepDuration + stepDuration, () =>
        {
            mAnimating = false;
        }, mView.Card));
    }

    private void EnsureOrder(int total)
    {
        if (mPixelOrder.Count == total)
        {
            return;
        }

        mPixelOrder.Clear();
        for (var i = 0; i < total; i++)
        {
            mPixelOrder.Add(i);
        }
    }

    private void ShuffleOrder()
    {
        var rng = new System.Random(Environment.TickCount);
        for (var i = mPixelOrder.Count - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (mPixelOrder[i], mPixelOrder[j]) = (mPixelOrder[j], mPixelOrder[i]);
        }
    }

    private void HideAllPixels()
    {
        var pixels = mView != null ? mView.Pixels : null;
        if (pixels == null)
        {
            return;
        }

        for (var i = 0; i < pixels.Count; i++)
        {
            if (pixels[i] != null)
            {
                pixels[i].gameObject.SetActive(false);
            }
        }
    }

    private void SetActiveLayer(bool active)
    {
        if (mView == null)
        {
            return;
        }

        if (mView.DefaultLayer != null)
        {
            mView.DefaultLayer.gameObject.SetActive(!active);
        }

        if (mView.ActiveLayer != null)
        {
            mView.ActiveLayer.gameObject.SetActive(active);
        }
    }

    private IReplicaTweenHandle FadeCanvasGroup(UnityEngine.CanvasGroup group, float targetAlpha, float duration, UnityEngine.Object owner, ReplicaEase ease)
    {
        var alpha = group.alpha;
        return mContext.Tweens
            .ToFloat(
                () => alpha,
                value =>
                {
                    alpha = value;
                    group.alpha = value;
                },
                targetAlpha,
                duration,
                owner)
            .SetEase(ease);
    }

    private void KillTransitionTweens()
    {
        for (var i = 0; i < mTransitionTweens.Count; i++)
        {
            mTransitionTweens[i]?.Kill();
        }

        mTransitionTweens.Clear();
    }

    private void KillPixelTweens()
    {
        for (var i = 0; i < mPixelTweens.Count; i++)
        {
            mPixelTweens[i]?.Kill();
        }

        mPixelTweens.Clear();
        mAnimating = false;
    }

    private static float ResolveDuration(ReplicaTransition transition, float fallback)
    {
        var duration = transition.Duration > 0f ? transition.Duration : fallback;
        return Mathf.Max(0.05f, duration);
    }

    private static Vector2 EnterOffset(ReplicaEnterDirection direction, float distance)
    {
        switch (direction)
        {
            case ReplicaEnterDirection.FromTop: return new Vector2(0f, distance);
            case ReplicaEnterDirection.FromBottom: return new Vector2(0f, -distance);
            case ReplicaEnterDirection.FromLeft: return new Vector2(-distance, 0f);
            case ReplicaEnterDirection.FromRight: return new Vector2(distance, 0f);
            default: return Vector2.zero;
        }
    }

    private static Vector2 ExitOffset(ReplicaExitDirection direction, float distance)
    {
        switch (direction)
        {
            case ReplicaExitDirection.ToTop: return new Vector2(0f, distance);
            case ReplicaExitDirection.ToBottom: return new Vector2(0f, -distance);
            case ReplicaExitDirection.ToLeft: return new Vector2(-distance, 0f);
            case ReplicaExitDirection.ToRight: return new Vector2(distance, 0f);
            case ReplicaExitDirection.FadeOnly:
            default:
                return Vector2.zero;
        }
    }
}
