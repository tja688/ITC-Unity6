using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class GlassIconsTiltEffectItemModel
{
    public string Label;
    public string Glyph;
    public Color BackColor;

    public GlassIconsTiltEffectItemModel Clone()
    {
        return new GlassIconsTiltEffectItemModel
        {
            Label = Label,
            Glyph = Glyph,
            BackColor = BackColor
        };
    }
}

[Serializable]
public sealed class GlassIconsTiltEffectModel
{
    public string Hint = "GlassIcons  |  Hover each icon tile";
    public List<GlassIconsTiltEffectItemModel> Items = new List<GlassIconsTiltEffectItemModel>();

    public GlassIconsTiltEffectModel Clone()
    {
        var clone = new GlassIconsTiltEffectModel
        {
            Hint = Hint,
            Items = new List<GlassIconsTiltEffectItemModel>()
        };

        if (Items != null)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                clone.Items.Add(Items[i] != null ? Items[i].Clone() : null);
            }
        }

        return clone;
    }

    public static GlassIconsTiltEffectModel CreateDefault()
    {
        return new GlassIconsTiltEffectModel
        {
            Items = new List<GlassIconsTiltEffectItemModel>
            {
                new GlassIconsTiltEffectItemModel { Label = "Home", Glyph = "H", BackColor = new Color(0.22f, 0.46f, 0.90f, 1f) },
                new GlassIconsTiltEffectItemModel { Label = "Email", Glyph = "@", BackColor = new Color(0.64f, 0.34f, 0.92f, 1f) },
                new GlassIconsTiltEffectItemModel { Label = "Play", Glyph = ">", BackColor = new Color(0.88f, 0.36f, 0.30f, 1f) },
                new GlassIconsTiltEffectItemModel { Label = "Photo", Glyph = "P", BackColor = new Color(0.34f, 0.44f, 0.88f, 1f) },
                new GlassIconsTiltEffectItemModel { Label = "Chart", Glyph = "#", BackColor = new Color(0.84f, 0.58f, 0.24f, 1f) },
                new GlassIconsTiltEffectItemModel { Label = "Cloud", Glyph = "C", BackColor = new Color(0.28f, 0.66f, 0.42f, 1f) }
            }
        };
    }
}
