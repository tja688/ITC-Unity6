using UnityEngine;
using DG.Tweening;

/// <summary>
/// Drives a DOTweenAnimation on the same GameObject to spin around Y forever.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(DOTweenAnimation))]
public class CowboySpinTweenController : MonoBehaviour
{
    [Header("Spin Settings")]
    [SerializeField, Tooltip("Seconds per 360 degrees"), Min(0.01f)]
    private float mDuration = 2f;

    [SerializeField, Tooltip("Ease applied to the rotation")]
    private Ease mEase = Ease.Linear;

    [SerializeField, Tooltip("Use local rotation (true) or world rotation (false)")]
    private bool mUseLocalRotation = true;

    private DOTweenAnimation mTween;

    private void Awake()
    {
        if (!TryGetComponent(out mTween))
        {
            mTween = gameObject.AddComponent<DOTweenAnimation>();
        }

        ConfigureTween();
    }

    private void Start()
    {
        if (mTween != null)
        {
            mTween.RecreateTweenAndPlay();
        }
    }

    private void ConfigureTween()
    {
        mTween.targetIsSelf = true;
        mTween.tweenTargetIsTargetGO = true;
        mTween.targetGO = null;
        mTween.target = transform;
        mTween.targetType = DOTweenAnimation.TargetType.Transform;
        mTween.forcedTargetType = DOTweenAnimation.TargetType.Transform;
        mTween.isValid = true;

        mTween.animationType = mUseLocalRotation
            ? DOTweenAnimation.AnimationType.LocalRotate
            : DOTweenAnimation.AnimationType.Rotate;
        mTween.endValueV3 = new Vector3(0f, 360f, 0f);
        mTween.optionalRotationMode = RotateMode.FastBeyond360;

        mTween.duration = mDuration;
        mTween.easeType = mEase;
        mTween.loopType = LoopType.Incremental;
        mTween.loops = -1;
        mTween.isRelative = true;
        mTween.isFrom = false;

        mTween.delay = 0f;
        mTween.autoGenerate = true;
        mTween.autoPlay = false;
        mTween.autoKill = false;
        mTween.isIndependentUpdate = false;
    }
}
