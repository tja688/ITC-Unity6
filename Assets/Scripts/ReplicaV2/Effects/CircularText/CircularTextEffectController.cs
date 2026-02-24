using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class CircularTextEffectController : IReplicaEffect<CircularTextEffectConfig, CircularTextEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private CircularTextEffectConfig mConfig;
    private CircularTextEffectView mView;
    private CircularTextEffectModel mModel;

    private bool mInitialized;
    private float mBaseSpeed;
    private float mCurrentSpeed;
    private float mTargetSpeed;
    private float mCurrentScale = 1f;
    private float mTargetScale = 1f;
    private bool mHoverInside;

    public string EffectId => "circular-text-v2";

    public void Initialize(ReplicaHostContext context, CircularTextEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<CircularTextEffectConfig>();
        mView = CircularTextEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        SetModel(CircularTextEffectModel.CreateDefault(), false);
        mInitialized = true;
    }

    public void SetModel(CircularTextEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : CircularTextEffectModel.CreateDefault();
        mBaseSpeed = 360f / Mathf.Max(0.1f, mModel.SpinDuration);
        mCurrentSpeed = mBaseSpeed;
        mTargetSpeed = mBaseSpeed;
        mCurrentScale = 1f;
        mTargetScale = 1f;
        mHoverInside = false;

        CircularTextEffectViewBuilder.RebuildLetters(mView, mConfig, mModel.Text);

        if (!animated)
        {
            mView.Group.alpha = 1f;
            mView.Content.anchoredPosition = mConfig.RingHolderPosition;
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
        var basePos = mConfig.RingHolderPosition;

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
        var basePos = mConfig.RingHolderPosition;

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
        UpdateHover();

        var response = Mathf.Max(0.1f, mConfig.Response);
        var lerp = 1f - Mathf.Exp(-response * dt);
        mCurrentSpeed = Mathf.Lerp(mCurrentSpeed, mTargetSpeed, lerp);
        mCurrentScale = Mathf.Lerp(mCurrentScale, mTargetScale, lerp);

        if (mView.Ring != null)
        {
            mView.Ring.localRotation *= Quaternion.Euler(0f, 0f, mCurrentSpeed * dt);
            mView.Ring.localScale = Vector3.one * mCurrentScale;
        }
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

    private void UpdateHover()
    {
        var inside = false;
        if (mContext.Pointer != null && mView.RingHolder != null && mContext.Pointer.TryGetLocalPoint(mView.RingHolder, out var localPoint, mContext.UICamera))
        {
            var halfW = Mathf.Max(1f, mView.RingHolder.rect.width * 0.5f);
            var halfH = Mathf.Max(1f, mView.RingHolder.rect.height * 0.5f);
            inside = Mathf.Abs(localPoint.x) <= halfW && Mathf.Abs(localPoint.y) <= halfH;
        }

        if (inside == mHoverInside)
        {
            return;
        }

        mHoverInside = inside;
        if (inside)
        {
            mTargetSpeed = mBaseSpeed * Mathf.Max(1f, mConfig.HoverSpeedMultiplier);
            mTargetScale = Mathf.Clamp(mConfig.HoverScale, 0.6f, 1.1f);
        }
        else
        {
            mTargetSpeed = mBaseSpeed;
            mTargetScale = 1f;
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
