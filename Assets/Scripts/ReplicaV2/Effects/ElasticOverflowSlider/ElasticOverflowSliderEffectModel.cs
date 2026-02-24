using System;

[Serializable]
public sealed class ElasticOverflowSliderEffectModel
{
    public string Hint = "ElasticSlider  |  Drag and over-pull both sides";
    public float Value = 50f;

    public ElasticOverflowSliderEffectModel Clone()
    {
        return new ElasticOverflowSliderEffectModel
        {
            Hint = Hint,
            Value = Value
        };
    }

    public static ElasticOverflowSliderEffectModel CreateDefault()
    {
        return new ElasticOverflowSliderEffectModel();
    }
}
