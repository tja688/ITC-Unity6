using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class BounceCardsEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Backdrop;
    public RectTransform Container;
    public Text HintText;

    public readonly List<RectTransform> Cards = new List<RectTransform>();
    public readonly List<Vector2> BaseOffsets = new List<Vector2>();
    public readonly List<float> BaseRotations = new List<float>();
}

public static class BounceCardsEffectViewBuilder
{
    private static readonly Vector2[] sBaseOffsets =
    {
        new Vector2(-320f, -8f),
        new Vector2(-160f, 10f),
        Vector2.zero,
        new Vector2(160f, -8f),
        new Vector2(320f, 6f)
    };

    private static readonly float[] sBaseRotations = { 10f, 5f, -3f, -10f, 2f };

    private static readonly Color[] sCardColors =
    {
        new Color(0.20f, 0.49f, 0.82f, 1f),
        new Color(0.34f, 0.62f, 0.44f, 1f),
        new Color(0.77f, 0.42f, 0.30f, 1f),
        new Color(0.47f, 0.35f, 0.77f, 1f),
        new Color(0.87f, 0.66f, 0.29f, 1f)
    };

    public static BounceCardsEffectView Build(RectTransform mountRoot, BounceCardsEffectConfig config)
    {
        var view = new BounceCardsEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("BounceCardsEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        view.Backdrop = ReplicaUIFactoryV2.CreatePanel("Backdrop", view.Root, config.BackdropColor);
        view.Backdrop.anchorMin = new Vector2(0.5f, 0.5f);
        view.Backdrop.anchorMax = new Vector2(0.5f, 0.5f);
        view.Backdrop.pivot = new Vector2(0.5f, 0.5f);
        view.Backdrop.sizeDelta = config.StageSize;
        view.Backdrop.anchoredPosition = new Vector2(0f, -10f);
        view.Backdrop.GetComponent<Image>().raycastTarget = false;

        var plateA = ReplicaUIFactoryV2.CreatePanel("PlateA", view.Backdrop, config.PlateAColor);
        plateA.anchorMin = new Vector2(0.5f, 0.5f);
        plateA.anchorMax = new Vector2(0.5f, 0.5f);
        plateA.sizeDelta = new Vector2(1000f, 420f);
        plateA.anchoredPosition = new Vector2(-110f, 30f);
        plateA.localRotation = Quaternion.Euler(0f, 0f, -8f);
        plateA.GetComponent<Image>().raycastTarget = false;

        var plateB = ReplicaUIFactoryV2.CreatePanel("PlateB", view.Backdrop, config.PlateBColor);
        plateB.anchorMin = new Vector2(0.5f, 0.5f);
        plateB.anchorMax = new Vector2(0.5f, 0.5f);
        plateB.sizeDelta = new Vector2(980f, 380f);
        plateB.anchoredPosition = new Vector2(100f, -46f);
        plateB.localRotation = Quaternion.Euler(0f, 0f, 6f);
        plateB.GetComponent<Image>().raycastTarget = false;

        view.Container = ReplicaUIFactoryV2.CreateRect("Cards", view.Backdrop);
        view.Container.anchorMin = new Vector2(0.5f, 0.5f);
        view.Container.anchorMax = new Vector2(0.5f, 0.5f);
        view.Container.pivot = new Vector2(0.5f, 0.5f);
        view.Container.sizeDelta = config.ContainerSize;
        view.Container.anchoredPosition = Vector2.zero;

        view.HintText = ReplicaUIFactoryV2.CreateText(
            "Hint",
            view.Backdrop,
            "BounceCards  |  Hover a card to push siblings",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.HintColor);
        var titleRect = (RectTransform)view.HintText.transform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(980f, 56f);
        titleRect.anchoredPosition = new Vector2(0f, -40f);

        view.Cards.Clear();
        view.BaseOffsets.Clear();
        view.BaseRotations.Clear();

        var count = Mathf.Min(Mathf.Min(sBaseOffsets.Length, sBaseRotations.Length), sCardColors.Length);
        for (var i = 0; i < count; i++)
        {
            var card = BuildCard(view.Container, config, i);
            view.Cards.Add(card);
            view.BaseOffsets.Add(sBaseOffsets[i]);
            view.BaseRotations.Add(sBaseRotations[i]);
        }

        return view;
    }

    private static RectTransform BuildCard(RectTransform parent, BounceCardsEffectConfig config, int index)
    {
        var cardRect = ReplicaUIFactoryV2.CreateRect($"Card_{index}", parent);
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.sizeDelta = config.CardSize;
        cardRect.anchoredPosition = sBaseOffsets[index];
        cardRect.localRotation = Quaternion.Euler(0f, 0f, sBaseRotations[index]);

        var cardImage = ReplicaUIFactoryV2.EnsureComponent<Image>(cardRect.gameObject);
        cardImage.color = sCardColors[index];
        cardImage.raycastTarget = false;

        var border = ReplicaUIFactoryV2.EnsureComponent<Outline>(cardRect.gameObject);
        border.effectColor = config.OutlineColor;
        border.effectDistance = config.OutlineDistance;
        border.useGraphicAlpha = true;

        var shadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(cardRect.gameObject);
        shadow.effectColor = config.ShadowColor;
        shadow.effectDistance = config.ShadowDistance;
        shadow.useGraphicAlpha = true;

        var label = ReplicaUIFactoryV2.CreateText(
            "Label",
            cardRect,
            $"Card {index + 1}",
            32,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(1f, 1f, 1f, 0.96f));
        ReplicaUIFactoryV2.Stretch((RectTransform)label.transform);

        return cardRect;
    }
}

