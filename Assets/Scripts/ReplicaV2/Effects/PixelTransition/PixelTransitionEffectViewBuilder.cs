using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class PixelTransitionEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Card;
    public RectTransform DefaultLayer;
    public Image DefaultImage;
    public Text DefaultText;
    public RectTransform ActiveLayer;
    public Image ActiveImage;
    public Text ActiveText;
    public RectTransform PixelsRoot;
    public readonly List<Image> Pixels = new List<Image>();
}

public static class PixelTransitionEffectViewBuilder
{
    public static PixelTransitionEffectView Build(
        RectTransform mountRoot,
        PixelTransitionEffectConfig config,
        PixelTransitionEffectController controller,
        PixelTransitionEffectModel model)
    {
        var view = new PixelTransitionEffectView();
        var safe = model != null ? model : PixelTransitionEffectModel.CreateDefault();

        view.Root = ReplicaUIFactoryV2.CreateRect("PixelTransitionEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        view.Card = ReplicaUIFactoryV2.CreatePanel("Card", view.Root, config.CardColor);
        view.Card.anchorMin = new Vector2(0.5f, 0.5f);
        view.Card.anchorMax = new Vector2(0.5f, 0.5f);
        view.Card.pivot = new Vector2(0.5f, 0.5f);
        view.Card.sizeDelta = config.CardSize;
        view.Card.anchoredPosition = Vector2.zero;

        var outline = ReplicaUIFactoryV2.EnsureComponent<Outline>(view.Card.gameObject);
        outline.effectColor = config.OutlineColor;
        outline.effectDistance = config.OutlineDistance;

        var relay = ReplicaUIFactoryV2.EnsureComponent<PixelTransitionInputRelay>(view.Card.gameObject);
        relay.Initialize(controller);

        view.DefaultLayer = ReplicaUIFactoryV2.CreatePanel("DefaultLayer", view.Card, new Color(1f, 1f, 1f, 0.001f));
        ReplicaUIFactoryV2.Stretch(view.DefaultLayer);
        view.DefaultLayer.GetComponent<Image>().raycastTarget = false;

        var defaultSpriteRect = ReplicaUIFactoryV2.CreatePanel("DefaultSprite", view.DefaultLayer, new Color(1f, 1f, 1f, 0.001f));
        ReplicaUIFactoryV2.Stretch(defaultSpriteRect);
        var defaultSpriteImage = defaultSpriteRect.GetComponent<Image>();
        defaultSpriteImage.raycastTarget = false;
        view.DefaultImage = defaultSpriteImage;

        view.DefaultText = ReplicaUIFactoryV2.CreateText(
            "DefaultLabel",
            view.DefaultLayer,
            safe.DefaultLabel,
            Mathf.Max(10, config.LabelFontSize),
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.DefaultLabelColor);
        ReplicaUIFactoryV2.Stretch((RectTransform)view.DefaultText.transform);

        view.ActiveLayer = ReplicaUIFactoryV2.CreatePanel("ActiveLayer", view.Card, new Color(1f, 1f, 1f, 0.001f));
        ReplicaUIFactoryV2.Stretch(view.ActiveLayer);
        view.ActiveLayer.gameObject.SetActive(false);
        view.ActiveLayer.GetComponent<Image>().raycastTarget = false;

        var activeSpriteRect = ReplicaUIFactoryV2.CreatePanel("ActiveSprite", view.ActiveLayer, new Color(1f, 1f, 1f, 0.001f));
        ReplicaUIFactoryV2.Stretch(activeSpriteRect);
        var activeSpriteImage = activeSpriteRect.GetComponent<Image>();
        activeSpriteImage.raycastTarget = false;
        view.ActiveImage = activeSpriteImage;

        view.ActiveText = ReplicaUIFactoryV2.CreateText(
            "ActiveLabel",
            view.ActiveLayer,
            safe.ActiveLabel,
            Mathf.Max(10, config.LabelFontSize),
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.ActiveLabelColor);
        ReplicaUIFactoryV2.Stretch((RectTransform)view.ActiveText.transform);

        view.PixelsRoot = ReplicaUIFactoryV2.CreateRect("Pixels", view.Card);
        ReplicaUIFactoryV2.Stretch(view.PixelsRoot);
        view.Pixels.Clear();

        var grid = Mathf.Clamp(config.GridSize, 2, 20);
        var inv = 1f / grid;
        for (var row = 0; row < grid; row++)
        {
            for (var col = 0; col < grid; col++)
            {
                var pixel = ReplicaUIFactoryV2.CreatePanel($"Px_{row}_{col}", view.PixelsRoot, config.PixelColor).GetComponent<Image>();
                var r = pixel.rectTransform;
                r.anchorMin = new Vector2(col * inv, row * inv);
                r.anchorMax = new Vector2((col + 1) * inv, (row + 1) * inv);
                r.offsetMin = Vector2.zero;
                r.offsetMax = Vector2.zero;
                pixel.raycastTarget = false;
                pixel.gameObject.SetActive(false);
                view.Pixels.Add(pixel);
            }
        }

        ApplyModel(view, safe);
        return view;
    }

    public static void ApplyModel(PixelTransitionEffectView view, PixelTransitionEffectModel model)
    {
        if (view == null || model == null)
        {
            return;
        }

        view.DefaultText.text = string.IsNullOrWhiteSpace(model.DefaultLabel) ? "DEFAULT" : model.DefaultLabel;
        view.ActiveText.text = string.IsNullOrWhiteSpace(model.ActiveLabel) ? "ACTIVE" : model.ActiveLabel;

        view.DefaultImage.sprite = model.DefaultSprite;
        view.DefaultImage.color = model.DefaultSprite != null ? Color.white : new Color(1f, 1f, 1f, 0.001f);
        view.DefaultImage.type = Image.Type.Simple;
        view.DefaultImage.preserveAspect = true;

        view.ActiveImage.sprite = model.ActiveSprite;
        view.ActiveImage.color = model.ActiveSprite != null ? Color.white : new Color(1f, 1f, 1f, 0.001f);
        view.ActiveImage.type = Image.Type.Simple;
        view.ActiveImage.preserveAspect = true;
    }
}
