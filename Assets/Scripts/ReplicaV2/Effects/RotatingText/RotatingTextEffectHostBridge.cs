using UnityEngine;

[ReplicaShowcase("Text", "RotatingText")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class RotatingTextEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private RotatingTextEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string[] mTexts = { "Fast", "Clean", "Reusable" };
    [SerializeField] private float mRotationInterval = 2f;
    [SerializeField] private float mStaggerDuration = 0.03f;
    [SerializeField] private RotatingTextStaggerFrom mStaggerFrom = RotatingTextStaggerFrom.First;
    [SerializeField] private bool mLoop = true;
    [SerializeField] private bool mAuto = true;
    [SerializeField] private RotatingTextSplitBy mSplitBy = RotatingTextSplitBy.Characters;

    private RectTransform mRootRect;
    private RotatingTextEffectController mController;
    private RotatingTextEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("RotatingTextEffectHostBridge requires RectTransform.", this);
            enabled = false;
            return;
        }
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

    public void SetModel(RotatingTextEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : RotatingTextEffectModel.CreateDefault();
        mController.SetModel(mModel.Clone(), animated);
    }

    public void Next(bool animated = true)
    {
        EnsureController();
        mController.Next(animated);
    }

    public void Previous(bool animated = true)
    {
        EnsureController();
        mController.Previous(animated);
    }

    public void JumpTo(int index, bool animated = true)
    {
        EnsureController();
        mController.JumpTo(index, animated);
    }

    public void Reset(bool animated = true)
    {
        EnsureController();
        mController.Reset(animated);
    }

    public void UpdateTexts(string[] texts, bool animated = true)
    {
        EnsureModel();
        mModel.Texts = texts != null ? (string[])texts.Clone() : null;
        mModel.CurrentIndex = 0;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateAuto(bool auto, bool animated = true)
    {
        EnsureModel();
        mModel.Auto = auto;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateLoop(bool loop, bool animated = true)
    {
        EnsureModel();
        mModel.Loop = loop;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateRotationInterval(float seconds, bool animated = true)
    {
        EnsureModel();
        mModel.RotationInterval = seconds;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateStagger(float staggerDuration, RotatingTextStaggerFrom staggerFrom, bool animated = true)
    {
        EnsureModel();
        mModel.StaggerDuration = staggerDuration;
        mModel.StaggerFrom = staggerFrom;
        mController.SetModel(mModel.Clone(), animated);
    }

    public void UpdateSplitBy(RotatingTextSplitBy splitBy, bool animated = true)
    {
        EnsureModel();
        mModel.SplitBy = splitBy;
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

        mController = new RotatingTextEffectController();
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

        mModel = new RotatingTextEffectModel
        {
            Texts = mTexts != null ? (string[])mTexts.Clone() : null,
            CurrentIndex = 0,
            RotationInterval = mRotationInterval,
            StaggerDuration = mStaggerDuration,
            StaggerFrom = mStaggerFrom,
            Loop = mLoop,
            Auto = mAuto,
            SplitBy = mSplitBy
        };
    }
}
