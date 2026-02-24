using UnityEngine;

[ReplicaShowcase("Cards", "Spotlight")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class SpotlightCardEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private SpotlightCardEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string mHint = "SpotlightCard  |  Hover card to move light";
    [SerializeField] private string mTitle = "Interactive Spotlight Surface";
    [SerializeField] private string mBody = "Hover to reveal follow-light glow.\nThe spotlight tracks pointer position in real time.";

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private SpotlightCardEffectController mController;
    private SpotlightCardEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("SpotlightCardEffectHostBridge requires RectTransform.", this);
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

    public void SetModel(SpotlightCardEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : SpotlightCardEffectModel.CreateDefault();
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateHint(string hint, bool animated = true)
    {
        EnsureModel();
        mModel.Hint = hint;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateTitle(string title, bool animated = true)
    {
        EnsureModel();
        mModel.Title = title;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateBody(string body, bool animated = true)
    {
        EnsureModel();
        mModel.Body = body;
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

        mController = new SpotlightCardEffectController();
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

        mModel = new SpotlightCardEffectModel
        {
            Hint = mHint,
            Title = mTitle,
            Body = mBody
        };
    }
}


