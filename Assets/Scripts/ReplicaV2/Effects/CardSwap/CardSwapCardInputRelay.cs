using UnityEngine;
using UnityEngine.EventSystems;

[DisallowMultipleComponent]
public sealed class CardSwapCardInputRelay : MonoBehaviour, IPointerClickHandler
{
    private CardSwapEffectController mController;
    private RectTransform mRect;

    public void Initialize(CardSwapEffectController controller, RectTransform rect)
    {
        mController = controller;
        mRect = rect;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        mController?.OnCardClick(mRect, eventData);
    }
}

