using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class DockMagnifyToolbarEffectController : IReplicaEffect<DockMagnifyToolbarEffectConfig, DockMagnifyToolbarEffectModel>
{
    private ReplicaHostContext mContext;
    private DockMagnifyToolbarEffectConfig mConfig;
    private DockMagnifyToolbarEffectView mView;
    private DockMagnifyToolbarEffectModel mModel;

    private bool mInitialized;
    private bool mPanelHovered;
    private int mHoverIndex = -1;
    private float mCurrentPanelHeight;

    public string EffectId => "dock-magnify-toolbar-v2";

    public void Initialize(ReplicaHostContext context, DockMagnifyToolbarEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<DockMagnifyToolbarEffectConfig>();
        mModel = DockMagnifyToolbarEffectModel.CreateDefault();
        mView = DockMagnifyToolbarEffectViewBuilder.Build(mContext.MountRoot, mConfig, this, mModel);
        ResetState();
        mInitialized = true;
    }

    public void SetModel(DockMagnifyToolbarEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        mModel = model != null ? model.Clone() : DockMagnifyToolbarEffectModel.CreateDefault();
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
        if (!mInitialized || mView == null || mView.Items.Count == 0)
        {
            return;
        }

        var dt = mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;
        UpdatePanelHeight(dt);
        UpdateMagnification(dt);
        UpdateLabels(dt);
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

    public void SetPanelHovered(bool value)
    {
        mPanelHovered = value;
        if (!value)
        {
            mHoverIndex = -1;
        }
    }

    public void SetItemHovered(int index, bool hovered)
    {
        if (!hovered && mHoverIndex == index)
        {
            mHoverIndex = -1;
            return;
        }

        if (hovered)
        {
            mHoverIndex = index;
        }
    }

    public void OnItemClicked(int index)
    {
        if (!mInitialized || mView == null || index < 0 || index >= mView.Items.Count)
        {
            return;
        }

        mHoverIndex = index;
        var item = mView.Items[index];

        mContext.Tweens.Kill(item.Root);
        mContext.Tweens
            .PunchScale(
                item.Root,
                mConfig.ClickPunchScale,
                Mathf.Max(0.01f, mConfig.ClickPunchDuration),
                Mathf.Max(1, mConfig.ClickPunchVibrato),
                Mathf.Clamp01(mConfig.ClickPunchElasticity),
                item.Root)
            .SetEase(ReplicaEase.OutBack, 1.05f)
            .SetUpdate(mContext.UseUnscaledTime);
    }

    private void RebuildView()
    {
        DestroyView();
        mView = DockMagnifyToolbarEffectViewBuilder.Build(mContext.MountRoot, mConfig, this, mModel);
        ResetState();
    }

    private void DestroyView()
    {
        if (mView == null)
        {
            return;
        }

        for (var i = 0; i < mView.Items.Count; i++)
        {
            var item = mView.Items[i];
            if (item?.Root != null)
            {
                mContext.Tweens.Kill(item.Root);
                UnityEngine.Object.Destroy(item.Root.gameObject);
            }
        }

        if (mView.DockPanel != null)
        {
            mContext.Tweens.Kill(mView.DockPanel);
        }

        if (mView.Root != null)
        {
            mContext.Tweens.Kill(mView.Root);
            UnityEngine.Object.Destroy(mView.Root.gameObject);
        }

        mView = null;
    }

    private void ResetState()
    {
        mPanelHovered = false;
        mHoverIndex = -1;
        mCurrentPanelHeight = mConfig.PanelHeightClosed;

        for (var i = 0; i < mView.Items.Count; i++)
        {
            var item = mView.Items[i];
            item.CurrentSize = mConfig.BaseItemSize;
            item.Root.sizeDelta = new Vector2(mConfig.BaseItemSize, mConfig.BaseItemSize);
            item.LabelGroup.alpha = 0f;
            item.Root.anchoredPosition = item.BasePos;
        }

        if (mView.DockPanel != null)
        {
            mView.DockPanel.sizeDelta = new Vector2(mView.DockPanel.sizeDelta.x, mConfig.PanelHeightClosed);
        }
    }

    private void UpdatePanelHeight(float dt)
    {
        if (mView.DockPanel == null)
        {
            return;
        }

        var targetHeight = mPanelHovered ? mConfig.PanelHeightOpen : mConfig.PanelHeightClosed;
        var speed = Mathf.Max(0.01f, mConfig.PanelHeightLerpSpeed);
        mCurrentPanelHeight = Mathf.Lerp(mCurrentPanelHeight, targetHeight, dt * speed);
        mView.DockPanel.sizeDelta = new Vector2(mView.DockPanel.sizeDelta.x, mCurrentPanelHeight);
    }

    private void UpdateMagnification(float dt)
    {
        var localPointer = new Vector2(float.PositiveInfinity, 0f);
        if (mPanelHovered && mContext.Pointer != null && mContext.Pointer.IsPointerValid)
        {
            _ = mContext.Pointer.TryGetLocalPoint(mView.DockPanel, out localPointer, mContext.UICamera);
        }

        var influenceDistance = Mathf.Max(1f, mConfig.InfluenceDistance);
        var spring = Mathf.Max(0.01f, mConfig.SpringSpeed);
        var baseSize = Mathf.Max(1f, mConfig.BaseItemSize);
        var maxSize = Mathf.Max(baseSize, mConfig.MagnifiedItemSize);

        for (var i = 0; i < mView.Items.Count; i++)
        {
            var item = mView.Items[i];
            var distance = Mathf.Abs(localPointer.x - item.BasePos.x);
            var influence = mPanelHovered ? Mathf.Clamp01(1f - (distance / influenceDistance)) : 0f;
            var eased = influence * influence * (3f - (2f * influence));
            var targetSize = Mathf.Lerp(baseSize, maxSize, eased);

            item.CurrentSize = Mathf.Lerp(item.CurrentSize, targetSize, dt * spring);
            item.Root.sizeDelta = new Vector2(item.CurrentSize, item.CurrentSize);
            item.Root.anchoredPosition = new Vector2(item.BasePos.x, item.BasePos.y + ((item.CurrentSize - baseSize) * 0.22f));

            var font = Mathf.Lerp(mConfig.FontSizeMin, mConfig.FontSizeMax, eased);
            item.Icon.fontSize = Mathf.RoundToInt(font);

            var hoverBlend = Mathf.Max(eased, (mHoverIndex == i) ? 1f : 0f);
            var baseColor = item.BaseColor;
            var dimmed = baseColor * 0.85f;
            dimmed.a = baseColor.a;
            item.Background.color = Color.Lerp(dimmed, baseColor, hoverBlend);

            var labelTargetY = item.CurrentSize + Mathf.Lerp(mConfig.LabelOffsetMin, mConfig.LabelOffsetMax, item.LabelGroup.alpha);
            item.LabelRoot.anchoredPosition = new Vector2(0f, labelTargetY);
        }
    }

    private void UpdateLabels(float dt)
    {
        var speed = Mathf.Max(0.01f, mConfig.LabelFadeSpeed);
        for (var i = 0; i < mView.Items.Count; i++)
        {
            var item = mView.Items[i];
            var target = (mPanelHovered && mHoverIndex == i) ? 1f : 0f;
            item.LabelGroup.alpha = Mathf.MoveTowards(item.LabelGroup.alpha, target, dt * speed);
        }
    }
}
