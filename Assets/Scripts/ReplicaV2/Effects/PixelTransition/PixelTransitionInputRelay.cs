using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class PixelTransitionInputRelay : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler,
    ISelectHandler,
    IDeselectHandler
{
    private PixelTransitionEffectController mController;

    public void Initialize(PixelTransitionEffectController controller)
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

    public void OnPointerClick(PointerEventData eventData)
    {
        mController?.OnPointerClick(eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        mController?.OnSelect(eventData);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        mController?.OnDeselect(eventData);
    }
}

