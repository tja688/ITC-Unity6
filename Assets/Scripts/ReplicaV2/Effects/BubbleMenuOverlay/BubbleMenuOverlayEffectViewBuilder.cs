using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class BubbleMenuOverlayEffectView
{
    public RectTransform Root;
    public RectTransform OverlayRect;
    public CanvasGroup OverlayGroup;
    public Button ToggleButton;
    public RectTransform LineTop;
    public RectTransform LineBottom;
    public readonly List<BubbleMenuOverlayPillView> Pills = new List<BubbleMenuOverlayPillView>();
}

public sealed class BubbleMenuOverlayPillView
{
    public RectTransform Rect;
    public Image Image;
    public Text Label;
    public RectTransform LabelRect;
    public Color BaseBg = Color.white;
    public Color HoverBg = Color.white;
    public Color BaseText = Color.black;
    public Color HoverText = Color.white;
}

public static class BubbleMenuOverlayEffectViewBuilder
{
    public static BubbleMenuOverlayEffectView Build(
        RectTransform mountRoot,
        BubbleMenuOverlayEffectConfig config,
        BubbleMenuOverlayEffectController controller,
        BubbleMenuOverlayEffectModel model)
    {
        var view = new BubbleMenuOverlayEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("BubbleMenuOverlayEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var backdrop = ReplicaUIFactoryV2.CreatePanel("Backdrop", view.Root, config.BackdropColor);
        ReplicaUIFactoryV2.Stretch(backdrop);

        var topBar = ReplicaUIFactoryV2.CreateRect("TopBar", backdrop);
        topBar.anchorMin = new Vector2(0f, 1f);
        topBar.anchorMax = new Vector2(1f, 1f);
        topBar.pivot = new Vector2(0.5f, 1f);
        topBar.sizeDelta = new Vector2(0f, config.TopBarHeight);
        topBar.anchoredPosition = Vector2.zero;

        var logoBubble = ReplicaUIFactoryV2.CreateButton("LogoBubble", topBar, config.TopBarBubbleColor);
        var logoRect = (RectTransform)logoBubble.transform;
        logoRect.anchorMin = new Vector2(0f, 0.5f);
        logoRect.anchorMax = new Vector2(0f, 0.5f);
        logoRect.pivot = new Vector2(0f, 0.5f);
        logoRect.sizeDelta = config.LogoSize;
        logoRect.anchoredPosition = new Vector2(config.TogglePadding.x, -config.TogglePadding.y);
        logoBubble.interactable = false;

        var logoLabel = ReplicaUIFactoryV2.CreateText(
            "LogoLabel",
            logoBubble.transform,
            "react-bits",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.07f, 0.08f, 0.10f, 1f));
        ReplicaUIFactoryV2.Stretch((RectTransform)logoLabel.transform);

        view.ToggleButton = ReplicaUIFactoryV2.CreateButton("ToggleBubble", topBar, config.TopBarBubbleColor);
        var toggleRect = (RectTransform)view.ToggleButton.transform;
        toggleRect.anchorMin = new Vector2(1f, 0.5f);
        toggleRect.anchorMax = new Vector2(1f, 0.5f);
        toggleRect.pivot = new Vector2(1f, 0.5f);
        toggleRect.sizeDelta = config.ToggleSize;
        toggleRect.anchoredPosition = new Vector2(-config.TogglePadding.x, -config.TogglePadding.y);
        view.ToggleButton.onClick.AddListener(controller.ToggleMenu);

        view.LineTop = ReplicaUIFactoryV2.CreateRect("LineTop", toggleRect);
        view.LineTop.anchorMin = new Vector2(0.5f, 0.5f);
        view.LineTop.anchorMax = new Vector2(0.5f, 0.5f);
        view.LineTop.pivot = new Vector2(0.5f, 0.5f);
        view.LineTop.sizeDelta = new Vector2(28f, 3f);
        view.LineTop.anchoredPosition = new Vector2(0f, 5f);
        ReplicaUIFactoryV2.EnsureComponent<Image>(view.LineTop.gameObject).color = config.ToggleLineColor;

        view.LineBottom = ReplicaUIFactoryV2.CreateRect("LineBottom", toggleRect);
        view.LineBottom.anchorMin = new Vector2(0.5f, 0.5f);
        view.LineBottom.anchorMax = new Vector2(0.5f, 0.5f);
        view.LineBottom.pivot = new Vector2(0.5f, 0.5f);
        view.LineBottom.sizeDelta = new Vector2(28f, 3f);
        view.LineBottom.anchoredPosition = new Vector2(0f, -5f);
        ReplicaUIFactoryV2.EnsureComponent<Image>(view.LineBottom.gameObject).color = config.ToggleLineColor;

        var title = ReplicaUIFactoryV2.CreateText(
            "Hint",
            backdrop,
            "BubbleMenu  |  Click top-right bubble to toggle",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.93f, 0.96f, 1f, 0.95f));
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(920f, 58f);
        titleRect.anchoredPosition = new Vector2(0f, -136f);

        view.OverlayRect = ReplicaUIFactoryV2.CreatePanel("Overlay", backdrop, config.OverlayColor);
        ReplicaUIFactoryV2.Stretch(view.OverlayRect);
        var overlayImage = view.OverlayRect.GetComponent<Image>();
        overlayImage.raycastTarget = false;

        view.OverlayGroup = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.OverlayRect.gameObject);
        view.OverlayGroup.alpha = 0f;
        view.OverlayGroup.interactable = false;
        view.OverlayGroup.blocksRaycasts = false;

        var pillLayer = ReplicaUIFactoryV2.CreateRect("PillLayer", view.OverlayRect);
        ReplicaUIFactoryV2.Stretch(pillLayer);
        pillLayer.SetAsLastSibling();

        view.Pills.Clear();
        var safe = model != null ? model : BubbleMenuOverlayEffectModel.CreateDefault();
        var count = safe.Items != null ? safe.Items.Count : 0;

        for (var i = 0; i < count; i++)
        {
            var item = safe.Items[i] ?? new BubbleMenuOverlayItemData();
            view.Pills.Add(CreatePill(i, pillLayer, config, controller, item));
        }

        topBar.SetAsLastSibling();
        return view;
    }

    private static BubbleMenuOverlayPillView CreatePill(
        int index,
        RectTransform parent,
        BubbleMenuOverlayEffectConfig config,
        BubbleMenuOverlayEffectController controller,
        BubbleMenuOverlayItemData data)
    {
        var pillButton = ReplicaUIFactoryV2.CreateButton($"Pill_{index}", parent, Color.white);
        var pillRect = (RectTransform)pillButton.transform;
        pillRect.anchorMin = new Vector2(0.5f, 0.5f);
        pillRect.anchorMax = new Vector2(0.5f, 0.5f);
        pillRect.pivot = new Vector2(0.5f, 0.5f);
        pillRect.sizeDelta = config.PillSize;
        pillRect.anchoredPosition = data.Position;
        pillRect.localRotation = Quaternion.Euler(0f, 0f, data.RotationZ);
        pillRect.localScale = Vector3.zero;

        var label = ReplicaUIFactoryV2.CreateText(
            "Label",
            pillRect,
            data.Label ?? "",
            48,
            FontStyle.Normal,
            TextAnchor.MiddleCenter,
            config.PillBaseTextColor);
        var labelRect = (RectTransform)label.transform;
        ReplicaUIFactoryV2.Stretch(labelRect);
        labelRect.anchoredPosition = config.PillLabelClosedOffset;

        var relay = ReplicaUIFactoryV2.EnsureComponent<BubbleMenuOverlayPillInputRelay>(pillRect.gameObject);
        relay.Initialize(controller, index);

        return new BubbleMenuOverlayPillView
        {
            Rect = pillRect,
            Image = pillButton.image,
            Label = label,
            LabelRect = labelRect,
            BaseBg = Color.white,
            HoverBg = data.HoverBackgroundColor,
            BaseText = config.PillBaseTextColor,
            HoverText = config.PillHoverTextColor
        };
    }
}
