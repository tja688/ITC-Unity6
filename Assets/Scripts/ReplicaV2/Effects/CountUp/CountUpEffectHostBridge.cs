using UnityEngine;

[ReplicaShowcase("Text", "CountUp")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class CountUpEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private CountUpEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private float mFromValue = 0f;
    [SerializeField] private float mInitialTarget = 12840f;
    [SerializeField] private float mDuration = 2f;
    [SerializeField] private float mDelay = 0.25f;
    [SerializeField] private float mCycleInterval = 2.4f;
    [SerializeField] private int mDecimals = 0;
    [SerializeField] private string mSeparator = ",";
    [SerializeField] private bool mCountDown = false;

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private CountUpEffectController mController;
    private CountUpEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("CountUpEffectHostBridge requires RectTransform.", this);
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

    public void SetModel(CountUpEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : CountUpEffectModel.CreateDefault();
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

        mController = new CountUpEffectController();
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

        mModel = new CountUpEffectModel
        {
            FromValue = mFromValue,
            InitialTarget = mInitialTarget,
            Duration = mDuration,
            Delay = mDelay,
            CycleInterval = mCycleInterval,
            Decimals = mDecimals,
            Separator = mSeparator,
            CountDown = mCountDown
        };
    }
}

