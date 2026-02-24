using System;

[Serializable]
public sealed class AnimatedContentEffectModel
{
    public string Label = "AnimatedContent";

    public AnimatedContentEffectModel Clone()
    {
        return new AnimatedContentEffectModel
        {
            Label = Label
        };
    }

    public static AnimatedContentEffectModel CreateDefault()
    {
        return new AnimatedContentEffectModel();
    }
}
