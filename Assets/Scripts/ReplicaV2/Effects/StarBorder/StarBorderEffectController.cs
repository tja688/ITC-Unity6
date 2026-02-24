using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class StarBorderEffectController : IReplicaEffect<StarBorderEffectConfig, StarBorderEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private StarBorderEffectConfig mConfig;
    private StarBorderEffectView mView;

    private bool mInitialized;

    private IReplicaTweenHandle mTopPosTween;
    private IReplicaTweenHandle mTopFadeTween;
    private IReplicaTweenHandle mBottomPosTween;
    private IReplicaTweenHandle mBottomFadeTween;

    private float mTopBaseY;
    private float mBottomBaseY;

    public string EffectId => "starborder-v2";

    public void Initialize(ReplicaHostContext context, StarBorderEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<StarBorderEffectConfig>();
        mView = StarBorderEffectViewBuilder.Build(mContext.MountRoot, mConfig);

        if (mView.GlowTop != null)
        {
            mTopBaseY = mView.GlowTop.anchoredPosition.y;
        }

        if (mView.GlowBottom != null)
        {
            mBottomBaseY = mView.GlowBottom.anchoredPosition.y;
        }

        ApplyModel(StarBorderEffectModel.CreateDefault());
        mInitialized = true;
        StartLoops();
    }

    public void SetModel(StarBorderEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        ApplyModel(model ?? StarBorderEffectModel.CreateDefault());

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
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        StopLoops();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    private void ApplyModel(StarBorderEffectModel model)
    {
        var safe = model ?? StarBorderEffectModel.CreateDefault();
        if (mView.LabelText != null)
        {
            mView.LabelText.text = string.IsNullOrWhiteSpace(safe.Label) ? "StarBorder" : safe.Label;
        }
    }

    private void StartLoops()
    {
        StartTopCycle(true);
        StartBottomCycle(true);
    }

    private void StopLoops()
    {
        mTopPosTween?.Kill();
        mTopFadeTween?.Kill();
        mBottomPosTween?.Kill();
        mBottomFadeTween?.Kill();
        mTopPosTween = null;
        mTopFadeTween = null;
        mBottomPosTween = null;
        mBottomFadeTween = null;
    }

    private void StartTopCycle(bool leftToRight)
    {
        if (!mInitialized || mView?.GlowTop == null || mView.GlowTopImage == null)
        {
            return;
        }

        mTopPosTween?.Kill();
        mTopFadeTween?.Kill();

        var span = Mathf.Max(50f, mConfig.ContainerSize.x * 1.25f);
        var startX = leftToRight ? -span : span;
        var endX = leftToRight ? span : -span;

        mView.GlowTop.anchoredPosition = new Vector2(startX, mTopBaseY);
        SetGlowAlpha(mView.GlowTopImage, Mathf.Clamp01(mConfig.GlowOpacity));

        var duration = Mathf.Max(0.05f, mConfig.SpeedSeconds);
        mTopPosTween = mContext.Tweens
            .AnchoredPosXTo(mView.GlowTop, endX, duration, mView.GlowTop)
            .SetEase(ReplicaEase.Linear)
            .OnComplete(() => StartTopCycle(!leftToRight));

        mTopFadeTween = mContext.Tweens
            .FadeImageTo(mView.GlowTopImage, 0f, duration, mView.GlowTopImage)
            .SetEase(ReplicaEase.Linear);
    }

    private void StartBottomCycle(bool rightToLeft)
    {
        if (!mInitialized || mView?.GlowBottom == null || mView.GlowBottomImage == null)
        {
            return;
        }

        mBottomPosTween?.Kill();
        mBottomFadeTween?.Kill();

        var span = Mathf.Max(50f, mConfig.ContainerSize.x * 1.25f);
        var startX = rightToLeft ? span : -span;
        var endX = rightToLeft ? -span : span;

        mView.GlowBottom.anchoredPosition = new Vector2(startX, mBottomBaseY);
        SetGlowAlpha(mView.GlowBottomImage, Mathf.Clamp01(mConfig.GlowOpacity));

        var duration = Mathf.Max(0.05f, mConfig.SpeedSeconds);
        mBottomPosTween = mContext.Tweens
            .AnchoredPosXTo(mView.GlowBottom, endX, duration, mView.GlowBottom)
            .SetEase(ReplicaEase.Linear)
            .OnComplete(() => StartBottomCycle(!rightToLeft));

        mBottomFadeTween = mContext.Tweens
            .FadeImageTo(mView.GlowBottomImage, 0f, duration, mView.GlowBottomImage)
            .SetEase(ReplicaEase.Linear);
    }

    private void SetGlowAlpha(Image image, float alpha)
    {
        if (image == null)
        {
            return;
        }

        image.color = new Color(mConfig.GlowColor.r, mConfig.GlowColor.g, mConfig.GlowColor.b, alpha);
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
