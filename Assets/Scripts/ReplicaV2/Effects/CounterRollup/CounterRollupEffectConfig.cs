using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Counter Rollup Effect Config", fileName = "CounterRollupEffectConfig")]
public sealed class CounterRollupEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 PanelSize = new Vector2(980f, 380f);
    public Vector2 PanelPosition = new Vector2(0f, -20f);
    public Vector2 CounterSize = new Vector2(760f, 180f);
    public Vector2 CounterPosition = new Vector2(0f, 0f);
    public float ColumnSpacing = 12f;
    public Vector2 DigitSize = new Vector2(124f, 132f);
    public Vector2 NumberSize = new Vector2(120f, 124f);
    public int NumberFontSize = 112;
    public int[] Places = { 10000, 1000, 100, 10, 1 };

    [Header("Motion")]
    public float TweenDuration = 1.05f;
    public ReplicaEase TweenEase = ReplicaEase.OutCubic;

    [Header("Visual")]
    public Color BackdropColor = new Color(0.05f, 0.07f, 0.12f, 0.88f);
    public Color HintColor = new Color(0.93f, 0.96f, 1f, 0.95f);
    public Color PanelColor = new Color(0.10f, 0.13f, 0.22f, 0.94f);
    public Color DigitBackground = new Color(0.16f, 0.20f, 0.30f, 0.92f);
    public Color FadeOverlay = new Color(0.04f, 0.05f, 0.10f, 0.72f);

    [Header("Animation")]
    public float EnterOffset = 160f;
    public float ExitOffset = 160f;
}
