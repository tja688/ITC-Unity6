using UnityEngine;

[ReplicaShowcase("Animations", "AnimatedContent")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class AnimatedContentEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = false;
    [SerializeField] private bool mPlayInWhenVisible = true;

    [Header("Visibility Trigger")]
    [SerializeField] private RectTransform mViewport;

    [Header("Config")]
    [SerializeField] private AnimatedContentEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private string mLabel = "AnimatedContent";

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private AnimatedContentEffectController mController;
    private AnimatedContentEffectModel mModel;
    private bool mTriggered;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("AnimatedContentEffectHostBridge requires RectTransform.", this);
            enabled = false;
            return;
        }

        mPointerRelay = ReplicaUIFactoryV2.EnsureComponent<ReplicaPointerRelay>(gameObject);
    }

    private void OnEnable()
    {
        mTriggered = false;
        EnsureController();
        EnsureModel();
        mController.SetModel(mModel.Clone(), false);

        if (mPlayInOnEnable)
        {
            PlayIn(ReplicaEnterDirection.FromBottom);
            mTriggered = true;
        }
    }

    private void Update()
    {
        if (mController == null)
        {
            return;
        }

        mController.Tick(Time.deltaTime, Time.unscaledDeltaTime);

        if (mPlayInWhenVisible && !mTriggered)
        {
            var threshold = mConfig != null ? Mathf.Clamp01(mConfig.Threshold) : 0.1f;
            if (VisibleRatio(mViewport, mRootRect) >= threshold)
            {
                PlayIn(ReplicaEnterDirection.FromBottom);
                mTriggered = true;
            }
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

    public void SetModel(AnimatedContentEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : AnimatedContentEffectModel.CreateDefault();
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

        mController = new AnimatedContentEffectController();
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

        mModel = new AnimatedContentEffectModel
        {
            Label = mLabel
        };
    }

    private static float VisibleRatio(RectTransform viewport, RectTransform target)
    {
        if (target == null)
        {
            return 0f;
        }

        if (viewport == null)
        {
            return 1f;
        }

        var targetBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, target);
        var viewRect = viewport.rect;

        var xMin = Mathf.Max(targetBounds.min.x, viewRect.xMin);
        var xMax = Mathf.Min(targetBounds.max.x, viewRect.xMax);
        var yMin = Mathf.Max(targetBounds.min.y, viewRect.yMin);
        var yMax = Mathf.Min(targetBounds.max.y, viewRect.yMax);

        var w = Mathf.Max(0f, xMax - xMin);
        var h = Mathf.Max(0f, yMax - yMin);
        var visibleArea = w * h;

        var totalW = Mathf.Max(0.0001f, targetBounds.size.x);
        var totalH = Mathf.Max(0.0001f, targetBounds.size.y);
        var totalArea = totalW * totalH;

        return Mathf.Clamp01(visibleArea / totalArea);
    }
}
