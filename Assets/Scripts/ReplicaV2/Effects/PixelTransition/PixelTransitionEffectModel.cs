using System;
using UnityEngine;

[Serializable]
public sealed class PixelTransitionEffectModel
{
    public string DefaultLabel = "DEFAULT";
    public string ActiveLabel = "ACTIVE";
    public Sprite DefaultSprite;
    public Sprite ActiveSprite;

    public PixelTransitionEffectModel Clone()
    {
        return new PixelTransitionEffectModel
        {
            DefaultLabel = DefaultLabel,
            ActiveLabel = ActiveLabel,
            DefaultSprite = DefaultSprite,
            ActiveSprite = ActiveSprite
        };
    }

    public static PixelTransitionEffectModel CreateDefault()
    {
        return new PixelTransitionEffectModel();
    }
}
