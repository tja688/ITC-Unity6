// =============================================================================
// TALinePresenter.cs - Text Animator + Yarn Spinner Integration
// A DialoguePresenter that uses Text Animator for text display and typewriter
// =============================================================================

using System;
using System.Collections.Generic;
using System.Threading;
using Febucci.TextAnimatorForUnity;
using Febucci.TextAnimatorForUnity.TextMeshPro;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace ITC.Dialogue
{
    /// <summary>
    /// 基于 Text Animator 的 Yarn Spinner 行呈现器
    /// 替代默认的 LinePresenter，使用 Text Animator 的打字机和动画效果
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class TALinePresenter : DialoguePresenterBase
    {
        #region Serialized Fields

        [Header("Text Animator 组件")]
        [SerializeField] private TextAnimator_TMP textAnimator;
        [SerializeField] private TypewriterComponent typewriter;

        [Header("UI 元素")]
        [SerializeField] private TMP_Text lineText;
        [SerializeField] private TMP_Text characterNameText;
        [SerializeField] private CanvasGroup canvasGroup;

        [Tooltip("可选：单独用于文本区域的 CanvasGroup，用于行间过渡。如果设置，行间切换只淡入淡出文本，不影响角色名和按钮")]
        [SerializeField] private CanvasGroup lineTextCanvasGroup;

        [Header("Sign Scene Routing")]
        [SerializeField] private bool routeToSignSlots = false;
        [SerializeField] private SignDialogueSlotRuntime signDialogueSlotRuntime;

        [Header("显示设置")]
        [SerializeField] private bool showCharacterName = true;
        [SerializeField] private bool useFadeEffect = true;
        [SerializeField] private float fadeInDuration = 0.25f;
        [SerializeField] private float fadeOutDuration = 0.15f;

        [Header("自动前进")]
        [SerializeField] private bool autoAdvance = false;
        [SerializeField] private float autoAdvanceDelay = 2f;

        [Header("事件")]
        public UnityEvent onLineStart;
        public UnityEvent onLineFinished;
        public UnityEvent<string> onCharacterNameChanged;

        #endregion

        #region Private Fields

        private bool isShowingLine = false;
        private bool isSkipping = false;
        private bool isTextFullyShown = false;
        private bool isFirstLineOfDialogue = true; // 用于区分首行和后续行的淡入淡出
        private Action onTextShowComplete;
        private System.Threading.Tasks.TaskCompletionSource<bool> currentTextShowCompletionSource;
        private string[] baselineAppearanceTags = Array.Empty<string>();
        private string[] baselineDisappearanceTags = Array.Empty<string>();

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            // 自动获取组件
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            if (textAnimator == null && lineText != null)
                textAnimator = lineText.GetComponent<TextAnimator_TMP>();

            if (typewriter == null)
                typewriter = GetComponent<TypewriterComponent>();

            if (signDialogueSlotRuntime == null)
            {
                signDialogueSlotRuntime = FindFirstObjectByType<SignDialogueSlotRuntime>();
            }

            CacheBaselineEffectTags();

            // 初始隐藏
            if (canvasGroup != null)
            {
                canvasGroup.alpha = IsSignLegacyVisualSuppressed() ? 1f : 0f;
            }

            if (lineTextCanvasGroup != null && IsSignLegacyVisualSuppressed())
            {
                lineTextCanvasGroup.alpha = 0f;
            }
        }

        private void OnEnable()
        {
            if (typewriter != null)
            {
                typewriter.onTextShowed.AddListener(OnTextShowedHandler);
            }
        }

        private void OnDisable()
        {
            if (typewriter != null)
            {
                typewriter.onTextShowed.RemoveListener(OnTextShowedHandler);
            }

            // 兜底：场景切换或对象 Disable 时确保不会死等
            currentTextShowCompletionSource?.TrySetResult(true);
            isShowingLine = false;
            isSkipping = false;
            isTextFullyShown = false;
        }

        #endregion

        #region DialoguePresenterBase Implementation

        public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
            isShowingLine = true;
            isTextFullyShown = false;
            onLineStart?.Invoke();

            bool useSignRouting = routeToSignSlots &&
                                  signDialogueSlotRuntime != null &&
                                  signDialogueSlotRuntime.IsRoutingEnabled;
            bool suppressLegacyVisual = useSignRouting &&
                                        signDialogueSlotRuntime.ShouldSuppressLegacyPresenterVisuals;
            bool useLegacyTextAnimator = !suppressLegacyVisual && textAnimator != null && typewriter != null;

            if (textAnimator != null)
            {
                textAnimator.enabled = useLegacyTextAnimator;
            }

            if (typewriter != null)
            {
                typewriter.enabled = useLegacyTextAnimator;
            }

            // 1. 处理角色名
            string characterName = line.CharacterName;
            if (!suppressLegacyVisual && showCharacterName && characterNameText != null && !string.IsNullOrEmpty(characterName))
            {
                characterNameText.text = characterName;
                characterNameText.gameObject.SetActive(true);
                onCharacterNameChanged?.Invoke(characterName);
            }
            else if (characterNameText != null)
            {
                characterNameText.gameObject.SetActive(false);
            }

            // 2. 获取文本内容（移除角色名前缀）
            string displayText = line.TextWithoutCharacterName.Text;
            string normalizedDisplayText = ExtractLeadingPipeTags(
                displayText,
                out var lineAppearanceTags,
                out var lineDisappearanceTags);
            ApplyLineDefaultTags(lineAppearanceTags, lineDisappearanceTags);

            if (useSignRouting)
            {
                signDialogueSlotRuntime.RouteLine(characterName, normalizedDisplayText);
            }

            // 3. 设置文本（隐藏），避免淡入时显示上一行文字
            var textShowCompletionSource = new System.Threading.Tasks.TaskCompletionSource<bool>();
            currentTextShowCompletionSource = textShowCompletionSource;
            onTextShowComplete = () => textShowCompletionSource.TrySetResult(true);
            isSkipping = false;

            if (useLegacyTextAnimator)
            {
                // 快进优化：如果处于快进模式，禁用渐入特效，确保文字能立即被看见
                if (DialogueContinueHandler.IsFastForwarding)
                {
                    textAnimator.SetAppearancesActive(false);
                }

                // 使用 TextAnimator 设置文本并启动打字机
                textAnimator.SetText(normalizedDisplayText, true);
            }
            else
            {
                // SignSlot 路由下，显示逻辑由 SignDialogueSlotRuntime 托管。
                textShowCompletionSource.TrySetResult(true);
            }

            // 4. 淡入 UI
            // 快进优化：快进模式下不进行渐入动画
            float actualFadeInDuration = DialogueContinueHandler.IsFastForwarding ? 0 : fadeInDuration;

            if (suppressLegacyVisual)
            {
                if (canvasGroup != null)
                {
                    // SignSlot 路由模式下只隐藏 legacy 文本层，不能隐藏整个 Panel。
                    canvasGroup.alpha = 1f;
                }

                if (lineTextCanvasGroup != null)
                {
                    lineTextCanvasGroup.alpha = 0f;
                }
            }
            else if (useFadeEffect && canvasGroup != null)
            {
                if (isFirstLineOfDialogue)
                {
                    // 首次：淡入整个面板
                    if (lineTextCanvasGroup != null)
                        lineTextCanvasGroup.alpha = 1; // 确保文本区域可见
                    await FadeAlphaAsync(canvasGroup, 0, 1, actualFadeInDuration, token.HurryUpToken);
                    isFirstLineOfDialogue = false;
                }
                else if (lineTextCanvasGroup != null)
                {
                    // 后续行：只淡入文本区域
                    await FadeAlphaAsync(lineTextCanvasGroup, 0, 1, actualFadeInDuration, token.HurryUpToken);
                }
            }
            else if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
                if (lineTextCanvasGroup != null)
                    lineTextCanvasGroup.alpha = 1;
            }

            // 5. 启动打字机
            if (useLegacyTextAnimator)
            {
                typewriter.StartShowingText(true);

                // 5.1 边界情况：纯标签无可见文本
                if (textAnimator.CharactersCount == 0)
                {
                    textShowCompletionSource.TrySetResult(true);
                }
            }

            // 6. 注册加速/跳过处理
            using var hurryUpRegistration = token.HurryUpToken.Register(() =>
            {
                if (!isShowingLine)
                {
                    return;
                }

                if (useLegacyTextAnimator && typewriter != null)
                {
                    isSkipping = true;
                    typewriter.SkipTypewriter();
                    isTextFullyShown = true;
                    textShowCompletionSource.TrySetResult(true);
                }
                else
                {
                    isTextFullyShown = true;
                    textShowCompletionSource.TrySetResult(true);
                }
            });

            // 7. 等待文本显示完成
            await textShowCompletionSource.Task;

            // 恢复特效状态
            if (useLegacyTextAnimator)
            {
                textAnimator.SetAppearancesActive(true);
            }

            isTextFullyShown = true;
            isSkipping = false;
            currentTextShowCompletionSource = null;
            onLineFinished?.Invoke();

            // 8. 等待用户确认或自动前进
            if (autoAdvance && !DialogueContinueHandler.IsFastForwarding)
            {
                var delayTask = YarnTask.Delay(TimeSpan.FromSeconds(autoAdvanceDelay), token.NextContentToken);
                await delayTask;
            }
            else
            {
                // 等待用户点击继续（快进时，由 DialogueContinueHandler 驱动 token.NextContentToken 取消）
                await YarnTask.WaitUntilCanceled(token.NextContentToken);
            }

            // 9. 淡出 UI
            // 快进优化：快进模式下不进行渐出动画
            float actualFadeOutDuration = DialogueContinueHandler.IsFastForwarding ? 0 : fadeOutDuration;

            if (suppressLegacyVisual)
            {
                if (canvasGroup != null)
                {
                    // SignSlot 路由模式下保持整体可见，只关闭 legacy 文本层。
                    canvasGroup.alpha = 1f;
                }

                if (lineTextCanvasGroup != null)
                {
                    lineTextCanvasGroup.alpha = 0f;
                }
            }
            else if (useFadeEffect && lineTextCanvasGroup != null)
            {
                await FadeAlphaAsync(lineTextCanvasGroup, 1, 0, actualFadeOutDuration, token.HurryUpToken);
            }
            else if (useFadeEffect && canvasGroup != null)
            {
                await FadeAlphaAsync(canvasGroup, 1, 0, actualFadeOutDuration, token.HurryUpToken);
            }
            else if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }



            isShowingLine = false;
            isTextFullyShown = false;
        }

        public override YarnTask OnDialogueStartedAsync()
        {
            // 对话开始时重置状态
            isFirstLineOfDialogue = true;
            if (IsSignLegacyVisualSuppressed())
            {
                if (canvasGroup != null)
                    canvasGroup.alpha = 1f;

                if (lineTextCanvasGroup != null)
                    lineTextCanvasGroup.alpha = 0f;
            }

            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueCompleteAsync()
        {
            // 对话结束时隐藏 UI
            if (IsSignLegacyVisualSuppressed())
            {
                if (canvasGroup != null)
                    canvasGroup.alpha = 1f;

                if (lineTextCanvasGroup != null)
                    lineTextCanvasGroup.alpha = 0f;
            }
            else if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }

            return YarnTask.CompletedTask;
        }

        public override YarnTask<DialogueOption?> RunOptionsAsync(DialogueOption[] dialogueOptions, CancellationToken cancellationToken)
        {
            // 此组件不处理选项，返回 null 让其他 Presenter 处理
            return DialogueRunner.NoOptionSelected;
        }

        #endregion

        #region Private Methods

        private void OnTextShowedHandler()
        {
            isTextFullyShown = true;
            onTextShowComplete?.Invoke();
            onTextShowComplete = null;
        }

        private void CacheBaselineEffectTags()
        {
            if (textAnimator == null)
            {
                return;
            }

            var appearanceTags = textAnimator.localSettings.defaultAppearanceTags;
            var disappearanceTags = textAnimator.localSettings.defaultDisappearanceTags;

            baselineAppearanceTags = appearanceTags != null
                ? (string[])appearanceTags.Clone()
                : Array.Empty<string>();
            baselineDisappearanceTags = disappearanceTags != null
                ? (string[])disappearanceTags.Clone()
                : Array.Empty<string>();
        }

        private void ApplyLineDefaultTags(List<string> lineAppearanceTags, List<string> lineDisappearanceTags)
        {
            if (textAnimator == null)
            {
                return;
            }

            var appearanceTags = new List<string>(baselineAppearanceTags.Length + lineAppearanceTags.Count);
            var disappearanceTags = new List<string>(baselineDisappearanceTags.Length + lineDisappearanceTags.Count);

            AppendUnique(appearanceTags, baselineAppearanceTags);
            AppendUnique(disappearanceTags, baselineDisappearanceTags);
            AppendUnique(appearanceTags, lineAppearanceTags);
            AppendUnique(disappearanceTags, lineDisappearanceTags);

            textAnimator.localSettings.defaultAppearanceTags = appearanceTags.ToArray();
            textAnimator.localSettings.defaultDisappearanceTags = disappearanceTags.ToArray();
        }

        private static string ExtractLeadingPipeTags(string text, out List<string> lineAppearanceTags, out List<string> lineDisappearanceTags)
        {
            lineAppearanceTags = new List<string>(2);
            lineDisappearanceTags = new List<string>(1);

            if (string.IsNullOrEmpty(text) || text[0] != '|')
            {
                return text;
            }

            if (!TryParseLeadingPipeTags(text, out var openingTags, out int contentStart))
            {
                return text;
            }

            foreach (var openingTag in openingTags)
            {
                if (ShouldIgnoreLeadingPipeTag(openingTag.Name))
                {
                    continue;
                }

                if (openingTag.IsDisappearance)
                {
                    AppendUnique(lineDisappearanceTags, openingTag.Name);
                }
                else
                {
                    AppendUnique(lineAppearanceTags, openingTag.Name);
                }
            }

            return text.Substring(contentStart);
        }

        private bool IsSignLegacyVisualSuppressed()
        {
            return routeToSignSlots &&
                   signDialogueSlotRuntime != null &&
                   signDialogueSlotRuntime.IsRoutingEnabled &&
                   signDialogueSlotRuntime.ShouldSuppressLegacyPresenterVisuals;
        }

        private static bool TryParseLeadingPipeTags(string text, out List<PipeOpeningTag> openingTags, out int contentStart)
        {
            openingTags = new List<PipeOpeningTag>(2);
            contentStart = 0;

            while (contentStart < text.Length && text[contentStart] == '|')
            {
                int closeIndex = text.IndexOf('|', contentStart + 1);
                if (closeIndex <= contentStart + 1)
                {
                    break;
                }

                string tagContent = text.Substring(contentStart + 1, closeIndex - contentStart - 1);
                if (!TryParsePipeOpeningTag(tagContent, out var openingTag))
                {
                    break;
                }

                openingTags.Add(openingTag);
                contentStart = closeIndex + 1;
            }

            return openingTags.Count > 0;
        }

        private static bool TryParsePipeOpeningTag(string tagContent, out PipeOpeningTag openingTag)
        {
            openingTag = default;

            if (string.IsNullOrWhiteSpace(tagContent))
            {
                return false;
            }

            string normalized = tagContent.Trim();
            if (normalized.StartsWith("/", StringComparison.Ordinal))
            {
                return false;
            }

            bool isDisappearance = normalized.StartsWith("#", StringComparison.Ordinal);
            if (isDisappearance)
            {
                normalized = normalized.Substring(1);
            }

            if (normalized.Length == 0)
            {
                return false;
            }

            int parameterStart = normalized.IndexOf(' ');
            string candidateName = parameterStart < 0
                ? normalized
                : normalized.Substring(0, parameterStart);

            if (!IsValidPipeTagName(candidateName))
            {
                return false;
            }

            openingTag = new PipeOpeningTag(candidateName, isDisappearance);
            return true;
        }

        private static bool IsValidPipeTagName(string candidateName)
        {
            if (string.IsNullOrWhiteSpace(candidateName))
            {
                return false;
            }

            if (!char.IsLetter(candidateName[0]))
            {
                return false;
            }

            for (int i = 0; i < candidateName.Length; i++)
            {
                char c = candidateName[i];
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '-')
                {
                    return false;
                }
            }

            return true;
        }

        private static bool ShouldIgnoreLeadingPipeTag(string tagName)
        {
            return tagName.Equals("typewriter", StringComparison.OrdinalIgnoreCase);
        }

        private static void AppendUnique(List<string> target, IEnumerable<string> source)
        {
            if (source == null)
            {
                return;
            }

            foreach (var tag in source)
            {
                AppendUnique(target, tag);
            }
        }

        private static void AppendUnique(List<string> target, string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return;
            }

            for (int i = 0; i < target.Count; i++)
            {
                if (string.Equals(target[i], tag, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }

            target.Add(tag);
        }

        private readonly struct PipeOpeningTag
        {
            public PipeOpeningTag(string name, bool isDisappearance)
            {
                Name = name;
                IsDisappearance = isDisappearance;
            }

            public string Name { get; }
            public bool IsDisappearance { get; }
        }

        /// <summary>
        /// 异步淡入淡出效果
        /// </summary>
        private static async YarnTask FadeAlphaAsync(CanvasGroup canvasGroup, float from, float to, float duration, CancellationToken cancellationToken)
        {
            if (duration <= 0 || canvasGroup == null)
            {
                if (canvasGroup != null) canvasGroup.alpha = to;
                return;
            }

            float elapsed = 0f;
            canvasGroup.alpha = from;

            while (elapsed < duration && !cancellationToken.IsCancellationRequested)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                canvasGroup.alpha = Mathf.Lerp(from, to, t);
                await YarnTask.Yield();
            }

            canvasGroup.alpha = to;
        }

        #endregion

        #region Public API

        /// <summary>
        /// 手动跳过当前行的打字机效果
        /// </summary>
        public void SkipCurrentLine()
        {
            if (isShowingLine && typewriter != null)
            {
                typewriter.SkipTypewriter();
            }
        }

        /// <summary>
        /// 检查是否正在显示文本
        /// </summary>
        public bool IsShowingLine => isShowingLine;

        /// <summary>
        /// 检查是否正在跳过 (Skip 期间为 true，可用于事件过滤)
        /// </summary>
        public bool IsSkipping => isSkipping;

        /// <summary>
        /// 当前行文字是否已经完整显示
        /// </summary>
        public bool IsTextFullyShown => isTextFullyShown;

        #endregion
    }
}
