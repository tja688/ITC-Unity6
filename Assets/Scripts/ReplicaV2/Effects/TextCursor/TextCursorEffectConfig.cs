using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Text Cursor Effect Config", fileName = "TextCursorEffectConfig")]
public sealed class TextCursorEffectConfig : ScriptableObject
{
    [Header("Text")]
    public int FontSize = 30;
    public FontStyle FontStyle = FontStyle.Normal;
    public TextAnchor Alignment = TextAnchor.MiddleCenter;
    public Color TextColor = Color.white;

    [Header("Float")]
    public float FloatPeriod = 2f;
    public float RandomOffsetRange = 5f;
    public float RandomRotateRange = 5f;

    [Header("Transition")]
    public ReplicaEase ExitEase = ReplicaEase.OutCubic;
    public float EnterOffset = 160f;
    public float ExitOffset = 160f;
}
