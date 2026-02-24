using UnityEngine;
using UnityEngine.UI;

public sealed class TextCursorEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform MotionRoot;
    public RectTransform Overlay;
}

public static class TextCursorEffectViewBuilder
{
    public static TextCursorEffectView Build(RectTransform mountRoot, TextCursorEffectConfig config)
    {
        var view = new TextCursorEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("TextCursorEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = false;
        view.Group.interactable = false;

        view.MotionRoot = ReplicaUIFactoryV2.CreateRect("MotionRoot", view.Root);
        view.MotionRoot.anchorMin = new Vector2(0.5f, 0.5f);
        view.MotionRoot.anchorMax = new Vector2(0.5f, 0.5f);
        view.MotionRoot.pivot = new Vector2(0.5f, 0.5f);
        view.MotionRoot.sizeDelta = Vector2.zero;
        view.MotionRoot.anchoredPosition = Vector2.zero;

        view.Overlay = ReplicaUIFactoryV2.CreateRect("Overlay", view.MotionRoot);
        ReplicaUIFactoryV2.Stretch(view.Overlay);

        return view;
    }
}
