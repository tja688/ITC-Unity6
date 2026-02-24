using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class CurvedLoopEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;
    public RectTransform Viewport;
    public RectTransform Track;
    public readonly List<RectTransform> LetterRects = new List<RectTransform>();
    public readonly List<float> LetterAdvances = new List<float>();
}

public static class CurvedLoopEffectViewBuilder
{
    public static CurvedLoopEffectView Build(RectTransform mountRoot, CurvedLoopEffectConfig config)
    {
        var view = new CurvedLoopEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("CurvedLoopEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var bg = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        bg.color = config.RootBackground;
        bg.raycastTarget = true;

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        var title = ReplicaUIFactoryV2.CreateText(
            "Title",
            view.Root,
            "CurvedLoop  |  drag to change direction",
            config.TitleFontSize,
            FontStyle.Bold,
            TextAnchor.UpperCenter,
            config.TitleColor);
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(900f, 52f);
        titleRect.anchoredPosition = new Vector2(0f, -40f);

        view.Viewport = ReplicaUIFactoryV2.CreatePanel("Viewport", view.Root, config.ViewportColor);
        view.Viewport.anchorMin = new Vector2(0.5f, 0.5f);
        view.Viewport.anchorMax = new Vector2(0.5f, 0.5f);
        view.Viewport.pivot = new Vector2(0.5f, 0.5f);
        view.Viewport.sizeDelta = config.ViewportSize;
        view.Viewport.anchoredPosition = config.ViewportPosition;
        ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(view.Viewport.gameObject);

        view.Content = view.Viewport;

        view.Track = ReplicaUIFactoryV2.CreateRect("Track", view.Viewport);
        view.Track.anchorMin = new Vector2(0.5f, 0.5f);
        view.Track.anchorMax = new Vector2(0.5f, 0.5f);
        view.Track.pivot = new Vector2(0.5f, 0.5f);
        view.Track.sizeDelta = config.TrackSize;
        view.Track.anchoredPosition = Vector2.zero;

        return view;
    }

    public static void RebuildLetters(CurvedLoopEffectView view, CurvedLoopEffectConfig config, string text)
    {
        if (view == null || view.Track == null)
        {
            return;
        }

        for (var i = 0; i < view.LetterRects.Count; i++)
        {
            if (view.LetterRects[i] != null)
            {
                Object.Destroy(view.LetterRects[i].gameObject);
            }
        }
        view.LetterRects.Clear();
        view.LetterAdvances.Clear();

        var prepared = PrepareText(text, Mathf.Max(1, config.RepeatCount));
        var spacing = Mathf.Max(1f, config.DefaultSpacing);

        for (var i = 0; i < prepared.Length; i++)
        {
            var letter = ReplicaUIFactoryV2.CreateText(
                $"Letter_{i}",
                view.Track,
                prepared[i].ToString(),
                config.LetterFontSize,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            var rect = (RectTransform)letter.transform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = config.LetterSize;

            view.LetterRects.Add(rect);
            view.LetterAdvances.Add(i * spacing);
        }
    }

    private static string PrepareText(string raw, int repeatCount)
    {
        var safe = string.IsNullOrWhiteSpace(raw) ? "CURVED LOOP" : raw;
        safe = safe.TrimEnd() + "\u00A0";

        var buffer = string.Empty;
        for (var i = 0; i < repeatCount; i++)
        {
            buffer += safe;
        }

        return buffer;
    }
}
