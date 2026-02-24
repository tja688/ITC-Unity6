using System;
using System.Collections.Generic;
using UnityEngine;

[ReplicaShowcase("Container", "Scroll Stack")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class ScrollStackEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private ScrollStackEffectConfig mConfig;

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private ScrollStackEffectController mController;
    private ScrollStackEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("ScrollStackEffectHostBridge requires RectTransform.", this);
            enabled = false;
            return;
        }

        mPointerRelay = ReplicaUIFactoryV2.EnsureComponent<ReplicaPointerRelay>(gameObject);
    }

    private void OnEnable()
    {
        EnsureController();
        EnsureModel();
        mController.SetModel(mModel, false);

        if (mPlayInOnEnable)
        {
            PlayIn(ReplicaEnterDirection.FromBottom);
        }
    }

    private void Update()
    {
        if (mController == null) return;
        mController.Tick(Time.deltaTime, Time.unscaledDeltaTime);
    }

    private void OnDestroy()
    {
        DisposeController();
    }

    public void SetBackend(ReplicaTweenBackend backend, bool rebuild = true)
    {
        if (mBackend == backend && mController != null) return;
        mBackend = backend;
        if (rebuild) Rebuild();
    }

    public void PlayIn(ReplicaEnterDirection direction)
    {
        EnsureController();
        var transition = ReplicaTransition.Default;
        transition.EnterDirection = direction;
        mController.PlayIn(transition);
    }

    public void PlayOut(ReplicaExitDirection direction)
    {
        EnsureController();
        var transition = ReplicaTransition.Default;
        transition.ExitDirection = (ReplicaExitDirection)direction;
        mController.PlayOut(transition);
    }

    public void DriveToLayer(int layerIndex, bool animated = true)
    {
        EnsureController();
        mController.DriveToLayer(layerIndex, animated);
    }

    public void DriveToProgress(float progress01, bool animated = true)
    {
        EnsureController();
        mController.DriveToProgress(progress01, animated);
    }

    [ContextMenu("Drive To Top")]
    private void DriveToTop()
    {
        DriveToProgress(0f, true);
    }

    [ContextMenu("Drive To Bottom")]
    private void DriveToBottom()
    {
        DriveToProgress(1f, true);
    }

    private void Rebuild()
    {
        DisposeController();
        EnsureController();
        if (mModel != null) mController.SetModel(mModel, false);
    }

    private void EnsureController()
    {
        if (mController != null) return;

        var context = new ReplicaHostContext(
            mRootRect,
            null,
            mPointerRelay,
            ReplicaTweenAdapterFactory.Create(mBackend),
            mUseUnscaledTime);

        mController = new ScrollStackEffectController();
        mController.Initialize(context, mConfig);
    }

    private void DisposeController()
    {
        if (mController == null) return;
        mController.Dispose();
        mController = null;
    }

    private void EnsureModel()
    {
        if (mModel != null) return;

        mModel = new ScrollStackEffectModel();
        for (int i = 0; i < 10; i++)
        {
            mModel.items.Add(new ScrollStackItemModel
            {
                title = $"Scroll Card {i + 1}",
                description = "This is a stackable card that pins to the top as you scroll. Keep scrolling to see more!",
                backgroundColor = GetRandomColor(i)
            });
        }
    }

    private Color GetRandomColor(int index)
    {
        float hue = (index * 0.13f) % 1.0f;
        return Color.HSVToRGB(hue, 0.6f, 0.9f);
    }
}
