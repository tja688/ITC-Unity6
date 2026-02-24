using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Stepper Progress Slide Effect Config", fileName = "StepperProgressSlideEffectConfig")]
public sealed class StepperProgressSlideEffectConfig : ScriptableObject
{
    [Header("Steps")]
    public int StepCount = 4;

    [Header("Layout")]
    public Vector2 CardSize = new Vector2(860f, 620f);
    public Vector2 ViewportFrameOffsetMin = new Vector2(42f, -460f);
    public Vector2 ViewportFrameOffsetMax = new Vector2(-42f, -156f);
    public float ViewportHeightPadding = 12f;

    [Header("Motion")]
    public float SlideOutDuration = 0.36f;
    public float SlideInDuration = 0.38f;
    public float HeightDuration = 0.35f;
    public float ConnectorFillDuration = 0.30f;

    [Header("Visual")]
    public Color RootBackground = new Color(0.05f, 0.07f, 0.12f, 0.90f);
    public Color AmbientA = new Color(0.22f, 0.34f, 0.56f, 0.26f);
    public Color CardColor = new Color(0.95f, 0.97f, 1f, 0.98f);
    public Color TitleColor = new Color(0.08f, 0.11f, 0.18f, 1f);
    public Color LabelColor = new Color(0.08f, 0.11f, 0.18f, 0.95f);
    public Color BodyColor = new Color(0.22f, 0.25f, 0.35f, 1f);

    public Color NodeInactive = new Color(0.12f, 0.14f, 0.19f, 1f);
    public Color NodeActive = new Color(0.32f, 0.15f, 1f, 1f);
    public Color NodeComplete = new Color(0.32f, 0.15f, 1f, 1f);
    public Color NodeTextInactive = new Color(0.63f, 0.66f, 0.74f, 1f);

    public Color ConnectorTrack = new Color(0.82f, 0.84f, 0.90f, 0.9f);

    public Color BackButtonBg = new Color(0.91f, 0.93f, 0.97f, 1f);
    public Color BackButtonText = new Color(0.34f, 0.37f, 0.45f, 1f);

    public Color NextButtonBg = new Color(0.32f, 0.15f, 1f, 1f);
    public Color NextButtonText = Color.white;

    public Color CompletionColor = new Color(0.18f, 0.52f, 0.34f, 0.92f);

    [Header("Animation")]
    public float EnterOffset = 180f;
    public float ExitOffset = 180f;
}
