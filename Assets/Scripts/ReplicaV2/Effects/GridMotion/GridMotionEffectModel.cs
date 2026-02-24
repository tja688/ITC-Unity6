using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class GridMotionEffectItemModel
{
    public string Text;
    public Sprite Sprite;

    public GridMotionEffectItemModel Clone()
    {
        return new GridMotionEffectItemModel
        {
            Text = Text,
            Sprite = Sprite
        };
    }
}

[Serializable]
public sealed class GridMotionEffectModel
{
    public List<GridMotionEffectItemModel> Items = new List<GridMotionEffectItemModel>();

    public GridMotionEffectModel Clone()
    {
        var clone = new GridMotionEffectModel
        {
            Items = new List<GridMotionEffectItemModel>()
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

    public static GridMotionEffectModel CreateDefault(int count)
    {
        var model = new GridMotionEffectModel();
        count = Mathf.Max(0, count);
        for (var i = 0; i < count; i++)
        {
            model.Items.Add(new GridMotionEffectItemModel { Text = $"Item {i + 1}" });
        }

        return model;
    }
}
