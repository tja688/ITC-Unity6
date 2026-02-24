using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GlareHoverEffectController : IReplicaEffect<GlareHoverEffectConfig, GlareHoverEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private GlareHoverEffectConfig mConfig;
    private GlareHoverEffectView mView;

    private bool mInitialized;
    private bool mHovered;

    private Vector2 mStartGlarePos;
    private Vector2 mEndGlarePos;

    private IReplicaTweenHandle mGlareTween;

    public string EffectId => "glarehover-v2";

    public void Initialize(ReplicaHostContext context, GlareHoverEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<GlareHoverEffectConfig>();
        mView = GlareHoverEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        RecomputeGlarePath();
        ResetVisual();
        mInitialized = true;
    }

    public void SetModel(GlareHoverEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model ?? GlareHoverEffectModel.CreateDefault();
        if (mView.LabelText != null)
        {
            mView.LabelText.text = string.IsNullOrWhiteSpace(safe.Label) ? "GlareHover" : safe.Label;
        }

        if (!animated)
        {
            if (mView.Group != null)
            {
                mView.Group.alpha = 1f;
            }

            if (mView.Container != null)
            {
                mView.Container.anchoredPosition = Vector2.zero;
            }
        }
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var offset = EnterOffset(transition.EnterDirection, mConfig.EnterOffset);

        if (mView.Container != null)
        {
            mView.Container.anchoredPosition = offset;
            mTransitionTweens.Add(mContext.Tweens.AnchoredPosTo(mView.Container, Vector2.zero, duration, mView.Root).SetEase(transition.Ease));
        }

        if (mView.Group != null)
        {
            mView.Group.alpha = 0f;
            mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Root, transition.Ease));
        }
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        if (!mInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        KillTransitionTweens();

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var offset = ExitOffset(transition.ExitDirection, mConfig.ExitOffset);

        if (mView.Container != null)
        {
            mTransitionTweens.Add(
                mContext.Tweens
                    .AnchoredPosTo(mView.Container, offset, duration, mView.Root)
                    .SetEase(transition.Ease)
                    .OnComplete(onComplete));
        }
        else
        {
            onComplete?.Invoke();
        }

        if (mView.Group != null)
        {
            mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 0f, duration, mView.Root, transition.Ease));
        }
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized)
        {
            return;
        }

        UpdateHover();
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        KillGlareTween();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    private void ResetVisual()
    {
        mHovered = false;
        if (mView.ContainerOutline != null)
        {
            mView.ContainerOutline.effectColor = mConfig.BorderColor;
        }

        if (mView.Glare != null)
        {
            mView.Glare.anchoredPosition = mStartGlarePos;
        }
    }

    private void UpdateHover()
    {
        var hovered = false;

        if (mView?.Container != null && mContext.Pointer != null && mContext.Pointer.IsPointerValid)
        {
            if (mContext.Pointer.TryGetLocalPoint(mView.Container, out var localPoint, mContext.UICamera))
            {
                var halfW = Mathf.Max(1f, mView.Container.rect.width * 0.5f);
                var halfH = Mathf.Max(1f, mView.Container.rect.height * 0.5f);
                hovered = Mathf.Abs(localPoint.x) <= halfW && Mathf.Abs(localPoint.y) <= halfH;
            }
        }

        if (hovered == mHovered)
        {
            return;
        }

        mHovered = hovered;
        if (hovered)
        {
            PlayGlareSweep();
        }
        else
        {
            ResetGlare();
        }
    }

    private void PlayGlareSweep()
    {
        if (mView?.Glare == null)
        {
            return;
        }

        RecomputeGlarePath();

        KillGlareTween();
        mView.Glare.anchoredPosition = mStartGlarePos;

        var duration = Mathf.Max(0.01f, mConfig.TransitionDuration);
        mGlareTween = mContext.Tweens
            .AnchoredPosTo(mView.Glare, mEndGlarePos, duration, mView.Glare)
            .SetEase(mConfig.TransitionEase);
    }

    private void ResetGlare()
    {
        if (mView?.Glare == null)
        {
            return;
        }

        RecomputeGlarePath();
        KillGlareTween();

        if (mConfig.PlayOnce)
        {
            mView.Glare.anchoredPosition = mStartGlarePos;
            return;
        }

        var duration = Mathf.Max(0.01f, mConfig.TransitionDuration);
        mGlareTween = mContext.Tweens
            .AnchoredPosTo(mView.Glare, mStartGlarePos, duration, mView.Glare)
            .SetEase(ReplicaEase.OutCubic);
    }

    private void RecomputeGlarePath()
    {
        if (mView?.Container == null)
        {
            mStartGlarePos = Vector2.zero;
            mEndGlarePos = Vector2.zero;
            return;
        }

        var sizePercent = Mathf.Max(100f, mConfig.GlareSizePercent) / 100f;
        var glareSize = Mathf.Max(mConfig.ContainerSize.x, mConfig.ContainerSize.y) * sizePercent;
        var travel = Mathf.Max(10f, glareSize * 0.5f);
        mStartGlarePos = new Vector2(-travel, -travel);
        mEndGlarePos = new Vector2(travel, travel);
    }

    private IReplicaTweenHandle FadeCanvasGroup(CanvasGroup group, float targetAlpha, float duration, UnityEngine.Object owner, ReplicaEase ease)
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

    private void KillGlareTween()
    {
        mGlareTween?.Kill();
        mGlareTween = null;
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
