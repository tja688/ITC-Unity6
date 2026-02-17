using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class UGUISpotlightCardReplica : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    private RectTransform mRootRect;
    private RectTransform mCardRect;
    private RectTransform mSpotlightRect;
    private CanvasGroup mSpotlightGroup;
    private Image mBorderImage;

    private Vector2 mTargetLocalPos;
    private Vector2 mCurrentLocalPos;
    private bool mHovered;
    private float mTargetAlpha;
    private float mCurrentAlpha;

    private static Sprite sRadialSprite;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("UGUISpotlightCardReplica requires RectTransform.", this);
            return;
        }

        BuildView();
        mTargetLocalPos = Vector2.zero;
        mCurrentLocalPos = Vector2.zero;
    }

    private void OnEnable()
    {
        mHovered = false;
        mTargetLocalPos = Vector2.zero;
        mCurrentLocalPos = Vector2.zero;
        mCurrentAlpha = 0f;
        mTargetAlpha = 0f;
        if (mSpotlightGroup != null)
        {
            mSpotlightGroup.alpha = 0f;
        }
    }

    private void Update()
    {
        if (mSpotlightRect == null)
        {
            return;
        }

        // Smooth spotlight position tracking — slower lerp for organic, fluid movement
        mCurrentLocalPos = Vector2.Lerp(mCurrentLocalPos, mTargetLocalPos, Time.unscaledDeltaTime * 6f);
        mSpotlightRect.anchoredPosition = mCurrentLocalPos;

        // Smooth opacity transition matching CSS transition: opacity 0.5s ease
        mTargetAlpha = mHovered ? 0.6f : 0f;
        mCurrentAlpha = Mathf.Lerp(mCurrentAlpha, mTargetAlpha, Time.unscaledDeltaTime * 4f);
        mSpotlightGroup.alpha = mCurrentAlpha;

        // Border glow transition — smooth and gradual
        if (mBorderImage != null)
        {
            var targetBorderColor = mHovered
                ? new Color(0.44f, 0.86f, 1f, 0.72f)
                : new Color(0.22f, 0.28f, 0.38f, 0.70f);
            mBorderImage.color = Color.Lerp(mBorderImage.color, targetBorderColor, Time.unscaledDeltaTime * 4f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mHovered = true;
        UpdateTargetFromPointer(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mHovered = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        UpdateTargetFromPointer(eventData);
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

        var backdrop = UGUIReplicaUIFactory.CreatePanel("Backdrop", mRootRect, new Color(0.05f, 0.07f, 0.13f, 0.92f));
        Stretch(backdrop);

        var hint = UGUIReplicaUIFactory.CreateText(
            "Hint",
            backdrop,
            "SpotlightCard  |  Hover card to move light",
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

        // Card container — receives pointer events
        mCardRect = UGUIReplicaUIFactory.CreatePanel("SpotlightCard", backdrop, new Color(0.07f, 0.09f, 0.16f, 0.96f));
        mCardRect.anchorMin = new Vector2(0.5f, 0.5f);
        mCardRect.anchorMax = new Vector2(0.5f, 0.5f);
        mCardRect.pivot = new Vector2(0.5f, 0.5f);
        mCardRect.sizeDelta = new Vector2(900f, 500f);
        mCardRect.anchoredPosition = new Vector2(0f, -24f);

        // Rounded border effect — outer frame
        var borderFrame = UGUIReplicaUIFactory.CreatePanel("BorderFrame", mCardRect, new Color(0.22f, 0.28f, 0.38f, 0.70f));
        Stretch(borderFrame.GetComponent<RectTransform>());
        mBorderImage = borderFrame.GetComponent<Image>();
        mBorderImage.raycastTarget = false;

        // Inner card background (sits inside border)
        var inner = UGUIReplicaUIFactory.CreatePanel("Inner", mCardRect, new Color(0.05f, 0.07f, 0.13f, 0.98f));
        Stretch(inner);
        inner.offsetMin = new Vector2(2f, 2f);
        inner.offsetMax = new Vector2(-2f, -2f);

        // Mask to contain the spotlight effect within the card
        UGUIReplicaUIFactory.EnsureComponent<RectMask2D>(inner.gameObject);

        var title = UGUIReplicaUIFactory.CreateText(
            "Title",
            inner,
            "Interactive Spotlight Surface",
            46,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            new Color(0.94f, 0.97f, 1f, 0.98f));
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0f, 1f);
        titleRect.sizeDelta = new Vector2(-72f, 80f);
        titleRect.anchoredPosition = new Vector2(36f, -34f);

        var body = UGUIReplicaUIFactory.CreateText(
            "Body",
            inner,
            "Hover to reveal follow-light glow.\nThe spotlight tracks pointer position in real time.",
            28,
            FontStyle.Normal,
            TextAnchor.UpperLeft,
            new Color(0.84f, 0.90f, 1f, 0.72f));
        var bodyRect = (RectTransform)body.transform;
        bodyRect.anchorMin = new Vector2(0f, 0f);
        bodyRect.anchorMax = new Vector2(1f, 1f);
        bodyRect.pivot = new Vector2(0f, 1f);
        bodyRect.sizeDelta = new Vector2(-72f, -180f);
        bodyRect.anchoredPosition = new Vector2(36f, -140f);

        // Spotlight effect — large soft radial gradient
        mSpotlightRect = UGUIReplicaUIFactory.CreateRect("Spotlight", inner);
        mSpotlightRect.anchorMin = new Vector2(0.5f, 0.5f);
        mSpotlightRect.anchorMax = new Vector2(0.5f, 0.5f);
        mSpotlightRect.pivot = new Vector2(0.5f, 0.5f);
        mSpotlightRect.sizeDelta = new Vector2(700f, 700f);
        mSpotlightRect.anchoredPosition = Vector2.zero;

        var spotlightImage = UGUIReplicaUIFactory.EnsureComponent<Image>(mSpotlightRect.gameObject);
        spotlightImage.sprite = GetOrCreateRadialSprite();
        spotlightImage.color = new Color(0.48f, 0.84f, 1f, 0.5f);
        spotlightImage.raycastTarget = false;

        mSpotlightGroup = UGUIReplicaUIFactory.EnsureComponent<CanvasGroup>(mSpotlightRect.gameObject);
        mSpotlightGroup.alpha = 0f;
        mSpotlightGroup.blocksRaycasts = false;
        mSpotlightGroup.interactable = false;
    }

    private void UpdateTargetFromPointer(PointerEventData eventData)
    {
        if (mCardRect == null)
        {
            return;
        }

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(mCardRect, eventData.position, eventData.pressEventCamera, out var localPos))
        {
            return;
        }

        mTargetLocalPos = localPos;
    }

    private static Sprite GetOrCreateRadialSprite()
    {
        if (sRadialSprite != null)
        {
            return sRadialSprite;
        }

        const int size = 128;
        var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear,
            name = "ReplicaRadialSprite_Spotlight"
        };

        var center = (size - 1) * 0.5f;
        var maxDistance = center; // Use radius for circular gradient

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var dx = x - center;
                var dy = y - center;
                var distance = Mathf.Sqrt((dx * dx) + (dy * dy)) / maxDistance;
                // Softer falloff: smoothstep for natural CSS radial-gradient feel
                var alpha = Mathf.Clamp01(1f - distance);
                alpha = alpha * alpha * (3f - 2f * alpha); // smoothstep
                // Additional softening at edges (transparency at 80% from original CSS)
                alpha *= Mathf.Clamp01(1f - (distance * 0.8f));
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply(false, false);
        sRadialSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f), size);
        return sRadialSprite;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
