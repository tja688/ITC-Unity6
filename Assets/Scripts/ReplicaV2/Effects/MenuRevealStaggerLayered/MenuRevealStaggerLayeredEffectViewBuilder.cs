using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class MenuRevealStaggerLayeredEffectView
{
    public RectTransform Root;
    public readonly List<RectTransform> PreLayers = new List<RectTransform>();
    public readonly List<RectTransform> ItemLabels = new List<RectTransform>();
    public readonly List<Text> ItemNumbers = new List<Text>();
    public readonly List<RectTransform> ItemRows = new List<RectTransform>();
    public readonly List<RectTransform> SocialLinks = new List<RectTransform>();

    public RectTransform Panel;
    public RectTransform ToggleIcon;
    public RectTransform ToggleTextStack;
    public RectTransform ToggleLabelMask;
    public Text ToggleTextTop;
    public Text ToggleTextBottom;
    public CanvasGroup DismissGroup;
    public Button DismissButton;
    public RectTransform MenuListRoot;
    public RectTransform SocialRoot;
    public Text SocialTitle;
}

public static class MenuRevealStaggerLayeredEffectViewBuilder
{
    public static MenuRevealStaggerLayeredEffectView Build(
        RectTransform mountRoot,
        MenuRevealStaggerLayeredEffectConfig config,
        MenuRevealStaggerLayeredEffectController controller,
        MenuRevealStaggerLayeredEffectModel model)
    {
        var view = new MenuRevealStaggerLayeredEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("MenuRevealStaggerLayeredEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var rootImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        rootImage.color = config.RootBackground;
        rootImage.raycastTarget = true;

        CreateAmbientPlate("AmbientA", view.Root, config.AmbientAColor, new Vector2(-220f, 180f), 16f, 1100f, 460f);
        CreateAmbientPlate("AmbientB", view.Root, config.AmbientBColor, new Vector2(260f, -120f), -11f, 1280f, 520f);

        var dismissGo = new GameObject("Dismiss", typeof(RectTransform), typeof(Image), typeof(Button), typeof(CanvasGroup));
        var dismissRect = dismissGo.GetComponent<RectTransform>();
        dismissRect.SetParent(view.Root, false);
        ReplicaUIFactoryV2.Stretch(dismissRect);

        var dismissImage = dismissGo.GetComponent<Image>();
        dismissImage.color = new Color(0f, 0f, 0f, 0f);
        dismissImage.raycastTarget = true;

        view.DismissButton = dismissGo.GetComponent<Button>();
        view.DismissButton.transition = Selectable.Transition.None;
        view.DismissButton.onClick.AddListener(controller.CloseMenuIfOpen);

        view.DismissGroup = dismissGo.GetComponent<CanvasGroup>();
        view.DismissGroup.alpha = 1f;
        view.DismissGroup.blocksRaycasts = false;
        view.DismissGroup.interactable = false;

        var topBar = ReplicaUIFactoryV2.CreateRect("TopBar", view.Root);
        topBar.anchorMin = new Vector2(0f, 1f);
        topBar.anchorMax = new Vector2(1f, 1f);
        topBar.pivot = new Vector2(0.5f, 1f);
        topBar.sizeDelta = new Vector2(0f, 108f);
        topBar.anchoredPosition = Vector2.zero;

        var logo = ReplicaUIFactoryV2.CreateText(
            "Logo",
            topBar,
            "ReactBits / Staggered Menu",
            28,
            FontStyle.Bold,
            TextAnchor.MiddleLeft,
            new Color(0.92f, 0.95f, 1f, 1f));
        var logoRect = (RectTransform)logo.transform;
        logoRect.anchorMin = new Vector2(0f, 0.5f);
        logoRect.anchorMax = new Vector2(0f, 0.5f);
        logoRect.pivot = new Vector2(0f, 0.5f);
        logoRect.sizeDelta = new Vector2(620f, 44f);
        logoRect.anchoredPosition = new Vector2(36f, 0f);

        var toggle = ReplicaUIFactoryV2.CreateButton("Toggle", topBar, new Color(1f, 1f, 1f, 0f));
        var toggleRect = (RectTransform)toggle.transform;
        toggleRect.anchorMin = new Vector2(1f, 0.5f);
        toggleRect.anchorMax = new Vector2(1f, 0.5f);
        toggleRect.pivot = new Vector2(1f, 0.5f);
        toggleRect.sizeDelta = new Vector2(182f, 52f);
        toggleRect.anchoredPosition = new Vector2(-30f, 0f);
        toggle.onClick.AddListener(controller.ToggleMenu);

        view.ToggleLabelMask = ReplicaUIFactoryV2.CreateRect("ToggleLabelMask", toggle.transform);
        view.ToggleLabelMask.anchorMin = new Vector2(0f, 0.5f);
        view.ToggleLabelMask.anchorMax = new Vector2(0f, 0.5f);
        view.ToggleLabelMask.pivot = new Vector2(0f, 0.5f);
        view.ToggleLabelMask.sizeDelta = new Vector2(116f, 30f);
        view.ToggleLabelMask.anchoredPosition = Vector2.zero;
        ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(view.ToggleLabelMask.gameObject);

        view.ToggleTextStack = ReplicaUIFactoryV2.CreateRect("ToggleTextStack", view.ToggleLabelMask);
        view.ToggleTextStack.anchorMin = new Vector2(0f, 1f);
        view.ToggleTextStack.anchorMax = new Vector2(1f, 1f);
        view.ToggleTextStack.pivot = new Vector2(0.5f, 1f);
        view.ToggleTextStack.sizeDelta = new Vector2(0f, 60f);
        view.ToggleTextStack.anchoredPosition = Vector2.zero;

        view.ToggleTextTop = ReplicaUIFactoryV2.CreateText("MenuLine", view.ToggleTextStack, "Menu", 26, FontStyle.Bold, TextAnchor.MiddleLeft, config.ToggleClosedColor);
        var topTextRect = (RectTransform)view.ToggleTextTop.transform;
        topTextRect.anchorMin = new Vector2(0f, 1f);
        topTextRect.anchorMax = new Vector2(1f, 1f);
        topTextRect.pivot = new Vector2(0.5f, 1f);
        topTextRect.sizeDelta = new Vector2(0f, 30f);
        topTextRect.anchoredPosition = Vector2.zero;

        view.ToggleTextBottom = ReplicaUIFactoryV2.CreateText("CloseLine", view.ToggleTextStack, "Close", 26, FontStyle.Bold, TextAnchor.MiddleLeft, config.ToggleOpenColor);
        var bottomTextRect = (RectTransform)view.ToggleTextBottom.transform;
        bottomTextRect.anchorMin = new Vector2(0f, 1f);
        bottomTextRect.anchorMax = new Vector2(1f, 1f);
        bottomTextRect.pivot = new Vector2(0.5f, 1f);
        bottomTextRect.sizeDelta = new Vector2(0f, 30f);
        bottomTextRect.anchoredPosition = new Vector2(0f, -30f);

        view.ToggleIcon = ReplicaUIFactoryV2.CreateRect("Icon", toggle.transform);
        view.ToggleIcon.anchorMin = new Vector2(1f, 0.5f);
        view.ToggleIcon.anchorMax = new Vector2(1f, 0.5f);
        view.ToggleIcon.pivot = new Vector2(1f, 0.5f);
        view.ToggleIcon.sizeDelta = new Vector2(24f, 24f);
        view.ToggleIcon.anchoredPosition = new Vector2(-8f, 0f);

        CreateIconLine("IconHorizontal", view.ToggleIcon, Vector2.zero, 0f, config.ToggleClosedColor);
        CreateIconLine("IconVertical", view.ToggleIcon, Vector2.zero, 90f, config.ToggleClosedColor);

        var layerRoot = ReplicaUIFactoryV2.CreateRect("PreLayers", view.Root);
        layerRoot.anchorMin = new Vector2(1f, 0f);
        layerRoot.anchorMax = new Vector2(1f, 1f);
        layerRoot.pivot = new Vector2(1f, 0.5f);
        layerRoot.sizeDelta = new Vector2(config.PanelWidth, 0f);
        layerRoot.anchoredPosition = Vector2.zero;

        view.PreLayers.Clear();
        view.PreLayers.Add(ReplicaUIFactoryV2.CreatePanel("Layer_0", layerRoot, config.Layer0Color));
        view.PreLayers.Add(ReplicaUIFactoryV2.CreatePanel("Layer_1", layerRoot, config.Layer1Color));
        view.PreLayers.Add(ReplicaUIFactoryV2.CreatePanel("Layer_2", layerRoot, config.Layer2Color));
        for (var i = 0; i < view.PreLayers.Count; i++)
        {
            ReplicaUIFactoryV2.Stretch(view.PreLayers[i]);
            view.PreLayers[i].SetSiblingIndex(0);
        }

        view.Panel = ReplicaUIFactoryV2.CreatePanel("Panel", view.Root, config.PanelColor);
        view.Panel.anchorMin = new Vector2(1f, 0f);
        view.Panel.anchorMax = new Vector2(1f, 1f);
        view.Panel.pivot = new Vector2(1f, 0.5f);
        view.Panel.sizeDelta = new Vector2(config.PanelWidth, 0f);
        view.Panel.anchoredPosition = Vector2.zero;

        var panelShadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(view.Panel.gameObject);
        panelShadow.effectColor = config.PanelShadowColor;
        panelShadow.effectDistance = new Vector2(-8f, -2f);

        var panelInner = ReplicaUIFactoryV2.CreateRect("PanelInner", view.Panel);
        panelInner.anchorMin = Vector2.zero;
        panelInner.anchorMax = Vector2.one;
        panelInner.offsetMin = new Vector2(config.PanelPaddingLeft, config.PanelPaddingBottom);
        panelInner.offsetMax = new Vector2(-config.PanelPaddingRight, -config.PanelPaddingTop);

        view.MenuListRoot = ReplicaUIFactoryV2.CreateRect("MenuList", panelInner);
        view.MenuListRoot.anchorMin = new Vector2(0f, 1f);
        view.MenuListRoot.anchorMax = new Vector2(1f, 1f);
        view.MenuListRoot.pivot = new Vector2(0.5f, 1f);
        view.MenuListRoot.sizeDelta = new Vector2(0f, 420f);
        view.MenuListRoot.anchoredPosition = Vector2.zero;

        view.ItemLabels.Clear();
        view.ItemNumbers.Clear();
        view.ItemRows.Clear();

        var safe = model != null ? model : MenuRevealStaggerLayeredEffectModel.CreateDefault();
        var menuCount = safe.MenuItems != null ? safe.MenuItems.Count : 0;
        for (var i = 0; i < menuCount; i++)
        {
            var itemRow = ReplicaUIFactoryV2.CreateRect($"Item_{i}", view.MenuListRoot);
            itemRow.anchorMin = new Vector2(0f, 1f);
            itemRow.anchorMax = new Vector2(1f, 1f);
            itemRow.pivot = new Vector2(0f, 1f);
            itemRow.sizeDelta = new Vector2(0f, config.MenuRowHeight);
            itemRow.anchoredPosition = new Vector2(0f, -i * config.MenuRowStep);
            ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(itemRow.gameObject);
            view.ItemRows.Add(itemRow);

            var label = ReplicaUIFactoryV2.CreateText(
                "Label",
                itemRow,
                (safe.MenuItems[i] ?? "").ToUpperInvariant(),
                52,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                config.MenuLabelColor);
            var labelRect = (RectTransform)label.transform;
            labelRect.anchorMin = new Vector2(0f, 0.5f);
            labelRect.anchorMax = new Vector2(1f, 0.5f);
            labelRect.pivot = new Vector2(0f, 0.5f);
            labelRect.sizeDelta = new Vector2(0f, 70f);
            labelRect.anchoredPosition = Vector2.zero;

            var number = ReplicaUIFactoryV2.CreateText(
                "Number",
                itemRow,
                (i + 1).ToString("00"),
                20,
                FontStyle.Normal,
                TextAnchor.MiddleRight,
                config.AccentColor);
            var numberRect = (RectTransform)number.transform;
            numberRect.anchorMin = new Vector2(1f, 0.5f);
            numberRect.anchorMax = new Vector2(1f, 0.5f);
            numberRect.pivot = new Vector2(1f, 0.5f);
            numberRect.sizeDelta = new Vector2(74f, 32f);
            numberRect.anchoredPosition = new Vector2(0f, 8f);

            view.ItemLabels.Add(labelRect);
            view.ItemNumbers.Add(number);
        }

        view.SocialRoot = ReplicaUIFactoryV2.CreateRect("Socials", panelInner);
        view.SocialRoot.anchorMin = new Vector2(0f, 0f);
        view.SocialRoot.anchorMax = new Vector2(1f, 0f);
        view.SocialRoot.pivot = new Vector2(0.5f, 0f);
        view.SocialRoot.sizeDelta = new Vector2(0f, 160f);
        view.SocialRoot.anchoredPosition = Vector2.zero;

        view.SocialTitle = ReplicaUIFactoryV2.CreateText(
            "SocialTitle",
            view.SocialRoot,
            "Socials",
            24,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            config.AccentColor);
        var socialTitleRect = (RectTransform)view.SocialTitle.transform;
        socialTitleRect.anchorMin = new Vector2(0f, 1f);
        socialTitleRect.anchorMax = new Vector2(1f, 1f);
        socialTitleRect.pivot = new Vector2(0f, 1f);
        socialTitleRect.sizeDelta = new Vector2(0f, 38f);
        socialTitleRect.anchoredPosition = Vector2.zero;

        view.SocialLinks.Clear();
        var socialCount = safe.SocialItems != null ? safe.SocialItems.Count : 0;
        for (var i = 0; i < socialCount; i++)
        {
            var link = ReplicaUIFactoryV2.CreateText(
                $"Social_{i}",
                view.SocialRoot,
                safe.SocialItems[i] ?? "",
                26,
                FontStyle.Bold,
                TextAnchor.UpperLeft,
                config.SocialLinkColor);
            var linkRect = (RectTransform)link.transform;
            linkRect.anchorMin = new Vector2(0f, 1f);
            linkRect.anchorMax = new Vector2(1f, 1f);
            linkRect.pivot = new Vector2(0f, 1f);
            linkRect.sizeDelta = new Vector2(0f, 34f);
            linkRect.anchoredPosition = new Vector2(0f, -52f - (i * 34f));
            view.SocialLinks.Add(linkRect);
        }

        return view;
    }

    private static void CreateAmbientPlate(string name, RectTransform parent, Color color, Vector2 pos, float rotZ, float width, float height)
    {
        var plate = ReplicaUIFactoryV2.CreatePanel(name, parent, color);
        plate.anchorMin = new Vector2(0.5f, 0.5f);
        plate.anchorMax = new Vector2(0.5f, 0.5f);
        plate.pivot = new Vector2(0.5f, 0.5f);
        plate.sizeDelta = new Vector2(width, height);
        plate.anchoredPosition = pos;
        plate.localRotation = Quaternion.Euler(0f, 0f, rotZ);
        plate.SetSiblingIndex(0);
    }

    private static void CreateIconLine(string name, RectTransform parent, Vector2 pos, float rotZ, Color color)
    {
        var line = ReplicaUIFactoryV2.CreatePanel(name, parent, color);
        line.anchorMin = new Vector2(0.5f, 0.5f);
        line.anchorMax = new Vector2(0.5f, 0.5f);
        line.pivot = new Vector2(0.5f, 0.5f);
        line.sizeDelta = new Vector2(22f, 2.2f);
        line.anchoredPosition = pos;
        line.localRotation = Quaternion.Euler(0f, 0f, rotZ);
    }
}
