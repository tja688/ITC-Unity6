using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class CounterRollupEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;
    public RectTransform CounterRect;
    public readonly List<DigitColumnView> Columns = new List<DigitColumnView>();
}

public sealed class DigitColumnView
{
    public int Place;
    public RectTransform Root;
    public readonly List<RectTransform> Numbers = new List<RectTransform>();
}

public static class CounterRollupEffectViewBuilder
{
    public static CounterRollupEffectView Build(RectTransform mountRoot, CounterRollupEffectConfig config)
    {
        var view = new CounterRollupEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("CounterRollupEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var backdrop = ReplicaUIFactoryV2.CreatePanel("Backdrop", view.Root, config.BackdropColor);
        ReplicaUIFactoryV2.Stretch(backdrop);

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        var hint = ReplicaUIFactoryV2.CreateText(
            "Hint",
            backdrop,
            "Counter  |  Digits roll with spring transitions",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.HintColor);
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(920f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -44f);

        var panel = ReplicaUIFactoryV2.CreatePanel("CounterPanel", backdrop, config.PanelColor);
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = config.PanelSize;
        panel.anchoredPosition = config.PanelPosition;

        view.Content = panel;

        var panelShadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(panel.gameObject);
        panelShadow.effectColor = new Color(0f, 0f, 0f, 0.36f);
        panelShadow.effectDistance = new Vector2(0f, -8f);

        view.CounterRect = ReplicaUIFactoryV2.CreateRect("Counter", panel);
        view.CounterRect.anchorMin = new Vector2(0.5f, 0.5f);
        view.CounterRect.anchorMax = new Vector2(0.5f, 0.5f);
        view.CounterRect.pivot = new Vector2(0.5f, 0.5f);
        view.CounterRect.sizeDelta = config.CounterSize;
        view.CounterRect.anchoredPosition = config.CounterPosition;

        var row = ReplicaUIFactoryV2.EnsureComponent<HorizontalLayoutGroup>(view.CounterRect.gameObject);
        row.childAlignment = TextAnchor.MiddleCenter;
        row.childControlHeight = false;
        row.childControlWidth = false;
        row.childForceExpandHeight = false;
        row.childForceExpandWidth = false;
        row.spacing = config.ColumnSpacing;

        view.Columns.Clear();
        var places = config.Places != null && config.Places.Length > 0 ? config.Places : new[] { 10000, 1000, 100, 10, 1 };
        for (var i = 0; i < places.Length; i++)
        {
            view.Columns.Add(CreateDigitColumn(view.CounterRect, config, places[i]));
        }

        var gradientOverlay = ReplicaUIFactoryV2.CreateRect("Gradients", panel);
        gradientOverlay.anchorMin = new Vector2(0.5f, 0.5f);
        gradientOverlay.anchorMax = new Vector2(0.5f, 0.5f);
        gradientOverlay.pivot = new Vector2(0.5f, 0.5f);
        gradientOverlay.sizeDelta = view.CounterRect.sizeDelta;
        gradientOverlay.anchoredPosition = view.CounterRect.anchoredPosition;

        var top = ReplicaUIFactoryV2.CreatePanel("TopFade", gradientOverlay, config.FadeOverlay);
        top.anchorMin = new Vector2(0f, 1f);
        top.anchorMax = new Vector2(1f, 1f);
        top.pivot = new Vector2(0.5f, 1f);
        top.sizeDelta = new Vector2(0f, 56f);
        top.anchoredPosition = Vector2.zero;
        top.SetAsLastSibling();

        var bottom = ReplicaUIFactoryV2.CreatePanel("BottomFade", gradientOverlay, config.FadeOverlay);
        bottom.anchorMin = new Vector2(0f, 0f);
        bottom.anchorMax = new Vector2(1f, 0f);
        bottom.pivot = new Vector2(0.5f, 0f);
        bottom.sizeDelta = new Vector2(0f, 56f);
        bottom.anchoredPosition = Vector2.zero;
        bottom.SetAsLastSibling();

        return view;
    }

    private static DigitColumnView CreateDigitColumn(RectTransform parent, CounterRollupEffectConfig config, int place)
    {
        var digitRect = ReplicaUIFactoryV2.CreatePanel($"Digit_{place}", parent, config.DigitBackground);
        digitRect.sizeDelta = config.DigitSize;
        ReplicaUIFactoryV2.EnsureComponent<LayoutElement>(digitRect.gameObject).preferredWidth = config.DigitSize.x;

        var mask = ReplicaUIFactoryV2.EnsureComponent<Mask>(digitRect.gameObject);
        mask.showMaskGraphic = true;

        var numbersRoot = ReplicaUIFactoryV2.CreateRect("Numbers", digitRect);
        ReplicaUIFactoryV2.Stretch(numbersRoot);

        var column = new DigitColumnView
        {
            Place = place,
            Root = digitRect
        };

        for (var i = 0; i < 10; i++)
        {
            var number = ReplicaUIFactoryV2.CreateText(
                $"N_{i}",
                numbersRoot,
                i.ToString(),
                config.NumberFontSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            var numberRect = (RectTransform)number.transform;
            numberRect.anchorMin = new Vector2(0.5f, 0.5f);
            numberRect.anchorMax = new Vector2(0.5f, 0.5f);
            numberRect.pivot = new Vector2(0.5f, 0.5f);
            numberRect.sizeDelta = config.NumberSize;
            column.Numbers.Add(numberRect);
        }

        return column;
    }
}
