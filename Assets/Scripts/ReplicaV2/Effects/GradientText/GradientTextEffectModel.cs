using System;
using UnityEngine;

[Serializable]
public enum GradientTextDirection
{
    Horizontal,
    Vertical,
    Diagonal
}

[Serializable]
public sealed class GradientTextEffectModel
{
    public string Text;
    public Color[] Colors;
    public float AnimationSpeed = 8f;
    public bool ShowBorder;
    public GradientTextDirection Direction = GradientTextDirection.Horizontal;
    public bool PauseOnHover;
    public bool Yoyo = true;

    public static GradientTextEffectModel CreateDefault()
    {
        return new GradientTextEffectModel
        {
            Text = "GradientText",
            Colors = new[]
            {
                new Color(0.321f, 0.153f, 1f, 1f),
                new Color(1f, 0.624f, 0.988f, 1f),
                new Color(0.694f, 0.620f, 0.937f, 1f)
            },
            AnimationSpeed = 8f,
            ShowBorder = false,
            Direction = GradientTextDirection.Horizontal,
            PauseOnHover = false,
            Yoyo = true
        };
    }

    public GradientTextEffectModel Clone()
    {
        return new GradientTextEffectModel
        {
            Text = Text,
            Colors = Colors != null ? (Color[])Colors.Clone() : null,
            AnimationSpeed = AnimationSpeed,
            ShowBorder = ShowBorder,
            Direction = Direction,
            PauseOnHover = PauseOnHover,
            Yoyo = Yoyo
        };
    }
}
