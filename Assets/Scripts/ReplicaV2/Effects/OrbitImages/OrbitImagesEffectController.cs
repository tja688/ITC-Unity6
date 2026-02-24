using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class OrbitImagesEffectController : IReplicaEffect<OrbitImagesEffectConfig, OrbitImagesEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private OrbitImagesEffectConfig mConfig;
    private OrbitImagesEffectView mView;
    private OrbitImagesEffectModel mModel;
    private float mProgress01;
    private bool mInitialized;

    public string EffectId => "orbitimages-v2";

    public void Initialize(ReplicaHostContext context, OrbitImagesEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<OrbitImagesEffectConfig>();
        mModel = OrbitImagesEffectModel.CreateDefault();
        mView = OrbitImagesEffectViewBuilder.Build(mContext.MountRoot, mConfig, mModel);
        mProgress01 = 0f;
        mInitialized = true;
    }

    public void SetModel(OrbitImagesEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model != null ? model.Clone() : OrbitImagesEffectModel.CreateDefault();
        var newCount = safe.Images != null ? safe.Images.Count : 0;
        var oldCount = mView != null && mView.Items != null ? mView.Items.Count : 0;
        if (newCount > 0 && newCount != oldCount)
        {
            RebuildView(safe);
        }
        else
        {
            OrbitImagesEffectViewBuilder.ApplyModel(mView, mConfig, safe);
        }

        mModel = safe;

        if (!animated)
        {
            mView.Group.alpha = 1f;
            mView.Stage.anchoredPosition = Vector2.zero;
        }
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();

        var duration = ResolveDuration(transition, 0.7f);
        var offset = EnterOffset(transition.EnterDirection, mConfig.EnterOffset);

        mView.Stage.anchoredPosition = offset;
        mView.Group.alpha = 0f;

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Stage, Vector2.zero, duration, mView.Stage)
            .SetEase(transition.Ease));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Stage, transition.Ease));
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        if (!mInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        KillTransitionTweens();

        var duration = ResolveDuration(transition, 0.5f);
        var offset = ExitOffset(transition.ExitDirection, mConfig.ExitOffset);

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Stage, offset, duration, mView.Stage)
            .SetEase(transition.Ease)
            .OnComplete(onComplete));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 0f, duration, mView.Stage, transition.Ease));
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized || mView == null || mView.Items == null)
        {
            return;
        }

        if (mModel != null && mModel.Paused)
        {
            return;
        }

        var dt = mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;
        var duration = Mathf.Max(0.1f, mConfig.DurationPerLoop);
        var dir = mConfig.Reverse ? -1f : 1f;
        mProgress01 = Mathf.Repeat(mProgress01 + (dir * (dt / duration)), 1f);

        var count = mView.Items.Count;
        if (count <= 0)
        {
            return;
        }

        var fill = mConfig.Fill;
        var rx = mConfig.RadiusX;
        var ry = mConfig.RadiusY;
        var keepUpright = mConfig.KeepUpright;
        var uprightRotation = keepUpright ? Quaternion.Euler(0f, 0f, -mConfig.OrbitRotationZ) : Quaternion.identity;

        for (var i = 0; i < count; i++)
        {
            var item = mView.Items[i];
            if (item == null || item.Rect == null)
            {
                continue;
            }

            var offset01 = fill ? (i / (float)count) : 0f;
            var t = Mathf.Repeat(mProgress01 + offset01, 1f);
            var angle = t * Mathf.PI * 2f;
            var pos = new Vector2(Mathf.Cos(angle) * rx, Mathf.Sin(angle) * ry);
            item.Rect.anchoredPosition = pos;
            item.Rect.localRotation = uprightRotation;
        }
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

    private void RebuildView(OrbitImagesEffectModel model)
    {
        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mView = OrbitImagesEffectViewBuilder.Build(mContext.MountRoot, mConfig, model);
    }

    private IReplicaTweenHandle FadeCanvasGroup(UnityEngine.CanvasGroup group, float targetAlpha, float duration, UnityEngine.Object owner, ReplicaEase ease)
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

    private static float ResolveDuration(ReplicaTransition transition, float fallback)
    {
        var duration = transition.Duration > 0f ? transition.Duration : fallback;
        return Mathf.Max(0.05f, duration);
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
