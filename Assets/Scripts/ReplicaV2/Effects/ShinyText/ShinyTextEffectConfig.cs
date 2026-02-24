using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Shiny Text Effect Config", fileName = "ShinyTextEffectConfig")]
public sealed class ShinyTextEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 ContainerSize = new Vector2(900f, 180f);

    [Header("Text")]
    public int FontSize = 56;
    public FontStyle FontStyle = FontStyle.Bold;
    public TextAnchor Alignment = TextAnchor.MiddleCenter;

    [Header("Gradient")]
    public int GradientTextureWidth = 512;
    public int GradientTextureHeight = 64;
    [Range(1f, 3f)] public float GradientTiling = 2f;

    [Header("Transition")]
    public float EnterOffset = 160f;
    public float ExitOffset = 160f;
}
