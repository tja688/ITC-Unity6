using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Pixel Transition Effect Config", fileName = "PixelTransitionEffectConfig")]
public sealed class PixelTransitionEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 CardSize = new Vector2(520f, 360f);

    [Header("Pixels")]
    [Range(2, 20)] public int GridSize = 7;
    public Color PixelColor = Color.white;
    public float AnimationStepDuration = 0.3f;
    public bool Once = false;

    [Header("Visual")]
    public Color CardColor = new Color(0.13f, 0.13f, 0.13f, 1f);
    public Color OutlineColor = new Color(1f, 1f, 1f, 1f);
    public Vector2 OutlineDistance = new Vector2(2f, -2f);
    public Color DefaultLabelColor = new Color(0.93f, 0.96f, 1f, 1f);
    public Color ActiveLabelColor = new Color(0.93f, 0.96f, 1f, 1f);
    public int LabelFontSize = 28;

    [Header("Animation")]
    public float EnterOffset = 120f;
    public float ExitOffset = 120f;
}
