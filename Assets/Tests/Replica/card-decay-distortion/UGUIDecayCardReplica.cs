using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUIDecayCardReplica : MonoBehaviour
{
    [SerializeField] private Vector2 mCardSize = new(560f, 700f);
    [SerializeField] private float mMoveBound = 50f;

    private RectTransform mCardRoot;
    private Image mCardImage;
    private readonly List<RectTransform> mNoiseStrips = new();
    private readonly List<float> mNoiseBaseY = new();
    private readonly List<Image> mNoiseImages = new();

    private Vector2 mCurrentOffset;
    private float mCurrentRotationZ;
    private Vector2 mCachedCursor;
    private float mDistortionScale;

    private void Awake()
    {
        BuildView();
        mCachedCursor = Input.mousePosition;
    }

    private void Update()
    {
        UpdateCardTilt();
        UpdateDistortion();
    }

    private void BuildView()
    {
        var root = (RectTransform)transform;
        root.anchorMin = Vector2.zero;
        root.anchorMax = Vector2.one;
        root.offsetMin = Vector2.zero;
        root.offsetMax = Vector2.zero;

        var backdrop = UGUIReplicaUIFactory.CreatePanel("Backdrop", root, new Color(0.06f, 0.08f, 0.13f, 0.9f));
        Stretch(backdrop);

        var hint = UGUIReplicaUIFactory.CreateText(
            "Hint",
            backdrop,
            "DecayCard  |  Move cursor to tilt and disturb",
            30,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            new Color(0.93f, 0.96f, 1f, 0.95f));
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(920f, 58f);
        hintRect.anchoredPosition = new Vector2(0f, -42f);

        var stage = UGUIReplicaUIFactory.CreatePanel("Stage", backdrop, new Color(0.09f, 0.12f, 0.19f, 0.9f));
        stage.anchorMin = new Vector2(0.5f, 0.5f);
        stage.anchorMax = new Vector2(0.5f, 0.5f);
        stage.pivot = new Vector2(0.5f, 0.5f);
        stage.sizeDelta = new Vector2(1240f, 760f);
        stage.anchoredPosition = new Vector2(0f, -20f);

        var stageShadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(stage.gameObject);
        stageShadow.effectColor = new Color(0f, 0f, 0f, 0.34f);
        stageShadow.effectDistance = new Vector2(0f, -10f);

        mCardRoot = UGUIReplicaUIFactory.CreateRect("DecayCard", stage);
        mCardRoot.anchorMin = new Vector2(0.5f, 0.5f);
        mCardRoot.anchorMax = new Vector2(0.5f, 0.5f);
        mCardRoot.pivot = new Vector2(0.5f, 0.5f);
        mCardRoot.sizeDelta = mCardSize;
        mCardRoot.anchoredPosition = Vector2.zero;

        var cardMask = UGUIReplicaUIFactory.CreatePanel("CardMask", mCardRoot, new Color(0.32f, 0.36f, 0.44f, 1f));
        Stretch(cardMask);
        UGUIReplicaUIFactory.EnsureComponent<Mask>(cardMask.gameObject).showMaskGraphic = true;

        mCardImage = UGUIReplicaUIFactory.CreatePanel("Photo", cardMask, new Color(0.36f, 0.39f, 0.46f, 1f)).GetComponent<Image>();
        var photoRect = mCardImage.rectTransform;
        photoRect.anchorMin = Vector2.zero;
        photoRect.anchorMax = Vector2.one;
        photoRect.offsetMin = Vector2.zero;
        photoRect.offsetMax = Vector2.zero;

        var gradientA = UGUIReplicaUIFactory.CreatePanel("GradientA", photoRect, new Color(0.16f, 0.25f, 0.52f, 0.45f));
        gradientA.anchorMin = Vector2.zero;
        gradientA.anchorMax = Vector2.one;
        gradientA.offsetMin = Vector2.zero;
        gradientA.offsetMax = Vector2.zero;

        var gradientB = UGUIReplicaUIFactory.CreatePanel("GradientB", photoRect, new Color(0.57f, 0.25f, 0.22f, 0.30f));
        gradientB.anchorMin = Vector2.zero;
        gradientB.anchorMax = Vector2.one;
        gradientB.offsetMin = new Vector2(0f, -130f);
        gradientB.offsetMax = new Vector2(0f, 200f);

        var noiseRoot = UGUIReplicaUIFactory.CreateRect("Noise", photoRect);
        Stretch(noiseRoot);

        mNoiseStrips.Clear();
        mNoiseBaseY.Clear();
        mNoiseImages.Clear();

        const int stripCount = 28;
        var stripHeight = mCardSize.y / stripCount;
        for (var i = 0; i < stripCount; i++)
        {
            var strip = UGUIReplicaUIFactory.CreatePanel($"Strip_{i}", noiseRoot, new Color(1f, 1f, 1f, 0.03f));
            strip.anchorMin = new Vector2(0.5f, 0f);
            strip.anchorMax = new Vector2(0.5f, 0f);
            strip.pivot = new Vector2(0.5f, 0f);
            strip.sizeDelta = new Vector2(mCardSize.x + 40f, stripHeight + 2f);
            var y = i * stripHeight;
            strip.anchoredPosition = new Vector2(0f, y);

            mNoiseStrips.Add(strip);
            mNoiseBaseY.Add(y);
            mNoiseImages.Add(strip.GetComponent<Image>());
        }

        var cardOutline = UGUIReplicaUIFactory.EnsureComponent<Outline>(mCardRoot.gameObject);
        cardOutline.effectColor = new Color(1f, 1f, 1f, 0.46f);
        cardOutline.effectDistance = new Vector2(1.8f, -1.8f);

        var title = UGUIReplicaUIFactory.CreateText(
            "CardText",
            mCardRoot,
            "NEXT\nGEN",
            94,
            FontStyle.Bold,
            TextAnchor.LowerLeft,
            new Color(0.98f, 0.98f, 1f, 0.95f));
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0f, 0f);
        titleRect.anchorMax = new Vector2(0f, 0f);
        titleRect.pivot = new Vector2(0f, 0f);
        titleRect.sizeDelta = new Vector2(340f, 260f);
        titleRect.anchoredPosition = new Vector2(36f, 34f);
    }

    private void UpdateCardTilt()
    {
        var cursor = (Vector2)Input.mousePosition;
        var width = Mathf.Max(1f, Screen.width);
        var height = Mathf.Max(1f, Screen.height);

        var mapX = Mathf.Lerp(-120f, 120f, cursor.x / width);
        var mapY = Mathf.Lerp(-120f, 120f, cursor.y / height);
        var mapRz = Mathf.Lerp(-10f, 10f, cursor.x / width);

        var targetX = Mathf.Lerp(mCurrentOffset.x, mapX, 0.1f);
        var targetY = Mathf.Lerp(mCurrentOffset.y, mapY, 0.1f);
        var targetRz = Mathf.Lerp(mCurrentRotationZ, mapRz, 0.1f);

        if (targetX > mMoveBound)
        {
            targetX = mMoveBound + ((targetX - mMoveBound) * 0.2f);
        }
        else if (targetX < -mMoveBound)
        {
            targetX = -mMoveBound + ((targetX + mMoveBound) * 0.2f);
        }

        if (targetY > mMoveBound)
        {
            targetY = mMoveBound + ((targetY - mMoveBound) * 0.2f);
        }
        else if (targetY < -mMoveBound)
        {
            targetY = -mMoveBound + ((targetY + mMoveBound) * 0.2f);
        }

        mCurrentOffset = new Vector2(targetX, targetY);
        mCurrentRotationZ = targetRz;

        mCardRoot.anchoredPosition = mCurrentOffset;
        mCardRoot.localRotation = Quaternion.Euler(0f, 0f, mCurrentRotationZ);
    }

    private void UpdateDistortion()
    {
        var cursor = (Vector2)Input.mousePosition;
        var travelled = Vector2.Distance(mCachedCursor, cursor);
        var mapped = Mathf.Clamp01(travelled / 200f);
        mDistortionScale = Mathf.Lerp(mDistortionScale, mapped, 0.06f);

        for (var i = 0; i < mNoiseStrips.Count; i++)
        {
            var strip = mNoiseStrips[i];
            var wave = Mathf.Sin((Time.time * 7.2f) + (i * 0.65f));
            var direction = (i % 2 == 0) ? 1f : -1f;
            var x = direction * wave * (18f + (i * 0.4f)) * mDistortionScale;
            strip.anchoredPosition = new Vector2(x, mNoiseBaseY[i]);

            var alphaWave = 0.5f + (0.5f * Mathf.Sin((Time.time * 11.5f) + (i * 0.9f)));
            var c = mNoiseImages[i].color;
            c.a = Mathf.Lerp(0.02f, 0.20f, mDistortionScale) * alphaWave;
            mNoiseImages[i].color = c;
        }

        mCardImage.color = Color.Lerp(
            new Color(0.36f, 0.39f, 0.46f, 1f),
            new Color(0.70f, 0.72f, 0.77f, 1f),
            mDistortionScale * 0.6f);

        mCachedCursor = cursor;
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
