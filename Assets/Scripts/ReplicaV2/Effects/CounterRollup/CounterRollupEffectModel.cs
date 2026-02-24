using System;

[Serializable]
public sealed class CounterRollupEffectModel
{
    public int StartValue = 17284;
    public int MinTarget = 800;
    public int MaxTarget = 99999;
    public float ChangeInterval = 2.3f;

    public CounterRollupEffectModel Clone()
    {
        return new CounterRollupEffectModel
        {
            StartValue = StartValue,
            MinTarget = MinTarget,
            MaxTarget = MaxTarget,
            ChangeInterval = ChangeInterval
        };
    }

    public static CounterRollupEffectModel CreateDefault()
    {
        return new CounterRollupEffectModel();
    }
}
