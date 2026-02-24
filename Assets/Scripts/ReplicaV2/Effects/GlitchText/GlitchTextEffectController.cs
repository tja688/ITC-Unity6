using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public sealed class GlitchTextEffectController : IReplicaEffect<GlitchTextEffectConfig, GlitchTextEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private GlitchTextEffectConfig mConfig;
    private GlitchTextEffectView mView;
    private GlitchTextEffectModel mModel;

    private bool mInitialized;
    private bool mGlitchActive;
    private bool mLastHoverInside;
    private float mTick;
    private IReplicaTweenHandle mMainPunch;

    public string EffectId => "glitch-text-v2";

    public void Initialize(ReplicaHostContext context, GlitchTextEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<GlitchTextEffectConfig>();
        mView = GlitchTextEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        SetModel(GlitchTextEffectModel.CreateDefault(), false);
        mInitialized = true;
    }

    public void SetModel(GlitchTextEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : GlitchTextEffectModel.CreateDefault();
        GlitchTextEffectViewBuilder.ApplyModel(mView, mConfig, mModel);

        mGlitchActive = !mModel.EnableOnHover;
        mLastHoverInside = false;
        mTick = 0f;

        ResetSlices();

        if (!animated)
        {
            mView.Group.alpha = 1f;
            mView.Content.anchoredPosition = mConfig.BackdropPosition;
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
        var basePos = mConfig.BackdropPosition;

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
        var basePos = mConfig.BackdropPosition;

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

        UpdateHoverState();
        if (!mGlitchActive)
        {
            return;
        }

        mTick += dt;
        var speed = Mathf.Max(0.01f, mModel != null ? mModel.Speed : 1f);
        var step = Mathf.Max(mConfig.StepMin, mConfig.StepBase * speed);
        if (mTick < step)
        {
            return;
        }

        mTick = 0f;
        GlitchStep();
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        mMainPunch?.Kill();
        mMainPunch = null;

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    private void UpdateHoverState()
    {
        if (mModel == null || !mModel.EnableOnHover)
        {
            mGlitchActive = true;
            return;
        }

        var inside = false;
        if (mContext.Pointer != null && mView.Container != null && mContext.Pointer.TryGetLocalPoint(mView.Container, out var localPoint, mContext.UICamera))
        {
            var halfW = Mathf.Max(1f, mView.Container.rect.width * 0.5f);
            var halfH = Mathf.Max(1f, mView.Container.rect.height * 0.5f);
            inside = Mathf.Abs(localPoint.x) <= halfW && Mathf.Abs(localPoint.y) <= halfH;
        }

        if (inside == mLastHoverInside)
        {
            mGlitchActive = inside;
            return;
        }

        mLastHoverInside = inside;
        mGlitchActive = inside;
        mTick = 0f;

        if (!inside)
        {
            ResetSlices();
        }
    }

    private void GlitchStep()
    {
        if (mView.MainRect == null)
        {
            return;
        }

        var height = mView.MainRect.rect.height;
        var half = height * 0.5f;

        var afterBand = Random.Range(mConfig.BandHeightRange.x, mConfig.BandHeightRange.y);
        var afterY = Random.Range(-half + mConfig.BandEdgePadding, half - mConfig.BandEdgePadding);
        GlitchTextEffectViewBuilder.SetBand(mView.AfterMask, afterBand, afterY);
        if (mView.AfterTextRect != null)
        {
            mView.AfterTextRect.anchoredPosition = new Vector2(
                mConfig.AfterBaseX + Random.Range(mConfig.SliceOffsetXRange.x, mConfig.SliceOffsetXRange.y),
                Random.Range(mConfig.SliceOffsetYRange.x, mConfig.SliceOffsetYRange.y));
        }

        if (mView.AfterText != null)
        {
            var a = Random.Range(mConfig.SliceAlphaRange.x, mConfig.SliceAlphaRange.y);
            mView.AfterText.color = new Color(1f, 1f, 1f, a);
        }

        var beforeBand = Random.Range(mConfig.BandHeightRange.x, mConfig.BandHeightRange.y);
        var beforeY = Random.Range(-half + mConfig.BandEdgePadding, half - mConfig.BandEdgePadding);
        GlitchTextEffectViewBuilder.SetBand(mView.BeforeMask, beforeBand, beforeY);
        if (mView.BeforeTextRect != null)
        {
            mView.BeforeTextRect.anchoredPosition = new Vector2(
                mConfig.BeforeBaseX + Random.Range(mConfig.SliceOffsetXRange.x, mConfig.SliceOffsetXRange.y),
                Random.Range(mConfig.SliceOffsetYRange.x, mConfig.SliceOffsetYRange.y));
        }

        if (mView.BeforeText != null)
        {
            var a = Random.Range(mConfig.SliceAlphaRange.x, mConfig.SliceAlphaRange.y);
            mView.BeforeText.color = new Color(1f, 1f, 1f, a);
        }

        mView.MainRect.anchoredPosition = new Vector2(
            Random.Range(mConfig.MainJitterRange.x, mConfig.MainJitterRange.y),
            Random.Range(mConfig.MainJitterRange.x, mConfig.MainJitterRange.y));

        mMainPunch?.Kill();
        mMainPunch = mContext.Tweens.PunchScale(
            mView.MainRect,
            mConfig.PunchScale,
            Mathf.Max(0.01f, mConfig.PunchDuration),
            Mathf.Max(1, mConfig.PunchVibrato),
            Mathf.Clamp01(mConfig.PunchElasticity),
            mView.MainRect);
    }

    private void ResetSlices()
    {
        if (mView.MainRect != null)
        {
            mView.MainRect.anchoredPosition = Vector2.zero;
            mView.MainRect.localScale = Vector3.one;
        }

        if (mView.AfterTextRect != null)
        {
            mView.AfterTextRect.anchoredPosition = new Vector2(mConfig.AfterBaseX, 0f);
        }

        if (mView.BeforeTextRect != null)
        {
            mView.BeforeTextRect.anchoredPosition = new Vector2(mConfig.BeforeBaseX, 0f);
        }

        if (mView.AfterText != null)
        {
            mView.AfterText.color = Color.white;
        }

        if (mView.BeforeText != null)
        {
            mView.BeforeText.color = Color.white;
        }

        if (mModel != null && mModel.EnableOnHover)
        {
            GlitchTextEffectViewBuilder.SetBand(mView.AfterMask, 0f, 0f);
            GlitchTextEffectViewBuilder.SetBand(mView.BeforeMask, 0f, 0f);
        }
        else
        {
            GlitchTextEffectViewBuilder.SetBand(mView.AfterMask, 68f, 30f);
            GlitchTextEffectViewBuilder.SetBand(mView.BeforeMask, 58f, -22f);
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
