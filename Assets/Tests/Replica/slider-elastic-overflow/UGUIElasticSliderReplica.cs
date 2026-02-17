using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UGUIElasticSliderReplica : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private float mDefaultValue = 50f;
    [SerializeField] private float mStartingValue = 0f;
    [SerializeField] private float mMaxValue = 100f;
    [SerializeField] private bool mStepped = false;
    [SerializeField] private float mStepSize = 1f;
    [SerializeField] private float mMaxOverflow = 50f;

    private RectTransform mRootRect;
    private RectTransform mTrackRect;
    private RectTransform mTrackWrapper;
    private RectTransform mFillRect;
    private RectTransform mKnobRect;
    private RectTransform mLeftIconRect;
    private RectTransform mRightIconRect;

    private Text mValueText;
    private Image mTrackBackground;
    private Image mFillImage;
    private Image mKnobImage;

    private float mValue;
    private float mOverflow;
    private float mHoverScale = 1f;
    // Track height animates between thin (idle) and thicker (hover), matching React's height transform
    private float mTrackHeight;

    private bool mIsHovering;
    private bool mIsDragging;

    private OverflowRegion mRegion = OverflowRegion.Middle;
    private readonly Vector3[] mTrackCorners = new Vector3[4];

    // Spring state for overflow bounce-back (replaces Lerp with spring physics)
    private float mOverflowVelocity;

    // Left/right icon smooth positions
    private float mLeftIconTargetX;
    private float mRightIconTargetX;

    private enum OverflowRegion
    {
        Left,
        Middle,
        Right
    }

    private const float kTrackHeightIdle = 6f;
    private const float kTrackHeightHover = 14f;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("UGUIElasticSliderReplica requires RectTransform.", this);
            return;
        }

        BuildView();
        mValue = Mathf.Clamp(mDefaultValue, mStartingValue, mMaxValue);
        mTrackHeight = kTrackHeightIdle;
        RefreshValueVisual();
    }

    private void OnEnable()
    {
        mIsDragging = false;
        mIsHovering = false;
        mOverflow = 0f;
        mOverflowVelocity = 0f;
        mHoverScale = 1f;
        mTrackHeight = kTrackHeightIdle;
        mRegion = OverflowRegion.Middle;
        mValue = Mathf.Clamp(mDefaultValue, mStartingValue, mMaxValue);
        mLeftIconTargetX = 0f;
        mRightIconTargetX = 0f;
        RefreshValueVisual();
    }

    private void Update()
    {
        var dt = Time.unscaledDeltaTime;

        // Spring-based overflow return (matching React's spring with bounce: 0.5)
        if (!mIsDragging)
        {
            var spring = 180f; // stiffness
            var damping = 12f;
            var force = -spring * mOverflow - damping * mOverflowVelocity;
            mOverflowVelocity += force * dt;
            mOverflow += mOverflowVelocity * dt;

            if (Mathf.Abs(mOverflow) < 0.1f && Mathf.Abs(mOverflowVelocity) < 0.5f)
            {
                mOverflow = 0f;
                mOverflowVelocity = 0f;
            }
        }

        // Smooth hover scale (frame-rate independent exponential)
        var targetHoverScale = (mIsHovering || mIsDragging) ? 1.2f : 1f;
        mHoverScale = SmoothDamp(mHoverScale, targetHoverScale, 10f, dt);

        // Smooth track height
        var targetHeight = (mIsHovering || mIsDragging) ? kTrackHeightHover : kTrackHeightIdle;
        mTrackHeight = SmoothDamp(mTrackHeight, targetHeight, 10f, dt);

        // Smooth icon positions
        float leftTarget, rightTarget;
        var overflowAbs = Mathf.Abs(mOverflow);
        if (mRegion == OverflowRegion.Left)
        {
            leftTarget = -(overflowAbs / Mathf.Max(1f, mHoverScale));
            rightTarget = 0f;
        }
        else if (mRegion == OverflowRegion.Right)
        {
            leftTarget = 0f;
            rightTarget = overflowAbs / Mathf.Max(1f, mHoverScale);
        }
        else
        {
            leftTarget = 0f;
            rightTarget = 0f;
        }

        mLeftIconTargetX = SmoothDamp(mLeftIconTargetX, leftTarget, 15f, dt);
        mRightIconTargetX = SmoothDamp(mRightIconTargetX, rightTarget, 15f, dt);

        ApplyElasticVisual();
    }

    private void OnDisable()
    {
        mLeftIconRect?.DOKill();
        mRightIconRect?.DOKill();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mIsHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!mIsDragging)
        {
            mIsHovering = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mIsDragging = true;
        mIsHovering = true;
        mOverflowVelocity = 0f; // Reset spring velocity on new drag
        UpdateFromPointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!mIsDragging)
        {
            return;
        }

        UpdateFromPointer(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mIsDragging = false;
        // Spring will handle the bounce-back in Update()
    }

    private void BuildView()
    {
        mRootRect.anchorMin = Vector2.zero;
        mRootRect.anchorMax = Vector2.one;
        mRootRect.offsetMin = Vector2.zero;
        mRootRect.offsetMax = Vector2.zero;

        var raycastImage = UGUIReplicaUIFactory.EnsureComponent<Image>(gameObject);
        raycastImage.color = new Color(1f, 1f, 1f, 0.001f);
        raycastImage.raycastTarget = true;

        var backdrop = UGUIReplicaUIFactory.CreatePanel("Backdrop", mRootRect, new Color(0.06f, 0.08f, 0.14f, 0.90f));
        Stretch(backdrop);

        var hint = UGUIReplicaUIFactory.CreateText(
            "Hint",
            backdrop,
            "ElasticSlider  |  Drag and over-pull both sides",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.93f, 0.96f, 1f, 0.95f));
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(980f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -42f);

        var frame = UGUIReplicaUIFactory.CreatePanel("Frame", backdrop, new Color(0.09f, 0.12f, 0.20f, 0.92f));
        frame.anchorMin = new Vector2(0.5f, 0.5f);
        frame.anchorMax = new Vector2(0.5f, 0.5f);
        frame.pivot = new Vector2(0.5f, 0.5f);
        frame.sizeDelta = new Vector2(900f, 380f);
        frame.anchoredPosition = new Vector2(0f, -16f);

        var frameShadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(frame.gameObject);
        frameShadow.effectColor = new Color(0f, 0f, 0f, 0.32f);
        frameShadow.effectDistance = new Vector2(0f, -8f);

        // Slider row: icon – track – icon
        var row = UGUIReplicaUIFactory.CreateRect("SliderRow", frame);
        row.anchorMin = new Vector2(0.5f, 0.5f);
        row.anchorMax = new Vector2(0.5f, 0.5f);
        row.pivot = new Vector2(0.5f, 0.5f);
        row.sizeDelta = new Vector2(760f, 120f);
        row.anchoredPosition = new Vector2(0f, 12f);

        mLeftIconRect = UGUIReplicaUIFactory.CreateRect("LeftIcon", row);
        mLeftIconRect.anchorMin = new Vector2(0f, 0.5f);
        mLeftIconRect.anchorMax = new Vector2(0f, 0.5f);
        mLeftIconRect.pivot = new Vector2(0.5f, 0.5f);
        mLeftIconRect.sizeDelta = new Vector2(48f, 48f);
        mLeftIconRect.anchoredPosition = new Vector2(28f, 0f);
        var leftIconText = UGUIReplicaUIFactory.CreateText(
            "Glyph",
            mLeftIconRect,
            "−",
            36,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.76f, 0.82f, 0.92f, 0.92f));
        Stretch((RectTransform)leftIconText.transform);

        // Track container sits between the icons
        var sliderContainer = UGUIReplicaUIFactory.CreateRect("SliderContainer", row);
        sliderContainer.anchorMin = new Vector2(0f, 0.5f);
        sliderContainer.anchorMax = new Vector2(1f, 0.5f);
        sliderContainer.pivot = new Vector2(0.5f, 0.5f);
        sliderContainer.offsetMin = new Vector2(72f, -40f);
        sliderContainer.offsetMax = new Vector2(-72f, 40f);

        mTrackRect = UGUIReplicaUIFactory.CreateRect("TrackRoot", sliderContainer);
        mTrackRect.anchorMin = new Vector2(0f, 0.5f);
        mTrackRect.anchorMax = new Vector2(1f, 0.5f);
        mTrackRect.pivot = new Vector2(0.5f, 0.5f);
        mTrackRect.sizeDelta = new Vector2(0f, kTrackHeightIdle);
        mTrackRect.anchoredPosition = Vector2.zero;

        // TrackWrapper: this is what gets scaleX/scaleY for elastic effect
        mTrackWrapper = UGUIReplicaUIFactory.CreateRect("TrackWrapper", mTrackRect);
        Stretch(mTrackWrapper);

        var track = UGUIReplicaUIFactory.CreatePanel("Track", mTrackWrapper, new Color(0.58f, 0.62f, 0.72f, 0.28f));
        Stretch(track);
        mTrackBackground = track.GetComponent<Image>();

        mFillRect = UGUIReplicaUIFactory.CreatePanel("Fill", track, new Color(0.74f, 0.78f, 0.88f, 0.96f));
        mFillRect.anchorMin = new Vector2(0f, 0f);
        mFillRect.anchorMax = new Vector2(0.5f, 1f);
        mFillRect.offsetMin = Vector2.zero;
        mFillRect.offsetMax = Vector2.zero;
        mFillImage = mFillRect.GetComponent<Image>();

        // Knob – small circle on the track
        mKnobRect = UGUIReplicaUIFactory.CreatePanel("Knob", mTrackRect, Color.white);
        mKnobRect.anchorMin = new Vector2(0.5f, 0.5f);
        mKnobRect.anchorMax = new Vector2(0.5f, 0.5f);
        mKnobRect.pivot = new Vector2(0.5f, 0.5f);
        mKnobRect.sizeDelta = new Vector2(20f, 20f);
        mKnobRect.anchoredPosition = Vector2.zero;
        mKnobImage = mKnobRect.GetComponent<Image>();
        var knobOutline = UGUIReplicaUIFactory.EnsureComponent<Outline>(mKnobRect.gameObject);
        knobOutline.effectColor = new Color(0f, 0f, 0f, 0.25f);
        knobOutline.effectDistance = new Vector2(1f, -1f);

        mRightIconRect = UGUIReplicaUIFactory.CreateRect("RightIcon", row);
        mRightIconRect.anchorMin = new Vector2(1f, 0.5f);
        mRightIconRect.anchorMax = new Vector2(1f, 0.5f);
        mRightIconRect.pivot = new Vector2(0.5f, 0.5f);
        mRightIconRect.sizeDelta = new Vector2(48f, 48f);
        mRightIconRect.anchoredPosition = new Vector2(-28f, 0f);
        var rightIconText = UGUIReplicaUIFactory.CreateText(
            "Glyph",
            mRightIconRect,
            "+",
            36,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.76f, 0.82f, 0.92f, 0.92f));
        Stretch((RectTransform)rightIconText.transform);

        mValueText = UGUIReplicaUIFactory.CreateText(
            "Value",
            frame,
            "50",
            26,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.84f, 0.90f, 1f, 0.92f));
        var valueRect = (RectTransform)mValueText.transform;
        valueRect.anchorMin = new Vector2(0.5f, 0f);
        valueRect.anchorMax = new Vector2(0.5f, 0f);
        valueRect.pivot = new Vector2(0.5f, 0f);
        valueRect.sizeDelta = new Vector2(120f, 42f);
        valueRect.anchoredPosition = new Vector2(0f, 34f);
    }

    private void UpdateFromPointer(PointerEventData eventData)
    {
        if (mTrackRect == null)
        {
            return;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(mTrackRect, eventData.position, eventData.pressEventCamera, out var localPoint))
        {
            var width = Mathf.Max(1f, mTrackRect.rect.width);
            var normalized = Mathf.Clamp01((localPoint.x + (width * 0.5f)) / width);
            var newValue = Mathf.Lerp(mStartingValue, mMaxValue, normalized);

            if (mStepped)
            {
                var step = Mathf.Max(0.0001f, mStepSize);
                newValue = Mathf.Round(newValue / step) * step;
            }

            mValue = Mathf.Clamp(newValue, mStartingValue, mMaxValue);
        }

        mTrackRect.GetWorldCorners(mTrackCorners);
        var left = mTrackCorners[0].x;
        var right = mTrackCorners[3].x;

        OverflowRegion nextRegion;
        float rawOverflow;
        if (eventData.position.x < left)
        {
            nextRegion = OverflowRegion.Left;
            rawOverflow = left - eventData.position.x;
        }
        else if (eventData.position.x > right)
        {
            nextRegion = OverflowRegion.Right;
            rawOverflow = eventData.position.x - right;
        }
        else
        {
            nextRegion = OverflowRegion.Middle;
            rawOverflow = 0f;
        }

        if (nextRegion != mRegion)
        {
            mRegion = nextRegion;
            PlayRegionPulse(nextRegion);
        }

        mOverflow = Decay(rawOverflow, mMaxOverflow);
        mOverflowVelocity = 0f; // Reset velocity during active drag
        RefreshValueVisual();
    }

    private void RefreshValueVisual()
    {
        var normalized = GetValueNormalized();
        mFillRect.anchorMax = new Vector2(normalized, 1f);
        mValueText.text = Mathf.RoundToInt(mValue).ToString();

        mKnobRect.anchorMin = new Vector2(normalized, 0.5f);
        mKnobRect.anchorMax = new Vector2(normalized, 0.5f);
        mKnobRect.anchoredPosition = Vector2.zero;
    }

    private void ApplyElasticVisual()
    {
        var width = Mathf.Max(1f, mTrackRect.rect.width);
        var overflowAbs = Mathf.Abs(mOverflow);
        var overflow01 = Mathf.Clamp01(overflowAbs / Mathf.Max(1f, mMaxOverflow));

        // Animate track height smoothly
        mTrackRect.sizeDelta = new Vector2(0f, mTrackHeight);

        // Scale X stretches with overflow, scale Y squishes slightly (React: scaleY [1, 0.8])
        var scaleX = 1f + (overflowAbs / width);
        var scaleY = Mathf.Lerp(1f, 0.82f, overflow01);
        mTrackWrapper.localScale = new Vector3(scaleX, scaleY, 1f);

        // Transform origin follows pointer side (React: clientX < center ? 'right' : 'left')
        switch (mRegion)
        {
            case OverflowRegion.Left:
                mTrackWrapper.pivot = new Vector2(1f, 0.5f);
                break;
            case OverflowRegion.Right:
                mTrackWrapper.pivot = new Vector2(0f, 0.5f);
                break;
            default:
                mTrackWrapper.pivot = new Vector2(0.5f, 0.5f);
                break;
        }

        // Icon positions push outward based on overflow
        mLeftIconRect.anchoredPosition = new Vector2(28f + mLeftIconTargetX, 0f);
        mRightIconRect.anchoredPosition = new Vector2(-28f + mRightIconTargetX, 0f);

        // Subtle glow on overflow
        var glow = Mathf.Lerp(0.28f, 0.46f, overflow01);
        mTrackBackground.color = new Color(0.58f, 0.62f, 0.72f, glow);
        mFillImage.color = Color.Lerp(new Color(0.74f, 0.78f, 0.88f, 0.86f), Color.white, overflow01 * 0.5f);
        mKnobImage.color = Color.Lerp(new Color(0.93f, 0.95f, 1f, 1f), Color.white, overflow01);

        // Knob scales with hover, matching track
        var knobSize = Mathf.Lerp(14f, 20f, Mathf.InverseLerp(1f, 1.2f, mHoverScale));
        mKnobRect.sizeDelta = new Vector2(knobSize, knobSize);
    }

    private void PlayRegionPulse(OverflowRegion region)
    {
        if (region == OverflowRegion.Left)
        {
            mLeftIconRect.DOKill();
            mLeftIconRect.DOPunchScale(Vector3.one * 0.3f, 0.25f, 1, 0.5f);
        }
        else if (region == OverflowRegion.Right)
        {
            mRightIconRect.DOKill();
            mRightIconRect.DOPunchScale(Vector3.one * 0.3f, 0.25f, 1, 0.5f);
        }
    }

    private float GetValueNormalized()
    {
        var total = mMaxValue - mStartingValue;
        if (Mathf.Approximately(total, 0f))
        {
            return 0f;
        }

        return Mathf.Clamp01((mValue - mStartingValue) / total);
    }

    private static float Decay(float value, float max)
    {
        if (max <= 0f)
        {
            return 0f;
        }

        var entry = value / max;
        var sigmoid = 2f * ((1f / (1f + Mathf.Exp(-entry))) - 0.5f);
        return sigmoid * max;
    }

    /// <summary>
    /// Frame-rate independent exponential smoothing (replaces Time.deltaTime * factor lerp).
    /// </summary>
    private static float SmoothDamp(float current, float target, float speed, float dt)
    {
        return Mathf.Lerp(current, target, 1f - Mathf.Exp(-speed * dt));
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
