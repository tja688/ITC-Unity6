using System;

[Serializable]
public sealed class CircularTextEffectModel
{
    public string Text = "CIRCULAR MOTION REPLICA";
    public float SpinDuration = 20f;

    public CircularTextEffectModel Clone()
    {
        return new CircularTextEffectModel
        {
            Text = Text,
            SpinDuration = SpinDuration
        };
    }

    public static CircularTextEffectModel CreateDefault()
    {
        return new CircularTextEffectModel();
    }
}
