using System.Collections.Generic;
using UnityEngine;

[ReplicaShowcase("Backgrounds", "GridMotion")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class GridMotionEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private GridMotionEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private List<GridMotionEffectItemModel> mItems = new List<GridMotionEffectItemModel>();

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private GridMotionEffectController mController;
    private GridMotionEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("GridMotionEffectHostBridge requires RectTransform.", this);
            enabled = false;
            return;
        }

        mPointerRelay = ReplicaUIFactoryV2.EnsureComponent<ReplicaPointerRelay>(gameObject);
    }

    private void OnEnable()
    {
        EnsureController();
        EnsureModel();
        mController.SetModel(mModel.Clone(), false);

        if (mPlayInOnEnable)
        {
            PlayIn(ReplicaEnterDirection.FromBottom);
        }
    }

    private void Update()
    {
        if (mController == null)
        {
            return;
        }

        mController.Tick(Time.deltaTime, Time.unscaledDeltaTime);
    }

    private void OnDestroy()
    {
        DisposeController();
    }

    public void SetBackend(ReplicaTweenBackend backend, bool rebuild = true)
    {
        if (mBackend == backend && mController != null)
        {
            return;
        }

        mBackend = backend;
        if (rebuild)
        {
            Rebuild();
        }
    }

    public void SetModel(GridMotionEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : CreateDefaultModelFromInspector();
        mController.SetModel(mModel.Clone(), animated);
    }

    public void PlayIn(ReplicaEnterDirection direction = ReplicaEnterDirection.FromBottom)
    {
        EnsureController();
        var transition = ReplicaTransition.Default;
        transition.EnterDirection = direction;
        transition.Ease = ReplicaEase.OutCubic;
        mController.PlayIn(transition);
    }

    public void PlayOut(ReplicaExitDirection direction = ReplicaExitDirection.ToBottom)
    {
        EnsureController();
        var transition = ReplicaTransition.Default;
        transition.ExitDirection = direction;
        transition.Ease = ReplicaEase.InCubic;
        mController.PlayOut(transition);
    }

    private void Rebuild()
    {
        var snapshot = mModel != null ? mModel.Clone() : null;
        DisposeController();
        EnsureController();

        if (snapshot != null)
        {
            mModel = snapshot;
            mController.SetModel(snapshot.Clone(), false);
        }
    }

    private void EnsureController()
    {
        if (mController != null)
        {
            return;
        }

        var context = new ReplicaHostContext(
            mRootRect,
            null,
            mPointerRelay,
            ReplicaTweenAdapterFactory.Create(mBackend),
            mUseUnscaledTime);

        mController = new GridMotionEffectController();
        mController.Initialize(context, mConfig);
    }

    private void DisposeController()
    {
        if (mController == null)
        {
            return;
        }

        mController.Dispose();
        mController = null;
    }

    private void EnsureModel()
    {
        if (mModel != null)
        {
            return;
        }

        mModel = CreateDefaultModelFromInspector();
    }

    private GridMotionEffectModel CreateDefaultModelFromInspector()
    {
        var rows = mConfig != null ? Mathf.Max(1, mConfig.RowCount) : 4;
        var cols = mConfig != null ? Mathf.Max(1, mConfig.ColumnCount) : 7;
        var count = rows * cols;

        var model = GridMotionEffectModel.CreateDefault(count);
        if (mItems != null && mItems.Count > 0)
        {
            for (var i = 0; i < count; i++)
            {
                if (i < mItems.Count && mItems[i] != null)
                {
                    model.Items[i] = mItems[i].Clone();
                }
            }
        }

        return model;
    }
}
