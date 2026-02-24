using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class ScrollStackEffectModel
{
    public List<ScrollStackItemModel> items = new List<ScrollStackItemModel>();
}

[Serializable]
public sealed class ScrollStackItemModel
{
    public string title;
    public string description;
    public Sprite image;
    public Color backgroundColor = Color.white;
}
