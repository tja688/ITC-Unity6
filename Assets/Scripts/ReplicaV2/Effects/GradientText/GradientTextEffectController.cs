using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class GradientTextEffectController : IReplicaEffect<GradientTextEffectConfig, GradientTextEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private GradientTextEffectConfig mConfig;
    private GradientTextEffectView mView;
    private GradientTextEffectModel mModel;

    private bool mInitialized;
    private float mElapsed;
    private bool mPausedByHover;

    private Texture2D mGradientTexture;
    private Sprite mGradientSprite;

    public string EffectId => "gradient-text-v2";

    public void Initialize(ReplicaHostContext context, GradientTextEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<GradientTextEffectConfig>();
        mView = GradientTextEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        SetModel(GradientTextEffectModel.CreateDefault(), false);
        mInitialized = true;
    }

    public void SetModel(GradientTextEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : GradientTextEffectModel.CreateDefault();
        GradientTextEffectViewBuilder.ApplyModel(mView, mConfig, mModel);

        RebuildGradientSprite(mModel.Colors);

        if (mView.TextGradientImage != null)
        {
            mView.TextGradientImage.sprite = mGradientSprite;
            mView.TextGradientImage.preserveAspect = false;
        }

        if (mView.BorderGradientImage != null)
        {
            mView.BorderGradientImage.sprite = mGradientSprite;
            mView.BorderGradientImage.preserveAspect = false;
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
            ApplyGradientMotion();
            return;
        }

        if (mPausedByHover)
        {
            mPausedByHover = false;
        }

        if (mModel.AnimationSpeed > 0.01f)
        {
            mElapsed += dt;
        }

        ApplyGradientMotion();
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

    private void ApplyGradientMotion()
    {
        if (mView == null)
        {
            return;
        }

        var speed = Mathf.Max(0.1f, mModel.AnimationSpeed);
        var p = NormalizedProgress(mElapsed, speed, mModel.Yoyo);

        var tiling = Mathf.Max(1f, mConfig.GradientTiling);

        if (mView.InnerBackground != null)
        {
            ApplyGradientToRect(mView.TextGradientRect, mView.InnerBackground, tiling, mModel.Direction, p);
        }

        if (mModel.ShowBorder && mView.Frame != null && mView.BorderGradientRect != null)
        {
            ApplyGradientToRect(mView.BorderGradientRect, mView.Frame, tiling, mModel.Direction, p);
        }
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

        if (mView?.Frame == null)
        {
            return false;
        }

        if (!mContext.Pointer.TryGetLocalPoint(mView.Frame, out var local))
        {
            return false;
        }

        return mView.Frame.rect.Contains(local);
    }

    private void ApplyGradientToRect(RectTransform gradientRect, RectTransform targetRect, float tiling, GradientTextDirection direction, float p01)
    {
        if (gradientRect == null || targetRect == null)
        {
            return;
        }

        var baseSize = targetRect.rect.size;
        if (baseSize.x <= 0.01f || baseSize.y <= 0.01f)
        {
            baseSize = mConfig.PanelSize;
        }

        gradientRect.anchorMin = new Vector2(0.5f, 0.5f);
        gradientRect.anchorMax = new Vector2(0.5f, 0.5f);
        gradientRect.pivot = new Vector2(0.5f, 0.5f);

        var axis = direction == GradientTextDirection.Vertical ? 1 : 0;
        if (axis == 0)
        {
            gradientRect.sizeDelta = new Vector2(baseSize.x * tiling, baseSize.y);
            var range = Mathf.Max(0f, (gradientRect.sizeDelta.x - baseSize.x) * 0.5f);
            gradientRect.anchoredPosition = new Vector2(Mathf.Lerp(range, -range, p01), 0f);
        }
        else
        {
            gradientRect.sizeDelta = new Vector2(baseSize.x, baseSize.y * tiling);
            var range = Mathf.Max(0f, (gradientRect.sizeDelta.y - baseSize.y) * 0.5f);
            gradientRect.anchoredPosition = new Vector2(0f, Mathf.Lerp(-range, range, p01));
        }
    }

    private void RebuildGradientSprite(Color[] colors)
    {
        var safeColors = colors != null && colors.Length >= 2 ? colors : GradientTextEffectModel.CreateDefault().Colors;
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

        for (var x = 0; x < width; x++)
        {
            var t = width <= 1 ? 0f : x / (float)(width - 1);
            var c = SampleMultiStopGradient(safeColors, t);
            for (var y = 0; y < height; y++)
            {
                mGradientTexture.SetPixel(x, y, c);
            }
        }

        mGradientTexture.Apply(false, false);
        mGradientSprite = Sprite.Create(mGradientTexture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
    }

    private static Color SampleMultiStopGradient(Color[] colors, float t)
    {
        if (colors == null || colors.Length == 0)
        {
            return Color.white;
        }

        if (colors.Length == 1)
        {
            return colors[0];
        }

        t = Mathf.Clamp01(t);
        var count = colors.Length;
        var scaled = t * count;
        var idx = Mathf.Clamp(Mathf.FloorToInt(scaled), 0, count - 1);
        var next = (idx + 1) % count;
        var localT = scaled - idx;
        return Color.Lerp(colors[idx], colors[next], localT);
    }

    private static float NormalizedProgress(float elapsed, float duration, bool yoyo)
    {
        if (duration <= 0.0001f)
        {
            return 0f;
        }

        if (!yoyo)
        {
            return Mathf.Repeat(elapsed / duration, 1f);
        }

        var fullCycle = duration * 2f;
        var t = Mathf.Repeat(elapsed, fullCycle);
        if (t <= duration)
        {
            return t / duration;
        }

        return 1f - ((t - duration) / duration);
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
