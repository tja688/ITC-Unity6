using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class AnimatedContentViewLinker : MonoBehaviour
{
    public RectTransform Root;
    public RectTransform Container;
    public Image ContainerImage;
    public CanvasGroup ContainerGroup;
    public Text LabelText;

    public AnimatedContentEffectView ToView()
    {
        return new AnimatedContentEffectView
        {
            Root = Root,
            Container = Container,
            ContainerImage = ContainerImage,
            ContainerGroup = ContainerGroup,
            LabelText = LabelText
        };
    }
}
