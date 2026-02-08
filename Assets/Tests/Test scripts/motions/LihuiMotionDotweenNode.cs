using DG.Tweening;
using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(Transform))]
public sealed class LihuiMotionDotweenNode : MonoBehaviour
{
    [SerializeField]
    private LihuiDotweenEffectId mEffectId;

    private DOTweenAnimation[] mTweens = System.Array.Empty<DOTweenAnimation>();
    private SpriteRenderer mRenderer;
    private Vector3 mDefaultLocalPosition;
    private Vector3 mDefaultLocalScale;
    private Quaternion mDefaultLocalRotation;
    private bool mDefaultsCached;

    public LihuiDotweenEffectId EffectId => mEffectId;

    public bool IsLoopingEffect =>
        mEffectId == LihuiDotweenEffectId.HorizontalHappySway ||
        mEffectId == LihuiDotweenEffectId.AngryTremble;

    public void Configure(LihuiDotweenEffectId effectId, SpriteRenderer sourceRenderer)
    {
        mEffectId = effectId;
        EnsureRenderer(sourceRenderer);
        EnsureTweensConfigured();
        CacheDefaultTransform();
    }

    public void PlayEffect()
    {
        EnsureTweensConfigured();
        CacheDefaultTransform();
        ResetToDefaultTransform();
        gameObject.SetActive(true);

        for (var i = 0; i < mTweens.Length; i++)
        {
            if (mTweens[i] == null)
            {
                continue;
            }

            mTweens[i].RecreateTweenAndPlay();
        }
    }

    public void StopEffect()
    {
        for (var i = 0; i < mTweens.Length; i++)
        {
            if (mTweens[i] == null)
            {
                continue;
            }

            mTweens[i].DOKill();
        }

        transform.DOKill();
        ResetToDefaultTransform();
        gameObject.SetActive(false);
    }

    private void CacheDefaultTransform()
    {
        if (mDefaultsCached)
        {
            return;
        }

        mDefaultLocalPosition = transform.localPosition;
        mDefaultLocalRotation = transform.localRotation;
        mDefaultLocalScale = transform.localScale;
        mDefaultsCached = true;
    }

    private void ResetToDefaultTransform()
    {
        if (!mDefaultsCached)
        {
            CacheDefaultTransform();
        }

        transform.localPosition = mDefaultLocalPosition;
        transform.localRotation = mDefaultLocalRotation;
        transform.localScale = mDefaultLocalScale;
    }

    private void EnsureRenderer(SpriteRenderer sourceRenderer)
    {
        if (!TryGetComponent(out mRenderer))
        {
            mRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        if (sourceRenderer == null)
        {
            return;
        }

        mRenderer.sprite = sourceRenderer.sprite;
        mRenderer.color = sourceRenderer.color;
        mRenderer.flipX = sourceRenderer.flipX;
        mRenderer.flipY = sourceRenderer.flipY;
        mRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
        mRenderer.sortingOrder = sourceRenderer.sortingOrder;
        mRenderer.sharedMaterial = sourceRenderer.sharedMaterial;
    }

    private void EnsureTweensConfigured()
    {
        EnsureTweenComponentCount(GetTweenCount(mEffectId));
        for (var i = 0; i < mTweens.Length; i++)
        {
            ConfigureTween(mTweens[i], mEffectId, i);
        }
    }

    private void EnsureTweenComponentCount(int count)
    {
        var existing = GetComponents<DOTweenAnimation>();
        if (existing.Length < count)
        {
            for (var i = existing.Length; i < count; i++)
            {
                gameObject.AddComponent<DOTweenAnimation>();
            }
        }
        else if (existing.Length > count)
        {
            for (var i = count; i < existing.Length; i++)
            {
                if (Application.isPlaying)
                {
                    Destroy(existing[i]);
                }
                else
                {
                    DestroyImmediate(existing[i]);
                }
            }
        }

        mTweens = GetComponents<DOTweenAnimation>();
    }

    private static int GetTweenCount(LihuiDotweenEffectId effectId)
    {
        switch (effectId)
        {
            case LihuiDotweenEffectId.SurpriseBackLean:
                return 3;
            case LihuiDotweenEffectId.EmphasisForwardPush:
            case LihuiDotweenEffectId.RotateXFallBack:
                return 2;
            default:
                return 1;
        }
    }

    private static void ConfigureTween(DOTweenAnimation tween, LihuiDotweenEffectId effectId, int tweenIndex)
    {
        ApplyBaseConfig(tween);

        switch (effectId)
        {
            case LihuiDotweenEffectId.VerticalNodAgree:
                tween.animationType = DOTweenAnimation.AnimationType.LocalMove;
                tween.endValueV3 = new Vector3(0f, -0.12f, 0f);
                tween.duration = 0.16f;
                tween.easeType = Ease.InOutSine;
                tween.loopType = LoopType.Yoyo;
                tween.loops = 2;
                tween.isRelative = true;
                return;

            case LihuiDotweenEffectId.VerticalHappyBounce:
                tween.animationType = DOTweenAnimation.AnimationType.LocalMove;
                tween.endValueV3 = new Vector3(0f, 0.18f, 0f);
                tween.duration = 0.35f;
                tween.easeType = Ease.OutQuad;
                tween.loopType = LoopType.Yoyo;
                tween.loops = 4;
                tween.isRelative = true;
                return;

            case LihuiDotweenEffectId.HorizontalNoShake:
                tween.animationType = DOTweenAnimation.AnimationType.LocalMove;
                tween.endValueV3 = new Vector3(0.14f, 0f, 0f);
                tween.duration = 0.08f;
                tween.easeType = Ease.InOutSine;
                tween.loopType = LoopType.Yoyo;
                tween.loops = 4;
                tween.isRelative = true;
                return;

            case LihuiDotweenEffectId.HorizontalHappySway:
                tween.animationType = DOTweenAnimation.AnimationType.LocalMove;
                tween.endValueV3 = new Vector3(0.15f, 0f, 0f);
                tween.duration = 1.1f;
                tween.easeType = Ease.InOutSine;
                tween.loopType = LoopType.Yoyo;
                tween.loops = -1;
                tween.isRelative = true;
                return;

            case LihuiDotweenEffectId.AngryTremble:
                tween.animationType = DOTweenAnimation.AnimationType.LocalMove;
                tween.endValueV3 = new Vector3(0.032f, 0f, 0f);
                tween.duration = 0.035f;
                tween.easeType = Ease.Linear;
                tween.loopType = LoopType.Yoyo;
                tween.loops = -1;
                tween.isRelative = true;
                return;

            case LihuiDotweenEffectId.SurpriseBackLean:
                ConfigureSurpriseBackLean(tween, tweenIndex);
                return;

            case LihuiDotweenEffectId.EmphasisForwardPush:
                ConfigureEmphasisForwardPush(tween, tweenIndex);
                return;

            case LihuiDotweenEffectId.EnterSlideIn:
                tween.animationType = DOTweenAnimation.AnimationType.LocalMove;
                tween.endValueV3 = new Vector3(-2.4f, 0f, 0f);
                tween.duration = 0.45f;
                tween.easeType = Ease.OutCubic;
                tween.isRelative = true;
                tween.isFrom = true;
                return;

            case LihuiDotweenEffectId.ExitSlideOut:
                tween.animationType = DOTweenAnimation.AnimationType.LocalMove;
                tween.endValueV3 = new Vector3(2.4f, 0f, 0f);
                tween.duration = 0.4f;
                tween.easeType = Ease.InCubic;
                tween.isRelative = true;
                return;

            case LihuiDotweenEffectId.ZoomInApproach:
                tween.animationType = DOTweenAnimation.AnimationType.Scale;
                tween.endValueV3 = new Vector3(1.12f, 1.12f, 1f);
                tween.duration = 0.24f;
                tween.easeType = Ease.OutQuad;
                tween.loopType = LoopType.Yoyo;
                tween.loops = 2;
                return;

            case LihuiDotweenEffectId.ZoomOutLeave:
                tween.animationType = DOTweenAnimation.AnimationType.Scale;
                tween.endValueV3 = new Vector3(0.86f, 0.86f, 1f);
                tween.duration = 0.3f;
                tween.easeType = Ease.InOutSine;
                tween.loopType = LoopType.Yoyo;
                tween.loops = 2;
                return;

            case LihuiDotweenEffectId.TiltConfused:
                tween.animationType = DOTweenAnimation.AnimationType.LocalRotate;
                tween.endValueV3 = new Vector3(0f, 0f, 11f);
                tween.duration = 0.2f;
                tween.easeType = Ease.InOutSine;
                tween.loopType = LoopType.Yoyo;
                tween.loops = 4;
                tween.isRelative = true;
                tween.optionalRotationMode = RotateMode.Fast;
                return;

            case LihuiDotweenEffectId.RotateYTurnBack:
                tween.animationType = DOTweenAnimation.AnimationType.LocalRotate;
                tween.endValueV3 = new Vector3(0f, 180f, 0f);
                tween.duration = 0.46f;
                tween.easeType = Ease.InOutSine;
                tween.loopType = LoopType.Yoyo;
                tween.loops = 2;
                tween.optionalRotationMode = RotateMode.FastBeyond360;
                return;

            case LihuiDotweenEffectId.RotateXFallBack:
                ConfigureRotateXFallBack(tween, tweenIndex);
                return;
        }
    }

    private static void ConfigureSurpriseBackLean(DOTweenAnimation tween, int tweenIndex)
    {
        if (tweenIndex == 0)
        {
            tween.animationType = DOTweenAnimation.AnimationType.LocalMove;
            tween.endValueV3 = new Vector3(0f, 0.22f, 0f);
            tween.duration = 0.18f;
            tween.easeType = Ease.OutQuad;
            tween.loopType = LoopType.Yoyo;
            tween.loops = 2;
            tween.isRelative = true;
            return;
        }

        if (tweenIndex == 1)
        {
            tween.animationType = DOTweenAnimation.AnimationType.Scale;
            tween.endValueV3 = new Vector3(0.9f, 0.9f, 1f);
            tween.duration = 0.18f;
            tween.easeType = Ease.OutQuad;
            tween.loopType = LoopType.Yoyo;
            tween.loops = 2;
            return;
        }

        tween.animationType = DOTweenAnimation.AnimationType.LocalRotate;
        tween.endValueV3 = new Vector3(0f, 0f, 7f);
        tween.duration = 0.18f;
        tween.easeType = Ease.OutQuad;
        tween.loopType = LoopType.Yoyo;
        tween.loops = 2;
        tween.isRelative = true;
    }

    private static void ConfigureEmphasisForwardPush(DOTweenAnimation tween, int tweenIndex)
    {
        if (tweenIndex == 0)
        {
            tween.animationType = DOTweenAnimation.AnimationType.LocalMove;
            tween.endValueV3 = new Vector3(0f, -0.2f, 0f);
            tween.duration = 0.16f;
            tween.easeType = Ease.OutBack;
            tween.loopType = LoopType.Yoyo;
            tween.loops = 2;
            tween.isRelative = true;
            return;
        }

        tween.animationType = DOTweenAnimation.AnimationType.Scale;
        tween.endValueV3 = new Vector3(1.08f, 1.08f, 1f);
        tween.duration = 0.16f;
        tween.easeType = Ease.OutBack;
        tween.loopType = LoopType.Yoyo;
        tween.loops = 2;
    }

    private static void ConfigureRotateXFallBack(DOTweenAnimation tween, int tweenIndex)
    {
        if (tweenIndex == 0)
        {
            tween.animationType = DOTweenAnimation.AnimationType.LocalRotate;
            tween.endValueV3 = new Vector3(65f, 0f, 0f);
            tween.duration = 0.28f;
            tween.easeType = Ease.InOutSine;
            tween.loopType = LoopType.Yoyo;
            tween.loops = 2;
            tween.optionalRotationMode = RotateMode.Fast;
            return;
        }

        tween.animationType = DOTweenAnimation.AnimationType.LocalMove;
        tween.endValueV3 = new Vector3(0f, -0.36f, 0f);
        tween.duration = 0.28f;
        tween.easeType = Ease.InOutSine;
        tween.loopType = LoopType.Yoyo;
        tween.loops = 2;
        tween.isRelative = true;
    }

    private static void ApplyBaseConfig(DOTweenAnimation tween)
    {
        tween.targetIsSelf = true;
        tween.targetGO = null;
        tween.tweenTargetIsTargetGO = true;
        tween.target = tween.transform;
        tween.targetType = DOTweenAnimation.TargetType.Transform;
        tween.forcedTargetType = DOTweenAnimation.TargetType.Transform;

        tween.delay = 0f;
        tween.duration = 0.3f;
        tween.easeType = Ease.OutQuad;
        tween.loopType = LoopType.Restart;
        tween.loops = 1;
        tween.isRelative = false;
        tween.isFrom = false;
        tween.isSpeedBased = false;

        tween.autoGenerate = true;
        tween.autoPlay = false;
        tween.autoKill = false;
        tween.isIndependentUpdate = false;
        tween.isActive = true;
        tween.isValid = true;
        tween.useTargetAsV3 = false;

        tween.endValueV3 = Vector3.zero;
        tween.optionalBool0 = false;
        tween.optionalBool1 = false;
        tween.optionalFloat0 = 0f;
        tween.optionalInt0 = 0;
        tween.optionalRotationMode = RotateMode.Fast;
        tween.optionalShakeRandomnessMode = ShakeRandomnessMode.Full;
    }
}
