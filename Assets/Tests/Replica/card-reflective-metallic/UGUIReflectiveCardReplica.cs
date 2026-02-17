using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUIReflectiveCardReplica : MonoBehaviour
{
    [SerializeField] private float mTiltStrength = 10f;
    [SerializeField] private float mParallaxStrength = 16f;

    private RectTransform mRootRect;
    private RectTransform mCardRect;
    private RectTransform mSheenRect;
    private RectTransform mSpotlightRect;
    private Image mCardImage;
    private Image mSheenImage;
    private Image mSpotlightImage;

    private readonly List<RectTransform> mNoiseStrips = new();
    private readonly List<float> mNoiseBaseY = new();
    private readonly List<Image> mNoiseImages = new();

    private Vector2 mCurrentTilt;
    private Vector2 mCurrentOffset;
    private Vector2 mLastMousePos;
    private float mMotionEnergy;

    private static Sprite sRadialSprite;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("UGUIReflectiveCardReplica requires RectTransform.", this);
            return;
        }

        BuildView();
        mLastMousePos = Input.mousePosition;
    }

    private void Update()
    {
        UpdateMotionEnergy();
        UpdateCardTransform();
        UpdateSheenAndSpotlight();
        UpdateNoise();
    }

    private void BuildView()
    {
        mRootRect.anchorMin = Vector2.zero;
        mRootRect.anchorMax = Vector2.one;
        mRootRect.offsetMin = Vector2.zero;
        mRootRect.offsetMax = Vector2.zero;

        var backdrop = UGUIReplicaUIFactory.CreatePanel("Backdrop", mRootRect, new Color(0.04f, 0.06f, 0.11f, 0.92f));
        Stretch(backdrop);

        var plateA = UGUIReplicaUIFactory.CreatePanel("PlateA", backdrop, new Color(0.24f, 0.38f, 0.82f, 0.20f));
        plateA.anchorMin = new Vector2(0.5f, 0.5f);
        plateA.anchorMax = new Vector2(0.5f, 0.5f);
        plateA.pivot = new Vector2(0.5f, 0.5f);
        plateA.sizeDelta = new Vector2(1160f, 420f);
        plateA.anchoredPosition = new Vector2(-180f, 90f);
        plateA.localRotation = Quaternion.Euler(0f, 0f, -9f);

        var plateB = UGUIReplicaUIFactory.CreatePanel("PlateB", backdrop, new Color(0.56f, 0.33f, 0.82f, 0.18f));
        plateB.anchorMin = new Vector2(0.5f, 0.5f);
        plateB.anchorMax = new Vector2(0.5f, 0.5f);
        plateB.pivot = new Vector2(0.5f, 0.5f);
        plateB.sizeDelta = new Vector2(1020f, 360f);
        plateB.anchoredPosition = new Vector2(170f, -70f);
        plateB.localRotation = Quaternion.Euler(0f, 0f, 8f);

        var hint = UGUIReplicaUIFactory.CreateText(
            "Hint",
            backdrop,
            "ReflectiveCard  |  Move cursor for metallic sheen",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.93f, 0.96f, 1f, 0.95f));
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(1080f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -42f);

        var stage = UGUIReplicaUIFactory.CreatePanel("Stage", backdrop, new Color(0.08f, 0.10f, 0.17f, 0.90f));
        stage.anchorMin = new Vector2(0.5f, 0.5f);
        stage.anchorMax = new Vector2(0.5f, 0.5f);
        stage.pivot = new Vector2(0.5f, 0.5f);
        stage.sizeDelta = new Vector2(1080f, 760f);
        stage.anchoredPosition = new Vector2(0f, -20f);

        var stageShadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(stage.gameObject);
        stageShadow.effectColor = new Color(0f, 0f, 0f, 0.35f);
        stageShadow.effectDistance = new Vector2(0f, -10f);

        mCardRect = UGUIReplicaUIFactory.CreatePanel("Card", stage, new Color(0.13f, 0.15f, 0.20f, 1f));
        mCardRect.anchorMin = new Vector2(0.5f, 0.5f);
        mCardRect.anchorMax = new Vector2(0.5f, 0.5f);
        mCardRect.pivot = new Vector2(0.5f, 0.5f);
        mCardRect.sizeDelta = new Vector2(380f, 580f);
        mCardRect.anchoredPosition = Vector2.zero;
        mCardImage = mCardRect.GetComponent<Image>();

        var cardOutline = UGUIReplicaUIFactory.EnsureComponent<Outline>(mCardRect.gameObject);
        cardOutline.effectColor = new Color(1f, 1f, 1f, 0.22f);
        cardOutline.effectDistance = new Vector2(1f, -1f);

        var cardShadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(mCardRect.gameObject);
        cardShadow.effectColor = new Color(0f, 0f, 0f, 0.40f);
        cardShadow.effectDistance = new Vector2(0f, -10f);

        var overlayTint = UGUIReplicaUIFactory.CreatePanel("OverlayTint", mCardRect, new Color(0.90f, 0.92f, 0.98f, 0.08f));
        Stretch(overlayTint);

        var content = UGUIReplicaUIFactory.CreateRect("Content", mCardRect);
        Stretch(content);

        BuildCardText(content);
        BuildNoise(content);
        BuildSheen(content);
        BuildSpotlight(content);
    }

    private void BuildCardText(RectTransform content)
    {
        var header = UGUIReplicaUIFactory.CreateRect("Header", content);
        header.anchorMin = new Vector2(0f, 1f);
        header.anchorMax = new Vector2(1f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        header.sizeDelta = new Vector2(-44f, 70f);
        header.anchoredPosition = new Vector2(0f, -26f);

        var secure = UGUIReplicaUIFactory.CreatePanel("SecureBadge", header, new Color(1f, 1f, 1f, 0.14f));
        secure.anchorMin = new Vector2(0f, 0.5f);
        secure.anchorMax = new Vector2(0f, 0.5f);
        secure.pivot = new Vector2(0f, 0.5f);
        secure.sizeDelta = new Vector2(170f, 34f);
        secure.anchoredPosition = new Vector2(0f, 0f);
        var secureLabel = UGUIReplicaUIFactory.CreateText(
            "Label",
            secure,
            "SECURE ACCESS",
            15,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.94f, 0.96f, 1f, 0.92f));
        Stretch((RectTransform)secureLabel.transform);

        var statusDot = UGUIReplicaUIFactory.CreatePanel("Status", header, new Color(0.54f, 0.94f, 0.74f, 0.92f));
        statusDot.anchorMin = new Vector2(1f, 0.5f);
        statusDot.anchorMax = new Vector2(1f, 0.5f);
        statusDot.pivot = new Vector2(1f, 0.5f);
        statusDot.sizeDelta = new Vector2(22f, 22f);
        statusDot.anchoredPosition = new Vector2(0f, 0f);

        var userName = UGUIReplicaUIFactory.CreateText(
            "UserName",
            content,
            "ALEXANDER DOE",
            42,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.96f, 0.97f, 1f, 0.98f));
        var nameRect = (RectTransform)userName.transform;
        nameRect.anchorMin = new Vector2(0.5f, 0.5f);
        nameRect.anchorMax = new Vector2(0.5f, 0.5f);
        nameRect.pivot = new Vector2(0.5f, 0.5f);
        nameRect.sizeDelta = new Vector2(330f, 64f);
        nameRect.anchoredPosition = new Vector2(0f, -20f);

        var role = UGUIReplicaUIFactory.CreateText(
            "Role",
            content,
            "SENIOR DEVELOPER",
            18,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.92f, 0.94f, 1f, 0.68f));
        var roleRect = (RectTransform)role.transform;
        roleRect.anchorMin = new Vector2(0.5f, 0.5f);
        roleRect.anchorMax = new Vector2(0.5f, 0.5f);
        roleRect.pivot = new Vector2(0.5f, 0.5f);
        roleRect.sizeDelta = new Vector2(300f, 40f);
        roleRect.anchoredPosition = new Vector2(0f, -68f);

        var footer = UGUIReplicaUIFactory.CreateRect("Footer", content);
        footer.anchorMin = new Vector2(0f, 0f);
        footer.anchorMax = new Vector2(1f, 0f);
        footer.pivot = new Vector2(0.5f, 0f);
        footer.sizeDelta = new Vector2(-44f, 84f);
        footer.anchoredPosition = new Vector2(0f, 24f);

        var idLabel = UGUIReplicaUIFactory.CreateText(
            "IDLabel",
            footer,
            "ID NUMBER",
            12,
            FontStyle.Bold,
            TextAnchor.UpperLeft,
            new Color(0.94f, 0.96f, 1f, 0.55f));
        var idLabelRect = (RectTransform)idLabel.transform;
        idLabelRect.anchorMin = new Vector2(0f, 1f);
        idLabelRect.anchorMax = new Vector2(0f, 1f);
        idLabelRect.pivot = new Vector2(0f, 1f);
        idLabelRect.sizeDelta = new Vector2(160f, 24f);
        idLabelRect.anchoredPosition = Vector2.zero;

        var idValue = UGUIReplicaUIFactory.CreateText(
            "IDValue",
            footer,
            "8901-2345-6789",
            22,
            FontStyle.Bold,
            TextAnchor.LowerLeft,
            new Color(0.94f, 0.97f, 1f, 0.90f));
        var idValueRect = (RectTransform)idValue.transform;
        idValueRect.anchorMin = new Vector2(0f, 0f);
        idValueRect.anchorMax = new Vector2(0f, 0f);
        idValueRect.pivot = new Vector2(0f, 0f);
        idValueRect.sizeDelta = new Vector2(230f, 34f);
        idValueRect.anchoredPosition = Vector2.zero;

        var mark = UGUIReplicaUIFactory.CreateText(
            "Fingerprint",
            footer,
            "ID",
            32,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.95f, 0.98f, 1f, 0.46f));
        var markRect = (RectTransform)mark.transform;
        markRect.anchorMin = new Vector2(1f, 0.5f);
        markRect.anchorMax = new Vector2(1f, 0.5f);
        markRect.pivot = new Vector2(1f, 0.5f);
        markRect.sizeDelta = new Vector2(72f, 72f);
        markRect.anchoredPosition = new Vector2(0f, 0f);
    }

    private void BuildNoise(RectTransform parent)
    {
        var noiseRoot = UGUIReplicaUIFactory.CreateRect("Noise", parent);
        Stretch(noiseRoot);
        mNoiseStrips.Clear();
        mNoiseBaseY.Clear();
        mNoiseImages.Clear();

        const int stripCount = 30;
        var height = mCardRect.sizeDelta.y / stripCount;

        for (var i = 0; i < stripCount; i++)
        {
            var strip = UGUIReplicaUIFactory.CreatePanel($"Strip_{i}", noiseRoot, new Color(1f, 1f, 1f, 0.02f));
            strip.anchorMin = new Vector2(0.5f, 0f);
            strip.anchorMax = new Vector2(0.5f, 0f);
            strip.pivot = new Vector2(0.5f, 0f);
            strip.sizeDelta = new Vector2(mCardRect.sizeDelta.x + 56f, height + 2f);
            var y = i * height;
            strip.anchoredPosition = new Vector2(0f, y);

            mNoiseStrips.Add(strip);
            mNoiseBaseY.Add(y);
            mNoiseImages.Add(strip.GetComponent<Image>());
        }
    }

    private void BuildSheen(RectTransform parent)
    {
        mSheenRect = UGUIReplicaUIFactory.CreatePanel("Sheen", parent, new Color(1f, 1f, 1f, 0.18f));
        mSheenRect.anchorMin = new Vector2(0.5f, 0.5f);
        mSheenRect.anchorMax = new Vector2(0.5f, 0.5f);
        mSheenRect.pivot = new Vector2(0.5f, 0.5f);
        mSheenRect.sizeDelta = new Vector2(180f, 820f);
        mSheenRect.anchoredPosition = new Vector2(-220f, 0f);
        mSheenRect.localRotation = Quaternion.Euler(0f, 0f, 24f);
        mSheenImage = mSheenRect.GetComponent<Image>();
        mSheenImage.raycastTarget = false;
    }

    private void BuildSpotlight(RectTransform parent)
    {
        mSpotlightRect = UGUIReplicaUIFactory.CreateRect("Spotlight", parent);
        mSpotlightRect.anchorMin = new Vector2(0.5f, 0.5f);
        mSpotlightRect.anchorMax = new Vector2(0.5f, 0.5f);
        mSpotlightRect.pivot = new Vector2(0.5f, 0.5f);
        mSpotlightRect.sizeDelta = new Vector2(520f, 520f);
        mSpotlightRect.anchoredPosition = Vector2.zero;

        mSpotlightImage = UGUIReplicaUIFactory.EnsureComponent<Image>(mSpotlightRect.gameObject);
        mSpotlightImage.sprite = GetOrCreateRadialSprite();
        mSpotlightImage.color = new Color(0.82f, 0.88f, 1f, 0.16f);
        mSpotlightImage.raycastTarget = false;
    }

    private void UpdateMotionEnergy()
    {
        var mouse = (Vector2)Input.mousePosition;
        var speed = Vector2.Distance(mouse, mLastMousePos) / 90f;
        mMotionEnergy = Mathf.Lerp(mMotionEnergy, Mathf.Clamp01(speed), Time.unscaledDeltaTime * 7f);
        mLastMousePos = mouse;
    }

    private void UpdateCardTransform()
    {
        if (mCardRect == null)
        {
            return;
        }

        _ = RectTransformUtility.ScreenPointToLocalPointInRectangle(mCardRect, Input.mousePosition, null, out var localPos);
        var halfW = Mathf.Max(1f, mCardRect.rect.width * 0.5f);
        var halfH = Mathf.Max(1f, mCardRect.rect.height * 0.5f);
        var normalized = new Vector2(
            Mathf.Clamp(localPos.x / halfW, -1f, 1f),
            Mathf.Clamp(localPos.y / halfH, -1f, 1f));

        var targetTilt = new Vector2(-normalized.y * mTiltStrength, normalized.x * mTiltStrength);
        var targetOffset = new Vector2(normalized.x * mParallaxStrength, normalized.y * (mParallaxStrength * 0.8f));

        mCurrentTilt = Vector2.Lerp(mCurrentTilt, targetTilt, Time.unscaledDeltaTime * 6f);
        mCurrentOffset = Vector2.Lerp(mCurrentOffset, targetOffset, Time.unscaledDeltaTime * 5f);

        mCardRect.localRotation = Quaternion.Euler(mCurrentTilt.x, mCurrentTilt.y, -normalized.x * 2f);
        mCardRect.anchoredPosition = mCurrentOffset;
    }

    private void UpdateSheenAndSpotlight()
    {
        _ = RectTransformUtility.ScreenPointToLocalPointInRectangle(mCardRect, Input.mousePosition, null, out var localPos);
        var halfW = Mathf.Max(1f, mCardRect.rect.width * 0.5f);
        var halfH = Mathf.Max(1f, mCardRect.rect.height * 0.5f);
        var normalized = new Vector2(
            Mathf.Clamp(localPos.x / halfW, -1f, 1f),
            Mathf.Clamp(localPos.y / halfH, -1f, 1f));

        var sweep = Mathf.Sin(Time.unscaledTime * 0.8f);
        var sheenX = Mathf.Lerp(-250f, 250f, (sweep + 1f) * 0.5f) + (normalized.x * 100f);
        mSheenRect.anchoredPosition = new Vector2(sheenX, 0f);
        mSheenImage.color = new Color(1f, 1f, 1f, Mathf.Lerp(0.12f, 0.30f, mMotionEnergy));

        mSpotlightRect.anchoredPosition = new Vector2(normalized.x * 100f, normalized.y * 130f);
        mSpotlightImage.color = new Color(0.82f, 0.88f, 1f, Mathf.Lerp(0.12f, 0.34f, mMotionEnergy));

        mCardImage.color = Color.Lerp(
            new Color(0.12f, 0.15f, 0.20f, 1f),
            new Color(0.26f, 0.30f, 0.38f, 1f),
            mMotionEnergy * 0.45f);
    }

    private void UpdateNoise()
    {
        for (var i = 0; i < mNoiseStrips.Count; i++)
        {
            var strip = mNoiseStrips[i];
            var wave = Mathf.Sin((Time.unscaledTime * 8.2f) + (i * 0.52f));
            var direction = (i % 2 == 0) ? 1f : -1f;
            var offset = direction * wave * (4f + (14f * mMotionEnergy));

            strip.anchoredPosition = new Vector2(offset, mNoiseBaseY[i]);

            var alphaWave = 0.55f + (0.45f * Mathf.Sin((Time.unscaledTime * 10.2f) + (i * 0.73f)));
            var color = mNoiseImages[i].color;
            color.a = Mathf.Lerp(0.01f, 0.16f, mMotionEnergy) * alphaWave;
            mNoiseImages[i].color = color;
        }
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
            name = "ReplicaRadialSprite_Reflective"
        };

        var center = (size - 1) * 0.5f;
        var maxDistance = Mathf.Sqrt((center * center) + (center * center));

        for (var y = 0; y < size; y++)
        {
            for (var x = 0; x < size; x++)
            {
                var dx = x - center;
                var dy = y - center;
                var distance = Mathf.Sqrt((dx * dx) + (dy * dy)) / maxDistance;
                var alpha = Mathf.Clamp01(1f - distance);
                alpha *= alpha;
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
