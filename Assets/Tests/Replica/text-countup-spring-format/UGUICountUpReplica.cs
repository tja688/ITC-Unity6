using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUICountUpReplica : MonoBehaviour
{
    [SerializeField] private float mFromValue = 0f;
    [SerializeField] private float mInitialTarget = 12840f;
    [SerializeField] private float mDuration = 2f;
    [SerializeField] private float mDelay = 0.25f;
    [SerializeField] private float mCycleInterval = 2.4f;
    [SerializeField] private int mDecimals = 0;
    [SerializeField] private string mSeparator = ",";
    [SerializeField] private bool mCountDown = false;

    private Text mValueLabel;
    private RectTransform mValueRect;
    private float mDisplayedValue;
    private float mCurrentTarget;
    private float mCycleTimer;
    private Tween mDelayTween;
    private Tween mCountTween;

    private void Awake()
    {
        BuildView();
    }

    private void OnEnable()
    {
        mDisplayedValue = mCountDown ? mInitialTarget : mFromValue;
        mCurrentTarget = mInitialTarget;
        mCycleTimer = 0f;
        RefreshLabel();
        StartCount();
    }

    private void OnDisable()
    {
        KillTweens();
    }

    private void Update()
    {
        if (mCountTween != null && mCountTween.IsActive() && mCountTween.IsPlaying())
        {
            return;
        }

        mCycleTimer += Time.unscaledDeltaTime;
        if (mCycleTimer < Mathf.Max(1f, mCycleInterval))
        {
            return;
        }

        mCycleTimer = 0f;
        mCurrentTarget = Random.Range(800f, 99999f);
        StartCount();
    }

    private void BuildView()
    {
        var root = (RectTransform)transform;
        Stretch(root);

        var rootImage = UGUIReplicaUIFactory.EnsureComponent<Image>(gameObject);
        rootImage.color = new Color(0.04f, 0.06f, 0.11f, 0.95f);
        rootImage.raycastTarget = true;

        var ambient = UGUIReplicaUIFactory.CreatePanel("Ambient", root, new Color(0.24f, 0.37f, 0.74f, 0.28f));
        ambient.anchorMin = new Vector2(0.5f, 0.5f);
        ambient.anchorMax = new Vector2(0.5f, 0.5f);
        ambient.pivot = new Vector2(0.5f, 0.5f);
        ambient.sizeDelta = new Vector2(1240f, 540f);
        ambient.anchoredPosition = new Vector2(-90f, 120f);
        ambient.localRotation = Quaternion.Euler(0f, 0f, 10f);

        var panel = UGUIReplicaUIFactory.CreatePanel("Panel", root, new Color(0.10f, 0.13f, 0.22f, 0.95f));
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(980f, 460f);
        panel.anchoredPosition = new Vector2(0f, -20f);
        UGUIReplicaUIFactory.EnsureComponent<Shadow>(panel.gameObject).effectDistance = new Vector2(0f, -10f);

        var title = UGUIReplicaUIFactory.CreateText(
            "Title",
            panel,
            "CountUp  |  spring number interpolation",
            30,
            FontStyle.Bold,
            TextAnchor.UpperCenter,
            new Color(0.93f, 0.96f, 1f, 1f));
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(840f, 56f);
        titleRect.anchoredPosition = new Vector2(0f, -42f);

        var valueWrap = UGUIReplicaUIFactory.CreateRect("ValueWrap", panel);
        valueWrap.anchorMin = new Vector2(0.5f, 0.5f);
        valueWrap.anchorMax = new Vector2(0.5f, 0.5f);
        valueWrap.pivot = new Vector2(0.5f, 0.5f);
        valueWrap.sizeDelta = new Vector2(860f, 190f);
        valueWrap.anchoredPosition = new Vector2(0f, 0f);

        var valueGlow = UGUIReplicaUIFactory.CreatePanel("ValueGlow", valueWrap, new Color(0.24f, 0.33f, 0.74f, 0.35f));
        Stretch(valueGlow);

        mValueLabel = UGUIReplicaUIFactory.CreateText(
            "Value",
            valueWrap,
            "0",
            128,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        mValueRect = (RectTransform)mValueLabel.transform;
        Stretch(mValueRect);

        var hint = UGUIReplicaUIFactory.CreateText(
            "Hint",
            panel,
            "Auto retargeting every few seconds",
            24,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.76f, 0.82f, 0.95f, 0.95f));
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 0f);
        hintRect.anchorMax = new Vector2(0.5f, 0f);
        hintRect.pivot = new Vector2(0.5f, 0f);
        hintRect.sizeDelta = new Vector2(640f, 44f);
        hintRect.anchoredPosition = new Vector2(0f, 46f);
    }

    private void StartCount()
    {
        KillTweens();
        var from = mDisplayedValue;
        var to = mCountDown ? mFromValue : mCurrentTarget;
        if (!mCountDown && Mathf.Abs(from - to) < 0.01f)
        {
            from = mFromValue;
            mDisplayedValue = from;
            RefreshLabel();
        }

        mDelayTween = DOVirtual.DelayedCall(mDelay, () =>
        {
            mCountTween = DOTween
                .To(() => from, value =>
                {
                    mDisplayedValue = value;
                    from = value;
                    RefreshLabel();
                }, to, Mathf.Max(0.2f, mDuration))
                .SetEase(Ease.OutCubic)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    mDisplayedValue = to;
                    RefreshLabel();
                    mValueRect.DOPunchScale(new Vector3(0.06f, 0.06f, 0f), 0.26f, 4, 0.5f).SetUpdate(true);
                    if (mCountDown)
                    {
                        mFromValue = Random.Range(30000f, 98000f);
                    }
                });
        }, true).SetUpdate(true);
    }

    private void RefreshLabel()
    {
        mValueLabel.text = FormatValue(mDisplayedValue);
    }

    private string FormatValue(float value)
    {
        var rounded = mDecimals <= 0 ? Mathf.RoundToInt(value).ToString() : value.ToString($"F{mDecimals}");
        if (string.IsNullOrEmpty(mSeparator))
        {
            return rounded;
        }

        var sign = "";
        var number = rounded;
        if (number.StartsWith("-"))
        {
            sign = "-";
            number = number.Substring(1);
        }

        var dotIndex = number.IndexOf('.');
        var integerPart = dotIndex >= 0 ? number.Substring(0, dotIndex) : number;
        var fractionPart = dotIndex >= 0 ? number.Substring(dotIndex) : string.Empty;

        for (var i = integerPart.Length - 3; i > 0; i -= 3)
        {
            integerPart = integerPart.Insert(i, mSeparator);
        }

        return sign + integerPart + fractionPart;
    }

    private void KillTweens()
    {
        mDelayTween?.Kill();
        mCountTween?.Kill();
        mDelayTween = null;
        mCountTween = null;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
