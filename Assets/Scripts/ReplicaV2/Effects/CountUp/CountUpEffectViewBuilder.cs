using UnityEngine;
using UnityEngine.UI;

public sealed class CountUpEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;
    public Text ValueLabel;
    public RectTransform ValueRect;
}

public static class CountUpEffectViewBuilder
{
    public static CountUpEffectView Build(RectTransform mountRoot, CountUpEffectConfig config)
    {
        var view = new CountUpEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("CountUpEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var rootImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        rootImage.color = config.RootBackground;
        rootImage.raycastTarget = true;

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        var ambient = ReplicaUIFactoryV2.CreatePanel("Ambient", view.Root, config.AmbientColor);
        ambient.anchorMin = new Vector2(0.5f, 0.5f);
        ambient.anchorMax = new Vector2(0.5f, 0.5f);
        ambient.pivot = new Vector2(0.5f, 0.5f);
        ambient.sizeDelta = config.AmbientSize;
        ambient.anchoredPosition = config.AmbientPosition;
        ambient.localRotation = Quaternion.Euler(0f, 0f, config.AmbientRotationZ);
        ambient.GetComponent<Image>().raycastTarget = false;

        var panel = ReplicaUIFactoryV2.CreatePanel("Panel", view.Root, config.PanelColor);
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = config.PanelSize;
        panel.anchoredPosition = config.PanelPosition;

        view.Content = panel;

        var shadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(panel.gameObject);
        shadow.effectDistance = new Vector2(0f, -10f);

        var title = ReplicaUIFactoryV2.CreateText(
            "Title",
            panel,
            "CountUp  |  spring number interpolation",
            config.TitleFontSize,
            FontStyle.Bold,
            TextAnchor.UpperCenter,
            config.TitleColor);
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(840f, 56f);
        titleRect.anchoredPosition = new Vector2(0f, -42f);

        var valueWrap = ReplicaUIFactoryV2.CreateRect("ValueWrap", panel);
        valueWrap.anchorMin = new Vector2(0.5f, 0.5f);
        valueWrap.anchorMax = new Vector2(0.5f, 0.5f);
        valueWrap.pivot = new Vector2(0.5f, 0.5f);
        valueWrap.sizeDelta = config.ValueWrapSize;
        valueWrap.anchoredPosition = config.ValueWrapPosition;

        var valueGlow = ReplicaUIFactoryV2.CreatePanel("ValueGlow", valueWrap, config.ValueGlowColor);
        ReplicaUIFactoryV2.Stretch(valueGlow);
        valueGlow.GetComponent<Image>().raycastTarget = false;

        view.ValueLabel = ReplicaUIFactoryV2.CreateText(
            "Value",
            valueWrap,
            "0",
            config.ValueFontSize,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        view.ValueRect = (RectTransform)view.ValueLabel.transform;
        ReplicaUIFactoryV2.Stretch(view.ValueRect);

        var hint = ReplicaUIFactoryV2.CreateText(
            "Hint",
            panel,
            "Auto retargeting every few seconds",
            config.HintFontSize,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.HintColor);
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 0f);
        hintRect.anchorMax = new Vector2(0.5f, 0f);
        hintRect.pivot = new Vector2(0.5f, 0f);
        hintRect.sizeDelta = new Vector2(640f, 44f);
        hintRect.anchoredPosition = new Vector2(0f, 46f);

        return view;
    }
}
