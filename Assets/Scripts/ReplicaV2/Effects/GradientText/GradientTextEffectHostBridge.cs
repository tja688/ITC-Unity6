using UnityEngine;

[ReplicaShowcase("Text", "GradientText")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class GradientTextEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private GradientTextEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string mText = "GradientText";
    [SerializeField] private Color[] mColors = new[]
    {
        new Color(0.321f, 0.153f, 1f, 1f),
        new Color(1f, 0.624f, 0.988f, 1f),
        new Color(0.694f, 0.620f, 0.937f, 1f)
    };
    [SerializeField] private float mAnimationSpeed = 8f;
    [SerializeField] private bool mShowBorder;
    [SerializeField] private GradientTextDirection mDirection = GradientTextDirection.Horizontal;
    [SerializeField] private bool mPauseOnHover;
    [SerializeField] private bool mYoyo = true;

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private GradientTextEffectController mController;
    private GradientTextEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("GradientTextEffectHostBridge requires RectTransform.", this);
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

    public void SetModel(GradientTextEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : GradientTextEffectModel.CreateDefault();
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateText(string text, bool animated = true)
    {
        EnsureModel();
        mModel.Text = text;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateColors(Color[] colors, bool animated = true)
    {
        EnsureModel();
        mModel.Colors = colors != null ? (Color[])colors.Clone() : null;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateSpeed(float animationSpeed, bool animated = true)
    {
        EnsureModel();
        mModel.AnimationSpeed = animationSpeed;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateShowBorder(bool showBorder, bool animated = true)
    {
        EnsureModel();
        mModel.ShowBorder = showBorder;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateDirection(GradientTextDirection direction, bool animated = true)
    {
        EnsureModel();
        mModel.Direction = direction;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdatePauseOnHover(bool pauseOnHover, bool animated = true)
    {
        EnsureModel();
        mModel.PauseOnHover = pauseOnHover;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateYoyo(bool yoyo, bool animated = true)
    {
        EnsureModel();
        mModel.Yoyo = yoyo;
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

        mController = new GradientTextEffectController();
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

        mModel = new GradientTextEffectModel
        {
            Text = mText,
            Colors = mColors != null ? (Color[])mColors.Clone() : null,
            AnimationSpeed = mAnimationSpeed,
            ShowBorder = mShowBorder,
            Direction = mDirection,
            PauseOnHover = mPauseOnHover,
            Yoyo = mYoyo
        };
    }
}
