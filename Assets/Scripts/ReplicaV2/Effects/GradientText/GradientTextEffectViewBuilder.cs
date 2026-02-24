using UnityEngine;
using UnityEngine.UI;

public sealed class GradientTextEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;

    public RectTransform Frame;
    public RectTransform BorderGradientClip;
    public RectTransform BorderGradientRect;
    public Image BorderGradientImage;

    public RectTransform InnerBackground;
    public Image InnerBackgroundImage;

    public RectTransform TextMaskRect;
    public Mask TextMask;
    public Text MaskText;

    public RectTransform TextGradientRect;
    public Image TextGradientImage;
}

public static class GradientTextEffectViewBuilder
{
    public static GradientTextEffectView Build(RectTransform mountRoot, GradientTextEffectConfig config)
    {
        var view = new GradientTextEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("GradientTextEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        view.Content = ReplicaUIFactoryV2.CreateRect("Content", view.Root);
        view.Content.anchorMin = new Vector2(0.5f, 0.5f);
        view.Content.anchorMax = new Vector2(0.5f, 0.5f);
        view.Content.pivot = new Vector2(0.5f, 0.5f);
        view.Content.sizeDelta = config.PanelSize;
        view.Content.anchoredPosition = Vector2.zero;

        view.Frame = ReplicaUIFactoryV2.CreateRect("Frame", view.Content);
        ReplicaUIFactoryV2.Stretch(view.Frame);
        ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(view.Frame.gameObject);

        view.BorderGradientClip = ReplicaUIFactoryV2.CreateRect("BorderGradientClip", view.Frame);
        ReplicaUIFactoryV2.Stretch(view.BorderGradientClip);
        ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(view.BorderGradientClip.gameObject);

        view.BorderGradientRect = ReplicaUIFactoryV2.CreateRect("BorderGradient", view.BorderGradientClip);
        ReplicaUIFactoryV2.Stretch(view.BorderGradientRect);
        view.BorderGradientImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.BorderGradientRect.gameObject);
        view.BorderGradientImage.raycastTarget = false;
        view.BorderGradientImage.color = Color.white;

        view.InnerBackground = ReplicaUIFactoryV2.CreatePanel("InnerBackground", view.Frame, config.PanelBackground);
        view.InnerBackgroundImage = view.InnerBackground.GetComponent<Image>();
        view.InnerBackgroundImage.raycastTarget = false;

        view.TextMaskRect = ReplicaUIFactoryV2.CreateRect("TextMask", view.InnerBackground);
        ReplicaUIFactoryV2.Stretch(view.TextMaskRect);

        view.MaskText = ReplicaUIFactoryV2.CreateText(
            "MaskText",
            view.TextMaskRect,
            "GradientText",
            config.FontSize,
            config.FontStyle,
            config.Alignment,
            Color.white);
        view.MaskText.horizontalOverflow = HorizontalWrapMode.Overflow;
        view.MaskText.verticalOverflow = VerticalWrapMode.Overflow;
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

    public static void ApplyModel(GradientTextEffectView view, GradientTextEffectConfig config, GradientTextEffectModel model)
    {
        if (view == null)
        {
            return;
        }

        view.Content.sizeDelta = config.PanelSize;

        if (view.MaskText != null)
        {
            view.MaskText.text = string.IsNullOrWhiteSpace(model.Text) ? "GradientText" : model.Text;
            view.MaskText.fontSize = config.FontSize;
            view.MaskText.fontStyle = config.FontStyle;
            view.MaskText.alignment = config.Alignment;
        }

        var showBorder = model.ShowBorder;
        if (view.BorderGradientClip != null)
        {
            view.BorderGradientClip.gameObject.SetActive(showBorder);
        }

        if (view.InnerBackground != null)
        {
            if (showBorder)
            {
                var t = Mathf.Max(0f, config.BorderThickness);
                view.InnerBackground.offsetMin = new Vector2(t, t);
                view.InnerBackground.offsetMax = new Vector2(-t, -t);
            }
            else
            {
                view.InnerBackground.offsetMin = Vector2.zero;
                view.InnerBackground.offsetMax = Vector2.zero;
            }
        }
    }
}
