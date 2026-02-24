using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public sealed class RotatingTextEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;
    public RectTransform Viewport;
}

public static class RotatingTextEffectViewBuilder
{
    public static RotatingTextEffectView Build(RectTransform mountRoot, RotatingTextEffectConfig config)
    {
        var view = new RotatingTextEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("RotatingTextEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        view.Content = ReplicaUIFactoryV2.CreateRect("Content", view.Root);
        view.Content.anchorMin = new Vector2(0.5f, 0.5f);
        view.Content.anchorMax = new Vector2(0.5f, 0.5f);
        view.Content.pivot = new Vector2(0.5f, 0.5f);
        view.Content.sizeDelta = config.ViewportSize;
        view.Content.anchoredPosition = Vector2.zero;

        view.Viewport = ReplicaUIFactoryV2.CreateRect("Viewport", view.Content);
        ReplicaUIFactoryV2.Stretch(view.Viewport);
        ReplicaUIFactoryV2.EnsureComponent<RectMask2D>(view.Viewport.gameObject);

        return view;
    }

    public static void ApplyModel(RotatingTextEffectView view, RotatingTextEffectConfig config)
    {
        if (view == null)
        {
            return;
        }

        view.Content.sizeDelta = config.ViewportSize;
    }

    public static RotatingTextContainer BuildContainer(
        RectTransform parent,
        RotatingTextEffectConfig config,
        string text,
        RotatingTextSplitBy splitBy)
    {
        var containerRoot = ReplicaUIFactoryV2.CreateRect("TextContainer", parent);
        containerRoot.anchorMin = new Vector2(0.5f, 0.5f);
        containerRoot.anchorMax = new Vector2(0.5f, 0.5f);
        containerRoot.pivot = new Vector2(0.5f, 0.5f);
        containerRoot.anchoredPosition = Vector2.zero;
        containerRoot.sizeDelta = Vector2.zero;

        var elements = new List<Text>();

        if (splitBy == RotatingTextSplitBy.Lines)
        {
            var vlg = ReplicaUIFactoryV2.EnsureComponent<VerticalLayoutGroup>(containerRoot.gameObject);
            vlg.childAlignment = TextAnchor.MiddleCenter;
            vlg.spacing = 0f;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = false;
            vlg.childForceExpandHeight = false;

            var fitter = ReplicaUIFactoryV2.EnsureComponent<ContentSizeFitter>(containerRoot.gameObject);
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var lines = (text ?? "").Split('\n');
            for (var i = 0; i < lines.Length; i++)
            {
                var t = CreateElement($"Line_{i}", containerRoot, config, lines[i]);
                elements.Add(t);
            }
        }
        else
        {
            var hlg = ReplicaUIFactoryV2.EnsureComponent<HorizontalLayoutGroup>(containerRoot.gameObject);
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.spacing = 0f;
            hlg.childControlWidth = true;
            hlg.childControlHeight = true;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = false;

            var fitter = ReplicaUIFactoryV2.EnsureComponent<ContentSizeFitter>(containerRoot.gameObject);
            fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            if (splitBy == RotatingTextSplitBy.Words)
            {
                var words = (text ?? "").Split(' ');
                for (var i = 0; i < words.Length; i++)
                {
                    var w = CreateElement($"Word_{i}", containerRoot, config, words[i]);
                    elements.Add(w);
                    if (i != words.Length - 1)
                    {
                        var space = CreateElement($"Space_{i}", containerRoot, config, " ");
                        elements.Add(space);
                    }
                }
            }
            else
            {
                var words = (text ?? "").Split(' ');
                var elementIndex = 0;
                for (var w = 0; w < words.Length; w++)
                {
                    var wordRoot = ReplicaUIFactoryV2.CreateRect($"Word_{w}", containerRoot);
                    var whlg = ReplicaUIFactoryV2.EnsureComponent<HorizontalLayoutGroup>(wordRoot.gameObject);
                    whlg.childAlignment = TextAnchor.MiddleCenter;
                    whlg.spacing = 0f;
                    whlg.childControlWidth = true;
                    whlg.childControlHeight = true;
                    whlg.childForceExpandWidth = false;
                    whlg.childForceExpandHeight = false;

                    var chars = SplitGraphemes(words[w]);
                    for (var c = 0; c < chars.Count; c++)
                    {
                        var t = CreateElement($"Char_{elementIndex++}", wordRoot, config, chars[c]);
                        elements.Add(t);
                    }

                    if (w != words.Length - 1)
                    {
                        var space = CreateElement($"Space_{elementIndex++}", containerRoot, config, " ");
                        elements.Add(space);
                    }
                }
            }
        }

        return new RotatingTextContainer
        {
            Root = containerRoot,
            Elements = elements
        };
    }

    private static Text CreateElement(string name, Transform parent, RotatingTextEffectConfig config, string content)
    {
        var t = ReplicaUIFactoryV2.CreateText(
            name,
            parent,
            content,
            config.FontSize,
            config.FontStyle,
            config.Alignment,
            config.TextColor);
        var rect = (RectTransform)t.transform;
        rect.sizeDelta = Vector2.zero;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        return t;
    }

    private static List<string> SplitGraphemes(string text)
    {
        var list = new List<string>();
        if (string.IsNullOrEmpty(text))
        {
            return list;
        }

        var enumerator = StringInfo.GetTextElementEnumerator(text);
        while (enumerator.MoveNext())
        {
            list.Add(enumerator.GetTextElement());
        }

        return list;
    }
}

public sealed class RotatingTextContainer
{
    public RectTransform Root;
    public List<Text> Elements;
}
