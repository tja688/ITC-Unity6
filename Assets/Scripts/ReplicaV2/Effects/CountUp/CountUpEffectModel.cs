using System;

[Serializable]
public sealed class CountUpEffectModel
{
    public float FromValue = 0f;
    public float InitialTarget = 12840f;
    public float Duration = 2f;
    public float Delay = 0.25f;
    public float CycleInterval = 2.4f;
    public int Decimals = 0;
    public string Separator = ",";
    public bool CountDown = false;

    public CountUpEffectModel Clone()
    {
        return new CountUpEffectModel
        {
            FromValue = FromValue,
            InitialTarget = InitialTarget,
            Duration = Duration,
            Delay = Delay,
            CycleInterval = CycleInterval,
            Decimals = Decimals,
            Separator = Separator,
            CountDown = CountDown
        };
    }

    public static CountUpEffectModel CreateDefault()
    {
        return new CountUpEffectModel();
    }
}
