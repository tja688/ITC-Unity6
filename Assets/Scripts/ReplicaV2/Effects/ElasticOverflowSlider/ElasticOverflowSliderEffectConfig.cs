using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Elastic Overflow Slider Effect Config", fileName = "ElasticOverflowSliderEffectConfig")]
public sealed class ElasticOverflowSliderEffectConfig : ScriptableObject
{
    [Header("Range")]
    public float MinValue = 0f;
    public float MaxValue = 100f;
    public bool Stepped = false;
    public float StepSize = 1f;

    [Header("Overflow")]
    public float MaxOverflow = 50f;
    public float OverflowSpring = 180f;
    public float OverflowDamping = 12f;

    [Header("Hover")]
    public float HoverScaleIdle = 1f;
    public float HoverScaleActive = 1.2f;
    public float HoverScaleSpeed = 10f;

    [Header("Track")]
    public float TrackHeightIdle = 6f;
    public float TrackHeightHover = 14f;
    public float TrackHeightSpeed = 10f;

    [Header("Icons")]
    public float IconPushSpeed = 15f;
    public float IconBaseOffset = 28f;

    [Header("Layout")]
    public Vector2 FrameSize = new Vector2(900f, 380f);
    public Vector2 RowSize = new Vector2(760f, 120f);

    [Header("Visual")]
    public Color BackdropColor = new Color(0.06f, 0.08f, 0.14f, 0.90f);
    public Color FrameColor = new Color(0.09f, 0.12f, 0.20f, 0.92f);
    public Color HintColor = new Color(0.93f, 0.96f, 1f, 0.95f);
    public Color IconColor = new Color(0.76f, 0.82f, 0.92f, 0.92f);
    public Color TrackColor = new Color(0.58f, 0.62f, 0.72f, 0.28f);
    public Color FillColor = new Color(0.74f, 0.78f, 0.88f, 0.96f);
    public Color KnobColor = new Color(0.93f, 0.95f, 1f, 1f);
    public Color ValueColor = new Color(0.84f, 0.90f, 1f, 0.92f);

    [Header("Animation")]
    public float EnterOffset = 180f;
    public float ExitOffset = 180f;
}
