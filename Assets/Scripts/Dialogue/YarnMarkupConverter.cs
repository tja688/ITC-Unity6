// =============================================================================
// YarnMarkupConverter.cs - Yarn Spinner Markup to Text Animator Tag Converter
// Converts Yarn Spinner's [markup] format to Text Animator's <tag> format
// =============================================================================

using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Yarn.Markup;

namespace ITC.Dialogue
{
    /// <summary>
    /// 标签类型枚举
    /// </summary>
    public enum TagType
    {
        /// <summary>包裹型：成对标签 <tag>...</tag></summary>
        Wrap,
        /// <summary>单标签型：独立标签 <tag=value></summary>
        Single,
        /// <summary>事件型：转换为事件标签 <?event=value></summary>
        Event
    }

    /// <summary>
    /// 将 Yarn Spinner 的标记转换为 Text Animator 的标签
    /// 支持自定义标签映射
    /// </summary>
    [CreateAssetMenu(fileName = "YarnMarkupConverter", menuName = "ITC/Dialogue/Yarn Markup Converter")]
    public class YarnMarkupConverter : ScriptableObject
    {
        #region Serialized Fields

        [System.Serializable]
        public class TagMapping
        {
            [Tooltip("Yarn Spinner 中的标记名称")]
            public string yarnTag;

            [Tooltip("Text Animator 中对应的标签名称")]
            public string taTag;

            [Tooltip("标签类型：Wrap=成对包裹, Single=单标签, Event=转为事件")]
            public TagType type = TagType.Wrap;

            [Tooltip("是否需要转换属性值")]
            public bool convertValue;

            [Tooltip("值的转换系数（如 ms 转 s 需要 0.001）")]
            public float valueMultiplier = 1f;
        }

        [SerializeField]
        [Tooltip("标签映射列表")]
        private List<TagMapping> tagMappings = new List<TagMapping>
        {
            // 单标签动作（不需要闭合标签）
            new TagMapping { yarnTag = "pause", taTag = "waitfor", type = TagType.Single, convertValue = true, valueMultiplier = 0.001f },
            new TagMapping { yarnTag = "waitinput", taTag = "waitinput", type = TagType.Single, convertValue = false },
            // 包裹型效果（需要闭合标签）
            new TagMapping { yarnTag = "shake", taTag = "shake", type = TagType.Wrap, convertValue = false },
            new TagMapping { yarnTag = "wave", taTag = "wave", type = TagType.Wrap, convertValue = false },
            new TagMapping { yarnTag = "wobble", taTag = "wiggle", type = TagType.Wrap, convertValue = false },
            new TagMapping { yarnTag = "bounce", taTag = "bounce", type = TagType.Wrap, convertValue = false },
            new TagMapping { yarnTag = "rainbow", taTag = "rainb", type = TagType.Wrap, convertValue = false },
            // 事件型（转为 <?event> 格式）
            new TagMapping { yarnTag = "playsound", taTag = "playsound", type = TagType.Event, convertValue = false },
            new TagMapping { yarnTag = "camerashake", taTag = "camerashake", type = TagType.Event, convertValue = false },
        };

        [SerializeField]
        [Tooltip("是否保留未映射的标记（原样传递）")]
        private bool passUnmappedTags = true;

        #endregion

        #region Private Fields

        private Dictionary<string, TagMapping> _mappingCache;

        #endregion

        #region Public Methods

        /// <summary>
        /// 将 MarkupParseResult 转换为 Text Animator 格式的字符串
        /// </summary>
        public string Convert(MarkupParseResult markup)
        {
            if (markup.Attributes == null || markup.Attributes.Count == 0)
            {
                return markup.Text;
            }

            EnsureCacheBuilt();

            var result = new StringBuilder(markup.Text);

            // 从后往前处理，避免位置偏移问题
            var sortedAttributes = new List<MarkupAttribute>(markup.Attributes);
            sortedAttributes.Sort((a, b) => b.Position.CompareTo(a.Position));

            foreach (var attr in sortedAttributes)
            {
                // 跳过 character 标记（由其他逻辑处理）
                if (attr.Name == "character") continue;

                ProcessAttribute(result, attr);
            }

            return result.ToString();
        }

        /// <summary>
        /// 将简单字符串中的 Yarn 风格标记转换为 Text Animator 格式
        /// 格式：[tag] 或 [tag=value] 转为 <tag> 或 <tag=value>
        /// </summary>
        public string ConvertSimple(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            EnsureCacheBuilt();

            var result = new StringBuilder(text);

            // 替换已知的 Yarn 标记
            foreach (var mapping in tagMappings)
            {
                // 替换开始标签 [tag] -> <tag>
                result.Replace($"[{mapping.yarnTag}]", $"<{mapping.taTag}>");

                // 替换结束标签 [/tag] -> </tag>
                result.Replace($"[/{mapping.yarnTag}]", $"</{mapping.taTag}>");

                // 处理带值的开始标签 [tag=value] -> <tag=value>
                // 需要更复杂的处理，这里简化处理
            }

            return result.ToString();
        }

        #endregion

        #region Private Methods

        private void EnsureCacheBuilt()
        {
            if (_mappingCache != null) return;

            _mappingCache = new Dictionary<string, TagMapping>();
            foreach (var mapping in tagMappings)
            {
                if (!string.IsNullOrEmpty(mapping.yarnTag))
                {
                    _mappingCache[mapping.yarnTag.ToLower()] = mapping;
                }
            }
        }

        private void ProcessAttribute(StringBuilder result, MarkupAttribute attr)
        {
            var tagName = attr.Name.ToLower();

            if (!_mappingCache.TryGetValue(tagName, out var mapping))
            {
                if (!passUnmappedTags) return;

                // 未映射的标记原样传递（假设 Text Animator 能识别，默认为 Wrap 类型）
                InsertTag(result, attr, attr.Name, null, TagType.Wrap);
                return;
            }

            // 处理值转换
            string valueStr = null;
            if (mapping.convertValue)
            {
                // 尝试获取默认属性值
                if (attr.Properties.TryGetValue(attr.Name, out var value))
                {
                    float numValue = value.FloatValue * mapping.valueMultiplier;
                    valueStr = numValue.ToString("F2");
                }
                else if (attr.Properties.TryGetValue("time", out var timeValue))
                {
                    float numValue = timeValue.FloatValue * mapping.valueMultiplier;
                    valueStr = numValue.ToString("F2");
                }
            }

            InsertTag(result, attr, mapping.taTag, valueStr, mapping.type);
        }

        private void InsertTag(StringBuilder result, MarkupAttribute attr, string tagName, string value, TagType tagType)
        {
            int endPos = attr.Position + attr.Length;

            // 确保不越界
            if (endPos > result.Length) endPos = result.Length;
            if (attr.Position > result.Length) return;

            switch (tagType)
            {
                case TagType.Wrap:
                    // 包裹型：插入成对标签 <tag>...</tag>
                    result.Insert(endPos, $"</{tagName}>");
                    string wrapStartTag = string.IsNullOrEmpty(value)
                        ? $"<{tagName}>"
                        : $"<{tagName}={value}>";
                    result.Insert(attr.Position, wrapStartTag);
                    break;

                case TagType.Single:
                    // 单标签型：只插入单个标签 <tag=value>（不插入闭合标签）
                    string singleTag = string.IsNullOrEmpty(value)
                        ? $"<{tagName}>"
                        : $"<{tagName}={value}>";
                    result.Insert(attr.Position, singleTag);
                    break;

                case TagType.Event:
                    // 事件型：转换为事件标签 <?event=value>
                    string eventTag = string.IsNullOrEmpty(value)
                        ? $"<?{tagName}>"
                        : $"<?{tagName}={value}>";
                    result.Insert(attr.Position, eventTag);
                    break;
            }
        }

        #endregion

        #region Editor Methods

        private void OnValidate()
        {
            // 清除缓存以便重新构建
            _mappingCache = null;
        }

        #endregion
    }
}
