using QFramework;
using UnityEngine;

public sealed class MainMenuUIKitConfig : UIKitConfig
{
    public MainMenuUIKitConfig()
    {
        PanelLoaderPool = new ResKitPanelLoaderPool();
    }

    public override void SetDefaultSizeOfPanel(IPanel panel)
    {
        if (panel.Transform is RectTransform rectTransform)
        {
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.localScale = Vector3.one;
        }
    }
}
