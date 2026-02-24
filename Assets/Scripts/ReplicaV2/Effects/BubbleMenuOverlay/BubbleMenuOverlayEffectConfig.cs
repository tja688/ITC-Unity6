using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Bubble Menu Overlay Effect Config", fileName = "BubbleMenuOverlayEffectConfig")]
public sealed class BubbleMenuOverlayEffectConfig : ScriptableObject
{
    [Header("Timing")]
    public float OpenDuration = 0.5f;
    public float StaggerDelay = 0.12f;
    public float OverlayFadeInDuration = 0.28f;
    public float OverlayFadeOutDuration = 0.18f;
    public float IconMorphDuration = 0.22f;

    [Header("Layout")]
    public float TopBarHeight = 120f;
    public Vector2 LogoSize = new Vector2(220f, 64f);
    public Vector2 ToggleSize = new Vector2(64f, 64f);
    public Vector2 TogglePadding = new Vector2(40f, 56f);
    public Vector2 PillSize = new Vector2(320f, 132f);
    public Vector2 PillLabelClosedOffset = new Vector2(0f, 24f);

    [Header("Pill Hover")]
    public float HoverScale = 1.06f;
    public float HoverDuration = 0.18f;
    public float DownScale = 0.94f;
    public float DownDuration = 0.12f;
    public float UpDuration = 0.14f;

    [Header("Visual")]
    public Color BackdropColor = new Color(0.05f, 0.08f, 0.12f, 0.86f);
    public Color OverlayColor = new Color(0f, 0f, 0f, 0.08f);
    public Color TopBarBubbleColor = Color.white;
    public Color ToggleLineColor = new Color(0.08f, 0.10f, 0.12f, 1f);
    public Color PillBaseTextColor = new Color(0.07f, 0.08f, 0.11f, 1f);
    public Color PillHoverTextColor = Color.white;
}
