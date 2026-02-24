using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class TiltedCardEffectController : IReplicaEffect<TiltedCardEffectConfig, TiltedCardEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private TiltedCardEffectConfig mConfig;
    private TiltedCardEffectView mView;

    private bool mInitialized;

    private float mTargetRotX;
    private float mTargetRotY;
    private float mCurrentRotX;
    private float mCurrentRotY;
    private float mVelRotX;
    private float mVelRotY;

    private float mTargetScale = 1f;
    private float mCurrentScale = 1f;
    private float mVelScale;

    private float mTargetTooltipRot;
    private float mCurrentTooltipRot;

    private float mOverlayTargetY;
    private float mOverlayCurrentY;

    private Vector2 mLastLocalPoint;
    private Vector2 mCurrentLocalPoint;
    private bool mPointerInside;

    public string EffectId => "tilted-card-v2";

    public void Initialize(ReplicaHostContext context, TiltedCardEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<TiltedCardEffectConfig>();
        mView = TiltedCardEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        ResetState();
        mInitialized = true;
    }

    public void SetModel(TiltedCardEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model ?? TiltedCardEffectModel.CreateDefault();
        if (mView.HintText != null)
        {
            mView.HintText.text = string.IsNullOrWhiteSpace(safe.Hint) ? "TiltedCard" : safe.Hint;
        }

        if (mView.BadgeText != null)
        {
            mView.BadgeText.text = string.IsNullOrWhiteSpace(safe.Badge) ? "HOVER TO TILT" : safe.Badge;
        }

        if (!animated)
        {
            mView.Group.alpha = 1f;
            mView.Content.anchoredPosition = new Vector2(0f, -22f);
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
        var basePos = new Vector2(0f, -22f);

        mView.Content.anchoredPosition = basePos + offset;
        mView.Group.alpha = 0f;

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Content, basePos, duration, mView.Root)
            .SetEase(transition.Ease));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Root, transition.Ease));
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
        var basePos = new Vector2(0f, -22f);

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Content, basePos + offset, duration, mView.Root)
            .SetEase(transition.Ease)
            .OnComplete(onComplete));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 0f, duration, mView.Root, transition.Ease));
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized)
        {
            return;
        }

        var dt = mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;
        UpdatePointerTargets(dt);
        UpdateSpring(dt);
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
        mPointerInside = false;
        mTargetRotX = 0f;
        mTargetRotY = 0f;
        mCurrentRotX = 0f;
        mCurrentRotY = 0f;
        mVelRotX = 0f;
        mVelRotY = 0f;
        mTargetScale = 1f;
        mCurrentScale = 1f;
        mVelScale = 0f;
        mTargetTooltipRot = 0f;
        mCurrentTooltipRot = 0f;
        mOverlayTargetY = 0f;
        mOverlayCurrentY = 0f;
        mLastLocalPoint = Vector2.zero;
        mCurrentLocalPoint = Vector2.zero;
        if (mView?.TooltipGroup != null)
        {
            mView.TooltipGroup.alpha = 0f;
        }
    }

    private void UpdatePointerTargets(float dt)
    {
        var inside = false;
        var localPoint = Vector2.zero;

        if (mContext.Pointer != null && mContext.Pointer.TryGetLocalPoint(mView.Card, out localPoint, mContext.UICamera))
        {
            var halfW = Mathf.Max(1f, mView.Card.rect.width * 0.5f);
            var halfH = Mathf.Max(1f, mView.Card.rect.height * 0.5f);
            inside = Mathf.Abs(localPoint.x) <= halfW && Mathf.Abs(localPoint.y) <= halfH;
        }

        mPointerInside = inside;
        mCurrentLocalPoint = localPoint;

        if (!inside)
        {
            mTargetRotX = 0f;
            mTargetRotY = 0f;
            mTargetScale = 1f;
            mTargetTooltipRot = 0f;
            mOverlayTargetY = 0f;
            return;
        }

        var half = mView.Card.rect.size * 0.5f;
        var normalizedX = Mathf.Clamp(localPoint.x / Mathf.Max(1f, half.x), -1f, 1f);
        var normalizedY = Mathf.Clamp(localPoint.y / Mathf.Max(1f, half.y), -1f, 1f);
        mTargetRotX = -normalizedY * mConfig.RotateAmplitude;
        mTargetRotY = normalizedX * mConfig.RotateAmplitude;
        mTargetScale = Mathf.Max(0.6f, mConfig.HoverScale);

        mOverlayTargetY = normalizedY * mConfig.OverlayParallax;

        var velocityY = localPoint.y - mLastLocalPoint.y;
        mTargetTooltipRot = Mathf.Clamp(
            -velocityY * mConfig.TooltipRotationVelocityScale,
            -Mathf.Abs(mConfig.TooltipRotationMax),
            Mathf.Abs(mConfig.TooltipRotationMax));

        mLastLocalPoint = localPoint;

        if (mView.TooltipText != null)
        {
            mView.TooltipText.text = $"x:{normalizedX:0.00} y:{normalizedY:0.00}";
        }
    }

    private void UpdateSpring(float dt)
    {
        SpringStep(ref mCurrentRotX, ref mVelRotX, mTargetRotX, mConfig.SpringStiffness, mConfig.SpringDamping, mConfig.SpringMass, dt);
        SpringStep(ref mCurrentRotY, ref mVelRotY, mTargetRotY, mConfig.SpringStiffness, mConfig.SpringDamping, mConfig.SpringMass, dt);
        SpringStep(ref mCurrentScale, ref mVelScale, mTargetScale, mConfig.SpringStiffness, mConfig.SpringDamping, mConfig.SpringMass, dt);

        var tooltipLerp = 1f - Mathf.Pow(1f - Mathf.Clamp01(mConfig.TooltipRotationLerp), Mathf.Max(1f, dt * 60f));
        mCurrentTooltipRot = Mathf.Lerp(mCurrentTooltipRot, mTargetTooltipRot, tooltipLerp);

        var overlayLerp = 1f - Mathf.Pow(1f - Mathf.Clamp01(mConfig.OverlayLerp), Mathf.Max(1f, dt * 60f));
        mOverlayCurrentY = Mathf.Lerp(mOverlayCurrentY, mOverlayTargetY, overlayLerp);
    }

    private void ApplyVisual(float dt)
    {
        if (mView.Card != null)
        {
            mView.Card.localRotation = Quaternion.Euler(mCurrentRotX, mCurrentRotY, 0f);
            mView.Card.localScale = Vector3.one * mCurrentScale;
        }

        if (mView.Glint != null)
        {
            var glintX = Mathf.InverseLerp(-mConfig.RotateAmplitude, mConfig.RotateAmplitude, mCurrentRotY);
            var glintY = Mathf.InverseLerp(-mConfig.RotateAmplitude, mConfig.RotateAmplitude, mCurrentRotX);
            mView.Glint.anchoredPosition = new Vector2((glintX - 0.5f) * 120f, (glintY - 0.5f) * -100f);
            mView.Glint.localRotation = Quaternion.Euler(0f, 0f, (glintX - 0.5f) * 24f);
        }

        if (mView.OverlayBand != null)
        {
            var overlayShiftX = mCurrentRotY * 0.4f;
            var overlayShiftY = mOverlayCurrentY + (mCurrentRotX * 0.3f);
            mView.OverlayBand.anchoredPosition = new Vector2(overlayShiftX, overlayShiftY);
        }

        if (mView.Tooltip != null)
        {
            if (mPointerInside)
            {
                mView.Tooltip.anchoredPosition = mCurrentLocalPoint + mConfig.TooltipOffset;
            }

            mView.Tooltip.localRotation = Quaternion.Euler(0f, 0f, mCurrentTooltipRot);
        }

        if (mView.TooltipGroup != null)
        {
            var alphaLerp = 1f - Mathf.Pow(1f - Mathf.Clamp01(mConfig.TooltipAlphaLerp), Mathf.Max(1f, dt * 60f));
            var target = mPointerInside ? 1f : 0f;
            mView.TooltipGroup.alpha = Mathf.Lerp(mView.TooltipGroup.alpha, target, alphaLerp);
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

    private static void SpringStep(ref float pos, ref float vel, float target, float stiffness, float damping, float mass, float dt)
    {
        var displacement = pos - target;
        var springForce = -stiffness * displacement;
        var dampForce = -damping * vel;
        var accel = (springForce + dampForce) / Mathf.Max(0.001f, mass);
        vel += accel * dt;
        pos += vel * dt;

        if (Mathf.Abs(displacement) < 0.001f && Mathf.Abs(vel) < 0.01f)
        {
            pos = target;
            vel = 0f;
        }
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
