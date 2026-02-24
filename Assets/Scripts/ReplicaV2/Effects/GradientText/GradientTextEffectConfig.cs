using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Gradient Text Effect Config", fileName = "GradientTextEffectConfig")]
public sealed class GradientTextEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 PanelSize = new Vector2(720f, 140f);
    public Color PanelBackground = new Color(0.024f, 0.00f, 0.063f, 0.90f);
    public float BorderThickness = 2f;
    public Vector2 BorderPadding = new Vector2(22f, 10f);

    [Header("Text")]
    public int FontSize = 56;
    public FontStyle FontStyle = FontStyle.Bold;
    public TextAnchor Alignment = TextAnchor.MiddleCenter;

    [Header("Gradient")]
    public int GradientTextureWidth = 512;
    public int GradientTextureHeight = 64;
    [Range(1f, 6f)] public float GradientTiling = 3f;

    [Header("Transition")]
    public float EnterOffset = 160f;
    public float ExitOffset = 160f;
}
