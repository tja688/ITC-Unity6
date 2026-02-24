using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class SpotlightCardEffectController : IReplicaEffect<SpotlightCardEffectConfig, SpotlightCardEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private SpotlightCardEffectConfig mConfig;
    private SpotlightCardEffectView mView;

    private bool mInitialized;

    private bool mHovered;
    private Vector2 mTargetLocalPos;
    private Vector2 mCurrentLocalPos;

    private float mTargetAlpha;
    private float mCurrentAlpha;

    public string EffectId => "spotlight-card-v2";

    public void Initialize(ReplicaHostContext context, SpotlightCardEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<SpotlightCardEffectConfig>();
        mView = SpotlightCardEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        ResetState();
        mInitialized = true;
    }

    public void SetModel(SpotlightCardEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model ?? SpotlightCardEffectModel.CreateDefault();
        if (mView.HintText != null)
        {
            mView.HintText.text = string.IsNullOrWhiteSpace(safe.Hint) ? "SpotlightCard" : safe.Hint;
        }

        if (mView.TitleText != null)
        {
            mView.TitleText.text = string.IsNullOrWhiteSpace(safe.Title) ? "Interactive Spotlight Surface" : safe.Title;
        }

        if (mView.BodyText != null)
        {
            mView.BodyText.text = string.IsNullOrWhiteSpace(safe.Body) ? "Hover to reveal follow-light glow." : safe.Body;
        }

        if (!animated)
        {
            mView.Group.alpha = 1f;
            if (mView.Card != null)
            {
                mView.Card.anchoredPosition = mConfig.CardOffset;
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
        var basePos = mConfig.CardOffset;
        var offset = EnterOffset(transition.EnterDirection, mConfig.EnterOffset);

        if (mView.Card != null)
        {
            mView.Card.anchoredPosition = basePos + offset;
        }

        if (mView.Group != null)
        {
            mView.Group.alpha = 0f;
        }

        if (mView.Card != null)
        {
            mTransitionTweens.Add(mContext.Tweens.AnchoredPosTo(mView.Card, basePos, duration, mView.Root).SetEase(transition.Ease));
        }

        if (mView.Group != null)
        {
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
        var basePos = mConfig.CardOffset;
        var offset = ExitOffset(transition.ExitDirection, mConfig.ExitOffset);

        if (mView.Card != null)
        {
            mTransitionTweens.Add(
                mContext.Tweens
                    .AnchoredPosTo(mView.Card, basePos + offset, duration, mView.Root)
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

        var dt = mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;
        UpdatePointerTargets();
        UpdateFollow(dt);
        ApplyVisual(dt);
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    private void ResetState()
    {
        mHovered = false;
        mTargetLocalPos = Vector2.zero;
        mCurrentLocalPos = Vector2.zero;
        mTargetAlpha = 0f;
        mCurrentAlpha = 0f;

        if (mView?.SpotlightGroup != null)
        {
            mView.SpotlightGroup.alpha = 0f;
        }

        if (mView?.BorderImage != null)
        {
            mView.BorderImage.color = mConfig.BorderIdleColor;
        }
    }

    private void UpdatePointerTargets()
    {
        if (mView?.Card == null || mContext.Pointer == null || !mContext.Pointer.IsPointerValid)
        {
            mHovered = false;
            return;
        }

        if (!mContext.Pointer.TryGetLocalPoint(mView.Card, out var localPoint, mContext.UICamera))
        {
            mHovered = false;
            return;
        }

        var halfW = Mathf.Max(1f, mView.Card.rect.width * 0.5f);
        var halfH = Mathf.Max(1f, mView.Card.rect.height * 0.5f);
        var inside = Mathf.Abs(localPoint.x) <= halfW && Mathf.Abs(localPoint.y) <= halfH;

        mHovered = inside;
        if (inside)
        {
            mTargetLocalPos = localPoint;
        }
    }

    private void UpdateFollow(float dt)
    {
        if (mView?.Spotlight == null)
        {
            return;
        }

        var follow = 1f - Mathf.Pow(1f - Mathf.Clamp01(0.15f), Mathf.Max(1f, dt * 60f));
        if (mConfig.SpotlightFollowSpeed > 0f)
        {
            follow = 1f - Mathf.Exp(-mConfig.SpotlightFollowSpeed * dt);
        }

        mCurrentLocalPos = Vector2.Lerp(mCurrentLocalPos, mTargetLocalPos, Mathf.Clamp01(follow));
    }

    private void ApplyVisual(float dt)
    {
        if (mView?.Spotlight != null)
        {
            mView.Spotlight.anchoredPosition = mCurrentLocalPos;
        }

        mTargetAlpha = mHovered ? Mathf.Clamp01(mConfig.HoverSpotlightAlpha) : 0f;
        var alphaLerp = 1f - Mathf.Exp(-Mathf.Max(0.01f, mConfig.SpotlightAlphaFollowSpeed) * dt);
        mCurrentAlpha = Mathf.Lerp(mCurrentAlpha, mTargetAlpha, Mathf.Clamp01(alphaLerp));

        if (mView?.SpotlightGroup != null)
        {
            mView.SpotlightGroup.alpha = mCurrentAlpha;
        }

        if (mView?.BorderImage != null)
        {
            var targetBorderColor = mHovered ? mConfig.BorderHoverColor : mConfig.BorderIdleColor;
            var colorLerp = 1f - Mathf.Exp(-Mathf.Max(0.01f, mConfig.BorderColorFollowSpeed) * dt);
            mView.BorderImage.color = Color.Lerp(mView.BorderImage.color, targetBorderColor, Mathf.Clamp01(colorLerp));
        }
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

