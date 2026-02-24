using System;

[Serializable]
public sealed class TextCursorEffectModel
{
    public string Text = "⚛️";
    public float Spacing = 100f;
    public bool FollowMouseDirection = true;
    public bool RandomFloat = true;
    public float ExitDuration = 0.5f;
    public int RemovalIntervalMs = 30;
    public int MaxPoints = 5;

    public static TextCursorEffectModel CreateDefault()
    {
        return new TextCursorEffectModel
        {
            Text = "⚛️",
            Spacing = 100f,
            FollowMouseDirection = true,
            RandomFloat = true,
            ExitDuration = 0.5f,
            RemovalIntervalMs = 30,
            MaxPoints = 5
        };
    }

    public TextCursorEffectModel Clone()
    {
        return new TextCursorEffectModel
        {
            Text = Text,
            Spacing = Spacing,
            FollowMouseDirection = FollowMouseDirection,
            RandomFloat = RandomFloat,
            ExitDuration = ExitDuration,
            RemovalIntervalMs = RemovalIntervalMs,
            MaxPoints = MaxPoints
        };
    }
}
