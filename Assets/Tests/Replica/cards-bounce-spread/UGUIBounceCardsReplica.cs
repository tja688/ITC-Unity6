using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUIBounceCardsReplica : MonoBehaviour
{
    [SerializeField] private Vector2 mCardSize = new(240f, 240f);
    [SerializeField] private float mHoverPushOffset = 160f;
    [SerializeField] private float mEntryDelay = 0.42f;
    [SerializeField] private float mEntryStagger = 0.08f;

    private readonly List<BounceCardData> mCards = new();
    private RectTransform mContainer;
    private int mCurrentHoveredIndex = -1;

    private static readonly Vector2[] sBaseOffsets =
    {
        new(-320f, -8f),
        new(-160f, 10f),
        Vector2.zero,
        new(160f, -8f),
        new(320f, 6f)
    };

    private static readonly float[] sBaseRotations = { 10f, 5f, -3f, -10f, 2f };
    private static readonly Color[] sCardColors =
    {
        new(0.20f, 0.49f, 0.82f, 1f),
        new(0.34f, 0.62f, 0.44f, 1f),
        new(0.77f, 0.42f, 0.30f, 1f),
        new(0.47f, 0.35f, 0.77f, 1f),
        new(0.87f, 0.66f, 0.29f, 1f)
    };

    private void Awake()
    {
        BuildView();
    }

    private void OnEnable()
    {
        if (mCards.Count == 0)
        {
            return;
        }

        ResetCardsImmediate();
        PlayEntryAnimation();
    }

    private void OnDisable()
    {
        foreach (var card in mCards)
        {
            card.Rect.DOKill();
        }

        mCurrentHoveredIndex = -1;
    }

    private void BuildView()
    {
        var root = (RectTransform)transform;
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;

        var backdrop = UGUIReplicaUIFactory.CreatePanel("Backdrop", root, new Color(0.05f, 0.08f, 0.14f, 0.82f));
        backdrop.anchorMin = new Vector2(0.5f, 0.5f);
        backdrop.anchorMax = new Vector2(0.5f, 0.5f);
        backdrop.pivot = new Vector2(0.5f, 0.5f);
        backdrop.sizeDelta = new Vector2(1450f, 760f);
        backdrop.anchoredPosition = new Vector2(0f, -10f);

        var plateA = UGUIReplicaUIFactory.CreatePanel("PlateA", backdrop, new Color(0.11f, 0.21f, 0.33f, 0.45f));
        plateA.anchorMin = new Vector2(0.5f, 0.5f);
        plateA.anchorMax = new Vector2(0.5f, 0.5f);
        plateA.sizeDelta = new Vector2(1000f, 420f);
        plateA.anchoredPosition = new Vector2(-110f, 30f);
        plateA.localRotation = Quaternion.Euler(0f, 0f, -8f);

        var plateB = UGUIReplicaUIFactory.CreatePanel("PlateB", backdrop, new Color(0.22f, 0.16f, 0.30f, 0.40f));
        plateB.anchorMin = new Vector2(0.5f, 0.5f);
        plateB.anchorMax = new Vector2(0.5f, 0.5f);
        plateB.sizeDelta = new Vector2(980f, 380f);
        plateB.anchoredPosition = new Vector2(100f, -46f);
        plateB.localRotation = Quaternion.Euler(0f, 0f, 6f);

        mContainer = UGUIReplicaUIFactory.CreateRect("Cards", backdrop);
        mContainer.anchorMin = new Vector2(0.5f, 0.5f);
        mContainer.anchorMax = new Vector2(0.5f, 0.5f);
        mContainer.pivot = new Vector2(0.5f, 0.5f);
        mContainer.sizeDelta = new Vector2(1200f, 520f);
        mContainer.anchoredPosition = new Vector2(0f, 0f);

        var title = UGUIReplicaUIFactory.CreateText(
            "Hint",
            backdrop,
            "BounceCards  |  Hover a card to push siblings",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.90f, 0.94f, 1f, 0.95f));
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(980f, 56f);
        titleRect.anchoredPosition = new Vector2(0f, -40f);

        mCards.Clear();
        for (var i = 0; i < sBaseOffsets.Length; i++)
        {
            var card = CreateCard(i);
            mCards.Add(card);
        }
    }

    private BounceCardData CreateCard(int index)
    {
        var cardRect = UGUIReplicaUIFactory.CreateRect($"Card_{index}", mContainer);
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = mCardSize;
        cardRect.anchoredPosition = sBaseOffsets[index];
        cardRect.localRotation = Quaternion.Euler(0f, 0f, sBaseRotations[index]);

        var cardImage = UGUIReplicaUIFactory.EnsureComponent<Image>(cardRect.gameObject);
        cardImage.color = sCardColors[index];
        cardImage.raycastTarget = true;

        var border = UGUIReplicaUIFactory.EnsureComponent<Outline>(cardRect.gameObject);
        border.effectColor = Color.white;
        border.effectDistance = new Vector2(4f, -4f);
        border.useGraphicAlpha = true;

        var shadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(cardRect.gameObject);
        shadow.effectColor = new Color(0f, 0f, 0f, 0.3f);
        shadow.effectDistance = new Vector2(0f, -10f);
        shadow.useGraphicAlpha = true;

        var label = UGUIReplicaUIFactory.CreateText(
            "Label",
            cardRect,
            $"Card {index + 1}",
            32,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(1f, 1f, 1f, 0.96f));
        var labelRect = (RectTransform)label.transform;
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        var hover = cardRect.gameObject.AddComponent<BounceHoverRelay>();
        hover.Initialize(index, this);

        return new BounceCardData
        {
            Rect = cardRect,
            BasePos = sBaseOffsets[index],
            BaseRot = sBaseRotations[index]
        };
    }

    private void PlayEntryAnimation()
    {
        for (var i = 0; i < mCards.Count; i++)
        {
            var card = mCards[i];
            card.Rect.localScale = Vector3.zero;
            card.Rect
                .DOScale(Vector3.one, 0.7f)
                .SetDelay(mEntryDelay + (i * mEntryStagger))
                .SetEase(Ease.OutElastic);
        }
    }

    private void ResetCardsImmediate()
    {
        for (var i = 0; i < mCards.Count; i++)
        {
            var card = mCards[i];
            card.Rect.DOKill();
            card.Rect.anchoredPosition = card.BasePos;
            card.Rect.localRotation = Quaternion.Euler(0f, 0f, card.BaseRot);
            card.Rect.localScale = Vector3.one;
        }

        mCurrentHoveredIndex = -1;
    }

    internal void OnCardHovered(int hoveredIndex)
    {
        // Skip redundant updates when hovering the same card
        if (hoveredIndex == mCurrentHoveredIndex)
        {
            return;
        }

        mCurrentHoveredIndex = hoveredIndex;

        for (var i = 0; i < mCards.Count; i++)
        {
            var card = mCards[i];

            if (i == hoveredIndex)
            {
                // Hovered card: straighten rotation, slight lift
                card.Rect
                    .DOAnchorPos(card.BasePos + new Vector2(0f, 18f), 0.4f)
                    .SetEase(Ease.OutBack, 1.4f)
                    .SetId(card.Rect.GetInstanceID());
                card.Rect
                    .DOLocalRotate(Vector3.zero, 0.4f)
                    .SetEase(Ease.OutBack, 1.4f)
                    .SetId(card.Rect.GetInstanceID() + 10000);
                continue;
            }

            var direction = i < hoveredIndex ? -1f : 1f;
            var distance = Mathf.Abs(i - hoveredIndex);
            var targetPos = card.BasePos + new Vector2(direction * mHoverPushOffset, 0f);
            var delay = distance * 0.03f;

            card.Rect
                .DOAnchorPos(targetPos, 0.4f)
                .SetDelay(delay)
                .SetEase(Ease.OutBack, 1.4f)
                .SetId(card.Rect.GetInstanceID());
            card.Rect
                .DOLocalRotate(new Vector3(0f, 0f, card.BaseRot), 0.4f)
                .SetDelay(delay)
                .SetEase(Ease.OutBack, 1.4f)
                .SetId(card.Rect.GetInstanceID() + 10000);
        }
    }

    internal void ResetCards()
    {
        mCurrentHoveredIndex = -1;

        for (var i = 0; i < mCards.Count; i++)
        {
            var card = mCards[i];
            card.Rect
                .DOAnchorPos(card.BasePos, 0.4f)
                .SetEase(Ease.OutBack, 1.4f)
                .SetId(card.Rect.GetInstanceID());
            card.Rect
                .DOLocalRotate(new Vector3(0f, 0f, card.BaseRot), 0.4f)
                .SetEase(Ease.OutBack, 1.4f)
                .SetId(card.Rect.GetInstanceID() + 10000);
        }
    }

    private struct BounceCardData
    {
        public RectTransform Rect;
        public Vector2 BasePos;
        public float BaseRot;
    }

    private sealed class BounceHoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private int mIndex;
        private UGUIBounceCardsReplica mOwner;

        public void Initialize(int index, UGUIBounceCardsReplica owner)
        {
            mIndex = index;
            mOwner = owner;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mOwner?.OnCardHovered(mIndex);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mOwner?.ResetCards();
        }
    }
}
