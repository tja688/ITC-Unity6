using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public sealed class UGUIReactBitsReplicaShowcase_V2 : MonoBehaviour
{
    private class ShowcaseItem
    {
        public string Group;
        public string Name;
        public Type BridgeType;
        public Image ToggleImage;
    }

    private enum HostMode
    {
        Fullscreen,
        Windowed
    }

    [Header("Startup")]
    [SerializeField] private ReplicaTweenBackend mStartBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private string mStartGroup = "Cards";
    [SerializeField] private string mStartEffect = "Stack";
    [SerializeField] private HostMode mStartHostMode = HostMode.Fullscreen;
    [SerializeField] [Range(4, 8)] private int mStartStackCount = 5;

    private RectTransform mRootRect;
    private ReplicaTweenBackend mCurrentBackend;
    private string mCurrentGroup;
    private string mCurrentEffect;
    private HostMode mCurrentHostMode;
    private int mCurrentStackCount;

    private RectTransform mStageFullHost;
    private RectTransform mStageWindowHost;
    private RectTransform mCurrentEffectRoot;
    private IReplicaShowcaseBridge mCurrentBridgeInstance;

    private List<ShowcaseItem> mAllItems = new List<ShowcaseItem>();
    private List<string> mGroups = new List<string>();
    private Dictionary<string, Image> mGroupToggles = new Dictionary<string, Image>();

    private Text mStatusText;
    private Image mBackendDotweenImage;
    private Image mBackendXTweenImage;
    private Image mHostFullscreenImage;
    private Image mHostWindowedImage;

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
            Debug.LogError("UGUIReactBitsReplicaShowcase_V2 requires RectTransform.", this);
            return;
        }

        DiscoverEffects();

        EnsureCanvas();
        EnsureEventSystem();
        mCurrentBackend = mStartBackend;
        mCurrentGroup = mStartGroup;
        mCurrentEffect = mStartEffect;
        mCurrentHostMode = mStartHostMode;
        mCurrentStackCount = Mathf.Clamp(mStartStackCount, 4, 8);

        if (!mGroups.Contains(mCurrentGroup) && mGroups.Count > 0)
        {
            mCurrentGroup = mGroups[0];
        }

        var effectExists = mAllItems.Any(i => i.Group == mCurrentGroup && i.Name == mCurrentEffect);
        if (!effectExists)
        {
            mCurrentEffect = mAllItems.FirstOrDefault(i => i.Group == mCurrentGroup)?.Name;
        }

        BuildLayout();

        RefreshToggles();
        RebuildEffect();
    }

    private void DiscoverEffects()
    {
        mAllItems.Clear();
        mGroups.Clear();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    var attributes = type.GetCustomAttributes(typeof(ReplicaShowcaseAttribute), false);
                    if (attributes.Length > 0)
                    {
                        var attr = (ReplicaShowcaseAttribute)attributes[0];
                        mAllItems.Add(new ShowcaseItem
                        {
                            Group = attr.Group,
                            Name = attr.Name,
                            BridgeType = type
                        });

                        if (!mGroups.Contains(attr.Group))
                        {
                            mGroups.Add(attr.Group);
                        }
                    }
                }
            }
        }
    }

    private void BuildLayout()
    {
        for (var i = mRootRect.childCount - 1; i >= 0; i--)
        {
            Destroy(mRootRect.GetChild(i).gameObject);
        }

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
        sidebar.offsetMax = new Vector2(390f, -16f);

        var stage = ReplicaUIFactoryV2.CreatePanel("Stage", shell, sStage);
        stage.anchorMin = new Vector2(0f, 0f);
        stage.anchorMax = new Vector2(1f, 1f);
        stage.offsetMin = new Vector2(408f, 16f);
        stage.offsetMax = new Vector2(-16f, -16f);

        BuildSidebar(sidebar);
        BuildStage(stage);
    }

    private void BuildSidebar(RectTransform sidebar)
    {
        var title = ReplicaUIFactoryV2.CreateText("Title", sidebar, "Replica V2 Showcase", 30, FontStyle.Bold, TextAnchor.UpperLeft, Color.white);
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.offsetMin = new Vector2(18f, -56f);
        titleRect.offsetMax = new Vector2(-18f, -12f);

        var subtitle = ReplicaUIFactoryV2.CreateText("Subtitle", sidebar, "UGUI V2 | auto-discovery backend effects", 14, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.80f, 0.88f, 1f, 0.9f));
        var subtitleRect = (RectTransform)subtitle.transform;
        subtitleRect.anchorMin = new Vector2(0f, 1f);
        subtitleRect.anchorMax = new Vector2(1f, 1f);
        subtitleRect.pivot = new Vector2(0.5f, 1f);
        subtitleRect.offsetMin = new Vector2(18f, -88f);
        subtitleRect.offsetMax = new Vector2(-18f, -62f);

        var y = -120f;

        y = BuildSectionLabel(sidebar, y, "Backend");
        BuildTwoButtons(sidebar, ref y, "DOTween", () => SetBackend(ReplicaTweenBackend.DOTween), out mBackendDotweenImage, "XTween", () => SetBackend(ReplicaTweenBackend.XTween), out mBackendXTweenImage);

        y = BuildSectionLabel(sidebar, y, "Host Mode");
        BuildTwoButtons(sidebar, ref y, "Fullscreen", () => SetHostMode(HostMode.Fullscreen), out mHostFullscreenImage, "Windowed", () => SetHostMode(HostMode.Windowed), out mHostWindowedImage);

        y = BuildSectionLabel(sidebar, y, "Group");
        mGroupToggles.Clear();
        for (int i = 0; i < mGroups.Count; i += 2)
        {
            if (i + 1 < mGroups.Count)
            {
                var g1 = mGroups[i];
                var g2 = mGroups[i + 1];
                BuildTwoButtons(sidebar, ref y, g1, () => SetGroup(g1), out var t1, g2, () => SetGroup(g2), out var t2);
                mGroupToggles[g1] = t1;
                mGroupToggles[g2] = t2;
            }
            else
            {
                var g1 = mGroups[i];
                BuildOneButton(sidebar, ref y, g1, () => SetGroup(g1), out var t1);
                mGroupToggles[g1] = t1;
            }
        }

        y = BuildSectionLabel(sidebar, y, "Effect");
        var currentItems = mAllItems.Where(i => i.Group == mCurrentGroup).ToList();
        for (int i = 0; i < currentItems.Count; i += 2)
        {
            if (i + 1 < currentItems.Count)
            {
                var item1 = currentItems[i];
                var item2 = currentItems[i + 1];
                BuildTwoButtons(sidebar, ref y, item1.Name, () => SetEffect(item1.Name), out item1.ToggleImage, item2.Name, () => SetEffect(item2.Name), out item2.ToggleImage);
            }
            else
            {
                var item1 = currentItems[i];
                BuildOneButton(sidebar, ref y, item1.Name, () => SetEffect(item1.Name), out item1.ToggleImage);
            }
        }

        if (mCurrentGroup == "Cards")
        {
            y = BuildSectionLabel(sidebar, y, "Stack Count");
            BuildStackCountRow(sidebar, ref y);
        }

        y = BuildSectionLabel(sidebar, y, "Transitions");
        BuildTransitionRow(sidebar, ref y);

        mStatusText = ReplicaUIFactoryV2.CreateText("Status", sidebar, "-", 14, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.82f, 0.90f, 1f, 0.94f));
        var statusRect = (RectTransform)mStatusText.transform;
        statusRect.anchorMin = new Vector2(0f, 0f);
        statusRect.anchorMax = new Vector2(1f, 0f);
        statusRect.pivot = new Vector2(0.5f, 0f);
        statusRect.offsetMin = new Vector2(18f, 20f);
        statusRect.offsetMax = new Vector2(-18f, 120f);
    }

    private void BuildStage(RectTransform stage)
    {
        mStageFullHost = ReplicaUIFactoryV2.CreatePanel("FullHost", stage, new Color(0f, 0f, 0f, 0f));
        mStageFullHost.anchorMin = Vector2.zero;
        mStageFullHost.anchorMax = Vector2.one;
        mStageFullHost.offsetMin = new Vector2(20f, 20f);
        mStageFullHost.offsetMax = new Vector2(-20f, -20f);
        mStageFullHost.GetComponent<Image>().raycastTarget = false;

        mStageWindowHost = ReplicaUIFactoryV2.CreatePanel("WindowHost", stage, new Color(0.08f, 0.11f, 0.19f, 0.94f));
        mStageWindowHost.anchorMin = new Vector2(0.5f, 0.5f);
        mStageWindowHost.anchorMax = new Vector2(0.5f, 0.5f);
        mStageWindowHost.pivot = new Vector2(0.5f, 0.5f);
        mStageWindowHost.sizeDelta = new Vector2(860f, 620f);
        mStageWindowHost.anchoredPosition = Vector2.zero;
        mStageWindowHost.GetComponent<Image>().raycastTarget = false;

        var outline = ReplicaUIFactoryV2.EnsureComponent<Outline>(mStageWindowHost.gameObject);
        outline.effectColor = new Color(1f, 1f, 1f, 0.22f);
        outline.effectDistance = new Vector2(1f, -1f);

        var shadow = ReplicaUIFactoryV2.EnsureComponent<Shadow>(mStageWindowHost.gameObject);
        shadow.effectColor = new Color(0f, 0f, 0f, 0.34f);
        shadow.effectDistance = new Vector2(0f, -10f);
    }

    private float BuildSectionLabel(RectTransform parent, float y, string label)
    {
        var text = ReplicaUIFactoryV2.CreateText($"Label_{label}", parent, label, 18, FontStyle.Bold, TextAnchor.UpperLeft, new Color(0.93f, 0.96f, 1f, 1f));
        var rect = (RectTransform)text.transform;
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.offsetMin = new Vector2(18f, y - 26f);
        rect.offsetMax = new Vector2(-18f, y);
        return y - 36f;
    }

    private void BuildTwoButtons(RectTransform parent, ref float y, string leftText, UnityEngine.Events.UnityAction onLeft, out Image leftImage, string rightText, UnityEngine.Events.UnityAction onRight, out Image rightImage)
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

    private void BuildOneButton(RectTransform parent, ref float y, string text, UnityEngine.Events.UnityAction onClick, out Image image)
    {
        var row = ReplicaUIFactoryV2.CreateRect("RowSingle", parent);
        row.anchorMin = new Vector2(0f, 1f);
        row.anchorMax = new Vector2(1f, 1f);
        row.pivot = new Vector2(0.5f, 1f);
        row.offsetMin = new Vector2(18f, y - 42f);
        row.offsetMax = new Vector2(-18f, y);

        var button = ReplicaUIFactoryV2.CreateButton("Button", row, sIdle);
        var buttonRect = (RectTransform)button.transform;
        buttonRect.anchorMin = Vector2.zero;
        buttonRect.anchorMax = Vector2.one;
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        var label = ReplicaUIFactoryV2.CreateText("Label", button.transform, text, 15, FontStyle.Bold, TextAnchor.MiddleCenter, sTextIdle);
        ReplicaUIFactoryV2.Stretch((RectTransform)label.transform);
        button.onClick.AddListener(onClick);
        image = button.image;
        y -= 50f;
    }

    private void BuildStackCountRow(RectTransform parent, ref float y)
    {
        var row = ReplicaUIFactoryV2.CreateRect("StackCountRow", parent);
        row.anchorMin = new Vector2(0f, 1f);
        row.anchorMax = new Vector2(1f, 1f);
        row.pivot = new Vector2(0.5f, 1f);
        row.offsetMin = new Vector2(18f, y - 40f);
        row.offsetMax = new Vector2(-18f, y);

        var layout = ReplicaUIFactoryV2.EnsureComponent<HorizontalLayoutGroup>(row.gameObject);
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = true;
        layout.childForceExpandWidth = true;
        layout.spacing = 6f;

        for (var c = 4; c <= 8; c++)
        {
            var count = c;
            var button = ReplicaUIFactoryV2.CreateButton($"Count_{c}", row, sIdle);
            var label = ReplicaUIFactoryV2.CreateText("Label", button.transform, c.ToString(), 14, FontStyle.Bold, TextAnchor.MiddleCenter, sTextIdle);
            ReplicaUIFactoryV2.Stretch((RectTransform)label.transform);
            button.onClick.AddListener(() => SetStackCount(count));
        }
        y -= 48f;
    }

    private void BuildTransitionRow(RectTransform parent, ref float y)
    {
        var row = ReplicaUIFactoryV2.CreateRect("TransitionRow", parent);
        row.anchorMin = new Vector2(0f, 1f);
        row.anchorMax = new Vector2(1f, 1f);
        row.pivot = new Vector2(0.5f, 1f);
        row.offsetMin = new Vector2(18f, y - 88f);
        row.offsetMax = new Vector2(-18f, y);

        var layout = ReplicaUIFactoryV2.EnsureComponent<VerticalLayoutGroup>(row.gameObject);
        layout.childControlHeight = true;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = true;
        layout.childForceExpandWidth = true;
        layout.spacing = 6f;

        var inBottom = ReplicaUIFactoryV2.CreateButton("InBottom", row, sIdle);
        var inBottomLabel = ReplicaUIFactoryV2.CreateText("Label", inBottom.transform, "Play In Bottom", 14, FontStyle.Bold, TextAnchor.MiddleCenter, sTextIdle);
        ReplicaUIFactoryV2.Stretch((RectTransform)inBottomLabel.transform);
        inBottom.onClick.AddListener(() => PlayIn(ReplicaEnterDirection.FromBottom));

        var inLeft = ReplicaUIFactoryV2.CreateButton("InLeft", row, sIdle);
        var inLeftLabel = ReplicaUIFactoryV2.CreateText("Label", inLeft.transform, "Play In Left", 14, FontStyle.Bold, TextAnchor.MiddleCenter, sTextIdle);
        ReplicaUIFactoryV2.Stretch((RectTransform)inLeftLabel.transform);
        inLeft.onClick.AddListener(() => PlayIn(ReplicaEnterDirection.FromLeft));

        var outRight = ReplicaUIFactoryV2.CreateButton("OutRight", row, sIdle);
        var outRightLabel = ReplicaUIFactoryV2.CreateText("Label", outRight.transform, "Play Out Right", 14, FontStyle.Bold, TextAnchor.MiddleCenter, sTextIdle);
        ReplicaUIFactoryV2.Stretch((RectTransform)outRightLabel.transform);
        outRight.onClick.AddListener(() => PlayOut(ReplicaExitDirection.ToRight));
        y -= 96f;
    }

    private void SetBackend(ReplicaTweenBackend backend)
    {
        if (mCurrentBackend == backend) return;
        mCurrentBackend = backend;
        RebuildEffect();
    }

    private void SetHostMode(HostMode mode)
    {
        if (mCurrentHostMode == mode) return;
        mCurrentHostMode = mode;
        RebuildEffect();
    }

    private void SetGroup(string group)
    {
        if (mCurrentGroup == group) return;
        mCurrentGroup = group;
        var firstGroupEffect = mAllItems.FirstOrDefault(i => i.Group == mCurrentGroup);
        if (firstGroupEffect != null)
        {
            mCurrentEffect = firstGroupEffect.Name;
        }
        BuildLayout();
        RebuildEffect();
        RefreshToggles();
    }

    private void SetEffect(string effect)
    {
        if (mCurrentEffect == effect) return;
        mCurrentEffect = effect;
        RebuildEffect();
    }

    private void SetStackCount(int count)
    {
        mCurrentStackCount = Mathf.Clamp(count, 4, 8);
        if (mCurrentEffectRoot != null)
        {
            mCurrentEffectRoot.gameObject.SendMessage("SetItemCount", mCurrentStackCount, SendMessageOptions.DontRequireReceiver);
            SetStatus($"Stack count: {mCurrentStackCount}");
        }
    }

    private void PlayIn(ReplicaEnterDirection direction)
    {
        if (mCurrentBridgeInstance != null)
        {
            mCurrentBridgeInstance.PlayIn(direction);
        }
        SetStatus($"PlayIn: {direction}");
    }

    private void PlayOut(ReplicaExitDirection direction)
    {
        if (mCurrentBridgeInstance != null)
        {
            mCurrentBridgeInstance.PlayOut(direction);
        }
        SetStatus($"PlayOut: {direction}");
    }

    private void RebuildEffect()
    {
        if (mCurrentEffectRoot != null)
        {
            Destroy(mCurrentEffectRoot.gameObject);
            mCurrentEffectRoot = null;
            mCurrentBridgeInstance = null;
        }

        var mount = mCurrentHostMode == HostMode.Fullscreen ? mStageFullHost : mStageWindowHost;
        if (mount == null) return;

        mCurrentEffectRoot = ReplicaUIFactoryV2.CreateRect("CurrentEffect", mount);
        ReplicaUIFactoryV2.Stretch(mCurrentEffectRoot);

        var item = mAllItems.FirstOrDefault(i => i.Group == mCurrentGroup && i.Name == mCurrentEffect);
        if (item != null)
        {
            var bridgeComponent = mCurrentEffectRoot.gameObject.AddComponent(item.BridgeType);
            mCurrentBridgeInstance = bridgeComponent as IReplicaShowcaseBridge;
            
            if (mCurrentBridgeInstance != null)
            {
                mCurrentBridgeInstance.SetBackend(mCurrentBackend, false);
                
                // Hack for stack count to not break existing behavior
                if (mCurrentGroup == "Cards" && mCurrentEffect == "Stack")
                {
                    bridgeComponent.gameObject.SendMessage("SetItemCount", mCurrentStackCount, SendMessageOptions.DontRequireReceiver);
                }

                // If some effects need custom default models, they now instantiate it internally if it's null in their OnEnable.
                mCurrentBridgeInstance.PlayIn(ReplicaEnterDirection.FromBottom);
            }
        }

        RefreshToggles();
        SetStatus($"Effect: {mCurrentEffect} | Backend: {mCurrentBackend} | Host: {mCurrentHostMode}");
    }

    private void RefreshToggles()
    {
        SetToggle(mBackendDotweenImage, mCurrentBackend == ReplicaTweenBackend.DOTween);
        SetToggle(mBackendXTweenImage, mCurrentBackend == ReplicaTweenBackend.XTween);

        SetToggle(mHostFullscreenImage, mCurrentHostMode == HostMode.Fullscreen);
        SetToggle(mHostWindowedImage, mCurrentHostMode == HostMode.Windowed);

        foreach (var kvp in mGroupToggles)
        {
            SetToggle(kvp.Value, kvp.Key == mCurrentGroup);
        }

        foreach (var item in mAllItems)
        {
            if (item.Group == mCurrentGroup && item.ToggleImage != null)
            {
                SetToggle(item.ToggleImage, item.Name == mCurrentEffect);
            }
        }

        if (mStageWindowHost != null)
        {
            mStageWindowHost.gameObject.SetActive(mCurrentHostMode == HostMode.Windowed);
        }
    }

    private static void SetToggle(Image image, bool active)
    {
        if (image == null) return;
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

    private void EnsureCanvas()
    {
        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null) return;

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
        if (FindAnyObjectByType<EventSystem>() != null) return;

#if ENABLE_INPUT_SYSTEM
        _ = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
#else
        _ = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
#endif
    }
}
