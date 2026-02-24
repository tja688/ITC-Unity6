using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ReflectiveCardEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;
    public RectTransform Stage;
    public RectTransform Card;
    public Image CardImage;
    public Text HintText;
    public Text UserNameText;
    public Text RoleText;
    public Text IdNumberText;
    public Text BadgeText;
    public RectTransform Sheen;
    public Image SheenImage;
    public CanvasGroup SheenGroup;
    public RectTransform Spotlight;
    public Image SpotlightImage;
    public CanvasGroup SpotlightGroup;
    public readonly List<RectTransform> NoiseStrips = new List<RectTransform>();
    public readonly List<float> NoiseBaseY = new List<float>();
    public readonly List<Image> NoiseImages = new List<Image>();
}

public static class ReflectiveCardEffectViewBuilder
{
    private static Sprite sRadialSprite;

    public static ReflectiveCardEffectView Build(RectTransform mountRoot, ReflectiveCardEffectConfig config)
    {
        var view = new ReflectiveCardEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("ReflectiveCardEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var rootImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        rootImage.color = new Color(1f, 1f, 1f, 0f);
        rootImage.raycastTarget = false;

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        view.HintText = ReplicaUIFactoryV2.CreateText(
            "Hint",
            view.Root,
            "ReflectiveCard  |  Hover card for metallic sheen",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.HintColor);
        var hintRect = (RectTransform)view.HintText.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(1080f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -42f);

        view.Content = ReplicaUIFactoryV2.CreateRect("Content", view.Root);
        view.Content.anchorMin = new Vector2(0.5f, 0.5f);
        view.Content.anchorMax = new Vector2(0.5f, 0.5f);
        view.Content.pivot = new Vector2(0.5f, 0.5f);
        view.Content.sizeDelta = config.StageSize;
        view.Content.anchoredPosition = new Vector2(0f, -20f);

        view.Stage = ReplicaUIFactoryV2.CreatePanel("Stage", view.Content, config.StageColor);
        ReplicaUIFactoryV2.Stretch(view.Stage);
        view.Stage.GetComponent<Image>().raycastTarget = false;

        var stageShadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(view.Content.gameObject);
        stageShadow.effectColor = new Color(0f, 0f, 0f, 0.35f);
        stageShadow.effectDistance = new Vector2(0f, -10f);

        view.Card = ReplicaUIFactoryV2.CreatePanel("Card", view.Content, config.CardRestColor);
        view.Card.anchorMin = new Vector2(0.5f, 0.5f);
        view.Card.anchorMax = new Vector2(0.5f, 0.5f);
        view.Card.pivot = new Vector2(0.5f, 0.5f);
        view.Card.sizeDelta = config.CardSize;
        view.Card.anchoredPosition = Vector2.zero;

        view.CardImage = view.Card.GetComponent<Image>();
        view.CardImage.raycastTarget = false;

        var cardOutline = ReplicaUIFactoryV2.EnsureComponent<Outline>(view.Card.gameObject);
        cardOutline.effectColor = new Color(1f, 1f, 1f, 0.22f);
        cardOutline.effectDistance = new Vector2(1f, -1f);

        var cardShadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(view.Card.gameObject);
        cardShadow.effectColor = new Color(0f, 0f, 0f, 0.40f);
        cardShadow.effectDistance = new Vector2(0f, -10f);

        var overlayTint = ReplicaUIFactoryV2.CreatePanel("OverlayTint", view.Card, config.OverlayTint);
        ReplicaUIFactoryV2.Stretch(overlayTint);
        overlayTint.GetComponent<Image>().raycastTarget = false;

        var cardContent = ReplicaUIFactoryV2.CreateRect("CardContent", view.Card);
        ReplicaUIFactoryV2.Stretch(cardContent);

        BuildHeader(cardContent, config, view);
        BuildCenter(cardContent, config, view);
        BuildFooter(cardContent, config, view);
        BuildNoise(cardContent, config, view);
        BuildSheen(cardContent, config, view);
        BuildSpotlight(cardContent, config, view);

        return view;
    }

    private static void BuildHeader(RectTransform parent, ReflectiveCardEffectConfig config, ReflectiveCardEffectView view)
    {
        var header = ReplicaUIFactoryV2.CreateRect("Header", parent);
        header.anchorMin = new Vector2(0f, 1f);
        header.anchorMax = new Vector2(1f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        header.sizeDelta = new Vector2(-44f, 70f);
        header.anchoredPosition = new Vector2(0f, -26f);

        var headerLine = ReplicaUIFactoryV2.CreatePanel("HeaderLine", parent, new Color(1f, 1f, 1f, 0.18f));
        headerLine.anchorMin = new Vector2(0f, 1f);
        headerLine.anchorMax = new Vector2(1f, 1f);
        headerLine.pivot = new Vector2(0.5f, 1f);
        headerLine.sizeDelta = new Vector2(-44f, 1f);
        headerLine.anchoredPosition = new Vector2(0f, -96f);
        headerLine.GetComponent<Image>().raycastTarget = false;

        var secure = ReplicaUIFactoryV2.CreatePanel("SecureBadge", header, new Color(1f, 1f, 1f, 0.14f));
        secure.anchorMin = new Vector2(0f, 0.5f);
        secure.anchorMax = new Vector2(0f, 0.5f);
        secure.pivot = new Vector2(0f, 0.5f);
        secure.sizeDelta = new Vector2(170f, 34f);
        secure.anchoredPosition = Vector2.zero;
        secure.GetComponent<Image>().raycastTarget = false;

        var secureBorder = ReplicaUIFactoryV2.EnsureComponent<Outline>(secure.gameObject);
        secureBorder.effectColor = new Color(1f, 1f, 1f, 0.18f);
        secureBorder.effectDistance = new Vector2(1f, -1f);

        view.BadgeText = ReplicaUIFactoryV2.CreateText(
            "Label",
            secure,
            "\u25CF  SECURE ACCESS",
            14,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.94f, 0.96f, 1f, 0.92f));
        ReplicaUIFactoryV2.Stretch((RectTransform)view.BadgeText.transform);

        var statusDot = ReplicaUIFactoryV2.CreatePanel("Status", header, new Color(0.54f, 0.94f, 0.74f, 0.92f));
        statusDot.anchorMin = new Vector2(1f, 0.5f);
        statusDot.anchorMax = new Vector2(1f, 0.5f);
        statusDot.pivot = new Vector2(1f, 0.5f);
        statusDot.sizeDelta = new Vector2(22f, 22f);
        statusDot.anchoredPosition = Vector2.zero;
        statusDot.GetComponent<Image>().raycastTarget = false;
    }

    private static void BuildCenter(RectTransform parent, ReflectiveCardEffectConfig config, ReflectiveCardEffectView view)
    {
        view.UserNameText = ReplicaUIFactoryV2.CreateText(
            "UserName",
            parent,
            "ALEXANDER DOE",
            42,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.96f, 0.97f, 1f, 0.98f));
        var nameRect = (RectTransform)view.UserNameText.transform;
        nameRect.anchorMin = new Vector2(0.5f, 0.5f);
        nameRect.anchorMax = new Vector2(0.5f, 0.5f);
        nameRect.pivot = new Vector2(0.5f, 0.5f);
        nameRect.sizeDelta = new Vector2(330f, 64f);
        nameRect.anchoredPosition = new Vector2(0f, -20f);

        view.RoleText = ReplicaUIFactoryV2.CreateText(
            "Role",
            parent,
            "SENIOR DEVELOPER",
            16,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.92f, 0.94f, 1f, 0.58f));
        var roleRect = (RectTransform)view.RoleText.transform;
        roleRect.anchorMin = new Vector2(0.5f, 0.5f);
        roleRect.anchorMax = new Vector2(0.5f, 0.5f);
        roleRect.pivot = new Vector2(0.5f, 0.5f);
        roleRect.sizeDelta = new Vector2(300f, 40f);
        roleRect.anchoredPosition = new Vector2(0f, -68f);
    }

    private static void BuildFooter(RectTransform parent, ReflectiveCardEffectConfig config, ReflectiveCardEffectView view)
    {
        var footerLine = ReplicaUIFactoryV2.CreatePanel("FooterLine", parent, new Color(1f, 1f, 1f, 0.18f));
        footerLine.anchorMin = new Vector2(0f, 0f);
        footerLine.anchorMax = new Vector2(1f, 0f);
        footerLine.pivot = new Vector2(0.5f, 0f);
        footerLine.sizeDelta = new Vector2(-44f, 1f);
        footerLine.anchoredPosition = new Vector2(0f, 108f);
        footerLine.GetComponent<Image>().raycastTarget = false;

        var footer = ReplicaUIFactoryV2.CreateRect("Footer", parent);
        footer.anchorMin = new Vector2(0f, 0f);
        footer.anchorMax = new Vector2(1f, 0f);
        footer.pivot = new Vector2(0.5f, 0f);
        footer.sizeDelta = new Vector2(-44f, 84f);
        footer.anchoredPosition = new Vector2(0f, 24f);

        var idLabel = ReplicaUIFactoryV2.CreateText(
            "IDLabel",
            footer,
            "ID NUMBER",
            11,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            new Color(0.94f, 0.96f, 1f, 0.50f));
        var idLabelRect = (RectTransform)idLabel.transform;
        idLabelRect.anchorMin = new Vector2(0f, 1f);
        idLabelRect.anchorMax = new Vector2(0f, 1f);
        idLabelRect.pivot = new Vector2(0f, 1f);
        idLabelRect.sizeDelta = new Vector2(160f, 24f);
        idLabelRect.anchoredPosition = Vector2.zero;

        view.IdNumberText = ReplicaUIFactoryV2.CreateText(
            "IDValue",
            footer,
            "8901-2345-6789",
            20,
            FontStyle.Bold,
            TextAnchor.LowerLeft,
            new Color(0.94f, 0.97f, 1f, 0.90f));
        var idValueRect = (RectTransform)view.IdNumberText.transform;
        idValueRect.anchorMin = new Vector2(0f, 0f);
        idValueRect.anchorMax = new Vector2(0f, 0f);
        idValueRect.pivot = new Vector2(0f, 0f);
        idValueRect.sizeDelta = new Vector2(230f, 34f);
        idValueRect.anchoredPosition = Vector2.zero;

        var mark = ReplicaUIFactoryV2.CreateText(
            "Fingerprint",
            footer,
            "\u25A3",
            38,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.95f, 0.98f, 1f, 0.36f));
        var markRect = (RectTransform)mark.transform;
        markRect.anchorMin = new Vector2(1f, 0.5f);
        markRect.anchorMax = new Vector2(1f, 0.5f);
        markRect.pivot = new Vector2(1f, 0.5f);
        markRect.sizeDelta = new Vector2(72f, 72f);
        markRect.anchoredPosition = Vector2.zero;
    }

    private static void BuildNoise(RectTransform parent, ReflectiveCardEffectConfig config, ReflectiveCardEffectView view)
    {
        var noiseRoot = ReplicaUIFactoryV2.CreateRect("Noise", parent);
        ReplicaUIFactoryV2.Stretch(noiseRoot);
        ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(noiseRoot.gameObject);

        view.NoiseStrips.Clear();
        view.NoiseBaseY.Clear();
        view.NoiseImages.Clear();

        var stripCount = Mathf.Max(6, config.NoiseStripCount);
        var height = config.CardSize.y / stripCount;

        for (var i = 0; i < stripCount; i++)
        {
            var strip = ReplicaUIFactoryV2.CreatePanel($"Strip_{i}", noiseRoot, config.NoiseStripColor);
            strip.anchorMin = new Vector2(0.5f, 0f);
            strip.anchorMax = new Vector2(0.5f, 0f);
            strip.pivot = new Vector2(0.5f, 0f);
            strip.sizeDelta = new Vector2(config.CardSize.x + 56f, height + 2f);
            var y = i * height;
            strip.anchoredPosition = new Vector2(0f, y);
            strip.GetComponent<Image>().raycastTarget = false;

            view.NoiseStrips.Add(strip);
            view.NoiseBaseY.Add(y);
            view.NoiseImages.Add(strip.GetComponent<Image>());
        }
    }

    private static void BuildSheen(RectTransform parent, ReflectiveCardEffectConfig config, ReflectiveCardEffectView view)
    {
        view.Sheen = ReplicaUIFactoryV2.CreatePanel("Sheen", parent, config.SheenColor);
        view.Sheen.anchorMin = new Vector2(0.5f, 0.5f);
        view.Sheen.anchorMax = new Vector2(0.5f, 0.5f);
        view.Sheen.pivot = new Vector2(0.5f, 0.5f);
        view.Sheen.sizeDelta = config.SheenSize;
        view.Sheen.anchoredPosition = new Vector2(-220f, 0f);
        view.Sheen.localRotation = Quaternion.Euler(0f, 0f, 24f);

        view.SheenImage = view.Sheen.GetComponent<Image>();
        view.SheenImage.raycastTarget = false;

        view.SheenGroup = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Sheen.gameObject);
        view.SheenGroup.alpha = 0f;
        view.SheenGroup.blocksRaycasts = false;
    }

    private static void BuildSpotlight(RectTransform parent, ReflectiveCardEffectConfig config, ReflectiveCardEffectView view)
    {
        view.Spotlight = ReplicaUIFactoryV2.CreateRect("Spotlight", parent);
        view.Spotlight.anchorMin = new Vector2(0.5f, 0.5f);
        view.Spotlight.anchorMax = new Vector2(0.5f, 0.5f);
        view.Spotlight.pivot = new Vector2(0.5f, 0.5f);
        view.Spotlight.sizeDelta = config.SpotlightSize;
        view.Spotlight.anchoredPosition = Vector2.zero;

        view.SpotlightImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Spotlight.gameObject);
        view.SpotlightImage.sprite = GetOrCreateRadialSprite();
        view.SpotlightImage.color = config.SpotlightColor;
        view.SpotlightImage.raycastTarget = false;

        view.SpotlightGroup = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Spotlight.gameObject);
        view.SpotlightGroup.alpha = 0f;
        view.SpotlightGroup.blocksRaycasts = false;
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
            name = "ReplicaRadialSprite_Reflective_V2"
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
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply(false, false);
        sRadialSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return sRadialSprite;
    }
}

