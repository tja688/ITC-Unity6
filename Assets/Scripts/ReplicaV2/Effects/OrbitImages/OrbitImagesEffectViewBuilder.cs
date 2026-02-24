using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class OrbitImagesEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Stage;
    public RectTransform OrbitRotation;
    public RectTransform ItemsRoot;
    public RectTransform CenterPanel;
    public Text CenterTitle;
    public readonly List<OrbitImagesItemView> Items = new List<OrbitImagesItemView>();
}

public sealed class OrbitImagesItemView
{
    public RectTransform Rect;
    public Image Image;
}

public static class OrbitImagesEffectViewBuilder
{
    public static OrbitImagesEffectView Build(RectTransform mountRoot, OrbitImagesEffectConfig config, OrbitImagesEffectModel model)
    {
        var view = new OrbitImagesEffectView();
        var safe = model != null ? model : OrbitImagesEffectModel.CreateDefault();

        view.Root = ReplicaUIFactoryV2.CreateRect("OrbitImagesEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        var backdrop = ReplicaUIFactoryV2.CreatePanel("Backdrop", view.Root, config.BackgroundColor);
        ReplicaUIFactoryV2.Stretch(backdrop);

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        view.Stage = ReplicaUIFactoryV2.CreateRect("Stage", backdrop);
        view.Stage.anchorMin = new Vector2(0.5f, 0.5f);
        view.Stage.anchorMax = new Vector2(0.5f, 0.5f);
        view.Stage.pivot = new Vector2(0.5f, 0.5f);
        view.Stage.sizeDelta = config.StageSize;
        view.Stage.anchoredPosition = Vector2.zero;

        view.OrbitRotation = ReplicaUIFactoryV2.CreateRect("OrbitRotation", view.Stage);
        ReplicaUIFactoryV2.Stretch(view.OrbitRotation);
        view.OrbitRotation.localRotation = Quaternion.Euler(0f, 0f, config.OrbitRotationZ);

        view.ItemsRoot = ReplicaUIFactoryV2.CreateRect("Items", view.OrbitRotation);
        ReplicaUIFactoryV2.Stretch(view.ItemsRoot);

        view.Items.Clear();
        var count = safe.Images != null ? safe.Images.Count : 0;
        if (count <= 0)
        {
            count = 8;
        }

        for (var i = 0; i < count; i++)
        {
            view.Items.Add(CreateItem(i, view.ItemsRoot, config));
        }

        view.CenterPanel = ReplicaUIFactoryV2.CreatePanel("Center", view.Stage, config.CenterPanelColor);
        view.CenterPanel.anchorMin = new Vector2(0.5f, 0.5f);
        view.CenterPanel.anchorMax = new Vector2(0.5f, 0.5f);
        view.CenterPanel.pivot = new Vector2(0.5f, 0.5f);
        view.CenterPanel.sizeDelta = config.CenterPanelSize;
        view.CenterPanel.anchoredPosition = Vector2.zero;

        view.CenterTitle = ReplicaUIFactoryV2.CreateText(
            "CenterTitle",
            view.CenterPanel,
            string.IsNullOrWhiteSpace(safe.CenterTitle) ? "Orbit Images" : safe.CenterTitle,
            Mathf.Max(10, config.CenterTitleFontSize),
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.CenterTitleColor);
        ReplicaUIFactoryV2.Stretch((RectTransform)view.CenterTitle.transform);

        ApplyModel(view, config, safe);
        return view;
    }

    public static void ApplyModel(OrbitImagesEffectView view, OrbitImagesEffectConfig config, OrbitImagesEffectModel model)
    {
        if (view == null || config == null || model == null)
        {
            return;
        }

        if (view.CenterTitle != null)
        {
            view.CenterTitle.text = string.IsNullOrWhiteSpace(model.CenterTitle) ? "Orbit Images" : model.CenterTitle;
        }

        var sprites = model.Images;
        var count = view.Items != null ? view.Items.Count : 0;
        for (var i = 0; i < count; i++)
        {
            var item = view.Items[i];
            if (item == null || item.Image == null)
            {
                continue;
            }

            var sprite = sprites != null && i < sprites.Count ? sprites[i] : null;
            item.Image.sprite = sprite;
            item.Image.type = Image.Type.Simple;
            item.Image.preserveAspect = true;
            item.Image.color = sprite != null ? Color.white : config.ItemFallbackColor;
        }
    }

    private static OrbitImagesItemView CreateItem(int index, RectTransform parent, OrbitImagesEffectConfig config)
    {
        var view = new OrbitImagesItemView();
        view.Rect = ReplicaUIFactoryV2.CreatePanel($"Item_{index}", parent, config.ItemFallbackColor);
        view.Rect.anchorMin = new Vector2(0.5f, 0.5f);
        view.Rect.anchorMax = new Vector2(0.5f, 0.5f);
        view.Rect.pivot = new Vector2(0.5f, 0.5f);
        view.Rect.sizeDelta = new Vector2(config.ItemSize, config.ItemSize);
        view.Rect.anchoredPosition = Vector2.zero;

        view.Image = view.Rect.GetComponent<Image>();
        view.Image.raycastTarget = false;

        return view;
    }
}
