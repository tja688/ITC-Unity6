using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public sealed class UGUIReactBitsSequenceShowcase_V2 : MonoBehaviour
{
    private enum EffectType
    {
        GlassIconsTilt,
        ElasticOverflowSlider,
        StepperProgressSlide
    }

    [Header("Startup")]
    [SerializeField] private ReplicaTweenBackend mStartBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private EffectType mStartEffect = EffectType.GlassIconsTilt;
    [SerializeField] private bool mAutoSequence = true;
    [SerializeField] [Min(4f)] private float mAutoSequenceInterval = 10f;

    private RectTransform mRootRect;
    private ReplicaTweenBackend mCurrentBackend;
    private EffectType mCurrentEffect;
    private float mTimer;

    private RectTransform mStageHost;
    private RectTransform mCurrentEffectRoot;

    private Image mBackendDotweenImage;
    private Image mBackendXTweenImage;
    private Image mEffectGlassIconsImage;
    private Image mEffectElasticSliderImage;
    private Image mEffectStepperImage;

    private Text mStatusText;

    private static readonly Color sPanel = new Color(0.07f, 0.10f, 0.18f, 0.92f);
    private static readonly Color sSidebar = new Color(0.08f, 0.12f, 0.22f, 0.95f);
    private static readonly Color sStage = new Color(0.06f, 0.08f, 0.14f, 0.96f);
    private static readonly Color sIdle = new Color(0.20f, 0.27f, 0.40f, 0.95f);
    private static readonly Color sActive = new Color(0.90f, 0.95f, 1f, 0.98f);
    private static readonly Color sTextIdle = new Color(0.88f, 0.92f, 1f, 1f);
    private static readonly Color sTextActive = new Color(0.08f, 0.12f, 0.20f, 1f);

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("UGUIReactBitsSequenceShowcase_V2 requires RectTransform.", this);
            return;
        }

        EnsureCanvas();
        EnsureEventSystem();
        BuildLayout();

        mCurrentBackend = mStartBackend;
        mCurrentEffect = mStartEffect;
        RefreshToggles();
        RebuildEffect();
    }

    private void Update()
    {
        if (!mAutoSequence)
        {
            return;
        }

        mTimer += Time.unscaledDeltaTime;
        if (mTimer < Mathf.Max(4f, mAutoSequenceInterval))
        {
            return;
        }

        mTimer = 0f;
        SelectEffect(NextEffect(mCurrentEffect));
    }

    private static EffectType NextEffect(EffectType current)
    {
        switch (current)
        {
            case EffectType.GlassIconsTilt: return EffectType.ElasticOverflowSlider;
            case EffectType.ElasticOverflowSlider: return EffectType.StepperProgressSlide;
            default: return EffectType.GlassIconsTilt;
        }
    }

    private void BuildLayout()
    {
        ReplicaUIFactoryV2.Stretch(mRootRect);
        var rootImage = ReplicaUIFactoryV2.EnsureComponent<Image>(gameObject);
        rootImage.color = new Color(0.03f, 0.04f, 0.08f, 1f);

        var shell = ReplicaUIFactoryV2.CreatePanel("V2Shell", mRootRect, sPanel);
        shell.anchorMin = new Vector2(0.03f, 0.05f);
        shell.anchorMax = new Vector2(0.97f, 0.95f);
        shell.offsetMin = Vector2.zero;
        shell.offsetMax = Vector2.zero;

        var sidebar = ReplicaUIFactoryV2.CreatePanel("Sidebar", shell, sSidebar);
        sidebar.anchorMin = new Vector2(0f, 0f);
        sidebar.anchorMax = new Vector2(0f, 1f);
        sidebar.offsetMin = new Vector2(16f, 16f);
        sidebar.offsetMax = new Vector2(420f, -16f);

        var stage = ReplicaUIFactoryV2.CreatePanel("Stage", shell, sStage);
        stage.anchorMin = new Vector2(0f, 0f);
        stage.anchorMax = new Vector2(1f, 1f);
        stage.offsetMin = new Vector2(438f, 16f);
        stage.offsetMax = new Vector2(-16f, -16f);

        BuildSidebar(sidebar);

        mStageHost = ReplicaUIFactoryV2.CreatePanel("StageHost", stage, new Color(0f, 0f, 0f, 0f));
        mStageHost.anchorMin = Vector2.zero;
        mStageHost.anchorMax = Vector2.one;
        mStageHost.offsetMin = new Vector2(20f, 20f);
        mStageHost.offsetMax = new Vector2(-20f, -20f);
        mStageHost.GetComponent<Image>().raycastTarget = false;
    }

    private void BuildSidebar(RectTransform sidebar)
    {
        var title = ReplicaUIFactoryV2.CreateText(
            "Title",
            sidebar,
            "ReactBits Sequence (V2)",
            30,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            Color.white);
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.offsetMin = new Vector2(18f, -56f);
        titleRect.offsetMax = new Vector2(-18f, -12f);

        var subtitle = ReplicaUIFactoryV2.CreateText(
            "Subtitle",
            sidebar,
            "Backend-injected ReplicaV2 effects",
            14,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            new Color(0.80f, 0.88f, 1f, 0.9f));
        var subtitleRect = (RectTransform)subtitle.transform;
        subtitleRect.anchorMin = new Vector2(0f, 1f);
        subtitleRect.anchorMax = new Vector2(1f, 1f);
        subtitleRect.pivot = new Vector2(0.5f, 1f);
        subtitleRect.offsetMin = new Vector2(18f, -88f);
        subtitleRect.offsetMax = new Vector2(-18f, -62f);

        var y = -120f;

        y = BuildSectionLabel(sidebar, y, "Backend");
        BuildTwoButtons(
            sidebar,
            ref y,
            "DOTween",
            () => SetBackend(ReplicaTweenBackend.DOTween),
            out mBackendDotweenImage,
            "XTween",
            () => SetBackend(ReplicaTweenBackend.XTween),
            out mBackendXTweenImage);

        y = BuildSectionLabel(sidebar, y, "Effect");
        BuildOneButton(sidebar, ref y, "GlassIconsTilt", () => SelectEffect(EffectType.GlassIconsTilt), out mEffectGlassIconsImage);
        BuildOneButton(sidebar, ref y, "ElasticOverflowSlider", () => SelectEffect(EffectType.ElasticOverflowSlider), out mEffectElasticSliderImage);
        BuildOneButton(sidebar, ref y, "StepperProgressSlide", () => SelectEffect(EffectType.StepperProgressSlide), out mEffectStepperImage);

        y = BuildSectionLabel(sidebar, y, "Auto Sequence");
        BuildTwoButtons(
            sidebar,
            ref y,
            "On",
            () => SetAutoSequence(true),
            out _,
            "Off",
            () => SetAutoSequence(false),
            out _);

        mStatusText = ReplicaUIFactoryV2.CreateText(
            "Status",
            sidebar,
            "-",
            14,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            new Color(0.82f, 0.90f, 1f, 0.94f));
        var statusRect = (RectTransform)mStatusText.transform;
        statusRect.anchorMin = new Vector2(0f, 0f);
        statusRect.anchorMax = new Vector2(1f, 0f);
        statusRect.pivot = new Vector2(0.5f, 0f);
        statusRect.offsetMin = new Vector2(18f, 20f);
        statusRect.offsetMax = new Vector2(-18f, 160f);
    }

    private float BuildSectionLabel(RectTransform parent, float y, string label)
    {
        var text = ReplicaUIFactoryV2.CreateText(
            $"Label_{label}",
            parent,
            label,
            18,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            new Color(0.93f, 0.96f, 1f, 1f));

        var rect = (RectTransform)text.transform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.offsetMin = new Vector2(18f, y - 26f);
        rect.offsetMax = new Vector2(-18f, y);
        return y - 36f;
    }

    private void BuildTwoButtons(
        RectTransform parent,
        ref float y,
        string leftText,
        UnityEngine.Events.UnityAction onLeft,
        out Image leftImage,
        string rightText,
        UnityEngine.Events.UnityAction onRight,
        out Image rightImage)
    {
        var row = ReplicaUIFactoryV2.CreateRect("Row", parent);
        row.anchorMin = new Vector2(0f, 1f);
        row.anchorMax = new Vector2(1f, 1f);
        row.pivot = new Vector2(0.5f, 1f);
        row.offsetMin = new Vector2(18f, y - 42f);
        row.offsetMax = new Vector2(-18f, y);

        var left = ReplicaUIFactoryV2.CreateButton("Left", row, sIdle);
        var leftRect = (RectTransform)left.transform;
        leftRect.anchorMin = new Vector2(0f, 0f);
        leftRect.anchorMax = new Vector2(0.5f, 1f);
        leftRect.offsetMin = Vector2.zero;
        leftRect.offsetMax = new Vector2(-6f, 0f);
        var leftLabel = ReplicaUIFactoryV2.CreateText("Label", left.transform, leftText, 15, FontStyle.Bold, TextAnchor.MiddleCenter, sTextIdle);
        ReplicaUIFactoryV2.Stretch((RectTransform)leftLabel.transform);
        left.onClick.AddListener(onLeft);
        leftImage = left.image;

        var right = ReplicaUIFactoryV2.CreateButton("Right", row, sIdle);
        var rightRect = (RectTransform)right.transform;
        rightRect.anchorMin = new Vector2(0.5f, 0f);
        rightRect.anchorMax = new Vector2(1f, 1f);
        rightRect.offsetMin = new Vector2(6f, 0f);
        rightRect.offsetMax = Vector2.zero;
        var rightLabel = ReplicaUIFactoryV2.CreateText("Label", right.transform, rightText, 15, FontStyle.Bold, TextAnchor.MiddleCenter, sTextIdle);
        ReplicaUIFactoryV2.Stretch((RectTransform)rightLabel.transform);
        right.onClick.AddListener(onRight);
        rightImage = right.image;

        y -= 50f;
    }

    private void BuildOneButton(
        RectTransform parent,
        ref float y,
        string text,
        UnityEngine.Events.UnityAction onClick,
        out Image buttonImage)
    {
        var button = ReplicaUIFactoryV2.CreateButton(text, parent, sIdle);
        var rect = (RectTransform)button.transform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.offsetMin = new Vector2(18f, y - 42f);
        rect.offsetMax = new Vector2(-18f, y);

        var label = ReplicaUIFactoryV2.CreateText("Label", button.transform, text, 15, FontStyle.Bold, TextAnchor.MiddleCenter, sTextIdle);
        ReplicaUIFactoryV2.Stretch((RectTransform)label.transform);
        button.onClick.AddListener(onClick);
        buttonImage = button.image;

        y -= 50f;
    }

    private void SetAutoSequence(bool value)
    {
        mAutoSequence = value;
        mTimer = 0f;
        SetStatus($"AutoSequence: {(mAutoSequence ? "On" : "Off")}");
    }

    private void SetBackend(ReplicaTweenBackend backend)
    {
        if (mCurrentBackend == backend)
        {
            return;
        }

        mCurrentBackend = backend;
        RefreshToggles();
        RebuildEffect();
    }

    private void SelectEffect(EffectType effect)
    {
        if (mCurrentEffect == effect)
        {
            return;
        }

        mCurrentEffect = effect;
        RefreshToggles();
        RebuildEffect();
    }

    private void RefreshToggles()
    {
        SetActiveButton(mBackendDotweenImage, mCurrentBackend == ReplicaTweenBackend.DOTween);
        SetActiveButton(mBackendXTweenImage, mCurrentBackend == ReplicaTweenBackend.XTween);

        SetActiveButton(mEffectGlassIconsImage, mCurrentEffect == EffectType.GlassIconsTilt);
        SetActiveButton(mEffectElasticSliderImage, mCurrentEffect == EffectType.ElasticOverflowSlider);
        SetActiveButton(mEffectStepperImage, mCurrentEffect == EffectType.StepperProgressSlide);

        SetStatus($"{mCurrentBackend} | {mCurrentEffect}");
    }

    private void SetActiveButton(Image image, bool active)
    {
        if (image == null)
        {
            return;
        }

        image.color = active ? sActive : sIdle;
        var text = image.GetComponentInChildren<Text>();
        if (text != null)
        {
            text.color = active ? sTextActive : sTextIdle;
        }
    }

    private void SetStatus(string message)
    {
        if (mStatusText != null)
        {
            mStatusText.text = message;
        }
    }

    private void RebuildEffect()
    {
        if (mCurrentEffectRoot != null)
        {
            Destroy(mCurrentEffectRoot.gameObject);
            mCurrentEffectRoot = null;
        }

        mCurrentEffectRoot = ReplicaUIFactoryV2.CreatePanel("EffectRoot", mStageHost, new Color(0f, 0f, 0f, 0f));
        ReplicaUIFactoryV2.Stretch(mCurrentEffectRoot);
        mCurrentEffectRoot.GetComponent<Image>().raycastTarget = false;

        switch (mCurrentEffect)
        {
            case EffectType.GlassIconsTilt:
            {
                var bridge = mCurrentEffectRoot.gameObject.AddComponent<GlassIconsTiltEffectHostBridge>();
                bridge.SetBackend(mCurrentBackend, true);
                break;
            }
            case EffectType.ElasticOverflowSlider:
            {
                var bridge = mCurrentEffectRoot.gameObject.AddComponent<ElasticOverflowSliderEffectHostBridge>();
                bridge.SetBackend(mCurrentBackend, true);
                break;
            }
            case EffectType.StepperProgressSlide:
            {
                var bridge = mCurrentEffectRoot.gameObject.AddComponent<StepperProgressSlideEffectHostBridge>();
                bridge.SetBackend(mCurrentBackend, true);
                break;
            }
        }
    }

    private void EnsureCanvas()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            return;
        }

        var canvasRect = ReplicaUIFactoryV2.CreateRect("ReplicaV2Canvas", null);
        var createdCanvas = ReplicaUIFactoryV2.EnsureComponent<Canvas>(canvasRect.gameObject);
        createdCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = ReplicaUIFactoryV2.EnsureComponent<CanvasScaler>(canvasRect.gameObject);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        ReplicaUIFactoryV2.EnsureComponent<GraphicRaycaster>(canvasRect.gameObject);
        mRootRect.SetParent(canvasRect, false);
    }

    private static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }

#if ENABLE_INPUT_SYSTEM
        _ = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
#else
        _ = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
#endif
    }
}
