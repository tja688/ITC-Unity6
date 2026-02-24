using UnityEngine;
using UnityEngine.UI;

public sealed class AnimatedContentEffectView
{
    public RectTransform Root;
    public RectTransform Container;
    public Image ContainerImage;
    public CanvasGroup ContainerGroup;
    public Text LabelText;
}

public static class AnimatedContentEffectViewBuilder
{
    public static AnimatedContentEffectView Build(RectTransform mountRoot, AnimatedContentEffectConfig config)
    {
        if (config != null && config.Prefab != null)
        {
            var go = UnityEngine.Object.Instantiate(config.Prefab, mountRoot);
            go.name = config.Prefab.name;
            if (go.TryGetComponent<AnimatedContentViewLinker>(out var linker))
            {
                return linker.ToView();
            }
        }

        var view = new AnimatedContentEffectView();
        if (config == null) config = ScriptableObject.CreateInstance<AnimatedContentEffectConfig>();

        view.Root = ReplicaUIFactoryV2.CreateRect("AnimatedContentEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var rootImage = ReplicaUIFactoryV2.EnsureComponent<Image>(view.Root.gameObject);
        rootImage.color = new Color(1f, 1f, 1f, 0f);
        rootImage.raycastTarget = false;

        view.Container = ReplicaUIFactoryV2.CreatePanel("Container", view.Root, config.BackgroundColor);
        view.Container.anchorMin = new Vector2(0.5f, 0.5f);
        view.Container.anchorMax = new Vector2(0.5f, 0.5f);
        view.Container.pivot = new Vector2(0.5f, 0.5f);
        view.Container.sizeDelta = config.ContainerSize;
        view.Container.anchoredPosition = Vector2.zero;

        view.ContainerImage = view.Container.GetComponent<Image>();
        view.ContainerImage.raycastTarget = false;

        view.ContainerGroup = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Container.gameObject);
        view.ContainerGroup.alpha = 1f;
        view.ContainerGroup.blocksRaycasts = true;
        view.ContainerGroup.interactable = true;

        view.LabelText = ReplicaUIFactoryV2.CreateText(
            "Label",
            view.Container,
            "AnimatedContent",
            Mathf.Max(1, config.FontSize),
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.TextColor);
        ReplicaUIFactoryV2.Stretch((RectTransform)view.LabelText.transform);

        return view;
    }

    public static void Link(GameObject go, AnimatedContentEffectView view)
    {
        var linker = ReplicaUIFactoryV2.EnsureComponent<AnimatedContentViewLinker>(go);
        linker.Root = view.Root;
        linker.Container = view.Container;
        linker.ContainerImage = view.ContainerImage;
        linker.ContainerGroup = view.ContainerGroup;
        linker.LabelText = view.LabelText;
    }
}
