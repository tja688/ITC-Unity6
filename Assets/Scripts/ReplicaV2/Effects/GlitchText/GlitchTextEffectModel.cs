using System;

[Serializable]
public sealed class GlitchTextEffectModel
{
    public string Text = "GLITCH";
    public float Speed = 1f;
    public bool EnableShadows = true;
    public bool EnableOnHover = true;

    public GlitchTextEffectModel Clone()
    {
        return new GlitchTextEffectModel
        {
            Text = Text,
            Speed = Speed,
            EnableShadows = EnableShadows,
            EnableOnHover = EnableOnHover
        };
    }

    public static GlitchTextEffectModel CreateDefault()
    {
        return new GlitchTextEffectModel();
    }
}
