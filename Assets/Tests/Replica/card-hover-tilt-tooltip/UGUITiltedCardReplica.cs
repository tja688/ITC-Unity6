using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUITiltedCardReplica : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [SerializeField] private float mRotateAmplitude = 14f;
    [SerializeField] private float mHoverScale = 1.1f;

    private RectTransform mCard;
    private RectTransform mGlint;
    private RectTransform mTooltip;
    private CanvasGroup mTooltipCanvas;
    private Text mTooltipText;
    private Vector2 mLastLocalPoint;
    private bool mPointerInside;
    private float mTargetRotX;
    private float mTargetRotY;
    private float mCurrentRotX;
    private float mCurrentRotY;
    private float mTargetScale = 1f;
    private float mCurrentScale = 1f;
    private float mTargetTooltipRot;
    private float mCurrentTooltipRot;

    private void Awake()
    {
        BuildView();
    }

    private void OnEnable()
    {
        mPointerInside = false;
        mTargetRotX = 0f;
        mTargetRotY = 0f;
        mCurrentRotX = 0f;
        mCurrentRotY = 0f;
        mTargetScale = 1f;
        mCurrentScale = 1f;
        mTargetTooltipRot = 0f;
        mCurrentTooltipRot = 0f;
        mTooltipCanvas.alpha = 0f;
    }

    private void Update()
    {
        var lerp = 1f - Mathf.Exp(-12f * Time.unscaledDeltaTime);
        mCurrentRotX = Mathf.Lerp(mCurrentRotX, mTargetRotX, lerp);
        mCurrentRotY = Mathf.Lerp(mCurrentRotY, mTargetRotY, lerp);
        mCurrentScale = Mathf.Lerp(mCurrentScale, mTargetScale, lerp);
        mCurrentTooltipRot = Mathf.Lerp(mCurrentTooltipRot, mTargetTooltipRot, lerp);

        mCard.localRotation = Quaternion.Euler(mCurrentRotX, mCurrentRotY, 0f);
        mCard.localScale = Vector3.one * mCurrentScale;

        var glintX = Mathf.InverseLerp(-mRotateAmplitude, mRotateAmplitude, mCurrentRotY);
        var glintY = Mathf.InverseLerp(-mRotateAmplitude, mRotateAmplitude, mCurrentRotX);
        mGlint.anchoredPosition = new Vector2((glintX - 0.5f) * 90f, (glintY - 0.5f) * -80f);
        mGlint.localRotation = Quaternion.Euler(0f, 0f, (glintX - 0.5f) * 24f);

        var tooltipAlpha = mPointerInside ? 1f : 0f;
        mTooltipCanvas.alpha = Mathf.Lerp(mTooltipCanvas.alpha, tooltipAlpha, lerp);
        mTooltip.localRotation = Quaternion.Euler(0f, 0f, mCurrentTooltipRot);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mPointerInside = true;
        mTargetScale = mHoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mPointerInside = false;
        mTargetRotX = 0f;
        mTargetRotY = 0f;
        mTargetScale = 1f;
        mTargetTooltipRot = 0f;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(mCard, eventData.position, eventData.pressEventCamera, out var localPoint))
        {
            return;
        }

        var half = mCard.rect.size * 0.5f;
        var normalizedX = Mathf.Clamp(localPoint.x / Mathf.Max(1f, half.x), -1f, 1f);
        var normalizedY = Mathf.Clamp(localPoint.y / Mathf.Max(1f, half.y), -1f, 1f);
        mTargetRotX = -normalizedY * mRotateAmplitude;
        mTargetRotY = normalizedX * mRotateAmplitude;

        var velocityY = localPoint.y - mLastLocalPoint.y;
        mTargetTooltipRot = Mathf.Clamp(-velocityY * 0.38f, -18f, 18f);
        mLastLocalPoint = localPoint;

        mTooltip.anchoredPosition = localPoint + new Vector2(24f, 42f);
        mTooltipText.text = $"x:{normalizedX:0.00} y:{normalizedY:0.00}";
    }

    private void BuildView()
    {
        var root = (RectTransform)transform;
        Stretch(root);

        var rootImage = UGUIReplicaUIFactory.EnsureComponent<Image>(gameObject);
        rootImage.color = new Color(0.04f, 0.06f, 0.11f, 0.96f);
        rootImage.raycastTarget = true;

        var ambientA = UGUIReplicaUIFactory.CreatePanel("AmbientA", root, new Color(0.17f, 0.36f, 0.66f, 0.23f));
        ambientA.anchorMin = new Vector2(0.5f, 0.5f);
        ambientA.anchorMax = new Vector2(0.5f, 0.5f);
        ambientA.pivot = new Vector2(0.5f, 0.5f);
        ambientA.sizeDelta = new Vector2(1380f, 560f);
        ambientA.anchoredPosition = new Vector2(0f, 120f);
        ambientA.localRotation = Quaternion.Euler(0f, 0f, 8f);

        var hint = UGUIReplicaUIFactory.CreateText(
            "Hint",
            root,
            "TiltedCard  |  Move cursor around the card",
            30,
            FontStyle.Bold,
            TextAnchor.UpperCenter,
            new Color(0.93f, 0.96f, 1f, 0.98f));
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(820f, 48f);
        hintRect.anchoredPosition = new Vector2(0f, -38f);

        var frame = UGUIReplicaUIFactory.CreateRect("CardFrame", root);
        frame.anchorMin = new Vector2(0.5f, 0.5f);
        frame.anchorMax = new Vector2(0.5f, 0.5f);
        frame.pivot = new Vector2(0.5f, 0.5f);
        frame.sizeDelta = new Vector2(600f, 420f);
        frame.anchoredPosition = new Vector2(0f, -22f);

        var perspectivePad = UGUIReplicaUIFactory.CreatePanel("PerspectivePad", frame, new Color(0.10f, 0.14f, 0.24f, 0.62f));
        Stretch(perspectivePad);

        mCard = UGUIReplicaUIFactory.CreatePanel("Card", frame, new Color(0.13f, 0.18f, 0.29f, 1f));
        mCard.anchorMin = new Vector2(0.5f, 0.5f);
        mCard.anchorMax = new Vector2(0.5f, 0.5f);
        mCard.pivot = new Vector2(0.5f, 0.5f);
        mCard.sizeDelta = new Vector2(460f, 300f);
        mCard.anchoredPosition = Vector2.zero;
        UGUIReplicaUIFactory.EnsureComponent<Shadow>(mCard.gameObject).effectDistance = new Vector2(0f, -12f);

        var artwork = UGUIReplicaUIFactory.CreatePanel("Artwork", mCard, new Color(0.34f, 0.45f, 0.72f, 0.95f));
        Stretch(artwork);

        var overlayBand = UGUIReplicaUIFactory.CreatePanel("OverlayBand", mCard, new Color(0.05f, 0.08f, 0.15f, 0.35f));
        overlayBand.anchorMin = new Vector2(0f, 0f);
        overlayBand.anchorMax = new Vector2(1f, 0f);
        overlayBand.pivot = new Vector2(0.5f, 0f);
        overlayBand.sizeDelta = new Vector2(0f, 84f);
        overlayBand.anchoredPosition = Vector2.zero;

        var badge = UGUIReplicaUIFactory.CreateText(
            "Badge",
            overlayBand,
            "HOVER TO TILT",
            21,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        Stretch((RectTransform)badge.transform);

        mGlint = UGUIReplicaUIFactory.CreatePanel("Glint", mCard, new Color(1f, 1f, 1f, 0.14f));
        mGlint.anchorMin = new Vector2(0.5f, 0.5f);
        mGlint.anchorMax = new Vector2(0.5f, 0.5f);
        mGlint.pivot = new Vector2(0.5f, 0.5f);
        mGlint.sizeDelta = new Vector2(180f, 360f);
        mGlint.anchoredPosition = Vector2.zero;

        mTooltip = UGUIReplicaUIFactory.CreatePanel("Tooltip", frame, new Color(1f, 1f, 1f, 0.96f));
        mTooltip.anchorMin = new Vector2(0.5f, 0.5f);
        mTooltip.anchorMax = new Vector2(0.5f, 0.5f);
        mTooltip.pivot = new Vector2(0f, 0.5f);
        mTooltip.sizeDelta = new Vector2(150f, 34f);
        mTooltip.anchoredPosition = new Vector2(40f, 80f);
        mTooltipCanvas = UGUIReplicaUIFactory.EnsureComponent<CanvasGroup>(mTooltip.gameObject);
        mTooltipCanvas.alpha = 0f;

        mTooltipText = UGUIReplicaUIFactory.CreateText(
            "TooltipText",
            mTooltip,
            "x:0.00 y:0.00",
            15,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.19f, 0.21f, 0.30f, 1f));
        Stretch((RectTransform)mTooltipText.transform);
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
