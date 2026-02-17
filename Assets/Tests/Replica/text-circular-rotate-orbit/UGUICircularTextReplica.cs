using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUICircularTextReplica : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string mText = "CIRCULAR MOTION REPLICA";
    [SerializeField] private float mSpinDuration = 20f;

    private RectTransform mRing;
    private readonly List<RectTransform> mLetterRects = new();
    private float mCurrentSpeed;
    private float mTargetSpeed;
    private float mCurrentScale = 1f;
    private float mTargetScale = 1f;

    private void Awake()
    {
        BuildView();
        var baseSpeed = 360f / Mathf.Max(0.1f, mSpinDuration);
        mCurrentSpeed = baseSpeed;
        mTargetSpeed = baseSpeed;
    }

    private void OnEnable()
    {
        var baseSpeed = 360f / Mathf.Max(0.1f, mSpinDuration);
        mCurrentSpeed = baseSpeed;
        mTargetSpeed = baseSpeed;
        mCurrentScale = 1f;
        mTargetScale = 1f;
    }

    private void Update()
    {
        var lerp = 1f - Mathf.Exp(-9f * Time.unscaledDeltaTime);
        mCurrentSpeed = Mathf.Lerp(mCurrentSpeed, mTargetSpeed, lerp);
        mCurrentScale = Mathf.Lerp(mCurrentScale, mTargetScale, lerp);

        mRing.localRotation *= Quaternion.Euler(0f, 0f, mCurrentSpeed * Time.unscaledDeltaTime);
        mRing.localScale = Vector3.one * mCurrentScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var baseSpeed = 360f / Mathf.Max(0.1f, mSpinDuration);
        mTargetSpeed = baseSpeed * 4f;
        mTargetScale = 0.92f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var baseSpeed = 360f / Mathf.Max(0.1f, mSpinDuration);
        mTargetSpeed = baseSpeed;
        mTargetScale = 1f;
    }

    private void BuildView()
    {
        var root = (RectTransform)transform;
        Stretch(root);

        var rootImage = UGUIReplicaUIFactory.EnsureComponent<Image>(gameObject);
        rootImage.color = new Color(0.03f, 0.05f, 0.10f, 0.95f);
        rootImage.raycastTarget = true;

        var ambient = UGUIReplicaUIFactory.CreatePanel("Ambient", root, new Color(0.30f, 0.19f, 0.63f, 0.30f));
        ambient.anchorMin = new Vector2(0.5f, 0.5f);
        ambient.anchorMax = new Vector2(0.5f, 0.5f);
        ambient.pivot = new Vector2(0.5f, 0.5f);
        ambient.sizeDelta = new Vector2(1000f, 1000f);
        ambient.anchoredPosition = new Vector2(0f, 10f);

        var title = UGUIReplicaUIFactory.CreateText(
            "Title",
            root,
            "CircularText  |  hover to speed up",
            30,
            FontStyle.Bold,
            TextAnchor.UpperCenter,
            new Color(0.92f, 0.95f, 1f, 1f));
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(720f, 52f);
        titleRect.anchoredPosition = new Vector2(0f, -42f);

        var ringHolder = UGUIReplicaUIFactory.CreateRect("RingHolder", root);
        ringHolder.anchorMin = new Vector2(0.5f, 0.5f);
        ringHolder.anchorMax = new Vector2(0.5f, 0.5f);
        ringHolder.pivot = new Vector2(0.5f, 0.5f);
        ringHolder.sizeDelta = new Vector2(460f, 460f);
        ringHolder.anchoredPosition = new Vector2(0f, -22f);

        var ringBack = UGUIReplicaUIFactory.CreatePanel("RingBack", ringHolder, new Color(0.10f, 0.14f, 0.24f, 0.58f));
        Stretch(ringBack);

        var centerCore = UGUIReplicaUIFactory.CreatePanel("Core", ringHolder, new Color(0.11f, 0.13f, 0.22f, 0.94f));
        centerCore.anchorMin = new Vector2(0.5f, 0.5f);
        centerCore.anchorMax = new Vector2(0.5f, 0.5f);
        centerCore.pivot = new Vector2(0.5f, 0.5f);
        centerCore.sizeDelta = new Vector2(158f, 158f);
        centerCore.anchoredPosition = Vector2.zero;

        var coreLabel = UGUIReplicaUIFactory.CreateText(
            "CoreLabel",
            centerCore,
            "360",
            42,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.85f, 0.88f, 1f, 1f));
        Stretch((RectTransform)coreLabel.transform);

        mRing = UGUIReplicaUIFactory.CreateRect("Ring", ringHolder);
        Stretch(mRing);

        var letters = mText.ToCharArray();
        if (letters.Length == 0)
        {
            letters = "CIRCULAR".ToCharArray();
        }

        mLetterRects.Clear();
        var radius = 170f;
        for (var i = 0; i < letters.Length; i++)
        {
            var ch = UGUIReplicaUIFactory.CreateText(
                $"Letter_{i}",
                mRing,
                letters[i].ToString(),
                28,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Color(1f, 1f, 1f, 0.96f));
            var chRect = (RectTransform)ch.transform;
            chRect.anchorMin = new Vector2(0.5f, 0.5f);
            chRect.anchorMax = new Vector2(0.5f, 0.5f);
            chRect.pivot = new Vector2(0.5f, 0.5f);
            chRect.sizeDelta = new Vector2(40f, 40f);

            var angle = (360f / letters.Length) * i * Mathf.Deg2Rad;
            var pos = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * radius;
            chRect.anchoredPosition = pos;
            chRect.localRotation = Quaternion.Euler(0f, 0f, -angle * Mathf.Rad2Deg);
            mLetterRects.Add(chRect);
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
