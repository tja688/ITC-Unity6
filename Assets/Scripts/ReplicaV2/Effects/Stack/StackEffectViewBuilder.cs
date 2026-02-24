using UnityEngine;
using UnityEngine.UI;

public sealed class StackEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Deck;
}

public sealed class StackCardView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Shadow;
    public Image ShadowImage;
    public RectTransform Tilt;
    public RectTransform Visual;
    public Image VisualImage;
    public Text TitleText;
    public Text MetaText;
    public Image HitImage;
    public StackCardInputRelay InputRelay;
}

public static class StackEffectViewBuilder
{
    public static StackEffectView Build(RectTransform mountRoot, StackEffectConfig config)
    {
        var root = ReplicaUIFactoryV2.CreateRect("StackEffect", mountRoot);
        root.anchorMin = new Vector2(0.5f, 0.5f);
        root.anchorMax = new Vector2(0.5f, 0.5f);
        root.pivot = new Vector2(0.5f, 0.5f);
        root.sizeDelta = config.StackSize;
        root.anchoredPosition = Vector2.zero;

        var group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(root.gameObject);
        group.alpha = 1f;
        group.blocksRaycasts = true;
        group.interactable = true;

        var deck = ReplicaUIFactoryV2.CreateRect("Deck", root);
        deck.anchorMin = new Vector2(0.5f, 0.5f);
        deck.anchorMax = new Vector2(0.5f, 0.5f);
        deck.pivot = new Vector2(0.5f, 0.5f);
        deck.sizeDelta = config.StackSize;
        deck.anchoredPosition = Vector2.zero;

        return new StackEffectView
        {
            Root = root,
            Group = group,
            Deck = deck
        };
    }

    public static StackCardView CreateCard(RectTransform deck, StackEffectConfig config, StackEffectController controller)
    {
        var cardGo = new GameObject("Card", typeof(RectTransform), typeof(Image), typeof(StackCardInputRelay));
        var root = cardGo.GetComponent<RectTransform>();
        root.SetParent(deck, false);
        root.anchorMin = new Vector2(0.5f, 0.5f);
        root.anchorMax = new Vector2(0.5f, 0.5f);
        root.pivot = new Vector2(0.5f, 0.5f);
        root.sizeDelta = config.CardSize;

        var hit = cardGo.GetComponent<Image>();
        hit.color = new Color(1f, 1f, 1f, 0.001f);
        hit.raycastTarget = true;

        var group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(cardGo);
        group.alpha = 1f;
        group.blocksRaycasts = true;
        group.interactable = true;

        var shadow = ReplicaUIFactoryV2.CreatePanel("Shadow", root, config.ShadowColor);
        shadow.anchorMin = new Vector2(0.5f, 0.5f);
        shadow.anchorMax = new Vector2(0.5f, 0.5f);
        shadow.pivot = new Vector2(0.5f, 0.5f);
        shadow.sizeDelta = config.CardSize + new Vector2(26f, 26f);
        shadow.anchoredPosition = new Vector2(0f, -10f);
        shadow.localScale = Vector3.one;

        var tilt = ReplicaUIFactoryV2.CreateRect("Tilt", root);
        tilt.anchorMin = new Vector2(0.5f, 0.5f);
        tilt.anchorMax = new Vector2(0.5f, 0.5f);
        tilt.pivot = new Vector2(0.5f, 0.5f);
        tilt.sizeDelta = config.CardSize;
        tilt.anchoredPosition = Vector2.zero;

        var visual = ReplicaUIFactoryV2.CreatePanel("CardVisual", tilt, config.FallbackCardColor);
        visual.anchorMin = new Vector2(0.5f, 0.5f);
        visual.anchorMax = new Vector2(0.5f, 0.5f);
        visual.pivot = new Vector2(0.5f, 0.5f);
        visual.sizeDelta = config.CardSize;
        visual.anchoredPosition = Vector2.zero;

        var outline = ReplicaUIFactoryV2.EnsureComponent<Outline>(visual.gameObject);
        outline.effectColor = new Color(0f, 0f, 0f, 0.18f);
        outline.effectDistance = new Vector2(2f, -2f);
        outline.useGraphicAlpha = true;

        var title = ReplicaUIFactoryV2.CreateText(
            "Title",
            visual,
            "",
            44,
            FontStyle.Bold,
            TextAnchor.UpperCenter,
            config.LabelColor);
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(-60f, 78f);
        titleRect.anchoredPosition = new Vector2(0f, -26f);

        var meta = ReplicaUIFactoryV2.CreateText(
            "Meta",
            visual,
            "",
            24,
            FontStyle.Normal,
            TextAnchor.LowerCenter,
            config.MetaColor);
        var metaRect = (RectTransform)meta.transform;
        metaRect.anchorMin = new Vector2(0.5f, 0f);
        metaRect.anchorMax = new Vector2(0.5f, 0f);
        metaRect.pivot = new Vector2(0.5f, 0f);
        metaRect.sizeDelta = new Vector2(config.CardSize.x - 84f, 56f);
        metaRect.anchoredPosition = new Vector2(0f, 26f);

        var relay = cardGo.GetComponent<StackCardInputRelay>();
        relay.Initialize(controller);

        return new StackCardView
        {
            Root = root,
            Group = group,
            Shadow = shadow,
            ShadowImage = shadow.GetComponent<Image>(),
            Tilt = tilt,
            Visual = visual,
            VisualImage = visual.GetComponent<Image>(),
            TitleText = title,
            MetaText = meta,
            HitImage = hit,
            InputRelay = relay
        };
    }
}
