using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ITC.Tests.SignPanel
{
    [DisallowMultipleComponent]
    public sealed class SignPanelCardFeelPrototype : MonoBehaviour
    {
        private enum CardPose
        {
            Hidden = 0,
            Upright = 1,
            Fallen = 2
        }

        [Header("Scene References")]
        [SerializeField] private RectTransform cardRoot;
        [SerializeField] private RectTransform hiddenAnchor;
        [SerializeField] private CanvasGroup cardCanvasGroup;

        [Header("Input")]
        [SerializeField] private bool allowMouseInput = true;
        [SerializeField] private bool startHidden = true;

        [Header("Entry")]
        [SerializeField] private float enterMoveDuration = 0.18f;
        [SerializeField] private Ease enterMoveEase = Ease.OutCubic;
        [SerializeField] private float enterFadeDuration = 0.12f;
        [SerializeField] private float enterScaleMultiplier = 0.965f;
        [SerializeField] private float enterShakeDuration = 0.14f;
        [SerializeField] private float enterShakeAngleZ = 2.2f;
        [SerializeField] private int enterShakeVibrato = 2;
        [SerializeField, Range(0f, 1f)] private float enterShakeElasticity = 0.5f;

        [Header("Fall")]
        [SerializeField] private float fallDuration = 0.18f;
        [SerializeField] private Ease fallEase = Ease.InQuad;
        [SerializeField] private float fallenXAngle = -90f;

        [Header("Return")]
        [SerializeField] private float returnOvershootDuration = 0.14f;
        [SerializeField] private Ease returnOvershootEase = Ease.OutCubic;
        [SerializeField] private float returnOvershootXAngle = 8f;
        [SerializeField] private float returnSettleDuration = 0.10f;
        [SerializeField] private Ease returnSettleEase = Ease.OutSine;

        [Header("Exit")]
        [SerializeField] private float exitDuration = 0.18f;
        [SerializeField] private Ease exitEase = Ease.InCubic;
        [SerializeField] private float exitFadeDuration = 0.12f;

        [Header("Fallback")]
        [SerializeField] private float hiddenFallbackOffsetY = -240f;

        private Sequence activeSequence;
        private bool isTransitionRunning;
        private bool hasCachedPose;
        private CardPose currentPose = CardPose.Hidden;

        private Vector2 activeAnchoredPosition;
        private Vector3 activeLocalEulerAngles;
        private Vector3 activeLocalScale;
        private Vector2 hiddenAnchoredPosition;

        private void Awake()
        {
            ResolveReferences();
            CacheActivePose();
        }

        private void OnEnable()
        {
            ResolveReferences();

            if (Application.isPlaying)
            {
                CacheActivePose();
                ApplyInitialRuntimePose();
            }
        }

        private void OnDisable()
        {
            KillActiveSequence();

            if (Application.isPlaying)
            {
                RestoreCachedPose();
            }
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            ResolveReferences();
        }

        private void Update()
        {
            if (!Application.isPlaying || !allowMouseInput || isTransitionRunning)
            {
                return;
            }

            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                HandleLeftClick();
                return;
            }

            if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            {
                HandleRightClick();
            }
        }

        [ContextMenu("Snap Hidden")]
        public void SnapHidden()
        {
            ResolveReferences();
            CacheActivePose();
            ApplyHiddenPoseImmediate();
        }

        [ContextMenu("Snap Upright")]
        public void SnapUpright()
        {
            ResolveReferences();
            CacheActivePose();
            ApplyUprightPoseImmediate();
        }

        [ContextMenu("Snap Fallen")]
        public void SnapFallen()
        {
            ResolveReferences();
            CacheActivePose();
            ApplyFallenPoseImmediate();
        }

        private void HandleLeftClick()
        {
            switch (currentPose)
            {
                case CardPose.Hidden:
                    PlayEnterSequence();
                    break;
                case CardPose.Upright:
                    PlayFallSequence();
                    break;
            }
        }

        private void HandleRightClick()
        {
            switch (currentPose)
            {
                case CardPose.Fallen:
                    PlayReturnSequence();
                    break;
                case CardPose.Upright:
                    PlayExitSequence();
                    break;
            }
        }

        private void PlayEnterSequence()
        {
            if (!ValidateReadyForTween())
            {
                return;
            }

            BeginTween();

            cardRoot.anchoredPosition = hiddenAnchoredPosition;
            cardRoot.localEulerAngles = activeLocalEulerAngles;
            cardRoot.localScale = activeLocalScale * enterScaleMultiplier;
            cardCanvasGroup.alpha = 0f;
            cardCanvasGroup.blocksRaycasts = false;
            cardCanvasGroup.interactable = false;

            var sequence = DOTween.Sequence();
            sequence.Append(cardRoot.DOAnchorPos(activeAnchoredPosition, enterMoveDuration).SetEase(enterMoveEase));
            sequence.Join(cardRoot.DOScale(activeLocalScale, enterMoveDuration).SetEase(Ease.OutBack));
            sequence.Join(cardCanvasGroup.DOFade(1f, enterFadeDuration).SetEase(Ease.OutQuad));
            sequence.Append(cardRoot.DOPunchRotation(new Vector3(0f, 0f, enterShakeAngleZ), enterShakeDuration, enterShakeVibrato, enterShakeElasticity));
            sequence.OnComplete(() =>
            {
                currentPose = CardPose.Upright;
                EndTween();
            });

            activeSequence = sequence;
        }

        private void PlayFallSequence()
        {
            if (!ValidateReadyForTween())
            {
                return;
            }

            BeginTween();

            cardCanvasGroup.alpha = 1f;
            cardRoot.localEulerAngles = activeLocalEulerAngles;

            var targetEulerAngles = new Vector3(fallenXAngle, activeLocalEulerAngles.y, activeLocalEulerAngles.z);
            var sequence = DOTween.Sequence();
            sequence.Append(cardRoot.DOLocalRotate(targetEulerAngles, fallDuration, RotateMode.Fast).SetEase(fallEase));
            sequence.OnComplete(() =>
            {
                currentPose = CardPose.Fallen;
                EndTween();
            });

            activeSequence = sequence;
        }

        private void PlayReturnSequence()
        {
            if (!ValidateReadyForTween())
            {
                return;
            }

            BeginTween();

            cardCanvasGroup.alpha = 1f;
            cardRoot.localEulerAngles = new Vector3(fallenXAngle, activeLocalEulerAngles.y, activeLocalEulerAngles.z);

            var overshootEulerAngles = new Vector3(returnOvershootXAngle, activeLocalEulerAngles.y, activeLocalEulerAngles.z);
            var sequence = DOTween.Sequence();
            sequence.Append(cardRoot.DOLocalRotate(overshootEulerAngles, returnOvershootDuration, RotateMode.Fast).SetEase(returnOvershootEase));
            sequence.Append(cardRoot.DOLocalRotate(activeLocalEulerAngles, returnSettleDuration, RotateMode.Fast).SetEase(returnSettleEase));
            sequence.OnComplete(() =>
            {
                currentPose = CardPose.Upright;
                EndTween();
            });

            activeSequence = sequence;
        }

        private void PlayExitSequence()
        {
            if (!ValidateReadyForTween())
            {
                return;
            }

            BeginTween();

            cardCanvasGroup.alpha = 1f;
            cardRoot.localEulerAngles = activeLocalEulerAngles;

            var sequence = DOTween.Sequence();
            sequence.Append(cardRoot.DOAnchorPos(hiddenAnchoredPosition, exitDuration).SetEase(exitEase));
            sequence.Join(cardCanvasGroup.DOFade(0f, exitFadeDuration).SetEase(Ease.OutQuad));
            sequence.OnComplete(() =>
            {
                currentPose = CardPose.Hidden;
                EndTween();
            });

            activeSequence = sequence;
        }

        private void ApplyInitialRuntimePose()
        {
            if (!Application.isPlaying || cardRoot == null)
            {
                return;
            }

            if (startHidden)
            {
                ApplyHiddenPoseImmediate();
                return;
            }

            ApplyUprightPoseImmediate();
        }

        private void ApplyHiddenPoseImmediate()
        {
            if (!ValidatePoseCache())
            {
                return;
            }

            KillActiveSequence();
            cardRoot.anchoredPosition = hiddenAnchoredPosition;
            cardRoot.localEulerAngles = activeLocalEulerAngles;
            cardRoot.localScale = activeLocalScale;
            cardCanvasGroup.alpha = 0f;
            currentPose = CardPose.Hidden;
        }

        private void ApplyUprightPoseImmediate()
        {
            if (!ValidatePoseCache())
            {
                return;
            }

            KillActiveSequence();
            cardRoot.anchoredPosition = activeAnchoredPosition;
            cardRoot.localEulerAngles = activeLocalEulerAngles;
            cardRoot.localScale = activeLocalScale;
            cardCanvasGroup.alpha = 1f;
            currentPose = CardPose.Upright;
        }

        private void ApplyFallenPoseImmediate()
        {
            if (!ValidatePoseCache())
            {
                return;
            }

            KillActiveSequence();
            cardRoot.anchoredPosition = activeAnchoredPosition;
            cardRoot.localEulerAngles = new Vector3(fallenXAngle, activeLocalEulerAngles.y, activeLocalEulerAngles.z);
            cardRoot.localScale = activeLocalScale;
            cardCanvasGroup.alpha = 1f;
            currentPose = CardPose.Fallen;
        }

        private void RestoreCachedPose()
        {
            if (!ValidatePoseCache())
            {
                return;
            }

            cardRoot.anchoredPosition = activeAnchoredPosition;
            cardRoot.localEulerAngles = activeLocalEulerAngles;
            cardRoot.localScale = activeLocalScale;
            cardCanvasGroup.alpha = 1f;
            currentPose = CardPose.Upright;
        }

        private void ResolveReferences()
        {
            if (cardRoot == null)
            {
                cardRoot = FindRectTransformByName("对话框1号");
            }

            if (hiddenAnchor == null)
            {
                hiddenAnchor = FindRectTransformByName("对话框0号");
            }

            if (cardRoot != null && cardCanvasGroup == null && !cardRoot.TryGetComponent(out cardCanvasGroup))
            {
                cardCanvasGroup = cardRoot.gameObject.AddComponent<CanvasGroup>();
            }
        }

        private void CacheActivePose()
        {
            if (hasCachedPose || cardRoot == null)
            {
                return;
            }

            activeAnchoredPosition = cardRoot.anchoredPosition;
            activeLocalEulerAngles = cardRoot.localEulerAngles;
            activeLocalScale = cardRoot.localScale;

            if (hiddenAnchor != null)
            {
                hiddenAnchoredPosition = hiddenAnchor.anchoredPosition;
            }
            else
            {
                hiddenAnchoredPosition = activeAnchoredPosition + new Vector2(0f, hiddenFallbackOffsetY);
            }

            hasCachedPose = true;
        }

        private bool ValidateReadyForTween()
        {
            if (!ValidatePoseCache())
            {
                return false;
            }

            if (cardCanvasGroup == null)
            {
                cardCanvasGroup = cardRoot.gameObject.AddComponent<CanvasGroup>();
            }

            if (activeSequence != null && activeSequence.IsActive())
            {
                activeSequence.Kill();
                activeSequence = null;
            }

            return true;
        }

        private bool ValidatePoseCache()
        {
            if (cardRoot == null)
            {
                Debug.LogError("[SignPanelCardFeelPrototype] 未找到对话框1号。", this);
                return false;
            }

            if (!hasCachedPose)
            {
                CacheActivePose();
            }

            if (!hasCachedPose)
            {
                Debug.LogError("[SignPanelCardFeelPrototype] 未能缓存对话框1号的初始姿态。", this);
                return false;
            }

            return true;
        }

        private void BeginTween()
        {
            KillActiveSequence();
            isTransitionRunning = true;
        }

        private void EndTween()
        {
            activeSequence = null;
            isTransitionRunning = false;
        }

        private void KillActiveSequence()
        {
            if (activeSequence != null && activeSequence.IsActive())
            {
                activeSequence.Kill();
            }

            activeSequence = null;
            isTransitionRunning = false;
        }

        private static RectTransform FindRectTransformByName(string targetName)
        {
            var rectTransforms = Object.FindObjectsByType<RectTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < rectTransforms.Length; i++)
            {
                var rectTransform = rectTransforms[i];
                if (rectTransform != null && rectTransform.name == targetName)
                {
                    return rectTransform;
                }
            }

            return null;
        }
    }
}
