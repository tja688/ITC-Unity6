using UnityEngine;
using UnityEngine.UI;

public sealed class GlitchTextEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;
    public RectTransform Container;
    public Text HintText;
    public RectTransform MainRect;
    public RectTransform AfterMask;
    public RectTransform BeforeMask;
    public RectTransform AfterTextRect;
    public RectTransform BeforeTextRect;
    public Text AfterText;
    public Text BeforeText;
}

public static class GlitchTextEffectViewBuilder
{
    public static GlitchTextEffectView Build(RectTransform mountRoot, GlitchTextEffectConfig config)
    {
        var view = new GlitchTextEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("GlitchTextEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var bg = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        bg.color = config.RootBackground;
        bg.raycastTarget = true;

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        view.Content = ReplicaUIFactoryV2.CreatePanel("Backdrop", view.Root, config.BackdropColor);
        view.Content.anchorMin = new Vector2(0.5f, 0.5f);
        view.Content.anchorMax = new Vector2(0.5f, 0.5f);
        view.Content.pivot = new Vector2(0.5f, 0.5f);
        view.Content.sizeDelta = config.BackdropSize;
        view.Content.anchoredPosition = config.BackdropPosition;

        view.HintText = ReplicaUIFactoryV2.CreateText(
            "Hint",
            view.Root,
            "GlitchText",
            config.TitleFontSize,
            FontStyle.Bold,
            TextAnchor.UpperCenter,
            config.HintColor);
        var hintRect = (RectTransform)view.HintText.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(900f, 52f);
        hintRect.anchoredPosition = new Vector2(0f, -40f);

        view.Container = ReplicaUIFactoryV2.CreateRect("GlitchContainer", view.Content);
        view.Container.anchorMin = new Vector2(0.5f, 0.5f);
        view.Container.anchorMax = new Vector2(0.5f, 0.5f);
        view.Container.pivot = new Vector2(0.5f, 0.5f);
        view.Container.sizeDelta = config.ContainerSize;
        view.Container.anchoredPosition = Vector2.zero;

        var mainText = ReplicaUIFactoryV2.CreateText(
            "MainText",
            view.Container,
            "GLITCH",
            config.MainFontSize,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.TextColor);
        view.MainRect = (RectTransform)mainText.transform;
        ReplicaUIFactoryV2.Stretch(view.MainRect);

        view.AfterMask = ReplicaUIFactoryV2.CreateRect("AfterMask", view.Container);
        ReplicaUIFactoryV2.Stretch(view.AfterMask);
        ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(view.AfterMask.gameObject);

        view.AfterText = ReplicaUIFactoryV2.CreateText(
            "AfterText",
            view.AfterMask,
            "GLITCH",
            config.MainFontSize,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.TextColor);
        view.AfterTextRect = (RectTransform)view.AfterText.transform;
        ReplicaUIFactoryV2.Stretch(view.AfterTextRect);

        view.BeforeMask = ReplicaUIFactoryV2.CreateRect("BeforeMask", view.Container);
        ReplicaUIFactoryV2.Stretch(view.BeforeMask);
        ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(view.BeforeMask.gameObject);

        view.BeforeText = ReplicaUIFactoryV2.CreateText(
            "BeforeText",
            view.BeforeMask,
            "GLITCH",
            config.MainFontSize,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.TextColor);
        view.BeforeTextRect = (RectTransform)view.BeforeText.transform;
        ReplicaUIFactoryV2.Stretch(view.BeforeTextRect);

        return view;
    }

    public static void ApplyModel(GlitchTextEffectView view, GlitchTextEffectConfig config, GlitchTextEffectModel model)
    {
        var text = string.IsNullOrWhiteSpace(model.Text) ? "GLITCH" : model.Text;

        if (view.HintText != null)
        {
            view.HintText.text = model.EnableOnHover ? "GlitchText  |  hover to trigger RGB slices" : "GlitchText  |  always on";
        }

        if (view.MainRect != null && view.MainRect.TryGetComponent(out Text main))
        {
            main.text = text;
        }

        if (view.AfterText != null)
        {
            view.AfterText.text = text;
        }

        if (view.BeforeText != null)
        {
            view.BeforeText.text = text;
        }

        if (view.AfterText != null)
        {
            if (model.EnableShadows)
            {
                var shadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(view.AfterText.gameObject);
                shadow.effectColor = config.AfterShadowColor;
                shadow.effectDistance = config.AfterShadowOffset;
            }
            else if (view.AfterText.gameObject.TryGetComponent(out Shadow shadow))
            {
                Object.Destroy(shadow);
            }
        }

        if (view.BeforeText != null)
        {
            if (model.EnableShadows)
            {
                var shadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(view.BeforeText.gameObject);
                shadow.effectColor = config.BeforeShadowColor;
                shadow.effectDistance = config.BeforeShadowOffset;
            }
            else if (view.BeforeText.gameObject.TryGetComponent(out Shadow shadow))
            {
                Object.Destroy(shadow);
            }
        }
    }

    public static void SetBand(RectTransform maskRect, float height, float centerY)
    {
        if (maskRect == null)
        {
            return;
        }

        maskRect.anchorMin = new Vector2(0f, 0.5f);
        maskRect.anchorMax = new Vector2(1f, 0.5f);
        maskRect.pivot = new Vector2(0.5f, 0.5f);
        maskRect.sizeDelta = new Vector2(0f, Mathf.Max(0f, height));
        maskRect.anchoredPosition = new Vector2(0f, centerY);
    }
}
