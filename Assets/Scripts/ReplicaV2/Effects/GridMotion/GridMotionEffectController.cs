using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class GridMotionEffectController : IReplicaEffect<GridMotionEffectConfig, GridMotionEffectModel>
{
    private readonly List<IReplicaTweenHandle> mTransitionTweens = new List<IReplicaTweenHandle>();

    private ReplicaHostContext mContext;
    private GridMotionEffectConfig mConfig;
    private GridMotionEffectView mView;

    private GridMotionEffectModel mModel;

    private bool mInitialized;
    private float[] mCurrentRowX;
    private float mLastLayoutW;
    private float mLastLayoutH;

    public string EffectId => "grid-motion-v2";

    public void Initialize(ReplicaHostContext context, GridMotionEffectConfig config)
    {
        if (mInitialized)
        {
            return;
        }

        mContext = context ?? throw new ArgumentNullException(nameof(context));
        mConfig = config != null ? config : ScriptableObject.CreateInstance<GridMotionEffectConfig>();
        mView = GridMotionEffectViewBuilder.Build(mContext.MountRoot, mConfig);
        EnsureRowBuffers();
        ResetState();
        mInitialized = true;
    }

    public void SetModel(GridMotionEffectModel model, bool animated = true)
    {
        if (!mInitialized)
        {
            return;
        }

        var tileCount = mView != null ? mView.Tiles.Count : 0;
        var safe = model != null ? model.Clone() : GridMotionEffectModel.CreateDefault(tileCount);
        if (safe.Items == null)
        {
            safe.Items = new List<GridMotionEffectItemModel>();
        }

        if (safe.Items.Count < tileCount)
        {
            for (var i = safe.Items.Count; i < tileCount; i++)
            {
                safe.Items.Add(new GridMotionEffectItemModel { Text = $"Item {i + 1}" });
            }
        }
        else if (safe.Items.Count > tileCount)
        {
            safe.Items.RemoveRange(tileCount, safe.Items.Count - tileCount);
        }

        mModel = safe;
        ApplyModelToView();

        if (!animated && mView?.Group != null)
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
        var duration = Mathf.Max(0.08f, transition.Duration > 0f ? transition.Duration : ReplicaTransition.Default.Duration);

        if (mView?.Group != null)
        {
            mView.Group.alpha = 0f;
            mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 1f, duration, mView.Root, transition.Ease));
        }
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

        if (mView?.Group != null)
        {
            mTransitionTweens.Add(FadeCanvasGroup(mView.Group, 0f, duration, mView.Root, transition.Ease).OnComplete(onComplete));
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    public void Tick(float deltaTime, float unscaledDeltaTime)
    {
        if (!mInitialized)
        {
            return;
        }

        var dt = mContext.UseUnscaledTime ? unscaledDeltaTime : deltaTime;
        EnsureLayout();
        UpdateMotion(dt);
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

    private void ResetState()
    {
        EnsureRowBuffers();
        if (mCurrentRowX != null)
        {
            for (var i = 0; i < mCurrentRowX.Length; i++)
            {
                mCurrentRowX[i] = 0f;
            }
        }

        mLastLayoutW = -1f;
        mLastLayoutH = -1f;
    }

    private void EnsureRowBuffers()
    {
        var rows = mView != null ? mView.RowContents.Count : 0;
        if (rows <= 0)
        {
            mCurrentRowX = Array.Empty<float>();
            return;
        }

        if (mCurrentRowX == null || mCurrentRowX.Length != rows)
        {
            mCurrentRowX = new float[rows];
        }
    }

    private void EnsureLayout()
    {
        if (mView?.RowsRoot == null || mView.RowGridLayouts == null)
        {
            return;
        }

        var w = Mathf.Max(1f, mView.RowsRoot.rect.width);
        var h = Mathf.Max(1f, mView.RowsRoot.rect.height);

        if (Mathf.Abs(w - mLastLayoutW) < 0.5f && Mathf.Abs(h - mLastLayoutH) < 0.5f)
        {
            return;
        }

        mLastLayoutW = w;
        mLastLayoutH = h;

        var rows = Mathf.Max(1, mConfig.RowCount);
        var cols = Mathf.Max(1, mConfig.ColumnCount);
        var gap = Mathf.Max(0f, mConfig.Gap);

        var cellW = Mathf.Max(1f, (w - (gap * (cols - 1))) / cols);
        var cellH = Mathf.Max(1f, (h - (gap * (rows - 1))) / rows);

        for (var i = 0; i < mView.RowGridLayouts.Count; i++)
        {
            var grid = mView.RowGridLayouts[i];
            if (grid == null)
            {
                continue;
            }

            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = cols;
            grid.spacing = new Vector2(gap, gap);
            grid.cellSize = new Vector2(cellW, cellH);
        }
    }

    private void UpdateMotion(float dt)
    {
        if (mView == null || mView.RowContents == null || mView.RowContents.Count == 0)
        {
            return;
        }

        EnsureRowBuffers();

        var t = 0.5f;
        if (mContext.Pointer != null && mContext.Pointer.IsPointerValid && mView.Root != null)
        {
            if (mContext.Pointer.TryGetLocalPoint(mView.Root, out var localPoint, mContext.UICamera))
            {
                var width = Mathf.Max(1f, mView.Root.rect.width);
                t = Mathf.Clamp01((localPoint.x / width) + 0.5f);
            }
        }

        var maxMove = Mathf.Max(0f, mConfig.MaxMoveAmount);
        var baseDuration = Mathf.Max(0.01f, mConfig.BaseFollowDuration);
        var inertia = mConfig.InertiaFactors != null && mConfig.InertiaFactors.Length > 0 ? mConfig.InertiaFactors : new[] { 0.6f, 0.4f, 0.3f, 0.2f };

        for (var i = 0; i < mView.RowContents.Count; i++)
        {
            var row = mView.RowContents[i];
            if (row == null)
            {
                continue;
            }

            var direction = i % 2 == 0 ? 1f : -1f;
            var targetX = ((t * maxMove) - (maxMove * 0.5f)) * direction;

            var duration = baseDuration + Mathf.Max(0f, inertia[i % inertia.Length]);
            var tau = Mathf.Max(0.01f, duration / 3f);
            var follow = 1f - Mathf.Exp(-dt / tau);

            var current = mCurrentRowX[i];
            current = Mathf.Lerp(current, targetX, Mathf.Clamp01(follow));
            mCurrentRowX[i] = current;

            var pos = row.anchoredPosition;
            pos.x = current;
            pos.y = 0f;
            row.anchoredPosition = pos;
        }
    }

    private void ApplyModelToView()
    {
        if (mView == null || mModel == null || mView.Tiles == null)
        {
            return;
        }

        if (mView.BackdropImage != null)
        {
            mView.BackdropImage.color = mConfig.GradientColor;
        }

        for (var i = 0; i < mView.Tiles.Count; i++)
        {
            var tile = mView.Tiles[i];
            if (tile == null)
            {
                continue;
            }

            if (tile.TileImage != null)
            {
                tile.TileImage.color = mConfig.TileColor;
            }

            var item = (mModel.Items != null && i < mModel.Items.Count) ? mModel.Items[i] : null;
            var sprite = item != null ? item.Sprite : null;

            if (tile.SpriteImage != null)
            {
                tile.SpriteImage.sprite = sprite;
                tile.SpriteImage.gameObject.SetActive(sprite != null);
            }

            if (tile.Text != null)
            {
                tile.Text.fontSize = mConfig.FontSize;
                tile.Text.color = mConfig.TextColor;

                var text = item != null ? item.Text : null;
                if (string.IsNullOrWhiteSpace(text))
                {
                    text = $"Item {i + 1}";
                }

                tile.Text.text = text;
                tile.Text.gameObject.SetActive(sprite == null);
            }
        }
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
}
