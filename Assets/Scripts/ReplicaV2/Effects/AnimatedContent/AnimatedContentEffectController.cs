using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class AnimatedContentEffectController : IReplicaEffect<AnimatedContentEffectConfig, AnimatedContentEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private AnimatedContentEffectConfig mConfig;
    private AnimatedContentEffectView mView;

    private bool mInitialized;
    private bool mPlayedOnce;

    private IReplicaTweenHandle mDelayHandle;
    private IReplicaTweenHandle mDisappearHandle;

    public string EffectId => "animatedcontent-v2";

    public void Initialize(ReplicaHostContext context, AnimatedContentEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<AnimatedContentEffectConfig>();
        mView = AnimatedContentEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        ResetInitialState();
        mInitialized = true;
    }

    public void SetModel(AnimatedContentEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model ?? AnimatedContentEffectModel.CreateDefault();
        if (mView.LabelText != null)
        {
            mView.LabelText.text = string.IsNullOrWhiteSpace(safe.Label) ? "AnimatedContent" : safe.Label;
        }

        if (!animated)
        {
            if (mView.Container != null)
            {
                mView.Container.anchoredPosition = Vector2.zero;
                mView.Container.localScale = Vector3.one;
            }

            if (mView.ContainerGroup != null)
            {
                mView.ContainerGroup.alpha = 1f;
            }
        }
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (!mInitialized)
        {
            return;
        }

        if (mConfig.PlayOnce && mPlayedOnce)
        {
            return;
        }

        mPlayedOnce = true;
        KillTransitionTweens();
        KillScheduled();

        ResetInitialState();

        var delay = Mathf.Max(0f, mConfig.Delay);
        if (delay > 0f)
        {
            mDelayHandle = mContext.Tweens.DelayedCall(delay, () => StartPlayInTweens(transition), mView.Root);
            return;
        }

        StartPlayInTweens(transition);
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        if (!mInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        KillTransitionTweens();
        KillScheduled();

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var offset = ExitOffset(transition.ExitDirection, mConfig.ExitOffset);

        if (mView.Container != null)
        {
            mTransitionTweens.Add(
                mContext.Tweens
                    .AnchoredPosTo(mView.Container, offset, duration, mView.Root)
                    .SetEase(transition.Ease)
                    .OnComplete(onComplete));

            mTransitionTweens.Add(mContext.Tweens.ScaleTo(mView.Container, Vector3.one * Mathf.Max(0.01f, mConfig.DisappearScale), duration, mView.Root).SetEase(transition.Ease));
        }
        else
        {
            onComplete?.Invoke();
        }

        if (mView.ContainerGroup != null)
        {
            mTransitionTweens.Add(FadeCanvasGroup(mView.ContainerGroup, 0f, duration, mView.Root, transition.Ease));
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
        KillScheduled();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    private void StartPlayInTweens(ReplicaTransition transition)
    {
        if (!mInitialized || mView?.Container == null)
        {
            return;
        }

        var duration = Mathf.Max(0.08f, mConfig.Duration);
        var offset = EnterOffset(transition.EnterDirection, mConfig.Distance, mConfig.Direction, mConfig.Reverse);

        mView.Container.anchoredPosition = offset;
        mTransitionTweens.Add(mContext.Tweens.AnchoredPosTo(mView.Container, Vector2.zero, duration, mView.Root).SetEase(transition.Ease));
        mTransitionTweens.Add(mContext.Tweens.ScaleTo(mView.Container, Vector3.one, duration, mView.Root).SetEase(transition.Ease));

        if (mView.ContainerGroup != null)
        {
            mView.ContainerGroup.alpha = mConfig.AnimateOpacity ? Mathf.Clamp01(mConfig.InitialOpacity) : 1f;
            mTransitionTweens.Add(FadeCanvasGroup(mView.ContainerGroup, 1f, duration, mView.Root, transition.Ease).OnComplete(HandlePlayInComplete));
        }
        else
        {
            HandlePlayInComplete();
        }
    }

    private void HandlePlayInComplete()
    {
        if (!mInitialized || mConfig.DisappearAfter <= 0f)
        {
            return;
        }

        var delay = Mathf.Max(0f, mConfig.DisappearAfter);
        mDisappearHandle = mContext.Tweens.DelayedCall(delay, PlayDisappearance, mView.Root);
    }

    private void PlayDisappearance()
    {
        if (!mInitialized || mView?.Container == null)
        {
            return;
        }

        KillTransitionTweens();

        var duration = Mathf.Max(0.08f, mConfig.DisappearDuration);
        var offset = DisappearOffset(mConfig.Distance, mConfig.Direction, mConfig.Reverse);

        mTransitionTweens.Add(mContext.Tweens.AnchoredPosTo(mView.Container, offset, duration, mView.Root).SetEase(mConfig.DisappearEase));
        mTransitionTweens.Add(mContext.Tweens.ScaleTo(mView.Container, Vector3.one * Mathf.Max(0.01f, mConfig.DisappearScale), duration, mView.Root).SetEase(mConfig.DisappearEase));

        if (mView.ContainerGroup != null)
        {
            var targetAlpha = mConfig.AnimateOpacity ? Mathf.Clamp01(mConfig.InitialOpacity) : 0f;
            mTransitionTweens.Add(FadeCanvasGroup(mView.ContainerGroup, targetAlpha, duration, mView.Root, mConfig.DisappearEase));
        }
    }

    private void ResetInitialState()
    {
        if (mView?.Container == null)
        {
            return;
        }

        var offset = EnterOffset(ReplicaEnterDirection.None, mConfig.Distance, mConfig.Direction, mConfig.Reverse);
        mView.Container.anchoredPosition = offset;
        mView.Container.localScale = Vector3.one * Mathf.Max(0.01f, mConfig.InitialScale);

        if (mView.ContainerGroup != null)
        {
            mView.ContainerGroup.alpha = mConfig.AnimateOpacity ? Mathf.Clamp01(mConfig.InitialOpacity) : 1f;
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

    private void KillScheduled()
    {
        mDelayHandle?.Kill();
        mDisappearHandle?.Kill();
        mDelayHandle = null;
        mDisappearHandle = null;
    }

    private static Vector2 EnterOffset(ReplicaEnterDirection direction, float distance, AnimatedContentDirection axis, bool reverse)
    {
        if (direction != ReplicaEnterDirection.None)
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

        if (axis == AnimatedContentDirection.Horizontal)
        {
            var signedX = reverse ? -distance : distance;
            return new Vector2(signedX, 0f);
        }

        var signedY = reverse ? distance : -distance;
        return new Vector2(0f, signedY);
    }

    private static Vector2 DisappearOffset(float distance, AnimatedContentDirection axis, bool reverse)
    {
        if (axis == AnimatedContentDirection.Horizontal)
        {
            var signedX = reverse ? distance : -distance;
            return new Vector2(signedX, 0f);
        }

        var signedY = reverse ? -distance : distance;
        return new Vector2(0f, signedY);
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
