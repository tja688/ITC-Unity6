using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class CurvedLoopEffectController : IReplicaEffect<CurvedLoopEffectConfig, CurvedLoopEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private CurvedLoopEffectConfig mConfig;
    private CurvedLoopEffectView mView;
    private CurvedLoopEffectModel mModel;

    private bool mInitialized;
    private float mOffset;
    private float mDirection = -1f;
    private float mLastDragDelta;
    private bool mDragging;
    private float mLoopLength = 1f;

    public string EffectId => "curved-loop-v2";

    public void Initialize(ReplicaHostContext context, CurvedLoopEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<CurvedLoopEffectConfig>();
        mView = CurvedLoopEffectViewBuilder.Build(mContext.MountRoot, mConfig);

        var relay = ReplicaUIFactoryV2.EnsureComponent<CurvedLoopInputRelay>(mView.Root.gameObject);
        relay.Initialize(this);

        SetModel(CurvedLoopEffectModel.CreateDefault(), false);
        mInitialized = true;
    }

    public void SetModel(CurvedLoopEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : CurvedLoopEffectModel.CreateDefault();
        mDirection = -1f;
        mDragging = false;
        mLastDragDelta = 0f;
        mOffset = 0f;

        CurvedLoopEffectViewBuilder.RebuildLetters(mView, mConfig, mModel.MarqueeText);
        var spacing = Mathf.Max(1f, mConfig.DefaultSpacing);
        mLoopLength = Mathf.Max(1f, mView.LetterRects.Count * spacing);

        if (!animated)
        {
            mView.Group.alpha = 1f;
            mView.Content.anchoredPosition = mConfig.ViewportPosition;
        }
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var offset = EnterOffset(transition.EnterDirection, mConfig.EnterOffset);
        var basePos = mConfig.ViewportPosition;

        mView.Content.anchoredPosition = basePos + offset;
        mView.Group.alpha = 0f;

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Content, basePos, duration, mView.Root)
            .SetEase(transition.Ease));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Root, transition.Ease));
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        if (!mInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        KillTransitionTweens();

        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);
        var offset = ExitOffset(transition.ExitDirection, mConfig.ExitOffset);
        var basePos = mConfig.ViewportPosition;

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Content, basePos + offset, duration, mView.Root)
            .SetEase(transition.Ease)
            .OnComplete(onComplete));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 0f, duration, mView.Root, transition.Ease));
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized)
        {
            return;
        }

        var dt = mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;
        if (!mDragging)
        {
            mOffset += mDirection * (mModel != null ? mModel.Speed : 160f) * dt;
        }

        LayoutLetters();
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (mModel == null || !mModel.Interactive)
        {
            return;
        }

        mDragging = true;
        mLastDragDelta = 0f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mModel == null || !mModel.Interactive || !mDragging || eventData == null)
        {
            return;
        }

        var deltaX = eventData.delta.x;
        mOffset += deltaX * mConfig.DragScale;
        mLastDragDelta = deltaX;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (mModel == null || !mModel.Interactive)
        {
            return;
        }

        mDragging = false;
        if (Mathf.Abs(mLastDragDelta) > mConfig.DragDirectionThreshold)
        {
            mDirection = mLastDragDelta >= 0f ? 1f : -1f;
        }
    }

    private void LayoutLetters()
    {
        if (mView == null || mView.LetterRects.Count == 0)
        {
            return;
        }

        var loop = Mathf.Max(1f, mLoopLength);
        var width = Mathf.Max(1f, mConfig.CurveWidth);
        var start = new Vector2(-width * 0.5f, mConfig.BaselineY);
        var control = new Vector2(0f, mModel != null ? mModel.CurveAmount : 220f);
        var end = new Vector2(width * 0.5f, mConfig.BaselineY);

        for (var i = 0; i < mView.LetterRects.Count; i++)
        {
            var advance = i < mView.LetterAdvances.Count ? mView.LetterAdvances[i] : 0f;
            var distance = Mathf.Repeat(advance + mOffset, loop);
            var t = distance / loop;

            var point = EvaluateQuadratic(start, control, end, t);
            var tangent = EvaluateQuadraticTangent(start, control, end, t);
            var angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;

            var rect = mView.LetterRects[i];
            if (rect == null)
            {
                continue;
            }

            rect.anchoredPosition = point;
            rect.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    private IReplicaTweenHandle FadeCanvasGroup(CanvasGroup group, float targetAlpha, float duration, UnityEngine.Object owner, ReplicaEase ease)
    {
        var alpha = group.alpha;
        return mContext.Tweens
            .ToFloat(
                () => alpha,
                value =>
                {
                    alpha = value;
                    group.alpha = value;
                },
                targetAlpha,
                duration,
                owner)
            .SetEase(ease);
    }

    private void KillTransitionTweens()
    {
        for (var i = 0; i < mTransitionTweens.Count; i++)
        {
            mTransitionTweens[i]?.Kill();
        }

        mTransitionTweens.Clear();
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

    private static Vector2 EnterOffset(ReplicaEnterDirection direction, float distance)
    {
        switch (direction)
        {
            case ReplicaEnterDirection.FromTop: return new Vector2(0f, distance);
            case ReplicaEnterDirection.FromBottom: return new Vector2(0f, -distance);
            case ReplicaEnterDirection.FromLeft: return new Vector2(-distance, 0f);
            case ReplicaEnterDirection.FromRight: return new Vector2(distance, 0f);
            default: return Vector2.zero;
        }
    }

    private static Vector2 ExitOffset(ReplicaExitDirection direction, float distance)
    {
        switch (direction)
        {
            case ReplicaExitDirection.ToTop: return new Vector2(0f, distance);
            case ReplicaExitDirection.ToBottom: return new Vector2(0f, -distance);
            case ReplicaExitDirection.ToLeft: return new Vector2(-distance, 0f);
            case ReplicaExitDirection.ToRight: return new Vector2(distance, 0f);
            case ReplicaExitDirection.FadeOnly:
            default:
                return Vector2.zero;
        }
    }
}
