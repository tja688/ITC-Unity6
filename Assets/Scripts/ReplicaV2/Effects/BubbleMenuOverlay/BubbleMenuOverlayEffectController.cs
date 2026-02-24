using System;
using UnityEngine;
using UnityEngine.UI;

public sealed class BubbleMenuOverlayEffectController : IReplicaEffect<BubbleMenuOverlayEffectConfig, BubbleMenuOverlayEffectModel>
{
    private ReplicaHostContext mContext;
    private BubbleMenuOverlayEffectConfig mConfig;
    private BubbleMenuOverlayEffectView mView;
    private BubbleMenuOverlayEffectModel mModel;

    private bool mInitialized;
    private bool mMenuOpen;

    public string EffectId => "menu-bubble-overlay-v2";

    public void Initialize(ReplicaHostContext context, BubbleMenuOverlayEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<BubbleMenuOverlayEffectConfig>();
        mModel = BubbleMenuOverlayEffectModel.CreateDefault();
        mView = BubbleMenuOverlayEffectViewBuilder.Build(mContext.MountRoot, mConfig, this, mModel);
        SetMenuOpen(false, true);
        mInitialized = true;
    }

    public void SetModel(BubbleMenuOverlayEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : BubbleMenuOverlayEffectModel.CreateDefault();
        RebuildView();
    }

    public void PlayIn(ReplicaTransition transition)
    {
    }

    public void PlayOut(ReplicaTransition transition, Action onComplete = null)
    {
        onComplete?.Invoke();
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        DestroyView();
        mInitialized = false;
    }

    public void ToggleMenu()
    {
        SetMenuOpen(!mMenuOpen, false);
    }

    public void OnPillPointerEnter(int index)
    {
        if (!mMenuOpen || !TryGetPill(index, out var pill))
        {
            return;
        }

        mContext.Tweens.Kill(pill.Rect);
        mContext.Tweens.Kill(pill.Image);
        mContext.Tweens.Kill(pill.Label);

        mContext.Tweens
            .ScaleTo(pill.Rect, Vector3.one * mConfig.HoverScale, mConfig.HoverDuration, pill.Rect)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime);
        mContext.Tweens
            .ColorImageTo(pill.Image, pill.HoverBg, mConfig.HoverDuration, pill.Image)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime);
        ColorTextTo(pill.Label, pill.Label.color, pill.HoverText, mConfig.HoverDuration, pill.Label, ReplicaEase.OutCubic);
    }

    public void OnPillPointerExit(int index)
    {
        if (!mMenuOpen || !TryGetPill(index, out var pill))
        {
            return;
        }

        ResetPillVisual(pill, false);
    }

    public void OnPillPointerDown(int index)
    {
        if (!mMenuOpen || !TryGetPill(index, out var pill))
        {
            return;
        }

        mContext.Tweens.Kill(pill.Rect);
        mContext.Tweens
            .ScaleTo(pill.Rect, Vector3.one * mConfig.DownScale, mConfig.DownDuration, pill.Rect)
            .SetEase(ReplicaEase.OutQuad)
            .SetUpdate(mContext.UseUnscaledTime);
    }

    public void OnPillPointerUp(int index)
    {
        if (!mMenuOpen || !TryGetPill(index, out var pill))
        {
            return;
        }

        mContext.Tweens.Kill(pill.Rect);
        mContext.Tweens
            .ScaleTo(pill.Rect, Vector3.one * mConfig.HoverScale, mConfig.UpDuration, pill.Rect)
            .SetEase(ReplicaEase.OutQuad)
            .SetUpdate(mContext.UseUnscaledTime);
    }

    private void SetMenuOpen(bool open, bool immediate)
    {
        if (!mInitialized || mView == null)
        {
            return;
        }

        mMenuOpen = open;
        mView.OverlayGroup.interactable = open;
        mView.OverlayGroup.blocksRaycasts = open;

        AnimateMenuIcon(open, immediate);

        if (immediate)
        {
            mView.OverlayGroup.alpha = open ? 1f : 0f;
            for (var i = 0; i < mView.Pills.Count; i++)
            {
                var pill = mView.Pills[i];
                pill.Rect.localScale = open ? Vector3.one : Vector3.zero;
                pill.LabelRect.anchoredPosition = open ? Vector2.zero : mConfig.PillLabelClosedOffset;
                var color = pill.Label.color;
                color.a = open ? 1f : 0f;
                pill.Label.color = color;
                if (open)
                {
                    ResetPillVisual(pill, true);
                }
            }

            return;
        }

        mContext.Tweens.Kill(mView.OverlayGroup);
        var overlayDuration = open ? mConfig.OverlayFadeInDuration : mConfig.OverlayFadeOutDuration;
        FadeCanvasGroup(mView.OverlayGroup, open ? 1f : 0f, overlayDuration, mView.OverlayGroup, open ? ReplicaEase.OutQuad : ReplicaEase.InQuad);

        for (var i = 0; i < mView.Pills.Count; i++)
        {
            var pill = mView.Pills[i];
            mContext.Tweens.Kill(pill.Rect);
            mContext.Tweens.Kill(pill.Label);
            mContext.Tweens.Kill(pill.Image);
            if (open)
            {
                ResetPillVisual(pill, true);
                var delay = (i * mConfig.StaggerDelay) + (((i % 2) - 0.5f) * 0.04f);
                pill.Rect.localScale = Vector3.zero;
                mContext.Tweens
                    .ScaleTo(pill.Rect, Vector3.one, mConfig.OpenDuration, pill.Rect)
                    .SetDelay(delay)
                    .SetEase(ReplicaEase.OutBack, 1.2f)
                    .SetUpdate(mContext.UseUnscaledTime);

                pill.LabelRect.anchoredPosition = mConfig.PillLabelClosedOffset;
                mContext.Tweens
                    .AnchoredPosYTo(pill.LabelRect, 0f, mConfig.OpenDuration * 0.9f, pill.LabelRect)
                    .SetDelay(delay + 0.02f)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);

                var c = pill.Label.color;
                c.a = 0f;
                pill.Label.color = c;
                mContext.Tweens
                    .FadeTextTo(pill.Label, 1f, mConfig.OpenDuration * 0.8f, pill.Label)
                    .SetDelay(delay + 0.02f)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
            }
            else
            {
                var delay = (mView.Pills.Count - i - 1) * 0.025f;
                mContext.Tweens
                    .FadeTextTo(pill.Label, 0f, 0.18f, pill.Label)
                    .SetDelay(delay)
                    .SetEase(ReplicaEase.InQuad)
                    .SetUpdate(mContext.UseUnscaledTime);

                mContext.Tweens
                    .ScaleTo(pill.Rect, Vector3.zero, 0.20f, pill.Rect)
                    .SetDelay(delay)
                    .SetEase(ReplicaEase.InBack, 1.2f)
                    .SetUpdate(mContext.UseUnscaledTime);
            }
        }
    }

    private void AnimateMenuIcon(bool open, bool immediate)
    {
        var topY = open ? 0f : 5f;
        var bottomY = open ? 0f : -5f;
        var topRot = open ? 45f : 0f;
        var bottomRot = open ? -45f : 0f;

        mContext.Tweens.Kill(mView.LineTop);
        mContext.Tweens.Kill(mView.LineBottom);

        if (immediate)
        {
            mView.LineTop.anchoredPosition = new Vector2(0f, topY);
            mView.LineTop.localRotation = Quaternion.Euler(0f, 0f, topRot);
            mView.LineBottom.anchoredPosition = new Vector2(0f, bottomY);
            mView.LineBottom.localRotation = Quaternion.Euler(0f, 0f, bottomRot);
            return;
        }

        mContext.Tweens
            .AnchoredPosYTo(mView.LineTop, topY, mConfig.IconMorphDuration, mView.LineTop)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime);
        mContext.Tweens
            .AnchoredPosYTo(mView.LineBottom, bottomY, mConfig.IconMorphDuration, mView.LineBottom)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime);
        mContext.Tweens
            .LocalRotateTo(mView.LineTop, new Vector3(0f, 0f, topRot), mConfig.IconMorphDuration, mView.LineTop)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime);
        mContext.Tweens
            .LocalRotateTo(mView.LineBottom, new Vector3(0f, 0f, bottomRot), mConfig.IconMorphDuration, mView.LineBottom)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime);
    }

    private void ResetPillVisual(BubbleMenuOverlayPillView pill, bool immediate)
    {
        if (pill == null)
        {
            return;
        }

        mContext.Tweens.Kill(pill.Rect);
        mContext.Tweens.Kill(pill.Image);
        mContext.Tweens.Kill(pill.Label);

        if (immediate)
        {
            pill.Rect.localScale = Vector3.one;
            pill.Image.color = pill.BaseBg;
            pill.Label.color = pill.BaseText;
            return;
        }

        mContext.Tweens
            .ScaleTo(pill.Rect, Vector3.one, 0.16f, pill.Rect)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime);
        mContext.Tweens
            .ColorImageTo(pill.Image, pill.BaseBg, 0.16f, pill.Image)
            .SetEase(ReplicaEase.OutCubic)
            .SetUpdate(mContext.UseUnscaledTime);
        ColorTextTo(pill.Label, pill.Label.color, pill.BaseText, 0.16f, pill.Label, ReplicaEase.OutCubic);
    }

    private bool TryGetPill(int index, out BubbleMenuOverlayPillView pill)
    {
        pill = null;
        if (mView == null || index < 0 || index >= mView.Pills.Count)
        {
            return false;
        }

        pill = mView.Pills[index];
        return pill != null;
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
            .SetEase(ease)
            .SetUpdate(mContext.UseUnscaledTime);
    }

    private IReplicaTweenHandle ColorTextTo(Text target, Color start, Color end, float duration, UnityEngine.Object owner, ReplicaEase ease)
    {
        var t = 0f;
        return mContext.Tweens
            .ToFloat(
                () => t,
                value =>
                {
                    t = value;
                    target.color = Color.Lerp(start, end, t);
                },
                1f,
                duration,
                owner)
            .SetEase(ease)
            .SetUpdate(mContext.UseUnscaledTime);
    }

    private void RebuildView()
    {
        DestroyView();
        mView = BubbleMenuOverlayEffectViewBuilder.Build(mContext.MountRoot, mConfig, this, mModel);
        SetMenuOpen(false, true);
    }

    private void DestroyView()
    {
        if (mView == null)
        {
            return;
        }

        if (mView.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mView = null;
    }
}
