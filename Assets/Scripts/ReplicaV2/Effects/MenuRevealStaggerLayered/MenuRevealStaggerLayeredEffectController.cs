using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class MenuRevealStaggerLayeredEffectController : IReplicaEffect<MenuRevealStaggerLayeredEffectConfig, MenuRevealStaggerLayeredEffectModel>
{
    private readonly List<IReplicaTweenHandle> mOpenTweens = new List<IReplicaTweenHandle>();
    private readonly List<IReplicaTweenHandle> mCloseTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private MenuRevealStaggerLayeredEffectConfig mConfig;
    private MenuRevealStaggerLayeredEffectView mView;
    private MenuRevealStaggerLayeredEffectModel mModel;

    private bool mInitialized;
    private bool mOpen;

    private IReplicaTweenHandle mCloseCompleteDelay;
    private IReplicaTweenHandle mIconTween;
    private IReplicaTweenHandle mTextTween;
    private IReplicaTweenHandle mColorTween;

    public string EffectId => "menu-reveal-stagger-layered-v2";

    public void Initialize(ReplicaHostContext context, MenuRevealStaggerLayeredEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<MenuRevealStaggerLayeredEffectConfig>();
        mModel = MenuRevealStaggerLayeredEffectModel.CreateDefault();
        mView = MenuRevealStaggerLayeredEffectViewBuilder.Build(mContext.MountRoot, mConfig, this, mModel);
        ApplyClosedPose(true);
        mInitialized = true;
    }

    public void SetModel(MenuRevealStaggerLayeredEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : MenuRevealStaggerLayeredEffectModel.CreateDefault();
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

        KillTweens();
        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mView = null;
        mInitialized = false;
    }

    public void ToggleMenu()
    {
        if (mOpen)
        {
            CloseMenu();
        }
        else
        {
            OpenMenu();
        }
    }

    public void CloseMenuIfOpen()
    {
        if (mOpen)
        {
            CloseMenu();
        }
    }

    private void OpenMenu()
    {
        mOpen = true;
        KillTweens();
        mView.DismissGroup.blocksRaycasts = true;
        mView.DismissGroup.interactable = true;

        SetItemHiddenPose();

        for (var i = 0; i < mView.PreLayers.Count; i++)
        {
            mOpenTweens.Add(
                mContext.Tweens
                    .AnchoredPosXTo(mView.PreLayers[i], 0f, mConfig.LayerSlideDuration, mView.PreLayers[i])
                    .SetEase(ReplicaEase.OutQuart)
                    .SetDelay(i * mConfig.LayerStagger)
                    .SetUpdate(mContext.UseUnscaledTime));
        }

        var panelDelay = Mathf.Max(0.05f, mView.PreLayers.Count * mConfig.LayerStagger);
        mOpenTweens.Add(
            mContext.Tweens
                .AnchoredPosXTo(mView.Panel, 0f, mConfig.PanelSlideDuration, mView.Panel)
                .SetEase(ReplicaEase.OutQuart)
                .SetDelay(panelDelay)
                .SetUpdate(mContext.UseUnscaledTime));

        var itemsStart = panelDelay + 0.06f;
        for (var i = 0; i < mView.ItemLabels.Count; i++)
        {
            var delay = itemsStart + (i * mConfig.ItemStagger);
            mOpenTweens.Add(
                mContext.Tweens
                    .AnchoredPosYTo(mView.ItemLabels[i], 0f, mConfig.ItemSlideDuration, mView.ItemLabels[i])
                    .SetEase(ReplicaEase.OutQuart)
                    .SetDelay(delay)
                    .SetUpdate(mContext.UseUnscaledTime));
            mOpenTweens.Add(
                mContext.Tweens
                    .LocalRotateTo(mView.ItemLabels[i], Vector3.zero, mConfig.ItemSlideDuration, mView.ItemLabels[i])
                    .SetEase(ReplicaEase.OutQuart)
                    .SetDelay(delay)
                    .SetUpdate(mContext.UseUnscaledTime));
            mOpenTweens.Add(
                mContext.Tweens
                    .FadeTextTo(mView.ItemNumbers[i], 1f, 0.3f, mView.ItemNumbers[i])
                    .SetEase(ReplicaEase.OutQuad)
                    .SetDelay(delay + 0.06f)
                    .SetUpdate(mContext.UseUnscaledTime));
        }

        var socialsStart = panelDelay + 0.15f;
        mOpenTweens.Add(
            mContext.Tweens
                .FadeTextTo(mView.SocialTitle, 1f, mConfig.SocialFadeDuration, mView.SocialTitle)
                .SetEase(ReplicaEase.OutQuad)
                .SetDelay(socialsStart)
                .SetUpdate(mContext.UseUnscaledTime));

        for (var i = 0; i < mView.SocialLinks.Count; i++)
        {
            var delay = socialsStart + 0.03f + (i * 0.05f);
            var targetY = -52f - (i * 34f);
            var text = mView.SocialLinks[i].GetComponent<Text>();

            mOpenTweens.Add(
                mContext.Tweens
                    .AnchoredPosYTo(mView.SocialLinks[i], targetY, mConfig.SocialLinkDuration, mView.SocialLinks[i])
                    .SetEase(ReplicaEase.OutCubic)
                    .SetDelay(delay)
                    .SetUpdate(mContext.UseUnscaledTime));
            mOpenTweens.Add(
                mContext.Tweens
                    .FadeTextTo(text, 1f, mConfig.SocialLinkDuration, text)
                    .SetDelay(delay)
                    .SetUpdate(mContext.UseUnscaledTime));
        }

        mIconTween = mContext.Tweens
            .LocalRotateTo(mView.ToggleIcon, new Vector3(0f, 0f, 225f), mConfig.IconRotateDurationOpen, mView.ToggleIcon)
            .SetEase(ReplicaEase.OutQuart)
            .SetUpdate(mContext.UseUnscaledTime);

        mTextTween = mContext.Tweens
            .AnchoredPosYTo(mView.ToggleTextStack, 30f, mConfig.ToggleTextMoveDuration, mView.ToggleTextStack)
            .SetEase(ReplicaEase.OutQuart)
            .SetUpdate(mContext.UseUnscaledTime);

        mColorTween = ColorTextTo(mView.ToggleTextTop, mView.ToggleTextTop.color, mConfig.ToggleOpenColor, 0.2f, mView.ToggleTextTop, ReplicaEase.OutQuad)
            .SetDelay(0.1f);
    }

    private void CloseMenu()
    {
        mOpen = false;
        KillTweens();

        var offscreen = GetOffscreenDistance();

        mCloseTweens.Add(
            mContext.Tweens
                .AnchoredPosXTo(mView.Panel, offscreen, mConfig.CloseSlideDuration, mView.Panel)
                .SetEase(ReplicaEase.InCubic)
                .SetUpdate(mContext.UseUnscaledTime));
        for (var i = 0; i < mView.PreLayers.Count; i++)
        {
            mCloseTweens.Add(
                mContext.Tweens
                    .AnchoredPosXTo(mView.PreLayers[i], offscreen, mConfig.CloseSlideDuration, mView.PreLayers[i])
                    .SetEase(ReplicaEase.InCubic)
                    .SetUpdate(mContext.UseUnscaledTime));
        }

        mCloseCompleteDelay = mContext.Tweens.DelayedCall(mConfig.CloseSlideDuration, () =>
        {
            SetItemHiddenPose();
            var a = mConfig.AccentColor;
            mView.SocialTitle.color = new Color(a.r, a.g, a.b, 0f);
            mView.DismissGroup.blocksRaycasts = false;
            mView.DismissGroup.interactable = false;
        }, mView.Root).SetUpdate(mContext.UseUnscaledTime);

        mIconTween = mContext.Tweens
            .LocalRotateTo(mView.ToggleIcon, Vector3.zero, mConfig.IconRotateDurationClose, mView.ToggleIcon)
            .SetEase(ReplicaEase.InOutCubic)
            .SetUpdate(mContext.UseUnscaledTime);

        mTextTween = mContext.Tweens
            .AnchoredPosYTo(mView.ToggleTextStack, 0f, mConfig.ToggleTextMoveCloseDuration, mView.ToggleTextStack)
            .SetEase(ReplicaEase.InOutCubic)
            .SetUpdate(mContext.UseUnscaledTime);

        mColorTween = ColorTextTo(mView.ToggleTextTop, mView.ToggleTextTop.color, mConfig.ToggleClosedColor, 0.2f, mView.ToggleTextTop, ReplicaEase.OutQuad)
            .SetUpdate(mContext.UseUnscaledTime);
    }

    private void SetItemHiddenPose()
    {
        var a = mConfig.AccentColor;
        for (var i = 0; i < mView.ItemLabels.Count; i++)
        {
            mView.ItemLabels[i].anchoredPosition = new Vector2(0f, mConfig.ItemHiddenY);
            mView.ItemLabels[i].localRotation = Quaternion.Euler(0f, 0f, mConfig.ItemHiddenRotZ);
            mView.ItemNumbers[i].color = new Color(a.r, a.g, a.b, 0f);
        }

        for (var i = 0; i < mView.SocialLinks.Count; i++)
        {
            var text = mView.SocialLinks[i].GetComponent<Text>();
            var openY = -52f - (i * 34f);
            mView.SocialLinks[i].anchoredPosition = new Vector2(0f, openY - mConfig.SocialHiddenYOffset);
            var c = mConfig.SocialLinkColor;
            text.color = new Color(c.r, c.g, c.b, 0f);
        }

        mView.SocialTitle.color = new Color(a.r, a.g, a.b, 0f);
    }

    private void ApplyClosedPose(bool instant)
    {
        if (instant)
        {
            KillTweens();
        }

        var offscreen = GetOffscreenDistance();
        for (var i = 0; i < mView.PreLayers.Count; i++)
        {
            mView.PreLayers[i].anchoredPosition = new Vector2(offscreen, 0f);
        }

        mView.Panel.anchoredPosition = new Vector2(offscreen, 0f);
        mView.ToggleIcon.localRotation = Quaternion.identity;
        mView.ToggleTextStack.anchoredPosition = Vector2.zero;
        mView.ToggleTextTop.color = mConfig.ToggleClosedColor;
        mView.DismissGroup.blocksRaycasts = false;
        mView.DismissGroup.interactable = false;
        SetItemHiddenPose();
    }

    private void ApplyOpenPose(bool instant)
    {
        if (instant)
        {
            KillTweens();
        }

        for (var i = 0; i < mView.PreLayers.Count; i++)
        {
            mView.PreLayers[i].anchoredPosition = Vector2.zero;
        }

        mView.Panel.anchoredPosition = Vector2.zero;
        mView.ToggleIcon.localRotation = Quaternion.Euler(0f, 0f, 225f);
        mView.ToggleTextStack.anchoredPosition = new Vector2(0f, 30f);
        mView.ToggleTextTop.color = mConfig.ToggleOpenColor;
        mView.DismissGroup.blocksRaycasts = true;
        mView.DismissGroup.interactable = true;

        var a = mConfig.AccentColor;
        for (var i = 0; i < mView.ItemLabels.Count; i++)
        {
            mView.ItemLabels[i].anchoredPosition = Vector2.zero;
            mView.ItemLabels[i].localRotation = Quaternion.identity;
            mView.ItemNumbers[i].color = new Color(a.r, a.g, a.b, 1f);
        }

        for (var i = 0; i < mView.SocialLinks.Count; i++)
        {
            var text = mView.SocialLinks[i].GetComponent<Text>();
            var openY = -52f - (i * 34f);
            mView.SocialLinks[i].anchoredPosition = new Vector2(0f, openY);
            var c = mConfig.SocialLinkColor;
            text.color = new Color(c.r, c.g, c.b, 1f);
        }

        mView.SocialTitle.color = a;
    }

    private float GetOffscreenDistance()
    {
        return Mathf.Max(480f, mConfig.PanelWidth + 80f);
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

    private void KillTweens()
    {
        for (var i = 0; i < mOpenTweens.Count; i++)
        {
            mOpenTweens[i]?.Kill();
        }

        for (var i = 0; i < mCloseTweens.Count; i++)
        {
            mCloseTweens[i]?.Kill();
        }

        mOpenTweens.Clear();
        mCloseTweens.Clear();

        mCloseCompleteDelay?.Kill();
        mIconTween?.Kill();
        mTextTween?.Kill();
        mColorTween?.Kill();

        mCloseCompleteDelay = null;
        mIconTween = null;
        mTextTween = null;
        mColorTween = null;
    }

    private void RebuildView()
    {
        var wasOpen = mOpen;
        KillTweens();
        if (mView?.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mView = MenuRevealStaggerLayeredEffectViewBuilder.Build(mContext.MountRoot, mConfig, this, mModel);
        if (wasOpen)
        {
            ApplyOpenPose(true);
        }
        else
        {
            ApplyClosedPose(true);
        }
    }
}
