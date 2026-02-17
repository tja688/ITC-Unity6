using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUIGlassIconsReplica : MonoBehaviour
{
    private RectTransform mRootRect;
    private readonly List<GlassIconData> mItems = new();

    private static readonly string[] sLabels = { "Home", "Email", "Play", "Photo", "Chart", "Cloud" };
    private static readonly string[] sGlyphs = { "H", "@", ">", "P", "#", "C" };
    private static readonly Color[] sBackColors =
    {
        new(0.22f, 0.46f, 0.90f, 1f),
        new(0.64f, 0.34f, 0.92f, 1f),
        new(0.88f, 0.36f, 0.30f, 1f),
        new(0.34f, 0.44f, 0.88f, 1f),
        new(0.84f, 0.58f, 0.24f, 1f),
        new(0.28f, 0.66f, 0.42f, 1f)
    };

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("UGUIGlassIconsReplica requires RectTransform.", this);
            return;
        }

        BuildView();
        ResetAllVisuals(true);
    }

    private void OnEnable()
    {
        ResetAllVisuals(true);
    }

    private void OnDisable()
    {
        for (var i = 0; i < mItems.Count; i++)
        {
            mItems[i].Root.DOKill();
            mItems[i].Back.DOKill();
            mItems[i].Front.DOKill();
            mItems[i].LabelRoot.DOKill();
            mItems[i].LabelGroup.DOKill();
        }
    }

    private void BuildView()
    {
        mRootRect.anchorMin = Vector2.zero;
        mRootRect.anchorMax = Vector2.one;
        mRootRect.offsetMin = Vector2.zero;
        mRootRect.offsetMax = Vector2.zero;

        var backdrop = UGUIReplicaUIFactory.CreatePanel("Backdrop", mRootRect, new Color(0.05f, 0.07f, 0.13f, 0.90f));
        Stretch(backdrop);

        var plateA = UGUIReplicaUIFactory.CreatePanel("PlateA", backdrop, new Color(0.24f, 0.42f, 0.88f, 0.22f));
        plateA.anchorMin = new Vector2(0.5f, 0.5f);
        plateA.anchorMax = new Vector2(0.5f, 0.5f);
        plateA.pivot = new Vector2(0.5f, 0.5f);
        plateA.sizeDelta = new Vector2(1180f, 420f);
        plateA.anchoredPosition = new Vector2(-160f, 72f);
        plateA.localRotation = Quaternion.Euler(0f, 0f, -6f);

        var plateB = UGUIReplicaUIFactory.CreatePanel("PlateB", backdrop, new Color(0.58f, 0.31f, 0.82f, 0.20f));
        plateB.anchorMin = new Vector2(0.5f, 0.5f);
        plateB.anchorMax = new Vector2(0.5f, 0.5f);
        plateB.pivot = new Vector2(0.5f, 0.5f);
        plateB.sizeDelta = new Vector2(1080f, 360f);
        plateB.anchoredPosition = new Vector2(180f, -62f);
        plateB.localRotation = Quaternion.Euler(0f, 0f, 7f);

        var hint = UGUIReplicaUIFactory.CreateText(
            "Hint",
            backdrop,
            "GlassIcons  |  Hover each icon tile",
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

        var stage = UGUIReplicaUIFactory.CreateRect("Stage", backdrop);
        stage.anchorMin = new Vector2(0.5f, 0.5f);
        stage.anchorMax = new Vector2(0.5f, 0.5f);
        stage.pivot = new Vector2(0.5f, 0.5f);
        stage.sizeDelta = new Vector2(980f, 620f);
        stage.anchoredPosition = new Vector2(0f, -10f);

        var grid = UGUIReplicaUIFactory.CreateRect("Grid", stage);
        Stretch(grid);

        var gridLayout = UGUIReplicaUIFactory.EnsureComponent<GridLayoutGroup>(grid.gameObject);
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 3;
        gridLayout.cellSize = new Vector2(210f, 230f);
        gridLayout.spacing = new Vector2(26f, 22f);
        gridLayout.childAlignment = TextAnchor.MiddleCenter;

        mItems.Clear();
        for (var i = 0; i < sLabels.Length; i++)
        {
            mItems.Add(CreateIconCell(i, grid));
        }
    }

    private GlassIconData CreateIconCell(int index, RectTransform parent)
    {
        var root = UGUIReplicaUIFactory.CreateRect($"Icon_{index}", parent);
        root.sizeDelta = new Vector2(180f, 220f);

        var button = UGUIReplicaUIFactory.EnsureComponent<Button>(root.gameObject);
        button.transition = Selectable.Transition.None;
        var clickArea = UGUIReplicaUIFactory.EnsureComponent<Image>(root.gameObject);
        clickArea.color = new Color(1f, 1f, 1f, 0.001f);
        clickArea.raycastTarget = true;

        var iconBase = UGUIReplicaUIFactory.CreateRect("IconBase", root);
        iconBase.anchorMin = new Vector2(0.5f, 1f);
        iconBase.anchorMax = new Vector2(0.5f, 1f);
        iconBase.pivot = new Vector2(0.5f, 1f);
        iconBase.sizeDelta = new Vector2(128f, 128f);
        iconBase.anchoredPosition = new Vector2(0f, -18f);

        var back = UGUIReplicaUIFactory.CreatePanel("Back", iconBase, sBackColors[index]);
        Stretch(back);
        back.localRotation = Quaternion.Euler(0f, 0f, 15f);
        var backShadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(back.gameObject);
        backShadow.effectColor = new Color(0f, 0f, 0f, 0.28f);
        backShadow.effectDistance = new Vector2(9f, -9f);

        var front = UGUIReplicaUIFactory.CreatePanel("Front", iconBase, new Color(1f, 1f, 1f, 0.14f));
        Stretch(front);
        var frontOutline = UGUIReplicaUIFactory.EnsureComponent<Outline>(front.gameObject);
        frontOutline.effectColor = new Color(1f, 1f, 1f, 0.40f);
        frontOutline.effectDistance = new Vector2(1f, -1f);

        var glyph = UGUIReplicaUIFactory.CreateText(
            "Glyph",
            front,
            sGlyphs[index],
            56,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.94f, 0.97f, 1f, 0.96f));
        Stretch((RectTransform)glyph.transform);

        var labelRoot = UGUIReplicaUIFactory.CreateRect("LabelRoot", root);
        labelRoot.anchorMin = new Vector2(0.5f, 1f);
        labelRoot.anchorMax = new Vector2(0.5f, 1f);
        labelRoot.pivot = new Vector2(0.5f, 1f);
        labelRoot.sizeDelta = new Vector2(170f, 32f);
        labelRoot.anchoredPosition = new Vector2(0f, -168f);

        var label = UGUIReplicaUIFactory.CreateText(
            "Label",
            labelRoot,
            sLabels[index],
            24,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.88f, 0.92f, 1f, 0.98f));
        Stretch((RectTransform)label.transform);

        var labelGroup = UGUIReplicaUIFactory.EnsureComponent<CanvasGroup>(labelRoot.gameObject);
        labelGroup.alpha = 0f;
        labelGroup.interactable = false;
        labelGroup.blocksRaycasts = false;

        var relay = root.gameObject.AddComponent<GlassIconRelay>();
        relay.Initialize(this, index);

        return new GlassIconData
        {
            Root = root,
            Back = back,
            Front = front,
            LabelRoot = labelRoot,
            LabelGroup = labelGroup
        };
    }

    internal void SetHoverState(int index, bool hovering)
    {
        if (index < 0 || index >= mItems.Count)
        {
            return;
        }

        var item = mItems[index];

        item.Root.DOKill();
        item.Back.DOKill();
        item.Front.DOKill();
        item.LabelRoot.DOKill();
        item.LabelGroup.DOKill();

        if (hovering)
        {
            item.Root.DOScale(1.04f, 0.18f).SetEase(Ease.OutCubic);
            item.Back.DOAnchorPos(new Vector2(-12f, 10f), 0.24f).SetEase(Ease.OutCubic);
            item.Back.DORotate(new Vector3(0f, 0f, 25f), 0.24f).SetEase(Ease.OutCubic);
            item.Front.DOAnchorPos(new Vector2(0f, -8f), 0.24f).SetEase(Ease.OutCubic);
            item.Front.DOScale(1.08f, 0.24f).SetEase(Ease.OutCubic);
            item.LabelRoot.DOAnchorPosY(-150f, 0.22f).SetEase(Ease.OutCubic);
            item.LabelGroup.DOFade(1f, 0.20f).SetEase(Ease.OutCubic);
        }
        else
        {
            item.Root.DOScale(1f, 0.16f).SetEase(Ease.OutCubic);
            item.Back.DOAnchorPos(Vector2.zero, 0.20f).SetEase(Ease.OutCubic);
            item.Back.DORotate(new Vector3(0f, 0f, 15f), 0.20f).SetEase(Ease.OutCubic);
            item.Front.DOAnchorPos(Vector2.zero, 0.20f).SetEase(Ease.OutCubic);
            item.Front.DOScale(1f, 0.20f).SetEase(Ease.OutCubic);
            item.LabelRoot.DOAnchorPosY(-168f, 0.18f).SetEase(Ease.OutCubic);
            item.LabelGroup.DOFade(0f, 0.16f).SetEase(Ease.OutCubic);
        }
    }

    internal void SetPressedState(int index, bool pressed)
    {
        if (index < 0 || index >= mItems.Count)
        {
            return;
        }

        var item = mItems[index];
        item.Root.DOKill();
        item.Root.DOScale(pressed ? 0.94f : 1.04f, pressed ? 0.12f : 0.16f).SetEase(Ease.OutCubic);
    }

    private void ResetAllVisuals(bool immediate)
    {
        for (var i = 0; i < mItems.Count; i++)
        {
            var item = mItems[i];

            item.Root.DOKill();
            item.Back.DOKill();
            item.Front.DOKill();
            item.LabelRoot.DOKill();
            item.LabelGroup.DOKill();

            if (immediate)
            {
                item.Root.localScale = Vector3.one;
                item.Back.anchoredPosition = Vector2.zero;
                item.Back.localRotation = Quaternion.Euler(0f, 0f, 15f);
                item.Front.anchoredPosition = Vector2.zero;
                item.Front.localScale = Vector3.one;
                item.LabelRoot.anchoredPosition = new Vector2(0f, -168f);
                item.LabelGroup.alpha = 0f;
            }
            else
            {
                SetHoverState(i, false);
            }
        }
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private struct GlassIconData
    {
        public RectTransform Root;
        public RectTransform Back;
        public RectTransform Front;
        public RectTransform LabelRoot;
        public CanvasGroup LabelGroup;
    }

    private sealed class GlassIconRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private UGUIGlassIconsReplica mOwner;
        private int mIndex;

        public void Initialize(UGUIGlassIconsReplica owner, int index)
        {
            mOwner = owner;
            mIndex = index;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            mOwner?.SetHoverState(mIndex, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            mOwner?.SetHoverState(mIndex, false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            mOwner?.SetPressedState(mIndex, true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            mOwner?.SetPressedState(mIndex, false);
        }
    }
}
