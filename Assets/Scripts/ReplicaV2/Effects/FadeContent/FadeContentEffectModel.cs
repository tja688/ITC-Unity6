using System;

[Serializable]
public sealed class FadeContentEffectModel
{
    public string Title = "Fade Content";
    public string Body = "Basic but essential transition wrapper.";

    public FadeContentEffectModel Clone()
    {
        return new FadeContentEffectModel
        {
            Title = Title,
            Body = Body
        };
    }

    public static FadeContentEffectModel CreateDefault()
    {
        return new FadeContentEffectModel();
    }
}
