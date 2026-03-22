// =============================================================================
// DialogueContinueHandler.cs - Input Handler for Dialogue Continuation
// Handles user input to continue dialogue (click/keyboard)
// =============================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;

namespace ITC.Dialogue
{
    /// <summary>
    /// 对话继续按钮/输入处理器
    /// 监听用户输入并请求对话继续
    /// </summary>
    public class DialogueContinueHandler : MonoBehaviour
    {
        [Header("对话系统")]
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private TALinePresenter linePresenter;
        [SerializeField] private SignDialogueRuntimeFacade signDialogueRuntimeFacade;
        [SerializeField] private SignDialogueSlotRuntime signDialogueSlotRuntime;

        [Header("输入设置")]
        [SerializeField] private bool useMouseClick = true;
        [SerializeField] private bool useKeyboard = true;
        [SerializeField] private Key continueKey = Key.Space;
        [SerializeField] private Key skipKey = Key.Enter;

        [Header("快进设置")]
        [SerializeField] private bool useFastForward = true;
        [SerializeField] private Key fastForwardKey = Key.LeftCtrl;
        [SerializeField] private float fastForwardInterval = 0.05f; // 旧逻辑保留兼容

        [Header("稳定快进设置")]
        [Tooltip("跳过打字机前等待的时间，确保能看见开头的字符 (秒)")]
        [SerializeField] private float ffMinShowTime = 0.12f;
        [Tooltip("全显示后等待多久跳到下一行 (秒)")]
        [SerializeField] private float ffNextLineDelay = 0.05f;

        [Header("UI 按钮（可选）")]
        [SerializeField] private UnityEngine.UI.Button continueButton;
        [SerializeField] private UnityEngine.UI.Button skipButton;

        private float fastForwardTimer = 0f;
        private float currentLineStateTime = 0f;
        private bool wasShowingLineLastFrame = false;
        private bool wasLineFullyShownLastFrame = false;

        /// <summary>
        /// 全局快进状态，供 Presenter 等组件查询以优化外观效果
        /// </summary>
        public static bool IsFastForwarding { get; private set; }

        private void Start()
        {
            // 自动查找 DialogueRunner
            if (dialogueRunner == null)
            {
                dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            }

            // 自动查找 TALinePresenter
            if (linePresenter == null)
            {
                linePresenter = FindFirstObjectByType<TALinePresenter>();
            }

            ResolveSignRuntimeReferences();

            // 绑定按钮事件
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked);
            }

            // 绑定 Skip 按钮事件
            if (skipButton != null)
            {
                skipButton.onClick.AddListener(OnSkipClicked);
            }
        }

        private void Update()
        {
            if (dialogueRunner == null || !dialogueRunner.IsDialogueRunning)
            {
                IsFastForwarding = false;
                return;
            }

            // 更新快进状态
            UpdateFastForwardStatus();

            if (IsFastForwarding)
            {
                if (IsContinueBlocked())
                {
                    return;
                }

                HandleFastForwardRhythm();
                return; // 快进时跳过其他常规输入检测
            }

            if (IsContinueBlocked())
            {
                return;
            }

            // 重置状态计时
            currentLineStateTime = 0f;
            wasShowingLineLastFrame = false;
            wasLineFullyShownLastFrame = false;

            // 键盘常规输入
            if (useKeyboard)
            {
                if (Keyboard.current != null)
                {
                    // 继续对话
                    if (Keyboard.current[continueKey].wasPressedThisFrame)
                    {
                        RequestContinue();
                    }

                    // 跳过打字机
                    if (Keyboard.current[skipKey].wasPressedThisFrame)
                    {
                        RequestSkip();
                    }
                }
            }

            // 鼠标点击
            if (useMouseClick)
            {
                if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
                {
                    RequestContinue();
                }
            }
        }

        private void UpdateFastForwardStatus()
        {
            if (!useFastForward || Keyboard.current == null)
            {
                IsFastForwarding = false;
                return;
            }

            IsFastForwarding = Keyboard.current[fastForwardKey].isPressed ||
                               Keyboard.current[Key.LeftCtrl].isPressed ||
                               Keyboard.current[Key.RightCtrl].isPressed;
        }

        /// <summary>
        /// 处理稳定的快进节奏
        /// </summary>
        private void HandleFastForwardRhythm()
        {
            if (linePresenter == null)
            {
                // 回退到简单逻辑
                fastForwardTimer += Time.deltaTime;
                if (fastForwardTimer >= fastForwardInterval)
                {
                    fastForwardTimer = 0f;
                    RequestContinueOrNext();
                }
                return;
            }

            bool isShowing = linePresenter.IsShowingLine;
            bool isFullyShown = linePresenter.IsTextFullyShown;

            // 状态切换检测
            if (isShowing != wasShowingLineLastFrame || isFullyShown != wasLineFullyShownLastFrame)
            {
                currentLineStateTime = 0f;
            }
            else
            {
                currentLineStateTime += Time.deltaTime;
            }

            wasShowingLineLastFrame = isShowing;
            wasLineFullyShownLastFrame = isFullyShown;

            if (isShowing)
            {
                if (!isFullyShown)
                {
                    // 阶段 1: 文本显示中。等待一小会儿（出俩字）再跳过。
                    if (currentLineStateTime >= ffMinShowTime)
                    {
                        linePresenter.SkipCurrentLine();
                        // 注意：跳过会立即让 isFullyShown 变为 true (在下帧或本帧)
                    }
                }
                else
                {
                    // 阶段 2: 文本已全显示。等待极短时间后再跳下一句，保持节奏感。
                    if (currentLineStateTime >= ffNextLineDelay)
                    {
                        dialogueRunner.RequestNextLine();
                        currentLineStateTime = 0f; // 重置以防连续请求
                    }
                }
            }
            else
            {
                // 阶段 3: 对话间隙（如等待指令或过渡）。持续请求以尽快进入下一行。
                dialogueRunner.RequestNextLine();
            }
        }

        private void RequestContinueOrNext()
        {
            if (IsContinueBlocked())
            {
                return;
            }

            if (linePresenter != null && linePresenter.IsShowingLine)
            {
                if (!linePresenter.IsTextFullyShown)
                    linePresenter.SkipCurrentLine();
                else
                    dialogueRunner.RequestNextLine();
            }
            else
            {
                dialogueRunner.RequestNextLine();
            }
        }

        private void OnContinueClicked()
        {
            RequestContinue();
        }

        private void OnSkipClicked()
        {
            RequestSkip();
        }

        public void RequestContinue()
        {
            if (dialogueRunner == null) return;
            if (IsContinueBlocked()) return;

            if (linePresenter != null && linePresenter.IsShowingLine)
            {
                if (linePresenter.IsTextFullyShown)
                    dialogueRunner.RequestNextLine();
                else
                    linePresenter.SkipCurrentLine();
                return;
            }

            dialogueRunner.RequestNextLine();
        }

        public void RequestSkip()
        {
            if (dialogueRunner == null) return;
            if (IsContinueBlocked()) return;
            dialogueRunner.RequestHurryUpLine();
        }

        private bool IsContinueBlocked()
        {
            ResolveSignRuntimeReferences();

            if (signDialogueRuntimeFacade != null && signDialogueRuntimeFacade.IsRoutingEnabled)
            {
                return signDialogueRuntimeFacade.IsContinueInputBlocked;
            }

            return signDialogueSlotRuntime != null && signDialogueSlotRuntime.IsContinueInputBlocked;
        }

        private void ResolveSignRuntimeReferences()
        {
            if (signDialogueRuntimeFacade == null)
            {
                signDialogueRuntimeFacade = FindFirstObjectByType<SignDialogueRuntimeFacade>();
            }

            if (signDialogueSlotRuntime == null)
            {
                signDialogueSlotRuntime = FindFirstObjectByType<SignDialogueSlotRuntime>();
            }
        }
    }
}

