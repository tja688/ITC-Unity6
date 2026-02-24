using System;

[Serializable]
public sealed class NoiseEffectModel
{
    public bool Paused = false;
    [UnityEngine.Range(0f, 2f)] public float AlphaMultiplier = 1f;

    public NoiseEffectModel Clone()
    {
        return new NoiseEffectModel
        {
            Paused = Paused,
            AlphaMultiplier = AlphaMultiplier
        };
    }

    public static NoiseEffectModel CreateDefault()
    {
        return new NoiseEffectModel();
    }
}
