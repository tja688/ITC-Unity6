using System.Collections.Generic;
using UnityEngine;

[ReplicaShowcase("Cursor", "TargetCursor")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class TargetCursorEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private TargetCursorEffectConfig mConfig;

    [Header("Default Model")]
    [SerializeField] private List<RectTransform> mHoverTargets = new List<RectTransform>();

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private TargetCursorEffectController mController;
    private TargetCursorEffectModel mModel;

    private void Awake()
    {
        if (!TryGetComponent(out mRootRect))
        {
            enabled = false;
            return;
        }

        mPointerRelay = ReplicaUIFactoryV2.EnsureComponent<ReplicaPointerRelay>(gameObject);

#if UNITY_EDITOR // Inject demo buttons for testing inside the Showcase
        if (Application.isPlaying && UnityEngine.Object.FindFirstObjectByType<UGUIReactBitsReplicaShowcase_V2>() != null)
        {
            gameObject.AddComponent<TargetCursorShowcaseHelper>();
        }
#endif
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
        if (mController != null)
        {
            mController.Tick(Time.deltaTime, Time.unscaledDeltaTime);
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

    public void SetHoverTargets(List<RectTransform> targets, bool animated = true)
    {
        EnsureModel();
        mModel.HoverTargets = targets != null ? new List<RectTransform>(targets) : new List<RectTransform>();
        mController?.SetModel(mModel.Clone(), animated);
    }

    public void AddHoverTarget(RectTransform target)
    {
        EnsureModel();
        if (target != null && !mModel.HoverTargets.Contains(target))
        {
            mModel.HoverTargets.Add(target);
            mController?.SetModel(mModel.Clone(), true);
        }
    }

    public void RemoveHoverTarget(RectTransform target)
    {
        EnsureModel();
        if (target != null && mModel.HoverTargets.Contains(target))
        {
            mModel.HoverTargets.Remove(target);
            mController?.SetModel(mModel.Clone(), true);
        }
    }

    public void PlayIn(ReplicaEnterDirection direction)
    {
        EnsureController();
        var transition = ReplicaTransition.Default;
        transition.EnterDirection = direction;
        mController.PlayIn(transition);
    }

    public void PlayOut(ReplicaExitDirection direction)
    {
        EnsureController();
        var transition = ReplicaTransition.Default;
        transition.ExitDirection = direction;
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
        if (mController != null) return;

        var context = new ReplicaHostContext(
            mRootRect,
            null,
            mPointerRelay,
            ReplicaTweenAdapterFactory.Create(mBackend),
            mUseUnscaledTime);

        mController = new TargetCursorEffectController();
        mController.Initialize(context, mConfig);
    }

    private void DisposeController()
    {
        mController?.Dispose();
        mController = null;
    }

    private void EnsureModel()
    {
        if (mModel != null) return;

        mModel = new TargetCursorEffectModel
        {
            HoverTargets = mHoverTargets != null ? new List<RectTransform>(mHoverTargets) : new List<RectTransform>()
        };
    }
}
