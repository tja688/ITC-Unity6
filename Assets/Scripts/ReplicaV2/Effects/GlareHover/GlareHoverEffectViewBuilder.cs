using UnityEngine;
using UnityEngine.UI;

public sealed class GlareHoverEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Container;
    public Image ContainerImage;
    public Outline ContainerOutline;
    public RectMask2D Mask;
    public RectTransform Glare;
    public Image GlareImage;
    public Text LabelText;
}

public static class GlareHoverEffectViewBuilder
{
    private static Sprite sGlareSprite;

    public static GlareHoverEffectView Build(RectTransform mountRoot, GlareHoverEffectConfig config)
    {
        var view = new GlareHoverEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("GlareHoverEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var rootImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        rootImage.color = new Color(1f, 1f, 1f, 0f);
        rootImage.raycastTarget = false;

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        view.Container = ReplicaUIFactoryV2.CreatePanel("Container", view.Root, config.BackgroundColor);
        view.Container.anchorMin = new Vector2(0.5f, 0.5f);
        view.Container.anchorMax = new Vector2(0.5f, 0.5f);
        view.Container.pivot = new Vector2(0.5f, 0.5f);
        view.Container.sizeDelta = config.ContainerSize;
        view.Container.anchoredPosition = Vector2.zero;

        view.ContainerImage = view.Container.GetComponent<Image>();
        view.ContainerImage.raycastTarget = false;

        view.ContainerOutline = ReplicaUIFactoryV2.EnsureComponent<Outline>(view.Container.gameObject);
        view.ContainerOutline.effectColor = config.BorderColor;
        view.ContainerOutline.effectDistance = new Vector2(config.BorderThickness, -config.BorderThickness);

        view.Mask = ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(view.Container.gameObject);

        view.Glare = ReplicaUIFactoryV2.CreateRect("Glare", view.Container);
        view.Glare.anchorMin = new Vector2(0.5f, 0.5f);
        view.Glare.anchorMax = new Vector2(0.5f, 0.5f);
        view.Glare.pivot = new Vector2(0.5f, 0.5f);

        var sizePercent = Mathf.Max(100f, config.GlareSizePercent) / 100f;
        var glareSize = Mathf.Max(config.ContainerSize.x, config.ContainerSize.y) * sizePercent;
        view.Glare.sizeDelta = new Vector2(glareSize, glareSize);
        view.Glare.localRotation = Quaternion.Euler(0f, 0f, config.GlareAngleDeg);

        view.GlareImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Glare.gameObject);
        view.GlareImage.sprite = GetOrCreateGlareSprite();
        view.GlareImage.color = new Color(config.GlareColor.r, config.GlareColor.g, config.GlareColor.b, Mathf.Clamp01(config.GlareOpacity));
        view.GlareImage.raycastTarget = false;

        view.LabelText = ReplicaUIFactoryV2.CreateText(
            "Label",
            view.Container,
            "GlareHover",
            Mathf.Max(1, config.LabelFontSize),
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.LabelColor);
        ReplicaUIFactoryV2.Stretch((RectTransform)view.LabelText.transform);

        return view;
    }

    private static Sprite GetOrCreateGlareSprite()
    {
        if (sGlareSprite != null)
        {
            return sGlareSprite;
        }

        const int width = 256;
        const int height = 64;

        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            name = "ReplicaGlareSprite_V2"
        };

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var t = x / (float)(width - 1);
                var d = Mathf.Abs(t - 0.68f) / 0.16f;
                var alpha = Mathf.Clamp01(1f - d);
                alpha = alpha * alpha * (3f - 2f * alpha);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply(false, false);
        sGlareSprite = Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        return sGlareSprite;
    }
}
