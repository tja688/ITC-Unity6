using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Magnet Effect Config", fileName = "MagnetEffectConfig")]
public sealed class MagnetEffectConfig : ScriptableObject
{
    [Header("Assets")]
    public GameObject Prefab;

    [Header("Interaction")]
    public float Padding = 100f;
    public float MagnetStrength = 2f;
    public float MaxOffset = 90f;
    public bool Disabled = false;

    [Header("Motion")]
    public float ActiveDuration = 0.3f;
    public float InactiveDuration = 0.5f;
    public ReplicaEase ActiveEase = ReplicaEase.OutQuad;
    public ReplicaEase InactiveEase = ReplicaEase.InOutQuad;

    [Header("Layout")]
    public Vector2 AreaSize = new Vector2(560f, 300f);
    public Vector2 ButtonSize = new Vector2(320f, 96f);

    [Header("Visual")]
    public Color BackgroundColor = new Color(0.10f, 0.14f, 0.24f, 0.96f);
    public Color BackgroundOutlineColor = new Color(1f, 1f, 1f, 0.16f);
    public Color ButtonColor = new Color(0.95f, 0.97f, 1f, 0.96f);
    public Color ButtonTextColor = new Color(0.08f, 0.12f, 0.22f, 1f);
    public Color HintTextColor = new Color(0.80f, 0.88f, 1f, 0.92f);
}
