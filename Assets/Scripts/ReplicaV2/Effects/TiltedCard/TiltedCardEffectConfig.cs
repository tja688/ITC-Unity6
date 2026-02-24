using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Tilted Card Effect Config", fileName = "TiltedCardEffectConfig")]
public sealed class TiltedCardEffectConfig : ScriptableObject
{
    [Header("Assets")]
    public GameObject Prefab;

    [Header("Layout")]
    public Vector2 FrameSize = new Vector2(560f, 400f);
    public Vector2 CardSize = new Vector2(420f, 280f);
    public Vector2 TooltipSize = new Vector2(140f, 30f);
    public Vector2 TooltipOffset = new Vector2(24f, 42f);

    [Header("Motion")]
    public float RotateAmplitude = 14f;
    public float HoverScale = 1.1f;
    public float TooltipRotationMax = 18f;
    public float TooltipRotationVelocityScale = 0.6f;
    public float TooltipRotationLerp = 0.26f;
    public float OverlayParallax = 6f;
    public float OverlayLerp = 0.12f;
    public float TooltipAlphaLerp = 0.16f;

    [Header("Spring")]
    public float SpringStiffness = 100f;
    public float SpringDamping = 30f;
    public float SpringMass = 2f;

    [Header("Visual")]
    public Color RootBackground = new Color(0.04f, 0.06f, 0.11f, 0.00f);
    public Color AmbientA = new Color(0.17f, 0.36f, 0.66f, 0.18f);
    public Color HintColor = new Color(0.93f, 0.96f, 1f, 0.98f);
    public Color CardColor = new Color(0.13f, 0.18f, 0.29f, 1f);
    public Color ArtworkColor = new Color(0.34f, 0.45f, 0.72f, 0.95f);
    public Color OverlayBandColor = new Color(0.05f, 0.08f, 0.15f, 0.50f);
    public Color GlintColor = new Color(1f, 1f, 1f, 0.10f);
    public Color TooltipColor = new Color(1f, 1f, 1f, 0.96f);
    public Color TooltipTextColor = new Color(0.19f, 0.21f, 0.30f, 1f);

    [Header("Animation")]
    public float EnterOffset = 180f;
    public float ExitOffset = 180f;
}

