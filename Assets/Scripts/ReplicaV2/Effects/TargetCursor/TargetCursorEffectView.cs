using UnityEngine;
using UnityEngine.UI;

public sealed class TargetCursorEffectView
{
    public RectTransform Root;
    public CanvasGroup Group;
    public RectTransform MotionRoot;

    public RectTransform Dot;
    public Image DotImage;

    // 0: TL, 1: TR, 2: BR, 3: BL
    public RectTransform[] Corners;
    public RectTransform[] HLines;
    public RectTransform[] VLines;
    public Image[] HImages;
    public Image[] VImages;
}
