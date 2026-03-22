using UnityEngine;
using UnityEngine.EventSystems;

namespace ITC.Dialogue
{
    [DisallowMultipleComponent]
    public sealed class SignCtrlHoverHitbox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private SignCtrlHoverDriver owner;

        public void Bind(SignCtrlHoverDriver driver)
        {
            owner = driver;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (owner != null)
            {
                owner.NotifyPointerEnter();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (owner != null)
            {
                owner.NotifyPointerExit();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (owner != null)
            {
                owner.NotifyPointerClick(eventData);
            }
        }

        private void OnDisable()
        {
            if (owner != null)
            {
                owner.NotifyPointerExit();
            }
        }
    }
}
