using UnityEngine;

[ReplicaShowcase("Text", "TextCursor")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class TextCursorEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private TextCursorEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string mText = "⚛️";
    [SerializeField] private float mSpacing = 100f;
    [SerializeField] private bool mFollowMouseDirection = true;
    [SerializeField] private bool mRandomFloat = true;
    [SerializeField] private float mExitDuration = 0.5f;
    [SerializeField] private int mRemovalIntervalMs = 30;
    [SerializeField] private int mMaxPoints = 5;

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private TextCursorEffectController mController;
    private TextCursorEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("TextCursorEffectHostBridge requires RectTransform.", this);
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

    public void SetModel(TextCursorEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : TextCursorEffectModel.CreateDefault();
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateText(string text, bool animated = true)
    {
        EnsureModel();
        mModel.Text = text;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateSpacing(float spacing, bool animated = true)
    {
        EnsureModel();
        mModel.Spacing = spacing;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateFollowMouseDirection(bool follow, bool animated = true)
    {
        EnsureModel();
        mModel.FollowMouseDirection = follow;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateRandomFloat(bool randomFloat, bool animated = true)
    {
        EnsureModel();
        mModel.RandomFloat = randomFloat;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateExitDuration(float exitDuration, bool animated = true)
    {
        EnsureModel();
        mModel.ExitDuration = exitDuration;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateRemovalIntervalMs(int removalIntervalMs, bool animated = true)
    {
        EnsureModel();
        mModel.RemovalIntervalMs = removalIntervalMs;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateMaxPoints(int maxPoints, bool animated = true)
    {
        EnsureModel();
        mModel.MaxPoints = maxPoints;
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

        mController = new TextCursorEffectController();
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

        mModel = new TextCursorEffectModel
        {
            Text = mText,
            Spacing = mSpacing,
            FollowMouseDirection = mFollowMouseDirection,
            RandomFloat = mRandomFloat,
            ExitDuration = mExitDuration,
            RemovalIntervalMs = mRemovalIntervalMs,
            MaxPoints = mMaxPoints
        };
    }
}
