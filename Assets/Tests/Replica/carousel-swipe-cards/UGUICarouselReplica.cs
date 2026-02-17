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
    private RectTransform mViewport;
    private RectTransform mTrack;

    private readonly List<CarouselItemData> mItems = new();
    private readonly List<Image> mIndicators = new();

    private int mCurrentIndex;
    private float mAutoplayTimer;
    private bool mDragging;
    private bool mHovered;
    private Vector2 mDragStartPointer;
    private float mDragStartTrackX;

    private const float ItemWidth = 390f;
    private const float ItemHeight = 460f;
    private const float Gap = 16f;

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
        new(0.12f, 0.17f, 0.32f, 1f),
        new(0.11f, 0.24f, 0.34f, 1f),
        new(0.22f, 0.16f, 0.33f, 1f),
        new(0.18f, 0.20f, 0.36f, 1f),
        new(0.28f, 0.17f, 0.22f, 1f)
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
        mTrack?.DOKill();
        foreach (var item in mItems)
        {
            item.Rect.DOKill();
            item.Rect3D.DOKill();
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
        mTrack.DOKill();
        mDragStartPointer = eventData.position;
        mDragStartTrackX = mTrack.anchoredPosition.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!mDragging)
        {
            return;
        }

        var delta = eventData.position - mDragStartPointer;
        mTrack.anchoredPosition = new Vector2(mDragStartTrackX + delta.x, mTrack.anchoredPosition.y);
        UpdateCardPerspective();
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
            AnimateTrackToIndex(false);
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

        mViewport = UGUIReplicaUIFactory.CreateRect("Viewport", frame);
        mViewport.anchorMin = new Vector2(0.5f, 0.5f);
        mViewport.anchorMax = new Vector2(0.5f, 0.5f);
        mViewport.pivot = new Vector2(0.5f, 0.5f);
        mViewport.sizeDelta = new Vector2(422f, 500f);
        mViewport.anchoredPosition = new Vector2(0f, 24f);
        UGUIReplicaUIFactory.EnsureComponent<Image>(mViewport.gameObject).color = new Color(1f, 1f, 1f, 0f);
        UGUIReplicaUIFactory.EnsureComponent<Mask>(mViewport.gameObject).showMaskGraphic = false;

        mTrack = UGUIReplicaUIFactory.CreateRect("Track", mViewport);
        mTrack.anchorMin = new Vector2(0f, 0.5f);
        mTrack.anchorMax = new Vector2(0f, 0.5f);
        mTrack.pivot = new Vector2(0f, 0.5f);
        mTrack.sizeDelta = new Vector2((sTitles.Length * (ItemWidth + Gap)) + 40f, ItemHeight);
        mTrack.anchoredPosition = Vector2.zero;

        mItems.Clear();
        for (var i = 0; i < sTitles.Length; i++)
        {
            mItems.Add(CreateItem(i));
        }

        var indicatorRoot = UGUIReplicaUIFactory.CreateRect("Indicators", frame);
        indicatorRoot.anchorMin = new Vector2(0.5f, 0f);
        indicatorRoot.anchorMax = new Vector2(0.5f, 0f);
        indicatorRoot.pivot = new Vector2(0.5f, 0f);
        indicatorRoot.sizeDelta = new Vector2(320f, 36f);
        indicatorRoot.anchoredPosition = new Vector2(0f, 36f);

        var row = UGUIReplicaUIFactory.EnsureComponent<HorizontalLayoutGroup>(indicatorRoot.gameObject);
        row.childAlignment = TextAnchor.MiddleCenter;
        row.childControlHeight = false;
        row.childControlWidth = false;
        row.childForceExpandWidth = false;
        row.childForceExpandHeight = false;
        row.spacing = 20f;

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
        var itemRect = UGUIReplicaUIFactory.CreatePanel($"Item_{index}", mTrack, sCardColors[index]);
        itemRect.anchorMin = new Vector2(0f, 0.5f);
        itemRect.anchorMax = new Vector2(0f, 0.5f);
        itemRect.pivot = new Vector2(0.5f, 0.5f);
        itemRect.sizeDelta = new Vector2(ItemWidth, ItemHeight);
        itemRect.anchoredPosition = new Vector2((ItemWidth * 0.5f) + (index * (ItemWidth + Gap)), 0f);

        var outline = UGUIReplicaUIFactory.EnsureComponent<Outline>(itemRect.gameObject);
        outline.effectColor = new Color(1f, 1f, 1f, 0.18f);
        outline.effectDistance = new Vector2(1.4f, -1.4f);

        var iconCircle = UGUIReplicaUIFactory.CreatePanel("Icon", itemRect, Color.white);
        iconCircle.anchorMin = new Vector2(0f, 1f);
        iconCircle.anchorMax = new Vector2(0f, 1f);
        iconCircle.pivot = new Vector2(0f, 1f);
        iconCircle.sizeDelta = new Vector2(52f, 52f);
        iconCircle.anchoredPosition = new Vector2(24f, -24f);

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
            36,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            Color.white);
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0f, 0f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.offsetMin = new Vector2(24f, 72f);
        titleRect.offsetMax = new Vector2(-24f, -200f);

        var desc = UGUIReplicaUIFactory.CreateText(
            "Description",
            itemRect,
            sDescriptions[index],
            24,
            FontStyle.Normal,
            TextAnchor.LowerLeft,
            new Color(1f, 1f, 1f, 0.92f));
        var descRect = (RectTransform)desc.transform;
        descRect.anchorMin = new Vector2(0f, 0f);
        descRect.anchorMax = new Vector2(1f, 0f);
        descRect.pivot = new Vector2(0.5f, 0f);
        descRect.sizeDelta = new Vector2(-48f, 96f);
        descRect.anchoredPosition = new Vector2(0f, 24f);

        return new CarouselItemData
        {
            Rect = itemRect,
            Rect3D = itemRect
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
        AnimateTrackToIndex(immediate);
        UpdateIndicators();
        mAutoplayTimer = 0f;
    }

    private void AnimateTrackToIndex(bool immediate)
    {
        var targetX = -(mCurrentIndex * (ItemWidth + Gap));
        mTrack.DOKill();
        if (immediate)
        {
            mTrack.anchoredPosition = new Vector2(targetX, 0f);
            UpdateCardPerspective();
            return;
        }

        mTrack
            .DOAnchorPosX(targetX, 0.55f)
            .SetEase(Ease.OutCubic)
            .OnUpdate(UpdateCardPerspective);
    }

    private void UpdateCardPerspective()
    {
        var currentFloat = -mTrack.anchoredPosition.x / (ItemWidth + Gap);
        for (var i = 0; i < mItems.Count; i++)
        {
            var card = mItems[i];
            var offset = i - currentFloat;
            var rotY = Mathf.Clamp(-offset * 40f, -90f, 90f);
            var zShift = Mathf.Abs(offset) * -36f;
            card.Rect3D.localRotation = Quaternion.Euler(0f, rotY, 0f);
            card.Rect.anchoredPosition = new Vector2((ItemWidth * 0.5f) + (i * (ItemWidth + Gap)), zShift * 0.1f);
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
        public RectTransform Rect3D;
    }
}
