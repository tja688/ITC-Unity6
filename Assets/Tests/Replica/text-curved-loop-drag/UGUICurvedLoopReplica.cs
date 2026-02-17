using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class UGUICurvedLoopReplica : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private string mMarqueeText = "CURVED LOOP DRAG INTERACTION";
    [SerializeField] private float mSpeed = 160f;
    [SerializeField] private float mCurveAmount = 220f;
    [SerializeField] private bool mInteractive = true;

    private readonly List<LetterNode> mLetters = new();
    private RectTransform mTrack;
    private float mOffset;
    private float mDirection = -1f;
    private float mLastDragDelta;
    private bool mDragging;
    private float mSpacing = 32f;
    private float mLoopLength = 1f;

    private struct LetterNode
    {
        public RectTransform Rect;
        public float Advance;
    }

    private void Awake()
    {
        BuildView();
    }

    private void OnEnable()
    {
        mDirection = -1f;
        mDragging = false;
        mLastDragDelta = 0f;
    }

    private void Update()
    {
        if (!mDragging)
        {
            mOffset += mDirection * mSpeed * Time.unscaledDeltaTime;
        }

        LayoutLetters();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!mInteractive)
        {
            return;
        }

        mDragging = true;
        mLastDragDelta = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!mInteractive || !mDragging)
        {
            return;
        }

        mOffset += eventData.delta.x * 1.2f;
        mLastDragDelta = eventData.delta.x;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!mInteractive)
        {
            return;
        }

        mDragging = false;
        if (Mathf.Abs(mLastDragDelta) > 0.1f)
        {
            mDirection = mLastDragDelta >= 0f ? 1f : -1f;
        }
    }

    private void BuildView()
    {
        var root = (RectTransform)transform;
        Stretch(root);

        var bg = UGUIReplicaUIFactory.EnsureComponent<Image>(gameObject);
        bg.color = new Color(0.04f, 0.06f, 0.12f, 0.96f);
        bg.raycastTarget = true;

        var title = UGUIReplicaUIFactory.CreateText(
            "Title",
            root,
            "CurvedLoop  |  drag to change direction",
            30,
            FontStyle.Bold,
            TextAnchor.UpperCenter,
            new Color(0.91f, 0.95f, 1f, 1f));
        var titleRect = (RectTransform)title.transform;
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(900f, 52f);
        titleRect.anchoredPosition = new Vector2(0f, -40f);

        var viewport = UGUIReplicaUIFactory.CreatePanel("Viewport", root, new Color(0.08f, 0.11f, 0.20f, 0.86f));
        viewport.anchorMin = new Vector2(0.5f, 0.5f);
        viewport.anchorMax = new Vector2(0.5f, 0.5f);
        viewport.pivot = new Vector2(0.5f, 0.5f);
        viewport.sizeDelta = new Vector2(1460f, 420f);
        viewport.anchoredPosition = new Vector2(0f, -16f);
        UGUIReplicaUIFactory.EnsureComponent<RectMask2D>(viewport.gameObject);

        mTrack = UGUIReplicaUIFactory.CreateRect("Track", viewport);
        mTrack.anchorMin = new Vector2(0.5f, 0.5f);
        mTrack.anchorMax = new Vector2(0.5f, 0.5f);
        mTrack.pivot = new Vector2(0.5f, 0.5f);
        mTrack.sizeDelta = new Vector2(1580f, 360f);
        mTrack.anchoredPosition = Vector2.zero;

        mLetters.Clear();
        var prepared = PrepareText();
        mSpacing = 44f;
        mLoopLength = Mathf.Max(1f, prepared.Length * mSpacing);

        for (var i = 0; i < prepared.Length; i++)
        {
            var letter = UGUIReplicaUIFactory.CreateText(
                $"Letter_{i}",
                mTrack,
                prepared[i].ToString(),
                56,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Color.white);
            var rect = (RectTransform)letter.transform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(54f, 64f);

            mLetters.Add(new LetterNode
            {
                Rect = rect,
                Advance = i * mSpacing
            });
        }

        LayoutLetters();
    }

    private void LayoutLetters()
    {
        if (mLetters.Count == 0)
        {
            return;
        }

        var loop = Mathf.Max(1f, mLoopLength);
        var width = 1520f;
        var start = new Vector2(-width * 0.5f, -20f);
        var control = new Vector2(0f, mCurveAmount);
        var end = new Vector2(width * 0.5f, -20f);

        for (var i = 0; i < mLetters.Count; i++)
        {
            var distance = Mathf.Repeat(mLetters[i].Advance + mOffset, loop);
            var t = distance / loop;

            var point = EvaluateQuadratic(start, control, end, t);
            var tangent = EvaluateQuadraticTangent(start, control, end, t);
            var angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

            var rect = mLetters[i].Rect;
            rect.anchoredPosition = point;
            rect.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private string PrepareText()
    {
        var text = mMarqueeText;
        if (string.IsNullOrWhiteSpace(text))
        {
            text = "CURVED LOOP";
        }

        text = text.TrimEnd() + "\u00A0";
        var repeat = 10;
        var buffer = string.Empty;
        for (var i = 0; i < repeat; i++)
        {
            buffer += text;
        }

        return buffer;
    }

    private static Vector2 EvaluateQuadratic(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        var u = 1f - t;
        return (u * u * a) + (2f * u * t * b) + (t * t * c);
    }

    private static Vector2 EvaluateQuadraticTangent(Vector2 a, Vector2 b, Vector2 c, float t)
    {
        return (2f * (1f - t) * (b - a)) + (2f * t * (c - b));
    }

    private static void Stretch(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
