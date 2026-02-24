using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Carousel Swipe Cards Effect Config", fileName = "CarouselSwipeCardsEffectConfig")]
public sealed class CarouselSwipeCardsEffectConfig : ScriptableObject
{
    [Header("Behavior")]
    public float AutoplayDelay = 3.1f;
    public bool Autoplay = true;
    public bool PauseOnHover = true;
    public bool Loop = true;

    [Header("Drag")]
    public float DragMoveScale = 0.25f;
    public float DragClamp = 80f;
    public float SwipeThreshold = 80f;

    [Header("Animation")]
    public float StageReturnDuration = 0.18f;
    public float CardMoveDuration = 0.45f;
    public float CardFadeDuration = 0.35f;

    [Header("Layout")]
    public Vector2 FrameSize = new Vector2(1120f, 680f);
    public Vector2 StageSize = new Vector2(980f, 470f);
    public Vector2 StageOffset = new Vector2(0f, 22f);
    public Vector2 CardSize = new Vector2(430f, 470f);
    public float CardStepX = 320f;
    public float CardStepY = 18f;
    public float CardRotateStep = 9f;

    [Header("Visual")]
    public Color BackdropColor = new Color(0.06f, 0.08f, 0.14f, 0.9f);
    public Color FrameColor = new Color(0.08f, 0.10f, 0.17f, 0.94f);
    public Color FrameOutlineColor = new Color(1f, 1f, 1f, 0.2f);
}
