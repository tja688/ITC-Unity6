using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UGUICardInteractionReplica : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerMoveHandler,
    IPointerDownHandler,
    IPointerUpHandler,
    IDragHandler
{
    [Header("Card Geometry")]
    [SerializeField] private Vector2 mRootSize = new(760f, 520f);
    [SerializeField] private Vector2 mCardSize = new(700f, 430f);
    [SerializeField] private Color mCardColor = new(0.97f, 0.94f, 0.88f, 1f);

    [Header("Paragraph Spotlight")]
    [SerializeField] private float mHighlightInsetX = 26f;
    [SerializeField] private float mHighlightInsetY = 8f;
    [SerializeField] private float mHighlightAlpha = 0.18f;
    [SerializeField] private float mHighlightTweenDuration = 0.17f;

    [Header("Motion")]
    [SerializeField] private float mHoverTilt = 3.5f;
    [SerializeField] private float mDragTilt = 14f;
    [SerializeField] private float mDragFollowDuration = 0.12f;
    [SerializeField] private float mReleaseDuration = 0.38f;

    private static readonly int SoftnessId = Shader.PropertyToID("_Softness");
    private static readonly int RadiusId = Shader.PropertyToID("_Radius");

    private readonly Vector3[] mWorldCorners = new Vector3[4];
    private readonly List<RectTransform> mParagraphRects = new();
    private RectTransform mRootRect = null!;
    private RectTransform mVisualRect = null!;
    private RectTransform mShadowRect = null!;
    private RectTransform mHighlightRect = null!;
    private RectTransform mContentRect = null!;
    private Image mRootRaycastImage = null!;
    private Image mShadowImage = null!;
    private Image mHighlightImage = null!;
    private Canvas mCanvas = null!;
    private Material mShadowMaterial = null!;

    private Tweener mMoveTween;
    private Tweener mRotateTween;
    private Tweener mScaleTween;
    private Tweener mHighlightMoveTween;
    private Tweener mHighlightSizeTween;
    private Tweener mHighlightFadeTween;
    private Tweener mShadowTween;

    private bool mPointerInside;
    private bool mDragging;
    private int mFocusedParagraph = -1;
    private Vector2 mPressPointerCanvasPos;
    private Vector2 mRootStartAnchoredPos;
    private Vector2 mGrabNormalized;

    private void Awake()
    {
        TryGetComponent(out mRootRect);
        TryGetComponent(out mRootRaycastImage);

        EnsureCanvas();
        EnsureEventSystem();
        BuildDemoHierarchy();
        InitializeVisualState();
    }

    private void OnDisable()
    {
        KillAllTweens();
    }

    private void OnDestroy()
    {
        if (mShadowMaterial != null)
        {
            if (Application.isPlaying)
            {
                Destroy(mShadowMaterial);
            }
            else
            {
                DestroyImmediate(mShadowMaterial);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mPointerInside = true;
        UpdateFocusedParagraph(eventData);
        UpdateHoverTilt(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mPointerInside = false;
        if (mDragging)
        {
            return;
        }

        FadeHighlight(0f);
        TweenVisualRotation(Vector3.zero, mReleaseDuration, Ease.OutCubic);
        TweenShadow(0f, 0f, 0f, mReleaseDuration);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (mDragging)
        {
            return;
        }

        UpdateFocusedParagraph(eventData);
        UpdateHoverTilt(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (mCanvas == null)
        {
            return;
        }

        mDragging = true;
        mRootStartAnchoredPos = mRootRect.anchoredPosition;

        var canvasRect = (RectTransform)mCanvas.transform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            mCanvas.worldCamera,
            out mPressPointerCanvasPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mVisualRect,
            eventData.position,
            eventData.pressEventCamera,
            out var grabLocal);

        var halfW = Mathf.Max(1f, mVisualRect.rect.width * 0.5f);
        var halfH = Mathf.Max(1f, mVisualRect.rect.height * 0.5f);
        mGrabNormalized = new Vector2(grabLocal.x / halfW, grabLocal.y / halfH);

        TweenVisualScale(1.01f, 0.14f, Ease.OutQuad);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mDragging = false;
        TweenRootPosition(Vector2.zero, mReleaseDuration, Ease.OutCubic);
        TweenVisualRotation(Vector3.zero, mReleaseDuration, Ease.OutBack);
        TweenVisualScale(1f, 0.24f, Ease.OutQuad);
        TweenShadow(0f, 0f, 0f, mReleaseDuration);

        if (!mPointerInside)
        {
            FadeHighlight(0f);
            mFocusedParagraph = -1;
        }
        else
        {
            UpdateFocusedParagraph(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!mDragging || mCanvas == null)
        {
            return;
        }

        var canvasRect = (RectTransform)mCanvas.transform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            eventData.position,
            mCanvas.worldCamera,
            out var pointerCanvasPos);

        var delta = pointerCanvasPos - mPressPointerCanvasPos;
        var targetPos = mRootStartAnchoredPos + delta;
        TweenRootPosition(targetPos, mDragFollowDuration, Ease.OutQuad);

        var normalizedDelta = delta / 220f;
        var rotX = Mathf.Clamp((-normalizedDelta.y * mDragTilt) + (mGrabNormalized.y * 4.5f), -mDragTilt, mDragTilt);
        var rotY = Mathf.Clamp((normalizedDelta.x * mDragTilt) - (mGrabNormalized.x * 4.5f), -mDragTilt, mDragTilt);
        var lift01 = Mathf.Clamp01(delta.magnitude / 260f);

        TweenVisualRotation(new Vector3(rotX, rotY, 0f), mDragFollowDuration, Ease.OutCubic);
        TweenShadow(rotX, rotY, lift01, mDragFollowDuration);
        UpdateFocusedParagraph(eventData);
    }

    private void EnsureCanvas()
    {
        var parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas == null)
        {
            var canvasGo = new GameObject("TestsCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            parentCanvas = canvasGo.GetComponent<Canvas>();
            parentCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            mRootRect.SetParent(canvasGo.transform, false);
        }

        mCanvas = parentCanvas;
    }

    private static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            _ = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
    }

    private void BuildDemoHierarchy()
    {
        name = "test";
        mRootRect.anchorMin = new Vector2(0.5f, 0.5f);
        mRootRect.anchorMax = new Vector2(0.5f, 0.5f);
        mRootRect.pivot = new Vector2(0.5f, 0.5f);
        mRootRect.sizeDelta = mRootSize;
        mRootRect.anchoredPosition = Vector2.zero;

        mRootRaycastImage.color = new Color(1f, 1f, 1f, 0.001f);
        mRootRaycastImage.raycastTarget = true;

        mShadowRect = GetOrCreateRect("Shadow", mRootRect);
        mShadowRect.anchorMin = new Vector2(0.5f, 0.5f);
        mShadowRect.anchorMax = new Vector2(0.5f, 0.5f);
        mShadowRect.pivot = new Vector2(0.5f, 0.5f);
        mShadowRect.sizeDelta = mCardSize;
        mShadowRect.anchoredPosition = new Vector2(0f, -6f);
        mShadowRect.localScale = Vector3.one;
        mShadowRect.SetSiblingIndex(0);

        mShadowImage = EnsureComponent<Image>(mShadowRect.gameObject);
        mShadowImage.raycastTarget = false;
        mShadowImage.color = new Color(0f, 0f, 0f, 0.25f);
        mShadowImage.sprite = null;
        mShadowImage.type = Image.Type.Simple;

        var shadowShader = Shader.Find("Tests/UI/SoftRectShadow");
        if (shadowShader != null)
        {
            mShadowMaterial = new Material(shadowShader);
            mShadowMaterial.SetFloat(SoftnessId, 0.16f);
            mShadowMaterial.SetFloat(RadiusId, 0.16f);
            mShadowImage.material = mShadowMaterial;
        }

        mVisualRect = GetOrCreateRect("CardVisual", mRootRect);
        mVisualRect.anchorMin = new Vector2(0.5f, 0.5f);
        mVisualRect.anchorMax = new Vector2(0.5f, 0.5f);
        mVisualRect.pivot = new Vector2(0.5f, 0.5f);
        mVisualRect.sizeDelta = mCardSize;
        mVisualRect.anchoredPosition = Vector2.zero;
        mVisualRect.localRotation = Quaternion.identity;
        mVisualRect.localScale = Vector3.one;
        mVisualRect.SetAsLastSibling();

        var visualImage = EnsureComponent<Image>(mVisualRect.gameObject);
        visualImage.color = mCardColor;
        visualImage.raycastTarget = false;

        var outline = EnsureComponent<Outline>(mVisualRect.gameObject);
        outline.effectColor = new Color(0.08f, 0.08f, 0.08f, 0.14f);
        outline.effectDistance = new Vector2(1f, -1f);
        outline.useGraphicAlpha = true;

        mHighlightRect = GetOrCreateRect("ParagraphHighlight", mVisualRect);
        mHighlightRect.anchorMin = new Vector2(0.5f, 0.5f);
        mHighlightRect.anchorMax = new Vector2(0.5f, 0.5f);
        mHighlightRect.pivot = new Vector2(0.5f, 0.5f);
        mHighlightRect.sizeDelta = new Vector2(620f, 60f);
        mHighlightRect.anchoredPosition = new Vector2(0f, 20f);
        mHighlightRect.SetSiblingIndex(0);

        mHighlightImage = EnsureComponent<Image>(mHighlightRect.gameObject);
        mHighlightImage.color = new Color(0.25f, 0.27f, 0.30f, 0f);
        mHighlightImage.raycastTarget = false;

        mContentRect = GetOrCreateRect("Content", mVisualRect);
        mContentRect.anchorMin = Vector2.zero;
        mContentRect.anchorMax = Vector2.one;
        mContentRect.offsetMin = new Vector2(34f, 28f);
        mContentRect.offsetMax = new Vector2(-34f, -24f);

        BuildParagraphTexts();
    }

    private void BuildParagraphTexts()
    {
        mParagraphRects.Clear();

        var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        var titleRect = GetOrCreateRect("Title", mContentRect);
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(620f, 56f);
        titleRect.anchoredPosition = new Vector2(0f, 0f);

        var titleText = EnsureComponent<Text>(titleRect.gameObject);
        titleText.font = font;
        titleText.fontSize = 34;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.UpperCenter;
        titleText.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        titleText.horizontalOverflow = HorizontalWrapMode.Wrap;
        titleText.verticalOverflow = VerticalWrapMode.Overflow;
        titleText.text = "Letter Micro Interaction";

        var paragraphs = new[]
        {
            "Hovering creates a soft reading spotlight that snaps to the current paragraph.",
            "Press and drag to pinch a point on the card. The body tilts with a leverage-like response.",
            "The farther you pull, the more the card appears lifted from the surface.",
            "Drop shadow expands and diffuses to reinforce depth, then eases back on release."
        };

        const float startTop = 74f;
        const float itemHeight = 70f;
        const float spacing = 12f;

        for (var i = 0; i < paragraphs.Length; i++)
        {
            var rowRect = GetOrCreateRect($"Paragraph_{i + 1}", mContentRect);
            rowRect.anchorMin = new Vector2(0.5f, 1f);
            rowRect.anchorMax = new Vector2(0.5f, 1f);
            rowRect.pivot = new Vector2(0.5f, 1f);
            rowRect.sizeDelta = new Vector2(620f, itemHeight);
            rowRect.anchoredPosition = new Vector2(0f, -(startTop + ((itemHeight + spacing) * i)));

            var text = EnsureComponent<Text>(rowRect.gameObject);
            text.font = font;
            text.fontSize = 24;
            text.fontStyle = FontStyle.Normal;
            text.alignment = TextAnchor.UpperLeft;
            text.color = new Color(0.13f, 0.13f, 0.13f, 0.92f);
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.text = paragraphs[i];

            mParagraphRects.Add(rowRect);
        }
    }

    private void InitializeVisualState()
    {
        mFocusedParagraph = -1;
        mVisualRect.localRotation = Quaternion.identity;
        mVisualRect.localScale = Vector3.one;
        mRootRect.anchoredPosition = Vector2.zero;
        mShadowRect.anchoredPosition = new Vector2(0f, -6f);
        mShadowRect.localScale = Vector3.one;

        var shadowColor = mShadowImage.color;
        shadowColor.a = 0.25f;
        mShadowImage.color = shadowColor;

        if (mShadowMaterial != null && mShadowMaterial.HasProperty(SoftnessId))
        {
            mShadowMaterial.SetFloat(SoftnessId, 0.16f);
        }

        if (mHighlightImage != null)
        {
            var c = mHighlightImage.color;
            c.a = 0f;
            mHighlightImage.color = c;
        }
    }

    private void UpdateFocusedParagraph(PointerEventData eventData)
    {
        if (mParagraphRects.Count == 0)
        {
            return;
        }

        if (!RectTransformUtility.RectangleContainsScreenPoint(mVisualRect, eventData.position, eventData.enterEventCamera))
        {
            FadeHighlight(0f);
            mFocusedParagraph = -1;
            return;
        }

        var index = FindParagraphIndex(eventData);
        if (index < 0 || index >= mParagraphRects.Count)
        {
            FadeHighlight(0f);
            mFocusedParagraph = -1;
            return;
        }

        if (index != mFocusedParagraph)
        {
            mFocusedParagraph = index;
        }

        var paragraph = mParagraphRects[index];
        paragraph.GetWorldCorners(mWorldCorners);
        var bottomLeft = (Vector2)mVisualRect.InverseTransformPoint(mWorldCorners[0]);
        var topRight = (Vector2)mVisualRect.InverseTransformPoint(mWorldCorners[2]);

        var center = (bottomLeft + topRight) * 0.5f;
        var size = new Vector2(
            Mathf.Abs(topRight.x - bottomLeft.x) + (mHighlightInsetX * 2f),
            Mathf.Abs(topRight.y - bottomLeft.y) + (mHighlightInsetY * 2f));

        mHighlightMoveTween?.Kill();
        mHighlightMoveTween = mHighlightRect
            .DOAnchorPos(center, mHighlightTweenDuration)
            .SetEase(Ease.OutCubic);

        mHighlightSizeTween?.Kill();
        mHighlightSizeTween = mHighlightRect
            .DOSizeDelta(size, mHighlightTweenDuration)
            .SetEase(Ease.OutCubic);

        FadeHighlight(mHighlightAlpha);
    }

    private int FindParagraphIndex(PointerEventData eventData)
    {
        var cameraForRaycast = eventData.enterEventCamera;
        for (var i = 0; i < mParagraphRects.Count; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(mParagraphRects[i], eventData.position, cameraForRaycast))
            {
                return i;
            }
        }

        var bestDistance = float.MaxValue;
        var bestIndex = -1;
        for (var i = 0; i < mParagraphRects.Count; i++)
        {
            var worldCenter = mParagraphRects[i].TransformPoint(mParagraphRects[i].rect.center);
            var screenCenter = RectTransformUtility.WorldToScreenPoint(cameraForRaycast, worldCenter);
            var verticalDistance = Mathf.Abs(screenCenter.y - eventData.position.y);
            if (verticalDistance < bestDistance)
            {
                bestDistance = verticalDistance;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private void UpdateHoverTilt(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mVisualRect,
                eventData.position,
                eventData.enterEventCamera,
                out var localPoint))
        {
            return;
        }

        var halfW = Mathf.Max(1f, mVisualRect.rect.width * 0.5f);
        var halfH = Mathf.Max(1f, mVisualRect.rect.height * 0.5f);
        var nx = Mathf.Clamp(localPoint.x / halfW, -1f, 1f);
        var ny = Mathf.Clamp(localPoint.y / halfH, -1f, 1f);

        var rotX = -ny * mHoverTilt;
        var rotY = nx * mHoverTilt;

        TweenVisualRotation(new Vector3(rotX, rotY, 0f), 0.16f, Ease.OutSine);
        TweenShadow(rotX, rotY, 0f, 0.16f);
    }

    private void TweenRootPosition(Vector2 target, float duration, Ease ease)
    {
        mMoveTween?.Kill();
        mMoveTween = mRootRect
            .DOAnchorPos(target, duration)
            .SetEase(ease);
    }

    private void TweenVisualRotation(Vector3 targetEuler, float duration, Ease ease)
    {
        mRotateTween?.Kill();
        mRotateTween = mVisualRect
            .DOLocalRotate(targetEuler, duration, RotateMode.Fast)
            .SetEase(ease);
    }

    private void TweenVisualScale(float target, float duration, Ease ease)
    {
        mScaleTween?.Kill();
        mScaleTween = mVisualRect
            .DOScale(target, duration)
            .SetEase(ease);
    }

    private void FadeHighlight(float targetAlpha)
    {
        mHighlightFadeTween?.Kill();
        mHighlightFadeTween = mHighlightImage
            .DOFade(targetAlpha, mHighlightTweenDuration)
            .SetEase(Ease.OutSine);
    }

    private void TweenShadow(float rotX, float rotY, float lift01, float duration)
    {
        var targetOffset = new Vector2(
            rotY * 1.2f,
            -6f - (rotX * 0.9f) - (18f * lift01));
        var targetScale = 1f + (0.24f * lift01);
        var targetAlpha = Mathf.Lerp(0.25f, 0.12f, lift01);
        var targetSoftness = Mathf.Lerp(0.16f, 0.32f, lift01);

        var startOffset = mShadowRect.anchoredPosition;
        var startScale = mShadowRect.localScale.x;
        var startAlpha = mShadowImage.color.a;
        var startSoftness = (mShadowMaterial != null && mShadowMaterial.HasProperty(SoftnessId))
            ? mShadowMaterial.GetFloat(SoftnessId)
            : targetSoftness;

        mShadowTween?.Kill();
        mShadowTween = DOVirtual
            .Float(0f, 1f, duration, t =>
            {
                mShadowRect.anchoredPosition = Vector2.Lerp(startOffset, targetOffset, t);
                var scale = Mathf.Lerp(startScale, targetScale, t);
                mShadowRect.localScale = new Vector3(scale, scale, 1f);

                var c = mShadowImage.color;
                c.a = Mathf.Lerp(startAlpha, targetAlpha, t);
                mShadowImage.color = c;

                if (mShadowMaterial != null && mShadowMaterial.HasProperty(SoftnessId))
                {
                    mShadowMaterial.SetFloat(SoftnessId, Mathf.Lerp(startSoftness, targetSoftness, t));
                }
            })
            .SetEase(Ease.OutQuad);
    }

    private void KillAllTweens()
    {
        mMoveTween?.Kill();
        mRotateTween?.Kill();
        mScaleTween?.Kill();
        mHighlightMoveTween?.Kill();
        mHighlightSizeTween?.Kill();
        mHighlightFadeTween?.Kill();
        mShadowTween?.Kill();
    }

    private static RectTransform GetOrCreateRect(string childName, RectTransform parent)
    {
        var child = parent.Find(childName);
        if (child != null)
        {
            return (RectTransform)child;
        }

        var go = new GameObject(childName, typeof(RectTransform));
        var rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        return rect;
    }

    private static T EnsureComponent<T>(GameObject go) where T : Component
    {
        if (!go.TryGetComponent<T>(out var component))
        {
            component = go.AddComponent<T>();
        }

        return component;
    }
}
