using System;

[Serializable]
public sealed class CardSwapEffectModel
{
    public string Hint = "CardSwap  |  Auto swaps top card to back";
    public string Caption = "Click the front card to swap now";

    public CardSwapEffectModel Clone()
    {
        return new CardSwapEffectModel
        {
            Hint = Hint,
            Caption = Caption
        };
    }

    public static CardSwapEffectModel CreateDefault()
    {
        return new CardSwapEffectModel();
    }
}

