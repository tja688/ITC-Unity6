using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class MagnetViewLinker : MonoBehaviour
{
    [Header("References")]
    public RectTransform Root;
    public RectTransform Surface;
    public RectTransform InnerContainer;
    public Image SurfaceImage;
    public Outline SurfaceOutline;
    public Image ButtonImage;
    public Shadow ButtonShadow;
    public Text ButtonLabel;
    public Text HintText;

    public MagnetEffectView ToView()
    {
        return new MagnetEffectView
        {
            Root = Root,
            Surface = Surface,
            InnerContainer = InnerContainer,
            SurfaceImage = SurfaceImage,
            SurfaceOutline = SurfaceOutline,
            ButtonImage = ButtonImage,
            ButtonShadow = ButtonShadow,
            ButtonLabel = ButtonLabel,
            HintText = HintText
        };
    }
}
