using System;

[Serializable]
public sealed class TiltedCardEffectModel
{
    public string Hint = "TiltedCard  |  Move cursor around the card";
    public string Badge = "HOVER TO TILT";

    public TiltedCardEffectModel Clone()
    {
        return new TiltedCardEffectModel
        {
            Hint = Hint,
            Badge = Badge
        };
    }

    public static TiltedCardEffectModel CreateDefault()
    {
        return new TiltedCardEffectModel();
    }
}

