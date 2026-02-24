using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Bounce Cards Effect Config", fileName = "BounceCardsEffectConfig")]
public sealed class BounceCardsEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 StageSize = new Vector2(1450f, 760f);
    public Vector2 ContainerSize = new Vector2(1200f, 520f);
    public Vector2 CardSize = new Vector2(240f, 240f);

    [Header("Motion")]
    public float HoverPushOffset = 160f;
    public float EntryDelay = 0.42f;
    public float EntryStagger = 0.08f;
    public float EntryDuration = 0.7f;
    public float HoverDuration = 0.4f;
    public float HoverSiblingDelayStep = 0.03f;
    public float HoverLiftY = 18f;
    public float HoverOvershoot = 1.4f;

    [Header("Visual")]
    public Color BackdropColor = new Color(0.05f, 0.08f, 0.14f, 0.82f);
    public Color PlateAColor = new Color(0.11f, 0.21f, 0.33f, 0.45f);
    public Color PlateBColor = new Color(0.22f, 0.16f, 0.30f, 0.40f);
    public Color HintColor = new Color(0.90f, 0.94f, 1f, 0.95f);
    public Color OutlineColor = Color.white;
    public Vector2 OutlineDistance = new Vector2(4f, -4f);
    public Color ShadowColor = new Color(0f, 0f, 0f, 0.3f);
    public Vector2 ShadowDistance = new Vector2(0f, -10f);

    [Header("Animation")]
    public float EnterOffset = 220f;
    public float ExitOffset = 220f;
}

