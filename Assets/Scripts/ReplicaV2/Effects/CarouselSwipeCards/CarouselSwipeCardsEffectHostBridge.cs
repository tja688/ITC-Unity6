using UnityEngine;

[ReplicaShowcase("UI", "Carousel")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class CarouselSwipeCardsEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;

    [Header("Config")]
    [SerializeField] private CarouselSwipeCardsEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private CarouselSwipeCardsEffectModel mDefaultModel;

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private CarouselSwipeCardsEffectController mController;
    private CarouselSwipeCardsEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("CarouselSwipeCardsEffectHostBridge requires RectTransform.", this);
            enabled = false;
            return;
        }

        mPointerRelay = ReplicaUIFactoryV2.EnsureComponent<ReplicaPointerRelay>(gameObject);
    }

    private void OnEnable()
    {
        EnsureController();

        if (mModel == null)
        {
            mModel = mDefaultModel != null ? mDefaultModel.Clone() : CarouselSwipeCardsEffectModel.CreateDefault();
        }

        mController.SetModel(mModel.Clone(), false);
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

    public void SetModel(CarouselSwipeCardsEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : CarouselSwipeCardsEffectModel.CreateDefault();
        mController.SetModel(mModel.Clone(), animated);
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

        mController = new CarouselSwipeCardsEffectController();
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

    public void PlayIn(ReplicaEnterDirection direction)
    {
        // default empty
    }

    public void PlayOut(ReplicaExitDirection direction)
    {
        // default empty
    }
}
