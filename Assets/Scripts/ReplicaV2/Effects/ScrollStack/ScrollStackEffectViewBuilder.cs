using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ScrollStackEffectView
{
    public RectTransform Root;
    public ScrollRect Scroller;
    public RectTransform Content;
    public RectTransform EndSpacer;
    public List<ScrollStackItemView> Items = new List<ScrollStackItemView>();
}

public sealed class ScrollStackItemView
{
    public RectTransform Root;
    public Image Background;
    public Text Title;
    public Text Description;
    public Image Image;
    public CanvasGroup Group;
}

public static class ScrollStackEffectViewBuilder
{
    public static ScrollStackEffectView Build(RectTransform mountRoot, ScrollStackEffectConfig config)
    {
        var view = new ScrollStackEffectView();

        // Root
        view.Root = ReplicaUIFactoryV2.CreateRect("ScrollStackEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        // ScrollRect
        var scrollerGo = new GameObject("Scroller", typeof(RectTransform), typeof(ScrollRect), typeof(Image), typeof(RectMask2D));
        view.Scroller = scrollerGo.GetComponent<ScrollRect>();
        var scrollerRect = (RectTransform)view.Scroller.transform;
        scrollerRect.SetParent(view.Root, false);
        ReplicaUIFactoryV2.Stretch(scrollerRect);

        // Setup viewport

        view.Scroller.viewport = scrollerRect;

        // Hide scrollbar background

        view.Scroller.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        view.Scroller.GetComponent<Image>().raycastTarget = true;

        // Content
        view.Content = ReplicaUIFactoryV2.CreateRect("Content", scrollerRect);
        view.Content.anchorMin = new Vector2(0, 1);
        view.Content.anchorMax = new Vector2(1, 1);
        view.Content.pivot = new Vector2(0.5f, 1);
        view.Content.anchoredPosition = Vector2.zero;
        view.Content.sizeDelta = new Vector2(0, 0);


        view.Scroller.content = view.Content;
        view.Scroller.horizontal = false;
        view.Scroller.vertical = true;
        view.Scroller.movementType = ScrollRect.MovementType.Elastic;
        view.Scroller.inertia = true;
        view.Scroller.scrollSensitivity = config != null ? Mathf.Max(1f, config.wheelSensitivity) : 55f;
        view.Scroller.elasticity = config != null ? Mathf.Clamp(config.elasticity, 0.01f, 0.5f) : 0.16f;
        view.Scroller.decelerationRate = config != null ? Mathf.Clamp(config.decelerationRate, 0.001f, 0.3f) : 0.06f;

        // End Spacer
        view.EndSpacer = ReplicaUIFactoryV2.CreateRect("EndSpacer", view.Content);
        view.EndSpacer.anchorMin = new Vector2(0, 0);
        view.EndSpacer.anchorMax = new Vector2(1, 0);
        view.EndSpacer.pivot = new Vector2(0.5f, 0);
        view.EndSpacer.sizeDelta = new Vector2(0, 400f); // More space for release

        return view;
    }

    public static ScrollStackItemView BuildItem(RectTransform parent, ScrollStackEffectConfig config, int index)
    {
        var itemView = new ScrollStackItemView();


        itemView.Root = ReplicaUIFactoryV2.CreateRect($"Item_{index}", parent);
        itemView.Root.anchorMin = new Vector2(0.5f, 1);
        itemView.Root.anchorMax = new Vector2(0.5f, 1);
        itemView.Root.pivot = new Vector2(0.5f, 1);
        itemView.Root.sizeDelta = new Vector2(600f, 400f);

        itemView.Background = ReplicaUIFactoryV2.EnsureComponent<Image>(itemView.Root.gameObject);
        itemView.Background.color = Color.white;


        itemView.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(itemView.Root.gameObject);

        // Image Placeholder (New)
        var imgRect = ReplicaUIFactoryV2.CreateRect("Image", itemView.Root);
        itemView.Image = ReplicaUIFactoryV2.EnsureComponent<Image>(imgRect.gameObject);
        imgRect.anchorMin = new Vector2(0, 0);
        imgRect.anchorMax = new Vector2(0.4f, 1);
        imgRect.offsetMin = new Vector2(10, 10);
        imgRect.offsetMax = new Vector2(-10, -10);
        itemView.Image.color = new Color(1, 1, 1, 0.5f);

        // Title
        itemView.Title = ReplicaUIFactoryV2.CreateText("Title", itemView.Root, "Item Title", 32, FontStyle.Bold, TextAnchor.MiddleLeft, Color.black);
        var titleRect = (RectTransform)itemView.Title.transform;
        titleRect.anchorMin = new Vector2(0.45f, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0, 1);
        titleRect.offsetMin = new Vector2(0, -100);
        titleRect.offsetMax = new Vector2(-20, -20);

        // Description
        itemView.Description = ReplicaUIFactoryV2.CreateText("Desc", itemView.Root, "Description", 20, FontStyle.Normal, TextAnchor.UpperLeft, new Color(0.2f, 0.2f, 0.2f));
        var descRect = (RectTransform)itemView.Description.transform;
        descRect.anchorMin = new Vector2(0.45f, 0);
        descRect.anchorMax = new Vector2(1, 1);
        descRect.pivot = new Vector2(0, 0.5f);
        descRect.offsetMin = new Vector2(0, 20);
        descRect.offsetMax = new Vector2(-20, -120);

        return itemView;
    }
}
