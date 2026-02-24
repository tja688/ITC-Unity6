using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class CircularTextEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;
    public RectTransform RingHolder;
    public RectTransform Ring;
    public readonly List<RectTransform> Letters = new List<RectTransform>();
}

public static class CircularTextEffectViewBuilder
{
    public static CircularTextEffectView Build(RectTransform mountRoot, CircularTextEffectConfig config)
    {
        var view = new CircularTextEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("CircularTextEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var rootImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        rootImage.color = config.RootBackground;
        rootImage.raycastTarget = true;

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        var ambient = ReplicaUIFactoryV2.CreatePanel("Ambient", view.Root, config.AmbientColor);
        ambient.anchorMin = new Vector2(0.5f, 0.5f);
        ambient.anchorMax = new Vector2(0.5f, 0.5f);
        ambient.pivot = new Vector2(0.5f, 0.5f);
        ambient.sizeDelta = config.AmbientSize;
        ambient.anchoredPosition = config.AmbientPosition;
        ambient.GetComponent<Image>().raycastTarget = false;

        var title = ReplicaUIFactoryV2.CreateText(
            "Title",
            view.Root,
            "CircularText  |  hover to speed up",
            30,
            FontStyle.Bold,
            TextAnchor.UpperCenter,
            config.TitleColor);
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(720f, 52f);
        titleRect.anchoredPosition = new Vector2(0f, -42f);

        view.RingHolder = ReplicaUIFactoryV2.CreateRect("RingHolder", view.Root);
        view.RingHolder.anchorMin = new Vector2(0.5f, 0.5f);
        view.RingHolder.anchorMax = new Vector2(0.5f, 0.5f);
        view.RingHolder.pivot = new Vector2(0.5f, 0.5f);
        view.RingHolder.sizeDelta = config.RingHolderSize;
        view.RingHolder.anchoredPosition = config.RingHolderPosition;

        view.Content = view.RingHolder;

        var ringBack = ReplicaUIFactoryV2.CreatePanel("RingBack", view.RingHolder, config.RingBackColor);
        ReplicaUIFactoryV2.Stretch(ringBack);
        ringBack.GetComponent<Image>().raycastTarget = false;

        var centerCore = ReplicaUIFactoryV2.CreatePanel("Core", view.RingHolder, config.CoreColor);
        centerCore.anchorMin = new Vector2(0.5f, 0.5f);
        centerCore.anchorMax = new Vector2(0.5f, 0.5f);
        centerCore.pivot = new Vector2(0.5f, 0.5f);
        centerCore.sizeDelta = new Vector2(158f, 158f);
        centerCore.anchoredPosition = Vector2.zero;
        centerCore.GetComponent<Image>().raycastTarget = false;

        var coreLabel = ReplicaUIFactoryV2.CreateText(
            "CoreLabel",
            centerCore,
            "360",
            42,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.CoreLabelColor);
        ReplicaUIFactoryV2.Stretch((RectTransform)coreLabel.transform);

        view.Ring = ReplicaUIFactoryV2.CreateRect("Ring", view.RingHolder);
        ReplicaUIFactoryV2.Stretch(view.Ring);

        return view;
    }

    public static void RebuildLetters(CircularTextEffectView view, CircularTextEffectConfig config, string text)
    {
        if (view == null || view.Ring == null)
        {
            return;
        }

        for (var i = 0; i < view.Letters.Count; i++)
        {
            if (view.Letters[i] != null)
            {
                Object.Destroy(view.Letters[i].gameObject);
            }
        }
        view.Letters.Clear();

        var safe = string.IsNullOrWhiteSpace(text) ? "CIRCULAR" : text;
        var letters = safe.ToCharArray();
        if (letters.Length == 0)
        {
            letters = "CIRCULAR".ToCharArray();
        }

        var radius = Mathf.Max(1f, config.RingRadius);
        for (var i = 0; i < letters.Length; i++)
        {
            var ch = ReplicaUIFactoryV2.CreateText(
                $"Letter_{i}",
                view.Ring,
                letters[i].ToString(),
                config.LetterFontSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                config.LetterColor);
            var chRect = (RectTransform)ch.transform;
            chRect.anchorMin = new Vector2(0.5f, 0.5f);
            chRect.anchorMax = new Vector2(0.5f, 0.5f);
            chRect.pivot = new Vector2(0.5f, 0.5f);
            chRect.sizeDelta = config.LetterSize;

            var angle = (360f / letters.Length) * i * Mathf.Deg2Rad;
            var pos = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
            chRect.anchoredPosition = pos;
            chRect.localRotation = Quaternion.Euler(0f, 0f, -angle * Mathf.Rad2Deg);
            view.Letters.Add(chRect);
        }
    }
}
