using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UGUIStackMotionReplica : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [System.Serializable]
    private struct CardPreset
    {
        public string Title;
        public Color Color;
    }

    [Header("Layout")]
    [SerializeField] private Vector2 mStackSize = new(760f, 520f);
    [SerializeField] private Vector2 mCardSize = new(620f, 390f);

    [Header("Motion")]
    [SerializeField] private float mDragSensitivity = 200f;
    [SerializeField] private float mMaxTilt = 22f;
    [SerializeField] private float mLayoutDuration = 0.32f;
    [SerializeField] private float mReturnDuration = 0.24f;
    [SerializeField] private float mRotationStep = 4f;
    [SerializeField] private float mScaleStep = 0.06f;

    [Header("Behavior")]
    [SerializeField] private bool mSendToBackOnClick = true;
    [SerializeField] private bool mAutoplay;
    [SerializeField] private float mAutoplayDelay = 3f;
    [SerializeField] private bool mPauseOnHover = true;

    [Header("Send To Back Transition")]
    [SerializeField] private float mSendToBackOutDuration = 0.22f;
    [SerializeField] private float mSendToBackRecoverDuration = 0.42f;
    [SerializeField] private float mSendToBackTravelDistance = 220f;

    [Header("Cards")]
    [SerializeField] private CardPreset[] mCardPresets = null;

    private readonly List<UGUIStackMotionCard> mCards = new();
    private RectTransform mRootRect = null;
    private Image mRaycastImage = null;
    private RectTransform mDeckRect = null;
    private bool mIsHoverPaused;
    private bool mIsDraggingCard;
    private bool mIsSendToBackAnimating;
    private float mAutoplayTimer;
    private Tween mSendToBackGateTween;

    private static readonly CardPreset[] sDefaultCards =
    {
        new CardPreset
        {
            Title = "Misty Valley",
            Color = new Color(0.33f, 0.46f, 0.76f, 1f)
        },
        new CardPreset
        {
            Title = "City Glow",
            Color = new Color(0.19f, 0.58f, 0.63f, 1f)
        },
        new CardPreset
        {
            Title = "Golden Cliff",
            Color = new Color(0.67f, 0.49f, 0.26f, 1f)
        },
        new CardPreset
        {
            Title = "Forest Echo",
            Color = new Color(0.28f, 0.55f, 0.35f, 1f)
        }
    };

    internal float DragSensitivity => mDragSensitivity;
    internal float MaxTilt => mMaxTilt;
    internal float ReturnDuration => mReturnDuration;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("UGUIStackMotionReplica requires RectTransform.", this);
            return;
        }

        if (!TryGetComponent(out mRaycastImage))
        {
            Debug.LogError("UGUIStackMotionReplica requires Image.", this);
            return;
        }

        EnsureCanvas();
        EnsureEventSystem();
        BuildLayout();
        BuildCards();
        ApplyStackLayout(false, mLayoutDuration, mReturnDuration);
    }

    private void Update()
    {
        if (!mAutoplay || mCards.Count <= 1 || mIsDraggingCard || mIsSendToBackAnimating || (mPauseOnHover && mIsHoverPaused))
        {
            return;
        }

        mAutoplayTimer += Time.deltaTime;
        if (mAutoplayTimer < Mathf.Max(0.2f, mAutoplayDelay))
        {
            return;
        }

        mAutoplayTimer = 0f;
        SendTopCardToBack();
    }

    private void OnDisable()
    {
        mSendToBackGateTween?.Kill();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (mPauseOnHover)
        {
            mIsHoverPaused = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (mPauseOnHover)
        {
            mIsHoverPaused = false;
        }
    }

    internal void NotifyDragState(bool isDragging)
    {
        mIsDraggingCard = isDragging;
        if (isDragging)
        {
            mAutoplayTimer = 0f;
        }
    }

    internal void HandleCardDragEnded(UGUIStackMotionCard card, Vector2 offset)
    {
        if (!IsTopCard(card))
        {
            card.TweenBackToNeutral(mReturnDuration);
            mIsDraggingCard = false;
            return;
        }

        var shouldSendToBack = Mathf.Abs(offset.x) > mDragSensitivity || Mathf.Abs(offset.y) > mDragSensitivity;
        if (shouldSendToBack)
        {
            SendToBack(card);
        }
        else
        {
            card.TweenBackToNeutral(mReturnDuration);
        }

        mIsDraggingCard = false;
        mAutoplayTimer = 0f;
    }

    internal void HandleCardClicked(UGUIStackMotionCard card)
    {
        if (mIsSendToBackAnimating || !mSendToBackOnClick || !IsTopCard(card))
        {
            return;
        }

        SendToBack(card);
        mAutoplayTimer = 0f;
    }

    private bool IsTopCard(UGUIStackMotionCard card)
    {
        return mCards.Count > 0 && mCards[mCards.Count - 1] == card;
    }

    private void SendTopCardToBack()
    {
        if (mCards.Count < 2)
        {
            return;
        }

        SendToBack(mCards[mCards.Count - 1]);
    }

    private void SendToBack(UGUIStackMotionCard card)
    {
        if (card == null || mIsSendToBackAnimating)
        {
            return;
        }

        mIsSendToBackAnimating = true;
        card.PlaySendToBackTransition(
            mSendToBackOutDuration,
            mSendToBackTravelDistance,
            () =>
            {
                if (!mCards.Remove(card))
                {
                    mIsSendToBackAnimating = false;
                    return;
                }

                mCards.Insert(0, card);
                ApplyStackLayout(true, mSendToBackRecoverDuration, mSendToBackRecoverDuration);
                mSendToBackGateTween?.Kill();
                mSendToBackGateTween = DOVirtual.DelayedCall(
                    mSendToBackRecoverDuration,
                    () => mIsSendToBackAnimating = false,
                    false);
            });
    }

    private void ApplyStackLayout(bool animate, float layoutDuration, float returnDuration)
    {
        var count = mCards.Count;
        if (count == 0)
        {
            return;
        }

        for (var i = 0; i < count; i++)
        {
            var card = mCards[i];
            var depth = count - i - 1;
            var rotationZ = (depth * mRotationStep) + card.RandomRotationOffset;
            var scale = 1f + (i * mScaleStep) - (count * mScaleStep);

            card.SetInteractionEnabled(i == count - 1);
            card.SetSiblingIndex(i);
            card.ApplyLayout(rotationZ, scale, animate, layoutDuration, returnDuration);
        }
    }

    private void BuildLayout()
    {
        name = "StackMotionReplica";
        mRootRect.anchorMin = new Vector2(0.5f, 0.5f);
        mRootRect.anchorMax = new Vector2(0.5f, 0.5f);
        mRootRect.pivot = new Vector2(0.5f, 0.5f);
        mRootRect.sizeDelta = mStackSize;
        mRootRect.anchoredPosition = Vector2.zero;

        mRaycastImage.color = new Color(1f, 1f, 1f, 0.001f);
        mRaycastImage.raycastTarget = true;

        var backdrop = GetOrCreateRect("Backdrop", mRootRect);
        backdrop.anchorMin = Vector2.zero;
        backdrop.anchorMax = Vector2.one;
        backdrop.offsetMin = new Vector2(-140f, -110f);
        backdrop.offsetMax = new Vector2(140f, 110f);
        backdrop.SetSiblingIndex(0);

        var backdropImage = EnsureComponent<Image>(backdrop.gameObject);
        backdropImage.color = new Color(0.06f, 0.09f, 0.16f, 1f);
        backdropImage.raycastTarget = false;

        var plateA = GetOrCreateRect("DepthPlateA", backdrop);
        plateA.anchorMin = new Vector2(0.5f, 0.5f);
        plateA.anchorMax = new Vector2(0.5f, 0.5f);
        plateA.pivot = new Vector2(0.5f, 0.5f);
        plateA.sizeDelta = new Vector2(mStackSize.x + 120f, mStackSize.y + 160f);
        plateA.anchoredPosition = new Vector2(-22f, 20f);
        plateA.localRotation = Quaternion.Euler(0f, 0f, -7f);

        var plateAImage = EnsureComponent<Image>(plateA.gameObject);
        plateAImage.color = new Color(0.17f, 0.24f, 0.38f, 0.54f);
        plateAImage.raycastTarget = false;

        var plateB = GetOrCreateRect("DepthPlateB", backdrop);
        plateB.anchorMin = new Vector2(0.5f, 0.5f);
        plateB.anchorMax = new Vector2(0.5f, 0.5f);
        plateB.pivot = new Vector2(0.5f, 0.5f);
        plateB.sizeDelta = new Vector2(mStackSize.x + 160f, mStackSize.y + 110f);
        plateB.anchoredPosition = new Vector2(35f, -26f);
        plateB.localRotation = Quaternion.Euler(0f, 0f, 5f);

        var plateBImage = EnsureComponent<Image>(plateB.gameObject);
        plateBImage.color = new Color(0.10f, 0.17f, 0.28f, 0.65f);
        plateBImage.raycastTarget = false;

        mDeckRect = GetOrCreateRect("Deck", mRootRect);
        mDeckRect.anchorMin = new Vector2(0.5f, 0.5f);
        mDeckRect.anchorMax = new Vector2(0.5f, 0.5f);
        mDeckRect.pivot = new Vector2(0.5f, 0.5f);
        mDeckRect.sizeDelta = mStackSize;
        mDeckRect.anchoredPosition = Vector2.zero;
        mDeckRect.SetAsLastSibling();
    }

    private void BuildCards()
    {
        mCards.Clear();

        if (mDeckRect == null)
        {
            return;
        }

        for (var i = mDeckRect.childCount - 1; i >= 0; i--)
        {
            var child = mDeckRect.GetChild(i);
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        var source = (mCardPresets != null && mCardPresets.Length > 0) ? mCardPresets : sDefaultCards;
        for (var i = 0; i < source.Length; i++)
        {
            var cardGo = new GameObject($"Card_{i + 1}", typeof(RectTransform), typeof(UGUIStackMotionCard));
            var cardRect = cardGo.GetComponent<RectTransform>();
            cardRect.SetParent(mDeckRect, false);

            if (!cardGo.TryGetComponent(out UGUIStackMotionCard card))
            {
                continue;
            }

            var randomRotate = (Mathf.PerlinNoise((i + 1f) * 11.8f, 0.31f) - 0.5f) * 10f;
            card.Initialize(this, source[i].Title, source[i].Color, randomRotate, mCardSize);
            mCards.Add(card);
        }
    }

    private void EnsureCanvas()
    {
        var parentCanvas = GetComponentInParent<Canvas>();
        if (parentCanvas != null)
        {
            return;
        }

        var canvasGo = new GameObject("TestsCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        if (!canvasGo.TryGetComponent(out Canvas canvas))
        {
            return;
        }

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        mRootRect.SetParent(canvas.transform, false);
    }

    private static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            _ = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }
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
