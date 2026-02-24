using System;
using System.Collections.Generic;

[Serializable]
public sealed class MenuRevealStaggerLayeredEffectModel
{
    public List<string> MenuItems = new List<string>();
    public List<string> SocialItems = new List<string>();

    public static MenuRevealStaggerLayeredEffectModel CreateDefault()
    {
        var model = new MenuRevealStaggerLayeredEffectModel();
        model.MenuItems.AddRange(new[] { "Home", "Works", "Services", "Journal", "Contact" });
        model.SocialItems.AddRange(new[] { "GitHub", "Dribbble", "Behance" });
        return model;
    }

    public MenuRevealStaggerLayeredEffectModel Clone()
    {
        var model = new MenuRevealStaggerLayeredEffectModel();
        model.MenuItems.AddRange(MenuItems);
        model.SocialItems.AddRange(SocialItems);
        return model;
    }
}
