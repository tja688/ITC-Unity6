using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class CardSwapEffectController : IReplicaEffect<CardSwapEffectConfig, CardSwapEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();
    private readonly List<IReplicaTweenHandle> mSwapTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private CardSwapEffectConfig mConfig;
    private CardSwapEffectView mView;

    private bool mInitialized;
    private float mSwapTimer;
    private bool mHoverPaused;
    private bool mAnimating;

    public string EffectId => "card-swap-v2";

    public void Initialize(ReplicaHostContext context, CardSwapEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<CardSwapEffectConfig>();
        mView = CardSwapEffectViewBuilder.Build(mContext.MountRoot, mConfig);

        for (var i = 0; i < mView.Cards.Count; i++)
        {
            var card = mView.Cards[i];
            if (card?.InputRelay != null)
            {
                card.InputRelay.Initialize(this, card.Rect);
            }
        }

        ApplySlotLayout(false, 0f);
        mSwapTimer = Mathf.Max(0f, mConfig.SwapDelay) * 0.25f;
        mInitialized = true;
    }

    public void SetModel(CardSwapEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model ?? CardSwapEffectModel.CreateDefault();
        if (mView.HintText != null)
        {
            mView.HintText.text = string.IsNullOrWhiteSpace(safe.Hint) ? "CardSwap" : safe.Hint;
        }

        var caption = string.IsNullOrWhiteSpace(safe.Caption) ? "Click the front card to swap now" : safe.Caption;
        for (var i = 0; i < mView.Cards.Count; i++)
        {
            var card = mView.Cards[i];
            if (card?.CaptionText != null)
            {
                card.CaptionText.text = caption;
            }
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
        KillSwapTweens();
        KillCardTweens();

        mAnimating = false;
        mSwapTimer = 0f;
        mHoverPaused = false;
        ApplySlotLayout(false, 0f);

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var basePos = Vector2.zero;
        var offset = EnterOffset(transition.EnterDirection, mConfig.EnterOffset);

        if (mView.Surface != null)
        {
            basePos = mView.Surface.anchoredPosition;
            mView.Surface.anchoredPosition = basePos + offset;
        }

        if (mView.Group != null)
        {
            mView.Group.alpha = 0f;
        }

        if (mView.Surface != null)
        {
            mTransitionTweens.Add(mContext.Tweens.AnchoredPosTo(mView.Surface, basePos, duration, mView.Root).SetEase(transition.Ease));
        }

        if (mView.Group != null)
        {
            mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Root, transition.Ease));
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
        KillSwapTweens();
        KillCardTweens();

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var basePos = mView.Surface != null ? mView.Surface.anchoredPosition : Vector2.zero;
        var offset = ExitOffset(transition.ExitDirection, mConfig.ExitOffset);

        if (mView.Surface != null)
        {
            mTransitionTweens.Add(mContext.Tweens.AnchoredPosTo(mView.Surface, basePos + offset, duration, mView.Root).SetEase(transition.Ease));
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

        var dt = mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;
        UpdateHoverPause();

        if (mAnimating || mView.Cards.Count <= 1 || (mConfig.PauseOnHover && mHoverPaused))
        {
            return;
        }

        mSwapTimer += dt;
        if (mSwapTimer < Mathf.Max(1.5f, mConfig.SwapDelay))
        {
            return;
        }

        mSwapTimer = 0f;
        StartSwap();
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        KillSwapTweens();
        KillCardTweens();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    public void OnCardClick(RectTransform card, PointerEventData eventData)
    {
        if (mAnimating || mView.Cards.Count == 0)
        {
            return;
        }

        if (mView.Cards[0].Rect != card)
        {
            return;
        }

        mSwapTimer = 0f;
        StartSwap();
    }

    private void UpdateHoverPause()
    {
        if (!mConfig.PauseOnHover || mContext.Pointer == null || !mContext.Pointer.IsPointerValid || mView?.Surface == null)
        {
            mHoverPaused = false;
            return;
        }

        if (!mContext.Pointer.TryGetLocalPoint(mView.Surface, out var localPoint, mContext.UICamera))
        {
            mHoverPaused = false;
            return;
        }

        var halfW = Mathf.Max(1f, mView.Surface.rect.width * 0.5f);
        var halfH = Mathf.Max(1f, mView.Surface.rect.height * 0.5f);
        mHoverPaused = Mathf.Abs(localPoint.x) <= halfW && Mathf.Abs(localPoint.y) <= halfH;
    }

    private void StartSwap()
    {
        if (mAnimating || mView.Cards.Count <= 1)
        {
            return;
        }

        mAnimating = true;
        var front = mView.Cards[0];

        KillSwapTweens();
        if (front?.Rect == null)
        {
            mAnimating = false;
            return;
        }

        mContext.Tweens.Kill(front.Rect);
        var outDuration = Mathf.Max(0.01f, mConfig.OutDuration);
        mSwapTweens.Add(
            mContext.Tweens
                .AnchoredPosYTo(front.Rect, front.Rect.anchoredPosition.y - Mathf.Abs(mConfig.OutDistanceY), outDuration, front.Rect)
                .SetEase(ReplicaEase.OutElastic));

        var layoutDuration = Mathf.Max(0.01f, mConfig.LayoutDuration);
        mSwapTweens.Add(mContext.Tweens.DelayedCall(outDuration, () =>
        {
            if (mView.Cards.Count <= 1)
            {
                return;
            }

            mView.Cards.RemoveAt(0);
            mView.Cards.Add(front);
            ApplySlotLayout(true, layoutDuration);
        }, mView.Root));

        var maxDelay = Mathf.Max(0f, (mView.Cards.Count - 1) * Mathf.Max(0f, mConfig.LayoutDelayStep));
        var finishDelay = outDuration + Mathf.Max(1.50f, maxDelay + layoutDuration);
        mSwapTweens.Add(mContext.Tweens.DelayedCall(finishDelay, () => mAnimating = false, mView.Root));
    }

    private void ApplySlotLayout(bool animate, float duration)
    {
        var count = mView.Cards.Count;
        if (count == 0)
        {
            return;
        }

        var centerBias = (count - 1) * 0.5f;
        for (var i = 0; i < count; i++)
        {
            var card = mView.Cards[i];
            if (card?.Rect == null || card.Visual == null)
            {
                continue;
            }

            var slotPos = new Vector2(
                (i - centerBias) * mConfig.CardDistance,
                (centerBias - i) * mConfig.VerticalDistance * 0.72f);
            var slotScale = Mathf.Clamp(1f - (i * 0.07f), 0.70f, 1f);
            var slotRot = i * 3.5f;

            card.Rect.SetSiblingIndex(count - i - 1);
            mContext.Tweens.Kill(card.Rect);
            mContext.Tweens.Kill(card.Visual);

            if (!animate)
            {
                card.Rect.anchoredPosition = slotPos;
                card.Visual.localScale = new Vector3(slotScale, slotScale, 1f);
                card.Visual.localRotation = Quaternion.Euler(0f, 0f, slotRot);
                continue;
            }

            var delay = i * Mathf.Max(0f, mConfig.LayoutDelayStep);
            mContext.Tweens
                .AnchoredPosTo(card.Rect, slotPos, duration, card.Rect)
                .SetDelay(delay)
                .SetEase(ReplicaEase.OutElastic);
            mContext.Tweens
                .ScaleTo(card.Visual, new Vector3(slotScale, slotScale, 1f), duration, card.Visual)
                .SetDelay(delay)
                .SetEase(ReplicaEase.OutCubic);
            mContext.Tweens
                .LocalRotateTo(card.Visual, new Vector3(0f, 0f, slotRot), duration, card.Visual)
                .SetDelay(delay)
                .SetEase(ReplicaEase.OutCubic);
        }
    }

    private void KillCardTweens()
    {
        if (mView == null)
        {
            return;
        }

        for (var i = 0; i < mView.Cards.Count; i++)
        {
            var card = mView.Cards[i];
            if (card?.Rect != null)
            {
                mContext.Tweens.Kill(card.Rect);
            }
            if (card?.Visual != null)
            {
                mContext.Tweens.Kill(card.Visual);
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

    private void KillSwapTweens()
    {
        for (var i = 0; i < mSwapTweens.Count; i++)
        {
            mSwapTweens[i]?.Kill();
        }

        mSwapTweens.Clear();
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

