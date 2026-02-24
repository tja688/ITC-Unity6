using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class DecayEffectController : IReplicaEffect<DecayEffectConfig, DecayEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private DecayEffectConfig mConfig;
    private DecayEffectView mView;

    private Vector2 mCurrentOffset;
    private float mCurrentRotationZ;
    private Vector2 mCachedCursor;
    private float mDistortionScale;

    private bool mInitialized;

    public string EffectId => "decay-v2";

    public void Initialize(ReplicaHostContext context, DecayEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<DecayEffectConfig>();
        mView = DecayEffectViewBuilder.Build(mContext.MountRoot, mConfig);

        mCachedCursor = mContext.Pointer != null ? mContext.Pointer.ScreenPosition : Vector2.zero;
        mInitialized = true;
    }

    public void SetModel(DecayEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model ?? DecayEffectModel.CreateDefault();

        mView.TitleText.text = string.IsNullOrWhiteSpace(safe.Title) ? "NEXT GEN" : safe.Title;
        mView.SubtitleText.text = string.IsNullOrWhiteSpace(safe.Subtitle) ? "Pointer-driven distortion" : safe.Subtitle;
        mView.MarkerText.text = string.IsNullOrWhiteSpace(safe.Marker) ? "DECAY" : safe.Marker;

        if (safe.PhotoSprite != null)
        {
            mView.CardImage.sprite = safe.PhotoSprite;
            mView.CardImage.type = mConfig.SpriteImageType;
            mView.CardImage.color = Color.white;
        }
        else
        {
            mView.CardImage.sprite = null;
            mView.CardImage.type = Image.Type.Simple;
            mView.CardImage.color = mConfig.BasePhotoColor;
        }

        if (!animated)
        {
            mView.Group.alpha = 1f;
            mView.Root.anchoredPosition = Vector2.zero;
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

        mView.Root.anchoredPosition = offset;
        mView.Group.alpha = 0f;

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Root, Vector2.zero, duration, mView.Root)
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

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Root, offset, duration, mView.Root)
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
        UpdateCardTilt(dt);
        UpdateDistortion(dt);
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

    private void UpdateCardTilt(float dt)
    {
        if (mView?.CardRoot == null)
        {
            return;
        }

        var normalized = Vector2.zero;
        if (mContext.Pointer != null && mContext.Pointer.TryGetLocalPoint(mView.CardRoot, out var local, mContext.UICamera))
        {
            var halfW = Mathf.Max(1f, mView.CardRoot.rect.width * 0.5f);
            var halfH = Mathf.Max(1f, mView.CardRoot.rect.height * 0.5f);
            normalized = new Vector2(
                Mathf.Clamp(local.x / halfW, -1f, 1f),
                Mathf.Clamp(local.y / halfH, -1f, 1f));
        }

        var mapX = normalized.x * 120f;
        var mapY = normalized.y * 120f;
        var mapRz = normalized.x * 10f;

        var lerpPos = 1f - Mathf.Pow(1f - Mathf.Clamp01(mConfig.PositionLerp), Mathf.Max(1f, dt * 60f));
        var lerpRot = 1f - Mathf.Pow(1f - Mathf.Clamp01(mConfig.RotationLerp), Mathf.Max(1f, dt * 60f));

        var targetX = Mathf.Lerp(mCurrentOffset.x, mapX, lerpPos);
        var targetY = Mathf.Lerp(mCurrentOffset.y, mapY, lerpPos);
        var targetRz = Mathf.Lerp(mCurrentRotationZ, mapRz, lerpRot);

        var bound = Mathf.Max(8f, mConfig.MoveBound);

        if (targetX > bound) targetX = bound + ((targetX - bound) * 0.2f);
        if (targetX < -bound) targetX = -bound + ((targetX + bound) * 0.2f);
        if (targetY > bound) targetY = bound + ((targetY - bound) * 0.2f);
        if (targetY < -bound) targetY = -bound + ((targetY + bound) * 0.2f);

        mCurrentOffset = new Vector2(targetX, targetY);
        mCurrentRotationZ = targetRz;

        mView.CardRoot.anchoredPosition = mCurrentOffset;
        mView.CardRoot.localRotation = Quaternion.Euler(0f, 0f, mCurrentRotationZ);
    }

    private void UpdateDistortion(float dt)
    {
        if (mContext.Pointer == null)
        {
            return;
        }

        var cursor = mContext.Pointer.ScreenPosition;
        var travelled = Vector2.Distance(mCachedCursor, cursor);
        var mapped = Mathf.Clamp01(travelled / Mathf.Max(1f, mConfig.DistortionDistanceMax));

        var lerp = 1f - Mathf.Pow(1f - Mathf.Clamp01(mConfig.DistortionLerp), Mathf.Max(1f, dt * 60f));
        mDistortionScale = Mathf.Lerp(mDistortionScale, mapped, lerp);

        var waveSpeed = Mathf.Max(0.1f, mConfig.StripWaveSpeed);
        var alphaSpeed = Mathf.Max(0.1f, mConfig.StripAlphaSpeed);

        for (var i = 0; i < mView.NoiseStrips.Count; i++)
        {
            var strip = mView.NoiseStrips[i];
            var wave = Mathf.Sin((Time.unscaledTime * waveSpeed) + (i * 0.65f));
            var direction = (i % 2 == 0) ? 1f : -1f;
            var x = direction * wave * (mConfig.StripAmplitude + (i * mConfig.StripAmplitudeStep)) * mDistortionScale;
            strip.anchoredPosition = new Vector2(x, mView.NoiseBaseY[i]);

            var alphaWave = 0.5f + (0.5f * Mathf.Sin((Time.unscaledTime * alphaSpeed) + (i * 0.9f)));
            var c = mView.NoiseImages[i].color;
            c.a = Mathf.Lerp(0.02f, 0.20f, mDistortionScale) * alphaWave;
            mView.NoiseImages[i].color = c;
        }

        mView.CardImage.color = Color.Lerp(mConfig.BasePhotoColor, mConfig.ActivePhotoColor, mDistortionScale * 0.6f);
        mCachedCursor = cursor;
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
