using System;

[Serializable]
public sealed class BounceCardsEffectModel
{
    public string Hint = "BounceCards  |  Hover a card to push siblings";

    public BounceCardsEffectModel Clone()
    {
        return new BounceCardsEffectModel
        {
            Hint = Hint
        };
    }

    public static BounceCardsEffectModel CreateDefault()
    {
        return new BounceCardsEffectModel();
    }
}

