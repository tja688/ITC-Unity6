using System;

[Serializable]
public sealed class ReflectiveCardEffectModel
{
    public string Hint = "ReflectiveCard  |  Hover card for metallic sheen";
    public string UserName = "ALEXANDER DOE";
    public string Role = "SENIOR DEVELOPER";
    public string IdNumber = "8901-2345-6789";
    public string Badge = "\u25CF  SECURE ACCESS";

    public ReflectiveCardEffectModel Clone()
    {
        return new ReflectiveCardEffectModel
        {
            Hint = Hint,
            UserName = UserName,
            Role = Role,
            IdNumber = IdNumber,
            Badge = Badge
        };
    }

    public static ReflectiveCardEffectModel CreateDefault()
    {
        return new ReflectiveCardEffectModel();
    }
}

