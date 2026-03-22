using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ITC.Dialogue
{
    public sealed class SignDialoguePlaceholderMinigameBridge
    {
        private readonly RectTransform overlayParent;
        private readonly TMP_Text textTemplate;
        private readonly RectTransform docReviewTarget;
        private readonly RectTransform runeTarget;

        private readonly Key completionKey = Key.Y;
        private readonly float fadeDuration = 0.18f;

        private CanvasGroup overlayGroup;
        private Image overlayImage;
        private TMP_Text overlayText;

        private Tween activeTargetPulseTween;
        private RectTransform activeTarget;
        private Vector3 activeTargetBaseScale;

        public bool IsRunning { get; private set; }

        public SignDialoguePlaceholderMinigameBridge(
            RectTransform overlayParent,
            TMP_Text textTemplate,
            RectTransform docReviewTarget,
            RectTransform runeTarget)
        {
            this.overlayParent = overlayParent;
            this.textTemplate = textTemplate;
            this.docReviewTarget = docReviewTarget;
            this.runeTarget = runeTarget;
        }

        public IEnumerator RunPlaceholderMinigame(string token)
        {
            EnsureOverlay();

            if (IsRunning)
            {
                while (IsRunning)
                {
                    yield return null;
                }

                yield break;
            }

            var target = ResolveMappedTarget(token);

            IsRunning = true;
            overlayGroup.gameObject.SetActive(true);
            overlayGroup.alpha = 0f;
            overlayGroup.blocksRaycasts = true;
            overlayGroup.interactable = true;
            overlayImage.color = ResolvePlaceholderColor(token);
            overlayText.text = BuildOverlayText(token, target);

            BeginTargetPulse(target);
            yield return FadeOverlay(0f, 1f);

            while (IsRunning)
            {
                if (Keyboard.current != null && Keyboard.current[completionKey].wasPressedThisFrame)
                {
                    IsRunning = false;
                }

                yield return null;
            }

            yield return FadeOverlay(1f, 0f);
            StopTargetPulse();
            HideImmediately();
        }

        public void HideImmediately()
        {
            EnsureOverlay();
            overlayGroup.alpha = 0f;
            overlayGroup.blocksRaycasts = false;
            overlayGroup.interactable = false;
            overlayGroup.gameObject.SetActive(false);
            StopTargetPulse();
            IsRunning = false;
        }

        private void EnsureOverlay()
        {
            if (overlayGroup != null)
            {
                return;
            }

            var overlayObject = new GameObject(
                "SignPlaceholderOverlay",
                typeof(RectTransform),
                typeof(CanvasGroup),
                typeof(Image));

            var overlayRect = overlayObject.GetComponent<RectTransform>();
            overlayRect.SetParent(overlayParent, false);
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;
            overlayRect.SetAsLastSibling();

            overlayGroup = overlayObject.GetComponent<CanvasGroup>();
            overlayImage = overlayObject.GetComponent<Image>();
            overlayImage.raycastTarget = true;

            var labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            var labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.SetParent(overlayRect, false);
            labelRect.anchorMin = new Vector2(0.5f, 0.5f);
            labelRect.anchorMax = new Vector2(0.5f, 0.5f);
            labelRect.pivot = new Vector2(0.5f, 0.5f);
            labelRect.sizeDelta = new Vector2(1200f, 320f);

            overlayText = labelObject.GetComponent<TextMeshProUGUI>();
            if (textTemplate != null)
            {
                overlayText.font = textTemplate.font;
                overlayText.fontSharedMaterial = textTemplate.fontSharedMaterial;
                overlayText.fontSize = textTemplate.fontSize;
                overlayText.color = textTemplate.color;
            }

            overlayText.alignment = TextAlignmentOptions.Center;
            overlayText.textWrappingMode = TextWrappingModes.Normal;
            overlayText.raycastTarget = false;

            HideImmediately();
        }

        private IEnumerator FadeOverlay(float from, float to)
        {
            if (fadeDuration <= 0.001f)
            {
                overlayGroup.alpha = to;
                yield break;
            }

            var elapsed = 0f;
            overlayGroup.alpha = from;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                overlayGroup.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / fadeDuration));
                yield return null;
            }

            overlayGroup.alpha = to;
        }

        private RectTransform ResolveMappedTarget(string token)
        {
            var normalized = string.IsNullOrWhiteSpace(token)
                ? string.Empty
                : token.Trim().ToLowerInvariant();

            return normalized switch
            {
                "doc_review" => docReviewTarget,
                "rune" => runeTarget,
                _ => null
            };
        }

        private void BeginTargetPulse(RectTransform target)
        {
            StopTargetPulse();

            if (target == null)
            {
                return;
            }

            activeTarget = target;
            activeTargetBaseScale = target.localScale;
            activeTargetPulseTween = target
                .DOScale(activeTargetBaseScale * 1.06f, 0.35f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);
        }

        private void StopTargetPulse()
        {
            activeTargetPulseTween?.Kill();
            activeTargetPulseTween = null;

            if (activeTarget != null)
            {
                activeTarget.localScale = activeTargetBaseScale == Vector3.zero ? Vector3.one : activeTargetBaseScale;
                activeTarget = null;
            }
        }

        private static Color ResolvePlaceholderColor(string token)
        {
            return string.IsNullOrWhiteSpace(token) ? new Color(0.16f, 0.20f, 0.26f, 0.88f) : token.Trim().ToLowerInvariant() switch
            {
                "doc_review" => new Color(0.20f, 0.16f, 0.26f, 0.88f),
                "rune" => new Color(0.12f, 0.30f, 0.24f, 0.88f),
                _ => new Color(0.16f, 0.20f, 0.26f, 0.88f)
            };
        }

        private string BuildOverlayText(string token, RectTransform target)
        {
            var label = string.IsNullOrWhiteSpace(token) ? "placeholder_minigame" : token.Trim();
            var mappedTargetName = target != null ? target.name : "未映射控件";
            return $"占位小游戏: {label}\n当前联动控件: {mappedTargetName}\n按 {completionKey} 键完成";
        }
    }
}
