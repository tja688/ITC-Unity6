using UnityEngine;
using UnityEngine.UI;

public static class TargetCursorEffectViewBuilder
{
    public static TargetCursorEffectView Build(RectTransform parent, TargetCursorEffectConfig config)
    {
        var root = ReplicaUIFactoryV2.CreateRect("TargetCursor", parent);
        ReplicaUIFactoryV2.Stretch(root);


        var group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(root.gameObject);
        group.interactable = false;
        group.blocksRaycasts = false;

        var motionRoot = ReplicaUIFactoryV2.CreateRect("MotionRoot", root);
        motionRoot.anchorMin = new Vector2(0.5f, 0.5f);
        motionRoot.anchorMax = new Vector2(0.5f, 0.5f);
        motionRoot.pivot = new Vector2(0.5f, 0.5f);
        motionRoot.sizeDelta = Vector2.zero;

        var dot = ReplicaUIFactoryV2.CreatePanel("Dot", motionRoot, config.CursorColor);
        dot.sizeDelta = new Vector2(config.DotSize, config.DotSize);
        var dotImage = dot.GetComponent<Image>();
        dotImage.raycastTarget = false;

        var corners = new RectTransform[4];
        var hLines = new RectTransform[4];
        var vLines = new RectTransform[4];
        var hImages = new Image[4];
        var vImages = new Image[4];

        for (int i = 0; i < 4; i++)
        {
            var corner = ReplicaUIFactoryV2.CreateRect($"Corner_{i}", motionRoot);
            corner.sizeDelta = new Vector2(config.CornerSize, config.CornerSize);
            // Must center pivot so interpolation around targets with BorderOffset works properly
            corner.pivot = new Vector2(0.5f, 0.5f);


            var hLine = ReplicaUIFactoryV2.CreatePanel("HLine", corner, config.CursorColor);
            var vLine = ReplicaUIFactoryV2.CreatePanel("VLine", corner, config.CursorColor);

            hLine.GetComponent<Image>().raycastTarget = false;
            vLine.GetComponent<Image>().raycastTarget = false;

            // Setup pivot/anchor based on TL(0), TR(1), BR(2), BL(3)
            var p = Vector2.zero;
            if (i == 0) p = new Vector2(0, 1);
            else if (i == 1) p = new Vector2(1, 1);
            else if (i == 2) p = new Vector2(1, 0);
            else if (i == 3) p = new Vector2(0, 0);

            hLine.pivot = p;
            hLine.anchorMin = p;
            hLine.anchorMax = p;
            hLine.anchoredPosition = Vector2.zero;
            hLine.sizeDelta = new Vector2(config.CornerSize, config.BorderWidth);

            vLine.pivot = p;
            vLine.anchorMin = p;
            vLine.anchorMax = p;
            vLine.anchoredPosition = Vector2.zero;
            vLine.sizeDelta = new Vector2(config.BorderWidth, config.CornerSize);

            corners[i] = corner;
            hLines[i] = hLine;
            vLines[i] = vLine;
            hImages[i] = hLine.GetComponent<Image>();
            vImages[i] = vLine.GetComponent<Image>();
        }

        return new TargetCursorEffectView
        {
            Root = root,
            Group = group,
            MotionRoot = motionRoot,
            Dot = dot,
            DotImage = dotImage,
            Corners = corners,
            HLines = hLines,
            VLines = vLines,
            HImages = hImages,
            VImages = vImages
        };
    }
}
