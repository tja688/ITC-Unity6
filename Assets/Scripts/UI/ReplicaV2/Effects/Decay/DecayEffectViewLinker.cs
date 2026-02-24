using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class DecayEffectViewLinker : MonoBehaviour
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform CardRoot;
    public Image CardImage;
    public Text TitleText;
    public Text SubtitleText;
    public Text MarkerText;

    [Header("Noise Strips")]
    public List<RectTransform> NoiseStrips;
    public List<float> NoiseBaseY;
    public List<Image> NoiseImages;

    public DecayEffectView ToView()
    {
        var view = new DecayEffectView
        {
            Root = Root,
            Group = Group,
            CardRoot = CardRoot,
            CardImage = CardImage,
            TitleText = TitleText,
            SubtitleText = SubtitleText,
            MarkerText = MarkerText
        };

        if (NoiseStrips != null) view.NoiseStrips.AddRange(NoiseStrips);
        if (NoiseBaseY != null) view.NoiseBaseY.AddRange(NoiseBaseY);
        if (NoiseImages != null) view.NoiseImages.AddRange(NoiseImages);

        return view;
    }
}
