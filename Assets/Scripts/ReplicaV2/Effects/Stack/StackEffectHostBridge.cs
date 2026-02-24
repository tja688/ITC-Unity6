using System.Collections.Generic;
using UnityEngine;

[ReplicaShowcase("Cards", "Stack")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class StackEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private StackEffectConfig mConfig;
    [SerializeField][Range(1, 12)] private int mInitialCount = 5;

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private StackEffectController mController;
    private StackEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("StackEffectHostBridge requires RectTransform.", this);
            enabled = false;
            return;
        }

        mPointerRelay = ReplicaUIFactoryV2.EnsureComponent<ReplicaPointerRelay>(gameObject);
    }

    private void OnEnable()
    {
        EnsureController();

        if (mModel == null || mModel.Items.Count == 0)
        {
            mModel = StackEffectModel.CreateDefault(mInitialCount);
        }

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

    public void SetItems(IReadOnlyList<StackItemData> items, bool animated = true)
    {
        EnsureController();

        var model = new StackEffectModel();
        if (items != null)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var item = items[i] ?? new StackItemData();
                model.Items.Add(item.Clone());
            }
        }

        mModel = model;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void AddItem(StackItemData item, int index = -1, bool animated = true)
    {
        EnsureModel();
        var clone = (item ?? new StackItemData()).Clone();

        if (string.IsNullOrWhiteSpace(clone.Id))
        {
            clone.Id = $"stack-card-{System.Guid.NewGuid():N}";
        }

        if (index < 0 || index > mModel.Items.Count)
        {
            mModel.Items.Add(clone);
        }
        else
        {
            mModel.Items.Insert(index, clone);
        }

        mController.SetModel(mModel.Clone(), animated);
    }

    public void RemoveAt(int index, bool animated = true)
    {
        EnsureModel();
        if (index < 0 || index >= mModel.Items.Count)
        {
            return;
        }

        mModel.Items.RemoveAt(index);
        mController.SetModel(mModel.Clone(), animated);
    }

    public void Reorder(int fromIndex, int toIndex, bool animated = true)
    {
        EnsureModel();
        if (fromIndex < 0 || fromIndex >= mModel.Items.Count)
        {
            return;
        }

        toIndex = Mathf.Clamp(toIndex, 0, mModel.Items.Count - 1);
        if (fromIndex == toIndex)
        {
            return;
        }

        var item = mModel.Items[fromIndex];
        mModel.Items.RemoveAt(fromIndex);
        mModel.Items.Insert(toIndex, item);
        mController.SetModel(mModel.Clone(), animated);
    }

    public void SetItemCount(int count)
    {
        SetItemCount(count, true);
    }

    public void SetItemCount(int count, bool animated = true)
    {
        mModel = StackEffectModel.CreateDefault(count);
        EnsureController();
        mController.SetModel(mModel.Clone(), animated);
    }

    public void PlayIn(ReplicaEnterDirection direction)
    {
        EnsureController();
        var transition = ReplicaTransition.Default;
        transition.EnterDirection = direction;
        transition.Ease = ReplicaEase.OutCubic;
        mController.PlayIn(transition);
    }

    public void PlayOut(ReplicaExitDirection direction)
    {
        EnsureController();
        var transition = ReplicaTransition.Default;
        transition.ExitDirection = direction;
        transition.Ease = ReplicaEase.InCubic;
        mController.PlayOut(transition);
    }

    public StackEffectModel GetModelClone()
    {
        EnsureModel();
        return mModel.Clone();
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

        mController = new StackEffectController();
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
        if (mModel == null)
        {
            mModel = StackEffectModel.CreateDefault(mInitialCount);
        }
    }
}

