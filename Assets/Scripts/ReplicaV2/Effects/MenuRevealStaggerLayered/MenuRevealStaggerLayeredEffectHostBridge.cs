using UnityEngine;

[ReplicaShowcase("UI", "MenuReveal")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class MenuRevealStaggerLayeredEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;

    [Header("Config")]
    [SerializeField] private MenuRevealStaggerLayeredEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private MenuRevealStaggerLayeredEffectModel mDefaultModel;

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private MenuRevealStaggerLayeredEffectController mController;
    private MenuRevealStaggerLayeredEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("MenuRevealStaggerLayeredEffectHostBridge requires RectTransform.", this);
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
            mModel = mDefaultModel != null ? mDefaultModel.Clone() : MenuRevealStaggerLayeredEffectModel.CreateDefault();
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

    public void SetModel(MenuRevealStaggerLayeredEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : MenuRevealStaggerLayeredEffectModel.CreateDefault();
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

        mController = new MenuRevealStaggerLayeredEffectController();
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
