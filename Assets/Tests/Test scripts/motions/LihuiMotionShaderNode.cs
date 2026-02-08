using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
[RequireComponent(typeof(Transform))]
public sealed class LihuiMotionShaderNode : MonoBehaviour
{
    private static readonly int BreathAmpId = Shader.PropertyToID("_BreathAmp");
    private static readonly int BreathSpeedId = Shader.PropertyToID("_BreathSpeed");
    private static readonly int WaveAmpId = Shader.PropertyToID("_WaveAmp");
    private static readonly int WaveFreqId = Shader.PropertyToID("_WaveFreq");
    private static readonly int WaveSpeedId = Shader.PropertyToID("_WaveSpeed");
    private static readonly int RuntimePulseId = Shader.PropertyToID("_RuntimePulse");
    private static readonly int AlphaMultiplierId = Shader.PropertyToID("_AlphaMultiplier");

    [SerializeField]
    private LihuiShaderEffectId mEffectId;

    [SerializeField]
    [Range(0.1f, 1f)]
    private float mOverlayAlpha = 0.42f;

    private SpriteRenderer mRenderer;
    private Material mRuntimeMaterial;
    private MaterialPropertyBlock mPropertyBlock;

    public LihuiShaderEffectId EffectId => mEffectId;

    public void Configure(LihuiShaderEffectId effectId, SpriteRenderer sourceRenderer, Material templateMaterial, int sortingOrderOffset)
    {
        mEffectId = effectId;
        EnsureRenderer(sourceRenderer, sortingOrderOffset);
        EnsureMaterial(templateMaterial);
        ApplyStaticDefaults();
    }

    public void SetEffectEnabled(bool enabled)
    {
        gameObject.SetActive(enabled);
        if (!enabled || mRenderer == null)
        {
            return;
        }

        mRenderer.enabled = true;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy || mRenderer == null || mRuntimeMaterial == null)
        {
            return;
        }

        var t = Time.realtimeSinceStartup;
        var breathAmp = 0f;
        var breathSpeed = 1.2f;
        var waveAmp = 0f;
        var waveFreq = 3f;
        var waveSpeed = 1.8f;
        var pulse = 0f;

        switch (mEffectId)
        {
            case LihuiShaderEffectId.Breathing:
                breathAmp = 0.016f;
                breathSpeed = 1.25f;
                break;

            case LihuiShaderEffectId.HeartbeatPulse:
                pulse = ComputeHeartbeatPulse(t) * 0.05f;
                breathAmp = 0.006f;
                breathSpeed = 0.9f;
                break;

            case LihuiShaderEffectId.MeshWave:
                breathAmp = 0.008f;
                breathSpeed = 0.8f;
                waveAmp = 0.028f;
                waveFreq = 2.8f;
                waveSpeed = 1.7f;
                break;
        }

        if (mPropertyBlock == null)
        {
            mPropertyBlock = new MaterialPropertyBlock();
        }

        mRenderer.GetPropertyBlock(mPropertyBlock);
        mPropertyBlock.SetFloat(BreathAmpId, breathAmp);
        mPropertyBlock.SetFloat(BreathSpeedId, breathSpeed);
        mPropertyBlock.SetFloat(WaveAmpId, waveAmp);
        mPropertyBlock.SetFloat(WaveFreqId, waveFreq);
        mPropertyBlock.SetFloat(WaveSpeedId, waveSpeed);
        mPropertyBlock.SetFloat(RuntimePulseId, pulse);
        mPropertyBlock.SetFloat(AlphaMultiplierId, mOverlayAlpha);
        mRenderer.SetPropertyBlock(mPropertyBlock);
    }

    private void EnsureRenderer(SpriteRenderer sourceRenderer, int sortingOrderOffset)
    {
        if (!TryGetComponent(out mRenderer))
        {
            mRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        if (sourceRenderer == null)
        {
            return;
        }

        mRenderer.sprite = sourceRenderer.sprite;
        mRenderer.color = sourceRenderer.color;
        mRenderer.flipX = sourceRenderer.flipX;
        mRenderer.flipY = sourceRenderer.flipY;
        mRenderer.sortingLayerID = sourceRenderer.sortingLayerID;
        mRenderer.sortingOrder = sourceRenderer.sortingOrder + sortingOrderOffset;
    }

    private void EnsureMaterial(Material templateMaterial)
    {
        if (templateMaterial == null)
        {
            var shader = Shader.Find(LihuiMotionEffectCatalog.ShaderName);
            if (shader == null)
            {
                return;
            }

            templateMaterial = new Material(shader);
        }

        if (mRuntimeMaterial == null || mRuntimeMaterial.shader != templateMaterial.shader)
        {
            if (mRuntimeMaterial != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(mRuntimeMaterial);
                }
                else
                {
                    DestroyImmediate(mRuntimeMaterial);
                }
            }

            mRuntimeMaterial = new Material(templateMaterial);
            mRuntimeMaterial.name = $"{name}_RuntimeMaterial";
        }

        mRenderer.sharedMaterial = mRuntimeMaterial;
    }

    private void ApplyStaticDefaults()
    {
        if (mRuntimeMaterial == null)
        {
            return;
        }

        mRuntimeMaterial.SetFloat(BreathAmpId, 0f);
        mRuntimeMaterial.SetFloat(BreathSpeedId, 1.2f);
        mRuntimeMaterial.SetFloat(WaveAmpId, 0f);
        mRuntimeMaterial.SetFloat(WaveFreqId, 3f);
        mRuntimeMaterial.SetFloat(WaveSpeedId, 1.8f);
        mRuntimeMaterial.SetFloat(RuntimePulseId, 0f);
        mRuntimeMaterial.SetFloat(AlphaMultiplierId, mOverlayAlpha);
    }

    private static float ComputeHeartbeatPulse(float timeValue)
    {
        var phase = Mathf.Repeat(timeValue, 1.2f);
        var beatA = Mathf.Exp(-Mathf.Pow((phase - 0.08f) / 0.05f, 2f));
        var beatB = Mathf.Exp(-Mathf.Pow((phase - 0.24f) / 0.06f, 2f)) * 0.7f;
        return beatA + beatB;
    }
}

