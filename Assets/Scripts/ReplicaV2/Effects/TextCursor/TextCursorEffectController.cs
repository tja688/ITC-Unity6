using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class TextCursorEffectController : IReplicaEffect<TextCursorEffectConfig, TextCursorEffectModel>
{
    private sealed class TrailPoint
    {
        public int Id;
        public RectTransform Rect;
        public Text Label;
        public Vector2 BasePos;
        public float Angle;
        public float RandomX;
        public float RandomY;
        public float RandomRotate;
        public float Phase;
        public bool Exiting;
        public IReplicaTweenHandle ExitFadeTween;
        public IReplicaTweenHandle ExitScaleTween;
        public IReplicaTweenHandle DestroyTween;
    }

    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();
    private readonly List<TrailPoint> mPoints = new List<TrailPoint>();

    private ReplicaHostContext mContext;
    private TextCursorEffectConfig mConfig;
    private TextCursorEffectView mView;
    private TextCursorEffectModel mModel;

    private bool mInitialized;
    private int mNextId;
    private Vector2 mLastPointerPos;
    private bool mHasLastPointer;
    private float mIdleTime;
    private float mRemovalTimer;
    private float mTime;

    public string EffectId => "text-cursor-v2";

    public void Initialize(ReplicaHostContext context, TextCursorEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<TextCursorEffectConfig>();
        mView = TextCursorEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        SetModel(TextCursorEffectModel.CreateDefault(), false);
        mInitialized = true;
    }

    public void SetModel(TextCursorEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : TextCursorEffectModel.CreateDefault();
        mHasLastPointer = false;
        mIdleTime = 0f;
        mRemovalTimer = 0f;

        for (var i = 0; i < mPoints.Count; i++)
        {
            if (mPoints[i]?.Label != null)
            {
                mPoints[i].Label.text = mModel.Text;
            }
        }

        if (!animated)
        {
            mView.Group.alpha = 1f;
            mView.MotionRoot.anchoredPosition = Vector2.zero;
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
        var basePos = Vector2.zero;

        mView.MotionRoot.anchoredPosition = basePos + offset;
        mView.Group.alpha = 0f;

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.MotionRoot, basePos, duration, mView.Root)
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
        var basePos = Vector2.zero;

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.MotionRoot, basePos + offset, duration, mView.Root)
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
        mTime += dt;

        UpdateMotionRootSize();
        UpdatePointerTrail(dt);
        UpdateFloat(dt);
        UpdateRemoval(dt);
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();

        for (var i = mPoints.Count - 1; i >= 0; i--)
        {
            DestroyPoint(mPoints[i], immediate: true);
        }

        mPoints.Clear();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }

    private void UpdateMotionRootSize()
    {
        if (mView?.MotionRoot == null || mContext.MountRoot == null)
        {
            return;
        }

        var size = mContext.MountRoot.rect.size;
        if (size.x <= 0.01f || size.y <= 0.01f)
        {
            return;
        }

        mView.MotionRoot.sizeDelta = size;
    }

    private void UpdatePointerTrail(float dt)
    {
        if (mContext.Pointer == null || !mContext.Pointer.IsPointerValid)
        {
            mHasLastPointer = false;
            mIdleTime += dt;
            return;
        }

        if (mView?.MotionRoot == null)
        {
            return;
        }

        if (!mContext.Pointer.TryGetLocalPoint(mView.MotionRoot, out var local))
        {
            mHasLastPointer = false;
            mIdleTime += dt;
            return;
        }

        if (!mView.MotionRoot.rect.Contains(local))
        {
            mHasLastPointer = false;
            mIdleTime += dt;
            return;
        }

        var moved = false;

        if (!mHasLastPointer || mPoints.Count == 0)
        {
            AddPoint(local, 0f);
            moved = true;
        }
        else
        {
            var last = mPoints[mPoints.Count - 1];
            var dx = local.x - last.BasePos.x;
            var dy = local.y - last.BasePos.y;
            var dist = Mathf.Sqrt(dx * dx + dy * dy);
            var spacing = Mathf.Max(1f, mModel.Spacing);
            if (dist >= spacing)
            {
                var steps = Mathf.FloorToInt(dist / spacing);
                var angle = mModel.FollowMouseDirection ? Mathf.Atan2(dy, dx) * Mathf.Rad2Deg : 0f;
                for (var i = 1; i <= steps; i++)
                {
                    var t = (spacing * i) / dist;
                    var p = new Vector2(last.BasePos.x + dx * t, last.BasePos.y + dy * t);
                    AddPoint(p, angle);
                }

                moved = true;
            }
        }

        if (moved)
        {
            mIdleTime = 0f;
            mRemovalTimer = 0f;
        }
        else
        {
            mIdleTime += dt;
        }

        mLastPointerPos = local;
        mHasLastPointer = true;
    }

    private void UpdateRemoval(float dt)
    {
        if (mPoints.Count == 0)
        {
            return;
        }

        if (mIdleTime <= 0.10f)
        {
            return;
        }

        var interval = Mathf.Max(0.01f, mModel.RemovalIntervalMs / 1000f);
        mRemovalTimer += dt;
        if (mRemovalTimer < interval)
        {
            return;
        }

        mRemovalTimer = 0f;
        RemoveOldest();
    }

    private void UpdateFloat(float dt)
    {
        if (mPoints.Count == 0)
        {
            return;
        }

        if (!mModel.RandomFloat)
        {
            for (var i = 0; i < mPoints.Count; i++)
            {
                var p = mPoints[i];
                if (p == null || p.Rect == null || p.Exiting)
                {
                    continue;
                }

                p.Rect.anchoredPosition = p.BasePos;
                p.Rect.localRotation = Quaternion.Euler(0f, 0f, p.Angle);
            }

            return;
        }

        var period = Mathf.Max(0.1f, mConfig.FloatPeriod);
        var w = (mTime / period) * (Mathf.PI * 2f);
        for (var i = 0; i < mPoints.Count; i++)
        {
            var p = mPoints[i];
            if (p == null || p.Rect == null || p.Exiting)
            {
                continue;
            }

            var ox = Mathf.Sin(w + p.Phase) * p.RandomX;
            var oy = Mathf.Sin(w + p.Phase * 1.3f) * p.RandomY;
            var or = Mathf.Sin(w + p.Phase * 0.7f) * p.RandomRotate;

            p.Rect.anchoredPosition = p.BasePos + new Vector2(ox, oy);
            p.Rect.localRotation = Quaternion.Euler(0f, 0f, p.Angle + or);
        }
    }

    private void AddPoint(Vector2 pos, float angle)
    {
        if (mView?.Overlay == null)
        {
            return;
        }

        var max = Mathf.Clamp(mModel.MaxPoints, 1, 64);

        var label = ReplicaUIFactoryV2.CreateText(
            $"Trail_{mNextId}",
            mView.Overlay,
            mModel.Text,
            mConfig.FontSize,
            mConfig.FontStyle,
            mConfig.Alignment,
            mConfig.TextColor);
        var startColor = label.color;
        startColor.a = 0f;
        label.color = startColor;

        var rect = (RectTransform)label.transform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        rect.localScale = Vector3.one;

        var point = new TrailPoint
        {
            Id = mNextId++,
            Rect = rect,
            Label = label,
            BasePos = pos,
            Angle = angle,
            RandomX = UnityEngine.Random.Range(-mConfig.RandomOffsetRange, mConfig.RandomOffsetRange),
            RandomY = UnityEngine.Random.Range(-mConfig.RandomOffsetRange, mConfig.RandomOffsetRange),
            RandomRotate = UnityEngine.Random.Range(-mConfig.RandomRotateRange, mConfig.RandomRotateRange),
            Phase = UnityEngine.Random.value * 10f,
            Exiting = false
        };

        if (mModel.FollowMouseDirection)
        {
            rect.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        mContext.Tweens
            .FadeTextTo(label, 1f, 0.12f, rect)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime);

        mPoints.Add(point);

        while (mPoints.Count > max)
        {
            RemoveOldest();
        }
    }

    private void RemoveOldest()
    {
        for (var i = 0; i < mPoints.Count; i++)
        {
            var p = mPoints[i];
            if (p == null || p.Exiting)
            {
                continue;
            }

            p.Exiting = true;
            StartExit(p);
            mPoints.RemoveAt(i);
            return;
        }
    }

    private void StartExit(TrailPoint point)
    {
        if (point == null || point.Rect == null || point.Label == null)
        {
            return;
        }

        var duration = Mathf.Max(0.01f, mModel.ExitDuration);

        point.ExitFadeTween?.Kill();
        point.ExitScaleTween?.Kill();
        point.DestroyTween?.Kill();

        point.ExitFadeTween = mContext.Tweens
            .FadeTextTo(point.Label, 0f, duration, point.Rect)
            .SetEase(mConfig.ExitEase)
            .SetUpdate(mContext.UseUnscaledTime);

        point.ExitScaleTween = mContext.Tweens
            .ScaleTo(point.Rect, Vector3.zero, duration, point.Rect)
            .SetEase(mConfig.ExitEase)
            .SetUpdate(mContext.UseUnscaledTime);

        point.DestroyTween = mContext.Tweens.DelayedCall(duration + 0.02f, () => { DestroyPoint(point, immediate: false); }, point.Rect)
            .SetUpdate(mContext.UseUnscaledTime);
    }

    private void DestroyPoint(TrailPoint point, bool immediate)
    {
        if (point == null)
        {
            return;
        }

        point.ExitFadeTween?.Kill();
        point.ExitScaleTween?.Kill();
        point.DestroyTween?.Kill();
        point.ExitFadeTween = null;
        point.ExitScaleTween = null;
        point.DestroyTween = null;

        if (point.Rect != null)
        {
            if (immediate)
            {
                UnityEngine.Object.Destroy(point.Rect.gameObject);
            }
            else
            {
                UnityEngine.Object.Destroy(point.Rect.gameObject);
            }
        }

        point.Rect = null;
        point.Label = null;
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
