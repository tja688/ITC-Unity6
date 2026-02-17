using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UGUICarouselReplica : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float mAutoplayDelay = 3.1f;
    [SerializeField] private bool mAutoplay = true;
    [SerializeField] private bool mPauseOnHover = true;
    [SerializeField] private bool mLoop = true;

    private RectTransform mRootRect;
    private RectTransform mCardStage;
    private readonly List<CarouselItemData> mItems = new();
    private readonly List<Image> mIndicators = new();

    private int mCurrentIndex;
    private float mAutoplayTimer;
    private bool mDragging;
    private bool mHovered;
    private Vector2 mDragStartPointer;
    private float mDragStartStageX;

    private static readonly string[] sTitles =
    {
        "Text Animations",
        "Animations",
        "Components",
        "Backgrounds",
        "Common UI"
    };

    private static readonly string[] sDescriptions =
    {
        "Cool text animations for your projects.",
        "Smooth animation recipes for interaction states.",
        "Reusable building blocks for rapid interfaces.",
        "Layered surfaces and visual atmosphere.",
        "Shared controls and practical UI patterns."
    };

    private static readonly Color[] sCardColors =
    {
        new(0.20f, 0.29f, 0.56f, 1f),
        new(0.10f, 0.47f, 0.53f, 1f),
        new(0.52f, 0.31f, 0.72f, 1f),
        new(0.70f, 0.43f, 0.28f, 1f),
        new(0.74f, 0.26f, 0.40f, 1f)
    };

    private static readonly Color[] sAccentColors =
    {
        new(0.42f, 0.58f, 0.94f, 0.9f),
        new(0.19f, 0.79f, 0.71f, 0.9f),
        new(0.72f, 0.52f, 0.97f, 0.9f),
        new(0.94f, 0.65f, 0.31f, 0.9f),
        new(0.97f, 0.44f, 0.58f, 0.9f)
    };

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("UGUICarouselReplica requires RectTransform.", this);
            return;
        }

        BuildView();
        SetIndex(0, true);
    }

    private void OnEnable()
    {
        if (mItems.Count == 0)
        {
            return;
        }

        mDragging = false;
        mAutoplayTimer = 0f;
        SetIndex(mCurrentIndex, true);
    }

    private void Update()
    {
        if (!mAutoplay || mDragging || (mPauseOnHover && mHovered) || mItems.Count <= 1)
        {
            return;
        }

        mAutoplayTimer += Time.unscaledDeltaTime;
        if (mAutoplayTimer < Mathf.Max(1.2f, mAutoplayDelay))
        {
            return;
        }

        mAutoplayTimer = 0f;
        SetIndex(mCurrentIndex + 1, false);
    }

    private void OnDisable()
    {
        mCardStage?.DOKill();
        foreach (var item in mItems)
        {
            item.Rect.DOKill();
            item.CanvasGroup.DOKill();
        }
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
        mDragging = true;
        mAutoplayTimer = 0f;
        mCardStage.DOKill();
        mDragStartPointer = eventData.position;
        mDragStartStageX = mCardStage.anchoredPosition.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!mDragging)
        {
            return;
        }

        var delta = eventData.position - mDragStartPointer;
        var targetX = Mathf.Clamp(mDragStartStageX + delta.x * 0.25f, -80f, 80f);
        mCardStage.anchoredPosition = new Vector2(targetX, mCardStage.anchoredPosition.y);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!mDragging)
        {
            return;
        }

        mDragging = false;
        var delta = eventData.position - mDragStartPointer;
        if (delta.x <= -80f)
        {
            SetIndex(mCurrentIndex + 1, false);
        }
        else if (delta.x >= 80f)
        {
            SetIndex(mCurrentIndex - 1, false);
        }
        else
        {
            AnimateCards(false);
        }
    }

    private void BuildView()
    {
        mRootRect.anchorMin = Vector2.zero;
        mRootRect.anchorMax = Vector2.one;
        mRootRect.offsetMin = Vector2.zero;
        mRootRect.offsetMax = Vector2.zero;

        var raycastImage = UGUIReplicaUIFactory.EnsureComponent<Image>(gameObject);
        raycastImage.color = new Color(1f, 1f, 1f, 0.001f);
        raycastImage.raycastTarget = true;

        var backdrop = UGUIReplicaUIFactory.CreatePanel("Backdrop", mRootRect, new Color(0.06f, 0.08f, 0.14f, 0.9f));
        Stretch(backdrop);

        var hint = UGUIReplicaUIFactory.CreateText(
            "Hint",
            backdrop,
            "Carousel  |  Drag cards or click indicators",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.93f, 0.96f, 1f, 0.95f));
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(880f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -40f);

        var frame = UGUIReplicaUIFactory.CreatePanel("Frame", backdrop, new Color(0.08f, 0.10f, 0.17f, 0.94f));
        frame.anchorMin = new Vector2(0.5f, 0.5f);
        frame.anchorMax = new Vector2(0.5f, 0.5f);
        frame.pivot = new Vector2(0.5f, 0.5f);
        frame.sizeDelta = new Vector2(1120f, 680f);
        frame.anchoredPosition = new Vector2(0f, -20f);

        var frameOutline = UGUIReplicaUIFactory.EnsureComponent<Outline>(frame.gameObject);
        frameOutline.effectColor = new Color(1f, 1f, 1f, 0.2f);
        frameOutline.effectDistance = new Vector2(1f, -1f);

        mCardStage = UGUIReplicaUIFactory.CreateRect("CardStage", frame);
        mCardStage.anchorMin = new Vector2(0.5f, 0.5f);
        mCardStage.anchorMax = new Vector2(0.5f, 0.5f);
        mCardStage.pivot = new Vector2(0.5f, 0.5f);
        mCardStage.sizeDelta = new Vector2(980f, 470f);
        mCardStage.anchoredPosition = new Vector2(0f, 22f);

        mItems.Clear();
        for (var i = 0; i < sTitles.Length; i++)
        {
            mItems.Add(CreateItem(i));
        }

        var indicatorRoot = UGUIReplicaUIFactory.CreateRect("Indicators", frame);
        indicatorRoot.anchorMin = new Vector2(0.5f, 0f);
        indicatorRoot.anchorMax = new Vector2(0.5f, 0f);
        indicatorRoot.pivot = new Vector2(0.5f, 0f);
        indicatorRoot.sizeDelta = new Vector2(360f, 36f);
        indicatorRoot.anchoredPosition = new Vector2(0f, 34f);

        var row = UGUIReplicaUIFactory.EnsureComponent<HorizontalLayoutGroup>(indicatorRoot.gameObject);
        row.childAlignment = TextAnchor.MiddleCenter;
        row.childControlHeight = false;
        row.childControlWidth = false;
        row.childForceExpandWidth = false;
        row.childForceExpandHeight = false;
        row.spacing = 18f;

        mIndicators.Clear();
        for (var i = 0; i < sTitles.Length; i++)
        {
            var index = i;
            var indicatorBtn = UGUIReplicaUIFactory.CreateButton($"Indicator_{i}", indicatorRoot, new Color(1f, 1f, 1f, 0.3f));
            var indicatorRect = (RectTransform)indicatorBtn.transform;
            indicatorRect.sizeDelta = new Vector2(14f, 14f);
            indicatorBtn.onClick.AddListener(() => SetIndex(index, false));
            mIndicators.Add(indicatorBtn.image);
        }
    }

    private CarouselItemData CreateItem(int index)
    {
        var itemRect = UGUIReplicaUIFactory.CreatePanel($"Item_{index}", mCardStage, sCardColors[index]);
        itemRect.anchorMin = new Vector2(0.5f, 0.5f);
        itemRect.anchorMax = new Vector2(0.5f, 0.5f);
        itemRect.pivot = new Vector2(0.5f, 0.5f);
        itemRect.sizeDelta = new Vector2(430f, 470f);
        itemRect.anchoredPosition = Vector2.zero;

        var itemGroup = UGUIReplicaUIFactory.EnsureComponent<CanvasGroup>(itemRect.gameObject);
        itemGroup.alpha = 1f;
        itemGroup.blocksRaycasts = false;
        itemGroup.interactable = false;

        var outline = UGUIReplicaUIFactory.EnsureComponent<Outline>(itemRect.gameObject);
        outline.effectColor = new Color(1f, 1f, 1f, 0.24f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        var accent = UGUIReplicaUIFactory.CreatePanel("Accent", itemRect, sAccentColors[index]);
        accent.anchorMin = new Vector2(0f, 1f);
        accent.anchorMax = new Vector2(1f, 1f);
        accent.pivot = new Vector2(0.5f, 1f);
        accent.sizeDelta = new Vector2(0f, 66f);
        accent.anchoredPosition = Vector2.zero;

        var iconCircle = UGUIReplicaUIFactory.CreatePanel("Icon", itemRect, Color.white);
        iconCircle.anchorMin = new Vector2(0f, 1f);
        iconCircle.anchorMax = new Vector2(0f, 1f);
        iconCircle.pivot = new Vector2(0f, 1f);
        iconCircle.sizeDelta = new Vector2(58f, 58f);
        iconCircle.anchoredPosition = new Vector2(20f, -82f);

        var iconText = UGUIReplicaUIFactory.CreateText(
            "IconText",
            iconCircle,
            ((char)('A' + index)).ToString(),
            24,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.05f, 0.07f, 0.10f, 1f));
        Stretch((RectTransform)iconText.transform);

        var title = UGUIReplicaUIFactory.CreateText(
            "Title",
            itemRect,
            sTitles[index],
            40,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            Color.white);
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0f, 1f);
        titleRect.sizeDelta = new Vector2(-34f, 96f);
        titleRect.anchoredPosition = new Vector2(96f, -92f);

        var desc = UGUIReplicaUIFactory.CreateText(
            "Description",
            itemRect,
            sDescriptions[index],
            28,
            FontStyle.Normal,
            TextAnchor.UpperLeft,
            new Color(1f, 1f, 1f, 0.92f));
        var descRect = (RectTransform)desc.transform;
        descRect.anchorMin = new Vector2(0f, 0f);
        descRect.anchorMax = new Vector2(1f, 1f);
        descRect.pivot = new Vector2(0f, 0f);
        descRect.sizeDelta = new Vector2(-54f, -220f);
        descRect.anchoredPosition = new Vector2(24f, 28f);

        return new CarouselItemData
        {
            Rect = itemRect,
            CanvasGroup = itemGroup
        };
    }

    private void SetIndex(int nextIndex, bool immediate)
    {
        if (mItems.Count == 0)
        {
            return;
        }

        if (mLoop)
        {
            if (nextIndex < 0)
            {
                nextIndex = mItems.Count - 1;
            }
            else if (nextIndex >= mItems.Count)
            {
                nextIndex = 0;
            }
        }
        else
        {
            nextIndex = Mathf.Clamp(nextIndex, 0, mItems.Count - 1);
        }

        mCurrentIndex = nextIndex;
        AnimateCards(immediate);
        UpdateIndicators();
        mAutoplayTimer = 0f;
    }

    private void AnimateCards(bool immediate)
    {
        mCardStage.DOKill();
        mCardStage
            .DOAnchorPosX(0f, immediate ? 0f : 0.18f)
            .SetEase(Ease.OutCubic);

        var count = mItems.Count;
        for (var i = 0; i < count; i++)
        {
            var card = mItems[i];
            var delta = WrapDelta(i - mCurrentIndex, count);
            var abs = Mathf.Abs(delta);

            var targetX = delta * 320f;
            var targetY = -abs * 18f;
            var targetScale = abs == 0 ? 1f : (abs == 1 ? 0.86f : 0.74f);
            var targetRot = -delta * 9f;
            var targetAlpha = abs == 0 ? 1f : (abs == 1 ? 0.58f : 0.18f);

            card.Rect.SetSiblingIndex(abs == 0 ? count - 1 : (count - 1 - abs));
            card.Rect.DOKill();
            card.CanvasGroup.DOKill();

            if (immediate)
            {
                card.Rect.anchoredPosition = new Vector2(targetX, targetY);
                card.Rect.localScale = Vector3.one * targetScale;
                card.Rect.localRotation = Quaternion.Euler(0f, 0f, targetRot);
                card.CanvasGroup.alpha = targetAlpha;
                continue;
            }

            card.Rect
                .DOAnchorPos(new Vector2(targetX, targetY), 0.45f)
                .SetEase(Ease.OutCubic);
            card.Rect
                .DOScale(targetScale, 0.45f)
                .SetEase(Ease.OutCubic);
            card.Rect
                .DORotate(new Vector3(0f, 0f, targetRot), 0.45f)
                .SetEase(Ease.OutCubic);
            card.CanvasGroup
                .DOFade(targetAlpha, 0.35f)
                .SetEase(Ease.OutCubic);
        }
    }

    private void UpdateIndicators()
    {
        for (var i = 0; i < mIndicators.Count; i++)
        {
            var isActive = i == mCurrentIndex;
            mIndicators[i].color = isActive
                ? new Color(1f, 1f, 1f, 1f)
                : new Color(1f, 1f, 1f, 0.3f);
            mIndicators[i].rectTransform.localScale = isActive ? Vector3.one * 1.2f : Vector3.one;
        }
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

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private struct CarouselItemData
    {
        public RectTransform Rect;
        public CanvasGroup CanvasGroup;
    }
}
