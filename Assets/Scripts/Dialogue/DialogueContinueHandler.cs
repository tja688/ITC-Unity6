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
    }
}
