using UnityEngine;

[ReplicaShowcase("Transitions", "Fade Content")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class FadeContentEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private FadeContentEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string mTitle = "Fade Content";
    [SerializeField] private string mBody = "Basic but essential transition wrapper.";

    private RectTransform mRootRect;
    private FadeContentEffectController mController;
    private FadeContentEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("FadeContentEffectHostBridge requires RectTransform.", this);
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        EnsureController();

        if (mModel == null)
        {
            mModel = new FadeContentEffectModel
            {
                Title = mTitle,
                Body = mBody
            };
        }

        mController.SetModel(mModel.Clone(), false);

        if (mPlayInOnEnable)
        {
            PlayIn(ReplicaEnterDirection.FromBottom);
        }
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

    public void SetModel(FadeContentEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : FadeContentEffectModel.CreateDefault();
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
            null,
            ReplicaTweenAdapterFactory.Create(mBackend),
            mUseUnscaledTime);

        mController = new FadeContentEffectController();
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

        mModel = new FadeContentEffectModel
        {
            Title = mTitle,
            Body = mBody
        };
    }
}
