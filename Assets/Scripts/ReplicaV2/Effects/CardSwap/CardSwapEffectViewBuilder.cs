using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class CardSwapCardView
{
    public RectTransform Rect;
    public RectTransform Visual;
    public Text TitleText;
    public Text CaptionText;
    public CardSwapCardInputRelay InputRelay;
}

public sealed class CardSwapEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Backdrop;
    public Text HintText;
    public RectTransform Surface;
    public RectTransform Deck;
    public readonly List<CardSwapCardView> Cards = new List<CardSwapCardView>();
}

public static class CardSwapEffectViewBuilder
{
    private static readonly Color[] sCardColors =
    {
        new Color(0.20f, 0.42f, 0.70f, 1f),
        new Color(0.17f, 0.56f, 0.50f, 1f),
        new Color(0.70f, 0.41f, 0.24f, 1f),
        new Color(0.46f, 0.33f, 0.72f, 1f)
    };

    public static CardSwapEffectView Build(RectTransform mountRoot, CardSwapEffectConfig config)
    {
        var view = new CardSwapEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("CardSwapEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        view.Backdrop = ReplicaUIFactoryV2.CreatePanel("Backdrop", view.Root, config.BackdropColor);
        ReplicaUIFactoryV2.Stretch(view.Backdrop);
        view.Backdrop.GetComponent<Image>().raycastTarget = false;

        view.HintText = ReplicaUIFactoryV2.CreateText(
            "Hint",
            view.Backdrop,
            "CardSwap  |  Auto swaps top card to back",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.HintColor);
        var hintRect = (RectTransform)view.HintText.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(900f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -40f);

        view.Surface = ReplicaUIFactoryV2.CreatePanel("Surface", view.Backdrop, config.SurfaceColor);
        view.Surface.anchorMin = new Vector2(0.5f, 0.5f);
        view.Surface.anchorMax = new Vector2(0.5f, 0.5f);
        view.Surface.pivot = new Vector2(0.5f, 0.5f);
        view.Surface.sizeDelta = config.SurfaceSize;
        view.Surface.anchoredPosition = new Vector2(0f, -12f);
        view.Surface.GetComponent<Image>().raycastTarget = false;

        var shadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(view.Surface.gameObject);
        shadow.effectColor = config.SurfaceShadowColor;
        shadow.effectDistance = config.SurfaceShadowDistance;

        view.Deck = ReplicaUIFactoryV2.CreateRect("Deck", view.Surface);
        view.Deck.anchorMin = new Vector2(0.5f, 0.5f);
        view.Deck.anchorMax = new Vector2(0.5f, 0.5f);
        view.Deck.pivot = new Vector2(0.5f, 0.5f);
        view.Deck.sizeDelta = config.DeckSize;
        view.Deck.anchoredPosition = Vector2.zero;

        view.Cards.Clear();
        for (var i = 0; i < sCardColors.Length; i++)
        {
            view.Cards.Add(BuildCard(view.Deck, config, i));
        }

        return view;
    }

    private static CardSwapCardView BuildCard(RectTransform parent, CardSwapEffectConfig config, int index)
    {
        var cardRect = ReplicaUIFactoryV2.CreateRect($"Card_{index + 1}", parent);
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = config.CardSize;
        cardRect.anchoredPosition = Vector2.zero;

        var cardImage = ReplicaUIFactoryV2.EnsureComponent<Image>(cardRect.gameObject);
        cardImage.color = sCardColors[index];
        cardImage.raycastTarget = true;

        var outline = ReplicaUIFactoryV2.EnsureComponent<Outline>(cardRect.gameObject);
        outline.effectColor = config.OutlineColor;
        outline.effectDistance = config.OutlineDistance;

        var title = ReplicaUIFactoryV2.CreateText(
            "Title",
            cardRect,
            $"Panel {index + 1}",
            42,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        ReplicaUIFactoryV2.Stretch((RectTransform)title.transform);

        var caption = ReplicaUIFactoryV2.CreateText(
            "Caption",
            cardRect,
            "Click the front card to swap now",
            24,
            FontStyle.Normal,
            TextAnchor.LowerCenter,
            new Color(1f, 1f, 1f, 0.86f));
        var captionRect = (RectTransform)caption.transform;
        captionRect.anchorMin = new Vector2(0.5f, 0f);
        captionRect.anchorMax = new Vector2(0.5f, 0f);
        captionRect.pivot = new Vector2(0.5f, 0f);
        captionRect.sizeDelta = new Vector2(420f, 42f);
        captionRect.anchoredPosition = new Vector2(0f, 18f);

        var clickProxy = cardRect.gameObject.AddComponent<CardSwapCardInputRelay>();

        return new CardSwapCardView
        {
            Rect = cardRect,
            Visual = cardRect,
            TitleText = title,
            CaptionText = caption,
            InputRelay = clickProxy
        };
    }
}

