using System;
using UnityEngine;

public sealed class MagnetEffectController : IReplicaEffect<MagnetEffectConfig, MagnetEffectModel>
{
    private ReplicaHostContext mContext;
    private MagnetEffectConfig mConfig;
    private MagnetEffectView mView;
    private MagnetEffectModel mModel;

    private bool mInitialized;
    private bool mIsActive;
    private Vector2 mCurrentTargetPos;
    private IReplicaTweenHandle mPositionTween;

    public string EffectId => "magnet-v2";

    public void Initialize(ReplicaHostContext context, MagnetEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<MagnetEffectConfig>();
        mModel = MagnetEffectModel.CreateDefault();
        mView = MagnetEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        mInitialized = true;
    }

    public void SetModel(MagnetEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model ?? MagnetEffectModel.CreateDefault();
        mModel.Position = safe.Position;
        mModel.IsActive = safe.IsActive;
        mModel.Title = safe.Title;
        mModel.Hint = safe.Hint;

        if (mView.ButtonLabel != null)
        {
            mView.ButtonLabel.text = string.IsNullOrWhiteSpace(mModel.Title) ? "Hover Me" : mModel.Title;
        }

        if (mView.HintText != null)
        {
            mView.HintText.text = string.IsNullOrWhiteSpace(mModel.Hint) ? "Move pointer near the button" : mModel.Hint;
        }

        if (!animated)
        {
            KillPositionTween();
            if (mView.InnerContainer != null)
            {
                mView.InnerContainer.anchoredPosition = mModel.Position;
            }
        }
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (mView?.Root != null)
        {
            mView.Root.gameObject.SetActive(true);
        }
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        if (mView?.Root != null)
        {
            mView.Root.gameObject.SetActive(false);
        }

        onComplete?.Invoke();
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized)
        {
            return;
        }

        if (mConfig.Disabled)
        {
            if (mIsActive)
            {
                mIsActive = false;
                LerpToPosition(Vector2.zero, mConfig.InactiveDuration, mConfig.InactiveEase);
            }

            return;
        }

        var dt = mContext != null && mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;
        UpdateMagnet(Mathf.Max(0.0001f, dt));
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillPositionTween();

        if (mView?.Root != null)
        {
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mContext = null;
        mConfig = null;
        mView = null;
        mModel = null;
        mIsActive = false;
        mCurrentTargetPos = Vector2.zero;
        mInitialized = false;
    }

    private void UpdateMagnet(float deltaTime)
    {
        if (mView?.Surface == null || mContext.Pointer == null || !mContext.Pointer.IsPointerValid)
        {
            if (mIsActive)
            {
                mIsActive = false;
                LerpToPosition(Vector2.zero, mConfig.InactiveDuration, mConfig.InactiveEase);
            }

            return;
        }

        if (!mContext.Pointer.TryGetLocalPoint(mView.Surface, out var localPoint, mContext.UICamera))
        {
            return;
        }

        var rect = mView.Surface.rect;
        var halfW = Mathf.Max(1f, rect.width * 0.5f);
        var halfH = Mathf.Max(1f, rect.height * 0.5f);
        var distX = Mathf.Abs(localPoint.x);
        var distY = Mathf.Abs(localPoint.y);
        var inRange = distX <= halfW + Mathf.Max(0f, mConfig.Padding) &&
                      distY <= halfH + Mathf.Max(0f, mConfig.Padding);

        if (inRange)
        {
            var safeStrength = Mathf.Max(0.01f, mConfig.MagnetStrength);
            var targetPos = new Vector2(localPoint.x / safeStrength, localPoint.y / safeStrength);
            mCurrentTargetPos = ClampOffset(targetPos);

            if (!mIsActive)
            {
                mIsActive = true;
                LerpToPosition(mCurrentTargetPos, mConfig.ActiveDuration, mConfig.ActiveEase);
            }
            else
            {
                KillPositionTween();
                UpdatePositionDirectly(mCurrentTargetPos, deltaTime);
            }
        }
        else if (mIsActive)
        {
            mIsActive = false;
            LerpToPosition(Vector2.zero, mConfig.InactiveDuration, mConfig.InactiveEase);
        }
    }

    private void LerpToPosition(Vector2 targetPos, float duration, ReplicaEase ease)
    {
        KillPositionTween();
        if (mView?.InnerContainer == null)
        {
            return;
        }

        var anchor = mView.Surface != null ? mView.Surface : mView.Root;
        mCurrentTargetPos = ClampOffset(targetPos);
        mPositionTween = mContext.Tweens
            .AnchoredPosTo(mView.InnerContainer, mCurrentTargetPos, Mathf.Max(0.01f, duration), anchor)
            .SetEase(ease);
    }

    private void UpdatePositionDirectly(Vector2 targetPos, float deltaTime)
    {
        if (mView?.InnerContainer == null)
        {
            return;
        }

        var current = mView.InnerContainer.anchoredPosition;
        var target = ClampOffset(targetPos);
        var t = Mathf.Clamp01(1f - Mathf.Pow(0.001f, deltaTime / 0.3f));
        mView.InnerContainer.anchoredPosition = Vector2.Lerp(current, target, t);
    }

    private Vector2 ClampOffset(Vector2 offset)
    {
        var maxOffset = Mathf.Max(0f, mConfig.MaxOffset);
        if (maxOffset <= 0f)
        {
            return offset;
        }

        var mag = offset.magnitude;
        if (mag <= maxOffset || mag <= 0.0001f)
        {
            return offset;
        }

        return offset * (maxOffset / mag);
    }

    private void KillPositionTween()
    {
        mPositionTween?.Kill();
        mPositionTween = null;
    }
}
