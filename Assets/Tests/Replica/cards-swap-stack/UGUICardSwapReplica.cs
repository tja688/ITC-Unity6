using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUICardSwapReplica : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float mCardDistance = 72f;
    [SerializeField] private float mVerticalDistance = 66f;
    [SerializeField] private float mSwapDelay = 4.2f;
    [SerializeField] private bool mPauseOnHover = true;

    private readonly List<CardData> mCards = new();
    private RectTransform mDeck;
    private float mSwapTimer;
    private bool mHoverPaused;
    private bool mAnimating;

    private static readonly Color[] sCardColors =
    {
        new(0.20f, 0.42f, 0.70f, 1f),
        new(0.17f, 0.56f, 0.50f, 1f),
        new(0.70f, 0.41f, 0.24f, 1f),
        new(0.46f, 0.33f, 0.72f, 1f)
    };

    private void Awake()
    {
        BuildView();
        ApplySlotLayout(false, 0f);
        mSwapTimer = mSwapDelay * 0.25f;
    }

    private void OnEnable()
    {
        if (mCards.Count == 0)
        {
            return;
        }

        mAnimating = false;
        mSwapTimer = 0f;
        ApplySlotLayout(false, 0f);
    }

    private void Update()
    {
        if (mAnimating || mCards.Count <= 1 || (mPauseOnHover && mHoverPaused))
        {
            return;
        }

        mSwapTimer += Time.unscaledDeltaTime;
        if (mSwapTimer < Mathf.Max(1.5f, mSwapDelay))
        {
            return;
        }

        mSwapTimer = 0f;
        StartSwap();
    }

    private void OnDisable()
    {
        foreach (var card in mCards)
        {
            card.Rect.DOKill();
            card.Visual.DOKill();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mHoverPaused = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mHoverPaused = false;
    }

    private void BuildView()
    {
        var root = (RectTransform)transform;
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;

        var backdrop = UGUIReplicaUIFactory.CreatePanel("Backdrop", root, new Color(0.07f, 0.09f, 0.14f, 0.86f));
        Stretch(backdrop);

        var hint = UGUIReplicaUIFactory.CreateText(
            "Hint",
            backdrop,
            "CardSwap  |  Auto swaps top card to back",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.93f, 0.95f, 1f, 0.95f));
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(900f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -40f);

        var surface = UGUIReplicaUIFactory.CreatePanel("Surface", backdrop, new Color(0.10f, 0.13f, 0.21f, 0.82f));
        surface.anchorMin = new Vector2(0.5f, 0.5f);
        surface.anchorMax = new Vector2(0.5f, 0.5f);
        surface.pivot = new Vector2(0.5f, 0.5f);
        surface.sizeDelta = new Vector2(1320f, 700f);
        surface.anchoredPosition = new Vector2(0f, -12f);

        var shadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(surface.gameObject);
        shadow.effectColor = new Color(0f, 0f, 0f, 0.35f);
        shadow.effectDistance = new Vector2(0f, -9f);

        mDeck = UGUIReplicaUIFactory.CreateRect("Deck", surface);
        mDeck.anchorMin = new Vector2(0.5f, 0.5f);
        mDeck.anchorMax = new Vector2(0.5f, 0.5f);
        mDeck.pivot = new Vector2(0.5f, 0.5f);
        mDeck.sizeDelta = new Vector2(840f, 580f);
        mDeck.anchoredPosition = new Vector2(0f, 0f);

        mCards.Clear();
        for (var i = 0; i < sCardColors.Length; i++)
        {
            mCards.Add(CreateCard(i));
        }
    }

    private CardData CreateCard(int index)
    {
        var cardRect = UGUIReplicaUIFactory.CreateRect($"Card_{index + 1}", mDeck);
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = new Vector2(480f, 340f);
        cardRect.anchoredPosition = Vector2.zero;

        var cardImage = UGUIReplicaUIFactory.EnsureComponent<Image>(cardRect.gameObject);
        cardImage.color = sCardColors[index];
        cardImage.raycastTarget = true;

        var outline = UGUIReplicaUIFactory.EnsureComponent<Outline>(cardRect.gameObject);
        outline.effectColor = new Color(1f, 1f, 1f, 0.6f);
        outline.effectDistance = new Vector2(1.8f, -1.8f);

        var label = UGUIReplicaUIFactory.CreateText(
            "Title",
            cardRect,
            $"Panel {index + 1}",
            42,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        Stretch((RectTransform)label.transform);

        var caption = UGUIReplicaUIFactory.CreateText(
            "Caption",
            cardRect,
            "Click the front card to swap now",
            24,
            FontStyle.Normal,
            TextAnchor.LowerCenter,
            new Color(1f, 1f, 1f, 0.86f));
        var captionRect = (RectTransform)caption.transform;
        captionRect.anchorMin = new Vector2(0.5f, 0f);
        captionRect.anchorMax = new Vector2(0.5f, 0f);
        captionRect.pivot = new Vector2(0.5f, 0f);
        captionRect.sizeDelta = new Vector2(420f, 42f);
        captionRect.anchoredPosition = new Vector2(0f, 18f);

        var clickProxy = cardRect.gameObject.AddComponent<CardSwapClickRelay>();
        clickProxy.Initialize(this, cardRect);

        return new CardData
        {
            Rect = cardRect,
            Visual = cardRect
        };
    }

    private void StartSwap()
    {
        if (mAnimating || mCards.Count <= 1)
        {
            return;
        }

        mAnimating = true;
        var front = mCards[0];
        var seq = DOTween.Sequence();
        seq.Append(front.Rect.DOAnchorPosY(front.Rect.anchoredPosition.y - 500f, 0.85f).SetEase(Ease.OutElastic));
        seq.AppendCallback(() =>
        {
            _ = mCards.Remove(front);
            mCards.Add(front);
            ApplySlotLayout(true, 1.45f);
        });
        seq.AppendInterval(1.50f);
        seq.OnComplete(() =>
        {
            mAnimating = false;
        });
    }

    private void ApplySlotLayout(bool animate, float duration)
    {
        for (var i = 0; i < mCards.Count; i++)
        {
            var card = mCards[i];
            var centerBias = (mCards.Count - 1) * 0.5f;
            var slotPos = new Vector2(
                (i - centerBias) * mCardDistance,
                (centerBias - i) * mVerticalDistance * 0.72f);
            var slotScale = Mathf.Clamp(1f - (i * 0.07f), 0.70f, 1f);
            var slotRot = i * 3.5f;

            card.Rect.SetSiblingIndex(mCards.Count - i - 1);
            card.Rect.DOKill();
            card.Visual.DOKill();

            if (!animate)
            {
                card.Rect.anchoredPosition = slotPos;
                card.Visual.localScale = new Vector3(slotScale, slotScale, 1f);
                card.Visual.localRotation = Quaternion.Euler(0f, 0f, slotRot);
                continue;
            }

            var delay = i * 0.12f;
            card.Rect
                .DOAnchorPos(slotPos, duration)
                .SetDelay(delay)
                .SetEase(Ease.OutElastic);
            card.Visual
                .DOScale(slotScale, duration)
                .SetDelay(delay)
                .SetEase(Ease.OutCubic);
            card.Visual
                .DORotate(new Vector3(0f, 0f, slotRot), duration)
                .SetDelay(delay)
                .SetEase(Ease.OutCubic);
        }
    }

    internal void HandleCardClicked(RectTransform card)
    {
        if (mCards.Count == 0 || mCards[0].Rect != card)
        {
            return;
        }

        mSwapTimer = 0f;
        StartSwap();
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private struct CardData
    {
        public RectTransform Rect;
        public RectTransform Visual;
    }

    private sealed class CardSwapClickRelay : MonoBehaviour, IPointerClickHandler
    {
        private UGUICardSwapReplica mOwner;
        private RectTransform mRect;

        public void Initialize(UGUICardSwapReplica owner, RectTransform rect)
        {
            mOwner = owner;
            mRect = rect;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            mOwner?.HandleCardClicked(mRect);
        }
    }
}
