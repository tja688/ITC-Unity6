using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class CarouselSwipeCardsCardData
{
    public string Title;
    public string Description;
    public Color CardColor = Color.white;
    public Color AccentColor = Color.white;

    public CarouselSwipeCardsCardData Clone()
    {
        return new CarouselSwipeCardsCardData
        {
            Title = Title,
            Description = Description,
            CardColor = CardColor,
            AccentColor = AccentColor
        };
    }
}

[Serializable]
public sealed class CarouselSwipeCardsEffectModel
{
    public List<CarouselSwipeCardsCardData> Cards = new List<CarouselSwipeCardsCardData>();

    public static CarouselSwipeCardsEffectModel CreateDefault()
    {
        var model = new CarouselSwipeCardsEffectModel();
        var titles = new[]
        {
            "Text Animations",
            "Animations",
            "Components",
            "Backgrounds",
            "Common UI"
        };
        var desc = new[]
        {
            "Cool text animations for your projects.",
            "Smooth animation recipes for interaction states.",
            "Reusable building blocks for rapid interfaces.",
            "Layered surfaces and visual atmosphere.",
            "Shared controls and practical UI patterns."
        };
        var cardColors = new[]
        {
            new Color(0.20f, 0.29f, 0.56f, 1f),
            new Color(0.10f, 0.47f, 0.53f, 1f),
            new Color(0.52f, 0.31f, 0.72f, 1f),
            new Color(0.70f, 0.43f, 0.28f, 1f),
            new Color(0.74f, 0.26f, 0.40f, 1f)
        };
        var accentColors = new[]
        {
            new Color(0.42f, 0.58f, 0.94f, 0.9f),
            new Color(0.19f, 0.79f, 0.71f, 0.9f),
            new Color(0.72f, 0.52f, 0.97f, 0.9f),
            new Color(0.94f, 0.65f, 0.31f, 0.9f),
            new Color(0.97f, 0.44f, 0.58f, 0.9f)
        };

        for (var i = 0; i < titles.Length; i++)
        {
            model.Cards.Add(new CarouselSwipeCardsCardData
            {
                Title = titles[i],
                Description = desc[i],
                CardColor = cardColors[i % cardColors.Length],
                AccentColor = accentColors[i % accentColors.Length]
            });
        }

        return model;
    }

    public CarouselSwipeCardsEffectModel Clone()
    {
        var model = new CarouselSwipeCardsEffectModel();
        for (var i = 0; i < Cards.Count; i++)
        {
            model.Cards.Add(Cards[i]?.Clone());
        }

        return model;
    }
}
