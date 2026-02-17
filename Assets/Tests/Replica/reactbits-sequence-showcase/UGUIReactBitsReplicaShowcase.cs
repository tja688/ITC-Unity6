using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UGUIReactBitsReplicaShowcase : MonoBehaviour
{
    [Header("Playback")]
    [SerializeField] private bool mAutoSequence = true;
    [SerializeField] private float mAutoSequenceInterval = 10f;

    private RectTransform mRootRect;
    private readonly List<RectTransform> mEffectPanels = new();
    private readonly List<Image> mTabImages = new();
    private readonly List<Text> mTabTexts = new();

    private int mCurrentIndex;
    private float mSequenceTimer;

    private static readonly string[] sEffectNames =
    {
        "Dock",
        "ElasticSlider",
        "GlassIcons",
        "ReflectiveCard",
        "SpotlightCard",
        "BounceCards",
        "BubbleMenu",
        "CardSwap",
        "Carousel",
        "Counter",
        "DecayCard",
        "StaggeredMenu",
        "Stepper",
        "TiltedCard",
        "CircularText",
        "CountUp",
        "CurvedLoop",
        "GlitchText"
    };

    private static readonly Type[] sEffectTypes =
    {
        typeof(UGUIDockReplica),
        typeof(UGUIElasticSliderReplica),
        typeof(UGUIGlassIconsReplica),
        typeof(UGUIReflectiveCardReplica),
        typeof(UGUISpotlightCardReplica),
        typeof(UGUIBounceCardsReplica),
        typeof(UGUIBubbleMenuReplica),
        typeof(UGUICardSwapReplica),
        typeof(UGUICarouselReplica),
        typeof(UGUICounterReplica),
        typeof(UGUIDecayCardReplica),
        typeof(UGUIStaggeredMenuReplica),
        typeof(UGUIStepperReplica),
        typeof(UGUITiltedCardReplica),
        typeof(UGUICircularTextReplica),
        typeof(UGUICountUpReplica),
        typeof(UGUICurvedLoopReplica),
        typeof(UGUIGlitchTextReplica)
    };

    private static readonly Color sTabActive = new(0.93f, 0.95f, 1f, 0.96f);
    private static readonly Color sTabIdle = new(0.24f, 0.30f, 0.44f, 0.92f);
    private static readonly Color sTabTextActive = new(0.08f, 0.13f, 0.22f, 1f);
    private static readonly Color sTabTextIdle = new(0.86f, 0.90f, 1f, 1f);
    private static readonly KeyCode[] sDirectSelectKeys =
    {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
        KeyCode.Alpha9,
        KeyCode.Alpha0
    };

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("UGUIReactBitsReplicaShowcase requires RectTransform.", this);
            return;
        }

        EnsureCanvas();
        EnsureEventSystem();
        BuildLayout();
        Application.runInBackground = true;
        Time.timeScale = 1f;
        SwitchTo(0, false);
    }

    private void Update()
    {
        if (Time.timeScale <= 0f)
        {
            Time.timeScale = 1f;
        }

        HandleKeyboardSwitch();

        if (!mAutoSequence || mEffectPanels.Count <= 1)
        {
            return;
        }

        mSequenceTimer += Time.unscaledDeltaTime;
        if (mSequenceTimer < Mathf.Max(4f, mAutoSequenceInterval))
        {
            return;
        }

        mSequenceTimer = 0f;
        SwitchTo((mCurrentIndex + 1) % mEffectPanels.Count, true);
    }

    private void HandleKeyboardSwitch()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SwitchTo((mCurrentIndex + 1) % mEffectPanels.Count, true);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SwitchTo((mCurrentIndex - 1 + mEffectPanels.Count) % mEffectPanels.Count, true);
        }
        else
        {
            var count = Mathf.Min(mEffectPanels.Count, sDirectSelectKeys.Length);
            for (var i = 0; i < count; i++)
            {
                if (Input.GetKeyDown(sDirectSelectKeys[i]))
                {
                    SwitchTo(i, true);
                    break;
                }
            }
        }
    }

    private void BuildLayout()
    {
        mRootRect.anchorMin = Vector2.zero;
        mRootRect.anchorMax = Vector2.one;
        mRootRect.offsetMin = Vector2.zero;
        mRootRect.offsetMax = Vector2.zero;
        mRootRect.pivot = new Vector2(0.5f, 0.5f);

        var rootImage = UGUIReplicaUIFactory.EnsureComponent<Image>(gameObject);
        rootImage.color = new Color(0.03f, 0.04f, 0.08f, 1f);
        rootImage.raycastTarget = true;

        CreateBackgroundPlate("AmbientPlateA", new Color(0.18f, 0.24f, 0.40f, 0.58f), new Vector2(-260f, 210f), 26f, 1500f, 620f);
        CreateBackgroundPlate("AmbientPlateB", new Color(0.08f, 0.42f, 0.62f, 0.28f), new Vector2(260f, -120f), -18f, 1360f, 560f);
        CreateBackgroundPlate("AmbientPlateC", new Color(0.46f, 0.23f, 0.63f, 0.22f), new Vector2(-180f, -250f), 9f, 1240f, 480f);

        var frame = UGUIReplicaUIFactory.CreatePanel("Frame", mRootRect, new Color(0.07f, 0.10f, 0.18f, 0.88f));
        frame.anchorMin = new Vector2(0.04f, 0.06f);
        frame.anchorMax = new Vector2(0.96f, 0.94f);
        frame.offsetMin = Vector2.zero;
        frame.offsetMax = Vector2.zero;

        var frameShadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(frame.gameObject);
        frameShadow.effectColor = new Color(0f, 0f, 0f, 0.35f);
        frameShadow.effectDistance = new Vector2(0f, -9f);

        var header = UGUIReplicaUIFactory.CreatePanel("Header", frame, new Color(0.10f, 0.14f, 0.24f, 0.95f));
        header.anchorMin = new Vector2(0f, 0.74f);
        header.anchorMax = new Vector2(1f, 1f);
        header.offsetMin = Vector2.zero;
        header.offsetMax = Vector2.zero;

        var title = UGUIReplicaUIFactory.CreateText(
            "Title",
            header,
            "React Bits Motion Replica  |  Unity UGUI",
            32,
            FontStyle.Bold,
            TextAnchor.MiddleLeft,
            new Color(0.95f, 0.97f, 1f, 1f));
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(0f, 1f);
        titleRect.pivot = new Vector2(0f, 1f);
        titleRect.sizeDelta = new Vector2(920f, 52f);
        titleRect.anchoredPosition = new Vector2(30f, -22f);

        var helper = UGUIReplicaUIFactory.CreateText(
            "Helper",
            header,
            "Press Left/Right or click tab to switch",
            18,
            FontStyle.Bold,
            TextAnchor.UpperRight,
            new Color(0.84f, 0.90f, 1f, 0.86f));
        var helperRect = (RectTransform)helper.transform;
        helperRect.anchorMin = new Vector2(1f, 1f);
        helperRect.anchorMax = new Vector2(1f, 1f);
        helperRect.pivot = new Vector2(1f, 1f);
        helperRect.sizeDelta = new Vector2(460f, 34f);
        helperRect.anchoredPosition = new Vector2(-30f, -26f);

        var tabRow = UGUIReplicaUIFactory.CreateRect("Tabs", header);
        tabRow.anchorMin = new Vector2(0f, 0f);
        tabRow.anchorMax = new Vector2(1f, 0f);
        tabRow.pivot = new Vector2(0.5f, 0f);
        tabRow.offsetMin = new Vector2(24f, 10f);
        tabRow.offsetMax = new Vector2(-24f, 116f);

        var layout = UGUIReplicaUIFactory.EnsureComponent<GridLayoutGroup>(tabRow.gameObject);
        layout.startAxis = GridLayoutGroup.Axis.Horizontal;
        layout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = 6;
        layout.cellSize = new Vector2(214f, 44f);
        layout.spacing = new Vector2(8f, 8f);
        layout.childAlignment = TextAnchor.MiddleCenter;

        for (var i = 0; i < sEffectNames.Length; i++)
        {
            var index = i;
            var tabButton = UGUIReplicaUIFactory.CreateButton($"Tab_{sEffectNames[i]}", tabRow, sTabIdle);
            var tabRect = (RectTransform)tabButton.transform;
            tabRect.sizeDelta = layout.cellSize;

            var le = UGUIReplicaUIFactory.EnsureComponent<LayoutElement>(tabButton.gameObject);
            le.preferredWidth = layout.cellSize.x;
            le.preferredHeight = layout.cellSize.y;

            var label = UGUIReplicaUIFactory.CreateText(
                "Label",
                tabButton.transform,
                sEffectNames[i],
                18,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                sTabTextIdle);
            var labelRect = (RectTransform)label.transform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            tabButton.onClick.AddListener(() => SwitchTo(index, true));
            mTabImages.Add(tabButton.image);
            mTabTexts.Add(label);
        }

        var content = UGUIReplicaUIFactory.CreatePanel("Content", frame, new Color(0.06f, 0.08f, 0.15f, 0.82f));
        content.anchorMin = new Vector2(0f, 0f);
        content.anchorMax = new Vector2(1f, 0.74f);
        content.offsetMin = new Vector2(24f, 22f);
        content.offsetMax = new Vector2(-24f, -22f);

        for (var i = 0; i < sEffectNames.Length; i++)
        {
            var panel = UGUIReplicaUIFactory.CreateRect($"{sEffectNames[i]}Panel", content);
            panel.anchorMin = Vector2.zero;
            panel.anchorMax = Vector2.one;
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;

            _ = panel.gameObject.AddComponent(sEffectTypes[i]);
            panel.gameObject.SetActive(false);
            mEffectPanels.Add(panel);
        }
    }

    private void CreateBackgroundPlate(string name, Color color, Vector2 pos, float rotZ, float width, float height)
    {
        var plate = UGUIReplicaUIFactory.CreatePanel(name, mRootRect, color);
        plate.anchorMin = new Vector2(0.5f, 0.5f);
        plate.anchorMax = new Vector2(0.5f, 0.5f);
        plate.pivot = new Vector2(0.5f, 0.5f);
        plate.sizeDelta = new Vector2(width, height);
        plate.anchoredPosition = pos;
        plate.localRotation = Quaternion.Euler(0f, 0f, rotZ);
        plate.SetSiblingIndex(0);
    }

    private void SwitchTo(int index, bool resetTimer)
    {
        if (mEffectPanels.Count == 0)
        {
            return;
        }

        mCurrentIndex = Mathf.Clamp(index, 0, mEffectPanels.Count - 1);
        for (var i = 0; i < mEffectPanels.Count; i++)
        {
            var active = i == mCurrentIndex;
            mEffectPanels[i].gameObject.SetActive(active);
            mTabImages[i].color = active ? sTabActive : sTabIdle;
            mTabTexts[i].color = active ? sTabTextActive : sTabTextIdle;
        }

        if (resetTimer)
        {
            mSequenceTimer = 0f;
        }
    }

    private void EnsureCanvas()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            return;
        }

        var canvasRect = UGUIReplicaUIFactory.CreateRect("ReplicaCanvas", null);
        var createdCanvas = UGUIReplicaUIFactory.EnsureComponent<Canvas>(canvasRect.gameObject);
        createdCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = UGUIReplicaUIFactory.EnsureComponent<CanvasScaler>(canvasRect.gameObject);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        _ = UGUIReplicaUIFactory.EnsureComponent<GraphicRaycaster>(canvasRect.gameObject);
        mRootRect.SetParent(canvasRect, false);
    }

    private static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null)
        {
            return;
        }

        _ = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }
}
