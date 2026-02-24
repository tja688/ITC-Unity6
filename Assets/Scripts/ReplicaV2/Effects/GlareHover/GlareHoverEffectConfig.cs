using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/GlareHover Effect Config", fileName = "GlareHoverEffectConfig")]
public sealed class GlareHoverEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 ContainerSize = new Vector2(500f, 500f);
    public float BorderThickness = 1f;
    public float EnterOffset = 220f;
    public float ExitOffset = 220f;

    [Header("Motion")]
    public float TransitionDuration = 0.65f;
    public ReplicaEase TransitionEase = ReplicaEase.OutCubic;
    public bool PlayOnce = false;

    [Header("Glare")]
    public Color GlareColor = Color.white;
    [Range(0f, 1f)] public float GlareOpacity = 0.5f;
    public float GlareAngleDeg = -45f;
    public float GlareSizePercent = 250f;

    [Header("Visual")]
    public Color BackgroundColor = Color.black;
    public Color BorderColor = new Color(0.2f, 0.2f, 0.2f, 1f);
    public Color LabelColor = new Color(0.94f, 0.96f, 1f, 0.92f);
    public int LabelFontSize = 28;
}
