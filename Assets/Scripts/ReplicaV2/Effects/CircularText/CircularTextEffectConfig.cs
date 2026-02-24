using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Circular Text Effect Config", fileName = "CircularTextEffectConfig")]
public sealed class CircularTextEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 AmbientSize = new Vector2(1000f, 1000f);
    public Vector2 AmbientPosition = new Vector2(0f, 10f);
    public Vector2 RingHolderSize = new Vector2(460f, 460f);
    public Vector2 RingHolderPosition = new Vector2(0f, -22f);
    public float RingRadius = 170f;
    public Vector2 LetterSize = new Vector2(40f, 40f);
    public int LetterFontSize = 28;

    [Header("Interaction")]
    public float HoverSpeedMultiplier = 4f;
    public float HoverScale = 0.92f;
    public float Response = 9f;

    [Header("Visual")]
    public Color RootBackground = new Color(0.03f, 0.05f, 0.10f, 0.95f);
    public Color AmbientColor = new Color(0.30f, 0.19f, 0.63f, 0.30f);
    public Color TitleColor = new Color(0.92f, 0.95f, 1f, 1f);
    public Color RingBackColor = new Color(0.10f, 0.14f, 0.24f, 0.58f);
    public Color CoreColor = new Color(0.11f, 0.13f, 0.22f, 0.94f);
    public Color CoreLabelColor = new Color(0.85f, 0.88f, 1f, 1f);
    public Color LetterColor = new Color(1f, 1f, 1f, 0.96f);

    [Header("Animation")]
    public float EnterOffset = 140f;
    public float ExitOffset = 140f;
}
