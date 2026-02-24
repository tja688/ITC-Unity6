using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Glass Icons Tilt Effect Config", fileName = "GlassIconsTiltEffectConfig")]
public sealed class GlassIconsTiltEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 StageSize = new Vector2(980f, 620f);
    public Vector2 CellSize = new Vector2(210f, 230f);
    public Vector2 CellSpacing = new Vector2(26f, 22f);
    public int ColumnCount = 3;

    [Header("Motion")]
    public float HoverScale = 1.04f;
    public float HoverDuration = 0.24f;
    public float IdleDuration = 0.20f;
    public Vector2 BackHoverOffset = new Vector2(-12f, 10f);
    public float BackHoverRotation = 25f;
    public float BackIdleRotation = 15f;
    public Vector2 FrontHoverOffset = new Vector2(0f, -8f);
    public float FrontHoverScale = 1.08f;
    public float LabelHoverY = -150f;
    public float LabelIdleY = -168f;

    [Header("Visual")]
    public Color BackdropColor = new Color(0.05f, 0.07f, 0.13f, 0.90f);
    public Color PlateAColor = new Color(0.24f, 0.42f, 0.88f, 0.22f);
    public Color PlateBColor = new Color(0.58f, 0.31f, 0.82f, 0.20f);
    public Color FrontGlassColor = new Color(1f, 1f, 1f, 0.14f);
    public Color HintColor = new Color(0.93f, 0.96f, 1f, 0.95f);
    public Color GlyphColor = new Color(0.94f, 0.97f, 1f, 0.96f);
    public Color LabelColor = new Color(0.88f, 0.92f, 1f, 0.98f);

    [Header("Animation")]
    public float EnterOffset = 120f;
    public float ExitOffset = 120f;
}
