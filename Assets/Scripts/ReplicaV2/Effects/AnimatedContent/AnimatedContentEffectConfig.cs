using UnityEngine;

public enum AnimatedContentDirection
{
    Vertical = 0,
    Horizontal = 1
}

[CreateAssetMenu(menuName = "ReplicaV2/AnimatedContent Effect Config", fileName = "AnimatedContentEffectConfig")]
public sealed class AnimatedContentEffectConfig : ScriptableObject
{
    [Header("Assets")]
    public GameObject Prefab;

    [Header("Motion")]
    public float Distance = 100f;
    public AnimatedContentDirection Direction = AnimatedContentDirection.Vertical;
    public bool Reverse = false;
    public float Duration = 0.8f;
    public ReplicaEase Ease = ReplicaEase.OutCubic;
    public float Delay = 0f;

    [Header("Opacity & Scale")]
    [Range(0f, 1f)] public float InitialOpacity = 0f;
    public bool AnimateOpacity = true;
    public float InitialScale = 1f;

    [Header("Trigger")]
    [Range(0f, 1f)] public float Threshold = 0.1f;
    public bool PlayOnce = true;

    [Header("Disappear")]
    public float DisappearAfter = 0f;
    public float DisappearDuration = 0.5f;
    public ReplicaEase DisappearEase = ReplicaEase.InCubic;
    public float DisappearScale = 0.8f;

    [Header("Transitions")]
    public float EnterOffset = 220f;
    public float ExitOffset = 220f;

    [Header("Visual")]
    public Vector2 ContainerSize = new Vector2(720f, 160f);
    public Color BackgroundColor = new Color(0f, 0f, 0f, 0.70f);
    public Color TextColor = new Color(0.94f, 0.96f, 1f, 0.92f);
    public int FontSize = 26;
}
