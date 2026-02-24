using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class TargetCursorEffectController : IReplicaEffect<TargetCursorEffectConfig, TargetCursorEffectModel>
{
    private ReplicaHostContext mContext;
    private TargetCursorEffectConfig mConfig;
    private TargetCursorEffectView mView;
    private TargetCursorEffectModel mModel;

    private bool mInitialized;

    private Vector2 mCurrentDotPos;
    private Vector2 mDotVelocity;


    private float mActiveStrength;
    private float mActiveVelocity;

    private float mRotationAngle;
    private float mRotationVelocity;
    private Vector3[] mWorldCorners = new Vector3[4];

    private Vector2[] mCornerCurrentPos = new Vector2[4];
    private Vector2[] mCornerVelocity = new Vector2[4];

    public string EffectId => "targetcursor-v2";

    public void Initialize(ReplicaHostContext context, TargetCursorEffectConfig config)
    {
        if (mInitialized) return;

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<TargetCursorEffectConfig>();
        mView = TargetCursorEffectViewBuilder.Build(mContext.MountRoot, mConfig);


        SetModel(TargetCursorEffectModel.CreateDefault(), false);
        mInitialized = true;

        if (mConfig.HideDefaultCursor)
        {
            Cursor.visible = false;
        }
    }

    public void SetModel(TargetCursorEffectModel model, bool animated = true)
    {
        if (!mInitialized) return;
        mModel = model != null ? model.Clone() : TargetCursorEffectModel.CreateDefault();
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (!mInitialized) return;
        mContext.Tweens.Kill(mView.Group);
        mView.Group.alpha = 0f;


        var duration = Mathf.Max(0.08f, transition.Duration > 0 ? transition.Duration : ReplicaTransition.Default.Duration);
        mContext.Tweens.ToFloat(() => mView.Group.alpha, a => mView.Group.alpha = a, 1f, duration, mView.Group).SetEase(mConfig.EnterEase);
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        if (!mInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        mContext.Tweens.Kill(mView.Group);
        var duration = Mathf.Max(0.08f, transition.Duration > 0 ? transition.Duration : ReplicaTransition.Default.Duration);
        mContext.Tweens.ToFloat(() => mView.Group.alpha, a => mView.Group.alpha = a, 0f, duration, mView.Group).SetEase(mConfig.ExitEase).OnComplete(onComplete);
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized) return;

        var dt = mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;

        // Pointer follow - relative to Root
        if (mContext.Pointer != null && mContext.Pointer.IsPointerValid && mContext.Pointer.TryGetLocalPoint(mView.Root, out var localPoint, null))
        {
            mCurrentDotPos = Vector2.SmoothDamp(mCurrentDotPos, localPoint, ref mDotVelocity, 0.05f, float.MaxValue, dt);
        }

        mView.MotionRoot.anchoredPosition = mCurrentDotPos;
        mView.Dot.anchoredPosition = Vector2.zero;

        // Active target logic
        RectTransform activeTarget = null;
        if (mContext.Pointer != null && mContext.Pointer.IsPointerValid)
        {
            // Check explicit model targets first
            if (mModel.HoverTargets != null)
            {
                for (int i = 0; i < mModel.HoverTargets.Count; i++)
                {
                    var target = mModel.HoverTargets[i];
                    if (target != null && target.gameObject.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(target, mContext.Pointer.ScreenPosition, null))
                    {
                        activeTarget = target;
                        break;
                    }
                }
            }

            // Check auto-discovered globally registered targets if nothing found
            if (activeTarget == null)
            {
                foreach (var target in TargetCursorInteractable.ActiveTargets)
                {
                    if (target != null && target.gameObject.activeInHierarchy && RectTransformUtility.RectangleContainsScreenPoint(target, mContext.Pointer.ScreenPosition, null))
                    {
                        activeTarget = target;
                        break;
                    }
                }
            }
        }

        float targetStrength = activeTarget != null ? 1f : 0f;
        mActiveStrength = Mathf.SmoothDamp(mActiveStrength, targetStrength, ref mActiveVelocity, mConfig.HoverDuration, float.MaxValue, dt);

        // Rotation
        if (mConfig.SpinDuration > 0)
        {
            if (activeTarget != null)
            {
                // Find the nearest multiple of 90 degrees to snap to
                // Since the square is symmetrical, snapping to nearest 90deg is visually identical to snapping to 0,
                // but avoids long un-spin animations
                float nearest90 = Mathf.Round(mRotationAngle / 90f) * 90f;
                // Smoothly snap rotation to the nearest 90-degree angle
                mRotationAngle = Mathf.SmoothDampAngle(mRotationAngle, nearest90, ref mRotationVelocity, 0.12f, float.MaxValue, dt);
            }
            else
            {
                // Continuously spin when idle
                mRotationAngle -= (360f / mConfig.SpinDuration) * dt;
            }

            mView.MotionRoot.localRotation = Quaternion.Euler(0, 0, mRotationAngle);
        }

        float cx = mConfig.CornerSize;
        Vector2[] targetCornerPos = new Vector2[4]
        {
            new Vector2(-cx, cx), // TL
            new Vector2(cx, cx), // TR
            new Vector2(cx, -cx), // BR
            new Vector2(-cx, -cx) // BL
        };

        if (activeTarget != null)
        {
            float nearest90 = Mathf.Round(mRotationAngle / 90f) * 90f;
            int steps = Mathf.RoundToInt(nearest90 / 90f);

            activeTarget.GetWorldCorners(mWorldCorners);
            // GetWorldCorners returns: 0: BottomLeft, 1: TopLeft, 2: TopRight, 3: BottomRight

            Vector2 bl = mView.MotionRoot.InverseTransformPoint(mWorldCorners[0]);
            Vector2 tl = mView.MotionRoot.InverseTransformPoint(mWorldCorners[1]);
            Vector2 tr = mView.MotionRoot.InverseTransformPoint(mWorldCorners[2]);
            Vector2 br = mView.MotionRoot.InverseTransformPoint(mWorldCorners[3]);

            var border = mConfig.BorderWidth;
            var cornerSize = mConfig.CornerSize;
            float cxhalf = cornerSize * 0.5f;

            Vector2[] baseTargets = new Vector2[4];
            baseTargets[0] = tl + new Vector2(cxhalf - border, -cxhalf + border); // TL
            baseTargets[1] = tr + new Vector2(-cxhalf + border, -cxhalf + border);  // TR
            baseTargets[2] = br + new Vector2(-cxhalf + border, cxhalf - border); // BR
            baseTargets[3] = bl + new Vector2(cxhalf - border, cxhalf - border); // BL

            if (mConfig.ParallaxOn && mActiveStrength >= 0.8f) // Relaxed parallax activation threshold slightly for smoother snapping
            {
                Vector2 targetCenterLocal = mView.MotionRoot.InverseTransformPoint(activeTarget.position);
                Vector2 parallaxOffset = -targetCenterLocal * 0.1f;
                for (int i = 0; i < 4; i++)
                {
                    baseTargets[i] += parallaxOffset;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                // Dynamic mapping of array indexes based on rotation steps
                int cornerIndex = ((i + steps) % 4 + 4) % 4;
                targetCornerPos[cornerIndex] = baseTargets[i];
            }
        }

        float smoothTime = activeTarget != null ? 0.08f : 0.2f;
        for (int i = 0; i < 4; i++)
        {
            mCornerCurrentPos[i] = Vector2.SmoothDamp(mCornerCurrentPos[i], targetCornerPos[i], ref mCornerVelocity[i], smoothTime, float.MaxValue, dt);
            mView.Corners[i].anchoredPosition = mCornerCurrentPos[i];
        }
    }

    public void Dispose()
    {
        if (!mInitialized) return;

        if (mConfig.HideDefaultCursor)
        {
            Cursor.visible = true;
        }

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Group);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mInitialized = false;
    }
}
