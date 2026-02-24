using UnityEngine;

[ReplicaShowcase("Text", "GlitchText")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class GlitchTextEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private GlitchTextEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string mText = "GLITCH";
    [SerializeField] private float mSpeed = 1f;
    [SerializeField] private bool mEnableShadows = true;
    [SerializeField] private bool mEnableOnHover = true;

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private GlitchTextEffectController mController;
    private GlitchTextEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("GlitchTextEffectHostBridge requires RectTransform.", this);
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

    public void SetModel(GlitchTextEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : GlitchTextEffectModel.CreateDefault();
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateText(string text, bool animated = true)
    {
        EnsureModel();
        mModel.Text = text;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateSpeed(float speed, bool animated = true)
    {
        EnsureModel();
        mModel.Speed = speed;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateEnableOnHover(bool enableOnHover, bool animated = true)
    {
        EnsureModel();
        mModel.EnableOnHover = enableOnHover;
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

        mController = new GlitchTextEffectController();
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

        mModel = new GlitchTextEffectModel
        {
            Text = mText,
            Speed = mSpeed,
            EnableShadows = mEnableShadows,
            EnableOnHover = mEnableOnHover
        };
    }
}

