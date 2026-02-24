using System;
using UnityEngine;

[Serializable]
public sealed class DecayEffectModel
{
    public string Title = "NEXT GEN";
    public string Subtitle = "Pointer-driven distortion";
    public string Marker = "DECAY";
    public Sprite PhotoSprite;

    public DecayEffectModel Clone()
    {
        return new DecayEffectModel
        {
            Title = Title,
            Subtitle = Subtitle,
            Marker = Marker,
            PhotoSprite = PhotoSprite
        };
    }

    public static DecayEffectModel CreateDefault()
    {
        return new DecayEffectModel();
    }
}
