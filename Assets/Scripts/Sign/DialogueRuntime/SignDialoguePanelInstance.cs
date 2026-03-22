using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ITC.Dialogue
{
    [DisallowMultipleComponent]
    public sealed class SignDialoguePanelInstance : MonoBehaviour
    {
        [Header("Runtime Bindings")]
        [SerializeField] private TMP_Text mainText;
        [SerializeField] private DOTweenAnimation popupAnimation;
        [SerializeField] private DOTweenAnimation shrinkAnimation;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Options Layout")]
        [SerializeField] private Vector4 optionPadding = new(110f, 96f, 96f, 96f);
        [SerializeField] private float optionSpacing = 18f;

        private RectTransform optionContainer;
        private RectTransform rectTransform;
        private bool destroyQueued;
        private bool visiblePoseCaptured;
        private Vector3 visibleLocalPosition;
        private Vector3 visibleLocalScale;
        private Quaternion visibleLocalRotation;
        private Vector2 visibleAnchoredPosition;
        private Vector2 visibleSizeDelta;

        public string CurrentText => mainText != null ? mainText.text : string.Empty;

        private void Awake()
        {
            CaptureVisiblePose();
            ResolveReferences();
            ConfigureAnimationsForManualControl();
        }

        public void ResolveReferences()
        {
            if (mainText == null)
            {
                mainText = GetComponentInChildren<TMP_Text>(true);
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            if (rectTransform == null)
            {
                rectTransform = transform as RectTransform;
            }

            if (optionContainer == null)
            {
                optionContainer = FindOptionContainer();
            }

            if (popupAnimation == null || shrinkAnimation == null)
            {
                var animations = GetComponentsInChildren<DOTweenAnimation>(true);
                foreach (var animation in animations)
                {
                    if (animation == null)
                    {
                        continue;
                    }

                    if (animation.gameObject.name == "弹出")
                    {
                        popupAnimation = animation;
                    }
                    else if (animation.gameObject.name == "回缩")
                    {
                        shrinkAnimation = animation;
                    }
                }
            }
        }

        public void ConfigureAnimationsForManualControl()
        {
            ResolveReferences();

            if (popupAnimation != null)
            {
                popupAnimation.autoPlay = false;
                popupAnimation.autoGenerate = false;
                popupAnimation.isIndependentUpdate = true;
                popupAnimation.DOKill();
            }

            if (shrinkAnimation != null)
            {
                shrinkAnimation.autoPlay = false;
                shrinkAnimation.autoGenerate = false;
                shrinkAnimation.isIndependentUpdate = true;
                shrinkAnimation.DOKill();
            }
        }

        public void PrepareForText(string text)
        {
            ResolveReferences();
            destroyQueued = false;

            if (optionContainer != null)
            {
                optionContainer.gameObject.SetActive(false);
            }

            if (mainText != null)
            {
                mainText.gameObject.SetActive(true);
                mainText.text = text ?? string.Empty;
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            RestoreVisiblePose();
        }

        public RectTransform PrepareForOptions()
        {
            ResolveReferences();
            destroyQueued = false;

            if (mainText != null)
            {
                mainText.text = string.Empty;
                mainText.gameObject.SetActive(false);
            }

            var container = EnsureOptionContainer();
            ClearOptionContainer(container);
            container.gameObject.SetActive(true);

            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            RestoreVisiblePose();
            return container;
        }

        public void PlayShow()
        {
            ResolveReferences();
            ConfigureAnimationsForManualControl();
            RestoreVisiblePose();

            if (popupAnimation != null)
            {
                popupAnimation.RecreateTweenAndPlay();
            }
        }

        public void ShowRetained(bool playAnimation)
        {
            gameObject.SetActive(true);
            RestoreVisiblePose();
            if (playAnimation)
            {
                PlayShow();
            }
        }

        public void HideRetained()
        {
            if (gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }

        public void PlayHideAndDestroy()
        {
            if (destroyQueued)
            {
                return;
            }

            destroyQueued = true;
            StopAllCoroutines();

            if (shrinkAnimation != null)
            {
                shrinkAnimation.RecreateTweenAndPlay();
                StartCoroutine(DestroyAfterDelay(shrinkAnimation.delay + shrinkAnimation.duration + 0.05f));
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void DestroyImmediateSafe()
        {
            StopAllCoroutines();
            if (gameObject != null)
            {
                Destroy(gameObject);
            }
        }

        private RectTransform EnsureOptionContainer()
        {
            if (optionContainer != null)
            {
                EnsureOptionContainerLayout(optionContainer.gameObject);
                return optionContainer;
            }

            var containerObject = new GameObject(
                "OptionsContainer",
                typeof(RectTransform),
                typeof(CanvasGroup),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));

            optionContainer = containerObject.GetComponent<RectTransform>();
            optionContainer.SetParent(transform, false);
            optionContainer.anchorMin = Vector2.zero;
            optionContainer.anchorMax = Vector2.one;
            optionContainer.pivot = new Vector2(0.5f, 0.5f);
            optionContainer.offsetMin = new Vector2(optionPadding.x, optionPadding.w);
            optionContainer.offsetMax = new Vector2(-optionPadding.y, -optionPadding.z);
            optionContainer.SetAsLastSibling();

            EnsureOptionContainerLayout(containerObject);
            return optionContainer;
        }

        private void EnsureOptionContainerLayout(GameObject containerObject)
        {
            var layout = containerObject.GetComponent<VerticalLayoutGroup>();
            if (layout == null)
            {
                layout = containerObject.AddComponent<VerticalLayoutGroup>();
            }

            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            layout.spacing = optionSpacing;

            var fitter = containerObject.GetComponent<ContentSizeFitter>();
            if (fitter == null)
            {
                fitter = containerObject.AddComponent<ContentSizeFitter>();
            }

            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        private static void ClearOptionContainer(RectTransform container)
        {
            for (var i = container.childCount - 1; i >= 0; i--)
            {
                var child = container.GetChild(i);
                if (child != null)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private void CaptureVisiblePose()
        {
            ResolveReferences();
            if (visiblePoseCaptured || rectTransform == null)
            {
                return;
            }

            visiblePoseCaptured = true;
            visibleLocalPosition = rectTransform.localPosition;
            visibleLocalScale = rectTransform.localScale;
            visibleLocalRotation = rectTransform.localRotation;
            visibleAnchoredPosition = rectTransform.anchoredPosition;
            visibleSizeDelta = rectTransform.sizeDelta;
        }

        private void RestoreVisiblePose()
        {
            CaptureVisiblePose();
            if (rectTransform == null)
            {
                return;
            }

            rectTransform.localPosition = visibleLocalPosition;
            rectTransform.localScale = visibleLocalScale;
            rectTransform.localRotation = visibleLocalRotation;
            rectTransform.anchoredPosition = visibleAnchoredPosition;
            rectTransform.sizeDelta = visibleSizeDelta;
        }

        private IEnumerator DestroyAfterDelay(float delay)
        {
            if (delay > 0f)
            {
                yield return new WaitForSecondsRealtime(delay);
            }

            Destroy(gameObject);
        }

        private RectTransform FindOptionContainer()
        {
            foreach (Transform child in transform)
            {
                if (child == null)
                {
                    continue;
                }

                if (child.name == "玩家框选项" || child.name == "OptionsContainer")
                {
                    return child as RectTransform;
                }
            }

            return null;
        }
    }
}
