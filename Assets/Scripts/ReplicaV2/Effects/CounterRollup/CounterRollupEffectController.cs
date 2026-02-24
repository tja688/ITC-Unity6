using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class CounterRollupEffectController : IReplicaEffect<CounterRollupEffectConfig, CounterRollupEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private CounterRollupEffectConfig mConfig;
    private CounterRollupEffectView mView;
    private CounterRollupEffectModel mModel;

    private bool mInitialized;
    private float mDisplayedValue;
    private float mTargetTimer;
    private IReplicaTweenHandle mTween;

    public string EffectId => "counter-rollup-v2";

    public void Initialize(ReplicaHostContext context, CounterRollupEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<CounterRollupEffectConfig>();
        mView = CounterRollupEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        SetModel(CounterRollupEffectModel.CreateDefault(), false);
        mInitialized = true;
    }

    public void SetModel(CounterRollupEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : CounterRollupEffectModel.CreateDefault();
        mTween?.Kill();
        mTween = null;
        mTargetTimer = 0f;
        mDisplayedValue = mModel.StartValue;
        RefreshDigits();

        if (!animated)
        {
            mView.Group.alpha = 1f;
            mView.Content.anchoredPosition = mConfig.PanelPosition;
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
        var basePos = mConfig.PanelPosition;

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
        var basePos = mConfig.PanelPosition;

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
        mTargetTimer += dt;
        if (mTargetTimer < Mathf.Max(1f, mModel.ChangeInterval))
        {
            return;
        }

        mTargetTimer = 0f;
        var min = Mathf.Min(mModel.MinTarget, mModel.MaxTarget);
        var max = Mathf.Max(mModel.MinTarget, mModel.MaxTarget);
        AnimateTo(Random.Range(min, max + 1));
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        mTween?.Kill();
        mTween = null;
        KillTransitionTweens();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    private void AnimateTo(int target)
    {
        mTween?.Kill();
        mTween = mContext.Tweens.ToFloat(
                () => mDisplayedValue,
                value =>
                {
                    mDisplayedValue = value;
                    RefreshDigits();
                },
                target,
                Mathf.Max(0.05f, mConfig.TweenDuration),
                mView.Root)
            .SetEase(mConfig.TweenEase);
    }

    private void RefreshDigits()
    {
        if (mView == null)
        {
            return;
        }

        for (var i = 0; i < mView.Columns.Count; i++)
        {
            var col = mView.Columns[i];
            if (col == null || col.Root == null)
            {
                continue;
            }

            var placeValue = Mathf.FloorToInt(mDisplayedValue / Mathf.Max(1, col.Place)) % 10;
            var digitHeight = col.Root.sizeDelta.y;

            for (var n = 0; n < col.Numbers.Count; n++)
            {
                var offset = (10 + n - placeValue) % 10;
                if (offset > 5)
                {
                    offset -= 10;
                }

                var numberRect = col.Numbers[n];
                if (numberRect == null)
                {
                    continue;
                }

                numberRect.anchoredPosition = new Vector2(0f, -offset * digitHeight);
            }
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
