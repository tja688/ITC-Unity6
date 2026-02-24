using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class CurvedLoopInputRelay : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private CurvedLoopEffectController mController;

    public void Initialize(CurvedLoopEffectController controller)
    {
        mController = controller;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mController?.OnPointerDown(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        mController?.OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mController?.OnPointerUp(eventData);
    }
}
