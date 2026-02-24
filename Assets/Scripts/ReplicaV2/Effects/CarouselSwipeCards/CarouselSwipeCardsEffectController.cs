using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class CarouselSwipeCardsEffectController : IReplicaEffect<CarouselSwipeCardsEffectConfig, CarouselSwipeCardsEffectModel>
{
    private ReplicaHostContext mContext;
    private CarouselSwipeCardsEffectConfig mConfig;
    private CarouselSwipeCardsEffectView mView;
    private CarouselSwipeCardsEffectModel mModel;

    private bool mInitialized;
    private int mCurrentIndex;
    private float mAutoplayTimer;
    private bool mDragging;
    private bool mHovered;
    private Vector2 mDragStartPointer;
    private float mDragStartStageX;

    public string EffectId => "carousel-swipe-cards-v2";

    public void Initialize(ReplicaHostContext context, CarouselSwipeCardsEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<CarouselSwipeCardsEffectConfig>();
        mModel = CarouselSwipeCardsEffectModel.CreateDefault();
        mView = CarouselSwipeCardsEffectViewBuilder.Build(mContext.MountRoot, mConfig, this, mModel);
        SetIndex(0, true);
        mInitialized = true;
    }

    public void SetModel(CarouselSwipeCardsEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : CarouselSwipeCardsEffectModel.CreateDefault();
        RebuildView();
    }

    public void PlayIn(ReplicaTransition transition)
    {
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        onComplete?.Invoke();
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized || mView == null)
        {
            return;
        }

        if (!mConfig.Autoplay || mDragging || (mConfig.PauseOnHover && mHovered) || mView.Cards.Count <= 1)
        {
            return;
        }

        var dt = mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;
        mAutoplayTimer += dt;
        if (mAutoplayTimer < Mathf.Max(1.2f, mConfig.AutoplayDelay))
        {
            return;
        }

        mAutoplayTimer = 0f;
        SetIndex(mCurrentIndex + 1, false);
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        DestroyView();
        mInitialized = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mHovered = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!mInitialized || mView == null)
        {
            return;
        }

        mDragging = true;
        mAutoplayTimer = 0f;
        mContext.Tweens.Kill(mView.CardStage);
        mDragStartPointer = eventData.position;
        mDragStartStageX = mView.CardStage.anchoredPosition.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!mDragging || mView == null)
        {
            return;
        }

        var delta = eventData.position - mDragStartPointer;
        var targetX = Mathf.Clamp(mDragStartStageX + (delta.x * mConfig.DragMoveScale), -mConfig.DragClamp, mConfig.DragClamp);
        mView.CardStage.anchoredPosition = new Vector2(targetX, mView.CardStage.anchoredPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!mDragging || mView == null)
        {
            return;
        }

        mDragging = false;
        var delta = eventData.position - mDragStartPointer;
        if (delta.x <= -mConfig.SwipeThreshold)
        {
            SetIndex(mCurrentIndex + 1, false);
        }
        else if (delta.x >= mConfig.SwipeThreshold)
        {
            SetIndex(mCurrentIndex - 1, false);
        }
        else
        {
            AnimateCards(false);
        }
    }

    public void SetIndex(int nextIndex, bool immediate)
    {
        if (!mInitialized || mView == null || mView.Cards.Count == 0)
        {
            return;
        }

        if (mConfig.Loop)
        {
            if (nextIndex < 0)
            {
                nextIndex = mView.Cards.Count - 1;
            }
            else if (nextIndex >= mView.Cards.Count)
            {
                nextIndex = 0;
            }
        }
        else
        {
            nextIndex = Mathf.Clamp(nextIndex, 0, mView.Cards.Count - 1);
        }

        mCurrentIndex = nextIndex;
        AnimateCards(immediate);
        UpdateIndicators();
        mAutoplayTimer = 0f;
    }

    private void AnimateCards(bool immediate)
    {
        if (mView == null)
        {
            return;
        }

        mContext.Tweens.Kill(mView.CardStage);
        mContext.Tweens
            .AnchoredPosXTo(mView.CardStage, 0f, immediate ? 0f : mConfig.StageReturnDuration, mView.CardStage)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime);

        var count = mView.Cards.Count;
        for (var i = 0; i < count; i++)
        {
            var card = mView.Cards[i];
            var delta = WrapDelta(i - mCurrentIndex, count);
            var abs = Mathf.Abs(delta);

            var targetX = delta * mConfig.CardStepX;
            var targetY = -abs * mConfig.CardStepY;
            var targetScale = abs == 0 ? 1f : (abs == 1 ? 0.86f : 0.74f);
            var targetRot = -delta * mConfig.CardRotateStep;
            var targetAlpha = abs == 0 ? 1f : (abs == 1 ? 0.58f : 0.18f);

            card.Rect.SetSiblingIndex(abs == 0 ? count - 1 : (count - 1 - abs));
            mContext.Tweens.Kill(card.Rect);
            mContext.Tweens.Kill(card.Group);

            if (immediate)
            {
                card.Rect.anchoredPosition = new Vector2(targetX, targetY);
                card.Rect.localScale = Vector3.one * targetScale;
                card.Rect.localRotation = Quaternion.Euler(0f, 0f, targetRot);
                card.Group.alpha = targetAlpha;
                continue;
            }

            mContext.Tweens
                .AnchoredPosTo(card.Rect, new Vector2(targetX, targetY), mConfig.CardMoveDuration, card.Rect)
                .SetEase(ReplicaEase.OutCubic)
                .SetUpdate(mContext.UseUnscaledTime);
            mContext.Tweens
                .ScaleTo(card.Rect, Vector3.one * targetScale, mConfig.CardMoveDuration, card.Rect)
                .SetEase(ReplicaEase.OutCubic)
                .SetUpdate(mContext.UseUnscaledTime);
            mContext.Tweens
                .LocalRotateTo(card.Rect, new Vector3(0f, 0f, targetRot), mConfig.CardMoveDuration, card.Rect)
                .SetEase(ReplicaEase.OutCubic)
                .SetUpdate(mContext.UseUnscaledTime);
            FadeCanvasGroup(card.Group, targetAlpha, mConfig.CardFadeDuration, card.Group, ReplicaEase.OutCubic);
        }
    }

    private void UpdateIndicators()
    {
        if (mView == null)
        {
            return;
        }

        for (var i = 0; i < mView.Indicators.Count; i++)
        {
            var isActive = i == mCurrentIndex;
            mView.Indicators[i].color = isActive
                ? new Color(1f, 1f, 1f, 1f)
                : new Color(1f, 1f, 1f, 0.3f);
            mView.Indicators[i].rectTransform.localScale = isActive ? Vector3.one * 1.2f : Vector3.one;
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
            .SetEase(ease)
            .SetUpdate(mContext.UseUnscaledTime);
    }

    private static int WrapDelta(int delta, int count)
    {
        if (count <= 0)
        {
            return delta;
        }

        var half = count / 2;
        while (delta > half)
        {
            delta -= count;
        }

        while (delta < -half)
        {
            delta += count;
        }

        return delta;
    }

    private void RebuildView()
    {
        var index = mCurrentIndex;
        DestroyView();
        mView = CarouselSwipeCardsEffectViewBuilder.Build(mContext.MountRoot, mConfig, this, mModel);
        mDragging = false;
        mHovered = false;
        mAutoplayTimer = 0f;
        SetIndex(index, true);
    }

    private void DestroyView()
    {
        if (mView == null)
        {
            return;
        }

        if (mView.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mView = null;
    }
}
