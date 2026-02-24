using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class OrbitImagesEffectModel
{
    public List<Sprite> Images = new List<Sprite>();
    public string CenterTitle = "Orbit Images";
    public bool Paused = false;

    public OrbitImagesEffectModel Clone()
    {
        return new OrbitImagesEffectModel
        {
            Images = Images != null ? new List<Sprite>(Images) : new List<Sprite>(),
            CenterTitle = CenterTitle,
            Paused = Paused
        };
    }

    public static OrbitImagesEffectModel CreateDefault()
    {
        return new OrbitImagesEffectModel();
    }
}
