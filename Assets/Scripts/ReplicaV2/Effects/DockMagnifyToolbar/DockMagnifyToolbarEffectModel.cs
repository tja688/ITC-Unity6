using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class DockMagnifyToolbarItemData
{
    public string Label;
    public string Glyph;
    public Color BackgroundColor = Color.white;

    public DockMagnifyToolbarItemData Clone()
    {
        return new DockMagnifyToolbarItemData
        {
            Label = Label,
            Glyph = Glyph,
            BackgroundColor = BackgroundColor
        };
    }
}

[Serializable]
public sealed class DockMagnifyToolbarEffectModel
{
    public List<DockMagnifyToolbarItemData> Items = new List<DockMagnifyToolbarItemData>();

    public static DockMagnifyToolbarEffectModel CreateDefault()
    {
        var model = new DockMagnifyToolbarEffectModel();
        var labels = new[] { "Finder", "Music", "Mail", "Code", "Photos", "Prefs" };
        var glyphs = new[] { "F", "M", "@", "C", "P", "S" };
        var colors = new[]
        {
            new Color(0.24f, 0.42f, 0.73f, 1f),
            new Color(0.24f, 0.62f, 0.50f, 1f),
            new Color(0.72f, 0.40f, 0.30f, 1f),
            new Color(0.54f, 0.39f, 0.78f, 1f),
            new Color(0.73f, 0.56f, 0.30f, 1f),
            new Color(0.38f, 0.56f, 0.79f, 1f)
        };

        for (var i = 0; i < labels.Length; i++)
        {
            model.Items.Add(new DockMagnifyToolbarItemData
            {
                Label = labels[i],
                Glyph = glyphs[i],
                BackgroundColor = colors[i % colors.Length]
            });
        }

        return model;
    }

    public DockMagnifyToolbarEffectModel Clone()
    {
        var model = new DockMagnifyToolbarEffectModel();
        for (var i = 0; i < Items.Count; i++)
        {
            model.Items.Add(Items[i]?.Clone());
        }

        return model;
    }
}
