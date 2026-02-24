using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class NoiseEffectController : IReplicaEffect<NoiseEffectConfig, NoiseEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private NoiseEffectConfig mConfig;
    private NoiseEffectView mView;
    private NoiseEffectModel mModel;

    private Texture2D mNoiseTexture;
    private Color32[] mPixels;
    private int mTextureSize;
    private int mFrame;
    private System.Random mRandom;
    private bool mInitialized;

    public string EffectId => "noise-v2";

    public void Initialize(ReplicaHostContext context, NoiseEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<NoiseEffectConfig>();
        mModel = NoiseEffectModel.CreateDefault();
        mView = NoiseEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        mRandom = new System.Random(Environment.TickCount);
        EnsureTexture();
        RenderNoise();
        ApplyAlpha();
        mInitialized = true;
    }

    public void SetModel(NoiseEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : NoiseEffectModel.CreateDefault();
        ApplyAlpha();

        if (!animated)
        {
            mView.Group.alpha = 1f;
        }
    }

    public void PlayIn(ReplicaTransition transition)
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        mView.Group.alpha = 0f;
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, ResolveDuration(transition, 0.4f), mView.Root, transition.Ease));
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        if (!mInitialized)
        {
            onComplete?.Invoke();
            return;
        }

        KillTransitionTweens();
        var duration = ResolveDuration(transition, 0.3f);
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 0f, duration, mView.Root, transition.Ease).OnComplete(onComplete));
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized)
        {
            return;
        }

        if (mModel != null && mModel.Paused)
        {
            return;
        }

        mFrame++;
        var interval = Mathf.Max(1, mConfig.RefreshFrameInterval);
        if (mFrame % interval != 0)
        {
            return;
        }

        EnsureTexture();
        RenderNoise();
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

        if (mNoiseTexture != null)
        {
            UnityEngine.Object.Destroy(mNoiseTexture);
            mNoiseTexture = null;
        }

        mPixels = null;
        mInitialized = false;
    }

    private void EnsureTexture()
    {
        var size = Mathf.Clamp(mConfig.TextureSize, 64, 2048);
        if (mNoiseTexture != null && mTextureSize == size)
        {
            return;
        }

        mTextureSize = size;
        if (mNoiseTexture != null)
        {
            UnityEngine.Object.Destroy(mNoiseTexture);
        }

        mNoiseTexture = new Texture2D(size, size, TextureFormat.ARGB32, false)
        {
            wrapMode = TextureWrapMode.Repeat
        };
        mNoiseTexture.filterMode = mConfig.Pixelated ? FilterMode.Point : FilterMode.Bilinear;
        mNoiseTexture.anisoLevel = 0;

        mPixels = new Color32[size * size];
        if (mView != null && mView.NoiseImage != null)
        {
            mView.NoiseImage.texture = mNoiseTexture;
        }
    }

    private void RenderNoise()
    {
        if (mNoiseTexture == null || mPixels == null)
        {
            return;
        }

        var total = mPixels.Length;
        for (var i = 0; i < total; i++)
        {
            var v = (byte)mRandom.Next(0, 256);
            mPixels[i] = new Color32(v, v, v, 255);
        }

        mNoiseTexture.SetPixels32(mPixels);
        mNoiseTexture.Apply(false, false);
    }

    private void ApplyAlpha()
    {
        if (mView == null || mView.NoiseImage == null)
        {
            return;
        }

        var alpha = Mathf.Clamp01((mConfig.NoiseAlphaByte / 255f) * (mModel != null ? mModel.AlphaMultiplier : 1f));
        var c = mConfig.Tint;
        c.a = alpha;
        mView.NoiseImage.color = c;
        mView.NoiseImage.uvRect = new Rect(0f, 0f, Mathf.Max(0.01f, mConfig.PatternScale.x), Mathf.Max(0.01f, mConfig.PatternScale.y));
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
}
