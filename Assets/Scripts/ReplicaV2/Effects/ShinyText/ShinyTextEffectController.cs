using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ShinyTextEffectController : IReplicaEffect<ShinyTextEffectConfig, ShinyTextEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private ShinyTextEffectConfig mConfig;
    private ShinyTextEffectView mView;
    private ShinyTextEffectModel mModel;

    private bool mInitialized;
    private float mElapsed;
    private bool mPausedByHover;

    private Texture2D mGradientTexture;
    private Sprite mGradientSprite;

    public string EffectId => "shiny-text-v2";

    public void Initialize(ReplicaHostContext context, ShinyTextEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<ShinyTextEffectConfig>();
        mView = ShinyTextEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        SetModel(ShinyTextEffectModel.CreateDefault(), false);
        mInitialized = true;
    }

    public void SetModel(ShinyTextEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : ShinyTextEffectModel.CreateDefault();
        ShinyTextEffectViewBuilder.ApplyModel(mView, mConfig, mModel);

        RebuildGradientSprite(mModel.BaseColor, mModel.ShineColor);

        if (mView.TextGradientImage != null)
        {
            mView.TextGradientImage.sprite = mGradientSprite;
            mView.TextGradientImage.preserveAspect = false;
        }

        mElapsed = 0f;
        mPausedByHover = false;

        if (!animated)
        {
            mView.Group.alpha = 1f;
            mView.Content.anchoredPosition = Vector2.zero;
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
        var basePos = Vector2.zero;

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

        var paused = ShouldPauseByHover();
        if (paused)
        {
            mPausedByHover = true;
            ApplyShineMotion();
            return;
        }

        if (mPausedByHover)
        {
            mPausedByHover = false;
        }

        if (!mModel.Disabled && mModel.Speed > 0.01f)
        {
            mElapsed += dt;
        }

        ApplyShineMotion();
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

        if (mGradientSprite != null)
        {
            UnityEngine.Object.Destroy(mGradientSprite);
            mGradientSprite = null;
        }

        if (mGradientTexture != null)
        {
            UnityEngine.Object.Destroy(mGradientTexture);
            mGradientTexture = null;
        }

        mInitialized = false;
    }

    private void ApplyShineMotion()
    {
        if (mView?.TextGradientRect == null || mView.Content == null)
        {
            return;
        }

        var duration = Mathf.Max(0.1f, mModel.Speed);
        var delay = Mathf.Max(0f, mModel.Delay);

        var p = ComputeCycleProgress(mElapsed, duration, delay, mModel.Yoyo);
        if (mModel.Direction == ShinyTextDirection.Right)
        {
            p = 1f - p;
        }

        var tiling = Mathf.Max(1f, mConfig.GradientTiling);

        var baseSize = mView.Content.rect.size;
        if (baseSize.x <= 0.01f || baseSize.y <= 0.01f)
        {
            baseSize = mConfig.ContainerSize;
        }

        var gradientRect = mView.TextGradientRect;
        gradientRect.anchorMin = new Vector2(0.5f, 0.5f);
        gradientRect.anchorMax = new Vector2(0.5f, 0.5f);
        gradientRect.pivot = new Vector2(0.5f, 0.5f);
        gradientRect.sizeDelta = new Vector2(baseSize.x * tiling, baseSize.y);

        var range = Mathf.Max(0f, (gradientRect.sizeDelta.x - baseSize.x) * 0.5f);
        gradientRect.anchoredPosition = new Vector2(Mathf.Lerp(range, -range, p), 0f);
    }

    private bool ShouldPauseByHover()
    {
        if (!mModel.PauseOnHover)
        {
            return false;
        }

        if (mContext.Pointer == null || !mContext.Pointer.IsPointerValid)
        {
            return false;
        }

        if (mView?.Content == null)
        {
            return false;
        }

        if (!mContext.Pointer.TryGetLocalPoint(mView.Content, out var local))
        {
            return false;
        }

        return mView.Content.rect.Contains(local);
    }

    private void RebuildGradientSprite(Color baseColor, Color shineColor)
    {
        var width = Mathf.Max(32, mConfig.GradientTextureWidth);
        var height = Mathf.Max(8, mConfig.GradientTextureHeight);

        if (mGradientTexture != null)
        {
            UnityEngine.Object.Destroy(mGradientTexture);
            mGradientTexture = null;
        }

        if (mGradientSprite != null)
        {
            UnityEngine.Object.Destroy(mGradientSprite);
            mGradientSprite = null;
        }

        mGradientTexture = new Texture2D(width, height, TextureFormat.RGBA32, false)
        {
            wrapMode = TextureWrapMode.Clamp,
            filterMode = FilterMode.Bilinear
        };

        var a0 = Mathf.Clamp01(0.35f);
        var a1 = Mathf.Clamp01(0.50f);
        var a2 = Mathf.Clamp01(0.65f);

        for (var x = 0; x < width; x++)
        {
            var t = width <= 1 ? 0f : x / (float)(width - 1);
            var c = baseColor;
            if (t < a0)
            {
                c = baseColor;
            }
            else if (t < a1)
            {
                c = Color.Lerp(baseColor, shineColor, Mathf.InverseLerp(a0, a1, t));
            }
            else if (t < a2)
            {
                c = Color.Lerp(shineColor, baseColor, Mathf.InverseLerp(a1, a2, t));
            }
            else
            {
                c = baseColor;
            }

            for (var y = 0; y < height; y++)
            {
                mGradientTexture.SetPixel(x, y, c);
            }
        }

        mGradientTexture.Apply(false, false);
        mGradientSprite = Sprite.Create(mGradientTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
    }

    private static float ComputeCycleProgress(float elapsed, float duration, float delay, bool yoyo)
    {
        if (duration <= 0.0001f)
        {
            return 0f;
        }

        if (!yoyo)
        {
            var cycle = duration + delay;
            var t = Mathf.Repeat(elapsed, cycle);
            if (t <= duration)
            {
                return t / duration;
            }

            return 1f;
        }

        var phase = duration + delay;
        var full = phase * 2f;
        var t2 = Mathf.Repeat(elapsed, full);
        if (t2 <= duration)
        {
            return t2 / duration;
        }

        if (t2 <= phase)
        {
            return 1f;
        }

        var reverseTime = t2 - phase;
        if (reverseTime <= duration)
        {
            return 1f - (reverseTime / duration);
        }

        return 0f;
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
