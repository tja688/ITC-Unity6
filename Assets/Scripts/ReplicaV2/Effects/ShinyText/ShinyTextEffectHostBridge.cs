using UnityEngine;

[ReplicaShowcase("Text", "ShinyText")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class ShinyTextEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private ShinyTextEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string mText = "ShinyText";
    [SerializeField] private bool mDisabled;
    [SerializeField] private float mSpeed = 2f;
    [SerializeField] private Color mColor = new Color(0.71f, 0.71f, 0.71f, 1f);
    [SerializeField] private Color mShineColor = Color.white;
    [SerializeField] private float mSpread = 120f;
    [SerializeField] private bool mYoyo;
    [SerializeField] private bool mPauseOnHover;
    [SerializeField] private ShinyTextDirection mDirection = ShinyTextDirection.Left;
    [SerializeField] private float mDelay;

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private ShinyTextEffectController mController;
    private ShinyTextEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("ShinyTextEffectHostBridge requires RectTransform.", this);
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

    public void SetModel(ShinyTextEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : ShinyTextEffectModel.CreateDefault();
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateText(string text, bool animated = true)
    {
        EnsureModel();
        mModel.Text = text;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateDisabled(bool disabled, bool animated = true)
    {
        EnsureModel();
        mModel.Disabled = disabled;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateSpeed(float speed, bool animated = true)
    {
        EnsureModel();
        mModel.Speed = speed;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateColors(Color baseColor, Color shineColor, bool animated = true)
    {
        EnsureModel();
        mModel.BaseColor = baseColor;
        mModel.ShineColor = shineColor;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateYoyo(bool yoyo, bool animated = true)
    {
        EnsureModel();
        mModel.Yoyo = yoyo;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdatePauseOnHover(bool pauseOnHover, bool animated = true)
    {
        EnsureModel();
        mModel.PauseOnHover = pauseOnHover;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateDirection(ShinyTextDirection direction, bool animated = true)
    {
        EnsureModel();
        mModel.Direction = direction;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateDelay(float delay, bool animated = true)
    {
        EnsureModel();
        mModel.Delay = delay;
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

        mController = new ShinyTextEffectController();
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

        mModel = new ShinyTextEffectModel
        {
            Text = mText,
            Disabled = mDisabled,
            Speed = mSpeed,
            BaseColor = mColor,
            ShineColor = mShineColor,
            SpreadDegrees = mSpread,
            Yoyo = mYoyo,
            PauseOnHover = mPauseOnHover,
            Direction = mDirection,
            Delay = mDelay
        };
    }
}
