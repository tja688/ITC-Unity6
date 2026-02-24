using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class StackEffectController : IReplicaEffect<StackEffectConfig, StackEffectModel>
{
    private sealed class CardRuntime
    {
        public string Id;
        public StackItemData Data;
        public StackCardView View;
        public float RandomOffset;
        public float LayoutRotationZ;
        public float LayoutScale = 1f;
        public bool IsDragging;
    }

    private readonly Dictionary<string, CardRuntime> mCardsById = new Dictionary<string, CardRuntime>();
    private readonly List<string> mOrder = new List<string>();
    private readonly HashSet<string> mPendingRemoval = new HashSet<string>();
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private StackEffectConfig mConfig;
    private StackEffectView mView;

    private bool mInitialized;
    private string mDraggingId;
    private Vector2 mPressPoint;
    private Vector2 mDragOffset;

    public string EffectId => "stack-v2";

    public void Initialize(ReplicaHostContext context, StackEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<StackEffectConfig>();
        mView = StackEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        mInitialized = true;
    }

    public void SetModel(StackEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safeModel = model ?? new StackEffectModel();
        var incoming = safeModel.Items != null ? safeModel.Items : new List<StackItemData>();

        var newOrder = new List<string>(incoming.Count);
        for (var i = 0; i < incoming.Count; i++)
        {
            var item = incoming[i] ?? new StackItemData();
            var id = NormalizeId(item.Id, i);
            item.Id = id;
            newOrder.Add(id);

            if (!mCardsById.TryGetValue(id, out var runtime))
            {
                runtime = CreateCardRuntime(id, item, animated);
                mCardsById.Add(id, runtime);
            }
            else
            {
                runtime.Data = item.Clone();
                ApplyVisual(runtime);
            }
        }

        RemoveMissingCards(newOrder, animated);

        mOrder.Clear();
        mOrder.AddRange(newOrder);

        ApplyLayout(animated);
        UpdateInteractivity();
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (!mInitialized || mView == null)
        {
            return;
        }

        KillTransitionTweens();

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var offset = EnterOffset(transition.EnterDirection, mConfig.EnterOffset);

        mView.Root.anchoredPosition = offset;
        mView.Group.alpha = 0f;

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Root, Vector2.zero, duration, mView.Root)
            .SetEase(transition.Ease));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Root, transition.Ease));
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        if (!mInitialized || mView == null)
        {
            onComplete?.Invoke();
            return;
        }

        KillTransitionTweens();

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var offset = ExitOffset(transition.ExitDirection, mConfig.ExitOffset);

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Root, offset, duration, mView.Root)
            .SetEase(transition.Ease)
            .OnComplete(onComplete));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 0f, duration, mView.Root, transition.Ease));
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

        foreach (var kv in mCardsById)
        {
            if (kv.Value?.View?.Root != null)
            {
                mContext.Tweens.Kill(kv.Value.View.Root);
                UnityEngine.Object.Destroy(kv.Value.View.Root.gameObject);
            }
        }

        mCardsById.Clear();
        mOrder.Clear();
        mPendingRemoval.Clear();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
        mDraggingId = null;
    }

    public void OnCardBeginDrag(string id, PointerEventData eventData)
    {
        if (!CanDragCard(id) || mDraggingId != null)
        {
            return;
        }

        if (!mCardsById.TryGetValue(id, out var card))
        {
            return;
        }

        mDraggingId = id;
        card.IsDragging = true;
        mDragOffset = Vector2.zero;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mContext.MountRoot,
                eventData.position,
                eventData.pressEventCamera,
                out mPressPoint))
        {
            mPressPoint = Vector2.zero;
        }

        mContext.Tweens.Kill(card.View.Root);
    }

    public void OnCardDrag(string id, PointerEventData eventData)
    {
        if (mDraggingId != id)
        {
            return;
        }

        if (!mCardsById.TryGetValue(id, out var card))
        {
            return;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mContext.MountRoot,
                eventData.position,
                eventData.pressEventCamera,
                out var localPoint))
        {
            return;
        }

        mDragOffset = localPoint - mPressPoint;
        card.View.Root.anchoredPosition = mDragOffset;

        var normalizedX = Mathf.Clamp(mDragOffset.x / 120f, -1f, 1f);
        var normalizedY = Mathf.Clamp(mDragOffset.y / 120f, -1f, 1f);
        var rotX = -normalizedY * mConfig.MaxTilt;
        var rotY = normalizedX * mConfig.MaxTilt;

        card.View.Tilt.localRotation = Quaternion.Euler(rotX, rotY, 0f);

        var lift = Mathf.Clamp01(mDragOffset.magnitude / 260f);
        var shadowAlpha = Mathf.Lerp(mConfig.ShadowColor.a, 0.12f, lift);
        card.View.Shadow.anchoredPosition = new Vector2(rotY * 1.2f, -10f - (rotX * 0.8f) - (18f * lift));
        var shadowScale = 1f + (0.22f * lift);
        card.View.Shadow.localScale = new Vector3(shadowScale, shadowScale, 1f);

        var color = card.View.ShadowImage.color;
        color.a = shadowAlpha;
        card.View.ShadowImage.color = color;
    }

    public void OnCardEndDrag(string id, PointerEventData eventData)
    {
        if (mDraggingId != id)
        {
            return;
        }

        if (!mCardsById.TryGetValue(id, out var card))
        {
            mDraggingId = null;
            return;
        }

        card.IsDragging = false;
        mDraggingId = null;

        if (!IsTopCard(id))
        {
            TweenCardBack(card, mConfig.ReturnDuration);
            return;
        }

        var shouldSendToBack = Mathf.Abs(mDragOffset.x) > mConfig.DragSensitivity || Mathf.Abs(mDragOffset.y) > mConfig.DragSensitivity;
        if (shouldSendToBack)
        {
            SendToBack(id, true);
        }
        else
        {
            TweenCardBack(card, mConfig.ReturnDuration);
        }
    }

    public void OnCardClick(string id, PointerEventData eventData)
    {
        if (!mConfig.SendToBackOnClick || mDraggingId != null)
        {
            return;
        }

        if (!IsTopCard(id))
        {
            return;
        }

        SendToBack(id, true);
    }

    private CardRuntime CreateCardRuntime(string id, StackItemData item, bool animated)
    {
        var view = StackEffectViewBuilder.CreateCard(mView.Deck, mConfig, this);
        view.InputRelay.SetItemId(id);

        var runtime = new CardRuntime
        {
            Id = id,
            Data = item.Clone(),
            View = view,
            RandomOffset = ComputeRandomOffset(id),
            LayoutRotationZ = 0f,
            LayoutScale = 1f
        };

        ApplyVisual(runtime);

        if (animated)
        {
            runtime.View.Group.alpha = 0f;
            runtime.View.Root.anchoredPosition = new Vector2(0f, 80f);
            runtime.View.Visual.localScale = Vector3.one * 0.9f;
        }

        return runtime;
    }

    private void ApplyVisual(CardRuntime runtime)
    {
        var item = runtime.Data;
        runtime.View.TitleText.text = string.IsNullOrWhiteSpace(item.Title) ? "Untitled" : item.Title;
        runtime.View.MetaText.text = string.IsNullOrWhiteSpace(item.Meta) ? "Drag to cycle stack" : item.Meta;

        if (item.Sprite != null)
        {
            runtime.View.VisualImage.sprite = item.Sprite;
            runtime.View.VisualImage.type = mConfig.SpriteImageType;
            runtime.View.VisualImage.color = Color.white;
        }
        else
        {
            runtime.View.VisualImage.sprite = null;
            runtime.View.VisualImage.type = Image.Type.Simple;
            runtime.View.VisualImage.color = item.Tint.a > 0f ? item.Tint : mConfig.FallbackCardColor;
        }
    }

    private void RemoveMissingCards(List<string> newOrder, bool animated)
    {
        var keepSet = new HashSet<string>(newOrder);
        var toRemove = new List<string>();

        foreach (var id in mCardsById.Keys)
        {
            if (!keepSet.Contains(id))
            {
                toRemove.Add(id);
            }
        }

        for (var i = 0; i < toRemove.Count; i++)
        {
            var id = toRemove[i];
            if (!mCardsById.TryGetValue(id, out var runtime))
            {
                continue;
            }

            mContext.Tweens.Kill(runtime.View.Root);

            if (!animated)
            {
                UnityEngine.Object.Destroy(runtime.View.Root.gameObject);
                mCardsById.Remove(id);
                mPendingRemoval.Remove(id);
                continue;
            }

            if (!mPendingRemoval.Add(id))
            {
                continue;
            }

            mContext.Tweens
                .ScaleTo(runtime.View.Visual, Vector3.one * 0.84f, Mathf.Max(0.1f, mConfig.ReturnDuration), runtime.View.Root)
                .SetEase(ReplicaEase.OutCubic);

            FadeCanvasGroup(runtime.View.Group, 0f, Mathf.Max(0.1f, mConfig.ReturnDuration), runtime.View.Root, ReplicaEase.OutQuad)
                .OnComplete(() =>
                {
                    if (runtime.View != null && runtime.View.Root != null)
                    {
                        UnityEngine.Object.Destroy(runtime.View.Root.gameObject);
                    }

                    mCardsById.Remove(id);
                    mPendingRemoval.Remove(id);
                });
        }
    }

    private void ApplyLayout(bool animated)
    {
        var count = mOrder.Count;
        if (count == 0)
        {
            return;
        }

        var randomOffsets = new List<float>(count);
        for (var i = 0; i < count; i++)
        {
            var id = mOrder[i];
            randomOffsets.Add(mCardsById[id].RandomOffset);
        }

        var states = StackEffectLayoutSolver.Solve(count, mConfig, randomOffsets);
        var duration = Mathf.Max(0.08f, mConfig.LayoutDuration);

        for (var i = 0; i < count; i++)
        {
            var id = mOrder[i];
            if (!mCardsById.TryGetValue(id, out var runtime))
            {
                continue;
            }

            var state = states[i];
            runtime.LayoutRotationZ = state.RotationZ;
            runtime.LayoutScale = state.Scale;

            runtime.View.Root.SetSiblingIndex(state.SiblingIndex);

            if (runtime.IsDragging)
            {
                continue;
            }

            if (!animated)
            {
                runtime.View.Root.anchoredPosition = state.Position;
                runtime.View.Tilt.localRotation = Quaternion.identity;
                runtime.View.Visual.localRotation = Quaternion.Euler(0f, 0f, state.RotationZ);
                runtime.View.Visual.localScale = new Vector3(state.Scale, state.Scale, 1f);
                runtime.View.Group.alpha = 1f;
                continue;
            }

            mContext.Tweens.Kill(runtime.View.Root);
            mContext.Tweens
                .AnchoredPosTo(runtime.View.Root, state.Position, duration, runtime.View.Root)
                .SetEase(ReplicaEase.OutCubic);
            mContext.Tweens
                .LocalRotateTo(runtime.View.Visual, new Vector3(0f, 0f, state.RotationZ), duration, runtime.View.Root)
                .SetEase(ReplicaEase.OutBack);
            mContext.Tweens
                .ScaleTo(runtime.View.Visual, new Vector3(state.Scale, state.Scale, 1f), duration, runtime.View.Root)
                .SetEase(ReplicaEase.OutQuad);

            if (runtime.View.Group.alpha < 0.999f)
            {
                FadeCanvasGroup(runtime.View.Group, 1f, duration, runtime.View.Root, ReplicaEase.OutQuad);
            }
        }
    }

    private void UpdateInteractivity()
    {
        for (var i = 0; i < mOrder.Count; i++)
        {
            var id = mOrder[i];
            if (!mCardsById.TryGetValue(id, out var runtime))
            {
                continue;
            }

            var interactive = i == mOrder.Count - 1;
            runtime.View.HitImage.raycastTarget = interactive;
            runtime.View.InputRelay.enabled = interactive;
            runtime.View.Group.blocksRaycasts = interactive;
        }
    }

    private bool CanDragCard(string id)
    {
        return mConfig.AllowDrag && IsTopCard(id);
    }

    private bool IsTopCard(string id)
    {
        return !string.IsNullOrEmpty(id) && mOrder.Count > 0 && mOrder[mOrder.Count - 1] == id;
    }

    private void SendToBack(string id, bool animated)
    {
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        if (!mOrder.Remove(id))
        {
            return;
        }

        mOrder.Insert(0, id);
        ApplyLayout(animated);
        UpdateInteractivity();
    }

    private void TweenCardBack(CardRuntime card, float duration)
    {
        var safe = Mathf.Max(0.06f, duration);
        card.View.Tilt.localRotation = Quaternion.identity;

        mContext.Tweens.Kill(card.View.Root);
        mContext.Tweens
            .AnchoredPosTo(card.View.Root, Vector2.zero, safe, card.View.Root)
            .SetEase(ReplicaEase.OutCubic);
        mContext.Tweens
            .LocalRotateTo(card.View.Tilt, Vector3.zero, safe, card.View.Root)
            .SetEase(ReplicaEase.OutCubic);
        mContext.Tweens
            .LocalRotateTo(card.View.Visual, new Vector3(0f, 0f, card.LayoutRotationZ), safe, card.View.Root)
            .SetEase(ReplicaEase.OutQuad);
        mContext.Tweens
            .ScaleTo(card.View.Visual, new Vector3(card.LayoutScale, card.LayoutScale, 1f), safe, card.View.Root)
            .SetEase(ReplicaEase.OutQuad);

        var shadowColor = card.View.ShadowImage.color;
        shadowColor.a = mConfig.ShadowColor.a;
        card.View.ShadowImage.color = shadowColor;
        card.View.Shadow.anchoredPosition = new Vector2(0f, -10f);
        card.View.Shadow.localScale = Vector3.one;
    }

    private IReplicaTweenHandle FadeCanvasGroup(CanvasGroup group, float targetAlpha, float duration, UnityEngine.Object owner, ReplicaEase ease)
    {
        var start = group.alpha;
        return mContext.Tweens
            .ToFloat(
                () => start,
                value =>
                {
                    start = value;
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

    private static string NormalizeId(string id, int fallbackIndex)
    {
        return string.IsNullOrWhiteSpace(id) ? $"stack-item-{fallbackIndex}" : id.Trim();
    }

    private static float ComputeRandomOffset(string id)
    {
        unchecked
        {
            var hash = 17;
            for (var i = 0; i < id.Length; i++)
            {
                hash = (hash * 31) + id[i];
            }

            var normalized = ((hash & 0x7fffffff) % 1000) / 999f;
            return (normalized - 0.5f) * 10f;
        }
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
