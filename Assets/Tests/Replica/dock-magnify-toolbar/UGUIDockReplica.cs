using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUIDockReplica : MonoBehaviour
{
    [SerializeField] private float mBaseItemSize = 74f;
    [SerializeField] private float mMagnifiedItemSize = 114f;
    [SerializeField] private float mInfluenceDistance = 260f;
    [SerializeField] private float mSpringSpeed = 12f;
    [SerializeField] private float mItemGap = 16f;

    private RectTransform mRootRect;
    private RectTransform mDockPanel;
    private readonly List<DockItemData> mItems = new();

    private bool mPanelHovered;
    private int mHoverIndex = -1;
    private float mCurrentPanelHeight;

    private static readonly string[] sLabels = { "Finder", "Music", "Mail", "Code", "Photos", "Prefs" };
    private static readonly string[] sGlyphs = { "F", "M", "@", "C", "P", "S" };
    private static readonly Color[] sIconBgColors =
    {
        new(0.24f, 0.42f, 0.73f, 1f),
        new(0.24f, 0.62f, 0.50f, 1f),
        new(0.72f, 0.40f, 0.30f, 1f),
        new(0.54f, 0.39f, 0.78f, 1f),
        new(0.73f, 0.56f, 0.30f, 1f),
        new(0.38f, 0.56f, 0.79f, 1f)
    };

    private static readonly Color sPanelColor = new(0.04f, 0.06f, 0.12f, 0.95f);
    private static readonly Color sPanelBorder = new(1f, 1f, 1f, 0.28f);

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("UGUIDockReplica requires RectTransform.", this);
            return;
        }

        BuildView();
    }

    private void OnEnable()
    {
        mPanelHovered = false;
        mHoverIndex = -1;
        mCurrentPanelHeight = 96f;

        for (var i = 0; i < mItems.Count; i++)
        {
            var item = mItems[i];
            item.CurrentSize = mBaseItemSize;
            item.Rect.sizeDelta = new Vector2(mBaseItemSize, mBaseItemSize);
            item.LabelGroup.alpha = 0f;
            item.Rect.anchoredPosition = item.BasePos;
            mItems[i] = item;
        }
    }

    private void Update()
    {
        if (mItems.Count == 0 || mDockPanel == null)
        {
            return;
        }

        UpdatePanelHeight();
        UpdateMagnification();
        UpdateLabels();
    }

    private void OnDisable()
    {
        for (var i = 0; i < mItems.Count; i++)
        {
            mItems[i].Rect.DOKill();
        }
    }

    private void BuildView()
    {
        mRootRect.anchorMin = Vector2.zero;
        mRootRect.anchorMax = Vector2.one;
        mRootRect.offsetMin = Vector2.zero;
        mRootRect.offsetMax = Vector2.zero;

        var backdrop = UGUIReplicaUIFactory.CreatePanel("Backdrop", mRootRect, new Color(0.05f, 0.07f, 0.13f, 0.88f));
        Stretch(backdrop);

        var plateA = UGUIReplicaUIFactory.CreatePanel("PlateA", backdrop, new Color(0.24f, 0.40f, 0.74f, 0.24f));
        plateA.anchorMin = new Vector2(0.5f, 0.5f);
        plateA.anchorMax = new Vector2(0.5f, 0.5f);
        plateA.pivot = new Vector2(0.5f, 0.5f);
        plateA.sizeDelta = new Vector2(1180f, 420f);
        plateA.anchoredPosition = new Vector2(-140f, 74f);
        plateA.localRotation = Quaternion.Euler(0f, 0f, -7f);

        var plateB = UGUIReplicaUIFactory.CreatePanel("PlateB", backdrop, new Color(0.58f, 0.29f, 0.72f, 0.20f));
        plateB.anchorMin = new Vector2(0.5f, 0.5f);
        plateB.anchorMax = new Vector2(0.5f, 0.5f);
        plateB.pivot = new Vector2(0.5f, 0.5f);
        plateB.sizeDelta = new Vector2(1040f, 360f);
        plateB.anchoredPosition = new Vector2(190f, -56f);
        plateB.localRotation = Quaternion.Euler(0f, 0f, 6f);

        var hint = UGUIReplicaUIFactory.CreateText(
            "Hint",
            backdrop,
            "Dock  |  Move cursor across toolbar",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.93f, 0.96f, 1f, 0.95f));
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(900f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -42f);

        mDockPanel = UGUIReplicaUIFactory.CreatePanel("DockPanel", backdrop, sPanelColor);
        mDockPanel.anchorMin = new Vector2(0.5f, 0f);
        mDockPanel.anchorMax = new Vector2(0.5f, 0f);
        mDockPanel.pivot = new Vector2(0.5f, 0f);
        mDockPanel.sizeDelta = new Vector2(760f, 96f);
        mDockPanel.anchoredPosition = new Vector2(0f, 82f);

        var panelOutline = UGUIReplicaUIFactory.EnsureComponent<Outline>(mDockPanel.gameObject);
        panelOutline.effectColor = sPanelBorder;
        panelOutline.effectDistance = new Vector2(1.3f, -1.3f);

        var panelShadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(mDockPanel.gameObject);
        panelShadow.effectColor = new Color(0f, 0f, 0f, 0.36f);
        panelShadow.effectDistance = new Vector2(0f, -10f);

        var panelHoverRelay = mDockPanel.gameObject.AddComponent<DockPanelHoverRelay>();
        panelHoverRelay.Initialize(this);

        mItems.Clear();
        var totalWidth = (sLabels.Length * mBaseItemSize) + ((sLabels.Length - 1) * mItemGap);
        var startX = -totalWidth * 0.5f + (mBaseItemSize * 0.5f);

        for (var i = 0; i < sLabels.Length; i++)
        {
            mItems.Add(CreateItem(i, startX + (i * (mBaseItemSize + mItemGap))));
        }
    }

    private DockItemData CreateItem(int index, float posX)
    {
        var itemRect = UGUIReplicaUIFactory.CreatePanel($"Item_{index}", mDockPanel, sIconBgColors[index]);
        itemRect.anchorMin = new Vector2(0.5f, 0f);
        itemRect.anchorMax = new Vector2(0.5f, 0f);
        itemRect.pivot = new Vector2(0.5f, 0f);
        itemRect.sizeDelta = new Vector2(mBaseItemSize, mBaseItemSize);
        itemRect.anchoredPosition = new Vector2(posX, 12f);

        var outline = UGUIReplicaUIFactory.EnsureComponent<Outline>(itemRect.gameObject);
        outline.effectColor = new Color(1f, 1f, 1f, 0.24f);
        outline.effectDistance = new Vector2(1f, -1f);

        var icon = UGUIReplicaUIFactory.CreateText(
            "Icon",
            itemRect,
            sGlyphs[index],
            34,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        Stretch((RectTransform)icon.transform);

        var labelRoot = UGUIReplicaUIFactory.CreatePanel("LabelRoot", itemRect, new Color(0.03f, 0.05f, 0.10f, 0.95f));
        labelRoot.anchorMin = new Vector2(0.5f, 0f);
        labelRoot.anchorMax = new Vector2(0.5f, 0f);
        labelRoot.pivot = new Vector2(0.5f, 0f);
        labelRoot.sizeDelta = new Vector2(128f, 34f);
        labelRoot.anchoredPosition = new Vector2(0f, mBaseItemSize + 10f);

        var labelOutline = UGUIReplicaUIFactory.EnsureComponent<Outline>(labelRoot.gameObject);
        labelOutline.effectColor = new Color(1f, 1f, 1f, 0.18f);
        labelOutline.effectDistance = new Vector2(1f, -1f);

        var label = UGUIReplicaUIFactory.CreateText(
            "Label",
            labelRoot,
            sLabels[index],
            18,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.90f, 0.95f, 1f, 1f));
        Stretch((RectTransform)label.transform);

        var labelGroup = UGUIReplicaUIFactory.EnsureComponent<CanvasGroup>(labelRoot.gameObject);
        labelGroup.alpha = 0f;
        labelGroup.blocksRaycasts = false;
        labelGroup.interactable = false;

        var relay = itemRect.gameObject.AddComponent<DockItemRelay>();
        relay.Initialize(this, index);

        return new DockItemData
        {
            Rect = itemRect,
            BasePos = itemRect.anchoredPosition,
            Icon = icon,
            Background = itemRect.GetComponent<Image>(),
            LabelRoot = labelRoot,
            LabelGroup = labelGroup,
            CurrentSize = mBaseItemSize
        };
    }

    private void UpdatePanelHeight()
    {
        var targetHeight = mPanelHovered ? 136f : 96f;
        mCurrentPanelHeight = Mathf.Lerp(mCurrentPanelHeight, targetHeight, Time.unscaledDeltaTime * 8f);
        mDockPanel.sizeDelta = new Vector2(mDockPanel.sizeDelta.x, mCurrentPanelHeight);
    }

    private void UpdateMagnification()
    {
        var localMouse = new Vector2(float.PositiveInfinity, 0f);
        if (mPanelHovered)
        {
            _ = RectTransformUtility.ScreenPointToLocalPointInRectangle(mDockPanel, Input.mousePosition, null, out localMouse);
        }

        for (var i = 0; i < mItems.Count; i++)
        {
            var item = mItems[i];
            var distance = Mathf.Abs(localMouse.x - item.BasePos.x);
            var influence = mPanelHovered
                ? Mathf.Clamp01(1f - (distance / Mathf.Max(1f, mInfluenceDistance)))
                : 0f;

            var eased = influence * influence * (3f - (2f * influence));
            var targetSize = Mathf.Lerp(mBaseItemSize, mMagnifiedItemSize, eased);

            item.CurrentSize = Mathf.Lerp(item.CurrentSize, targetSize, Time.unscaledDeltaTime * mSpringSpeed);
            item.Rect.sizeDelta = new Vector2(item.CurrentSize, item.CurrentSize);
            item.Rect.anchoredPosition = new Vector2(item.BasePos.x, item.BasePos.y + ((item.CurrentSize - mBaseItemSize) * 0.22f));
            item.Icon.fontSize = Mathf.RoundToInt(Mathf.Lerp(32f, 44f, eased));

            var hoverBlend = Mathf.Max(eased, (mHoverIndex == i) ? 1f : 0f);
            item.Background.color = Color.Lerp(sIconBgColors[i] * 0.85f, sIconBgColors[i], hoverBlend);

            var labelTargetY = item.CurrentSize + Mathf.Lerp(8f, 20f, item.LabelGroup.alpha);
            item.LabelRoot.anchoredPosition = new Vector2(0f, labelTargetY);

            mItems[i] = item;
        }
    }

    private void UpdateLabels()
    {
        for (var i = 0; i < mItems.Count; i++)
        {
            var item = mItems[i];
            var target = (mPanelHovered && mHoverIndex == i) ? 1f : 0f;
            item.LabelGroup.alpha = Mathf.MoveTowards(item.LabelGroup.alpha, target, Time.unscaledDeltaTime * 8f);
            mItems[i] = item;
        }
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    internal void SetPanelHovered(bool value)
    {
        mPanelHovered = value;
        if (!value)
        {
            mHoverIndex = -1;
        }
    }

    internal void SetItemHovered(int index, bool hovered)
    {
        if (!hovered && mHoverIndex == index)
        {
            mHoverIndex = -1;
            return;
        }

        if (hovered)
        {
            mHoverIndex = index;
        }
    }

    internal void OnItemClicked(int index)
    {
        if (index < 0 || index >= mItems.Count)
        {
            return;
        }

        mHoverIndex = index;
        mItems[index].Rect.DOPunchScale(Vector3.one * 0.12f, 0.22f, 1, 0.4f);
    }

    private struct DockItemData
    {
        public RectTransform Rect;
        public RectTransform LabelRoot;
        public Image Background;
        public Text Icon;
        public CanvasGroup LabelGroup;
        public Vector2 BasePos;
        public float CurrentSize;
    }

    private sealed class DockPanelHoverRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private UGUIDockReplica mOwner;

        public void Initialize(UGUIDockReplica owner)
        {
            mOwner = owner;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mOwner?.SetPanelHovered(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mOwner?.SetPanelHovered(false);
        }
    }

    private sealed class DockItemRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        private UGUIDockReplica mOwner;
        private int mIndex;

        public void Initialize(UGUIDockReplica owner, int index)
        {
            mOwner = owner;
            mIndex = index;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mOwner?.SetItemHovered(mIndex, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mOwner?.SetItemHovered(mIndex, false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            mOwner?.OnItemClicked(mIndex);
        }
    }
}
