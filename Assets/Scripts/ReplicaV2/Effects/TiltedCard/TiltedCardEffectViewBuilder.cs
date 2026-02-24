using UnityEngine;
using UnityEngine.UI;

public sealed class TiltedCardEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;
    public RectTransform Card;
    public RectTransform Glint;
    public RectTransform OverlayBand;
    public Text BadgeText;
    public RectTransform Tooltip;
    public CanvasGroup TooltipGroup;
    public Text TooltipText;
    public Text HintText;
}

public static class TiltedCardEffectViewBuilder
{
    public static TiltedCardEffectView Build(RectTransform mountRoot, TiltedCardEffectConfig config)
    {
        if (config != null && config.Prefab != null)
        {
            var go = UnityEngine.Object.Instantiate(config.Prefab, mountRoot);
            go.name = config.Prefab.name;
            if (go.TryGetComponent<TiltedCardViewLinker>(out var linker))
            {
                return linker.ToView();
            }
        }

        var view = new TiltedCardEffectView();
        if (config == null) config = ScriptableObject.CreateInstance<TiltedCardEffectConfig>();

        view.Root = ReplicaUIFactoryV2.CreateRect("TiltedCardEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var rootImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        rootImage.color = config.RootBackground;
        rootImage.raycastTarget = false;

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        var ambientA = ReplicaUIFactoryV2.CreatePanel("AmbientA", view.Root, config.AmbientA);
        ambientA.anchorMin = new Vector2(0.5f, 0.5f);
        ambientA.anchorMax = new Vector2(0.5f, 0.5f);
        ambientA.pivot = new Vector2(0.5f, 0.5f);
        ambientA.sizeDelta = new Vector2(1380f, 560f);
        ambientA.anchoredPosition = new Vector2(0f, 120f);
        ambientA.localRotation = Quaternion.Euler(0f, 0f, 8f);
        ambientA.GetComponent<Image>().raycastTarget = false;

        view.HintText = ReplicaUIFactoryV2.CreateText(
            "Hint",
            view.Root,
            "TiltedCard  |  Move cursor around the card",
            30,
            FontStyle.Bold,
            TextAnchor.UpperCenter,
            config.HintColor);
        var hintRect = (RectTransform)view.HintText.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(820f, 48f);
        hintRect.anchoredPosition = new Vector2(0f, -38f);

        view.Content = ReplicaUIFactoryV2.CreateRect("CardFrame", view.Root);
        view.Content.anchorMin = new Vector2(0.5f, 0.5f);
        view.Content.anchorMax = new Vector2(0.5f, 0.5f);
        view.Content.pivot = new Vector2(0.5f, 0.5f);
        view.Content.sizeDelta = config.FrameSize;
        view.Content.anchoredPosition = new Vector2(0f, -22f);

        view.Card = ReplicaUIFactoryV2.CreatePanel("Card", view.Content, config.CardColor);
        view.Card.anchorMin = new Vector2(0.5f, 0.5f);
        view.Card.anchorMax = new Vector2(0.5f, 0.5f);
        view.Card.pivot = new Vector2(0.5f, 0.5f);
        view.Card.sizeDelta = config.CardSize;
        view.Card.anchoredPosition = Vector2.zero;
        var cardImage = view.Card.GetComponent<Image>();
        cardImage.raycastTarget = false;

        var cardShadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(view.Card.gameObject);
        cardShadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
        cardShadow.effectDistance = new Vector2(0f, -12f);

        var artwork = ReplicaUIFactoryV2.CreatePanel("Artwork", view.Card, config.ArtworkColor);
        ReplicaUIFactoryV2.Stretch(artwork);
        artwork.GetComponent<Image>().raycastTarget = false;

        view.OverlayBand = ReplicaUIFactoryV2.CreatePanel("OverlayBand", view.Card, config.OverlayBandColor);
        view.OverlayBand.anchorMin = new Vector2(0f, 0f);
        view.OverlayBand.anchorMax = new Vector2(1f, 0f);
        view.OverlayBand.pivot = new Vector2(0.5f, 0f);
        view.OverlayBand.sizeDelta = new Vector2(0f, 72f);
        view.OverlayBand.anchoredPosition = Vector2.zero;
        view.OverlayBand.GetComponent<Image>().raycastTarget = false;

        view.BadgeText = ReplicaUIFactoryV2.CreateText(
            "Badge",
            view.OverlayBand,
            "HOVER TO TILT",
            18,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(1f, 1f, 1f, 0.9f));
        ReplicaUIFactoryV2.Stretch((RectTransform)view.BadgeText.transform);

        view.Glint = ReplicaUIFactoryV2.CreatePanel("Glint", view.Card, config.GlintColor);
        view.Glint.anchorMin = new Vector2(0.5f, 0.5f);
        view.Glint.anchorMax = new Vector2(0.5f, 0.5f);
        view.Glint.pivot = new Vector2(0.5f, 0.5f);
        view.Glint.sizeDelta = new Vector2(160f, 340f);
        view.Glint.anchoredPosition = Vector2.zero;
        view.Glint.GetComponent<Image>().raycastTarget = false;

        view.Tooltip = ReplicaUIFactoryV2.CreatePanel("Tooltip", view.Content, config.TooltipColor);
        view.Tooltip.anchorMin = new Vector2(0.5f, 0.5f);
        view.Tooltip.anchorMax = new Vector2(0.5f, 0.5f);
        view.Tooltip.pivot = new Vector2(0f, 0.5f);
        view.Tooltip.sizeDelta = config.TooltipSize;
        view.Tooltip.anchoredPosition = new Vector2(40f, 80f);
        view.Tooltip.GetComponent<Image>().raycastTarget = false;

        view.TooltipGroup = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Tooltip.gameObject);
        view.TooltipGroup.alpha = 0f;
        view.TooltipGroup.blocksRaycasts = false;

        view.TooltipText = ReplicaUIFactoryV2.CreateText(
            "TooltipText",
            view.Tooltip,
            "x:0.00 y:0.00",
            13,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.TooltipTextColor);
        ReplicaUIFactoryV2.Stretch((RectTransform)view.TooltipText.transform);

        return view;
    }

    public static void Link(GameObject go, TiltedCardEffectView view)
    {
        var linker = ReplicaUIFactoryV2.EnsureComponent<TiltedCardViewLinker>(go);
        linker.Root = view.Root;
        linker.Group = view.Group;
        linker.Content = view.Content;
        linker.Card = view.Card;
        linker.Glint = view.Glint;
        linker.OverlayBand = view.OverlayBand;
        linker.BadgeText = view.BadgeText;
        linker.Tooltip = view.Tooltip;
        linker.TooltipGroup = view.TooltipGroup;
        linker.TooltipText = view.TooltipText;
        linker.HintText = view.HintText;
    }
}

