using UnityEngine;

[ReplicaShowcase("Cards", "Reflective")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class ReflectiveCardEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private ReflectiveCardEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string mHint = "ReflectiveCard  |  Hover card for metallic sheen";
    [SerializeField] private string mUserName = "ALEXANDER DOE";
    [SerializeField] private string mRole = "SENIOR DEVELOPER";
    [SerializeField] private string mIdNumber = "8901-2345-6789";
    [SerializeField] private string mBadge = "\u25CF  SECURE ACCESS";

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private ReflectiveCardEffectController mController;
    private ReflectiveCardEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("ReflectiveCardEffectHostBridge requires RectTransform.", this);
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

    public void SetModel(ReflectiveCardEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : ReflectiveCardEffectModel.CreateDefault();
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateIdentity(string userName, string role, string idNumber, bool animated = true)
    {
        EnsureModel();
        mModel.UserName = userName;
        mModel.Role = role;
        mModel.IdNumber = idNumber;
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

        mController = new ReflectiveCardEffectController();
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

        mModel = new ReflectiveCardEffectModel
        {
            Hint = mHint,
            UserName = mUserName,
            Role = mRole,
            IdNumber = mIdNumber,
            Badge = mBadge
        };
    }
}


