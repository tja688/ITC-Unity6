using UnityEngine;

[ReplicaShowcase("Layouts", "Orbit Images")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class OrbitImagesEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private OrbitImagesEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string mCenterTitle = "Orbit Images";
    [SerializeField] private Sprite[] mImages;
    [SerializeField] private bool mPaused = false;

    private RectTransform mRootRect;
    private OrbitImagesEffectController mController;
    private OrbitImagesEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("OrbitImagesEffectHostBridge requires RectTransform.", this);
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        EnsureController();

        if (mModel == null)
        {
            mModel = new OrbitImagesEffectModel
            {
                CenterTitle = mCenterTitle,
                Paused = mPaused
            };

            if (mImages != null)
            {
                mModel.Images.AddRange(mImages);
            }
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

    public void SetModel(OrbitImagesEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : OrbitImagesEffectModel.CreateDefault();
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
            null,
            ReplicaTweenAdapterFactory.Create(mBackend),
            mUseUnscaledTime);

        mController = new OrbitImagesEffectController();
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
}
