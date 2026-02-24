using UnityEngine;

[ReplicaShowcase("Text", "CurvedLoop")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class CurvedLoopEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private CurvedLoopEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string mMarqueeText = "CURVED LOOP DRAG INTERACTION";
    [SerializeField] private float mSpeed = 160f;
    [SerializeField] private float mCurveAmount = 220f;
    [SerializeField] private bool mInteractive = true;

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private CurvedLoopEffectController mController;
    private CurvedLoopEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("CurvedLoopEffectHostBridge requires RectTransform.", this);
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

    public void SetModel(CurvedLoopEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : CurvedLoopEffectModel.CreateDefault();
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateMarqueeText(string text, bool animated = true)
    {
        EnsureModel();
        mModel.MarqueeText = text;
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

        mController = new CurvedLoopEffectController();
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

        mModel = new CurvedLoopEffectModel
        {
            MarqueeText = mMarqueeText,
            Speed = mSpeed,
            CurveAmount = mCurveAmount,
            Interactive = mInteractive
        };
    }
}

