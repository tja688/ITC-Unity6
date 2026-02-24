using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class StackCardInputRelay : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IPointerClickHandler
{
    private StackEffectController mController;
    private string mItemId;

    public void Initialize(StackEffectController controller)
    {
        mController = controller;
    }

    public void SetItemId(string itemId)
    {
        mItemId = itemId;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        mController?.OnCardBeginDrag(mItemId, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        mController?.OnCardDrag(mItemId, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        mController?.OnCardEndDrag(mItemId, eventData);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        mController?.OnCardClick(mItemId, eventData);
    }
}
