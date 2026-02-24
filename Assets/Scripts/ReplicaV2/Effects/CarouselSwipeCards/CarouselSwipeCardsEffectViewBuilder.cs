using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class CarouselSwipeCardsEffectView
{
    public RectTransform Root;
    public RectTransform CardStage;
    public readonly List<CarouselSwipeCardsCardView> Cards = new List<CarouselSwipeCardsCardView>();
    public readonly List<Image> Indicators = new List<Image>();
}

public sealed class CarouselSwipeCardsCardView
{
    public RectTransform Rect;
    public CanvasGroup Group;
}

public static class CarouselSwipeCardsEffectViewBuilder
{
    public static CarouselSwipeCardsEffectView Build(
        RectTransform mountRoot,
        CarouselSwipeCardsEffectConfig config,
        CarouselSwipeCardsEffectController controller,
        CarouselSwipeCardsEffectModel model)
    {
        var view = new CarouselSwipeCardsEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("CarouselSwipeCardsEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var raycastImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        raycastImage.color = new Color(1f, 1f, 1f, 0.001f);
        raycastImage.raycastTarget = true;

        var input = ReplicaUIFactoryV2.EnsureComponent<CarouselSwipeCardsInputRelay>(view.Root.gameObject);
        input.Initialize(controller);

        var backdrop = ReplicaUIFactoryV2.CreatePanel("Backdrop", view.Root, config.BackdropColor);
        ReplicaUIFactoryV2.Stretch(backdrop);

        var hint = ReplicaUIFactoryV2.CreateText(
            "Hint",
            backdrop,
            "Carousel  |  Drag cards or click indicators",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.93f, 0.96f, 1f, 0.95f));
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(880f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -40f);

        var frame = ReplicaUIFactoryV2.CreatePanel("Frame", backdrop, config.FrameColor);
        frame.anchorMin = new Vector2(0.5f, 0.5f);
        frame.anchorMax = new Vector2(0.5f, 0.5f);
        frame.pivot = new Vector2(0.5f, 0.5f);
        frame.sizeDelta = config.FrameSize;
        frame.anchoredPosition = new Vector2(0f, -20f);

        var frameOutline = ReplicaUIFactoryV2.EnsureComponent<Outline>(frame.gameObject);
        frameOutline.effectColor = config.FrameOutlineColor;
        frameOutline.effectDistance = new Vector2(1f, -1f);

        view.CardStage = ReplicaUIFactoryV2.CreateRect("CardStage", frame);
        view.CardStage.anchorMin = new Vector2(0.5f, 0.5f);
        view.CardStage.anchorMax = new Vector2(0.5f, 0.5f);
        view.CardStage.pivot = new Vector2(0.5f, 0.5f);
        view.CardStage.sizeDelta = config.StageSize;
        view.CardStage.anchoredPosition = config.StageOffset;

        view.Cards.Clear();
        var safe = model != null ? model : CarouselSwipeCardsEffectModel.CreateDefault();
        var count = safe.Cards != null ? safe.Cards.Count : 0;
        for (var i = 0; i < count; i++)
        {
            view.Cards.Add(CreateCard(i, view.CardStage, config, safe.Cards[i]));
        }

        var indicatorRoot = ReplicaUIFactoryV2.CreateRect("Indicators", frame);
        indicatorRoot.anchorMin = new Vector2(0.5f, 0f);
        indicatorRoot.anchorMax = new Vector2(0.5f, 0f);
        indicatorRoot.pivot = new Vector2(0.5f, 0f);
        indicatorRoot.sizeDelta = new Vector2(360f, 36f);
        indicatorRoot.anchoredPosition = new Vector2(0f, 34f);

        var row = ReplicaUIFactoryV2.EnsureComponent<HorizontalLayoutGroup>(indicatorRoot.gameObject);
        row.childAlignment = TextAnchor.MiddleCenter;
        row.childControlHeight = false;
        row.childControlWidth = false;
        row.childForceExpandWidth = false;
        row.childForceExpandHeight = false;
        row.spacing = 18f;

        view.Indicators.Clear();
        for (var i = 0; i < count; i++)
        {
            var index = i;
            var indicatorBtn = ReplicaUIFactoryV2.CreateButton($"Indicator_{i}", indicatorRoot, new Color(1f, 1f, 1f, 0.3f));
            var indicatorRect = (RectTransform)indicatorBtn.transform;
            indicatorRect.sizeDelta = new Vector2(14f, 14f);
            indicatorBtn.onClick.AddListener(() => controller.SetIndex(index, false));
            view.Indicators.Add(indicatorBtn.image);
        }

        return view;
    }

    private static CarouselSwipeCardsCardView CreateCard(int index, RectTransform stage, CarouselSwipeCardsEffectConfig config, CarouselSwipeCardsCardData data)
    {
        var itemRect = ReplicaUIFactoryV2.CreatePanel($"Item_{index}", stage, data != null ? data.CardColor : Color.white);
        itemRect.anchorMin = new Vector2(0.5f, 0.5f);
        itemRect.anchorMax = new Vector2(0.5f, 0.5f);
        itemRect.pivot = new Vector2(0.5f, 0.5f);
        itemRect.sizeDelta = config.CardSize;
        itemRect.anchoredPosition = Vector2.zero;

        var itemGroup = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(itemRect.gameObject);
        itemGroup.alpha = 1f;
        itemGroup.blocksRaycasts = false;
        itemGroup.interactable = false;

        var outline = ReplicaUIFactoryV2.EnsureComponent<Outline>(itemRect.gameObject);
        outline.effectColor = new Color(1f, 1f, 1f, 0.24f);
        outline.effectDistance = new Vector2(1.5f, -1.5f);

        var accent = ReplicaUIFactoryV2.CreatePanel("Accent", itemRect, data != null ? data.AccentColor : Color.white);
        accent.anchorMin = new Vector2(0f, 1f);
        accent.anchorMax = new Vector2(1f, 1f);
        accent.pivot = new Vector2(0.5f, 1f);
        accent.sizeDelta = new Vector2(0f, 66f);
        accent.anchoredPosition = Vector2.zero;

        var iconCircle = ReplicaUIFactoryV2.CreatePanel("Icon", itemRect, Color.white);
        iconCircle.anchorMin = new Vector2(0f, 1f);
        iconCircle.anchorMax = new Vector2(0f, 1f);
        iconCircle.pivot = new Vector2(0f, 1f);
        iconCircle.sizeDelta = new Vector2(58f, 58f);
        iconCircle.anchoredPosition = new Vector2(20f, -82f);

        var iconText = ReplicaUIFactoryV2.CreateText(
            "IconText",
            iconCircle,
            ((char)('A' + index)).ToString(),
            24,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.05f, 0.07f, 0.10f, 1f));
        ReplicaUIFactoryV2.Stretch((RectTransform)iconText.transform);

        var title = ReplicaUIFactoryV2.CreateText(
            "Title",
            itemRect,
            data != null ? data.Title : "",
            40,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            Color.white);
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0f, 1f);
        titleRect.sizeDelta = new Vector2(-34f, 96f);
        titleRect.anchoredPosition = new Vector2(96f, -92f);

        var desc = ReplicaUIFactoryV2.CreateText(
            "Description",
            itemRect,
            data != null ? data.Description : "",
            28,
            FontStyle.Normal,
            TextAnchor.UpperLeft,
            new Color(1f, 1f, 1f, 0.92f));
        var descRect = (RectTransform)desc.transform;
        descRect.anchorMin = new Vector2(0f, 0f);
        descRect.anchorMax = new Vector2(1f, 1f);
        descRect.pivot = new Vector2(0f, 0f);
        descRect.sizeDelta = new Vector2(-54f, -220f);
        descRect.anchoredPosition = new Vector2(24f, 28f);

        return new CarouselSwipeCardsCardView
        {
            Rect = itemRect,
            Group = itemGroup
        };
    }
}
