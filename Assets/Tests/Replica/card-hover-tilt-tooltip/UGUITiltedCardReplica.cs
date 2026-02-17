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
    private RectTransform mOverlayBand;
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

    // Spring parameters matching React: damping: 30, stiffness: 100, mass: 2
    private const float kSpringStiffness = 100f;
    private const float kSpringDamping = 30f;
    private const float kSpringMass = 2f;

    // Spring velocities for physical spring behavior
    private float mVelRotX;
    private float mVelRotY;
    private float mVelScale;

    // Overlay band smooth Y offset for subtle parallax
    private float mOverlayTargetY;
    private float mOverlayCurrentY;

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
        mVelRotX = 0f;
        mVelRotY = 0f;
        mVelScale = 0f;
        mOverlayTargetY = 0f;
        mOverlayCurrentY = 0f;
        mTooltipCanvas.alpha = 0f;
    }

    private void Update()
    {
        var dt = Time.unscaledDeltaTime;

        // Spring physics for rotation (matches React's useSpring with stiffness:100, damping:30, mass:2)
        SpringStep(ref mCurrentRotX, ref mVelRotX, mTargetRotX, kSpringStiffness, kSpringDamping, kSpringMass, dt);
        SpringStep(ref mCurrentRotY, ref mVelRotY, mTargetRotY, kSpringStiffness, kSpringDamping, kSpringMass, dt);

        // Scale spring (same params)
        SpringStep(ref mCurrentScale, ref mVelScale, mTargetScale, kSpringStiffness, kSpringDamping, kSpringMass, dt);

        // Tooltip rotation: stiffer spring (React: stiffness:350, damping:30, mass:1)
        var tooltipLerp = 1f - Mathf.Exp(-18f * dt);
        mCurrentTooltipRot = Mathf.Lerp(mCurrentTooltipRot, mTargetTooltipRot, tooltipLerp);

        // Overlay band parallax
        var overlayLerp = 1f - Mathf.Exp(-8f * dt);
        mOverlayCurrentY = Mathf.Lerp(mOverlayCurrentY, mOverlayTargetY, overlayLerp);

        // Apply card rotation and scale
        mCard.localRotation = Quaternion.Euler(mCurrentRotX, mCurrentRotY, 0f);
        mCard.localScale = Vector3.one * mCurrentScale;

        // Glint follows rotation for highlight effect
        var glintX = Mathf.InverseLerp(-mRotateAmplitude, mRotateAmplitude, mCurrentRotY);
        var glintY = Mathf.InverseLerp(-mRotateAmplitude, mRotateAmplitude, mCurrentRotX);
        mGlint.anchoredPosition = new Vector2((glintX - 0.5f) * 120f, (glintY - 0.5f) * -100f);
        mGlint.localRotation = Quaternion.Euler(0f, 0f, (glintX - 0.5f) * 24f);

        // Overlay band follows tilt with subtle parallax (linked to card rotation)
        var overlayShiftX = mCurrentRotY * 0.4f;
        var overlayShiftY = mOverlayCurrentY + mCurrentRotX * 0.3f;
        mOverlayBand.anchoredPosition = new Vector2(overlayShiftX, overlayShiftY);

        // Tooltip alpha fades with spring
        var alphaDamp = 1f - Mathf.Exp(-10f * dt);
        var tooltipAlpha = mPointerInside ? 1f : 0f;
        mTooltipCanvas.alpha = Mathf.Lerp(mTooltipCanvas.alpha, tooltipAlpha, alphaDamp);
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
        mOverlayTargetY = 0f;
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

        // Overlay parallax tracks vertical position
        mOverlayTargetY = normalizedY * 6f;

        var velocityY = localPoint.y - mLastLocalPoint.y;
        mTargetTooltipRot = Mathf.Clamp(-velocityY * 0.6f, -18f, 18f);
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

        // Ambient background shape
        var ambientA = UGUIReplicaUIFactory.CreatePanel("AmbientA", root, new Color(0.17f, 0.36f, 0.66f, 0.18f));
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

        // Card frame: this provides the "perspective" container like CSS perspective: 800px
        // The frame is larger than the card so the tooltip can overflow outside the card area
        var frame = UGUIReplicaUIFactory.CreateRect("CardFrame", root);
        frame.anchorMin = new Vector2(0.5f, 0.5f);
        frame.anchorMax = new Vector2(0.5f, 0.5f);
        frame.pivot = new Vector2(0.5f, 0.5f);
        frame.sizeDelta = new Vector2(560f, 400f);
        frame.anchoredPosition = new Vector2(0f, -22f);

        // The card itself with shadow for depth
        mCard = UGUIReplicaUIFactory.CreatePanel("Card", frame, new Color(0.13f, 0.18f, 0.29f, 1f));
        mCard.anchorMin = new Vector2(0.5f, 0.5f);
        mCard.anchorMax = new Vector2(0.5f, 0.5f);
        mCard.pivot = new Vector2(0.5f, 0.5f);
        mCard.sizeDelta = new Vector2(420f, 280f);
        mCard.anchoredPosition = Vector2.zero;
        var cardShadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(mCard.gameObject);
        cardShadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
        cardShadow.effectDistance = new Vector2(0f, -12f);

        // Artwork fill
        var artwork = UGUIReplicaUIFactory.CreatePanel("Artwork", mCard, new Color(0.34f, 0.45f, 0.72f, 0.95f));
        Stretch(artwork);

        // Overlay band at bottom — moves subtly with the card tilt for spatial feel
        mOverlayBand = UGUIReplicaUIFactory.CreatePanel("OverlayBand", mCard, new Color(0.05f, 0.08f, 0.15f, 0.50f));
        mOverlayBand.anchorMin = new Vector2(0f, 0f);
        mOverlayBand.anchorMax = new Vector2(1f, 0f);
        mOverlayBand.pivot = new Vector2(0.5f, 0f);
        mOverlayBand.sizeDelta = new Vector2(0f, 72f);
        mOverlayBand.anchoredPosition = Vector2.zero;

        var badge = UGUIReplicaUIFactory.CreateText(
            "Badge",
            mOverlayBand,
            "HOVER TO TILT",
            18,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(1f, 1f, 1f, 0.9f));
        Stretch((RectTransform)badge.transform);

        // Glint highlight that follows the tilt direction
        mGlint = UGUIReplicaUIFactory.CreatePanel("Glint", mCard, new Color(1f, 1f, 1f, 0.10f));
        mGlint.anchorMin = new Vector2(0.5f, 0.5f);
        mGlint.anchorMax = new Vector2(0.5f, 0.5f);
        mGlint.pivot = new Vector2(0.5f, 0.5f);
        mGlint.sizeDelta = new Vector2(160f, 340f);
        mGlint.anchoredPosition = Vector2.zero;

        // Tooltip that follows mouse position
        mTooltip = UGUIReplicaUIFactory.CreatePanel("Tooltip", frame, new Color(1f, 1f, 1f, 0.96f));
        mTooltip.anchorMin = new Vector2(0.5f, 0.5f);
        mTooltip.anchorMax = new Vector2(0.5f, 0.5f);
        mTooltip.pivot = new Vector2(0f, 0.5f);
        mTooltip.sizeDelta = new Vector2(140f, 30f);
        mTooltip.anchoredPosition = new Vector2(40f, 80f);
        mTooltipCanvas = UGUIReplicaUIFactory.EnsureComponent<CanvasGroup>(mTooltip.gameObject);
        mTooltipCanvas.alpha = 0f;
        mTooltipCanvas.blocksRaycasts = false;

        mTooltipText = UGUIReplicaUIFactory.CreateText(
            "TooltipText",
            mTooltip,
            "x:0.00 y:0.00",
            13,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.19f, 0.21f, 0.30f, 1f));
        Stretch((RectTransform)mTooltipText.transform);
    }

    /// <summary>
    /// Semi-implicit Euler spring step matching Framer Motion's spring physics.
    /// </summary>
    private static void SpringStep(ref float pos, ref float vel, float target, float stiffness, float damping, float mass, float dt)
    {
        var displacement = pos - target;
        var springForce = -stiffness * displacement;
        var dampForce = -damping * vel;
        var accel = (springForce + dampForce) / mass;
        vel += accel * dt;
        pos += vel * dt;

        // Snap when close enough to avoid jitter
        if (Mathf.Abs(displacement) < 0.001f && Mathf.Abs(vel) < 0.01f)
        {
            pos = target;
            vel = 0f;
        }
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
