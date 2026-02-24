using UnityEngine;
using UnityEngine.UI;

public sealed class ShinyTextEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;

    public RectTransform TextMaskRect;
    public Mask TextMask;
    public Text MaskText;

    public RectTransform TextGradientRect;
    public Image TextGradientImage;
}

public static class ShinyTextEffectViewBuilder
{
    public static ShinyTextEffectView Build(RectTransform mountRoot, ShinyTextEffectConfig config)
    {
        var view = new ShinyTextEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("ShinyTextEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        view.Content = ReplicaUIFactoryV2.CreateRect("Content", view.Root);
        view.Content.anchorMin = new Vector2(0.5f, 0.5f);
        view.Content.anchorMax = new Vector2(0.5f, 0.5f);
        view.Content.pivot = new Vector2(0.5f, 0.5f);
        view.Content.sizeDelta = config.ContainerSize;
        view.Content.anchoredPosition = Vector2.zero;

        view.TextMaskRect = ReplicaUIFactoryV2.CreateRect("TextMask", view.Content);
        ReplicaUIFactoryV2.Stretch(view.TextMaskRect);

        view.MaskText = ReplicaUIFactoryV2.CreateText(
            "MaskText",
            view.TextMaskRect,
            "ShinyText",
            config.FontSize,
            config.FontStyle,
            config.Alignment,
            Color.white);
        var maskTextRect = (RectTransform)view.MaskText.transform;
        ReplicaUIFactoryV2.Stretch(maskTextRect);

        view.TextMask = ReplicaUIFactoryV2.EnsureComponent<Mask>(view.MaskText.gameObject);
        view.TextMask.showMaskGraphic = false;

        view.TextGradientRect = ReplicaUIFactoryV2.CreateRect("TextGradient", view.MaskText.transform);
        ReplicaUIFactoryV2.Stretch(view.TextGradientRect);
        view.TextGradientImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.TextGradientRect.gameObject);
        view.TextGradientImage.raycastTarget = false;
        view.TextGradientImage.color = Color.white;

        return view;
    }

    public static void ApplyModel(ShinyTextEffectView view, ShinyTextEffectConfig config, ShinyTextEffectModel model)
    {
        if (view == null)
        {
            return;
        }

        view.Content.sizeDelta = config.ContainerSize;

        if (view.MaskText != null)
        {
            view.MaskText.text = string.IsNullOrWhiteSpace(model.Text) ? "ShinyText" : model.Text;
            view.MaskText.fontSize = config.FontSize;
            view.MaskText.fontStyle = config.FontStyle;
            view.MaskText.alignment = config.Alignment;
        }
    }
}
