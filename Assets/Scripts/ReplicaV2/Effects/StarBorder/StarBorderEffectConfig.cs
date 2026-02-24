using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/StarBorder Effect Config", fileName = "StarBorderEffectConfig")]
public sealed class StarBorderEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 ContainerSize = new Vector2(520f, 96f);
    public float BorderRadius = 20f;
    public float BorderThickness = 1f;
    public Vector2 Padding = new Vector2(26f, 16f);
    public float EnterOffset = 220f;
    public float ExitOffset = 220f;

    [Header("Loop Motion")]
    public float SpeedSeconds = 6f;

    [Header("Glow")]
    public Color GlowColor = Color.white;
    [Range(0f, 1f)] public float GlowOpacity = 0.7f;

    [Header("Inner")]
    public Color InnerBackgroundColor = Color.black;
    public Color InnerBorderColor = new Color(0.13333334f, 0.13333334f, 0.13333334f, 1f);
    public int FontSize = 16;
    public Color TextColor = Color.white;
}
