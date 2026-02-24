using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Card Swap Effect Config", fileName = "CardSwapEffectConfig")]
public sealed class CardSwapEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 SurfaceSize = new Vector2(1320f, 700f);
    public Vector2 DeckSize = new Vector2(840f, 580f);
    public Vector2 CardSize = new Vector2(480f, 340f);
    public float CardDistance = 72f;
    public float VerticalDistance = 66f;

    [Header("Timing")]
    public float SwapDelay = 4.2f;
    public bool PauseOnHover = true;

    [Header("Swap Animation")]
    public float OutDuration = 0.85f;
    public float OutDistanceY = 500f;
    public float LayoutDuration = 1.45f;
    public float LayoutDelayStep = 0.12f;

    [Header("Visual")]
    public Color BackdropColor = new Color(0.07f, 0.09f, 0.14f, 0.86f);
    public Color HintColor = new Color(0.93f, 0.95f, 1f, 0.95f);
    public Color SurfaceColor = new Color(0.10f, 0.13f, 0.21f, 0.82f);
    public Color OutlineColor = new Color(1f, 1f, 1f, 0.6f);
    public Vector2 OutlineDistance = new Vector2(1.8f, -1.8f);
    public Color SurfaceShadowColor = new Color(0f, 0f, 0f, 0.35f);
    public Vector2 SurfaceShadowDistance = new Vector2(0f, -9f);

    [Header("Animation")]
    public float EnterOffset = 220f;
    public float ExitOffset = 220f;
}

