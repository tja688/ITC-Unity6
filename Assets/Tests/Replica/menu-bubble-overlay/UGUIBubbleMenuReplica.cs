using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUIBubbleMenuReplica : MonoBehaviour
{
    [SerializeField] private float mOpenDuration = 0.5f;
    [SerializeField] private float mStaggerDelay = 0.12f;

    private RectTransform mRootRect;
    private RectTransform mOverlayRect;
    private Button mToggleButton;
    private RectTransform mLineTop;
    private RectTransform mLineBottom;
    private bool mMenuOpen;

    private readonly List<PillData> mPills = new();

    private static readonly string[] sMenuLabels = { "home", "about", "projects", "blog", "contact" };
    private static readonly Color[] sHoverBg =
    {
        new(0.23f, 0.51f, 0.96f, 1f),
        new(0.06f, 0.68f, 0.51f, 1f),
        new(0.96f, 0.63f, 0.11f, 1f),
        new(0.93f, 0.27f, 0.26f, 1f),
        new(0.54f, 0.36f, 0.95f, 1f)
    };

    private static readonly Vector2[] sPillPositions =
    {
        new(-340f, 140f),
        new(0f, 140f),
        new(340f, 140f),
        new(-190f, -80f),
        new(190f, -80f)
    };

    private static readonly float[] sPillRotation = { -8f, 8f, 8f, 8f, -8f };

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("UGUIBubbleMenuReplica requires RectTransform.", this);
            return;
        }

        BuildView();
        SetMenuOpen(false, true);
    }

    private void OnEnable()
    {
        if (mOverlayRect == null)
        {
            return;
        }

        SetMenuOpen(false, true);
    }

    private void OnDisable()
    {
        mOverlayRect?.DOKill();
        mLineTop?.DOKill();
        mLineBottom?.DOKill();
        foreach (var pill in mPills)
        {
            pill.Rect.DOKill();
            pill.Label.DOKill();
            pill.Image.DOKill();
        }
    }

    private void BuildView()
    {
        mRootRect.anchorMin = Vector2.zero;
        mRootRect.anchorMax = Vector2.one;
        mRootRect.offsetMin = Vector2.zero;
        mRootRect.offsetMax = Vector2.zero;

        var backdrop = UGUIReplicaUIFactory.CreatePanel("Backdrop", mRootRect, new Color(0.05f, 0.08f, 0.12f, 0.86f));
        backdrop.anchorMin = Vector2.zero;
        backdrop.anchorMax = Vector2.one;
        backdrop.offsetMin = Vector2.zero;
        backdrop.offsetMax = Vector2.zero;

        var topBar = UGUIReplicaUIFactory.CreateRect("TopBar", backdrop);
        topBar.anchorMin = new Vector2(0f, 1f);
        topBar.anchorMax = new Vector2(1f, 1f);
        topBar.pivot = new Vector2(0.5f, 1f);
        topBar.sizeDelta = new Vector2(0f, 120f);
        topBar.anchoredPosition = Vector2.zero;

        var logoBubble = UGUIReplicaUIFactory.CreateButton("LogoBubble", topBar, Color.white);
        var logoRect = (RectTransform)logoBubble.transform;
        logoRect.anchorMin = new Vector2(0f, 0.5f);
        logoRect.anchorMax = new Vector2(0f, 0.5f);
        logoRect.pivot = new Vector2(0f, 0.5f);
        logoRect.sizeDelta = new Vector2(220f, 64f);
        logoRect.anchoredPosition = new Vector2(40f, -56f);
        logoBubble.interactable = false;

        var logoLabel = UGUIReplicaUIFactory.CreateText(
            "LogoLabel",
            logoBubble.transform,
            "react-bits",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.07f, 0.08f, 0.10f, 1f));
        StretchToParent((RectTransform)logoLabel.transform);

        mToggleButton = UGUIReplicaUIFactory.CreateButton("ToggleBubble", topBar, Color.white);
        var toggleRect = (RectTransform)mToggleButton.transform;
        toggleRect.anchorMin = new Vector2(1f, 0.5f);
        toggleRect.anchorMax = new Vector2(1f, 0.5f);
        toggleRect.pivot = new Vector2(1f, 0.5f);
        toggleRect.sizeDelta = new Vector2(64f, 64f);
        toggleRect.anchoredPosition = new Vector2(-40f, -56f);
        mToggleButton.onClick.AddListener(() => SetMenuOpen(!mMenuOpen, false));

        mLineTop = UGUIReplicaUIFactory.CreateRect("LineTop", toggleRect);
        mLineTop.anchorMin = new Vector2(0.5f, 0.5f);
        mLineTop.anchorMax = new Vector2(0.5f, 0.5f);
        mLineTop.sizeDelta = new Vector2(28f, 3f);
        mLineTop.anchoredPosition = new Vector2(0f, 5f);
        UGUIReplicaUIFactory.EnsureComponent<Image>(mLineTop.gameObject).color = new Color(0.08f, 0.10f, 0.12f, 1f);

        mLineBottom = UGUIReplicaUIFactory.CreateRect("LineBottom", toggleRect);
        mLineBottom.anchorMin = new Vector2(0.5f, 0.5f);
        mLineBottom.anchorMax = new Vector2(0.5f, 0.5f);
        mLineBottom.sizeDelta = new Vector2(28f, 3f);
        mLineBottom.anchoredPosition = new Vector2(0f, -5f);
        UGUIReplicaUIFactory.EnsureComponent<Image>(mLineBottom.gameObject).color = new Color(0.08f, 0.10f, 0.12f, 1f);

        var title = UGUIReplicaUIFactory.CreateText(
            "Hint",
            backdrop,
            "BubbleMenu  |  Click top-right bubble to toggle",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.93f, 0.96f, 1f, 0.95f));
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(920f, 58f);
        titleRect.anchoredPosition = new Vector2(0f, -136f);

        mOverlayRect = UGUIReplicaUIFactory.CreatePanel("Overlay", backdrop, new Color(0f, 0f, 0f, 0.08f));
        mOverlayRect.anchorMin = Vector2.zero;
        mOverlayRect.anchorMax = Vector2.one;
        mOverlayRect.offsetMin = Vector2.zero;
        mOverlayRect.offsetMax = Vector2.zero;
        var overlayImage = mOverlayRect.GetComponent<Image>();
        overlayImage.raycastTarget = false;

        var overlayCanvasGroup = UGUIReplicaUIFactory.EnsureComponent<CanvasGroup>(mOverlayRect.gameObject);
        overlayCanvasGroup.alpha = 0f;
        overlayCanvasGroup.interactable = false;
        overlayCanvasGroup.blocksRaycasts = false;

        var pillLayer = UGUIReplicaUIFactory.CreateRect("PillLayer", mOverlayRect);
        StretchToParent(pillLayer);
        pillLayer.SetAsLastSibling();

        mPills.Clear();
        for (var i = 0; i < sMenuLabels.Length; i++)
        {
            mPills.Add(CreatePill(i, pillLayer));
        }

        topBar.SetAsLastSibling();
    }

    private PillData CreatePill(int index, RectTransform parent)
    {
        var pillButton = UGUIReplicaUIFactory.CreateButton($"Pill_{sMenuLabels[index]}", parent, Color.white);
        var pillRect = (RectTransform)pillButton.transform;
        pillRect.anchorMin = new Vector2(0.5f, 0.5f);
        pillRect.anchorMax = new Vector2(0.5f, 0.5f);
        pillRect.pivot = new Vector2(0.5f, 0.5f);
        pillRect.sizeDelta = new Vector2(320f, 132f);
        pillRect.anchoredPosition = sPillPositions[index];
        pillRect.localRotation = Quaternion.Euler(0f, 0f, sPillRotation[index]);
        pillRect.localScale = Vector3.zero;

        var label = UGUIReplicaUIFactory.CreateText(
            "Label",
            pillRect,
            sMenuLabels[index],
            48,
            FontStyle.Normal,
            TextAnchor.MiddleCenter,
            new Color(0.07f, 0.08f, 0.11f, 1f));
        var labelRect = (RectTransform)label.transform;
        StretchToParent(labelRect);
        labelRect.anchoredPosition = new Vector2(0f, 24f);

        var hover = pillRect.gameObject.AddComponent<BubbleMenuPillHover>();
        hover.Initialize(
            pillRect,
            pillButton.image,
            label,
            Color.white,
            sHoverBg[index],
            new Color(0.07f, 0.08f, 0.11f, 1f),
            Color.white);

        return new PillData
        {
            Rect = pillRect,
            Image = pillButton.image,
            Label = label,
            Hover = hover
        };
    }

    private void SetMenuOpen(bool open, bool immediate)
    {
        mMenuOpen = open;
        var overlayCanvasGroup = mOverlayRect.GetComponent<CanvasGroup>();
        overlayCanvasGroup.interactable = open;
        overlayCanvasGroup.blocksRaycasts = open;

        AnimateMenuIcon(open, immediate);

        if (immediate)
        {
            overlayCanvasGroup.alpha = open ? 1f : 0f;
            for (var i = 0; i < mPills.Count; i++)
            {
                var pill = mPills[i];
                pill.Rect.localScale = open ? Vector3.one : Vector3.zero;
                var rect = (RectTransform)pill.Label.transform;
                rect.anchoredPosition = open ? Vector2.zero : new Vector2(0f, 24f);
                var color = pill.Label.color;
                color.a = open ? 1f : 0f;
                pill.Label.color = color;
                pill.Hover.SetInteractable(open);
            }

            return;
        }

        overlayCanvasGroup.DOKill();
        overlayCanvasGroup
            .DOFade(open ? 1f : 0f, open ? 0.28f : 0.18f)
            .SetEase(open ? Ease.OutQuad : Ease.InQuad);

        for (var i = 0; i < mPills.Count; i++)
        {
            var pill = mPills[i];
            pill.Rect.DOKill();
            pill.Label.DOKill();
            pill.Image.DOKill();
            pill.Hover.SetInteractable(open);

            if (open)
            {
                var delay = (i * mStaggerDelay) + (((i % 2) - 0.5f) * 0.04f);
                pill.Rect.localScale = Vector3.zero;
                pill.Rect
                    .DOScale(1f, mOpenDuration)
                    .SetDelay(delay)
                    .SetEase(Ease.OutBack);

                var textRect = (RectTransform)pill.Label.transform;
                textRect.anchoredPosition = new Vector2(0f, 24f);
                textRect
                    .DOAnchorPosY(0f, mOpenDuration * 0.9f)
                    .SetDelay(delay + 0.02f)
                    .SetEase(Ease.OutCubic);

                var color = pill.Label.color;
                color.a = 0f;
                pill.Label.color = color;
                pill.Label
                    .DOFade(1f, mOpenDuration * 0.8f)
                    .SetDelay(delay + 0.02f)
                    .SetEase(Ease.OutCubic);
            }
            else
            {
                var delay = (mPills.Count - i - 1) * 0.025f;
                pill.Label
                    .DOFade(0f, 0.18f)
                    .SetDelay(delay)
                    .SetEase(Ease.InQuad);

                pill.Rect
                    .DOScale(0f, 0.20f)
                    .SetDelay(delay)
                    .SetEase(Ease.InBack);
            }
        }
    }

    private void AnimateMenuIcon(bool open, bool immediate)
    {
        var topY = open ? 0f : 5f;
        var bottomY = open ? 0f : -5f;
        var topRot = open ? 45f : 0f;
        var bottomRot = open ? -45f : 0f;

        mLineTop.DOKill();
        mLineBottom.DOKill();

        if (immediate)
        {
            mLineTop.anchoredPosition = new Vector2(0f, topY);
            mLineTop.localRotation = Quaternion.Euler(0f, 0f, topRot);
            mLineBottom.anchoredPosition = new Vector2(0f, bottomY);
            mLineBottom.localRotation = Quaternion.Euler(0f, 0f, bottomRot);
            return;
        }

        mLineTop.DOAnchorPosY(topY, 0.22f).SetEase(Ease.OutCubic);
        mLineBottom.DOAnchorPosY(bottomY, 0.22f).SetEase(Ease.OutCubic);
        mLineTop.DORotate(new Vector3(0f, 0f, topRot), 0.22f).SetEase(Ease.OutCubic);
        mLineBottom.DORotate(new Vector3(0f, 0f, bottomRot), 0.22f).SetEase(Ease.OutCubic);
    }

    private static void StretchToParent(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private struct PillData
    {
        public RectTransform Rect;
        public Image Image;
        public Text Label;
        public BubbleMenuPillHover Hover;
    }

    private sealed class BubbleMenuPillHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private RectTransform mRect;
        private Image mBackground;
        private Text mLabel;
        private Color mBaseBg;
        private Color mHoverBg;
        private Color mBaseText;
        private Color mHoverText;
        private bool mInteractable;

        public void Initialize(
            RectTransform rect,
            Image background,
            Text label,
            Color baseBg,
            Color hoverBg,
            Color baseText,
            Color hoverText)
        {
            mRect = rect;
            mBackground = background;
            mLabel = label;
            mBaseBg = baseBg;
            mHoverBg = hoverBg;
            mBaseText = baseText;
            mHoverText = hoverText;
        }

        public void SetInteractable(bool value)
        {
            mInteractable = value;
            if (!value)
            {
                ResetVisual();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!mInteractable)
            {
                return;
            }

            mRect.DOKill();
            mBackground.DOKill();
            mLabel.DOKill();

            mRect.DOScale(1.06f, 0.18f).SetEase(Ease.OutCubic);
            mBackground.DOColor(mHoverBg, 0.18f).SetEase(Ease.OutCubic);
            mLabel.DOColor(mHoverText, 0.18f).SetEase(Ease.OutCubic);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!mInteractable)
            {
                return;
            }

            ResetVisual();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!mInteractable)
            {
                return;
            }

            mRect.DOKill();
            mRect.DOScale(0.94f, 0.12f).SetEase(Ease.OutQuad);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!mInteractable)
            {
                return;
            }

            mRect.DOKill();
            mRect.DOScale(1.06f, 0.14f).SetEase(Ease.OutQuad);
        }

        private void ResetVisual()
        {
            if (mRect == null)
            {
                return;
            }

            mRect.DOKill();
            mBackground.DOKill();
            mLabel.DOKill();

            mRect.DOScale(1f, 0.16f).SetEase(Ease.OutCubic);
            mBackground.DOColor(mBaseBg, 0.16f).SetEase(Ease.OutCubic);
            mLabel.DOColor(mBaseText, 0.16f).SetEase(Ease.OutCubic);
        }
    }
}
