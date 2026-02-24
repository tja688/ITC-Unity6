using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/TargetCursor Effect Config", fileName = "TargetCursorEffectConfig")]
public sealed class TargetCursorEffectConfig : ScriptableObject
{
    [Min(0.1f)] public float SpinDuration = 2f;
    [Min(0.01f)] public float HoverDuration = 0.2f;
    public bool HideDefaultCursor = true;
    public bool ParallaxOn = true;
    public float BorderWidth = 3f;
    public float CornerSize = 12f;
    public Color CursorColor = Color.white;
    public float DotSize = 4f;


    public ReplicaEase EnterEase = ReplicaEase.OutCubic;
    public ReplicaEase ExitEase = ReplicaEase.InCubic;
    public float TransitionOffset = 50f;
}
