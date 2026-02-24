using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class ElasticOverflowSliderInputRelay : UIBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerDownHandler,
    IDragHandler,
    IPointerUpHandler
{
    public bool IsHovering { get; private set; }
    public bool IsDragging { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        var image = ReplicaUIFactoryV2.EnsureComponent<Image>(gameObject);
        image.color = new Color(1f, 1f, 1f, 0.001f);
        image.raycastTarget = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        IsDragging = false;
        IsHovering = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!IsDragging)
        {
            IsHovering = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsDragging = true;
        IsHovering = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsDragging)
        {
            IsHovering = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsDragging = false;
    }
}

public sealed class ElasticOverflowSliderEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;

    public Text HintText;
    public Text ValueText;

    public RectTransform TrackRect;
    public RectTransform TrackWrapper;
    public RectTransform FillRect;
    public RectTransform KnobRect;
    public RectTransform LeftIconRect;
    public RectTransform RightIconRect;

    public Image TrackBackground;
    public Image FillImage;
    public Image KnobImage;

    public ElasticOverflowSliderInputRelay Input;
}

public static class ElasticOverflowSliderEffectViewBuilder
{
    public static ElasticOverflowSliderEffectView Build(RectTransform mountRoot, ElasticOverflowSliderEffectConfig config)
    {
        var view = new ElasticOverflowSliderEffectView();

        view.Root = ReplicaUIFactoryV2.CreateRect("ElasticOverflowSliderEffect", mountRoot);
        ReplicaUIFactoryV2.Stretch(view.Root);

        view.Group = ReplicaUIFactoryV2.EnsureComponent<CanvasGroup>(view.Root.gameObject);
        view.Group.alpha = 1f;
        view.Group.blocksRaycasts = true;
        view.Group.interactable = true;

        var backdrop = ReplicaUIFactoryV2.CreatePanel("Backdrop", view.Root, config.BackdropColor);
        ReplicaUIFactoryV2.Stretch(backdrop);
        backdrop.GetComponent<Image>().raycastTarget = false;

        view.HintText = ReplicaUIFactoryV2.CreateText(
            "Hint",
            backdrop,
            "ElasticSlider  |  Drag and over-pull both sides",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.HintColor);
        var hintRect = (RectTransform)view.HintText.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(980f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -42f);

        view.Content = ReplicaUIFactoryV2.CreatePanel("Frame", backdrop, config.FrameColor);
        view.Content.anchorMin = new Vector2(0.5f, 0.5f);
        view.Content.anchorMax = new Vector2(0.5f, 0.5f);
        view.Content.pivot = new Vector2(0.5f, 0.5f);
        view.Content.sizeDelta = config.FrameSize;
        view.Content.anchoredPosition = new Vector2(0f, -16f);

        var frameShadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(view.Content.gameObject);
        frameShadow.effectColor = new Color(0f, 0f, 0f, 0.32f);
        frameShadow.effectDistance = new Vector2(0f, -8f);

        var row = ReplicaUIFactoryV2.CreateRect("SliderRow", view.Content);
        row.anchorMin = new Vector2(0.5f, 0.5f);
        row.anchorMax = new Vector2(0.5f, 0.5f);
        row.pivot = new Vector2(0.5f, 0.5f);
        row.sizeDelta = config.RowSize;
        row.anchoredPosition = new Vector2(0f, 12f);

        view.LeftIconRect = ReplicaUIFactoryV2.CreateRect("LeftIcon", row);
        view.LeftIconRect.anchorMin = new Vector2(0f, 0.5f);
        view.LeftIconRect.anchorMax = new Vector2(0f, 0.5f);
        view.LeftIconRect.pivot = new Vector2(0.5f, 0.5f);
        view.LeftIconRect.sizeDelta = new Vector2(48f, 48f);
        view.LeftIconRect.anchoredPosition = new Vector2(config.IconBaseOffset, 0f);
        var leftIconText = ReplicaUIFactoryV2.CreateText(
            "Glyph",
            view.LeftIconRect,
            "−",
            36,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.IconColor);
        ReplicaUIFactoryV2.Stretch((RectTransform)leftIconText.transform);

        var sliderContainer = ReplicaUIFactoryV2.CreateRect("SliderContainer", row);
        sliderContainer.anchorMin = new Vector2(0f, 0.5f);
        sliderContainer.anchorMax = new Vector2(1f, 0.5f);
        sliderContainer.pivot = new Vector2(0.5f, 0.5f);
        sliderContainer.offsetMin = new Vector2(72f, -40f);
        sliderContainer.offsetMax = new Vector2(-72f, 40f);

        view.TrackRect = ReplicaUIFactoryV2.CreateRect("TrackRoot", sliderContainer);
        view.TrackRect.anchorMin = new Vector2(0f, 0.5f);
        view.TrackRect.anchorMax = new Vector2(1f, 0.5f);
        view.TrackRect.pivot = new Vector2(0.5f, 0.5f);
        view.TrackRect.sizeDelta = new Vector2(0f, config.TrackHeightIdle);
        view.TrackRect.anchoredPosition = Vector2.zero;

        view.Input = ReplicaUIFactoryV2.EnsureComponent<ElasticOverflowSliderInputRelay>(view.TrackRect.gameObject);

        view.TrackWrapper = ReplicaUIFactoryV2.CreateRect("TrackWrapper", view.TrackRect);
        ReplicaUIFactoryV2.Stretch(view.TrackWrapper);

        var track = ReplicaUIFactoryV2.CreatePanel("Track", view.TrackWrapper, config.TrackColor);
        ReplicaUIFactoryV2.Stretch(track);
        view.TrackBackground = track.GetComponent<Image>();
        view.TrackBackground.raycastTarget = false;

        view.FillRect = ReplicaUIFactoryV2.CreatePanel("Fill", track, config.FillColor);
        view.FillRect.anchorMin = new Vector2(0f, 0f);
        view.FillRect.anchorMax = new Vector2(0.5f, 1f);
        view.FillRect.offsetMin = Vector2.zero;
        view.FillRect.offsetMax = Vector2.zero;
        view.FillImage = view.FillRect.GetComponent<Image>();
        view.FillImage.raycastTarget = false;

        view.KnobRect = ReplicaUIFactoryV2.CreatePanel("Knob", view.TrackRect, Color.white);
        view.KnobRect.anchorMin = new Vector2(0.5f, 0.5f);
        view.KnobRect.anchorMax = new Vector2(0.5f, 0.5f);
        view.KnobRect.pivot = new Vector2(0.5f, 0.5f);
        view.KnobRect.sizeDelta = new Vector2(20f, 20f);
        view.KnobRect.anchoredPosition = Vector2.zero;
        view.KnobImage = view.KnobRect.GetComponent<Image>();
        view.KnobImage.color = config.KnobColor;
        view.KnobImage.raycastTarget = false;
        var knobOutline = ReplicaUIFactoryV2.EnsureComponent<Outline>(view.KnobRect.gameObject);
        knobOutline.effectColor = new Color(0f, 0f, 0f, 0.25f);
        knobOutline.effectDistance = new Vector2(1f, -1f);

        view.RightIconRect = ReplicaUIFactoryV2.CreateRect("RightIcon", row);
        view.RightIconRect.anchorMin = new Vector2(1f, 0.5f);
        view.RightIconRect.anchorMax = new Vector2(1f, 0.5f);
        view.RightIconRect.pivot = new Vector2(0.5f, 0.5f);
        view.RightIconRect.sizeDelta = new Vector2(48f, 48f);
        view.RightIconRect.anchoredPosition = new Vector2(-config.IconBaseOffset, 0f);
        var rightIconText = ReplicaUIFactoryV2.CreateText(
            "Glyph",
            view.RightIconRect,
            "+",
            36,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.IconColor);
        ReplicaUIFactoryV2.Stretch((RectTransform)rightIconText.transform);

        view.ValueText = ReplicaUIFactoryV2.CreateText(
            "Value",
            view.Content,
            "50",
            26,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            config.ValueColor);
        var valueRect = (RectTransform)view.ValueText.transform;
        valueRect.anchorMin = new Vector2(0.5f, 0f);
        valueRect.anchorMax = new Vector2(0.5f, 0f);
        valueRect.pivot = new Vector2(0.5f, 0f);
        valueRect.sizeDelta = new Vector2(120f, 42f);
        valueRect.anchoredPosition = new Vector2(0f, 34f);

        return view;
    }
}
