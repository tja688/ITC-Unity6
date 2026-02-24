using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Rotating Text Effect Config", fileName = "RotatingTextEffectConfig")]
public sealed class RotatingTextEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 ViewportSize = new Vector2(900f, 180f);

    [Header("Text")]
    public int FontSize = 64;
    public FontStyle FontStyle = FontStyle.Bold;
    public TextAnchor Alignment = TextAnchor.MiddleCenter;
    public Color TextColor = Color.white;

    [Header("Element Motion")]
    public float EnterOffsetY = 120f;
    public float ExitOffsetY = 150f;
    public float EnterDuration = 0.35f;
    public float ExitDuration = 0.30f;
    public ReplicaEase EnterEase = ReplicaEase.OutCubic;
    public ReplicaEase ExitEase = ReplicaEase.InCubic;

    [Header("Transition")]
    public float EnterOffset = 160f;
    public float ExitOffset = 160f;
}
