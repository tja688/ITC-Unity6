using System;

[Serializable]
public sealed class StarBorderEffectModel
{
    public string Label = "StarBorder";

    public StarBorderEffectModel Clone()
    {
        return new StarBorderEffectModel
        {
            Label = Label
        };
    }

    public static StarBorderEffectModel CreateDefault()
    {
        return new StarBorderEffectModel();
    }
}
