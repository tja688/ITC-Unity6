using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class BubbleMenuOverlayItemData
{
    public string Label;
    public Color HoverBackgroundColor = Color.white;
    public Vector2 Position;
    public float RotationZ;

    public BubbleMenuOverlayItemData Clone()
    {
        return new BubbleMenuOverlayItemData
        {
            Label = Label,
            HoverBackgroundColor = HoverBackgroundColor,
            Position = Position,
            RotationZ = RotationZ
        };
    }
}

[Serializable]
public sealed class BubbleMenuOverlayEffectModel
{
    public List<BubbleMenuOverlayItemData> Items = new List<BubbleMenuOverlayItemData>();

    public static BubbleMenuOverlayEffectModel CreateDefault()
    {
        var model = new BubbleMenuOverlayEffectModel();
        var labels = new[] { "home", "about", "projects", "blog", "contact" };
        var hoverBg = new[]
        {
            new Color(0.23f, 0.51f, 0.96f, 1f),
            new Color(0.06f, 0.68f, 0.51f, 1f),
            new Color(0.96f, 0.63f, 0.11f, 1f),
            new Color(0.93f, 0.27f, 0.26f, 1f),
            new Color(0.54f, 0.36f, 0.95f, 1f)
        };
        var positions = new[]
        {
            new Vector2(-340f, 140f),
            new Vector2(0f, 140f),
            new Vector2(340f, 140f),
            new Vector2(-190f, -80f),
            new Vector2(190f, -80f)
        };
        var rotations = new[] { -8f, 8f, 8f, 8f, -8f };

        for (var i = 0; i < labels.Length; i++)
        {
            model.Items.Add(new BubbleMenuOverlayItemData
            {
                Label = labels[i],
                HoverBackgroundColor = hoverBg[i % hoverBg.Length],
                Position = positions[i % positions.Length],
                RotationZ = rotations[i % rotations.Length]
            });
        }

        return model;
    }

    public BubbleMenuOverlayEffectModel Clone()
    {
        var model = new BubbleMenuOverlayEffectModel();
        for (var i = 0; i < Items.Count; i++)
        {
            model.Items.Add(Items[i]?.Clone());
        }

        return model;
    }
}
