using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Glitch Text Effect Config", fileName = "GlitchTextEffectConfig")]
public sealed class GlitchTextEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 BackdropSize = new Vector2(1180f, 460f);
    public Vector2 BackdropPosition = new Vector2(0f, -10f);
    public Vector2 ContainerSize = new Vector2(900f, 220f);

    [Header("Text")]
    public int TitleFontSize = 30;
    public int MainFontSize = 132;

    [Header("Glitch Motion")]
    public float StepBase = 0.08f;
    public float StepMin = 0.03f;
    public float AfterBaseX = 10f;
    public float BeforeBaseX = -10f;
    public Vector2 SliceOffsetXRange = new Vector2(-6f, 6f);
    public Vector2 SliceOffsetYRange = new Vector2(-4f, 4f);
    public Vector2 MainJitterRange = new Vector2(-2f, 2f);
    public Vector2 BandHeightRange = new Vector2(24f, 92f);
    public float BandEdgePadding = 16f;
    public Vector2 SliceAlphaRange = new Vector2(0.70f, 1f);
    public Vector3 PunchScale = new Vector3(0.015f, 0.015f, 0f);
    public float PunchDuration = 0.08f;
    public int PunchVibrato = 2;
    public float PunchElasticity = 0.4f;

    [Header("Visual")]
    public Color RootBackground = new Color(0.02f, 0.02f, 0.07f, 0.98f);
    public Color BackdropColor = new Color(0.04f, 0.03f, 0.10f, 0.90f);
    public Color HintColor = new Color(0.95f, 0.97f, 1f, 1f);
    public Color TextColor = Color.white;
    public Color AfterShadowColor = Color.red;
    public Vector2 AfterShadowOffset = new Vector2(-5f, 0f);
    public Color BeforeShadowColor = Color.cyan;
    public Vector2 BeforeShadowOffset = new Vector2(5f, 0f);

    [Header("Animation")]
    public float EnterOffset = 160f;
    public float ExitOffset = 160f;
}
