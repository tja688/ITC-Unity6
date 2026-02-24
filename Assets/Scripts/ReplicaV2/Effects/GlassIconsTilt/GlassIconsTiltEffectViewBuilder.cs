using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class GlassIconsTiltEffectIconView
{
    public RectTransform Root;
    public RectTransform Back;
    public RectTransform Front;
    public RectTransform LabelRoot;
    public CanvasGroup LabelGroup;
    public Text GlyphText;
    public Text LabelText;
}

public sealed class GlassIconsTiltEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;
    public RectTransform Grid;
    public Text HintText;
    public List<GlassIconsTiltEffectIconView> Icons = new List<GlassIconsTiltEffectIconView>();
}

public static class GlassIconsTiltEffectViewBuilder
{
    public static GlassIconsTiltEffectView Build(RectTransform mountRoot, GlassIconsTiltEffectConfig config)
    {
        var view = new GlassIconsTiltEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("GlassIconsTiltEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        var backdrop = ReplicaUIFactoryV2.CreatePanel("Backdrop", view.Root, config.BackdropColor);
        ReplicaUIFactoryV2.Stretch(backdrop);
        backdrop.GetComponent<Image>().raycastTarget = false;

        var plateA = ReplicaUIFactoryV2.CreatePanel("PlateA", backdrop, config.PlateAColor);
        plateA.anchorMin = new Vector2(0.5f, 0.5f);
        plateA.anchorMax = new Vector2(0.5f, 0.5f);
        plateA.pivot = new Vector2(0.5f, 0.5f);
        plateA.sizeDelta = new Vector2(1180f, 420f);
        plateA.anchoredPosition = new Vector2(-160f, 72f);
        plateA.localRotation = Quaternion.Euler(0f, 0f, -6f);
        plateA.GetComponent<Image>().raycastTarget = false;

        var plateB = ReplicaUIFactoryV2.CreatePanel("PlateB", backdrop, config.PlateBColor);
        plateB.anchorMin = new Vector2(0.5f, 0.5f);
        plateB.anchorMax = new Vector2(0.5f, 0.5f);
        plateB.pivot = new Vector2(0.5f, 0.5f);
        plateB.sizeDelta = new Vector2(1080f, 360f);
        plateB.anchoredPosition = new Vector2(180f, -62f);
        plateB.localRotation = Quaternion.Euler(0f, 0f, 7f);
        plateB.GetComponent<Image>().raycastTarget = false;

        view.HintText = ReplicaUIFactoryV2.CreateText(
            "Hint",
            backdrop,
            "GlassIcons  |  Hover each icon tile",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.HintColor);
        var hintRect = (RectTransform)view.HintText.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(900f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -42f);

        view.Content = ReplicaUIFactoryV2.CreateRect("Stage", backdrop);
        view.Content.anchorMin = new Vector2(0.5f, 0.5f);
        view.Content.anchorMax = new Vector2(0.5f, 0.5f);
        view.Content.pivot = new Vector2(0.5f, 0.5f);
        view.Content.sizeDelta = config.StageSize;
        view.Content.anchoredPosition = new Vector2(0f, -10f);

        var grid = ReplicaUIFactoryV2.CreateRect("Grid", view.Content);
        ReplicaUIFactoryV2.Stretch(grid);
        view.Grid = grid;

        var layout = ReplicaUIFactoryV2.EnsureComponent<GridLayoutGroup>(grid.gameObject);
        layout.startAxis = GridLayoutGroup.Axis.Horizontal;
        layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = Mathf.Max(1, config.ColumnCount);
        layout.cellSize = config.CellSize;
        layout.spacing = config.CellSpacing;
        layout.childAlignment = TextAnchor.MiddleCenter;

        view.Icons.Clear();
        return view;
    }

    public static GlassIconsTiltEffectIconView CreateIconCell(int index, RectTransform parent, GlassIconsTiltEffectConfig config, GlassIconsTiltEffectItemModel item)
    {
        var icon = new GlassIconsTiltEffectIconView();

        icon.Root = ReplicaUIFactoryV2.CreateRect($"Icon_{index}", parent);
        icon.Root.sizeDelta = new Vector2(180f, 220f);

        var button = ReplicaUIFactoryV2.EnsureComponent<Button>(icon.Root.gameObject);
        button.transition = Selectable.Transition.None;
        var clickArea = ReplicaUIFactoryV2.EnsureComponent<Image>(icon.Root.gameObject);
        clickArea.color = new Color(1f, 1f, 1f, 0.001f);
        clickArea.raycastTarget = true;

        var iconBase = ReplicaUIFactoryV2.CreateRect("IconBase", icon.Root);
        iconBase.anchorMin = new Vector2(0.5f, 1f);
        iconBase.anchorMax = new Vector2(0.5f, 1f);
        iconBase.pivot = new Vector2(0.5f, 1f);
        iconBase.sizeDelta = new Vector2(128f, 128f);
        iconBase.anchoredPosition = new Vector2(0f, -18f);

        icon.Back = ReplicaUIFactoryV2.CreatePanel("Back", iconBase, item != null ? item.BackColor : Color.white);
        ReplicaUIFactoryV2.Stretch(icon.Back);
        icon.Back.localRotation = Quaternion.Euler(0f, 0f, config.BackIdleRotation);
        var backShadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(icon.Back.gameObject);
        backShadow.effectColor = new Color(0f, 0f, 0f, 0.28f);
        backShadow.effectDistance = new Vector2(9f, -9f);

        icon.Front = ReplicaUIFactoryV2.CreatePanel("Front", iconBase, config.FrontGlassColor);
        ReplicaUIFactoryV2.Stretch(icon.Front);
        var frontOutline = ReplicaUIFactoryV2.EnsureComponent<Outline>(icon.Front.gameObject);
        frontOutline.effectColor = new Color(1f, 1f, 1f, 0.40f);
        frontOutline.effectDistance = new Vector2(1f, -1f);

        icon.GlyphText = ReplicaUIFactoryV2.CreateText(
            "Glyph",
            icon.Front,
            item != null ? item.Glyph : "?",
            56,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.GlyphColor);
        ReplicaUIFactoryV2.Stretch((RectTransform)icon.GlyphText.transform);

        icon.LabelRoot = ReplicaUIFactoryV2.CreateRect("LabelRoot", icon.Root);
        icon.LabelRoot.anchorMin = new Vector2(0.5f, 1f);
        icon.LabelRoot.anchorMax = new Vector2(0.5f, 1f);
        icon.LabelRoot.pivot = new Vector2(0.5f, 1f);
        icon.LabelRoot.sizeDelta = new Vector2(170f, 32f);
        icon.LabelRoot.anchoredPosition = new Vector2(0f, config.LabelIdleY);

        icon.LabelText = ReplicaUIFactoryV2.CreateText(
            "Label",
            icon.LabelRoot,
            item != null ? item.Label : "Item",
            24,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.LabelColor);
        ReplicaUIFactoryV2.Stretch((RectTransform)icon.LabelText.transform);

        icon.LabelGroup = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(icon.LabelRoot.gameObject);
        icon.LabelGroup.alpha = 0f;
        icon.LabelGroup.interactable = false;
        icon.LabelGroup.blocksRaycasts = false;

        return icon;
    }
}
