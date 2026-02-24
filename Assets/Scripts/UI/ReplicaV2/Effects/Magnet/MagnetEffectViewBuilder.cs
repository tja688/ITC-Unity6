using UnityEngine;
using UnityEngine.UI;

public sealed class MagnetEffectView
{
    public RectTransform Root;
    public RectTransform Surface;
    public RectTransform InnerContainer;
    public Image SurfaceImage;
    public Outline SurfaceOutline;
    public Image ButtonImage;
    public Shadow ButtonShadow;
    public Text ButtonLabel;
    public Text HintText;
}

public static class MagnetEffectViewBuilder
{
    public static MagnetEffectView Build(RectTransform mountRoot, MagnetEffectConfig config)
    {
        if (config != null && config.Prefab != null)
        {
            var go = UnityEngine.Object.Instantiate(config.Prefab, mountRoot);
            go.name = config.Prefab.name;
            if (go.TryGetComponent<MagnetViewLinker>(out var linker))
            {
                var v = linker.ToView();
                // Ensure Root is set if linker didn't have it (though it should)
                if (v.Root == null) v.Root = go.GetComponent<RectTransform>();
                return v;
            }
            Debug.LogWarning("Prefab found but MagnetViewLinker component is missing.", go);
        }

        var view = new MagnetEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("MagnetEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        // Required for raycasting or checking bounds if needed
        var rootImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        rootImage.color = new Color(1f, 1f, 1f, 0f);
        rootImage.raycastTarget = false;

        view.Surface = ReplicaUIFactoryV2.CreatePanel("Surface", view.Root, config != null ? config.BackgroundColor : new Color(0.10f, 0.14f, 0.24f, 0.96f));
        view.Surface.anchorMin = new Vector2(0.5f, 0.5f);
        view.Surface.anchorMax = new Vector2(0.5f, 0.5f);
        view.Surface.pivot = new Vector2(0.5f, 0.5f);
        view.Surface.sizeDelta = config != null ? config.AreaSize : new Vector2(560f, 300f);
        view.Surface.anchoredPosition = Vector2.zero;
        view.SurfaceImage = view.Surface.GetComponent<Image>();
        view.SurfaceImage.raycastTarget = false;

        view.SurfaceOutline = ReplicaUIFactoryV2.EnsureComponent<Outline>(view.Surface.gameObject);
        view.SurfaceOutline.effectColor = config != null ? config.BackgroundOutlineColor : new Color(1f, 1f, 1f, 0.16f);
        view.SurfaceOutline.effectDistance = new Vector2(1f, -1f);

        view.HintText = ReplicaUIFactoryV2.CreateText(
            "Hint",
            view.Surface,
            "Move pointer near the button",
            20,
            FontStyle.Bold,
            TextAnchor.UpperCenter,
            config != null ? config.HintTextColor : new Color(0.80f, 0.88f, 1f, 0.92f));
        var hintRect = (RectTransform)view.HintText.transform;
        hintRect.anchorMin = new Vector2(0f, 1f);
        hintRect.anchorMax = new Vector2(1f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.offsetMin = new Vector2(20f, -64f);
        hintRect.offsetMax = new Vector2(-20f, -24f);

        view.InnerContainer = ReplicaUIFactoryV2.CreateRect("InnerContainer", view.Surface);
        ReplicaUIFactoryV2.Stretch(view.InnerContainer);

        var buttonRoot = ReplicaUIFactoryV2.CreatePanel("Button", view.InnerContainer, config != null ? config.ButtonColor : new Color(0.95f, 0.97f, 1f, 0.96f));
        buttonRoot.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRoot.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRoot.pivot = new Vector2(0.5f, 0.5f);
        buttonRoot.sizeDelta = config != null ? config.ButtonSize : new Vector2(320f, 96f);
        buttonRoot.anchoredPosition = new Vector2(0f, -14f);

        view.ButtonImage = buttonRoot.GetComponent<Image>();
        view.ButtonImage.raycastTarget = false;
        view.ButtonShadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(buttonRoot.gameObject);
        view.ButtonShadow.effectColor = new Color(0f, 0f, 0f, 0.28f);
        view.ButtonShadow.effectDistance = new Vector2(0f, -6f);

        view.ButtonLabel = ReplicaUIFactoryV2.CreateText(
            "Label",
            buttonRoot,
            "Hover Me",
            32,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config != null ? config.ButtonTextColor : new Color(0.08f, 0.12f, 0.22f, 1f));
        ReplicaUIFactoryV2.Stretch((RectTransform)view.ButtonLabel.transform);

        return view;
    }

    public static void Link(GameObject go, MagnetEffectView view)
    {
        var linker = ReplicaUIFactoryV2.EnsureComponent<MagnetViewLinker>(go);
        linker.Root = view.Root;
        linker.Surface = view.Surface;
        linker.InnerContainer = view.InnerContainer;
        linker.SurfaceImage = view.SurfaceImage;
        linker.SurfaceOutline = view.SurfaceOutline;
        linker.ButtonImage = view.ButtonImage;
        linker.ButtonShadow = view.ButtonShadow;
        linker.ButtonLabel = view.ButtonLabel;
        linker.HintText = view.HintText;
    }
}
