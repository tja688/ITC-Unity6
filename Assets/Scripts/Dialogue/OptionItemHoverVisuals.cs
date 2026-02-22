using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Yarn.Unity;

namespace ITC.Dialogue
{
    [RequireComponent(typeof(OptionItem))]
    public class OptionItemHoverVisuals : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private OptionItem _optionItem;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        [Header("Animation Settings")]
        public float hoverScale = 1.05f;
        public float clickScale = 0.95f;
        public float animDuration = 0.15f;

        [Header("Entrance Entrance")]
        public float entranceDelayMultiplier = 0.05f;
        public float entranceDuration = 0.35f;

        private Tween _scaleTween;

        private void Awake()
        {
            _optionItem = GetComponent<OptionItem>();
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (_canvasGroup != null)
            {
                // Kill any lingering tweens on recycle
                _canvasGroup.DOKill();
                _rectTransform.DOKill();

                _canvasGroup.alpha = 0f;
                _rectTransform.localScale = new Vector3(0.9f, 0.9f, 1f);

                float delay = transform.GetSiblingIndex() * entranceDelayMultiplier;

                _canvasGroup.DOFade(1f, entranceDuration).SetDelay(delay).SetEase(Ease.OutCubic);
                _rectTransform.DOScale(1f, entranceDuration).SetDelay(delay).SetEase(Ease.OutBack);
            }
            else
            {
                _rectTransform.localScale = Vector3.one;
            }
        }

        private void OnDisable()
        {
            _scaleTween?.Kill();
            _rectTransform.DOKill();
            if (_canvasGroup != null) _canvasGroup.DOKill();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_optionItem.interactable) return;

            _scaleTween?.Kill();
            _scaleTween = _rectTransform.DOScale(hoverScale, animDuration).SetEase(Ease.OutBack);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_optionItem.interactable) return;

            _scaleTween?.Kill();
            _scaleTween = _rectTransform.DOScale(1f, animDuration).SetEase(Ease.OutQuad);

            // Crucial: Deselect to allow normal Color to restore, 
            // since Unity Selectable doesn't do this inherently on MouseExit!
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_optionItem.interactable) return;

            _scaleTween?.Kill();
            _scaleTween = _rectTransform.DOScale(clickScale, animDuration).SetEase(Ease.OutQuad);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_optionItem.interactable) return;

            bool isHovering = RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, eventData.position, eventData.pressEventCamera);
            float targetScale = isHovering ? hoverScale : 1f;

            _scaleTween?.Kill();
            _scaleTween = _rectTransform.DOScale(targetScale, animDuration).SetEase(Ease.OutBack);
        }
    }
}
