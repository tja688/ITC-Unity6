using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ElasticOverflowSliderEffectController : IReplicaEffect<ElasticOverflowSliderEffectConfig, ElasticOverflowSliderEffectModel>
{
    private enum OverflowRegion
    {
        Left,
        Middle,
        Right
    }

    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();
    private readonly Vector3[] mTrackCorners = new Vector3[4];

    private ReplicaHostContext mContext;
    private ElasticOverflowSliderEffectConfig mConfig;
    private ElasticOverflowSliderEffectView mView;
    private bool mInitialized;

    private float mValue;
    private float mOverflow;
    private float mOverflowVelocity;
    private float mHoverScale;
    private float mTrackHeight;
    private float mLeftIconTargetX;
    private float mRightIconTargetX;
    private OverflowRegion mRegion;

    private Vector2 mBaseContentPos;

    public string EffectId => "elastic-overflow-slider-v2";

    public void Initialize(ReplicaHostContext context, ElasticOverflowSliderEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<ElasticOverflowSliderEffectConfig>();
        mView = ElasticOverflowSliderEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        mBaseContentPos = mView.Content.anchoredPosition;
        ResetState();
        mInitialized = true;
    }

    public void SetModel(ElasticOverflowSliderEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model != null ? model : ElasticOverflowSliderEffectModel.CreateDefault();
        if (mView.HintText != null)
        {
            mView.HintText.text = string.IsNullOrWhiteSpace(safe.Hint) ? "ElasticSlider  |  Drag and over-pull both sides" : safe.Hint;
        }

        mValue = Mathf.Clamp(safe.Value, mConfig.MinValue, mConfig.MaxValue);
        RefreshValueVisual();
        ApplyElasticVisual();
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

        mView.Content.anchoredPosition = mBaseContentPos + offset;
        mView.Group.alpha = 0f;

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Content, mBaseContentPos, duration, mView.Root)
            .SetEase(transition.Ease)
            .SetUpdate(mContext.UseUnscaledTime));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Root, transition.Ease).SetUpdate(mContext.UseUnscaledTime));
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

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Content, mBaseContentPos + offset, duration, mView.Root)
            .SetEase(transition.Ease)
            .SetUpdate(mContext.UseUnscaledTime)
            .OnComplete(onComplete));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 0f, duration, mView.Root, transition.Ease).SetUpdate(mContext.UseUnscaledTime));
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized)
        {
            return;
        }

        var dt = mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;

        if (mView.Input != null && !mView.Input.IsDragging)
        {
            var force = (-mConfig.OverflowSpring * mOverflow) - (mConfig.OverflowDamping * mOverflowVelocity);
            mOverflowVelocity += force * dt;
            mOverflow += mOverflowVelocity * dt;

            if (Mathf.Abs(mOverflow) < 0.1f && Mathf.Abs(mOverflowVelocity) < 0.5f)
            {
                mOverflow = 0f;
                mOverflowVelocity = 0f;
            }
        }

        var hoveringOrDragging = mView.Input != null && (mView.Input.IsHovering || mView.Input.IsDragging);
        var targetHoverScale = hoveringOrDragging ? mConfig.HoverScaleActive : mConfig.HoverScaleIdle;
        mHoverScale = SmoothTo(mHoverScale, targetHoverScale, mConfig.HoverScaleSpeed, dt);

        var targetHeight = hoveringOrDragging ? mConfig.TrackHeightHover : mConfig.TrackHeightIdle;
        mTrackHeight = SmoothTo(mTrackHeight, targetHeight, mConfig.TrackHeightSpeed, dt);

        UpdateFromPointerIfDragging();
        UpdateIconTargets(dt);
        ApplyElasticVisual();
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
        mOverflow = 0f;
        mOverflowVelocity = 0f;
        mHoverScale = mConfig.HoverScaleIdle;
        mTrackHeight = mConfig.TrackHeightIdle;
        mRegion = OverflowRegion.Middle;
        mLeftIconTargetX = 0f;
        mRightIconTargetX = 0f;
        mValue = Mathf.Clamp(mValue, mConfig.MinValue, mConfig.MaxValue);
    }

    private void UpdateFromPointerIfDragging()
    {
        if (mView.Input == null || !mView.Input.IsDragging)
        {
            return;
        }

        if (mContext.Pointer == null || !mContext.Pointer.IsPointerValid)
        {
            return;
        }

        if (mView.TrackRect == null)
        {
            return;
        }

        if (mContext.Pointer.TryGetLocalPoint(mView.TrackRect, out var localPoint, mContext.UICamera))
        {
            var width = Mathf.Max(1f, mView.TrackRect.rect.width);
            var normalized = Mathf.Clamp01((localPoint.x + (width * 0.5f)) / width);
            var newValue = Mathf.Lerp(mConfig.MinValue, mConfig.MaxValue, normalized);

            if (mConfig.Stepped)
            {
                var step = Mathf.Max(0.0001f, mConfig.StepSize);
                newValue = Mathf.Round(newValue / step) * step;
            }

            mValue = Mathf.Clamp(newValue, mConfig.MinValue, mConfig.MaxValue);
        }

        mView.TrackRect.GetWorldCorners(mTrackCorners);
        var left = mTrackCorners[0].x;
        var right = mTrackCorners[3].x;

        var screenX = mContext.Pointer.ScreenPosition.x;
        OverflowRegion nextRegion;
        float rawOverflow;
        if (screenX < left)
        {
            nextRegion = OverflowRegion.Left;
            rawOverflow = left - screenX;
        }
        else if (screenX > right)
        {
            nextRegion = OverflowRegion.Right;
            rawOverflow = screenX - right;
        }
        else
        {
            nextRegion = OverflowRegion.Middle;
            rawOverflow = 0f;
        }

        if (nextRegion != mRegion)
        {
            mRegion = nextRegion;
            PlayRegionPulse(nextRegion);
        }

        mOverflow = Decay(rawOverflow, mConfig.MaxOverflow);
        mOverflowVelocity = 0f;
        RefreshValueVisual();
    }

    private void UpdateIconTargets(float dt)
    {
        var overflowAbs = Mathf.Abs(mOverflow);
        float leftTarget;
        float rightTarget;

        if (mRegion == OverflowRegion.Left)
        {
            leftTarget = -(overflowAbs / Mathf.Max(1f, mHoverScale));
            rightTarget = 0f;
        }
        else if (mRegion == OverflowRegion.Right)
        {
            leftTarget = 0f;
            rightTarget = overflowAbs / Mathf.Max(1f, mHoverScale);
        }
        else
        {
            leftTarget = 0f;
            rightTarget = 0f;
        }

        mLeftIconTargetX = SmoothTo(mLeftIconTargetX, leftTarget, mConfig.IconPushSpeed, dt);
        mRightIconTargetX = SmoothTo(mRightIconTargetX, rightTarget, mConfig.IconPushSpeed, dt);
    }

    private void RefreshValueVisual()
    {
        var normalized = GetValueNormalized();

        if (mView.FillRect != null)
        {
            mView.FillRect.anchorMax = new Vector2(normalized, 1f);
        }

        if (mView.ValueText != null)
        {
            mView.ValueText.text = Mathf.RoundToInt(mValue).ToString();
        }

        if (mView.KnobRect != null)
        {
            mView.KnobRect.anchorMin = new Vector2(normalized, 0.5f);
            mView.KnobRect.anchorMax = new Vector2(normalized, 0.5f);
            mView.KnobRect.anchoredPosition = Vector2.zero;
        }
    }

    private void ApplyElasticVisual()
    {
        if (mView.TrackRect == null)
        {
            return;
        }

        var width = Mathf.Max(1f, mView.TrackRect.rect.width);
        var overflowAbs = Mathf.Abs(mOverflow);
        var overflow01 = Mathf.Clamp01(overflowAbs / Mathf.Max(1f, mConfig.MaxOverflow));

        mView.TrackRect.sizeDelta = new Vector2(0f, mTrackHeight);

        if (mView.TrackWrapper != null)
        {
            var scaleX = 1f + (overflowAbs / width);
            var scaleY = Mathf.Lerp(1f, 0.82f, overflow01);
            mView.TrackWrapper.localScale = new Vector3(scaleX, scaleY, 1f);

            switch (mRegion)
            {
                case OverflowRegion.Left:
                    mView.TrackWrapper.pivot = new Vector2(1f, 0.5f);
                    break;
                case OverflowRegion.Right:
                    mView.TrackWrapper.pivot = new Vector2(0f, 0.5f);
                    break;
                default:
                    mView.TrackWrapper.pivot = new Vector2(0.5f, 0.5f);
                    break;
            }
        }

        if (mView.LeftIconRect != null)
        {
            mView.LeftIconRect.anchoredPosition = new Vector2(mConfig.IconBaseOffset + mLeftIconTargetX, 0f);
        }

        if (mView.RightIconRect != null)
        {
            mView.RightIconRect.anchoredPosition = new Vector2(-mConfig.IconBaseOffset + mRightIconTargetX, 0f);
        }

        if (mView.TrackBackground != null)
        {
            var glow = Mathf.Lerp(mConfig.TrackColor.a, 0.46f, overflow01);
            var baseColor = mConfig.TrackColor;
            mView.TrackBackground.color = new Color(baseColor.r, baseColor.g, baseColor.b, glow);
        }

        if (mView.FillImage != null)
        {
            var baseColor = new Color(mConfig.FillColor.r, mConfig.FillColor.g, mConfig.FillColor.b, 0.86f);
            mView.FillImage.color = Color.Lerp(baseColor, Color.white, overflow01 * 0.5f);
        }

        if (mView.KnobImage != null)
        {
            mView.KnobImage.color = Color.Lerp(mConfig.KnobColor, Color.white, overflow01);
        }

        if (mView.KnobRect != null)
        {
            var knobSize = Mathf.Lerp(14f, 20f, Mathf.InverseLerp(mConfig.HoverScaleIdle, mConfig.HoverScaleActive, mHoverScale));
            mView.KnobRect.sizeDelta = new Vector2(knobSize, knobSize);
        }
    }

    private void PlayRegionPulse(OverflowRegion region)
    {
        if (region == OverflowRegion.Left && mView.LeftIconRect != null)
        {
            mContext.Tweens.Kill(mView.LeftIconRect);
            mContext.Tweens.PunchScale(mView.LeftIconRect, Vector3.one * 0.3f, 0.25f, 1, 0.5f, mView.LeftIconRect)
                .SetEase(ReplicaEase.OutCubic)
                .SetUpdate(mContext.UseUnscaledTime);
        }
        else if (region == OverflowRegion.Right && mView.RightIconRect != null)
        {
            mContext.Tweens.Kill(mView.RightIconRect);
            mContext.Tweens.PunchScale(mView.RightIconRect, Vector3.one * 0.3f, 0.25f, 1, 0.5f, mView.RightIconRect)
                .SetEase(ReplicaEase.OutCubic)
                .SetUpdate(mContext.UseUnscaledTime);
        }
    }

    private float GetValueNormalized()
    {
        var total = mConfig.MaxValue - mConfig.MinValue;
        if (Mathf.Approximately(total, 0f))
        {
            return 0f;
        }

        return Mathf.Clamp01((mValue - mConfig.MinValue) / total);
    }

    private static float Decay(float value, float max)
    {
        if (max <= 0f)
        {
            return 0f;
        }

        var entry = value / max;
        var sigmoid = 2f * ((1f / (1f + Mathf.Exp(-entry))) - 0.5f);
        return sigmoid * max;
    }

    private static float SmoothTo(float current, float target, float speed, float dt)
    {
        return Mathf.Lerp(current, target, 1f - Mathf.Exp(-Mathf.Max(0f, speed) * dt));
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
