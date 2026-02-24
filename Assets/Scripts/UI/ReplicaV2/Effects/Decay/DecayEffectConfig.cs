using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ReplicaV2/Decay Effect Config", fileName = "DecayEffectConfig")]
public sealed class DecayEffectConfig : ScriptableObject
{
    [Header("Assets")]
    public GameObject Prefab;

    [Header("Layout")]
    public Vector2 CardSize = new Vector2(560f, 700f);

    [Header("Motion")]
    public float MoveBound = 50f;
    public float PositionLerp = 0.1f;
    public float RotationLerp = 0.1f;
    public float DistortionLerp = 0.06f;
    public float DistortionDistanceMax = 200f;

    [Header("Noise")]
    public int StripCount = 28;
    public float StripAmplitude = 18f;
    public float StripAmplitudeStep = 0.4f;
    public float StripWaveSpeed = 7.2f;
    public float StripAlphaSpeed = 11.5f;

    [Header("Visual")]
    public Color BasePhotoColor = new Color(0.36f, 0.39f, 0.46f, 1f);
    public Color ActivePhotoColor = new Color(0.70f, 0.72f, 0.77f, 1f);
    public Color GradientAColor = new Color(0.16f, 0.25f, 0.52f, 0.45f);
    public Color GradientBColor = new Color(0.57f, 0.25f, 0.22f, 0.30f);
    public Color TitleColor = new Color(0.98f, 0.98f, 1f, 0.95f);
    public Color SubtitleColor = new Color(0.84f, 0.90f, 1f, 0.86f);
    public Image.Type SpriteImageType = Image.Type.Simple;

    [Header("Animation")]
    public float EnterOffset = 220f;
    public float ExitOffset = 220f;
}
