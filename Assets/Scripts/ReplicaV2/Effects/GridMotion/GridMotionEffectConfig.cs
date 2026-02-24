using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Grid Motion Effect Config", fileName = "GridMotionEffectConfig")]
public sealed class GridMotionEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public int RowCount = 4;
    public int ColumnCount = 7;
    public float Gap = 16f;
    public float ContainerScale = 1.5f;
    public float ContainerRotationDeg = -15f;

    [Header("Motion")]
    public float MaxMoveAmount = 300f;
    public float BaseFollowDuration = 0.8f;
    public float[] InertiaFactors = { 0.6f, 0.4f, 0.3f, 0.2f };

    [Header("Visual")]
    public Color GradientColor = Color.black;
    public Color TileColor = new Color(0.067f, 0.067f, 0.067f, 1f);
    public Color TextColor = Color.white;
    public int FontSize = 28;
}
