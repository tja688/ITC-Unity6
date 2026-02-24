using System;

[Serializable]
public sealed class SpotlightCardEffectModel
{
    public string Hint = "SpotlightCard  |  Hover card to move light";
    public string Title = "Interactive Spotlight Surface";
    public string Body = "Hover to reveal follow-light glow.\nThe spotlight tracks pointer position in real time.";

    public SpotlightCardEffectModel Clone()
    {
        return new SpotlightCardEffectModel
        {
            Hint = Hint,
            Title = Title,
            Body = Body
        };
    }

    public static SpotlightCardEffectModel CreateDefault()
    {
        return new SpotlightCardEffectModel();
    }
}

