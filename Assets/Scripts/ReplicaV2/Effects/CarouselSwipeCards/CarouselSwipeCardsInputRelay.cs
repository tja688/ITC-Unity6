using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class CarouselSwipeCardsInputRelay : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerEnterHandler,
    IPointerExitHandler
{
    private CarouselSwipeCardsEffectController mController;

    public void Initialize(CarouselSwipeCardsEffectController controller)
    {
        mController = controller;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mController?.OnPointerEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mController?.OnPointerExit(eventData);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mController?.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        mController?.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mController?.OnEndDrag(eventData);
    }
}
