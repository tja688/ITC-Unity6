using UnityEngine;
using UnityEngine.UI;

public sealed class StarBorderEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Container;
    public RectMask2D Mask;
    public RectTransform GlowTop;
    public Image GlowTopImage;
    public RectTransform GlowBottom;
    public Image GlowBottomImage;
    public RectTransform Inner;
    public Image InnerImage;
    public Outline InnerOutline;
    public Text LabelText;
}

public static class StarBorderEffectViewBuilder
{
    private static Sprite sRadialSprite;

    public static StarBorderEffectView Build(RectTransform mountRoot, StarBorderEffectConfig config)
    {
        var view = new StarBorderEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("StarBorderEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var rootImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        rootImage.color = new Color(1f, 1f, 1f, 0f);
        rootImage.raycastTarget = false;

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        view.Container = ReplicaUIFactoryV2.CreateRect("Container", view.Root);
        view.Container.anchorMin = new Vector2(0.5f, 0.5f);
        view.Container.anchorMax = new Vector2(0.5f, 0.5f);
        view.Container.pivot = new Vector2(0.5f, 0.5f);
        view.Container.sizeDelta = config.ContainerSize;
        view.Container.anchoredPosition = Vector2.zero;

        view.Mask = ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(view.Container.gameObject);

        view.GlowBottom = ReplicaUIFactoryV2.CreateRect("GlowBottom", view.Container);
        view.GlowBottom.anchorMin = new Vector2(0.5f, 0f);
        view.GlowBottom.anchorMax = new Vector2(0.5f, 0f);
        view.GlowBottom.pivot = new Vector2(0.5f, 0f);
        view.GlowBottom.sizeDelta = new Vector2(config.ContainerSize.x * 3f, config.ContainerSize.y * 0.5f);
        view.GlowBottom.anchoredPosition = new Vector2(config.ContainerSize.x * 1.25f, -12f);

        view.GlowBottomImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.GlowBottom.gameObject);
        view.GlowBottomImage.sprite = GetOrCreateRadialSprite();
        view.GlowBottomImage.color = new Color(config.GlowColor.r, config.GlowColor.g, config.GlowColor.b, Mathf.Clamp01(config.GlowOpacity));
        view.GlowBottomImage.raycastTarget = false;

        view.GlowTop = ReplicaUIFactoryV2.CreateRect("GlowTop", view.Container);
        view.GlowTop.anchorMin = new Vector2(0.5f, 1f);
        view.GlowTop.anchorMax = new Vector2(0.5f, 1f);
        view.GlowTop.pivot = new Vector2(0.5f, 1f);
        view.GlowTop.sizeDelta = new Vector2(config.ContainerSize.x * 3f, config.ContainerSize.y * 0.5f);
        view.GlowTop.anchoredPosition = new Vector2(-config.ContainerSize.x * 1.25f, 12f);

        view.GlowTopImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.GlowTop.gameObject);
        view.GlowTopImage.sprite = GetOrCreateRadialSprite();
        view.GlowTopImage.color = new Color(config.GlowColor.r, config.GlowColor.g, config.GlowColor.b, Mathf.Clamp01(config.GlowOpacity));
        view.GlowTopImage.raycastTarget = false;

        view.Inner = ReplicaUIFactoryV2.CreatePanel("Inner", view.Container, config.InnerBackgroundColor);
        view.Inner.anchorMin = new Vector2(0.5f, 0.5f);
        view.Inner.anchorMax = new Vector2(0.5f, 0.5f);
        view.Inner.pivot = new Vector2(0.5f, 0.5f);
        view.Inner.sizeDelta = new Vector2(
            Mathf.Max(1f, config.ContainerSize.x - (config.BorderThickness * 2f)),
            Mathf.Max(1f, config.ContainerSize.y - (config.BorderThickness * 2f)));
        view.Inner.anchoredPosition = Vector2.zero;

        view.InnerImage = view.Inner.GetComponent<Image>();
        view.InnerImage.raycastTarget = false;

        view.InnerOutline = ReplicaUIFactoryV2.EnsureComponent<Outline>(view.Inner.gameObject);
        view.InnerOutline.effectColor = config.InnerBorderColor;
        view.InnerOutline.effectDistance = new Vector2(1f, -1f);

        view.LabelText = ReplicaUIFactoryV2.CreateText(
            "Label",
            view.Inner,
            "StarBorder",
            Mathf.Max(1, config.FontSize),
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.TextColor);
        var labelRect = (RectTransform)view.LabelText.transform;
        ReplicaUIFactoryV2.Stretch(labelRect);
        labelRect.offsetMin = new Vector2(config.Padding.x, config.Padding.y);
        labelRect.offsetMax = new Vector2(-config.Padding.x, -config.Padding.y);

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
            name = "ReplicaRadialSprite_StarBorder_V2"
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
                var alpha = Mathf.Clamp01(1f - (distance / 0.10f));
                alpha = alpha * alpha * (3f - 2f * alpha);
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply(false, false);
        sRadialSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return sRadialSprite;
    }
}
