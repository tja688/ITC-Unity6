using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class TiltedCardViewLinker : MonoBehaviour
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform Content;
    public RectTransform Card;
    public RectTransform Glint;
    public RectTransform OverlayBand;
    public Text BadgeText;
    public RectTransform Tooltip;
    public CanvasGroup TooltipGroup;
    public Text TooltipText;
    public Text HintText;

    public TiltedCardEffectView ToView()
    {
        return new TiltedCardEffectView
        {
            Root = Root,
            Group = Group,
            Content = Content,
            Card = Card,
            Glint = Glint,
            OverlayBand = OverlayBand,
            BadgeText = BadgeText,
            Tooltip = Tooltip,
            TooltipGroup = TooltipGroup,
            TooltipText = TooltipText,
            HintText = HintText
        };
    }
}
