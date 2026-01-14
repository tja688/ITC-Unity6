// =============================================================================
// TADialogueEvents.cs - Event Bridge for Text Animator and Yarn Spinner
// Provides Unity Events for dialogue-related triggers (audio, animations, etc.)
// =============================================================================

using Febucci.TextAnimatorCore.Text;
using Febucci.TextAnimatorCore.Typing;
using Febucci.TextAnimatorForUnity;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

namespace ITC.Dialogue
{
    /// <summary>
    /// Skip 期间事件过滤模式
    /// </summary>
    public enum SkipEventFilterMode
    {
        /// <summary>黑名单模式：只屏蔽指定的事件</summary>
        Blacklist,
        /// <summary>白名单模式：只允许指定的事件</summary>
        Whitelist,
        /// <summary>全部屏蔽：Skip 期间不触发任何事件</summary>
        BlockAll
    }

    /// <summary>
    /// 对话事件桥接器
    /// 将 Text Animator 的事件转发到游戏系统
    /// </summary>
    public class TADialogueEvents : MonoBehaviour
    {
        [Header("组件引用")]
        [SerializeField] private TypewriterComponent typewriter;
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private TALinePresenter linePresenter;

        [Header("打字机事件")]
        [Tooltip("每个字符显示时触发")]
        public UnityEvent<char> onCharacterShown;

        [Tooltip("打字机完成时触发")]
        public UnityEvent onTypewriterComplete;

        [Tooltip("自定义消息事件（来自 <?event> 标签）")]
        public UnityEvent<string> onCustomMessage;

        [Header("对话事件")]
        [Tooltip("对话开始时触发")]
        public UnityEvent onDialogueStarted;

        [Tooltip("对话结束时触发")]
        public UnityEvent onDialogueEnded;

        [Tooltip("节点开始时触发")]
        public UnityEvent<string> onNodeStarted;

        [Tooltip("节点结束时触发")]
        public UnityEvent<string> onNodeEnded;

        [Header("音频设置")]
        [SerializeField] private bool playTypingSounds = true;
        [SerializeField] private AudioSource typingAudioSource;
        [SerializeField] private AudioClip[] typingSounds;
        [SerializeField] private float minSoundInterval = 0.05f;

        [Header("Skip 事件过滤")]
        [Tooltip("Skip 期间要屏蔽的事件名称列表（如 playsound, camerashake 等会叠加/爆炸的事件）")]
        [SerializeField] private string[] blockedEventsOnSkip = new string[] { "playsound", "camerashake", "shake", "sound" };

        [Tooltip("Skip 期间允许通过的事件名称列表（如为空则默认允许不在屏蔽列表中的事件）")]
        [SerializeField] private string[] allowedEventsOnSkip = new string[] { "emotion", "expression", "setvar" };

        [Tooltip("Skip 事件过滤策略: Blacklist=只屏蔽黑名单中的, Whitelist=只允许白名单中的, BlockAll=全部屏蔽")]
        [SerializeField] private SkipEventFilterMode skipEventFilterMode = SkipEventFilterMode.Blacklist;

        private float lastSoundTime;
        private System.Collections.Generic.HashSet<string> _blockedEventsSet;
        private System.Collections.Generic.HashSet<string> _allowedEventsSet;

        private void Awake()
        {
            // 自动查找组件
            if (typewriter == null)
                typewriter = FindFirstObjectByType<TypewriterComponent>();

            if (dialogueRunner == null)
                dialogueRunner = FindFirstObjectByType<DialogueRunner>();

            if (linePresenter == null)
                linePresenter = FindFirstObjectByType<TALinePresenter>();

            // 构建事件过滤集合
            BuildEventFilterSets();
        }

        private void BuildEventFilterSets()
        {
            _blockedEventsSet = new System.Collections.Generic.HashSet<string>(
                System.StringComparer.OrdinalIgnoreCase);
            _allowedEventsSet = new System.Collections.Generic.HashSet<string>(
                System.StringComparer.OrdinalIgnoreCase);

            if (blockedEventsOnSkip != null)
            {
                foreach (var e in blockedEventsOnSkip)
                    if (!string.IsNullOrEmpty(e)) _blockedEventsSet.Add(e.ToLower());
            }

            if (allowedEventsOnSkip != null)
            {
                foreach (var e in allowedEventsOnSkip)
                    if (!string.IsNullOrEmpty(e)) _allowedEventsSet.Add(e.ToLower());
            }
        }

        private void OnEnable()
        {
            // 订阅 Typewriter 事件
            if (typewriter != null)
            {
                typewriter.onCharacterVisible.AddListener(OnCharacterVisible);
                typewriter.onTextShowed.AddListener(OnTextShowed);
                typewriter.onMessage.AddListener(OnMessage);
            }

            // 订阅 DialogueRunner 事件
            if (dialogueRunner != null)
            {
                dialogueRunner.onDialogueStart.AddListener(OnDialogueStart);
                dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
                dialogueRunner.onNodeStart.AddListener(OnNodeStart);
                dialogueRunner.onNodeComplete.AddListener(OnNodeComplete);
            }
        }

        private void OnDisable()
        {
            // 取消订阅 Typewriter 事件
            if (typewriter != null)
            {
                typewriter.onCharacterVisible.RemoveListener(OnCharacterVisible);
                typewriter.onTextShowed.RemoveListener(OnTextShowed);
                typewriter.onMessage.RemoveListener(OnMessage);
            }

            // 取消订阅 DialogueRunner 事件
            if (dialogueRunner != null)
            {
                dialogueRunner.onDialogueStart.RemoveListener(OnDialogueStart);
                dialogueRunner.onDialogueComplete.RemoveListener(OnDialogueComplete);
                dialogueRunner.onNodeStart.RemoveListener(OnNodeStart);
                dialogueRunner.onNodeComplete.RemoveListener(OnNodeComplete);
            }
        }

        #region Typewriter Event Handlers

        private void OnCharacterVisible(CharacterData characterData)
        {
            char character = characterData.info.character;
            onCharacterShown?.Invoke(character);

            // 播放打字音效
            if (playTypingSounds && !char.IsWhiteSpace(character))
            {
                PlayTypingSound();
            }
        }

        private void OnTextShowed()
        {
            onTypewriterComplete?.Invoke();
        }

        private void OnMessage(EventMarker marker)
        {
            // Skip 期间事件过滤
            if (IsSkipping() && ShouldBlockEventOnSkip(marker.name))
            {
                Debug.Log($"[TADialogueEvents] Event '{marker.name}' blocked during Skip");
                return;
            }

            // 转发自定义消息
            onCustomMessage?.Invoke(marker.name);

            // 可以在这里处理特殊消息
            switch (marker.name.ToLower())
            {
                case "playsound":
                    if (marker.parameters.Length > 0)
                    {
                        // 可以通过参数指定音效
                        Debug.Log($"[TADialogueEvents] Play sound: {marker.parameters[0]}");
                    }
                    break;

                case "shake":
                    Debug.Log("[TADialogueEvents] Camera shake triggered");
                    break;
            }
        }

        /// <summary>
        /// 检查当前是否处于 Skip 状态
        /// </summary>
        private bool IsSkipping()
        {
            return linePresenter != null && linePresenter.IsSkipping;
        }

        /// <summary>
        /// 根据过滤策略判断事件是否应被屏蔽
        /// </summary>
        private bool ShouldBlockEventOnSkip(string eventName)
        {
            if (string.IsNullOrEmpty(eventName)) return false;

            var lowerName = eventName.ToLower();

            switch (skipEventFilterMode)
            {
                case SkipEventFilterMode.BlockAll:
                    return true;

                case SkipEventFilterMode.Whitelist:
                    // 只有白名单中的事件才允许触发
                    return !_allowedEventsSet.Contains(lowerName);

                case SkipEventFilterMode.Blacklist:
                default:
                    // 只屏蔽黑名单中的事件
                    return _blockedEventsSet.Contains(lowerName);
            }
        }

        #endregion

        #region DialogueRunner Event Handlers

        private void OnDialogueStart()
        {
            onDialogueStarted?.Invoke();
        }

        private void OnDialogueComplete()
        {
            onDialogueEnded?.Invoke();
        }

        private void OnNodeStart(string nodeName)
        {
            onNodeStarted?.Invoke(nodeName);
        }

        private void OnNodeComplete(string nodeName)
        {
            onNodeEnded?.Invoke(nodeName);
        }

        #endregion

        #region Audio

        private void PlayTypingSound()
        {
            if (typingAudioSource == null || typingSounds == null || typingSounds.Length == 0)
                return;

            if (Time.time - lastSoundTime < minSoundInterval)
                return;

            lastSoundTime = Time.time;

            var clip = typingSounds[Random.Range(0, typingSounds.Length)];
            typingAudioSource.PlayOneShot(clip);
        }

        #endregion
    }
}
