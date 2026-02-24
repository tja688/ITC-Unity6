using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Reflective Card Effect Config", fileName = "ReflectiveCardEffectConfig")]
public sealed class ReflectiveCardEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 StageSize = new Vector2(1080f, 760f);
    public Vector2 CardSize = new Vector2(380f, 580f);
    public int NoiseStripCount = 30;
    public Vector2 SheenSize = new Vector2(160f, 820f);
    public Vector2 SpotlightSize = new Vector2(520f, 520f);

    [Header("Motion")]
    public float TiltStrength = 12f;
    public float ParallaxStrength = 18f;
    public float TiltSmooth = 8f;
    public float HoverBlendSpeed = 4f;
    public float MotionEnergySmooth = 10f;

    [Header("Sheen")]
    public float SheenAutoSweepSpeed = 0.6f;
    public float SheenAutoSweepAmplitude = 40f;
    public float SheenMouseInfluence = 180f;
    public float SheenFadeSpeed = 6f;

    [Header("Spotlight")]
    public float SpotlightX = 120f;
    public float SpotlightY = 160f;
    public float SpotlightMoveSpeed = 8f;
    public float SpotlightFadeSpeed = 6f;

    [Header("Noise")]
    public float NoiseWaveSpeed = 7f;
    public float NoiseAlphaSpeed = 9f;

    [Header("Visual")]
    public Color HintColor = new Color(0.93f, 0.96f, 1f, 0.95f);
    public Color StageColor = new Color(0.08f, 0.10f, 0.17f, 0.90f);
    public Color CardRestColor = new Color(0.12f, 0.15f, 0.20f, 1f);
    public Color CardActiveColor = new Color(0.22f, 0.26f, 0.34f, 1f);
    public Color OverlayTint = new Color(0.90f, 0.92f, 0.98f, 0.08f);
    public Color SheenColor = new Color(1f, 1f, 1f, 0.22f);
    public Color SpotlightColor = new Color(0.82f, 0.88f, 1f, 0.16f);
    public Color NoiseStripColor = new Color(1f, 1f, 1f, 0.02f);

    [Header("Animation")]
    public float EnterOffset = 220f;
    public float ExitOffset = 220f;
}

