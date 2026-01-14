// =============================================================================
// DOTweenEventHandler.cs - 处理 Yarn 对话事件触发 DOTween 动画
// =============================================================================

using DG.Tweening;
using UnityEngine;

namespace ITC.Dialogue
{
    /// <summary>
    /// 监听 TADialogueEvents 的自定义消息，触发 DOTweenAnimation 播放
    /// </summary>
    public class DOTweenEventHandler : MonoBehaviour
    {
        [Header("事件配置")]
        [Tooltip("监听的事件名称（如 dotween、playtween 等）")]
        [SerializeField] private string eventName = "dotween";

        [Header("目标动画")]
        [Tooltip("要控制的 DOTweenAnimation 组件")]
        [SerializeField] private DOTweenAnimation targetAnimation;

        [Header("事件源")]
        [Tooltip("对话事件桥接器（自动查找）")]
        [SerializeField] private TADialogueEvents dialogueEvents;

        private void Awake()
        {
            // 自动查找组件
            if (dialogueEvents == null)
                dialogueEvents = FindFirstObjectByType<TADialogueEvents>();

            // 自动查找同一 GameObject 上的 DOTweenAnimation
            if (targetAnimation == null)
                targetAnimation = GetComponent<DOTweenAnimation>();
        }

        private void OnEnable()
        {
            if (dialogueEvents != null)
            {
                dialogueEvents.onCustomMessage.AddListener(OnCustomMessage);
            }
        }

        private void OnDisable()
        {
            if (dialogueEvents != null)
            {
                dialogueEvents.onCustomMessage.RemoveListener(OnCustomMessage);
            }
        }

        private void OnCustomMessage(string message)
        {
            // 解析事件名和参数
            // 格式: eventName 或 eventName=param1,param2
            string name = message;
            string[] parameters = null;

            int equalsIndex = message.IndexOf('=');
            if (equalsIndex >= 0)
            {
                name = message.Substring(0, equalsIndex);
                parameters = message.Substring(equalsIndex + 1).Split(',');
            }

            // 检查是否匹配我们监听的事件
            if (!string.Equals(name, eventName, System.StringComparison.OrdinalIgnoreCase))
                return;

            // 触发 DOTween 动画
            PlayDOTweenAnimation(parameters);
        }

        private void PlayDOTweenAnimation(string[] parameters)
        {
            if (targetAnimation == null)
            {
                Debug.LogWarning($"[DOTweenEventHandler] 目标动画未设置！");
                return;
            }

            // 默认动作是 restart（重新播放，最常用）
            string action = (parameters != null && parameters.Length > 0)
                ? parameters[0].Trim().ToLower()
                : "restart";

            switch (action)
            {
                case "play":
                    // DOPlay 只有在动画暂停时才会恢复播放，如果动画已完成则不会重新开始
                    targetAnimation.DOPlay();
                    Debug.Log($"[DOTweenEventHandler] DOTween 动画播放/恢复");
                    break;

                case "restart":
                    // DORestart 会重置动画并从头开始播放
                    targetAnimation.DORestart();
                    Debug.Log($"[DOTweenEventHandler] DOTween 动画重新开始");
                    break;

                case "rewind":
                    // 倒回到起始位置
                    targetAnimation.DORewind();
                    Debug.Log($"[DOTweenEventHandler] DOTween 动画倒回");
                    break;

                case "pause":
                    targetAnimation.DOPause();
                    Debug.Log($"[DOTweenEventHandler] DOTween 动画暂停");
                    break;

                case "toggle":
                    targetAnimation.DOTogglePause();
                    Debug.Log($"[DOTweenEventHandler] DOTween 动画切换暂停");
                    break;

                case "complete":
                    // 立即完成动画（跳到结束状态）
                    targetAnimation.DOComplete();
                    Debug.Log($"[DOTweenEventHandler] DOTween 动画完成");
                    break;

                case "kill":
                    // 停止并销毁动画
                    targetAnimation.DOKill();
                    Debug.Log($"[DOTweenEventHandler] DOTween 动画销毁");
                    break;

                default:
                    // 未知指令默认 restart
                    targetAnimation.DORestart();
                    Debug.Log($"[DOTweenEventHandler] 未知指令 '{action}'，执行 restart");
                    break;
            }
        }
    }
}
