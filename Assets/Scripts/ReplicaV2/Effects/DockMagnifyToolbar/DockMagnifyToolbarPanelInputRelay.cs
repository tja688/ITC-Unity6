using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class DockMagnifyToolbarPanelInputRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private DockMagnifyToolbarEffectController mController;

    public void Initialize(DockMagnifyToolbarEffectController controller)
    {
        mController = controller;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mController?.SetPanelHovered(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mController?.SetPanelHovered(false);
    }
}
