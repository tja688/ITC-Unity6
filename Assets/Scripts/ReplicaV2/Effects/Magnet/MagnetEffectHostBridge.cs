using UnityEngine;

[ReplicaShowcase("Animations", "Magnet")]
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class MagnetEffectHostBridge : MonoBehaviour, IReplicaShowcaseBridge
{
    [Header("Runtime")]
    [SerializeField] private ReplicaTweenBackend mBackend = ReplicaTweenBackend.DOTween;
    [SerializeField] private bool mUseUnscaledTime = true;
    [SerializeField] private bool mPlayInOnEnable = true;

    [Header("Config")]
    [SerializeField] private MagnetEffectConfig mConfig;
    [SerializeField] private GameObject mPrefabOverwrite;

    [Header("Default Model")]
    [SerializeField] private string mDefaultTitle = "Hover Me";
    [SerializeField] private string mDefaultHint = "Move pointer near the button";

    private RectTransform mRootRect;
    private ReplicaPointerRelay mPointerRelay;
    private MagnetEffectController mController;
    private MagnetEffectModel mModel;
    private MagnetEffectConfig mRuntimeConfig;

    private void Awake()
    {
#if UNITY_EDITOR
        // If this host is already attached to a baked view prefab (has linker),
        // force-clear overwrite to avoid nested-host recursion.
        if (TryGetComponent<MagnetViewLinker>(out _))
        {
            mPrefabOverwrite = null;
        }
        else if (mPrefabOverwrite == null)
        {
            mPrefabOverwrite = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ReplicaV2/Prefabs/Effects/Magnet_Baked.prefab");
        }
#endif
        if (!TryGetComponent(out mRootRect))
        {
            Debug.LogError("MagnetEffectHostBridge requires RectTransform.", this);
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
        mController?.Tick(Time.deltaTime, Time.unscaledDeltaTime);
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

    public void SetModel(MagnetEffectModel model, bool animated = true)
    {
        EnsureController();
        mModel = model != null ? model.Clone() : MagnetEffectModel.CreateDefault();
        mController.SetModel(mModel.Clone(), animated);
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

        mController = new MagnetEffectController();
        var initConfig = mConfig;
        if (mConfig != null)
        {
            mRuntimeConfig = ScriptableObject.Instantiate(mConfig);
            if (mPrefabOverwrite != null)
            {
                mRuntimeConfig.Prefab = mPrefabOverwrite;
            }

            initConfig = mRuntimeConfig;
        }
        else if (mPrefabOverwrite != null)
        {
            mRuntimeConfig = ScriptableObject.CreateInstance<MagnetEffectConfig>();
            mRuntimeConfig.Prefab = mPrefabOverwrite;
            initConfig = mRuntimeConfig;
        }

        mController.Initialize(context, initConfig);
    }

    private void DisposeController()
    {
        if (mController == null)
        {
            return;
        }

        mController.Dispose();
        mController = null;

        if (mRuntimeConfig != null)
        {
            Destroy(mRuntimeConfig);
            mRuntimeConfig = null;
        }
    }

    private void EnsureModel()
    {
        if (mModel != null)
        {
            return;
        }

        mModel = MagnetEffectModel.CreateDefault();
        mModel.Title = string.IsNullOrWhiteSpace(mDefaultTitle) ? "Hover Me" : mDefaultTitle;
        mModel.Hint = string.IsNullOrWhiteSpace(mDefaultHint) ? "Move pointer near the button" : mDefaultHint;
    }
}
