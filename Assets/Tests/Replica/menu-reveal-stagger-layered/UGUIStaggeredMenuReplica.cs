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
    private readonly List<Text> mItemNumbers = new();
    private readonly List<RectTransform> mSocialLinks = new();

    private RectTransform mPanel;
    private RectTransform mToggleIcon;
    private RectTransform mToggleTextStack;
    private Text mToggleTextTop;
    private Text mToggleTextBottom;
    private Button mDismissButton;
    private RectTransform mMenuListRoot;
    private RectTransform mSocialRoot;
    private bool mOpen;
    private bool mBusy;

    private Sequence mOpenSequence;
    private Sequence mCloseSequence;
    private Tween mIconTween;
    private Tween mTextTween;

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

        var toggle = UGUIReplicaUIFactory.CreateButton("Toggle", topBar, new Color(1f, 1f, 1f, 0f));
        var toggleRect = (RectTransform)toggle.transform;
        toggleRect.anchorMin = new Vector2(1f, 0.5f);
        toggleRect.anchorMax = new Vector2(1f, 0.5f);
        toggleRect.pivot = new Vector2(1f, 0.5f);
        toggleRect.sizeDelta = new Vector2(182f, 52f);
        toggleRect.anchoredPosition = new Vector2(-30f, 0f);
        toggle.onClick.AddListener(ToggleMenu);

        var toggleLabelMask = UGUIReplicaUIFactory.CreateRect("ToggleLabelMask", toggle.transform);
        toggleLabelMask.anchorMin = new Vector2(0f, 0.5f);
        toggleLabelMask.anchorMax = new Vector2(0f, 0.5f);
        toggleLabelMask.pivot = new Vector2(0f, 0.5f);
        toggleLabelMask.sizeDelta = new Vector2(116f, 30f);
        toggleLabelMask.anchoredPosition = new Vector2(0f, 0f);
        UGUIReplicaUIFactory.EnsureComponent<RectMask2D>(toggleLabelMask.gameObject);

        mToggleTextStack = UGUIReplicaUIFactory.CreateRect("ToggleTextStack", toggleLabelMask);
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

        mToggleIcon = UGUIReplicaUIFactory.CreateRect("Icon", toggle.transform);
        mToggleIcon.anchorMin = new Vector2(1f, 0.5f);
        mToggleIcon.anchorMax = new Vector2(1f, 0.5f);
        mToggleIcon.pivot = new Vector2(1f, 0.5f);
        mToggleIcon.sizeDelta = new Vector2(24f, 24f);
        mToggleIcon.anchoredPosition = new Vector2(-8f, 0f);

        CreateIconLine("IconHorizontal", mToggleIcon, Vector2.zero, 0f, sToggleClosed);
        CreateIconLine("IconVertical", mToggleIcon, Vector2.zero, 90f, sToggleClosed);

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

        mMenuListRoot = UGUIReplicaUIFactory.CreateRect("MenuList", panelInner);
        mMenuListRoot.anchorMin = new Vector2(0f, 1f);
        mMenuListRoot.anchorMax = new Vector2(1f, 1f);
        mMenuListRoot.pivot = new Vector2(0.5f, 1f);
        mMenuListRoot.sizeDelta = new Vector2(0f, 420f);
        mMenuListRoot.anchoredPosition = Vector2.zero;

        mItemLabels.Clear();
        mItemNumbers.Clear();
        for (var i = 0; i < mMenuItems.Length; i++)
        {
            var itemRow = UGUIReplicaUIFactory.CreateRect($"Item_{i}", mMenuListRoot);
            itemRow.anchorMin = new Vector2(0f, 1f);
            itemRow.anchorMax = new Vector2(1f, 1f);
            itemRow.pivot = new Vector2(0f, 1f);
            itemRow.sizeDelta = new Vector2(0f, 72f);
            itemRow.anchoredPosition = new Vector2(0f, -i * 74f);

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

        mSocialRoot = UGUIReplicaUIFactory.CreateRect("Socials", panelInner);
        mSocialRoot.anchorMin = new Vector2(0f, 0f);
        mSocialRoot.anchorMax = new Vector2(1f, 0f);
        mSocialRoot.pivot = new Vector2(0.5f, 0f);
        mSocialRoot.sizeDelta = new Vector2(0f, 160f);
        mSocialRoot.anchoredPosition = new Vector2(0f, 0f);

        var socialTitle = UGUIReplicaUIFactory.CreateText(
            "SocialTitle",
            mSocialRoot,
            "Socials",
            24,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            sAccent);
        var socialTitleRect = (RectTransform)socialTitle.transform;
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

        mDismissButton = UGUIReplicaUIFactory.CreateButton("Dismiss", root, new Color(0f, 0f, 0f, 0f));
        var dismissRect = (RectTransform)mDismissButton.transform;
        Stretch(dismissRect);
        dismissRect.SetSiblingIndex(mPanel.GetSiblingIndex() - 1);
        mDismissButton.onClick.AddListener(CloseMenuIfOpen);
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
        mDismissButton.interactable = true;

        SetItemPose(true);

        mOpenSequence = DOTween.Sequence().SetUpdate(true);
        for (var i = 0; i < mPreLayers.Count; i++)
        {
            mOpenSequence.Join(
                mPreLayers[i]
                    .DOAnchorPosX(0f, 0.5f)
                    .SetEase(Ease.OutQuart)
                    .SetDelay(i * 0.07f));
        }

        var panelDelay = Mathf.Max(0.08f, mPreLayers.Count * 0.07f);
        mOpenSequence.Join(
            mPanel
                .DOAnchorPosX(0f, 0.65f)
                .SetEase(Ease.OutQuart)
                .SetDelay(panelDelay));

        for (var i = 0; i < mItemLabels.Count; i++)
        {
            var delay = panelDelay + 0.18f + (i * 0.09f);
            mOpenSequence.Join(mItemLabels[i].DOAnchorPosY(0f, 0.9f).SetEase(Ease.OutQuart).SetDelay(delay));
            mOpenSequence.Join(mItemNumbers[i].DOFade(1f, 0.55f).SetEase(Ease.OutQuad).SetDelay(delay + 0.08f));
        }

        for (var i = 0; i < mSocialLinks.Count; i++)
        {
            var delay = panelDelay + 0.45f + (i * 0.08f);
            mOpenSequence.Join(mSocialLinks[i].DOAnchorPosY(-52f - (i * 34f), 0.5f).SetEase(Ease.OutCubic).SetDelay(delay));
            mOpenSequence.Join(mSocialLinks[i].GetComponent<Text>().DOFade(1f, 0.4f).SetDelay(delay));
        }

        mIconTween = mToggleIcon
            .DORotate(new Vector3(0f, 0f, 225f), 0.76f, RotateMode.Fast)
            .SetEase(Ease.OutQuart)
            .SetUpdate(true);

        mTextTween = mToggleTextStack
            .DOAnchorPosY(30f, 0.42f)
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);

        mOpenSequence.OnComplete(() => mBusy = false);
    }

    private void CloseMenu()
    {
        mOpen = false;
        mBusy = true;
        KillTweens();

        mCloseSequence = DOTween.Sequence().SetUpdate(true);
        mCloseSequence.Join(mPanel.DOAnchorPosX(GetOffscreenDistance(), 0.32f).SetEase(Ease.InCubic));
        for (var i = 0; i < mPreLayers.Count; i++)
        {
            mCloseSequence.Join(mPreLayers[i].DOAnchorPosX(GetOffscreenDistance(), 0.32f).SetEase(Ease.InCubic));
        }

        mCloseSequence.OnComplete(() =>
        {
            SetItemPose(false);
            mDismissButton.interactable = false;
            mBusy = false;
        });

        mIconTween = mToggleIcon
            .DORotate(Vector3.zero, 0.32f, RotateMode.Fast)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);

        mTextTween = mToggleTextStack
            .DOAnchorPosY(0f, 0.26f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true);
    }

    private void CloseMenuIfOpen()
    {
        if (mOpen)
        {
            CloseMenu();
        }
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
        mDismissButton.interactable = false;
        SetItemPose(false);
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
        mDismissButton.interactable = true;
        SetItemPose(true, true);
    }

    private void SetItemPose(bool openState, bool visibleImmediate = false)
    {
        for (var i = 0; i < mItemLabels.Count; i++)
        {
            mItemLabels[i].anchoredPosition = openState ? new Vector2(0f, 0f) : new Vector2(0f, -92f);
            mItemNumbers[i].color = new Color(sAccent.r, sAccent.g, sAccent.b, openState ? 1f : 0f);
        }

        for (var i = 0; i < mSocialLinks.Count; i++)
        {
            var text = mSocialLinks[i].GetComponent<Text>();
            var openY = -52f - (i * 34f);
            mSocialLinks[i].anchoredPosition = openState ? new Vector2(0f, openY) : new Vector2(0f, openY - 28f);
            text.color = new Color(0.12f, 0.13f, 0.20f, openState || visibleImmediate ? 1f : 0f);
        }

        mToggleTextTop.color = openState ? sToggleOpen : sToggleClosed;
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
        mOpenSequence = null;
        mCloseSequence = null;
        mIconTween = null;
        mTextTween = null;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
