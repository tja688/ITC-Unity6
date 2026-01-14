// =============================================================================
// TALinePresenter.cs - Text Animator + Yarn Spinner Integration
// A DialoguePresenter that uses Text Animator for text display and typewriter
// =============================================================================

using System;
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
        private Action onTextShowComplete;
        private System.Threading.Tasks.TaskCompletionSource<bool> currentTextShowCompletionSource;

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

            // 初始隐藏
            if (canvasGroup != null)
                canvasGroup.alpha = 0;
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
        }

        #endregion

        #region DialoguePresenterBase Implementation

        public override async YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
            isShowingLine = true;
            onLineStart?.Invoke();

            // 1. 处理角色名
            string characterName = line.CharacterName;
            if (showCharacterName && characterNameText != null && !string.IsNullOrEmpty(characterName))
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

            // 3. 淡入 UI
            if (useFadeEffect && canvasGroup != null)
            {
                await FadeAlphaAsync(canvasGroup, 0, 1, fadeInDuration, token.HurryUpToken);
            }
            else if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
            }

            // 4. 设置文本并启动打字机
            var textShowCompletionSource = new System.Threading.Tasks.TaskCompletionSource<bool>();
            currentTextShowCompletionSource = textShowCompletionSource;
            onTextShowComplete = () => textShowCompletionSource.TrySetResult(true);
            isSkipping = false;

            typewriter.ShowText(displayText);

            // 5. 注册加速/跳过处理（带兜底）
            // 注意: textShowCompletionSource 只代表"文字已完整呈现或被强制完成"
            // 兜底触发来源: onTextShowed / HurryUpToken(Skip) / OnDisable
            using var hurryUpRegistration = token.HurryUpToken.Register(() =>
            {
                if (isShowingLine && typewriter != null)
                {
                    isSkipping = true;
                    typewriter.SkipTypewriter();
                    // 兜底：确保 completion 被触发，防止死等
                    textShowCompletionSource.TrySetResult(true);
                }
            });

            // 6. 等待文本显示完成
            // 注意: 不注册 NextContentToken，保持语义隔离:
            // - textShowCompletionSource: 文字显示完成
            // - NextContentToken: 用户确认继续
            await textShowCompletionSource.Task;

            isSkipping = false;
            currentTextShowCompletionSource = null;
            onLineFinished?.Invoke();

            // 7. 等待用户确认或自动前进 (第二阶段等待)
            if (autoAdvance)
            {
                // 创建延迟任务
                var delayTask = YarnTask.Delay(TimeSpan.FromSeconds(autoAdvanceDelay), token.NextContentToken);
                await delayTask;
            }
            else
            {
                // 等待用户点击继续
                await YarnTask.WaitUntilCanceled(token.NextContentToken);
            }

            // 8. 淡出 UI
            if (useFadeEffect && canvasGroup != null)
            {
                await FadeAlphaAsync(canvasGroup, 1, 0, fadeOutDuration, token.HurryUpToken);
            }
            else if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }

            isShowingLine = false;
        }

        public override YarnTask OnDialogueStartedAsync()
        {
            // 对话开始时可以进行初始化
            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueCompleteAsync()
        {
            // 对话结束时隐藏 UI
            if (canvasGroup != null)
                canvasGroup.alpha = 0;

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
            onTextShowComplete?.Invoke();
            onTextShowComplete = null;
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

        #endregion
    }
}
