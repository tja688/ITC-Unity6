using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Fade Content Effect Config", fileName = "FadeContentEffectConfig")]
public sealed class FadeContentEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 CardSize = new Vector2(960f, 320f);
    public float EnterOffset = 140f;
    public float ExitOffset = 140f;

    [Header("Visual")]
    public Color CardColor = new Color(0.05f, 0.06f, 0.10f, 0.92f);
    public Color TitleColor = new Color(0.93f, 0.96f, 1f, 1f);
    public Color BodyColor = new Color(0.78f, 0.84f, 0.95f, 0.95f);
    public int TitleFontSize = 44;
    public int BodyFontSize = 22;

    [Header("Fade")]
    [Range(0f, 1f)] public float InitialOpacity = 0f;
    public bool SimulateBlurWithScale = false;
    public float BlurStartScale = 1.02f;

    [Header("Auto Disappear")]
    public float DisappearAfter = 0f;
    public float DisappearDuration = 0.5f;
    public ReplicaEase DisappearEase = ReplicaEase.InCubic;
}
