using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ReplicaV2/Stack Effect Config", fileName = "StackEffectConfig")]
public sealed class StackEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 StackSize = new Vector2(760f, 520f);
    public Vector2 CardSize = new Vector2(620f, 390f);
    public float RotationStep = 4f;
    public float ScaleStep = 0.06f;

    [Header("Interaction")]
    public bool AllowDrag = true;
    public float DragSensitivity = 200f;
    public float MaxTilt = 22f;
    public bool SendToBackOnClick = true;

    [Header("Animation")]
    public float LayoutDuration = 0.32f;
    public float ReturnDuration = 0.24f;
    public float EnterOffset = 260f;
    public float ExitOffset = 260f;

    [Header("Visual")]
    public Color FallbackCardColor = new Color(0.33f, 0.46f, 0.76f, 1f);
    public Color LabelColor = new Color(1f, 1f, 1f, 0.96f);
    public Color MetaColor = new Color(1f, 1f, 1f, 0.82f);
    public Color ShadowColor = new Color(0f, 0f, 0f, 0.26f);
    public Image.Type SpriteImageType = Image.Type.Simple;
}
