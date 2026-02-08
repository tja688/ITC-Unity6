using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(SpriteRenderer))]
public sealed class LihuiMotionRootInspector : MonoBehaviour
{
    private const string GeneratedFolderPath = "Assets/Tests/Test generated";
    private const string ShaderAssetPath = "Assets/Tests/Test generated/SHD_LihuiSpriteMotion.shader";
    private const string BreathMaterialPath = "Assets/Tests/Test generated/MAT_LihuiFX_Breathing.mat";
    private const string HeartbeatMaterialPath = "Assets/Tests/Test generated/MAT_LihuiFX_HeartbeatPulse.mat";
    private const string MeshWaveMaterialPath = "Assets/Tests/Test generated/MAT_LihuiFX_MeshWave.mat";

    [Header("Panel")]
    [SerializeField]
    private KeyCode mToggleKey = KeyCode.F2;

    [SerializeField]
    private bool mShowPanel;

    [SerializeField]
    private Rect mPanelRect = new Rect(24f, 24f, 420f, 680f);

    [Header("Runtime Refs")]
    [SerializeField]
    private SpriteRenderer mSourceRenderer;

    [SerializeField]
    private Transform mMotionRoot;

    [SerializeField]
    private Material mBreathingMaterial;

    [SerializeField]
    private Material mHeartbeatMaterial;

    [SerializeField]
    private Material mMeshWaveMaterial;

    private readonly LihuiMotionDotweenNode[] mDotweenNodes =
        new LihuiMotionDotweenNode[LihuiMotionEffectCatalog.DotweenEffects.Length];

    private readonly LihuiMotionShaderNode[] mShaderNodes =
        new LihuiMotionShaderNode[LihuiMotionEffectCatalog.ShaderEffects.Length];

    private readonly bool[] mShaderEnabled =
        new bool[LihuiMotionEffectCatalog.ShaderEffects.Length];

    private int mActiveDotweenIndex = -1;
    private Vector2 mScrollPosition;

    private void OnEnable()
    {
        if (!TryGetComponent(out mSourceRenderer))
        {
            mSourceRenderer = null;
            return;
        }

#if UNITY_EDITOR
        EnsureGeneratedMaterials();
#endif
        EnsureHierarchyAndNodes();
        DisableAllEffects();
    }

    private void OnDisable()
    {
        DisableAllEffects();
        if (mSourceRenderer != null)
        {
            mSourceRenderer.enabled = true;
        }
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (Input.GetKeyDown(mToggleKey))
        {
            mShowPanel = !mShowPanel;
        }

        SyncShaderOverlayTransforms();
    }

    private void OnGUI()
    {
        if (!Application.isPlaying || !mShowPanel)
        {
            return;
        }

        mPanelRect = GUILayout.Window(GetInstanceID(), mPanelRect, DrawPanelWindow, "Lihui Motion Inspector (F2)");
    }

    private void DrawPanelWindow(int windowId)
    {
        mScrollPosition = GUILayout.BeginScrollView(mScrollPosition, GUILayout.Width(mPanelRect.width - 10f), GUILayout.Height(mPanelRect.height - 40f));

        GUILayout.Label("DOTween Transform/Scale/Rotate");
        for (var i = 0; i < LihuiMotionEffectCatalog.DotweenEffects.Length; i++)
        {
            var labelPrefix = mActiveDotweenIndex == i ? "[ON]" : "[ ]";
            if (GUILayout.Button($"{labelPrefix} {LihuiMotionEffectCatalog.DotweenEffects[i].DisplayName}"))
            {
                PlayDotweenEffect(i);
            }
        }

        GUILayout.Space(8f);
        GUILayout.Label("Shader Material Overlay");
        for (var i = 0; i < LihuiMotionEffectCatalog.ShaderEffects.Length; i++)
        {
            var updated = GUILayout.Toggle(mShaderEnabled[i], LihuiMotionEffectCatalog.ShaderEffects[i].DisplayName);
            if (updated != mShaderEnabled[i])
            {
                ToggleShaderEffect(i, updated);
            }
        }

        GUILayout.Space(10f);
        if (GUILayout.Button("Disable All Effects"))
        {
            DisableAllEffects();
        }

        GUILayout.EndScrollView();
        GUI.DragWindow(new Rect(0f, 0f, 10000f, 24f));
    }

    private void EnsureHierarchyAndNodes()
    {
        if (mSourceRenderer == null)
        {
            return;
        }

        if (mMotionRoot == null)
        {
            var root = transform.Find(LihuiMotionEffectCatalog.MotionRootName);
            if (root == null)
            {
                var rootObject = new GameObject(LihuiMotionEffectCatalog.MotionRootName);
                rootObject.transform.SetParent(transform, false);
                root = rootObject.transform;
            }

            mMotionRoot = root;
        }

        for (var i = 0; i < LihuiMotionEffectCatalog.DotweenEffects.Length; i++)
        {
            var definition = LihuiMotionEffectCatalog.DotweenEffects[i];
            var node = GetOrCreateDotweenNode(definition.NodeName);
            node.Configure(definition.Id, mSourceRenderer);
            node.gameObject.SetActive(false);
            mDotweenNodes[i] = node;
        }

        for (var i = 0; i < LihuiMotionEffectCatalog.ShaderEffects.Length; i++)
        {
            var definition = LihuiMotionEffectCatalog.ShaderEffects[i];
            var node = GetOrCreateShaderNode(definition.NodeName);
            node.Configure(definition.Id, mSourceRenderer, GetShaderTemplate(definition.Id), 4 + i);
            node.gameObject.SetActive(false);
            mShaderNodes[i] = node;
        }
    }

    private LihuiMotionDotweenNode GetOrCreateDotweenNode(string childName)
    {
        var child = mMotionRoot.Find(childName);
        if (child == null)
        {
            var childObject = new GameObject(childName);
            childObject.transform.SetParent(mMotionRoot, false);
            child = childObject.transform;
        }

        if (!child.TryGetComponent<LihuiMotionDotweenNode>(out var node))
        {
            node = child.gameObject.AddComponent<LihuiMotionDotweenNode>();
        }

        return node;
    }

    private LihuiMotionShaderNode GetOrCreateShaderNode(string childName)
    {
        var child = mMotionRoot.Find(childName);
        if (child == null)
        {
            var childObject = new GameObject(childName);
            childObject.transform.SetParent(mMotionRoot, false);
            child = childObject.transform;
        }

        if (!child.TryGetComponent<LihuiMotionShaderNode>(out var node))
        {
            node = child.gameObject.AddComponent<LihuiMotionShaderNode>();
        }

        return node;
    }

    private Material GetShaderTemplate(LihuiShaderEffectId effectId)
    {
        switch (effectId)
        {
            case LihuiShaderEffectId.Breathing:
                return mBreathingMaterial;
            case LihuiShaderEffectId.HeartbeatPulse:
                return mHeartbeatMaterial;
            case LihuiShaderEffectId.MeshWave:
                return mMeshWaveMaterial;
            default:
                return mBreathingMaterial;
        }
    }

    private void PlayDotweenEffect(int index)
    {
        if (index < 0 || index >= mDotweenNodes.Length || mDotweenNodes[index] == null)
        {
            return;
        }

        if (mActiveDotweenIndex >= 0 &&
            mActiveDotweenIndex < mDotweenNodes.Length &&
            mDotweenNodes[mActiveDotweenIndex] != null &&
            mActiveDotweenIndex != index)
        {
            mDotweenNodes[mActiveDotweenIndex].StopEffect();
        }

        mActiveDotweenIndex = index;
        mDotweenNodes[index].PlayEffect();
        RefreshSourceVisibility();
    }

    private void ToggleShaderEffect(int index, bool enabled)
    {
        if (index < 0 || index >= mShaderNodes.Length || mShaderNodes[index] == null)
        {
            return;
        }

        mShaderEnabled[index] = enabled;
        mShaderNodes[index].SetEffectEnabled(enabled);
        RefreshSourceVisibility();
    }

    private void DisableAllEffects()
    {
        if (mActiveDotweenIndex >= 0 && mActiveDotweenIndex < mDotweenNodes.Length && mDotweenNodes[mActiveDotweenIndex] != null)
        {
            mDotweenNodes[mActiveDotweenIndex].StopEffect();
        }

        for (var i = 0; i < mDotweenNodes.Length; i++)
        {
            if (mDotweenNodes[i] != null)
            {
                mDotweenNodes[i].StopEffect();
            }
        }

        for (var i = 0; i < mShaderNodes.Length; i++)
        {
            mShaderEnabled[i] = false;
            if (mShaderNodes[i] != null)
            {
                mShaderNodes[i].SetEffectEnabled(false);
            }
        }

        mActiveDotweenIndex = -1;
        RefreshSourceVisibility();
    }

    private void RefreshSourceVisibility()
    {
        if (mSourceRenderer == null)
        {
            return;
        }
        
        // Shader effects are overlays; keep source visible unless a DOTween clone is currently active.
        mSourceRenderer.enabled = mActiveDotweenIndex < 0;
    }

    private void SyncShaderOverlayTransforms()
    {
        var anchor = transform;
        if (mActiveDotweenIndex >= 0 &&
            mActiveDotweenIndex < mDotweenNodes.Length &&
            mDotweenNodes[mActiveDotweenIndex] != null &&
            mDotweenNodes[mActiveDotweenIndex].gameObject.activeInHierarchy)
        {
            anchor = mDotweenNodes[mActiveDotweenIndex].transform;
        }

        for (var i = 0; i < mShaderNodes.Length; i++)
        {
            if (!mShaderEnabled[i] || mShaderNodes[i] == null || !mShaderNodes[i].gameObject.activeInHierarchy)
            {
                continue;
            }

            var shaderTransform = mShaderNodes[i].transform;
            shaderTransform.localPosition = anchor.localPosition;
            shaderTransform.localRotation = anchor.localRotation;
            shaderTransform.localScale = anchor.localScale;
        }
    }

#if UNITY_EDITOR
    private void EnsureGeneratedMaterials()
    {
        var shader = AssetDatabase.LoadAssetAtPath<Shader>(ShaderAssetPath);
        if (shader == null)
        {
            shader = Shader.Find(LihuiMotionEffectCatalog.ShaderName);
        }

        if (shader == null)
        {
            return;
        }

        EnsureGeneratedFolderExists();

        mBreathingMaterial = LoadOrCreateMaterial(
            BreathMaterialPath,
            shader,
            0.45f,
            0.012f,
            1.2f,
            0f,
            0f,
            0f);

        mHeartbeatMaterial = LoadOrCreateMaterial(
            HeartbeatMaterialPath,
            shader,
            0.5f,
            0.005f,
            0.9f,
            0f,
            0f,
            0f);

        mMeshWaveMaterial = LoadOrCreateMaterial(
            MeshWaveMaterialPath,
            shader,
            0.38f,
            0.008f,
            0.8f,
            0.02f,
            2.8f,
            1.7f);

        EditorUtility.SetDirty(this);
    }

    private static void EnsureGeneratedFolderExists()
    {
        var absoluteFolder = Path.Combine(Application.dataPath, "Tests/Test generated");
        if (!Directory.Exists(absoluteFolder))
        {
            Directory.CreateDirectory(absoluteFolder);
            AssetDatabase.Refresh();
        }
    }

    private static Material LoadOrCreateMaterial(
        string path,
        Shader shader,
        float alpha,
        float breathAmp,
        float breathSpeed,
        float waveAmp,
        float waveFreq,
        float waveSpeed)
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat == null)
        {
            mat = new Material(shader);
            AssetDatabase.CreateAsset(mat, path);
        }
        else if (mat.shader != shader)
        {
            mat.shader = shader;
        }

        mat.SetFloat("_AlphaMultiplier", alpha);
        mat.SetFloat("_BreathAmp", breathAmp);
        mat.SetFloat("_BreathSpeed", breathSpeed);
        mat.SetFloat("_WaveAmp", waveAmp);
        mat.SetFloat("_WaveFreq", waveFreq);
        mat.SetFloat("_WaveSpeed", waveSpeed);
        mat.SetFloat("_RuntimePulse", 0f);
        EditorUtility.SetDirty(mat);
        return mat;
    }
#endif
}
