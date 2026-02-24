using System;

[Serializable]
public sealed class CurvedLoopEffectModel
{
    public string MarqueeText = "CURVED LOOP DRAG INTERACTION";
    public float Speed = 160f;
    public float CurveAmount = 220f;
    public bool Interactive = true;

    public CurvedLoopEffectModel Clone()
    {
        return new CurvedLoopEffectModel
        {
            MarqueeText = MarqueeText,
            Speed = Speed,
            CurveAmount = CurveAmount,
            Interactive = Interactive
        };
    }

    public static CurvedLoopEffectModel CreateDefault()
    {
        return new CurvedLoopEffectModel();
    }
}
