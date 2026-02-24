using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class DockMagnifyToolbarEffectView
{
    public RectTransform Root;
    public RectTransform DockPanel;
    public readonly List<DockMagnifyToolbarItemView> Items = new List<DockMagnifyToolbarItemView>();
}

public sealed class DockMagnifyToolbarItemView
{
    public RectTransform Root;
    public RectTransform LabelRoot;
    public Image Background;
    public Text Icon;
    public CanvasGroup LabelGroup;
    public Vector2 BasePos;
    public float CurrentSize;
    public Color BaseColor;
}

public static class DockMagnifyToolbarEffectViewBuilder
{
    public static DockMagnifyToolbarEffectView Build(
        RectTransform mountRoot,
        DockMagnifyToolbarEffectConfig config,
        DockMagnifyToolbarEffectController controller,
        DockMagnifyToolbarEffectModel model)
    {
        var view = new DockMagnifyToolbarEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("DockMagnifyToolbarEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var backdrop = ReplicaUIFactoryV2.CreatePanel("Backdrop", view.Root, config.BackdropColor);
        ReplicaUIFactoryV2.Stretch(backdrop);

        var plateA = ReplicaUIFactoryV2.CreatePanel("PlateA", backdrop, config.PlateAColor);
        plateA.anchorMin = new Vector2(0.5f, 0.5f);
        plateA.anchorMax = new Vector2(0.5f, 0.5f);
        plateA.pivot = new Vector2(0.5f, 0.5f);
        plateA.sizeDelta = new Vector2(1180f, 420f);
        plateA.anchoredPosition = new Vector2(-140f, 74f);
        plateA.localRotation = Quaternion.Euler(0f, 0f, -7f);

        var plateB = ReplicaUIFactoryV2.CreatePanel("PlateB", backdrop, config.PlateBColor);
        plateB.anchorMin = new Vector2(0.5f, 0.5f);
        plateB.anchorMax = new Vector2(0.5f, 0.5f);
        plateB.pivot = new Vector2(0.5f, 0.5f);
        plateB.sizeDelta = new Vector2(1040f, 360f);
        plateB.anchoredPosition = new Vector2(190f, -56f);
        plateB.localRotation = Quaternion.Euler(0f, 0f, 6f);

        var hint = ReplicaUIFactoryV2.CreateText(
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

        view.DockPanel = ReplicaUIFactoryV2.CreatePanel("DockPanel", backdrop, config.PanelColor);
        view.DockPanel.anchorMin = new Vector2(0.5f, 0f);
        view.DockPanel.anchorMax = new Vector2(0.5f, 0f);
        view.DockPanel.pivot = new Vector2(0.5f, 0f);
        view.DockPanel.sizeDelta = config.PanelSize;
        view.DockPanel.anchoredPosition = new Vector2(0f, config.PanelBottomOffset);

        var panelOutline = ReplicaUIFactoryV2.EnsureComponent<Outline>(view.DockPanel.gameObject);
        panelOutline.effectColor = config.PanelBorderColor;
        panelOutline.effectDistance = new Vector2(1.3f, -1.3f);

        var panelShadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(view.DockPanel.gameObject);
        panelShadow.effectColor = config.PanelShadowColor;
        panelShadow.effectDistance = new Vector2(0f, -10f);

        var panelRelay = ReplicaUIFactoryV2.EnsureComponent<DockMagnifyToolbarPanelInputRelay>(view.DockPanel.gameObject);
        panelRelay.Initialize(controller);

        view.Items.Clear();
        var safeModel = model != null ? model : DockMagnifyToolbarEffectModel.CreateDefault();
        var count = safeModel.Items != null ? safeModel.Items.Count : 0;
        var totalWidth = (count * config.BaseItemSize) + ((count - 1) * config.ItemGap);
        var startX = -totalWidth * 0.5f + (config.BaseItemSize * 0.5f);

        for (var i = 0; i < count; i++)
        {
            var data = safeModel.Items[i] ?? new DockMagnifyToolbarItemData();
            var item = CreateItem(view.DockPanel, config, controller, i, data, startX + (i * (config.BaseItemSize + config.ItemGap)));
            view.Items.Add(item);
        }

        return view;
    }

    private static DockMagnifyToolbarItemView CreateItem(
        RectTransform dockPanel,
        DockMagnifyToolbarEffectConfig config,
        DockMagnifyToolbarEffectController controller,
        int index,
        DockMagnifyToolbarItemData data,
        float posX)
    {
        var itemRect = ReplicaUIFactoryV2.CreatePanel($"Item_{index}", dockPanel, data.BackgroundColor);
        itemRect.anchorMin = new Vector2(0.5f, 0f);
        itemRect.anchorMax = new Vector2(0.5f, 0f);
        itemRect.pivot = new Vector2(0.5f, 0f);
        itemRect.sizeDelta = new Vector2(config.BaseItemSize, config.BaseItemSize);
        itemRect.anchoredPosition = new Vector2(posX, config.ItemBaseBottomPadding);

        var outline = ReplicaUIFactoryV2.EnsureComponent<Outline>(itemRect.gameObject);
        outline.effectColor = config.ItemOutlineColor;
        outline.effectDistance = new Vector2(1f, -1f);

        var icon = ReplicaUIFactoryV2.CreateText(
            "Icon",
            itemRect,
            data.Glyph ?? "",
            Mathf.RoundToInt(config.FontSizeMin),
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        ReplicaUIFactoryV2.Stretch((RectTransform)icon.transform);

        var labelRoot = ReplicaUIFactoryV2.CreatePanel("LabelRoot", itemRect, config.LabelRootColor);
        labelRoot.anchorMin = new Vector2(0.5f, 0f);
        labelRoot.anchorMax = new Vector2(0.5f, 0f);
        labelRoot.pivot = new Vector2(0.5f, 0f);
        labelRoot.sizeDelta = config.LabelSize;
        labelRoot.anchoredPosition = new Vector2(0f, config.BaseItemSize + 10f);

        var labelOutline = ReplicaUIFactoryV2.EnsureComponent<Outline>(labelRoot.gameObject);
        labelOutline.effectColor = config.LabelBorderColor;
        labelOutline.effectDistance = new Vector2(1f, -1f);

        var label = ReplicaUIFactoryV2.CreateText(
            "Label",
            labelRoot,
            data.Label ?? "",
            18,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.LabelTextColor);
        ReplicaUIFactoryV2.Stretch((RectTransform)label.transform);

        var labelGroup = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(labelRoot.gameObject);
        labelGroup.alpha = 0f;
        labelGroup.blocksRaycasts = false;
        labelGroup.interactable = false;

        var relay = ReplicaUIFactoryV2.EnsureComponent<DockMagnifyToolbarItemInputRelay>(itemRect.gameObject);
        relay.Initialize(controller, index);

        return new DockMagnifyToolbarItemView
        {
            Root = itemRect,
            LabelRoot = labelRoot,
            Background = itemRect.GetComponent<Image>(),
            Icon = icon,
            LabelGroup = labelGroup,
            BasePos = itemRect.anchoredPosition,
            CurrentSize = config.BaseItemSize,
            BaseColor = data.BackgroundColor
        };
    }
}
