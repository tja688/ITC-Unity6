using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ITC.Dialogue;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace ITC.Tests
{
    public enum EffectRecommendationMode
    {
        Continuous = 0,
        LoopCount = 1,
        DurationSeconds = 2
    }

    [Serializable]
    public class EffectRecommendationEntry
    {
        public string effectTag;
        public EffectRecommendationMode recommendationMode;
        public int loopCount = 1;
        public float durationSeconds = 1f;
        public string updatedAtUtc;
    }

    [Serializable]
    public class EffectRecommendationDocument
    {
        public string schemaVersion = "1.0.0";
        public string generatedAtUtc;
        public string scenePath;
        public string sourceNode;
        public List<EffectRecommendationEntry> entries = new List<EffectRecommendationEntry>();
    }

    public class TextAnimatorEffectAnnotationPanel : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private TADialogueEvents dialogueEvents;

        [Header("Marker")]
        [SerializeField] private string markerPrefix = "fx_";
        [SerializeField] private string sourceNodeName = "TA_Effect_Annotation_Start";

        [Header("Output")]
        [SerializeField] private string jsonOutputRelativePath = "Tests/Test scripts/TextAnimatorEffectRecommendations.json";
        [SerializeField] private string markdownOutputRelativePath = "Tests/Test scripts/TextAnimatorEffectRecommendations.md";

        private static readonly string[] OrderedEffectTags =
        {
            "bounce", "dangle", "expand", "fade", "incr", "pend", "rainb",
            "rot", "shake", "slideh", "slidev", "swing", "wave", "wiggle"
        };

        private readonly Dictionary<string, EffectRecommendationEntry> entries =
            new Dictionary<string, EffectRecommendationEntry>(StringComparer.OrdinalIgnoreCase);

        private Font fallbackFont;
        private string currentEffectTag = string.Empty;
        private EffectRecommendationMode currentMode = EffectRecommendationMode.Continuous;
        private bool suppressUiCallbacks;
        private bool controlsInteractable;

        private Text currentEffectText;
        private Text saveStatusText;
        private Button continuousButton;
        private Button loopCountButton;
        private Button durationButton;
        private Button saveButton;
        private InputField loopCountInput;
        private InputField durationInput;

        private readonly Color selectedColor = new Color(0.19f, 0.47f, 0.24f, 1f);
        private readonly Color normalColor = new Color(0.22f, 0.22f, 0.22f, 1f);
        private readonly Color disabledColor = new Color(0.15f, 0.15f, 0.15f, 0.85f);

        private void Awake()
        {
            if (dialogueRunner == null)
            {
                dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            }

            if (dialogueEvents == null)
            {
                dialogueEvents = FindFirstObjectByType<TADialogueEvents>();
            }
        }

        private void Start()
        {
            LoadExistingDocument();
            BuildUi();
            RefreshUiFromCurrentEffect();
        }

        private void OnEnable()
        {
            if (dialogueEvents != null)
            {
                dialogueEvents.onCustomMessage.AddListener(HandleCustomMessage);
            }
        }

        private void OnDisable()
        {
            if (dialogueEvents != null)
            {
                dialogueEvents.onCustomMessage.RemoveListener(HandleCustomMessage);
            }
        }

        private void HandleCustomMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            if (!message.StartsWith(markerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var effectTag = message.Substring(markerPrefix.Length).Trim();
            if (string.IsNullOrEmpty(effectTag))
            {
                return;
            }

            currentEffectTag = effectTag.ToLowerInvariant();
            RefreshUiFromCurrentEffect();
        }

        private void BuildUi()
        {
            var canvas = FindDialogueCanvas();
            if (canvas == null)
            {
                Debug.LogError("[TextAnimatorEffectAnnotationPanel] Dialogue Canvas not found.");
                return;
            }

            fallbackFont = LoadFallbackFont();
            if (fallbackFont == null)
            {
                Debug.LogError("[TextAnimatorEffectAnnotationPanel] Built-in font not found.");
                return;
            }

            var existingPanel = canvas.transform.Find("TA_EffectAnnotationPanel");
            if (existingPanel != null)
            {
                Destroy(existingPanel.gameObject);
            }

            var panel = new GameObject(
                "TA_EffectAnnotationPanel",
                typeof(RectTransform),
                typeof(Image),
                typeof(VerticalLayoutGroup),
                typeof(ContentSizeFitter));
            panel.transform.SetParent(canvas.transform, false);

            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(1f, 1f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.pivot = new Vector2(1f, 1f);
            panelRect.anchoredPosition = new Vector2(-24f, -24f);
            panelRect.sizeDelta = new Vector2(500f, 0f);

            var panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0f, 0f, 0f, 0.72f);

            var panelLayout = panel.GetComponent<VerticalLayoutGroup>();
            panelLayout.padding = new RectOffset(12, 12, 10, 10);
            panelLayout.spacing = 8f;
            panelLayout.childAlignment = TextAnchor.UpperLeft;
            panelLayout.childControlWidth = true;
            panelLayout.childControlHeight = true;
            panelLayout.childForceExpandWidth = true;
            panelLayout.childForceExpandHeight = false;

            var fitter = panel.GetComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            CreateLabel(panel.transform, "Text Animator 效果标注", 20, FontStyle.Bold);
            currentEffectText = CreateLabel(panel.transform, "当前效果：等待 <?fx_xxx> 事件", 16, FontStyle.Normal);

            continuousButton = CreateSingleButtonRow(panel.transform, "1) 可以持续", () =>
            {
                SetMode(EffectRecommendationMode.Continuous, true);
            });

            var loopRow = CreateHorizontalRow(panel.transform, "LoopRow");
            loopCountButton = CreateButton(loopRow, "2) 不持续，按 Loop", () =>
            {
                SetMode(EffectRecommendationMode.LoopCount, true);
            }, 300f);
            loopCountInput = CreateInputField(loopRow, "1", true);
            loopCountInput.onEndEdit.AddListener(_ =>
            {
                if (!suppressUiCallbacks && currentMode == EffectRecommendationMode.LoopCount)
                {
                    SaveCurrentRecommendation();
                }
            });

            var durationRow = CreateHorizontalRow(panel.transform, "DurationRow");
            durationButton = CreateButton(durationRow, "3) 不持续，按时间(秒)", () =>
            {
                SetMode(EffectRecommendationMode.DurationSeconds, true);
            }, 300f);
            durationInput = CreateInputField(durationRow, "1", false);
            durationInput.onEndEdit.AddListener(_ =>
            {
                if (!suppressUiCallbacks && currentMode == EffectRecommendationMode.DurationSeconds)
                {
                    SaveCurrentRecommendation();
                }
            });

            var bottomRow = CreateHorizontalRow(panel.transform, "BottomRow");
            saveButton = CreateButton(bottomRow, "立即保存文档", SaveCurrentRecommendation, 180f);
            saveStatusText = CreateLabel(bottomRow, "未保存", 14, FontStyle.Italic);
            var statusLayout = saveStatusText.gameObject.AddComponent<LayoutElement>();
            statusLayout.preferredWidth = 290f;
        }

        private void SetMode(EffectRecommendationMode mode, bool saveImmediately)
        {
            currentMode = mode;
            UpdateSelectionVisuals();
            UpdateInputInteractivity();

            if (saveImmediately)
            {
                SaveCurrentRecommendation();
            }
        }

        private void RefreshUiFromCurrentEffect()
        {
            if (currentEffectText == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(currentEffectTag))
            {
                currentEffectText.text = "当前效果：等待 <?fx_xxx> 事件";
                SetControlsInteractable(false);
                SetMode(EffectRecommendationMode.Continuous, false);
                return;
            }

            currentEffectText.text = $"当前效果：{currentEffectTag}";
            SetControlsInteractable(true);

            if (entries.TryGetValue(currentEffectTag, out var entry))
            {
                suppressUiCallbacks = true;
                loopCountInput.SetTextWithoutNotify(Mathf.Max(1, entry.loopCount).ToString(CultureInfo.InvariantCulture));
                durationInput.SetTextWithoutNotify(
                    Mathf.Max(0.01f, entry.durationSeconds).ToString("0.###", CultureInfo.InvariantCulture));
                suppressUiCallbacks = false;

                SetMode(entry.recommendationMode, false);
            }
            else
            {
                suppressUiCallbacks = true;
                loopCountInput.SetTextWithoutNotify("1");
                durationInput.SetTextWithoutNotify("1");
                suppressUiCallbacks = false;
                SetMode(EffectRecommendationMode.Continuous, false);
            }
        }

        private void SaveCurrentRecommendation()
        {
            if (string.IsNullOrEmpty(currentEffectTag))
            {
                saveStatusText.text = "请先展示一个效果再标注";
                return;
            }

            var loopCount = ParsePositiveInt(loopCountInput, 1);
            var duration = ParsePositiveFloat(durationInput, 1f);

            entries[currentEffectTag] = new EffectRecommendationEntry
            {
                effectTag = currentEffectTag,
                recommendationMode = currentMode,
                loopCount = loopCount,
                durationSeconds = duration,
                updatedAtUtc = DateTime.UtcNow.ToString("O")
            };

            PersistRecommendationDocuments();
        }

        private void PersistRecommendationDocuments()
        {
            try
            {
                var now = DateTime.UtcNow.ToString("O");
                var jsonPath = ResolveAssetPath(jsonOutputRelativePath);
                var markdownPath = ResolveAssetPath(markdownOutputRelativePath);

                EnsureParentDirectory(jsonPath);
                EnsureParentDirectory(markdownPath);

                var document = new EffectRecommendationDocument
                {
                    generatedAtUtc = now,
                    scenePath = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path,
                    sourceNode = sourceNodeName,
                    entries = GetOrderedEntries()
                };

                File.WriteAllText(jsonPath, JsonUtility.ToJson(document, true), Encoding.UTF8);
                File.WriteAllText(markdownPath, BuildMarkdown(document), Encoding.UTF8);

                if (saveStatusText != null)
                {
                    saveStatusText.text = $"已保存：{DateTime.Now:HH:mm:ss}";
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                if (saveStatusText != null)
                {
                    saveStatusText.text = "保存失败，请看 Console";
                }
            }
        }

        private void LoadExistingDocument()
        {
            entries.Clear();

            var jsonPath = ResolveAssetPath(jsonOutputRelativePath);
            if (!File.Exists(jsonPath))
            {
                return;
            }

            try
            {
                var json = File.ReadAllText(jsonPath, Encoding.UTF8);
                var document = JsonUtility.FromJson<EffectRecommendationDocument>(json);
                if (document?.entries == null)
                {
                    return;
                }

                foreach (var entry in document.entries)
                {
                    if (entry == null || string.IsNullOrWhiteSpace(entry.effectTag))
                    {
                        continue;
                    }

                    entry.effectTag = entry.effectTag.Trim().ToLowerInvariant();
                    entry.loopCount = Mathf.Max(1, entry.loopCount);
                    entry.durationSeconds = Mathf.Max(0.01f, entry.durationSeconds);
                    entries[entry.effectTag] = entry;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"[TextAnimatorEffectAnnotationPanel] Failed to load existing document: {ex.Message}");
            }
        }

        private List<EffectRecommendationEntry> GetOrderedEntries()
        {
            var result = new List<EffectRecommendationEntry>(entries.Count);

            foreach (var tag in OrderedEffectTags)
            {
                if (entries.TryGetValue(tag, out var knownEntry))
                {
                    result.Add(CloneEntry(knownEntry));
                }
            }

            var remaining = entries.Keys
                .Where(key => !OrderedEffectTags.Contains(key, StringComparer.OrdinalIgnoreCase))
                .OrderBy(key => key, StringComparer.OrdinalIgnoreCase);

            foreach (var key in remaining)
            {
                result.Add(CloneEntry(entries[key]));
            }

            return result;
        }

        private static EffectRecommendationEntry CloneEntry(EffectRecommendationEntry source)
        {
            return new EffectRecommendationEntry
            {
                effectTag = source.effectTag,
                recommendationMode = source.recommendationMode,
                loopCount = source.loopCount,
                durationSeconds = source.durationSeconds,
                updatedAtUtc = source.updatedAtUtc
            };
        }

        private string BuildMarkdown(EffectRecommendationDocument document)
        {
            var builder = new StringBuilder(2048);
            builder.AppendLine("# Text Animator Effect Recommendations");
            builder.AppendLine();
            builder.AppendLine($"- Generated UTC: {document.generatedAtUtc}");
            builder.AppendLine($"- Scene: `{document.scenePath}`");
            builder.AppendLine($"- Source Node: `{document.sourceNode}`");
            builder.AppendLine();
            builder.AppendLine("| Effect Tag | Recommendation | Loop Count | Duration (s) | Updated UTC |");
            builder.AppendLine("| --- | --- | ---: | ---: | --- |");

            foreach (var entry in document.entries)
            {
                builder.AppendLine(
                    $"| `{entry.effectTag}` | {ModeToDisplayText(entry.recommendationMode)} | {entry.loopCount} | {entry.durationSeconds.ToString("0.###", CultureInfo.InvariantCulture)} | {entry.updatedAtUtc} |");
            }

            if (document.entries.Count == 0)
            {
                builder.AppendLine("| *(none)* | - | - | - | - |");
            }

            return builder.ToString();
        }

        private static string ModeToDisplayText(EffectRecommendationMode mode)
        {
            switch (mode)
            {
                case EffectRecommendationMode.Continuous:
                    return "可持续";
                case EffectRecommendationMode.LoopCount:
                    return "按 Loop 次数";
                case EffectRecommendationMode.DurationSeconds:
                    return "按时间秒数";
                default:
                    return mode.ToString();
            }
        }

        private void UpdateSelectionVisuals()
        {
            if (!controlsInteractable)
            {
                SetButtonColor(continuousButton, disabledColor);
                SetButtonColor(loopCountButton, disabledColor);
                SetButtonColor(durationButton, disabledColor);
                SetButtonColor(saveButton, disabledColor);
                return;
            }

            SetButtonColor(continuousButton, currentMode == EffectRecommendationMode.Continuous ? selectedColor : normalColor);
            SetButtonColor(loopCountButton, currentMode == EffectRecommendationMode.LoopCount ? selectedColor : normalColor);
            SetButtonColor(durationButton, currentMode == EffectRecommendationMode.DurationSeconds ? selectedColor : normalColor);
            SetButtonColor(saveButton, normalColor);
        }

        private void UpdateInputInteractivity()
        {
            if (loopCountInput != null)
            {
                loopCountInput.interactable = currentMode == EffectRecommendationMode.LoopCount;
            }

            if (durationInput != null)
            {
                durationInput.interactable = currentMode == EffectRecommendationMode.DurationSeconds;
            }
        }

        private void SetControlsInteractable(bool interactable)
        {
            controlsInteractable = interactable;

            SetButtonInteractable(continuousButton, interactable);
            SetButtonInteractable(loopCountButton, interactable);
            SetButtonInteractable(durationButton, interactable);
            SetButtonInteractable(saveButton, interactable);

            if (loopCountInput != null)
            {
                loopCountInput.interactable = interactable && currentMode == EffectRecommendationMode.LoopCount;
            }

            if (durationInput != null)
            {
                durationInput.interactable = interactable && currentMode == EffectRecommendationMode.DurationSeconds;
            }

            UpdateSelectionVisuals();
        }

        private static void SetButtonColor(Button button, Color color)
        {
            if (button == null)
            {
                return;
            }

            var image = button.GetComponent<Image>();
            if (image != null)
            {
                image.color = color;
            }
        }

        private void SetButtonInteractable(Button button, bool interactable)
        {
            if (button == null)
            {
                return;
            }

            button.interactable = interactable;
        }

        private static void EnsureParentDirectory(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private int ParsePositiveInt(InputField inputField, int fallback)
        {
            if (inputField == null)
            {
                return fallback;
            }

            if (!int.TryParse(inputField.text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) || value < 1)
            {
                value = fallback;
            }

            inputField.SetTextWithoutNotify(value.ToString(CultureInfo.InvariantCulture));
            return value;
        }

        private float ParsePositiveFloat(InputField inputField, float fallback)
        {
            if (inputField == null)
            {
                return fallback;
            }

            var raw = inputField.text;
            var parsed = float.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
                         || float.TryParse(raw, NumberStyles.Float, CultureInfo.CurrentCulture, out value);

            if (!parsed || value <= 0f)
            {
                value = fallback;
            }

            inputField.SetTextWithoutNotify(value.ToString("0.###", CultureInfo.InvariantCulture));
            return value;
        }

        private string ResolveAssetPath(string relativePath)
        {
            var normalized = (relativePath ?? string.Empty).Replace('\\', '/');
            if (normalized.StartsWith("Assets/", StringComparison.OrdinalIgnoreCase))
            {
                normalized = normalized.Substring("Assets/".Length);
            }

            return Path.Combine(Application.dataPath, normalized.Replace('/', Path.DirectorySeparatorChar));
        }

        private static Font LoadFallbackFont()
        {
            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }

            return font;
        }

        private Canvas FindDialogueCanvas()
        {
            var named = GameObject.Find("DialogueCanvas");
            if (named != null && named.TryGetComponent(out Canvas namedCanvas))
            {
                return namedCanvas;
            }

            return FindFirstObjectByType<Canvas>();
        }

        private Text CreateLabel(Transform parent, string text, int fontSize, FontStyle style)
        {
            var go = new GameObject("Label", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);

            var label = go.GetComponent<Text>();
            label.text = text;
            label.font = fallbackFont;
            label.fontSize = fontSize;
            label.fontStyle = style;
            label.color = Color.white;
            label.alignment = TextAnchor.MiddleLeft;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;

            return label;
        }

        private Button CreateSingleButtonRow(Transform parent, string text, Action onClick)
        {
            var row = CreateHorizontalRow(parent, "ButtonRow");
            return CreateButton(row, text, onClick, 300f);
        }

        private Transform CreateHorizontalRow(Transform parent, string name)
        {
            var row = new GameObject(name, typeof(RectTransform), typeof(HorizontalLayoutGroup));
            row.transform.SetParent(parent, false);

            var layout = row.GetComponent<HorizontalLayoutGroup>();
            layout.spacing = 8f;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;
            layout.childAlignment = TextAnchor.MiddleLeft;

            return row.transform;
        }

        private Button CreateButton(Transform parent, string labelText, Action onClick, float preferredWidth)
        {
            var buttonGo = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            buttonGo.transform.SetParent(parent, false);

            var layout = buttonGo.GetComponent<LayoutElement>();
            layout.minHeight = 34f;
            if (preferredWidth > 0f)
            {
                layout.preferredWidth = preferredWidth;
            }
            else
            {
                layout.minWidth = 180f;
                layout.flexibleWidth = 1f;
            }

            var image = buttonGo.GetComponent<Image>();
            image.color = normalColor;

            var button = buttonGo.GetComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => onClick?.Invoke());

            var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(buttonGo.transform, false);
            var text = textGo.GetComponent<Text>();
            text.text = labelText;
            text.font = fallbackFont;
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;

            var textRect = text.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(6f, 3f);
            textRect.offsetMax = new Vector2(-6f, -3f);

            return button;
        }

        private InputField CreateInputField(Transform parent, string defaultText, bool integerOnly)
        {
            var fieldGo = new GameObject("InputField", typeof(RectTransform), typeof(Image), typeof(InputField), typeof(LayoutElement));
            fieldGo.transform.SetParent(parent, false);

            var layout = fieldGo.GetComponent<LayoutElement>();
            layout.preferredWidth = 100f;
            layout.minHeight = 34f;

            var bg = fieldGo.GetComponent<Image>();
            bg.color = Color.white;

            var inputField = fieldGo.GetComponent<InputField>();
            inputField.contentType = integerOnly ? InputField.ContentType.IntegerNumber : InputField.ContentType.DecimalNumber;

            var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
            textGo.transform.SetParent(fieldGo.transform, false);
            var inputText = textGo.GetComponent<Text>();
            inputText.font = fallbackFont;
            inputText.fontSize = 14;
            inputText.alignment = TextAnchor.MiddleLeft;
            inputText.color = new Color(0.1f, 0.1f, 0.1f, 1f);
            inputText.supportRichText = false;

            var placeholderGo = new GameObject("Placeholder", typeof(RectTransform), typeof(Text));
            placeholderGo.transform.SetParent(fieldGo.transform, false);
            var placeholder = placeholderGo.GetComponent<Text>();
            placeholder.font = fallbackFont;
            placeholder.fontSize = 14;
            placeholder.alignment = TextAnchor.MiddleLeft;
            placeholder.text = integerOnly ? "Loop 次数" : "持续秒数";
            placeholder.color = new Color(0.3f, 0.3f, 0.3f, 0.6f);

            var textRect = inputText.GetComponent<RectTransform>();
            var placeholderRect = placeholder.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(8f, 4f);
            textRect.offsetMax = new Vector2(-8f, -4f);
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.offsetMin = new Vector2(8f, 4f);
            placeholderRect.offsetMax = new Vector2(-8f, -4f);

            inputField.targetGraphic = bg;
            inputField.textComponent = inputText;
            inputField.placeholder = placeholder;
            inputField.text = defaultText;

            return inputField;
        }
    }
}
