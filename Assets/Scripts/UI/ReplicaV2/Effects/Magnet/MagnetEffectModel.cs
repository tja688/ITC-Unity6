using UnityEngine;

public sealed class MagnetEffectModel
{
    public Vector2 Position;
    public bool IsActive;
    public string Title;
    public string Hint;

    public static MagnetEffectModel CreateDefault()
    {
        return new MagnetEffectModel
        {
            Position = Vector2.zero,
            IsActive = false,
            Title = "Hover Me",
            Hint = "Move pointer near the button"
        };
    }

    public MagnetEffectModel Clone()
    {
        return new MagnetEffectModel
        {
            Position = this.Position,
            IsActive = this.IsActive,
            Title = this.Title,
            Hint = this.Hint
        };
    }
}
