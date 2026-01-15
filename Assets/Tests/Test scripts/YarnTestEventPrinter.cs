using ITC.Dialogue;
using UnityEngine;

namespace ITC.Tests
{
    /// <summary>
    /// 用于测试 Yarn 触发事件的脚本。
    /// 接收到特定的自定义消息时，在控制台快速打印 1-9。
    /// </summary>
    public class YarnTestEventPrinter : MonoBehaviour
    {
        [Header("引用的事件组件 (可选，不选则自动查找)")]
        [SerializeField] private TADialogueEvents dialogueEvents;

        private void OnEnable()
        {
            // 如果没有手动指定，则尝试在场景中查找
            if (dialogueEvents == null)
            {
                dialogueEvents = Object.FindFirstObjectByType<TADialogueEvents>();
            }

            if (dialogueEvents != null)
            {
                dialogueEvents.onCustomMessage.AddListener(HandleCustomMessage);
                Debug.Log("[YarnTestEventPrinter] 已成功订阅 TADialogueEvents。");
            }
            else
            {
                Debug.LogError("[YarnTestEventPrinter] 未能在场景中找到 TADialogueEvents 组件，请检查场景配置。");
            }
        }

        private void OnDisable()
        {
            if (dialogueEvents != null)
            {
                dialogueEvents.onCustomMessage.RemoveListener(HandleCustomMessage);
            }
        }

        /// <summary>
        /// 处理来自 Text Animator (通过 TADialogueEvents 转发) 的自定义消息
        /// </summary>
        /// <param name="message">事件名称</param>
        private void HandleCustomMessage(string message)
        {
            // 匹配我们在 Yarn 中定义的事件名
            if (message == "test_print_1_to_9")
            {
                ExecutePrintTask();
            }
        }

        /// <summary>
        /// 执行打印 1-9 的任务
        /// </summary>
        private void ExecutePrintTask()
        {
            Debug.Log("<color=cyan>[YarnTestEvent] 开始快速顺序打印 1-9：</color>");

            for (int i = 1; i <= 9; i++)
            {
                // 分开打印，每个数字一行
                Debug.Log($"Test Message: {i}");
            }

            Debug.Log("<color=cyan>[YarnTestEvent] 打印任务完成。</color>");
        }
    }
}
