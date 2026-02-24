using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ReflectiveCardEffectController : IReplicaEffect<ReflectiveCardEffectConfig, ReflectiveCardEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private ReflectiveCardEffectConfig mConfig;
    private ReflectiveCardEffectView mView;

    private bool mInitialized;

    private Vector2 mCurrentTilt;
    private Vector2 mTargetTilt;
    private Vector2 mCurrentOffset;
    private Vector2 mTargetOffset;
    private Vector2 mLastPointerPos;
    private float mMotionEnergy;
    private float mHoverBlend;
    private Vector2 mNormalized;
    private float mClock;

    public string EffectId => "reflective-card-v2";

    public void Initialize(ReplicaHostContext context, ReflectiveCardEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<ReflectiveCardEffectConfig>();
        mView = ReflectiveCardEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        mLastPointerPos = mContext.Pointer != null ? mContext.Pointer.ScreenPosition : Vector2.zero;
        mInitialized = true;
    }

    public void SetModel(ReflectiveCardEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model ?? ReflectiveCardEffectModel.CreateDefault();

        if (mView.HintText != null)
        {
            mView.HintText.text = string.IsNullOrWhiteSpace(safe.Hint) ? "ReflectiveCard" : safe.Hint;
        }

        if (mView.UserNameText != null)
        {
            mView.UserNameText.text = string.IsNullOrWhiteSpace(safe.UserName) ? "ALEXANDER DOE" : safe.UserName;
        }

        if (mView.RoleText != null)
        {
            mView.RoleText.text = string.IsNullOrWhiteSpace(safe.Role) ? "SENIOR DEVELOPER" : safe.Role;
        }

        if (mView.IdNumberText != null)
        {
            mView.IdNumberText.text = string.IsNullOrWhiteSpace(safe.IdNumber) ? "8901-2345-6789" : safe.IdNumber;
        }

        if (mView.BadgeText != null)
        {
            mView.BadgeText.text = string.IsNullOrWhiteSpace(safe.Badge) ? "\u25CF  SECURE ACCESS" : safe.Badge;
        }

        if (!animated)
        {
            mView.Group.alpha = 1f;
            mView.Content.anchoredPosition = new Vector2(0f, -20f);
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
        var basePos = new Vector2(0f, -20f);

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
        var basePos = new Vector2(0f, -20f);

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
        mClock += dt;
        UpdateHoverBlend(dt);
        UpdatePointerNormalized();
        UpdateMotionEnergy(dt);
        UpdateCardTransform(dt);
        UpdateSheenAndSpotlight(dt);
        UpdateNoise();
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

    private void UpdateHoverBlend(float dt)
    {
        var inside = false;
        if (mContext.Pointer != null && mContext.Pointer.TryGetLocalPoint(mView.Card, out var local, mContext.UICamera))
        {
            var halfW = Mathf.Max(1f, mView.Card.rect.width * 0.5f);
            var halfH = Mathf.Max(1f, mView.Card.rect.height * 0.5f);
            inside = Mathf.Abs(local.x) <= halfW && Mathf.Abs(local.y) <= halfH;
        }

        var target = inside ? 1f : 0f;
        mHoverBlend = Mathf.Lerp(mHoverBlend, target, dt * Mathf.Max(0.1f, mConfig.HoverBlendSpeed));
    }

    private void UpdatePointerNormalized()
    {
        if (mContext.Pointer == null || mView.Card == null)
        {
            mNormalized = Vector2.zero;
            return;
        }

        if (!mContext.Pointer.TryGetLocalPoint(mView.Card, out var localPos, mContext.UICamera))
        {
            mNormalized = Vector2.zero;
            return;
        }

        var halfW = Mathf.Max(1f, mView.Card.rect.width * 0.5f);
        var halfH = Mathf.Max(1f, mView.Card.rect.height * 0.5f);
        mNormalized = new Vector2(
            Mathf.Clamp(localPos.x / halfW, -1f, 1f),
            Mathf.Clamp(localPos.y / halfH, -1f, 1f));
    }

    private void UpdateMotionEnergy(float dt)
    {
        if (mContext.Pointer == null)
        {
            return;
        }

        var pointer = mContext.Pointer.ScreenPosition;
        var speed = Vector2.Distance(pointer, mLastPointerPos) / 60f;
        mMotionEnergy = Mathf.Lerp(mMotionEnergy, Mathf.Clamp01(speed), dt * Mathf.Max(0.1f, mConfig.MotionEnergySmooth));
        mLastPointerPos = pointer;
    }

    private void UpdateCardTransform(float dt)
    {
        if (mView.Card == null)
        {
            return;
        }

        mTargetTilt = new Vector2(
            -mNormalized.y * mConfig.TiltStrength,
            mNormalized.x * mConfig.TiltStrength) * mHoverBlend;

        mTargetOffset = new Vector2(
            mNormalized.x * mConfig.ParallaxStrength,
            mNormalized.y * (mConfig.ParallaxStrength * 0.7f)) * mHoverBlend;

        var tiltSmooth = Mathf.Max(0.1f, mConfig.TiltSmooth);
        mCurrentTilt = Vector2.Lerp(mCurrentTilt, mTargetTilt, dt * tiltSmooth);
        mCurrentOffset = Vector2.Lerp(mCurrentOffset, mTargetOffset, dt * (tiltSmooth * 0.8f));

        mView.Card.localRotation = Quaternion.Euler(mCurrentTilt.x, mCurrentTilt.y, -mNormalized.x * 1.5f * mHoverBlend);
        mView.Card.anchoredPosition = mCurrentOffset;
    }

    private void UpdateSheenAndSpotlight(float dt)
    {
        if (mView.Sheen == null || mView.Spotlight == null)
        {
            return;
        }

        var autoSweep = Mathf.Sin(mClock * mConfig.SheenAutoSweepSpeed) * mConfig.SheenAutoSweepAmplitude;
        var mouseInfluence = mNormalized.x * mConfig.SheenMouseInfluence;
        var sheenX = (autoSweep + mouseInfluence) * mHoverBlend;
        mView.Sheen.anchoredPosition = new Vector2(sheenX, 0f);

        var sheenAlphaTarget = mHoverBlend * Mathf.Lerp(0.4f, 1f, mMotionEnergy);
        mView.SheenGroup.alpha = Mathf.Lerp(mView.SheenGroup.alpha, sheenAlphaTarget, dt * Mathf.Max(0.1f, mConfig.SheenFadeSpeed));
        mView.SheenImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.15f, 0.35f, mMotionEnergy));

        var spotTarget = new Vector2(mNormalized.x * mConfig.SpotlightX, mNormalized.y * mConfig.SpotlightY) * mHoverBlend;
        mView.Spotlight.anchoredPosition = Vector2.Lerp(mView.Spotlight.anchoredPosition, spotTarget, dt * Mathf.Max(0.1f, mConfig.SpotlightMoveSpeed));
        var spotAlphaTarget = mHoverBlend * Mathf.Lerp(0.2f, 0.6f, mMotionEnergy);
        mView.SpotlightGroup.alpha = Mathf.Lerp(mView.SpotlightGroup.alpha, spotAlphaTarget, dt * Mathf.Max(0.1f, mConfig.SpotlightFadeSpeed));

        var cardBlend = mHoverBlend * Mathf.Lerp(0.3f, 1f, mMotionEnergy);
        mView.CardImage.color = Color.Lerp(mConfig.CardRestColor, mConfig.CardActiveColor, cardBlend);
    }

    private void UpdateNoise()
    {
        var effectStrength = mHoverBlend;
        for (var i = 0; i < mView.NoiseStrips.Count; i++)
        {
            var strip = mView.NoiseStrips[i];
            var wave = Mathf.Sin((mClock * mConfig.NoiseWaveSpeed) + (i * 0.52f));
            var direction = (i % 2 == 0) ? 1f : -1f;
            var offset = direction * wave * (2f + (16f * mMotionEnergy)) * effectStrength;
            strip.anchoredPosition = new Vector2(offset, mView.NoiseBaseY[i]);

            var alphaWave = 0.55f + (0.45f * Mathf.Sin((mClock * mConfig.NoiseAlphaSpeed) + (i * 0.73f)));
            var color = mView.NoiseImages[i].color;
            color.a = Mathf.Lerp(0.005f, 0.12f, mMotionEnergy * effectStrength) * alphaWave;
            mView.NoiseImages[i].color = color;
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

