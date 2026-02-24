using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class BubbleMenuOverlayPillInputRelay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private BubbleMenuOverlayEffectController mController;
    private int mIndex;

    public void Initialize(BubbleMenuOverlayEffectController controller, int index)
    {
        mController = controller;
        mIndex = index;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mController?.OnPillPointerEnter(mIndex);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mController?.OnPillPointerExit(mIndex);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mController?.OnPillPointerDown(mIndex);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mController?.OnPillPointerUp(mIndex);
    }
}
