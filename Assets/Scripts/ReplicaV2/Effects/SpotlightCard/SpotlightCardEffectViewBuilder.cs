using UnityEngine;
using UnityEngine.UI;

public sealed class SpotlightCardEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;

    public RectTransform Backdrop;
    public Text HintText;

    public RectTransform Card;
    public Image BorderImage;
    public RectTransform Inner;
    public Text TitleText;
    public Text BodyText;

    public RectTransform Spotlight;
    public CanvasGroup SpotlightGroup;
}

public static class SpotlightCardEffectViewBuilder
{
    private static Sprite sRadialSprite;

    public static SpotlightCardEffectView Build(RectTransform mountRoot, SpotlightCardEffectConfig config)
    {
        var view = new SpotlightCardEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("SpotlightCardEffect", mountRoot);
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
            "SpotlightCard  |  Hover card to move light",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.HintColor);
        var hintRect = (RectTransform)view.HintText.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(980f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -42f);

        view.Card = ReplicaUIFactoryV2.CreatePanel("SpotlightCard", view.Backdrop, config.CardColor);
        view.Card.anchorMin = new Vector2(0.5f, 0.5f);
        view.Card.anchorMax = new Vector2(0.5f, 0.5f);
        view.Card.pivot = new Vector2(0.5f, 0.5f);
        view.Card.sizeDelta = config.CardSize;
        view.Card.anchoredPosition = config.CardOffset;
        view.Card.GetComponent<Image>().raycastTarget = false;

        var borderFrame = ReplicaUIFactoryV2.CreatePanel("BorderFrame", view.Card, config.BorderIdleColor);
        ReplicaUIFactoryV2.Stretch(borderFrame);
        view.BorderImage = borderFrame.GetComponent<Image>();
        view.BorderImage.raycastTarget = false;

        view.Inner = ReplicaUIFactoryV2.CreatePanel("Inner", view.Card, config.InnerColor);
        ReplicaUIFactoryV2.Stretch(view.Inner);
        view.Inner.offsetMin = new Vector2(config.InnerInset, config.InnerInset);
        view.Inner.offsetMax = new Vector2(-config.InnerInset, -config.InnerInset);
        view.Inner.GetComponent<Image>().raycastTarget = false;

        ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(view.Inner.gameObject);

        view.TitleText = ReplicaUIFactoryV2.CreateText(
            "Title",
            view.Inner,
            "Interactive Spotlight Surface",
            46,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            config.TitleColor);
        var titleRect = (RectTransform)view.TitleText.transform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0f, 1f);
        titleRect.sizeDelta = new Vector2(-72f, 80f);
        titleRect.anchoredPosition = new Vector2(36f, -34f);

        view.BodyText = ReplicaUIFactoryV2.CreateText(
            "Body",
            view.Inner,
            "Hover to reveal follow-light glow.\nThe spotlight tracks pointer position in real time.",
            28,
            FontStyle.Normal,
            TextAnchor.UpperLeft,
            config.BodyColor);
        var bodyRect = (RectTransform)view.BodyText.transform;
        bodyRect.anchorMin = new Vector2(0f, 0f);
        bodyRect.anchorMax = new Vector2(1f, 1f);
        bodyRect.pivot = new Vector2(0f, 1f);
        bodyRect.sizeDelta = new Vector2(-72f, -180f);
        bodyRect.anchoredPosition = new Vector2(36f, -140f);

        view.Spotlight = ReplicaUIFactoryV2.CreateRect("Spotlight", view.Inner);
        view.Spotlight.anchorMin = new Vector2(0.5f, 0.5f);
        view.Spotlight.anchorMax = new Vector2(0.5f, 0.5f);
        view.Spotlight.pivot = new Vector2(0.5f, 0.5f);
        view.Spotlight.sizeDelta = config.SpotlightSize;
        view.Spotlight.anchoredPosition = Vector2.zero;

        var spotlightImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Spotlight.gameObject);
        spotlightImage.sprite = GetOrCreateRadialSprite();
        spotlightImage.color = config.SpotlightColor;
        spotlightImage.raycastTarget = false;

        view.SpotlightGroup = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Spotlight.gameObject);
        view.SpotlightGroup.alpha = 0f;
        view.SpotlightGroup.blocksRaycasts = false;
        view.SpotlightGroup.interactable = false;

        return view;
    }

    private static Sprite GetOrCreateRadialSprite()
    {
        if (sRadialSprite != null)
        {
            return sRadialSprite;
        }

        const int size = 128;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            name = "ReplicaRadialSprite_Spotlight_V2"
        };

        var center = (size - 1) * 0.5f;
        var maxDistance = center;

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var dx = x - center;
                var dy = y - center;
                var distance = Mathf.Sqrt((dx * dx) + (dy * dy)) / maxDistance;
                var alpha = Mathf.Clamp01(1f - distance);
                alpha = alpha * alpha * (3f - 2f * alpha);
                alpha *= Mathf.Clamp01(1f - (distance * 0.8f));
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply(false, false);
        sRadialSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return sRadialSprite;
    }
}

