using System;
using System.Collections.Generic;

[Serializable]
public sealed class StackEffectModel
{
    public List<StackItemData> Items = new List<StackItemData>();

    public static StackEffectModel CreateDefault(int count)
    {
        var model = new StackEffectModel();
        var palette = new[]
        {
            new UnityEngine.Color(0.33f, 0.46f, 0.76f, 1f),
            new UnityEngine.Color(0.19f, 0.58f, 0.63f, 1f),
            new UnityEngine.Color(0.67f, 0.49f, 0.26f, 1f),
            new UnityEngine.Color(0.28f, 0.55f, 0.35f, 1f),
            new UnityEngine.Color(0.58f, 0.35f, 0.72f, 1f),
            new UnityEngine.Color(0.72f, 0.40f, 0.30f, 1f),
            new UnityEngine.Color(0.24f, 0.62f, 0.50f, 1f),
            new UnityEngine.Color(0.38f, 0.56f, 0.79f, 1f)
        };

        var safe = UnityEngine.Mathf.Clamp(count, 1, 12);
        for (var i = 0; i < safe; i++)
        {
            model.Items.Add(new StackItemData
            {
                Id = $"stack-card-{i + 1}",
                Title = $"Card {i + 1}",
                Meta = "Drag to cycle",
                Tint = palette[i % palette.Length]
            });
        }

        return model;
    }

    public StackEffectModel Clone()
    {
        var model = new StackEffectModel();
        for (var i = 0; i < Items.Count; i++)
        {
            model.Items.Add(Items[i]?.Clone());
        }

        return model;
    }
}
