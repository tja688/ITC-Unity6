using UnityEngine;

[CreateAssetMenu(menuName = "ReplicaV2/Noise Effect Config", fileName = "NoiseEffectConfig")]
public sealed class NoiseEffectConfig : ScriptableObject
{
    [Header("Texture")]
    [Range(64, 2048)] public int TextureSize = 1024;
    [Range(1, 30)] public int RefreshFrameInterval = 2;
    [Range(0, 255)] public int NoiseAlphaByte = 15;
    public bool Pixelated = true;

    [Header("Tiling")]
    public Vector2 PatternScale = Vector2.one;

    [Header("Visual")]
    public Color Tint = Color.white;

    [Header("Animation")]
    public float EnterOffset = 0f;
    public float ExitOffset = 0f;
}
