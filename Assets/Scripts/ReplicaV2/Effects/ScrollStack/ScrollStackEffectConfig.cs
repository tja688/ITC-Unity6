using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/ScrollStack Effect Config", fileName = "ScrollStackEffectConfig")]
public sealed class ScrollStackEffectConfig : ScriptableObject
{
    [Header("Spacing & Layout")]
    public float itemDistance = 100f;
    public float itemStackDistance = 30f;


    [Tooltip("Percentage (0-1) of container height where stacking starts")]
    [Range(0, 1)]
    public float stackPosition = 0.2f;


    [Tooltip("Percentage (0-1) of container height where scaling finishes")]
    [Range(0, 1)]
    public float scaleEndPosition = 0.1f;

    [Header("Visual Styles")]
    public float baseScale = 0.85f;
    public float itemScale = 0.03f;
    public float rotationAmount = 0f;
    public float blurAmount = 0f;

    [Header("Animation")]
    public float scaleDuration = 0.5f;
    public bool useSmoothScroll = true;
    public float scrollLerp = 0.1f;

    [Header("Scroller Physics")]
    [Min(1f)]
    public float wheelSensitivity = 55f;

    [Range(0.01f, 0.5f)]
    public float elasticity = 0.16f;

    [Range(0.001f, 0.3f)]
    public float decelerationRate = 0.06f;

    [Header("Manual Feel (Fast + Dramatic)")]
    [Min(1f)]
    public float manualSensitivity = 2.25f;

    [Min(0f)]
    public float manualBurst = 0.012f;

    [Min(0f)]
    public float manualBurstClamp = 220f;

    [Range(0.02f, 0.35f)]
    public float manualSmoothTime = 0.08f;

    [Min(100f)]
    public float manualMaxSpeed = 15000f;

    [Min(0f)]
    public float manualSettleToRaw = 9f;

    [Min(0f)]
    public float manualOverscroll = 120f;

    [Header("Programmatic Drive (Stable + Layered)")]
    [Range(0.03f, 0.45f)]
    public float programmaticSmoothTime = 0.12f;

    [Min(100f)]
    public float programmaticMaxSpeed = 12000f;

    [Min(0.1f)]
    public float programmaticSnapThreshold = 1.5f;

    public bool syncScrollerOnProgrammaticDrive = true;
}
