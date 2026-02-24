using UnityEngine;

[ReplicaShowcase("Transitions", "Pixel Transition")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class PixelTransitionEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private PixelTransitionEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string mDefaultLabel = "DEFAULT";
    [SerializeField] private string mActiveLabel = "ACTIVE";
    [SerializeField] private Sprite mDefaultSprite;
    [SerializeField] private Sprite mActiveSprite;

    private RectTransform mRootRect;
    private PixelTransitionEffectController mController;
    private PixelTransitionEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("PixelTransitionEffectHostBridge requires RectTransform.", this);
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        EnsureController();

        if (mModel == null)
        {
            mModel = new PixelTransitionEffectModel
            {
                DefaultLabel = mDefaultLabel,
                ActiveLabel = mActiveLabel,
                DefaultSprite = mDefaultSprite,
                ActiveSprite = mActiveSprite
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

    public void SetModel(PixelTransitionEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : PixelTransitionEffectModel.CreateDefault();
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

        mController = new PixelTransitionEffectController();
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
