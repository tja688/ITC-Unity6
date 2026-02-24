using UnityEngine;

[ReplicaShowcase("Cards", "Decay")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class DecayEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private DecayEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string mTitle = "NEXT GEN";
    [SerializeField] private string mSubtitle = "Pointer-driven distortion";
    [SerializeField] private string mMarker = "DECAY";
    [SerializeField] private Sprite mPhotoSprite;

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private DecayEffectController mController;
    private DecayEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("DecayEffectHostBridge requires RectTransform.", this);
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
            mModel = new DecayEffectModel
            {
                Title = mTitle,
                Subtitle = mSubtitle,
                Marker = mMarker,
                PhotoSprite = mPhotoSprite
            };
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

    public void SetModel(DecayEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : DecayEffectModel.CreateDefault();
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateTitle(string title, bool animated = true)
    {
        EnsureModel();
        mModel.Title = title;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateSubtitle(string subtitle, bool animated = true)
    {
        EnsureModel();
        mModel.Subtitle = subtitle;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateSprite(Sprite sprite, bool animated = true)
    {
        EnsureModel();
        mModel.PhotoSprite = sprite;
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

        mController = new DecayEffectController();
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

        mModel = new DecayEffectModel
        {
            Title = mTitle,
            Subtitle = mSubtitle,
            Marker = mMarker,
            PhotoSprite = mPhotoSprite
        };
    }
}

