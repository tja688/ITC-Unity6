using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class GlassIconsTiltEffectController : IReplicaEffect<GlassIconsTiltEffectConfig, GlassIconsTiltEffectModel>
{
    private sealed class IconRuntime
    {
        public GlassIconsTiltEffectIconView View;
        public bool Hovered;
    }

    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();
    private readonly List<IconRuntime> mIcons = new List<IconRuntime>();

    private ReplicaHostContext mContext;
    private GlassIconsTiltEffectConfig mConfig;
    private GlassIconsTiltEffectView mView;
    private bool mInitialized;

    private Vector2 mBaseContentPos;

    public string EffectId => "glass-icons-tilt-v2";

    public void Initialize(ReplicaHostContext context, GlassIconsTiltEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<GlassIconsTiltEffectConfig>();
        mView = GlassIconsTiltEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        mBaseContentPos = mView.Content.anchoredPosition;
        mInitialized = true;
    }

    public void SetModel(GlassIconsTiltEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var safe = model != null ? model : GlassIconsTiltEffectModel.CreateDefault();
        if (mView.HintText != null)
        {
            mView.HintText.text = string.IsNullOrWhiteSpace(safe.Hint) ? "GlassIcons  |  Hover each icon tile" : safe.Hint;
        }

        RebuildIconsIfNeeded(safe);
        ResetAllVisuals();
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

        mView.Content.anchoredPosition = mBaseContentPos + offset;
        mView.Group.alpha = 0f;

        mTransitionTweens.Add(mContext.Tweens
            .AnchoredPosTo(mView.Content, mBaseContentPos, duration, mView.Root)
            .SetEase(transition.Ease)
            .SetUpdate(mContext.UseUnscaledTime));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Root, transition.Ease).SetUpdate(mContext.UseUnscaledTime));
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
            .AnchoredPosTo(mView.Content, mBaseContentPos + offset, duration, mView.Root)
            .SetEase(transition.Ease)
            .SetUpdate(mContext.UseUnscaledTime)
            .OnComplete(onComplete));
        mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 0f, duration, mView.Root, transition.Ease).SetUpdate(mContext.UseUnscaledTime));
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized)
        {
            return;
        }

        UpdateHoverStates();
    }

    public void Dispose()
    {
        if (!mInitialized)
        {
            return;
        }

        KillTransitionTweens();
        KillAllIconTweens();

        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mIcons.Clear();
        mInitialized = false;
    }

    private void RebuildIconsIfNeeded(GlassIconsTiltEffectModel model)
    {
        var count = model.Items != null ? model.Items.Count : 0;
        if (count <= 0)
        {
            model = GlassIconsTiltEffectModel.CreateDefault();
            count = model.Items.Count;
        }

        if (mIcons.Count == count)
        {
            for (var i = 0; i < mIcons.Count; i++)
            {
                var item = model.Items[i];
                var view = mIcons[i].View;
                if (view != null)
                {
                    if (view.Back != null)
                    {
                        var backImage = view.Back.GetComponent<UnityEngine.UI.Image>();
                        if (backImage != null)
                        {
                            backImage.color = item.BackColor;
                        }
                    }

                    if (view.GlyphText != null)
                    {
                        view.GlyphText.text = item.Glyph;
                    }

                    if (view.LabelText != null)
                    {
                        view.LabelText.text = item.Label;
                    }
                }
            }

            return;
        }

        for (var i = 0; i < mIcons.Count; i++)
        {
            if (mIcons[i].View?.Root != null)
            {
                UnityEngine.Object.Destroy(mIcons[i].View.Root.gameObject);
            }
        }

        mIcons.Clear();
        mView.Icons.Clear();

        for (var i = 0; i < count; i++)
        {
            var iconView = GlassIconsTiltEffectViewBuilder.CreateIconCell(i, mView.Grid, mConfig, model.Items[i]);
            mView.Icons.Add(iconView);
            mIcons.Add(new IconRuntime { View = iconView, Hovered = false });
        }
    }

    private void ResetAllVisuals()
    {
        for (var i = 0; i < mIcons.Count; i++)
        {
            var view = mIcons[i].View;
            if (view == null)
            {
                continue;
            }

            if (view.Root != null)
            {
                view.Root.localScale = Vector3.one;
            }

            if (view.Back != null)
            {
                view.Back.anchoredPosition = Vector2.zero;
                view.Back.localRotation = Quaternion.Euler(0f, 0f, mConfig.BackIdleRotation);
            }

            if (view.Front != null)
            {
                view.Front.anchoredPosition = Vector2.zero;
                view.Front.localScale = Vector3.one;
            }

            if (view.LabelRoot != null)
            {
                view.LabelRoot.anchoredPosition = new Vector2(0f, mConfig.LabelIdleY);
            }

            if (view.LabelGroup != null)
            {
                view.LabelGroup.alpha = 0f;
            }

            mIcons[i].Hovered = false;
        }
    }

    private void UpdateHoverStates()
    {
        if (mContext.Pointer == null || !mContext.Pointer.IsPointerValid)
        {
            ApplyHoverMask(-1);
            return;
        }

        var hoveredIndex = -1;
        for (var i = 0; i < mIcons.Count; i++)
        {
            var icon = mIcons[i].View;
            if (icon?.Root == null)
            {
                continue;
            }

            if (mContext.Pointer.TryGetLocalPoint(icon.Root, out var local, mContext.UICamera))
            {
                var halfW = Mathf.Max(1f, icon.Root.rect.width * 0.5f);
                var halfH = Mathf.Max(1f, icon.Root.rect.height * 0.5f);
                if (Mathf.Abs(local.x) <= halfW && Mathf.Abs(local.y) <= halfH)
                {
                    hoveredIndex = i;
                    break;
                }
            }
        }

        ApplyHoverMask(hoveredIndex);
    }

    private void ApplyHoverMask(int hoveredIndex)
    {
        for (var i = 0; i < mIcons.Count; i++)
        {
            var shouldHover = i == hoveredIndex;
            if (mIcons[i].Hovered == shouldHover)
            {
                continue;
            }

            mIcons[i].Hovered = shouldHover;
            AnimateHover(mIcons[i].View, shouldHover);
        }
    }

    private void AnimateHover(GlassIconsTiltEffectIconView icon, bool hovering)
    {
        if (icon == null)
        {
            return;
        }

        if (icon.Root != null)
        {
            mContext.Tweens.Kill(icon.Root);
        }

        if (icon.Back != null)
        {
            mContext.Tweens.Kill(icon.Back);
        }

        if (icon.Front != null)
        {
            mContext.Tweens.Kill(icon.Front);
        }

        if (icon.LabelRoot != null)
        {
            mContext.Tweens.Kill(icon.LabelRoot);
        }

        if (icon.LabelGroup != null)
        {
            mContext.Tweens.Kill(icon.LabelGroup);
        }

        if (hovering)
        {
            if (icon.Root != null)
            {
                mContext.Tweens.ScaleTo(icon.Root, Vector3.one * mConfig.HoverScale, mConfig.HoverDuration, icon.Root)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
            }

            if (icon.Back != null)
            {
                mContext.Tweens.AnchoredPosTo(icon.Back, mConfig.BackHoverOffset, mConfig.HoverDuration, icon.Back)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
                mContext.Tweens.LocalRotateTo(icon.Back, new Vector3(0f, 0f, mConfig.BackHoverRotation), mConfig.HoverDuration, icon.Back)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
            }

            if (icon.Front != null)
            {
                mContext.Tweens.AnchoredPosTo(icon.Front, mConfig.FrontHoverOffset, mConfig.HoverDuration, icon.Front)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
                mContext.Tweens.ScaleTo(icon.Front, Vector3.one * mConfig.FrontHoverScale, mConfig.HoverDuration, icon.Front)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
            }

            if (icon.LabelRoot != null)
            {
                mContext.Tweens.AnchoredPosYTo(icon.LabelRoot, mConfig.LabelHoverY, mConfig.HoverDuration, icon.LabelRoot)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
            }

            if (icon.LabelGroup != null)
            {
                FadeCanvasGroup(icon.LabelGroup, 1f, Mathf.Max(0.08f, mConfig.HoverDuration - 0.04f), icon.LabelGroup, ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
            }
        }
        else
        {
            if (icon.Root != null)
            {
                mContext.Tweens.ScaleTo(icon.Root, Vector3.one, mConfig.IdleDuration, icon.Root)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
            }

            if (icon.Back != null)
            {
                mContext.Tweens.AnchoredPosTo(icon.Back, Vector2.zero, mConfig.IdleDuration, icon.Back)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
                mContext.Tweens.LocalRotateTo(icon.Back, new Vector3(0f, 0f, mConfig.BackIdleRotation), mConfig.IdleDuration, icon.Back)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
            }

            if (icon.Front != null)
            {
                mContext.Tweens.AnchoredPosTo(icon.Front, Vector2.zero, mConfig.IdleDuration, icon.Front)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
                mContext.Tweens.ScaleTo(icon.Front, Vector3.one, mConfig.IdleDuration, icon.Front)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
            }

            if (icon.LabelRoot != null)
            {
                mContext.Tweens.AnchoredPosYTo(icon.LabelRoot, mConfig.LabelIdleY, mConfig.IdleDuration, icon.LabelRoot)
                    .SetEase(ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
            }

            if (icon.LabelGroup != null)
            {
                FadeCanvasGroup(icon.LabelGroup, 0f, Mathf.Max(0.08f, mConfig.IdleDuration - 0.04f), icon.LabelGroup, ReplicaEase.OutCubic)
                    .SetUpdate(mContext.UseUnscaledTime);
            }
        }
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

    private void KillAllIconTweens()
    {
        for (var i = 0; i < mIcons.Count; i++)
        {
            var icon = mIcons[i].View;
            if (icon?.Root != null) mContext.Tweens.Kill(icon.Root);
            if (icon?.Back != null) mContext.Tweens.Kill(icon.Back);
            if (icon?.Front != null) mContext.Tweens.Kill(icon.Front);
            if (icon?.LabelRoot != null) mContext.Tweens.Kill(icon.LabelRoot);
            if (icon?.LabelGroup != null) mContext.Tweens.Kill(icon.LabelGroup);
        }
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
