using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Orbit Images Effect Config", fileName = "OrbitImagesEffectConfig")]
public sealed class OrbitImagesEffectConfig : ScriptableObject
{
    [Header("Layout")]
    public Vector2 StageSize = new Vector2(980f, 980f);
    public float RadiusX = 380f;
    public float RadiusY = 140f;
    public float OrbitRotationZ = -8f;
    public Vector2 CenterPanelSize = new Vector2(520f, 240f);

    [Header("Items")]
    public float ItemSize = 86f;
    public Color ItemFallbackColor = new Color(0.26f, 0.34f, 0.55f, 1f);

    [Header("Motion")]
    public float DurationPerLoop = 40f;
    public bool Reverse = false;
    public bool Fill = true;
    public bool KeepUpright = true;

    [Header("Visual")]
    public Color BackgroundColor = new Color(0.03f, 0.04f, 0.07f, 0.95f);
    public Color CenterPanelColor = new Color(0.10f, 0.12f, 0.18f, 0.92f);
    public Color CenterTitleColor = new Color(0.93f, 0.96f, 1f, 1f);
    public int CenterTitleFontSize = 40;

    [Header("Animation")]
    public float EnterOffset = 140f;
    public float ExitOffset = 140f;
}
