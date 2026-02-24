using System;
using UnityEngine;

[Serializable]
public sealed class StackItemData
{
    public string Id;
    public string Title;
    public string Meta;
    public Sprite Sprite;
    public Color Tint = Color.white;

    public StackItemData Clone()
    {
        return new StackItemData
        {
            Id = Id,
            Title = Title,
            Meta = Meta,
            Sprite = Sprite,
            Tint = Tint
        };
    }
}
