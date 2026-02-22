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

        [Header("输入设置")]
        [SerializeField] private bool useMouseClick = true;
        [SerializeField] private bool useKeyboard = true;
        [SerializeField] private Key continueKey = Key.Space;
        [SerializeField] private Key skipKey = Key.Enter;

        [Header("快进设置")]
        [SerializeField] private bool useFastForward = true;
        [SerializeField] private Key fastForwardKey = Key.LeftCtrl;
        [SerializeField] private float fastForwardInterval = 0.05f;
        private float fastForwardTimer = 0f;

        [Header("UI 按钮（可选）")]
        [SerializeField] private UnityEngine.UI.Button continueButton;
        [SerializeField] private UnityEngine.UI.Button skipButton;

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
                return;

            // 快进功能
            if (useFastForward && Keyboard.current != null)
            {
                // 检测是否长按对应的快进键或者左/右Ctrl键
                if (Keyboard.current[fastForwardKey].isPressed ||
                    Keyboard.current[Key.LeftCtrl].isPressed ||
                    Keyboard.current[Key.RightCtrl].isPressed)
                {
                    fastForwardTimer += Time.deltaTime;
                    if (fastForwardTimer >= fastForwardInterval)
                    {
                        fastForwardTimer = 0f;
                        RequestFastForwardContinue();
                    }
                    return; // 快进时跳过其他常规输入检测
                }
                else
                {
                    fastForwardTimer = 0f;
                }
            }

            // 键盘输入
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

        private void OnContinueClicked()
        {
            RequestContinue();
        }

        private void OnSkipClicked()
        {
            RequestSkip();
        }

        /// <summary>
        /// 请求继续对话
        /// </summary>
        public void RequestContinue()
        {
            if (dialogueRunner == null) return;

            // 如果正在显示文本，先跳过打字机
            if (linePresenter != null && linePresenter.IsShowingLine)
            {
                if (linePresenter.IsTextFullyShown)
                {
                    dialogueRunner.RequestNextLine();
                }
                else
                {
                    linePresenter.SkipCurrentLine();
                }
                return;
            }

            // 否则请求继续
            dialogueRunner.RequestNextLine();
        }

        /// <summary>
        /// 请求跳过当前打字机效果
        /// </summary>
        public void RequestSkip()
        {
            if (dialogueRunner == null) return;

            dialogueRunner.RequestHurryUpLine();
        }

        /// <summary>
        /// 专门用于长按快进的继续逻辑
        /// </summary>
        private void RequestFastForwardContinue()
        {
            if (dialogueRunner == null) return;

            if (linePresenter != null && linePresenter.IsShowingLine)
            {
                if (!linePresenter.IsTextFullyShown)
                {
                    // 如果尚未全显示，先跳过当前打字机
                    linePresenter.SkipCurrentLine();
                }

                // 跳过后，再次检查是否全显示并立即跳下一句，以达到高速效果
                if (linePresenter.IsTextFullyShown)
                {
                    dialogueRunner.RequestNextLine();
                }
                return;
            }

            // 防止对话不在显示行时停滞，也直接请求下一行
            dialogueRunner.RequestNextLine();
        }
    }
}
