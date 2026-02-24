using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class BounceCardsEffectController : IReplicaEffect<BounceCardsEffectConfig, BounceCardsEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private BounceCardsEffectConfig mConfig;
    private BounceCardsEffectView mView;

    private bool mInitialized;
    private int mCurrentHoveredIndex = -1;
    private float mEntryBlockRemaining;

    public string EffectId => "bounce-cards-v2";

    public void Initialize(ReplicaHostContext context, BounceCardsEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<BounceCardsEffectConfig>();
        mView = BounceCardsEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        ResetCardsImmediate();
        mInitialized = true;
    }

    public void SetModel(BounceCardsEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model ?? BounceCardsEffectModel.CreateDefault();
        if (mView.HintText != null)
        {
            mView.HintText.text = string.IsNullOrWhiteSpace(safe.Hint) ? "BounceCards" : safe.Hint;
        }

        if (!animated)
        {
            mView.Group.alpha = 1f;
        }
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        ResetCardsImmediate();

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var basePos = new Vector2(0f, -10f);
        var offset = EnterOffset(transition.EnterDirection, mConfig.EnterOffset);

        if (mView.Backdrop != null)
        {
            mView.Backdrop.anchoredPosition = basePos + offset;
        }

        if (mView.Group != null)
        {
            mView.Group.alpha = 0f;
        }

        if (mView.Backdrop != null)
        {
            mTransitionTweens.Add(mContext.Tweens.AnchoredPosTo(mView.Backdrop, basePos, duration, mView.Root).SetEase(transition.Ease));
        }

        if (mView.Group != null)
        {
            mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Root, transition.Ease));
        }

        PlayEntryAnimation();
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        if (!mInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        KillTransitionTweens();
        KillCardTweens();

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var basePos = new Vector2(0f, -10f);
        var offset = ExitOffset(transition.ExitDirection, mConfig.ExitOffset);

        if (mView.Backdrop != null)
        {
            mTransitionTweens.Add(
                mContext.Tweens
                    .AnchoredPosTo(mView.Backdrop, basePos + offset, duration, mView.Root)
                    .SetEase(transition.Ease));
        }

        if (mView.Group != null)
        {
            mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 0f, duration, mView.Root, transition.Ease).OnComplete(onComplete));
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized)
        {
            return;
        }

        if (mEntryBlockRemaining > 0f)
        {
            var step = mContext != null && mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;
            mEntryBlockRemaining = Mathf.Max(0f, mEntryBlockRemaining - Mathf.Max(0f, step));
            return;
        }

        var hovered = DetermineHoveredIndex();
        if (hovered == mCurrentHoveredIndex)
        {
            return;
        }

        mCurrentHoveredIndex = hovered;
        if (hovered < 0)
        {
            AnimateReset();
        }
        else
        {
            AnimateHover(hovered);
        }
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        KillCardTweens();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    private int DetermineHoveredIndex()
    {
        if (mContext.Pointer == null || !mContext.Pointer.IsPointerValid || mView == null)
        {
            return -1;
        }

        var count = mView.Cards.Count;
        for (var i = count - 1; i >= 0; i--)
        {
            var card = mView.Cards[i];
            if (card == null)
            {
                continue;
            }

            if (!mContext.Pointer.TryGetLocalPoint(card, out var localPoint, mContext.UICamera))
            {
                continue;
            }

            var halfW = Mathf.Max(1f, card.rect.width * 0.5f);
            var halfH = Mathf.Max(1f, card.rect.height * 0.5f);
            if (Mathf.Abs(localPoint.x) <= halfW && Mathf.Abs(localPoint.y) <= halfH)
            {
                return i;
            }
        }

        return -1;
    }

    private void PlayEntryAnimation()
    {
        var count = mView.Cards.Count;
        var safeEntryDelay = Mathf.Max(0f, mConfig.EntryDelay);
        var safeEntryStagger = Mathf.Max(0f, mConfig.EntryStagger);
        var safeEntryDuration = Mathf.Max(0.01f, mConfig.EntryDuration);

        mEntryBlockRemaining = count > 0
            ? safeEntryDelay + ((count - 1) * safeEntryStagger) + safeEntryDuration
            : 0f;

        for (var i = 0; i < count; i++)
        {
            var rect = mView.Cards[i];
            if (rect == null)
            {
                continue;
            }

            mContext.Tweens.Kill(rect);
            mContext.Tweens.Kill(rect.gameObject);
            rect.localScale = Vector3.zero;
            mContext.Tweens
                .ScaleTo(rect, Vector3.one, safeEntryDuration, rect.gameObject)
                .SetDelay(safeEntryDelay + (i * safeEntryStagger))
                .SetEase(ReplicaEase.OutElastic);
        }
    }

    private void ResetCardsImmediate()
    {
        var count = mView.Cards.Count;
        for (var i = 0; i < count; i++)
        {
            var rect = mView.Cards[i];
            if (rect == null)
            {
                continue;
            }

            mContext.Tweens.Kill(rect);
            mContext.Tweens.Kill(rect.gameObject);
            rect.anchoredPosition = mView.BaseOffsets[i];
            rect.localRotation = Quaternion.Euler(0f, 0f, mView.BaseRotations[i]);
            rect.localScale = Vector3.one;
        }

        mEntryBlockRemaining = 0f;
        mCurrentHoveredIndex = -1;
    }

    private void AnimateHover(int hoveredIndex)
    {
        var count = mView.Cards.Count;
        for (var i = 0; i < count; i++)
        {
            var rect = mView.Cards[i];
            if (rect == null)
            {
                continue;
            }

            mContext.Tweens.Kill(rect);

            if (i == hoveredIndex)
            {
                mContext.Tweens
                    .AnchoredPosTo(rect, mView.BaseOffsets[i] + new Vector2(0f, mConfig.HoverLiftY), mConfig.HoverDuration, rect)
                    .SetEase(ReplicaEase.OutBack, mConfig.HoverOvershoot);
                mContext.Tweens
                    .LocalRotateTo(rect, Vector3.zero, mConfig.HoverDuration, rect)
                    .SetEase(ReplicaEase.OutBack, mConfig.HoverOvershoot);
                continue;
            }

            var direction = i < hoveredIndex ? -1f : 1f;
            var distance = Mathf.Abs(i - hoveredIndex);
            var targetPos = mView.BaseOffsets[i] + new Vector2(direction * mConfig.HoverPushOffset, 0f);
            var delay = distance * mConfig.HoverSiblingDelayStep;
            var baseRot = mView.BaseRotations[i];

            mContext.Tweens
                .AnchoredPosTo(rect, targetPos, mConfig.HoverDuration, rect)
                .SetDelay(delay)
                .SetEase(ReplicaEase.OutBack, mConfig.HoverOvershoot);
            mContext.Tweens
                .LocalRotateTo(rect, new Vector3(0f, 0f, baseRot), mConfig.HoverDuration, rect)
                .SetDelay(delay)
                .SetEase(ReplicaEase.OutBack, mConfig.HoverOvershoot);
        }
    }

    private void AnimateReset()
    {
        var count = mView.Cards.Count;
        for (var i = 0; i < count; i++)
        {
            var rect = mView.Cards[i];
            if (rect == null)
            {
                continue;
            }

            mContext.Tweens.Kill(rect);
            mContext.Tweens
                .AnchoredPosTo(rect, mView.BaseOffsets[i], mConfig.HoverDuration, rect)
                .SetEase(ReplicaEase.OutBack, mConfig.HoverOvershoot);
            mContext.Tweens
                .LocalRotateTo(rect, new Vector3(0f, 0f, mView.BaseRotations[i]), mConfig.HoverDuration, rect)
                .SetEase(ReplicaEase.OutBack, mConfig.HoverOvershoot);
        }
    }

    private void KillCardTweens()
    {
        var count = mView.Cards.Count;
        for (var i = 0; i < count; i++)
        {
            var rect = mView.Cards[i];
            if (rect == null)
            {
                continue;
            }

            mContext.Tweens.Kill(rect);
            mContext.Tweens.Kill(rect.gameObject);
        }

        mEntryBlockRemaining = 0f;
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
