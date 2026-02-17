using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUIGlitchTextReplica : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string mText = "GLITCH";
    [SerializeField] private float mSpeed = 1f;
    [SerializeField] private bool mEnableShadows = true;
    [SerializeField] private bool mEnableOnHover = true;

    private RectTransform mMainRect;
    private RectTransform mAfterMask;
    private RectTransform mBeforeMask;
    private RectTransform mAfterTextRect;
    private RectTransform mBeforeTextRect;
    private Text mAfterText;
    private Text mBeforeText;
    private bool mGlitchActive;
    private float mTick;
    private Tween mMainPunch;

    private void Awake()
    {
        BuildView();
    }

    private void OnEnable()
    {
        mGlitchActive = !mEnableOnHover;
        ResetSlices();
    }

    private void OnDisable()
    {
        mMainPunch?.Kill();
    }

    private void Update()
    {
        if (!mGlitchActive)
        {
            return;
        }

        mTick += Time.unscaledDeltaTime;
        var step = Mathf.Max(0.03f, 0.08f * mSpeed);
        if (mTick < step)
        {
            return;
        }

        mTick = 0f;
        GlitchStep();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!mEnableOnHover)
        {
            return;
        }

        mGlitchActive = true;
        mTick = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!mEnableOnHover)
        {
            return;
        }

        mGlitchActive = false;
        ResetSlices();
    }

    private void BuildView()
    {
        var root = (RectTransform)transform;
        Stretch(root);

        var bg = UGUIReplicaUIFactory.EnsureComponent<Image>(gameObject);
        bg.color = new Color(0.02f, 0.02f, 0.07f, 0.98f);
        bg.raycastTarget = true;

        var backdrop = UGUIReplicaUIFactory.CreatePanel("Backdrop", root, new Color(0.04f, 0.03f, 0.10f, 0.9f));
        backdrop.anchorMin = new Vector2(0.5f, 0.5f);
        backdrop.anchorMax = new Vector2(0.5f, 0.5f);
        backdrop.pivot = new Vector2(0.5f, 0.5f);
        backdrop.sizeDelta = new Vector2(1180f, 460f);
        backdrop.anchoredPosition = new Vector2(0f, -10f);

        var hint = UGUIReplicaUIFactory.CreateText(
            "Hint",
            root,
            mEnableOnHover ? "GlitchText  |  hover to trigger RGB slices" : "GlitchText  |  always on",
            30,
            FontStyle.Bold,
            TextAnchor.UpperCenter,
            new Color(0.95f, 0.97f, 1f, 1f));
        var hintRect = (RectTransform)hint.transform;
        hintRect.anchorMin = new Vector2(0.5f, 1f);
        hintRect.anchorMax = new Vector2(0.5f, 1f);
        hintRect.pivot = new Vector2(0.5f, 1f);
        hintRect.sizeDelta = new Vector2(900f, 52f);
        hintRect.anchoredPosition = new Vector2(0f, -40f);

        var container = UGUIReplicaUIFactory.CreateRect("GlitchContainer", backdrop);
        container.anchorMin = new Vector2(0.5f, 0.5f);
        container.anchorMax = new Vector2(0.5f, 0.5f);
        container.pivot = new Vector2(0.5f, 0.5f);
        container.sizeDelta = new Vector2(900f, 220f);
        container.anchoredPosition = Vector2.zero;

        var main = UGUIReplicaUIFactory.CreateText(
            "MainText",
            container,
            mText,
            132,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        mMainRect = (RectTransform)main.transform;
        Stretch(mMainRect);

        var afterMaskRect = UGUIReplicaUIFactory.CreateRect("AfterMask", container);
        mAfterMask = afterMaskRect;
        Stretch(afterMaskRect);
        UGUIReplicaUIFactory.EnsureComponent<RectMask2D>(afterMaskRect.gameObject);

        mAfterText = UGUIReplicaUIFactory.CreateText(
            "AfterText",
            afterMaskRect,
            mText,
            132,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        mAfterTextRect = (RectTransform)mAfterText.transform;
        Stretch(mAfterTextRect);
        if (mEnableShadows)
        {
            var afterShadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(mAfterText.gameObject);
            afterShadow.effectColor = Color.red;
            afterShadow.effectDistance = new Vector2(-5f, 0f);
        }

        var beforeMaskRect = UGUIReplicaUIFactory.CreateRect("BeforeMask", container);
        mBeforeMask = beforeMaskRect;
        Stretch(beforeMaskRect);
        UGUIReplicaUIFactory.EnsureComponent<RectMask2D>(beforeMaskRect.gameObject);

        mBeforeText = UGUIReplicaUIFactory.CreateText(
            "BeforeText",
            beforeMaskRect,
            mText,
            132,
            FontStyle.Bold,
            TextAnchor.MiddleCenter,
            Color.white);
        mBeforeTextRect = (RectTransform)mBeforeText.transform;
        Stretch(mBeforeTextRect);
        if (mEnableShadows)
        {
            var beforeShadow = UGUIReplicaUIFactory.EnsureComponent<Shadow>(mBeforeText.gameObject);
            beforeShadow.effectColor = Color.cyan;
            beforeShadow.effectDistance = new Vector2(5f, 0f);
        }

        ResetSlices();
    }

    private void GlitchStep()
    {
        var height = mMainRect.rect.height;
        var half = height * 0.5f;

        var afterBand = Random.Range(24f, 92f);
        var afterY = Random.Range(-half + 16f, half - 16f);
        SetBand(mAfterMask, afterBand, afterY);
        mAfterTextRect.anchoredPosition = new Vector2(10f + Random.Range(-6f, 6f), Random.Range(-4f, 4f));
        mAfterText.color = new Color(1f, 1f, 1f, Random.Range(0.70f, 1f));

        var beforeBand = Random.Range(24f, 92f);
        var beforeY = Random.Range(-half + 16f, half - 16f);
        SetBand(mBeforeMask, beforeBand, beforeY);
        mBeforeTextRect.anchoredPosition = new Vector2(-10f + Random.Range(-6f, 6f), Random.Range(-4f, 4f));
        mBeforeText.color = new Color(1f, 1f, 1f, Random.Range(0.70f, 1f));

        mMainRect.anchoredPosition = new Vector2(Random.Range(-2f, 2f), Random.Range(-2f, 2f));
        mMainPunch?.Kill();
        mMainPunch = mMainRect.DOPunchScale(new Vector3(0.015f, 0.015f, 0f), 0.08f, 2, 0.4f).SetUpdate(true);
    }

    private void ResetSlices()
    {
        mMainRect.anchoredPosition = Vector2.zero;
        mMainRect.localScale = Vector3.one;

        mAfterTextRect.anchoredPosition = new Vector2(10f, 0f);
        mBeforeTextRect.anchoredPosition = new Vector2(-10f, 0f);
        mAfterText.color = Color.white;
        mBeforeText.color = Color.white;

        if (mEnableOnHover)
        {
            SetBand(mAfterMask, 0f, 0f);
            SetBand(mBeforeMask, 0f, 0f);
        }
        else
        {
            SetBand(mAfterMask, 68f, 30f);
            SetBand(mBeforeMask, 58f, -22f);
        }
    }

    private static void SetBand(RectTransform maskRect, float height, float centerY)
    {
        maskRect.anchorMin = new Vector2(0f, 0.5f);
        maskRect.anchorMax = new Vector2(1f, 0.5f);
        maskRect.pivot = new Vector2(0.5f, 0.5f);
        maskRect.sizeDelta = new Vector2(0f, Mathf.Max(0f, height));
        maskRect.anchoredPosition = new Vector2(0f, centerY);
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
