using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class DockMagnifyToolbarItemInputRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private DockMagnifyToolbarEffectController mController;
    private int mIndex = -1;

    public void Initialize(DockMagnifyToolbarEffectController controller, int index)
    {
        mController = controller;
        mIndex = index;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mController?.SetItemHovered(mIndex, true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mController?.SetItemHovered(mIndex, false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        mController?.OnItemClicked(mIndex);
    }
}
