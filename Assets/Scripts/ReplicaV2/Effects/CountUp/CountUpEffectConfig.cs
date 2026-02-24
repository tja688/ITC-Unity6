using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Count Up Effect Config", fileName = "CountUpEffectConfig")]
public sealed class CountUpEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 AmbientSize = new Vector2(1240f, 540f);
    public Vector2 AmbientPosition = new Vector2(-90f, 120f);
    public float AmbientRotationZ = 10f;
    public Vector2 PanelSize = new Vector2(980f, 460f);
    public Vector2 PanelPosition = new Vector2(0f, -20f);
    public Vector2 ValueWrapSize = new Vector2(860f, 190f);
    public Vector2 ValueWrapPosition = new Vector2(0f, 0f);

    [Header("Text")]
    public int TitleFontSize = 30;
    public int ValueFontSize = 128;
    public int HintFontSize = 24;

    [Header("Visual")]
    public Color RootBackground = new Color(0.04f, 0.06f, 0.11f, 0.95f);
    public Color AmbientColor = new Color(0.24f, 0.37f, 0.74f, 0.28f);
    public Color PanelColor = new Color(0.10f, 0.13f, 0.22f, 0.95f);
    public Color TitleColor = new Color(0.93f, 0.96f, 1f, 1f);
    public Color ValueGlowColor = new Color(0.24f, 0.33f, 0.74f, 0.35f);
    public Color HintColor = new Color(0.76f, 0.82f, 0.95f, 0.95f);

    [Header("Motion")]
    public ReplicaEase CountEase = ReplicaEase.OutCubic;
    public Vector3 PunchScale = new Vector3(0.06f, 0.06f, 0f);
    public float PunchDuration = 0.26f;
    public int PunchVibrato = 4;
    public float PunchElasticity = 0.5f;

    [Header("Animation")]
    public float EnterOffset = 160f;
    public float ExitOffset = 160f;
}
