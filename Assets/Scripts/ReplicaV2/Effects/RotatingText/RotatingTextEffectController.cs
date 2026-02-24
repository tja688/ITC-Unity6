using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class RotatingTextEffectController : IReplicaEffect<RotatingTextEffectConfig, RotatingTextEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();
    private readonly List<IReplicaTweenHandle> mSwitchTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private RotatingTextEffectConfig mConfig;
    private RotatingTextEffectView mView;
    private RotatingTextEffectModel mModel;

    private bool mInitialized;
    private float mAutoTimer;
    private RotatingTextContainer mCurrent;

    public string EffectId => "rotating-text-v2";

    public void Initialize(ReplicaHostContext context, RotatingTextEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<RotatingTextEffectConfig>();
        mView = RotatingTextEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        SetModel(RotatingTextEffectModel.CreateDefault(), false);
        mInitialized = true;
    }

    public void SetModel(RotatingTextEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : RotatingTextEffectModel.CreateDefault();
        RotatingTextEffectViewBuilder.ApplyModel(mView, mConfig);

        KillSwitchTweens();
        DestroyContainer(ref mCurrent);

        var texts = SafeTexts(mModel.Texts);
        mModel.Texts = texts;
        mModel.CurrentIndex = ClampIndex(mModel.CurrentIndex, texts.Length);

        var initial = texts.Length > 0 ? texts[mModel.CurrentIndex] : "";
        mCurrent = RotatingTextEffectViewBuilder.BuildContainer(mView.Viewport, mConfig, initial, mModel.SplitBy);
        ApplyElementAlpha(mCurrent.Elements, 1f);

        mAutoTimer = 0f;

        if (!animated)
        {
            mView.Group.alpha = 1f;
            mView.Content.anchoredPosition = Vector2.zero;
        }
    }

    public void Next(bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var texts = SafeTexts(mModel.Texts);
        if (texts.Length <= 1)
        {
            return;
        }

        var nextIndex = mModel.CurrentIndex == texts.Length - 1 ? (mModel.Loop ? 0 : mModel.CurrentIndex) : mModel.CurrentIndex + 1;
        if (nextIndex == mModel.CurrentIndex)
        {
            return;
        }

        SwitchToIndex(nextIndex, animated);
    }

    public void Previous(bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var texts = SafeTexts(mModel.Texts);
        if (texts.Length <= 1)
        {
            return;
        }

        var prevIndex = mModel.CurrentIndex == 0 ? (mModel.Loop ? texts.Length - 1 : mModel.CurrentIndex) : mModel.CurrentIndex - 1;
        if (prevIndex == mModel.CurrentIndex)
        {
            return;
        }

        SwitchToIndex(prevIndex, animated);
    }

    public void JumpTo(int index, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var texts = SafeTexts(mModel.Texts);
        if (texts.Length == 0)
        {
            return;
        }

        var clamped = ClampIndex(index, texts.Length);
        if (clamped == mModel.CurrentIndex)
        {
            return;
        }

        SwitchToIndex(clamped, animated);
    }

    public void Reset(bool animated = true)
    {
        JumpTo(0, animated);
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
        var basePos = Vector2.zero;

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
        var basePos = Vector2.zero;

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
        if (!mModel.Auto || mModel.RotationInterval <= 0.01f)
        {
            return;
        }

        mAutoTimer += dt;
        if (mAutoTimer < mModel.RotationInterval)
        {
            return;
        }

        mAutoTimer = 0f;
        Next(true);
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        KillSwitchTweens();
        DestroyContainer(ref mCurrent);

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    private void SwitchToIndex(int index, bool animated)
    {
        var texts = SafeTexts(mModel.Texts);
        if (texts.Length == 0)
        {
            return;
        }

        index = ClampIndex(index, texts.Length);

        mModel.CurrentIndex = index;
        mAutoTimer = 0f;

        if (!animated || mCurrent == null)
        {
            KillSwitchTweens();
            DestroyContainer(ref mCurrent);
            mCurrent = RotatingTextEffectViewBuilder.BuildContainer(mView.Viewport, mConfig, texts[index], mModel.SplitBy);
            ApplyElementAlpha(mCurrent.Elements, 1f);
            if (mCurrent.Root != null)
            {
                mCurrent.Root.anchoredPosition = Vector2.zero;
            }
            return;
        }

        KillSwitchTweens();

        var outgoing = mCurrent;
        var incoming = RotatingTextEffectViewBuilder.BuildContainer(mView.Viewport, mConfig, texts[index], mModel.SplitBy);

        if (incoming.Root != null)
        {
            incoming.Root.anchoredPosition = new Vector2(0f, mConfig.EnterOffsetY);
        }

        ApplyElementAlpha(incoming.Elements, 0f);

        var enterDuration = Mathf.Max(0.01f, mConfig.EnterDuration);
        var exitDuration = Mathf.Max(0.01f, mConfig.ExitDuration);

        if (incoming.Root != null)
        {
            mSwitchTweens.Add(mContext.Tweens
                .AnchoredPosYTo(incoming.Root, 0f, enterDuration, incoming.Root)
                .SetEase(mConfig.EnterEase)
                .SetUpdate(mContext.UseUnscaledTime));
        }

        if (outgoing.Root != null)
        {
            mSwitchTweens.Add(mContext.Tweens
                .AnchoredPosYTo(outgoing.Root, -mConfig.ExitOffsetY, exitDuration, outgoing.Root)
                .SetEase(mConfig.ExitEase)
                .SetUpdate(mContext.UseUnscaledTime));
        }

        var randomIndex = incoming.Elements != null && incoming.Elements.Count > 0 ? UnityEngine.Random.Range(0, incoming.Elements.Count) : 0;
        var maxEnterDelay = FadeElements(incoming.Elements, 1f, enterDuration, mConfig.EnterEase, randomIndex);
        var maxExitDelay = FadeElements(outgoing.Elements, 0f, exitDuration, mConfig.ExitEase, randomIndex);

        var cleanupDelay = Mathf.Max(enterDuration + maxEnterDelay, exitDuration + maxExitDelay) + 0.05f;
        mSwitchTweens.Add(mContext.Tweens.DelayedCall(cleanupDelay, () =>
        {
            if (outgoing.Root != null)
            {
                UnityEngine.Object.Destroy(outgoing.Root.gameObject);
            }
        }, mView.Root).SetUpdate(mContext.UseUnscaledTime));

        mCurrent = incoming;
    }

    private float FadeElements(List<UnityEngine.UI.Text> elements, float targetAlpha, float duration, ReplicaEase ease, int randomIndex)
    {
        if (elements == null || elements.Count == 0)
        {
            return 0f;
        }

        var total = elements.Count;
        var maxDelay = 0f;
        for (var i = 0; i < total; i++)
        {
            var element = elements[i];
            if (element == null)
            {
                continue;
            }

            var delay = GetStaggerDelay(i, total, randomIndex);
            if (delay > maxDelay)
            {
                maxDelay = delay;
            }

            if (delay <= 0.0001f)
            {
                delay = 0f;
            }

            mSwitchTweens.Add(mContext.Tweens
                .FadeTextTo(element, targetAlpha, duration, element)
                .SetDelay(delay)
                .SetEase(ease)
                .SetUpdate(mContext.UseUnscaledTime));
        }

        return maxDelay;
    }

    private float GetStaggerDelay(int index, int total, int randomIndex)
    {
        var step = Mathf.Max(0f, mModel.StaggerDuration);
        if (step <= 0.0001f)
        {
            return 0f;
        }

        switch (mModel.StaggerFrom)
        {
            case RotatingTextStaggerFrom.Last:
                return (total - 1 - index) * step;
            case RotatingTextStaggerFrom.Center:
                var center = total / 2;
                return Mathf.Abs(center - index) * step;
            case RotatingTextStaggerFrom.Random:
                return Mathf.Abs(randomIndex - index) * step;
            case RotatingTextStaggerFrom.First:
            default:
                return index * step;
        }
    }

    private static void ApplyElementAlpha(List<UnityEngine.UI.Text> elements, float alpha)
    {
        if (elements == null)
        {
            return;
        }

        for (var i = 0; i < elements.Count; i++)
        {
            var t = elements[i];
            if (t == null)
            {
                continue;
            }

            var c = t.color;
            c.a = alpha;
            t.color = c;
        }
    }

    private void DestroyContainer(ref RotatingTextContainer container)
    {
        if (container?.Root != null)
        {
            UnityEngine.Object.Destroy(container.Root.gameObject);
        }

        container = null;
    }

    private void KillSwitchTweens()
    {
        for (var i = 0; i < mSwitchTweens.Count; i++)
        {
            mSwitchTweens[i]?.Kill();
        }

        mSwitchTweens.Clear();
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

    private static string[] SafeTexts(string[] texts)
    {
        if (texts == null)
        {
            return Array.Empty<string>();
        }

        var hasAny = false;
        for (var i = 0; i < texts.Length; i++)
        {
            if (!string.IsNullOrEmpty(texts[i]))
            {
                hasAny = true;
                break;
            }
        }

        return hasAny ? texts : Array.Empty<string>();
    }

    private static int ClampIndex(int index, int count)
    {
        if (count <= 0)
        {
            return 0;
        }

        return Mathf.Clamp(index, 0, count - 1);
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
