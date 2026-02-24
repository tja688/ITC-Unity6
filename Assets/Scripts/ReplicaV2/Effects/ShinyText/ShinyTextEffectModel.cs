using System;
using UnityEngine;

[Serializable]
public enum ShinyTextDirection
{
    Left,
    Right
}

[Serializable]
public sealed class ShinyTextEffectModel
{
    public string Text = "ShinyText";
    public bool Disabled;
    public float Speed = 2f;
    public Color BaseColor = new Color(0.71f, 0.71f, 0.71f, 1f);
    public Color ShineColor = Color.white;
    public float SpreadDegrees = 120f;
    public bool Yoyo;
    public bool PauseOnHover;
    public ShinyTextDirection Direction = ShinyTextDirection.Left;
    public float Delay;

    public static ShinyTextEffectModel CreateDefault()
    {
        return new ShinyTextEffectModel
        {
            Text = "ShinyText",
            Disabled = false,
            Speed = 2f,
            BaseColor = new Color(0.71f, 0.71f, 0.71f, 1f),
            ShineColor = Color.white,
            SpreadDegrees = 120f,
            Yoyo = false,
            PauseOnHover = false,
            Direction = ShinyTextDirection.Left,
            Delay = 0f
        };
    }

    public ShinyTextEffectModel Clone()
    {
        return new ShinyTextEffectModel
        {
            Text = Text,
            Disabled = Disabled,
            Speed = Speed,
            BaseColor = BaseColor,
            ShineColor = ShineColor,
            SpreadDegrees = SpreadDegrees,
            Yoyo = Yoyo,
            PauseOnHover = PauseOnHover,
            Direction = Direction,
            Delay = Delay
        };
    }
}
