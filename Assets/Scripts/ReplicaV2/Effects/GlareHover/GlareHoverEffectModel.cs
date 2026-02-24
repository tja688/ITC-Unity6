using System;

[Serializable]
public sealed class GlareHoverEffectModel
{
    public string Label = "GlareHover";

    public GlareHoverEffectModel Clone()
    {
        return new GlareHoverEffectModel
        {
            Label = Label
        };
    }

    public static GlareHoverEffectModel CreateDefault()
    {
        return new GlareHoverEffectModel();
    }
}
