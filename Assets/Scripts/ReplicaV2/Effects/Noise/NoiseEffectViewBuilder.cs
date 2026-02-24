using UnityEngine;
using UnityEngine.UI;

public sealed class NoiseEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RawImage NoiseImage;
}

public static class NoiseEffectViewBuilder
{
    public static NoiseEffectView Build(RectTransform mountRoot, NoiseEffectConfig config)
    {
        var view = new NoiseEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("NoiseEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);
        view.Root.anchoredPosition = Vector2.zero;

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = false;
        view.Group.interactable = false;

        var imageRect = ReplicaUIFactoryV2.CreateRect("Noise", view.Root);
        ReplicaUIFactoryV2.Stretch(imageRect);
        view.NoiseImage = ReplicaUIFactoryV2.EnsureComponent<RawImage>(imageRect.gameObject);
        view.NoiseImage.raycastTarget = false;

        ApplyConfig(view, config);
        return view;
    }

    public static void ApplyConfig(NoiseEffectView view, NoiseEffectConfig config)
    {
        if (view == null || view.NoiseImage == null || config == null)
        {
            return;
        }

        var tint = config.Tint;
        tint.a = 1f;
        view.NoiseImage.color = tint;
        view.NoiseImage.uvRect = new Rect(0f, 0f, Mathf.Max(0.01f, config.PatternScale.x), Mathf.Max(0.01f, config.PatternScale.y));
    }
}
