using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUIStaggeredMenuReplica : MonoBehaviour
{
    [SerializeField] private string[] mMenuItems = { "Home", "Works", "Services", "Journal", "Contact" };
    [SerializeField] private string[] mSocialItems = { "GitHub", "Dribbble", "Behance" };

    private readonly List<RectTransform> mPreLayers = new();
    private readonly List<RectTransform> mItemLabels = new();
    private readonly List<RectTransform> mItemRows = new();
    private readonly List<Text> mItemNumbers = new();
    private readonly List<RectTransform> mSocialLinks = new();

    private RectTransform mPanel;
    private RectTransform mToggleIcon;
    private RectTransform mToggleTextStack;
    private RectTransform mToggleLabelMask;
    private Text mToggleTextTop;
    private Text mToggleTextBottom;
    private CanvasGroup mDismissGroup;
    private Button mDismissButton;
    private RectTransform mMenuListRoot;
    private RectTransform mSocialRoot;
    private Text mSocialTitle;
    private bool mOpen;
    private bool mBusy;

    private Sequence mOpenSequence;
    private Sequence mCloseSequence;
    private Tween mIconTween;
    private Tween mTextTween;
    private Tween mColorTween;

    private static readonly Color sAccent = new(0.32f, 0.15f, 1f, 1f);
    private static readonly Color sPanelColor = new(0.96f, 0.97f, 1f, 0.98f);
    private static readonly Color sToggleClosed = new(0.93f, 0.95f, 1f, 1f);
    private static readonly Color sToggleOpen = new(0.12f, 0.14f, 0.22f, 1f);
    private static readonly Color[] sLayerColors =
    {
        new Color(0.63f, 0.53f, 0.95f, 0.86f),
        new Color(0.32f, 0.15f, 1.00f, 0.72f),
        new Color(0.16f, 0.09f, 0.55f, 0.58f)
    };

    private void Awake()
    {
        BuildView();
        ApplyClosedPose(true);
    }

    private void OnEnable()
    {
        if (mOpen)
        {
            ApplyOpenPose(true);
        }
        else
        {
            ApplyClosedPose(true);
        }
    }

    private void OnDisable()
    {
        KillTweens();
    }

    private void BuildView()
    {
        var root = (RectTransform)transform;
        Stretch(root);

        var rootImage = UGUIReplicaUIFactory.EnsureComponent<Image>(gameObject);
        rootImage.color = new Color(0.05f, 0.07f, 0.14f, 0.92f);
        rootImage.raycastTarget = true;

        CreateAmbientPlate("AmbientA", root, new Color(0.15f, 0.22f, 0.40f, 0.52f), new Vector2(-220f, 180f), 16f, 1100f, 460f);
        CreateAmbientPlate("AmbientB", root, new Color(0.08f, 0.35f, 0.56f, 0.42f), new Vector2(260f, -120f), -11f, 1280f, 520f);

        // Top bar
        var topBar = UGUIReplicaUIFactory.CreateRect("TopBar", root);
        topBar.anchorMin = new Vector2(0f, 1f);
        topBar.anchorMax = new Vector2(1f, 1f);
        topBar.pivot = new Vector2(0.5f, 1f);
        topBar.sizeDelta = new Vector2(0f, 108f);
        topBar.anchoredPosition = Vector2.zero;

        var logo = UGUIReplicaUIFactory.CreateText(
            "Logo",
            topBar,
            "ReactBits / Staggered Menu",
            28,
            FontStyle.Bold,
            TextAnchor.MiddleLeft,
            new Color(0.92f, 0.95f, 1f, 1f));
        var logoRect = (RectTransform)logo.transform;
        logoRect.anchorMin = new Vector2(0f, 0.5f);
        logoRect.anchorMax = new Vector2(0f, 0.5f);
        logoRect.pivot = new Vector2(0f, 0.5f);
        logoRect.sizeDelta = new Vector2(620f, 44f);
        logoRect.anchoredPosition = new Vector2(36f, 0f);

        // Toggle button
        var toggle = UGUIReplicaUIFactory.CreateButton("Toggle", topBar, new Color(1f, 1f, 1f, 0f));
        var toggleRect = (RectTransform)toggle.transform;
        toggleRect.anchorMin = new Vector2(1f, 0.5f);
        toggleRect.anchorMax = new Vector2(1f, 0.5f);
        toggleRect.pivot = new Vector2(1f, 0.5f);
        toggleRect.sizeDelta = new Vector2(182f, 52f);
        toggleRect.anchoredPosition = new Vector2(-30f, 0f);
        toggle.onClick.AddListener(ToggleMenu);

        // Text cycling mask
        mToggleLabelMask = UGUIReplicaUIFactory.CreateRect("ToggleLabelMask", toggle.transform);
        mToggleLabelMask.anchorMin = new Vector2(0f, 0.5f);
        mToggleLabelMask.anchorMax = new Vector2(0f, 0.5f);
        mToggleLabelMask.pivot = new Vector2(0f, 0.5f);
        mToggleLabelMask.sizeDelta = new Vector2(116f, 30f);
        mToggleLabelMask.anchoredPosition = new Vector2(0f, 0f);
        UGUIReplicaUIFactory.EnsureComponent<RectMask2D>(mToggleLabelMask.gameObject);

        mToggleTextStack = UGUIReplicaUIFactory.CreateRect("ToggleTextStack", mToggleLabelMask);
        mToggleTextStack.anchorMin = new Vector2(0f, 1f);
        mToggleTextStack.anchorMax = new Vector2(1f, 1f);
        mToggleTextStack.pivot = new Vector2(0.5f, 1f);
        mToggleTextStack.sizeDelta = new Vector2(0f, 60f);
        mToggleTextStack.anchoredPosition = Vector2.zero;

        mToggleTextTop = UGUIReplicaUIFactory.CreateText("MenuLine", mToggleTextStack, "Menu", 26, FontStyle.Bold, TextAnchor.MiddleLeft, sToggleClosed);
        var topTextRect = (RectTransform)mToggleTextTop.transform;
        topTextRect.anchorMin = new Vector2(0f, 1f);
        topTextRect.anchorMax = new Vector2(1f, 1f);
        topTextRect.pivot = new Vector2(0.5f, 1f);
        topTextRect.sizeDelta = new Vector2(0f, 30f);
        topTextRect.anchoredPosition = Vector2.zero;

        mToggleTextBottom = UGUIReplicaUIFactory.CreateText("CloseLine", mToggleTextStack, "Close", 26, FontStyle.Bold, TextAnchor.MiddleLeft, sToggleOpen);
        var bottomTextRect = (RectTransform)mToggleTextBottom.transform;
        bottomTextRect.anchorMin = new Vector2(0f, 1f);
        bottomTextRect.anchorMax = new Vector2(1f, 1f);
        bottomTextRect.pivot = new Vector2(0.5f, 1f);
        bottomTextRect.sizeDelta = new Vector2(0f, 30f);
        bottomTextRect.anchoredPosition = new Vector2(0f, -30f);

        // Toggle icon (+/x)
        mToggleIcon = UGUIReplicaUIFactory.CreateRect("Icon", toggle.transform);
        mToggleIcon.anchorMin = new Vector2(1f, 0.5f);
        mToggleIcon.anchorMax = new Vector2(1f, 0.5f);
        mToggleIcon.pivot = new Vector2(1f, 0.5f);
        mToggleIcon.sizeDelta = new Vector2(24f, 24f);
        mToggleIcon.anchoredPosition = new Vector2(-8f, 0f);

        CreateIconLine("IconHorizontal", mToggleIcon, Vector2.zero, 0f, sToggleClosed);
        CreateIconLine("IconVertical", mToggleIcon, Vector2.zero, 90f, sToggleClosed);

        // Pre-layers (colored sliding panels behind the main panel)
        var layerRoot = UGUIReplicaUIFactory.CreateRect("PreLayers", root);
        layerRoot.anchorMin = new Vector2(1f, 0f);
        layerRoot.anchorMax = new Vector2(1f, 1f);
        layerRoot.pivot = new Vector2(1f, 0.5f);
        layerRoot.sizeDelta = new Vector2(470f, 0f);
        layerRoot.anchoredPosition = Vector2.zero;
        mPreLayers.Clear();

        for (var i = 0; i < sLayerColors.Length; i++)
        {
            var layer = UGUIReplicaUIFactory.CreatePanel($"Layer_{i}", layerRoot, sLayerColors[i]);
            Stretch(layer);
            layer.SetSiblingIndex(0);
            mPreLayers.Add(layer);
        }

        // Main panel
        mPanel = UGUIReplicaUIFactory.CreatePanel("Panel", root, sPanelColor);
        mPanel.anchorMin = new Vector2(1f, 0f);
        mPanel.anchorMax = new Vector2(1f, 1f);
        mPanel.pivot = new Vector2(1f, 0.5f);
        mPanel.sizeDelta = new Vector2(470f, 0f);
        mPanel.anchoredPosition = Vector2.zero;

        var panelShadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(mPanel.gameObject);
        panelShadow.effectColor = new Color(0f, 0f, 0f, 0.28f);
        panelShadow.effectDistance = new Vector2(-8f, -2f);

        var panelInner = UGUIReplicaUIFactory.CreateRect("PanelInner", mPanel);
        panelInner.anchorMin = Vector2.zero;
        panelInner.anchorMax = Vector2.one;
        panelInner.offsetMin = new Vector2(38f, 132f);
        panelInner.offsetMax = new Vector2(-36f, -36f);

        // Menu items with row masks for overflow hidden
        mMenuListRoot = UGUIReplicaUIFactory.CreateRect("MenuList", panelInner);
        mMenuListRoot.anchorMin = new Vector2(0f, 1f);
        mMenuListRoot.anchorMax = new Vector2(1f, 1f);
        mMenuListRoot.pivot = new Vector2(0.5f, 1f);
        mMenuListRoot.sizeDelta = new Vector2(0f, 420f);
        mMenuListRoot.anchoredPosition = Vector2.zero;

        mItemLabels.Clear();
        mItemNumbers.Clear();
        mItemRows.Clear();
        for (var i = 0; i < mMenuItems.Length; i++)
        {
            // Row with RectMask2D (mirrors CSS overflow:hidden on sm-panel-itemWrap)
            var itemRow = UGUIReplicaUIFactory.CreateRect($"Item_{i}", mMenuListRoot);
            itemRow.anchorMin = new Vector2(0f, 1f);
            itemRow.anchorMax = new Vector2(1f, 1f);
            itemRow.pivot = new Vector2(0f, 1f);
            itemRow.sizeDelta = new Vector2(0f, 72f);
            itemRow.anchoredPosition = new Vector2(0f, -i * 74f);
            UGUIReplicaUIFactory.EnsureComponent<RectMask2D>(itemRow.gameObject);
            mItemRows.Add(itemRow);

            var label = UGUIReplicaUIFactory.CreateText(
                "Label",
                itemRow,
                mMenuItems[i].ToUpperInvariant(),
                52,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Color(0.09f, 0.10f, 0.14f, 1f));
            var labelRect = (RectTransform)label.transform;
            labelRect.anchorMin = new Vector2(0f, 0.5f);
            labelRect.anchorMax = new Vector2(1f, 0.5f);
            labelRect.pivot = new Vector2(0f, 0.5f);
            labelRect.sizeDelta = new Vector2(0f, 70f);
            labelRect.anchoredPosition = new Vector2(0f, 0f);

            var number = UGUIReplicaUIFactory.CreateText(
                "Number",
                itemRow,
                (i + 1).ToString("00"),
                20,
                FontStyle.Normal,
                TextAnchor.MiddleRight,
                sAccent);
            var numberRect = (RectTransform)number.transform;
            numberRect.anchorMin = new Vector2(1f, 0.5f);
            numberRect.anchorMax = new Vector2(1f, 0.5f);
            numberRect.pivot = new Vector2(1f, 0.5f);
            numberRect.sizeDelta = new Vector2(74f, 32f);
            numberRect.anchoredPosition = new Vector2(0f, 8f);

            mItemLabels.Add(labelRect);
            mItemNumbers.Add(number);
        }

        // Social links section
        mSocialRoot = UGUIReplicaUIFactory.CreateRect("Socials", panelInner);
        mSocialRoot.anchorMin = new Vector2(0f, 0f);
        mSocialRoot.anchorMax = new Vector2(1f, 0f);
        mSocialRoot.pivot = new Vector2(0.5f, 0f);
        mSocialRoot.sizeDelta = new Vector2(0f, 160f);
        mSocialRoot.anchoredPosition = new Vector2(0f, 0f);

        mSocialTitle = UGUIReplicaUIFactory.CreateText(
            "SocialTitle",
            mSocialRoot,
            "Socials",
            24,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            sAccent);
        var socialTitleRect = (RectTransform)mSocialTitle.transform;
        socialTitleRect.anchorMin = new Vector2(0f, 1f);
        socialTitleRect.anchorMax = new Vector2(1f, 1f);
        socialTitleRect.pivot = new Vector2(0f, 1f);
        socialTitleRect.sizeDelta = new Vector2(0f, 38f);
        socialTitleRect.anchoredPosition = Vector2.zero;

        mSocialLinks.Clear();
        for (var i = 0; i < mSocialItems.Length; i++)
        {
            var link = UGUIReplicaUIFactory.CreateText(
                $"Social_{i}",
                mSocialRoot,
                mSocialItems[i],
                26,
                FontStyle.Bold,
                TextAnchor.UpperLeft,
                new Color(0.12f, 0.13f, 0.20f, 1f));
            var linkRect = (RectTransform)link.transform;
            linkRect.anchorMin = new Vector2(0f, 1f);
            linkRect.anchorMax = new Vector2(1f, 1f);
            linkRect.pivot = new Vector2(0f, 1f);
            linkRect.sizeDelta = new Vector2(0f, 34f);
            linkRect.anchoredPosition = new Vector2(0f, -52f - (i * 34f));
            mSocialLinks.Add(linkRect);
        }

        // Dismiss overlay — uses CanvasGroup so it doesn't block raycasts when closed
        var dismissGo = new GameObject("Dismiss", typeof(RectTransform), typeof(Image), typeof(Button), typeof(CanvasGroup));
        var dismissRect = dismissGo.GetComponent<RectTransform>();
        dismissRect.SetParent(root, false);
        Stretch(dismissRect);
        dismissRect.SetSiblingIndex(mPanel.GetSiblingIndex());

        var dismissImage = dismissGo.GetComponent<Image>();
        dismissImage.color = new Color(0f, 0f, 0f, 0f);
        dismissImage.raycastTarget = true;

        mDismissButton = dismissGo.GetComponent<Button>();
        mDismissButton.transition = Selectable.Transition.None;
        mDismissButton.onClick.AddListener(CloseMenuIfOpen);

        mDismissGroup = dismissGo.GetComponent<CanvasGroup>();
        mDismissGroup.alpha = 1f;
        mDismissGroup.blocksRaycasts = false;
        mDismissGroup.interactable = false;
    }

    private void ToggleMenu()
    {
        if (mBusy)
        {
            return;
        }

        if (mOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    private void OpenMenu()
    {
        mOpen = true;
        mBusy = true;
        KillTweens();
        mDismissGroup.blocksRaycasts = true;
        mDismissGroup.interactable = true;

        // Set items to hidden pose: offset down + slight rotation (matching gsap yPercent:140, rotate:10)
        SetItemHiddenPose();

        mOpenSequence = DOTween.Sequence().SetUpdate(true);

        // Pre-layers slide in with stagger
        for (var i = 0; i < mPreLayers.Count; i++)
        {
            mOpenSequence.Join(
                mPreLayers[i]
                    .DOAnchorPosX(0f, 0.5f)
                    .SetEase(Ease.OutQuart)
                    .SetDelay(i * 0.07f));
        }

        // Main panel slides in after layers
        var panelDelay = Mathf.Max(0.08f, mPreLayers.Count * 0.07f);
        mOpenSequence.Join(
            mPanel
                .DOAnchorPosX(0f, 0.65f)
                .SetEase(Ease.OutQuart)
                .SetDelay(panelDelay));

        // Item labels slide up + rotate back to 0 with stagger
        var itemsStart = panelDelay + 0.65f * 0.15f;
        for (var i = 0; i < mItemLabels.Count; i++)
        {
            var delay = itemsStart + (i * 0.1f);

            // Animate label Y back to 0
            mOpenSequence.Join(
                mItemLabels[i]
                    .DOAnchorPosY(0f, 1.0f)
                    .SetEase(Ease.OutQuart)
                    .SetDelay(delay));

            // Animate label rotation back to 0
            mOpenSequence.Join(
                mItemLabels[i]
                    .DOLocalRotate(Vector3.zero, 1.0f, RotateMode.Fast)
                    .SetEase(Ease.OutQuart)
                    .SetDelay(delay));

            // Fade in number
            mOpenSequence.Join(
                mItemNumbers[i]
                    .DOFade(1f, 0.6f)
                    .SetEase(Ease.OutQuad)
                    .SetDelay(delay + 0.1f));
        }

        // Social title fade in
        var socialsStart = panelDelay + 0.65f * 0.4f;
        mOpenSequence.Join(
            mSocialTitle
                .DOFade(1f, 0.5f)
                .SetEase(Ease.OutQuad)
                .SetDelay(socialsStart));

        // Social links slide up + fade in
        for (var i = 0; i < mSocialLinks.Count; i++)
        {
            var delay = socialsStart + 0.04f + (i * 0.08f);
            var targetY = -52f - (i * 34f);
            mOpenSequence.Join(
                mSocialLinks[i]
                    .DOAnchorPosY(targetY, 0.55f)
                    .SetEase(Ease.OutCubic)
                    .SetDelay(delay));
            mOpenSequence.Join(
                mSocialLinks[i].GetComponent<Text>()
                    .DOFade(1f, 0.55f)
                    .SetDelay(delay));
        }

        // Icon rotates to X (225°)
        mIconTween = mToggleIcon
            .DORotate(new Vector3(0f, 0f, 225f), 0.8f, RotateMode.Fast)
            .SetEase(Ease.OutQuart)
            .SetUpdate(true);

        // Text cycles from "Menu" to "Close"
        mTextTween = mToggleTextStack
            .DOAnchorPosY(30f, 0.5f)
            .SetEase(Ease.OutQuart)
            .SetUpdate(true);

        // Color transition: toggle label fades to dark
        mColorTween = DOTween.To(
            () => mToggleTextTop.color,
            c => mToggleTextTop.color = c,
            sToggleOpen,
            0.3f)
            .SetDelay(0.18f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);

        mOpenSequence.OnComplete(() => mBusy = false);
    }

    private void CloseMenu()
    {
        mOpen = false;
        mBusy = true;
        KillTweens();

        mCloseSequence = DOTween.Sequence().SetUpdate(true);
        var offscreen = GetOffscreenDistance();

        // Panel + layers slide out simultaneously
        mCloseSequence.Join(mPanel.DOAnchorPosX(offscreen, 0.32f).SetEase(Ease.InCubic));
        for (var i = 0; i < mPreLayers.Count; i++)
        {
            mCloseSequence.Join(mPreLayers[i].DOAnchorPosX(offscreen, 0.32f).SetEase(Ease.InCubic));
        }

        mCloseSequence.OnComplete(() =>
        {
            // Reset item poses to hidden after close finishes
            SetItemHiddenPose();
            mSocialTitle.color = new Color(sAccent.r, sAccent.g, sAccent.b, 0f);
            mDismissGroup.blocksRaycasts = false;
            mDismissGroup.interactable = false;
            mBusy = false;
        });

        // Icon rotates back to + (0°)
        mIconTween = mToggleIcon
            .DORotate(Vector3.zero, 0.35f, RotateMode.Fast)
            .SetEase(Ease.InOutCubic)
            .SetUpdate(true);

        // Text cycles back to "Menu"
        mTextTween = mToggleTextStack
            .DOAnchorPosY(0f, 0.35f)
            .SetEase(Ease.InOutCubic)
            .SetUpdate(true);

        // Color transition: toggle label fades to light
        mColorTween = DOTween.To(
            () => mToggleTextTop.color,
            c => mToggleTextTop.color = c,
            sToggleClosed,
            0.3f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    private void CloseMenuIfOpen()
    {
        if (mOpen && !mBusy)
        {
            CloseMenu();
        }
    }

    /// <summary>
    /// Set item labels to hidden pose: shifted down + rotated (matching gsap yPercent:140, rotate:10)
    /// </summary>
    private void SetItemHiddenPose()
    {
        for (var i = 0; i < mItemLabels.Count; i++)
        {
            // yPercent:140 in CSS means translate 140% of element height downward
            // Label height ~70, so offset = -70 * 1.4 ≈ -98
            mItemLabels[i].anchoredPosition = new Vector2(0f, -98f);
            mItemLabels[i].localRotation = Quaternion.Euler(0f, 0f, -10f);
            mItemNumbers[i].color = new Color(sAccent.r, sAccent.g, sAccent.b, 0f);
        }

        for (var i = 0; i < mSocialLinks.Count; i++)
        {
            var text = mSocialLinks[i].GetComponent<Text>();
            var openY = -52f - (i * 34f);
            mSocialLinks[i].anchoredPosition = new Vector2(0f, openY - 25f);
            text.color = new Color(0.12f, 0.13f, 0.20f, 0f);
        }

        mSocialTitle.color = new Color(sAccent.r, sAccent.g, sAccent.b, 0f);
    }

    private void ApplyClosedPose(bool instant)
    {
        if (instant)
        {
            KillTweens();
        }

        var offscreen = GetOffscreenDistance();
        for (var i = 0; i < mPreLayers.Count; i++)
        {
            mPreLayers[i].anchoredPosition = new Vector2(offscreen, 0f);
        }

        mPanel.anchoredPosition = new Vector2(offscreen, 0f);
        mToggleIcon.localRotation = Quaternion.identity;
        mToggleTextStack.anchoredPosition = Vector2.zero;
        mToggleTextTop.color = sToggleClosed;
        mDismissGroup.blocksRaycasts = false;
        mDismissGroup.interactable = false;
        SetItemHiddenPose();
    }

    private void ApplyOpenPose(bool instant)
    {
        if (instant)
        {
            KillTweens();
        }

        for (var i = 0; i < mPreLayers.Count; i++)
        {
            mPreLayers[i].anchoredPosition = Vector2.zero;
        }

        mPanel.anchoredPosition = Vector2.zero;
        mToggleIcon.localRotation = Quaternion.Euler(0f, 0f, 225f);
        mToggleTextStack.anchoredPosition = new Vector2(0f, 30f);
        mToggleTextTop.color = sToggleOpen;
        mDismissGroup.blocksRaycasts = true;
        mDismissGroup.interactable = true;

        // Items in visible state
        for (var i = 0; i < mItemLabels.Count; i++)
        {
            mItemLabels[i].anchoredPosition = Vector2.zero;
            mItemLabels[i].localRotation = Quaternion.identity;
            mItemNumbers[i].color = new Color(sAccent.r, sAccent.g, sAccent.b, 1f);
        }

        for (var i = 0; i < mSocialLinks.Count; i++)
        {
            var text = mSocialLinks[i].GetComponent<Text>();
            var openY = -52f - (i * 34f);
            mSocialLinks[i].anchoredPosition = new Vector2(0f, openY);
            text.color = new Color(0.12f, 0.13f, 0.20f, 1f);
        }

        mSocialTitle.color = sAccent;
    }

    private static void CreateAmbientPlate(string name, RectTransform parent, Color color, Vector2 pos, float rotZ, float width, float height)
    {
        var plate = UGUIReplicaUIFactory.CreatePanel(name, parent, color);
        plate.anchorMin = new Vector2(0.5f, 0.5f);
        plate.anchorMax = new Vector2(0.5f, 0.5f);
        plate.pivot = new Vector2(0.5f, 0.5f);
        plate.sizeDelta = new Vector2(width, height);
        plate.anchoredPosition = pos;
        plate.localRotation = Quaternion.Euler(0f, 0f, rotZ);
        plate.SetSiblingIndex(0);
    }

    private static void CreateIconLine(string name, RectTransform parent, Vector2 pos, float rotZ, Color color)
    {
        var line = UGUIReplicaUIFactory.CreatePanel(name, parent, color);
        line.anchorMin = new Vector2(0.5f, 0.5f);
        line.anchorMax = new Vector2(0.5f, 0.5f);
        line.pivot = new Vector2(0.5f, 0.5f);
        line.sizeDelta = new Vector2(22f, 2.2f);
        line.anchoredPosition = pos;
        line.localRotation = Quaternion.Euler(0f, 0f, rotZ);
    }

    private float GetOffscreenDistance()
    {
        return Mathf.Max(480f, mPanel.rect.width + 80f);
    }

    private void KillTweens()
    {
        mOpenSequence?.Kill();
        mCloseSequence?.Kill();
        mIconTween?.Kill();
        mTextTween?.Kill();
        mColorTween?.Kill();
        mOpenSequence = null;
        mCloseSequence = null;
        mIconTween = null;
        mTextTween = null;
        mColorTween = null;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
