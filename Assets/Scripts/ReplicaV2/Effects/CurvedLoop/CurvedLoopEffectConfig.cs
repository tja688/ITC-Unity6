using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Curved Loop Effect Config", fileName = "CurvedLoopEffectConfig")]
public sealed class CurvedLoopEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 ViewportSize = new Vector2(1460f, 420f);
    public Vector2 ViewportPosition = new Vector2(0f, -16f);
    public Vector2 TrackSize = new Vector2(1580f, 360f);
    public float CurveWidth = 1520f;
    public float BaselineY = -20f;
    public float DefaultSpacing = 44f;
    public int RepeatCount = 10;

    [Header("Interaction")]
    public float DragScale = 1.2f;
    public float DragDirectionThreshold = 0.1f;

    [Header("Text")]
    public int TitleFontSize = 30;
    public int LetterFontSize = 56;
    public Vector2 LetterSize = new Vector2(54f, 64f);

    [Header("Visual")]
    public Color RootBackground = new Color(0.04f, 0.06f, 0.12f, 0.96f);
    public Color TitleColor = new Color(0.91f, 0.95f, 1f, 1f);
    public Color ViewportColor = new Color(0.08f, 0.11f, 0.20f, 0.86f);

    [Header("Animation")]
    public float EnterOffset = 140f;
    public float ExitOffset = 140f;
}
