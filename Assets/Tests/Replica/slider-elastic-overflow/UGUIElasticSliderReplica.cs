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

    private bool mIsHovering;
    private bool mIsDragging;

    private OverflowRegion mRegion = OverflowRegion.Middle;
    private readonly Vector3[] mTrackCorners = new Vector3[4];

    private enum OverflowRegion
    {
        Left,
        Middle,
        Right
    }

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("UGUIElasticSliderReplica requires RectTransform.", this);
            return;
        }

        BuildView();
        mValue = Mathf.Clamp(mDefaultValue, mStartingValue, mMaxValue);
        RefreshValueVisual();
    }

    private void OnEnable()
    {
        mIsDragging = false;
        mIsHovering = false;
        mOverflow = 0f;
        mHoverScale = 1f;
        mRegion = OverflowRegion.Middle;
        mValue = Mathf.Clamp(mDefaultValue, mStartingValue, mMaxValue);
        RefreshValueVisual();
    }

    private void Update()
    {
        if (!mIsDragging)
        {
            mOverflow = Mathf.Lerp(mOverflow, 0f, Time.unscaledDeltaTime * 7f);
        }

        var targetHoverScale = (mIsHovering || mIsDragging) ? 1.2f : 1f;
        mHoverScale = Mathf.Lerp(mHoverScale, targetHoverScale, Time.unscaledDeltaTime * 9f);
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
        mLeftIconRect.sizeDelta = new Vector2(64f, 64f);
        mLeftIconRect.anchoredPosition = new Vector2(38f, 0f);
        var leftIconText = UGUIReplicaUIFactory.CreateText(
            "Glyph",
            mLeftIconRect,
            "-",
            54,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.76f, 0.82f, 0.92f, 0.92f));
        Stretch((RectTransform)leftIconText.transform);

        var sliderContainer = UGUIReplicaUIFactory.CreateRect("SliderContainer", row);
        sliderContainer.anchorMin = new Vector2(0f, 0.5f);
        sliderContainer.anchorMax = new Vector2(1f, 0.5f);
        sliderContainer.pivot = new Vector2(0.5f, 0.5f);
        sliderContainer.offsetMin = new Vector2(100f, -40f);
        sliderContainer.offsetMax = new Vector2(-100f, 40f);

        mTrackRect = UGUIReplicaUIFactory.CreateRect("TrackRoot", sliderContainer);
        mTrackRect.anchorMin = new Vector2(0f, 0.5f);
        mTrackRect.anchorMax = new Vector2(1f, 0.5f);
        mTrackRect.pivot = new Vector2(0.5f, 0.5f);
        mTrackRect.sizeDelta = new Vector2(0f, 26f);
        mTrackRect.anchoredPosition = Vector2.zero;

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

        mKnobRect = UGUIReplicaUIFactory.CreatePanel("Knob", mTrackRect, Color.white);
        mKnobRect.anchorMin = new Vector2(0.5f, 0.5f);
        mKnobRect.anchorMax = new Vector2(0.5f, 0.5f);
        mKnobRect.pivot = new Vector2(0.5f, 0.5f);
        mKnobRect.sizeDelta = new Vector2(26f, 26f);
        mKnobRect.anchoredPosition = Vector2.zero;
        mKnobImage = mKnobRect.GetComponent<Image>();
        var knobOutline = UGUIReplicaUIFactory.EnsureComponent<Outline>(mKnobRect.gameObject);
        knobOutline.effectColor = new Color(0f, 0f, 0f, 0.35f);
        knobOutline.effectDistance = new Vector2(1.2f, -1.2f);

        mRightIconRect = UGUIReplicaUIFactory.CreateRect("RightIcon", row);
        mRightIconRect.anchorMin = new Vector2(1f, 0.5f);
        mRightIconRect.anchorMax = new Vector2(1f, 0.5f);
        mRightIconRect.pivot = new Vector2(0.5f, 0.5f);
        mRightIconRect.sizeDelta = new Vector2(64f, 64f);
        mRightIconRect.anchoredPosition = new Vector2(-38f, 0f);
        var rightIconText = UGUIReplicaUIFactory.CreateText(
            "Glyph",
            mRightIconRect,
            "+",
            54,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.76f, 0.82f, 0.92f, 0.92f));
        Stretch((RectTransform)rightIconText.transform);

        mValueText = UGUIReplicaUIFactory.CreateText(
            "Value",
            frame,
            "50",
            30,
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

        var scaleX = (1f + (overflowAbs / width)) * mHoverScale;
        var scaleY = Mathf.Lerp(1f, 0.80f, overflow01) * mHoverScale;
        mTrackWrapper.localScale = new Vector3(scaleX, scaleY, 1f);

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

        var leftTarget = (mRegion == OverflowRegion.Left) ? -(overflowAbs / Mathf.Max(1f, mHoverScale)) : 0f;
        var rightTarget = (mRegion == OverflowRegion.Right) ? (overflowAbs / Mathf.Max(1f, mHoverScale)) : 0f;

        mLeftIconRect.anchoredPosition = Vector2.Lerp(
            mLeftIconRect.anchoredPosition,
            new Vector2(38f + leftTarget, 0f),
            Time.unscaledDeltaTime * 15f);
        mRightIconRect.anchoredPosition = Vector2.Lerp(
            mRightIconRect.anchoredPosition,
            new Vector2(-38f + rightTarget, 0f),
            Time.unscaledDeltaTime * 15f);

        var glow = Mathf.Lerp(0.28f, 0.46f, overflow01);
        mTrackBackground.color = new Color(0.58f, 0.62f, 0.72f, glow);
        mFillImage.color = Color.Lerp(new Color(0.74f, 0.78f, 0.88f, 0.86f), Color.white, overflow01 * 0.5f);
        mKnobImage.color = Color.Lerp(new Color(0.93f, 0.95f, 1f, 1f), Color.white, overflow01);
    }

    private void PlayRegionPulse(OverflowRegion region)
    {
        if (region == OverflowRegion.Left)
        {
            mLeftIconRect.DOKill();
            mLeftIconRect.DOPunchScale(Vector3.one * 0.14f, 0.22f, 1, 0.4f);
        }
        else if (region == OverflowRegion.Right)
        {
            mRightIconRect.DOKill();
            mRightIconRect.DOPunchScale(Vector3.one * 0.14f, 0.22f, 1, 0.4f);
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

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
