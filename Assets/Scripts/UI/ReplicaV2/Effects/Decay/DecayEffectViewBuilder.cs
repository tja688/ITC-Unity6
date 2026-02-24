using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class DecayEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform CardRoot;
    public Image CardImage;
    public Text TitleText;
    public Text SubtitleText;
    public Text MarkerText;
    public readonly List<RectTransform> NoiseStrips = new List<RectTransform>();
    public readonly List<float> NoiseBaseY = new List<float>();
    public readonly List<Image> NoiseImages = new List<Image>();
}

public static class DecayEffectViewBuilder
{
    public static DecayEffectView Build(RectTransform mountRoot, DecayEffectConfig config)
    {
        if (config != null && config.Prefab != null)
        {
            var go = UnityEngine.Object.Instantiate(config.Prefab, mountRoot);
            go.name = config.Prefab.name;
            if (go.TryGetComponent<DecayEffectViewLinker>(out var linker))
            {
                return linker.ToView();
            }
        }

        var view = new DecayEffectView();
        if (config == null) config = ScriptableObject.CreateInstance<DecayEffectConfig>();

        view.Root = ReplicaUIFactoryV2.CreateRect("DecayEffect", mountRoot);
        view.Root.anchorMin = new Vector2(0.5f, 0.5f);
        view.Root.anchorMax = new Vector2(0.5f, 0.5f);
        view.Root.pivot = new Vector2(0.5f, 0.5f);
        view.Root.sizeDelta = config.CardSize;
        view.Root.anchoredPosition = Vector2.zero;

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        view.CardRoot = ReplicaUIFactoryV2.CreateRect("DecayCard", view.Root);
        view.CardRoot.anchorMin = new Vector2(0.5f, 0.5f);
        view.CardRoot.anchorMax = new Vector2(0.5f, 0.5f);
        view.CardRoot.pivot = new Vector2(0.5f, 0.5f);
        view.CardRoot.sizeDelta = config.CardSize;
        view.CardRoot.anchoredPosition = Vector2.zero;

        var cardMask = ReplicaUIFactoryV2.CreatePanel("CardMask", view.CardRoot, new Color(0.32f, 0.36f, 0.44f, 1f));
        ReplicaUIFactoryV2.Stretch(cardMask);
        ReplicaUIFactoryV2.EnsureComponent<Mask>(cardMask.gameObject).showMaskGraphic = true;

        view.CardImage = ReplicaUIFactoryV2.CreatePanel("Photo", cardMask, config.BasePhotoColor).GetComponent<Image>();
        var photoRect = view.CardImage.rectTransform;
        ReplicaUIFactoryV2.Stretch(photoRect);

        var gradientA = ReplicaUIFactoryV2.CreatePanel("GradientA", photoRect, config.GradientAColor);
        ReplicaUIFactoryV2.Stretch(gradientA);

        var gradientB = ReplicaUIFactoryV2.CreatePanel("GradientB", photoRect, config.GradientBColor);
        ReplicaUIFactoryV2.Stretch(gradientB);
        gradientB.offsetMin = new Vector2(0f, -130f);
        gradientB.offsetMax = new Vector2(0f, 200f);

        var noiseRoot = ReplicaUIFactoryV2.CreateRect("Noise", photoRect);
        ReplicaUIFactoryV2.Stretch(noiseRoot);

        view.NoiseStrips.Clear();
        view.NoiseBaseY.Clear();
        view.NoiseImages.Clear();

        var stripCount = Mathf.Max(8, config.StripCount);
        var stripHeight = config.CardSize.y / stripCount;

        for (var i = 0; i < stripCount; i++)
        {
            var strip = ReplicaUIFactoryV2.CreatePanel($"Strip_{i}", noiseRoot, new Color(1f, 1f, 1f, 0.03f));
            strip.anchorMin = new Vector2(0.5f, 0f);
            strip.anchorMax = new Vector2(0.5f, 0f);
            strip.pivot = new Vector2(0.5f, 0f);
            strip.sizeDelta = new Vector2(config.CardSize.x + 40f, stripHeight + 2f);
            var y = i * stripHeight;
            strip.anchoredPosition = new Vector2(0f, y);

            view.NoiseStrips.Add(strip);
            view.NoiseBaseY.Add(y);
            view.NoiseImages.Add(strip.GetComponent<Image>());
        }

        var cardOutline = ReplicaUIFactoryV2.EnsureComponent<Outline>(view.CardRoot.gameObject);
        cardOutline.effectColor = new Color(1f, 1f, 1f, 0.46f);
        cardOutline.effectDistance = new Vector2(1.8f, -1.8f);

        view.TitleText = ReplicaUIFactoryV2.CreateText(
            "CardTitle",
            view.CardRoot,
            "NEXT GEN",
            88,
            FontStyle.Bold,
            TextAnchor.LowerLeft,
            config.TitleColor);
        var titleRect = (RectTransform)view.TitleText.transform;
        titleRect.anchorMin = new Vector2(0f, 0f);
        titleRect.anchorMax = new Vector2(0f, 0f);
        titleRect.pivot = new Vector2(0f, 0f);
        titleRect.sizeDelta = new Vector2(360f, 260f);
        titleRect.anchoredPosition = new Vector2(36f, 60f);

        view.SubtitleText = ReplicaUIFactoryV2.CreateText(
            "CardSubtitle",
            view.CardRoot,
            "Pointer-driven distortion",
            24,
            FontStyle.Normal,
            TextAnchor.LowerLeft,
            config.SubtitleColor);
        var subtitleRect = (RectTransform)view.SubtitleText.transform;
        subtitleRect.anchorMin = new Vector2(0f, 0f);
        subtitleRect.anchorMax = new Vector2(1f, 0f);
        subtitleRect.pivot = new Vector2(0f, 0f);
        subtitleRect.sizeDelta = new Vector2(-64f, 52f);
        subtitleRect.anchoredPosition = new Vector2(36f, 20f);

        view.MarkerText = ReplicaUIFactoryV2.CreateText(
            "Marker",
            view.CardRoot,
            "DECAY",
            18,
            FontStyle.Bold,
            TextAnchor.UpperRight,
            new Color(1f, 1f, 1f, 0.7f));
        var markerRect = (RectTransform)view.MarkerText.transform;
        markerRect.anchorMin = new Vector2(1f, 1f);
        markerRect.anchorMax = new Vector2(1f, 1f);
        markerRect.pivot = new Vector2(1f, 1f);
        markerRect.sizeDelta = new Vector2(220f, 44f);
        markerRect.anchoredPosition = new Vector2(-24f, -20f);

        return view;
    }

    public static void Link(GameObject go, DecayEffectView view)
    {
        var linker = ReplicaUIFactoryV2.EnsureComponent<DecayEffectViewLinker>(go);
        linker.Root = view.Root;
        linker.Group = view.Group;
        linker.CardRoot = view.CardRoot;
        linker.CardImage = view.CardImage;
        linker.TitleText = view.TitleText;
        linker.SubtitleText = view.SubtitleText;
        linker.MarkerText = view.MarkerText;


        linker.NoiseStrips = new List<RectTransform>(view.NoiseStrips);
        linker.NoiseBaseY = new List<float>(view.NoiseBaseY);
        linker.NoiseImages = new List<Image>(view.NoiseImages);
    }
}
