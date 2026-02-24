using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class FadeContentEffectController : IReplicaEffect<FadeContentEffectConfig, FadeContentEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private FadeContentEffectConfig mConfig;
    private FadeContentEffectView mView;
    private IReplicaTweenHandle mDisappearTween;
    private bool mInitialized;

    public string EffectId => "fadecontent-v2";

    public void Initialize(ReplicaHostContext context, FadeContentEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<FadeContentEffectConfig>();
        mView = FadeContentEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        mInitialized = true;
    }

    public void SetModel(FadeContentEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model != null ? model : FadeContentEffectModel.CreateDefault();
        mView.TitleText.text = string.IsNullOrWhiteSpace(safe.Title) ? "Fade Content" : safe.Title;
        mView.BodyText.text = string.IsNullOrWhiteSpace(safe.Body) ? "Basic but essential transition wrapper." : safe.Body;

        if (!animated)
        {
            mView.Group.alpha = 1f;
            mView.Card.anchoredPosition = Vector2.zero;
            mView.Card.localScale = Vector3.one;
        }
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        mDisappearTween?.Kill();
        mDisappearTween = null;

        var duration = ResolveDuration(transition, 0.6f);
        var offset = EnterOffset(transition.EnterDirection, mConfig.EnterOffset);

        mView.Card.anchoredPosition = offset;
        mView.Group.alpha = Mathf.Clamp01(mConfig.InitialOpacity);

        if (mConfig.SimulateBlurWithScale)
        {
            var s = Mathf.Max(0.6f, mConfig.BlurStartScale);
            mView.Card.localScale = new Vector3(s, s, 1f);
        }
        else
        {
            mView.Card.localScale = Vector3.one;
        }

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Card, Vector2.zero, duration, mView.Card)
            .SetEase(transition.Ease));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Card, transition.Ease));

        if (mConfig.SimulateBlurWithScale)
        {
            mTransitionTweens.Add(mContext.Tweens
                .ScaleTo(mView.Card, Vector3.one, duration, mView.Card)
                .SetEase(transition.Ease));
        }

        if (mConfig.DisappearAfter > 0f)
        {
            var disappearDelay = Mathf.Max(0f, mConfig.DisappearAfter);
            mDisappearTween = mContext.Tweens.DelayedCall(disappearDelay, () =>
            {
                var t = transition;
                t.ExitDirection = ReplicaExitDirection.FadeOnly;
                t.Duration = Mathf.Max(0.05f, mConfig.DisappearDuration);
                t.Ease = mConfig.DisappearEase;
                PlayOut(t);
            }, mView.Card);
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
        mDisappearTween?.Kill();
        mDisappearTween = null;

        var duration = ResolveDuration(transition, 0.4f);
        var offset = ExitOffset(transition.ExitDirection, mConfig.ExitOffset);
        var targetAlpha = Mathf.Clamp01(mConfig.InitialOpacity);

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Card, offset, duration, mView.Card)
            .SetEase(transition.Ease)
            .OnComplete(onComplete));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, targetAlpha, duration, mView.Card, transition.Ease));
    }

    public void Tick(float deltaTime, float unscaledDeltaTime) { }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        mDisappearTween?.Kill();
        mDisappearTween = null;

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
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

    private static float ResolveDuration(ReplicaTransition transition, float fallback)
    {
        var duration = transition.Duration > 0f ? transition.Duration : fallback;
        return Mathf.Max(0.05f, duration);
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
