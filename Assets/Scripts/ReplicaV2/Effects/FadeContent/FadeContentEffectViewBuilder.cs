using UnityEngine;
using UnityEngine.UI;

public sealed class FadeContentEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Card;
    public Text TitleText;
    public Text BodyText;
}

public static class FadeContentEffectViewBuilder
{
    public static FadeContentEffectView Build(RectTransform mountRoot, FadeContentEffectConfig config)
    {
        var view = new FadeContentEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("FadeContentEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);
        view.Root.anchoredPosition = Vector2.zero;

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

        var title = ReplicaUIFactoryV2.CreateText(
            "Title",
            view.Card,
            "Fade Content",
            Mathf.Max(10, config.TitleFontSize),
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            config.TitleColor);
        view.TitleText = title;
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0f, 1f);
        titleRect.sizeDelta = new Vector2(-56f, 74f);
        titleRect.anchoredPosition = new Vector2(28f, -22f);

        var body = ReplicaUIFactoryV2.CreateText(
            "Body",
            view.Card,
            "Basic but essential transition wrapper.",
            Mathf.Max(10, config.BodyFontSize),
            FontStyle.Normal,
            TextAnchor.UpperLeft,
            config.BodyColor);
        view.BodyText = body;
        var bodyRect = (RectTransform)body.transform;
        bodyRect.anchorMin = new Vector2(0f, 0f);
        bodyRect.anchorMax = new Vector2(1f, 1f);
        bodyRect.pivot = new Vector2(0f, 1f);
        bodyRect.sizeDelta = new Vector2(-56f, -96f);
        bodyRect.anchoredPosition = new Vector2(28f, -90f);

        return view;
    }
}
