using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUICounterReplica : MonoBehaviour
{
    [SerializeField] private int mStartValue = 17284;
    [SerializeField] private int mMinTarget = 800;
    [SerializeField] private int mMaxTarget = 99999;
    [SerializeField] private float mChangeInterval = 2.3f;

    private readonly List<DigitColumn> mColumns = new();
    private RectTransform mCounterRect;
    private float mDisplayedValue;
    private float mTargetTimer;
    private Tween mTween;

    private static readonly int[] sPlaces = { 10000, 1000, 100, 10, 1 };

    private void Awake()
    {
        BuildView();
        mDisplayedValue = mStartValue;
        RefreshDigits();
    }

    private void OnEnable()
    {
        mTween?.Kill();
        mTargetTimer = 0f;
        mDisplayedValue = mStartValue;
        RefreshDigits();
    }

    private void Update()
    {
        mTargetTimer += Time.unscaledDeltaTime;
        if (mTargetTimer < Mathf.Max(1f, mChangeInterval))
        {
            return;
        }

        mTargetTimer = 0f;
        AnimateTo(Random.Range(mMinTarget, mMaxTarget + 1));
    }

    private void OnDisable()
    {
        mTween?.Kill();
    }

    private void BuildView()
    {
        var root = (RectTransform)transform;
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;

        var backdrop = UGUIReplicaUIFactory.CreatePanel("Backdrop", root, new Color(0.05f, 0.07f, 0.12f, 0.88f));
        Stretch(backdrop);

        var hint = UGUIReplicaUIFactory.CreateText(
            "Hint",
            backdrop,
            "Counter  |  Digits roll with spring transitions",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.93f, 0.96f, 1f, 0.95f));
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(920f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -44f);

        var panel = UGUIReplicaUIFactory.CreatePanel("CounterPanel", backdrop, new Color(0.10f, 0.13f, 0.22f, 0.94f));
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.sizeDelta = new Vector2(980f, 380f);
        panel.anchoredPosition = new Vector2(0f, -20f);

        var panelShadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(panel.gameObject);
        panelShadow.effectColor = new Color(0f, 0f, 0f, 0.36f);
        panelShadow.effectDistance = new Vector2(0f, -8f);

        mCounterRect = UGUIReplicaUIFactory.CreateRect("Counter", panel);
        mCounterRect.anchorMin = new Vector2(0.5f, 0.5f);
        mCounterRect.anchorMax = new Vector2(0.5f, 0.5f);
        mCounterRect.pivot = new Vector2(0.5f, 0.5f);
        mCounterRect.sizeDelta = new Vector2(760f, 180f);
        mCounterRect.anchoredPosition = new Vector2(0f, 0f);

        var row = UGUIReplicaUIFactory.EnsureComponent<HorizontalLayoutGroup>(mCounterRect.gameObject);
        row.childAlignment = TextAnchor.MiddleCenter;
        row.childControlHeight = false;
        row.childControlWidth = false;
        row.childForceExpandHeight = false;
        row.childForceExpandWidth = false;
        row.spacing = 12f;

        mColumns.Clear();
        foreach (var place in sPlaces)
        {
            mColumns.Add(CreateDigitColumn(place));
        }

        var gradientOverlay = UGUIReplicaUIFactory.CreateRect("Gradients", panel);
        gradientOverlay.anchorMin = new Vector2(0.5f, 0.5f);
        gradientOverlay.anchorMax = new Vector2(0.5f, 0.5f);
        gradientOverlay.pivot = new Vector2(0.5f, 0.5f);
        gradientOverlay.sizeDelta = mCounterRect.sizeDelta;
        gradientOverlay.anchoredPosition = mCounterRect.anchoredPosition;

        var top = UGUIReplicaUIFactory.CreatePanel("TopFade", gradientOverlay, new Color(0.04f, 0.05f, 0.10f, 0.72f));
        top.anchorMin = new Vector2(0f, 1f);
        top.anchorMax = new Vector2(1f, 1f);
        top.pivot = new Vector2(0.5f, 1f);
        top.sizeDelta = new Vector2(0f, 56f);
        top.anchoredPosition = Vector2.zero;
        top.SetAsLastSibling();

        var bottom = UGUIReplicaUIFactory.CreatePanel("BottomFade", gradientOverlay, new Color(0.04f, 0.05f, 0.10f, 0.72f));
        bottom.anchorMin = new Vector2(0f, 0f);
        bottom.anchorMax = new Vector2(1f, 0f);
        bottom.pivot = new Vector2(0.5f, 0f);
        bottom.sizeDelta = new Vector2(0f, 56f);
        bottom.anchoredPosition = Vector2.zero;
        bottom.SetAsLastSibling();
    }

    private DigitColumn CreateDigitColumn(int place)
    {
        var digitRect = UGUIReplicaUIFactory.CreatePanel($"Digit_{place}", mCounterRect, new Color(0.16f, 0.20f, 0.30f, 0.92f));
        digitRect.sizeDelta = new Vector2(124f, 132f);
        UGUIReplicaUIFactory.EnsureComponent<LayoutElement>(digitRect.gameObject).preferredWidth = 124f;

        var mask = UGUIReplicaUIFactory.EnsureComponent<Mask>(digitRect.gameObject);
        mask.showMaskGraphic = true;

        var numbersRoot = UGUIReplicaUIFactory.CreateRect("Numbers", digitRect);
        Stretch(numbersRoot);

        var texts = new List<Text>(10);
        for (var i = 0; i < 10; i++)
        {
            var number = UGUIReplicaUIFactory.CreateText(
                $"N_{i}",
                numbersRoot,
                i.ToString(),
                112,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            var numberRect = (RectTransform)number.transform;
            numberRect.anchorMin = new Vector2(0.5f, 0.5f);
            numberRect.anchorMax = new Vector2(0.5f, 0.5f);
            numberRect.pivot = new Vector2(0.5f, 0.5f);
            numberRect.sizeDelta = new Vector2(120f, 124f);
            texts.Add(number);
        }

        return new DigitColumn
        {
            Place = place,
            Root = digitRect,
            Numbers = texts
        };
    }

    private void AnimateTo(int target)
    {
        mTween?.Kill();
        mTween = DOTween
            .To(() => mDisplayedValue, value =>
            {
                mDisplayedValue = value;
                RefreshDigits();
            }, target, 1.05f)
            .SetEase(Ease.OutCubic);
    }

    private void RefreshDigits()
    {
        for (var i = 0; i < mColumns.Count; i++)
        {
            var col = mColumns[i];
            var placeValue = Mathf.FloorToInt(mDisplayedValue / col.Place) % 10;
            var digitHeight = col.Root.sizeDelta.y;

            for (var n = 0; n < col.Numbers.Count; n++)
            {
                var offset = (10 + n - placeValue) % 10;
                if (offset > 5)
                {
                    offset -= 10;
                }

                var numberRect = (RectTransform)col.Numbers[n].transform;
                numberRect.anchoredPosition = new Vector2(0f, -offset * digitHeight);
            }
        }
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    private struct DigitColumn
    {
        public int Place;
        public RectTransform Root;
        public List<Text> Numbers;
    }
}
