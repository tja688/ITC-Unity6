using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public sealed class CountUpEffectController : IReplicaEffect<CountUpEffectConfig, CountUpEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private CountUpEffectConfig mConfig;
    private CountUpEffectView mView;
    private CountUpEffectModel mModel;

    private bool mInitialized;
    private float mDisplayedValue;
    private float mCurrentTarget;
    private float mCycleTimer;
    private IReplicaTweenHandle mDelayTween;
    private IReplicaTweenHandle mCountTween;
    private IReplicaTweenHandle mPunchTween;

    public string EffectId => "count-up-v2";

    public void Initialize(ReplicaHostContext context, CountUpEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<CountUpEffectConfig>();
        mView = CountUpEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        SetModel(CountUpEffectModel.CreateDefault(), false);
        mInitialized = true;
    }

    public void SetModel(CountUpEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : CountUpEffectModel.CreateDefault();
        mDisplayedValue = mModel.CountDown ? mModel.InitialTarget : mModel.FromValue;
        mCurrentTarget = mModel.InitialTarget;
        mCycleTimer = 0f;
        RefreshLabel();
        StartCount();

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

        if (mCountTween != null && mCountTween.IsActive && mCountTween.IsPlaying)
        {
            return;
        }

        var dt = mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;
        mCycleTimer += dt;
        if (mCycleTimer < Mathf.Max(1f, mModel.CycleInterval))
        {
            return;
        }

        mCycleTimer = 0f;
        mCurrentTarget = Random.Range(800f, 99999f);
        StartCount();
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTweens();
        KillTransitionTweens();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    private void StartCount()
    {
        KillTweens();

        var from = mDisplayedValue;
        var to = mModel.CountDown ? mModel.FromValue : mCurrentTarget;
        if (!mModel.CountDown && Mathf.Abs(from - to) < 0.01f)
        {
            from = mModel.FromValue;
            mDisplayedValue = from;
            RefreshLabel();
        }

        mDelayTween = mContext.Tweens.DelayedCall(Mathf.Max(0f, mModel.Delay), () =>
        {
            mCountTween = mContext.Tweens.ToFloat(
                    () => from,
                    value =>
                    {
                        mDisplayedValue = value;
                        from = value;
                        RefreshLabel();
                    },
                    to,
                    Mathf.Max(0.2f, mModel.Duration),
                    mView.Root)
                .SetEase(mConfig.CountEase)
                .SetUpdate(mContext.UseUnscaledTime)
                .OnComplete(() =>
                {
                    mDisplayedValue = to;
                    RefreshLabel();

                    if (mView.ValueRect != null)
                    {
                        mPunchTween?.Kill();
                        mPunchTween = mContext.Tweens.PunchScale(
                            mView.ValueRect,
                            mConfig.PunchScale,
                            Mathf.Max(0.01f, mConfig.PunchDuration),
                            Mathf.Max(1, mConfig.PunchVibrato),
                            Mathf.Clamp01(mConfig.PunchElasticity),
                            mView.ValueRect).SetUpdate(mContext.UseUnscaledTime);
                    }

                    if (mModel.CountDown)
                    {
                        mModel.FromValue = Random.Range(30000f, 98000f);
                    }
                });
        }, mView.Root).SetUpdate(mContext.UseUnscaledTime);
    }

    private void RefreshLabel()
    {
        if (mView.ValueLabel == null)
        {
            return;
        }

        mView.ValueLabel.text = FormatValue(mDisplayedValue);
    }

    private string FormatValue(float value)
    {
        var rounded = mModel.Decimals <= 0 ? Mathf.RoundToInt(value).ToString() : value.ToString($"F{mModel.Decimals}");
        if (string.IsNullOrEmpty(mModel.Separator))
        {
            return rounded;
        }

        var sign = "";
        var number = rounded;
        if (number.StartsWith("-"))
        {
            sign = "-";
            number = number.Substring(1);
        }

        var dotIndex = number.IndexOf('.');
        var integerPart = dotIndex >= 0 ? number.Substring(0, dotIndex) : number;
        var fractionPart = dotIndex >= 0 ? number.Substring(dotIndex) : string.Empty;

        for (var i = integerPart.Length - 3; i > 0; i -= 3)
        {
            integerPart = integerPart.Insert(i, mModel.Separator);
        }

        return sign + integerPart + fractionPart;
    }

    private void KillTweens()
    {
        mDelayTween?.Kill();
        mCountTween?.Kill();
        mPunchTween?.Kill();
        mDelayTween = null;
        mCountTween = null;
        mPunchTween = null;
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
