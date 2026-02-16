using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UGUIStackMotionCard : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerClickHandler
{
    private UGUIStackMotionReplica mOwner = null;
    private RectTransform mRootRect = null;
    private RectTransform mTiltRect = null;
    private RectTransform mShadowRect = null;
    private RectTransform mVisualRect = null;
    private Image mHitImage = null;
    private Image mShadowImage = null;
    private Image mVisualImage = null;
    private Text mLabelText = null;

    private Tween mMoveTween;
    private Tween mTiltTween;
    private Tween mRotationTween;
    private Tween mScaleTween;
    private Tween mShadowTween;

    private bool mInteractionEnabled;
    private bool mDragging;
    private Vector2 mDragOffset;
    private Vector2 mPressPoint;
    private float mLayoutRotationZ;
    private float mLayoutScale = 1f;

    internal float RandomRotationOffset { get; private set; }

    private const float ShadowDefaultY = -10f;
    private const float ShadowDefaultScale = 1f;
    private const float ShadowDefaultAlpha = 0.26f;

    private void OnDisable()
    {
        KillTweens();
    }

    private void OnDestroy()
    {
        KillTweens();
    }

    internal void Initialize(UGUIStackMotionReplica owner, string title, Color color, float randomOffset, Vector2 cardSize)
    {
        mOwner = owner;
        RandomRotationOffset = randomOffset;

        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("UGUIStackMotionCard requires RectTransform.", this);
            return;
        }

        if (!TryGetComponent(out mHitImage))
        {
            Debug.LogError("UGUIStackMotionCard requires Image.", this);
            return;
        }

        mRootRect.anchorMin = new Vector2(0.5f, 0.5f);
        mRootRect.anchorMax = new Vector2(0.5f, 0.5f);
        mRootRect.pivot = new Vector2(0.5f, 0.5f);
        mRootRect.sizeDelta = cardSize;
        mRootRect.anchoredPosition = Vector2.zero;

        mHitImage.color = new Color(1f, 1f, 1f, 0.001f);
        mHitImage.raycastTarget = false;

        mShadowRect = GetOrCreateRect("Shadow", mRootRect);
        mShadowRect.anchorMin = new Vector2(0.5f, 0.5f);
        mShadowRect.anchorMax = new Vector2(0.5f, 0.5f);
        mShadowRect.pivot = new Vector2(0.5f, 0.5f);
        mShadowRect.sizeDelta = cardSize + new Vector2(26f, 26f);
        mShadowRect.anchoredPosition = new Vector2(0f, ShadowDefaultY);
        mShadowRect.localScale = Vector3.one;
        mShadowRect.SetSiblingIndex(0);

        mShadowImage = EnsureComponent<Image>(mShadowRect.gameObject);
        mShadowImage.color = new Color(0f, 0f, 0f, ShadowDefaultAlpha);
        mShadowImage.raycastTarget = false;

        mTiltRect = GetOrCreateRect("Tilt", mRootRect);
        mTiltRect.anchorMin = new Vector2(0.5f, 0.5f);
        mTiltRect.anchorMax = new Vector2(0.5f, 0.5f);
        mTiltRect.pivot = new Vector2(0.5f, 0.5f);
        mTiltRect.sizeDelta = cardSize;
        mTiltRect.anchoredPosition = Vector2.zero;
        mTiltRect.localRotation = Quaternion.identity;

        mVisualRect = GetOrCreateRect("CardVisual", mTiltRect);
        mVisualRect.anchorMin = new Vector2(0.5f, 0.5f);
        mVisualRect.anchorMax = new Vector2(0.5f, 0.5f);
        mVisualRect.pivot = new Vector2(0.5f, 0.5f);
        mVisualRect.sizeDelta = cardSize;
        mVisualRect.anchoredPosition = Vector2.zero;
        mVisualRect.localRotation = Quaternion.identity;
        mVisualRect.localScale = Vector3.one;

        mVisualImage = EnsureComponent<Image>(mVisualRect.gameObject);
        mVisualImage.color = color;
        mVisualImage.raycastTarget = false;

        var outline = EnsureComponent<Outline>(mVisualRect.gameObject);
        outline.effectColor = new Color(0f, 0f, 0f, 0.18f);
        outline.effectDistance = new Vector2(2f, -2f);
        outline.useGraphicAlpha = true;

        var labelRect = GetOrCreateRect("Label", mVisualRect);
        labelRect.anchorMin = new Vector2(0f, 1f);
        labelRect.anchorMax = new Vector2(1f, 1f);
        labelRect.pivot = new Vector2(0.5f, 1f);
        labelRect.sizeDelta = new Vector2(-60f, 78f);
        labelRect.anchoredPosition = new Vector2(0f, -26f);

        mLabelText = EnsureComponent<Text>(labelRect.gameObject);
        mLabelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        mLabelText.fontSize = 44;
        mLabelText.fontStyle = FontStyle.Bold;
        mLabelText.alignment = TextAnchor.UpperCenter;
        mLabelText.color = new Color(1f, 1f, 1f, 0.96f);
        mLabelText.horizontalOverflow = HorizontalWrapMode.Wrap;
        mLabelText.verticalOverflow = VerticalWrapMode.Truncate;
        mLabelText.text = title;

        var captionRect = GetOrCreateRect("Caption", mVisualRect);
        captionRect.anchorMin = new Vector2(0.5f, 0f);
        captionRect.anchorMax = new Vector2(0.5f, 0f);
        captionRect.pivot = new Vector2(0.5f, 0f);
        captionRect.sizeDelta = new Vector2(cardSize.x - 84f, 54f);
        captionRect.anchoredPosition = new Vector2(0f, 28f);

        var captionText = EnsureComponent<Text>(captionRect.gameObject);
        captionText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        captionText.fontSize = 26;
        captionText.fontStyle = FontStyle.Normal;
        captionText.alignment = TextAnchor.LowerCenter;
        captionText.color = new Color(1f, 1f, 1f, 0.82f);
        captionText.horizontalOverflow = HorizontalWrapMode.Wrap;
        captionText.verticalOverflow = VerticalWrapMode.Truncate;
        captionText.text = "Drag to tilt. Throw past threshold to cycle stack.";
        captionText.raycastTarget = false;

        SetInteractionEnabled(false);
        SnapToNeutral();
    }

    internal void SetSiblingIndex(int index)
    {
        transform.SetSiblingIndex(index);
    }

    internal void SetInteractionEnabled(bool enabled)
    {
        mInteractionEnabled = enabled;
        if (mHitImage != null)
        {
            mHitImage.raycastTarget = enabled;
        }
    }

    internal void ApplyLayout(float targetRotationZ, float targetScale, bool animate, float duration)
    {
        mLayoutRotationZ = targetRotationZ;
        mLayoutScale = Mathf.Clamp(targetScale, 0.7f, 1.05f);

        if (mVisualRect == null)
        {
            return;
        }

        if (animate)
        {
            mRotationTween?.Kill();
            mScaleTween?.Kill();

            mRotationTween = mVisualRect
                .DOLocalRotate(new Vector3(0f, 0f, mLayoutRotationZ), duration)
                .SetEase(Ease.OutBack);

            mScaleTween = mVisualRect
                .DOScale(mLayoutScale, duration)
                .SetEase(Ease.OutQuad);
        }
        else
        {
            mVisualRect.localRotation = Quaternion.Euler(0f, 0f, mLayoutRotationZ);
            mVisualRect.localScale = new Vector3(mLayoutScale, mLayoutScale, 1f);
        }

        if (!mDragging)
        {
            TweenBackToNeutral(duration * 0.85f);
        }
    }

    internal void SnapToNeutral()
    {
        mDragOffset = Vector2.zero;
        if (mRootRect != null)
        {
            mRootRect.anchoredPosition = Vector2.zero;
        }

        if (mTiltRect != null)
        {
            mTiltRect.localRotation = Quaternion.identity;
        }

        if (mVisualRect != null)
        {
            mVisualRect.localRotation = Quaternion.Euler(0f, 0f, mLayoutRotationZ);
            mVisualRect.localScale = new Vector3(mLayoutScale, mLayoutScale, 1f);
        }

        if (mShadowRect != null)
        {
            mShadowRect.anchoredPosition = new Vector2(0f, ShadowDefaultY);
            mShadowRect.localScale = new Vector3(ShadowDefaultScale, ShadowDefaultScale, 1f);
        }

        if (mShadowImage != null)
        {
            var c = mShadowImage.color;
            c.a = ShadowDefaultAlpha;
            mShadowImage.color = c;
        }
    }

    internal void TweenBackToNeutral(float duration)
    {
        if (mRootRect == null || mTiltRect == null)
        {
            return;
        }

        var safeDuration = Mathf.Max(0.06f, duration);
        mDragOffset = Vector2.zero;

        mMoveTween?.Kill();
        mMoveTween = mRootRect
            .DOAnchorPos(Vector2.zero, safeDuration)
            .SetEase(Ease.OutCubic);

        mTiltTween?.Kill();
        mTiltTween = mTiltRect
            .DOLocalRotate(Vector3.zero, safeDuration)
            .SetEase(Ease.OutCubic);

        TweenShadow(0f, 0f, 0f, safeDuration);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!mInteractionEnabled || mOwner == null)
        {
            return;
        }

        mDragging = true;
        mOwner.NotifyDragState(true);
        mDragOffset = Vector2.zero;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)mOwner.transform,
                eventData.position,
                eventData.pressEventCamera,
                out var localPoint))
        {
            mPressPoint = localPoint;
        }
        else
        {
            mPressPoint = Vector2.zero;
        }

        KillPositionTweensOnly();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!mDragging || mOwner == null || mRootRect == null)
        {
            return;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)mOwner.transform,
                eventData.position,
                eventData.pressEventCamera,
                out var localPoint))
        {
            return;
        }

        mDragOffset = localPoint - mPressPoint;
        mRootRect.anchoredPosition = mDragOffset;

        var normalizedX = Mathf.Clamp(mDragOffset.x / 120f, -1f, 1f);
        var normalizedY = Mathf.Clamp(mDragOffset.y / 120f, -1f, 1f);
        var rotX = -normalizedY * mOwner.MaxTilt;
        var rotY = normalizedX * mOwner.MaxTilt;

        mTiltRect.localRotation = Quaternion.Euler(rotX, rotY, 0f);

        var lift = Mathf.Clamp01(mDragOffset.magnitude / 260f);
        TweenShadow(rotX, rotY, lift, 0.08f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!mDragging)
        {
            return;
        }

        mDragging = false;
        if (mOwner != null)
        {
            mOwner.HandleCardDragEnded(this, mDragOffset);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!mInteractionEnabled || mDragging || mOwner == null)
        {
            return;
        }

        mOwner.HandleCardClicked(this);
    }

    private void TweenShadow(float rotX, float rotY, float lift01, float duration)
    {
        if (mShadowRect == null || mShadowImage == null)
        {
            return;
        }

        var targetOffset = new Vector2(
            rotY * 1.2f,
            ShadowDefaultY - (rotX * 0.8f) - (18f * lift01));
        var targetScale = ShadowDefaultScale + (0.22f * lift01);
        var targetAlpha = Mathf.Lerp(ShadowDefaultAlpha, 0.12f, lift01);

        var startOffset = mShadowRect.anchoredPosition;
        var startScale = mShadowRect.localScale.x;
        var startAlpha = mShadowImage.color.a;
        var safeDuration = Mathf.Max(0.03f, duration);

        mShadowTween?.Kill();
        mShadowTween = DOVirtual
            .Float(0f, 1f, safeDuration, t =>
            {
                mShadowRect.anchoredPosition = Vector2.Lerp(startOffset, targetOffset, t);
                var scale = Mathf.Lerp(startScale, targetScale, t);
                mShadowRect.localScale = new Vector3(scale, scale, 1f);

                var c = mShadowImage.color;
                c.a = Mathf.Lerp(startAlpha, targetAlpha, t);
                mShadowImage.color = c;
            })
            .SetEase(Ease.OutQuad);
    }

    private void KillPositionTweensOnly()
    {
        mMoveTween?.Kill();
        mTiltTween?.Kill();
        mShadowTween?.Kill();
    }

    private void KillTweens()
    {
        mMoveTween?.Kill();
        mTiltTween?.Kill();
        mRotationTween?.Kill();
        mScaleTween?.Kill();
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
