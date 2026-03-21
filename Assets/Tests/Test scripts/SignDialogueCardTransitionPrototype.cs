using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace ITC.Dialogue.Testing
{
    [DisallowMultipleComponent]
    public sealed class SignDialogueCardTransitionPrototype : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private RectTransform dialogueSlot0Marker;
        [SerializeField] private RectTransform dialogueSlot1Template;
        [SerializeField] private RectTransform dialogueSlot2Template;
        [SerializeField] private RectTransform latestRuntimeParent;
        [SerializeField] private RectTransform historyRuntimeParent;
        [SerializeField] private TextMeshProUGUI textStyleTemplate;
        [SerializeField] private GameObject legacyDialogueSystemRoot;

        [Header("Interaction")]
        [SerializeField] private bool allowMouseClick = true;
        [SerializeField] private bool allowKeyboardTrigger = true;
        [SerializeField] private Key advanceKey = Key.Space;
        [SerializeField] private bool disableLegacyDialogueSystemWhilePlaying = true;

        [Header("Entry Timing")]
        [SerializeField] private float latestEntryDelay = 0f;
        [SerializeField] private float historyEntryDelay = 0f;
        [SerializeField] private float entryDuration = 0.24f;
        [SerializeField] private float settleDuration = 0.16f;
        [SerializeField] private Ease entryMoveEase = Ease.OutBack;
        [SerializeField] private Ease entryScaleEase = Ease.OutBack;

        [Header("Exit Timing")]
        [SerializeField] private float exitDuration = 0.24f;
        [SerializeField, Range(0f, 1f)] private float historyExitTriggerNormalized = 0f;
        [SerializeField] private float historyExitDelay = 0f;
        [SerializeField] private float exitRotationX = -90f;
        [SerializeField] private Ease exitEase = Ease.InQuad;

        [Header("Entry Feel")]
        [SerializeField] private Vector3 entryStartScaleMultiplier = new Vector3(0.92f, 0.84f, 1f);
        [SerializeField] private float settlePunchDistance = 18f;
        [SerializeField] private float settlePunchRotationZ = 2f;
        [SerializeField] private int settlePunchVibrato = 9;
        [SerializeField, Range(0f, 1f)] private float settlePunchElasticity = 0.45f;
        [SerializeField] private float latestAlphaOnEntry = 1f;
        [SerializeField] private float historyAlphaOnEntry = 0.88f;

        [Header("Text Layout")]
        [SerializeField] private Vector4 latestTextPadding = new Vector4(96f, 50f, -110f, -68f);
        [SerializeField] private Vector4 historyTextPadding = new Vector4(104f, 44f, -112f, -54f);
        [SerializeField] private Vector3 latestTextLocalEuler = Vector3.zero;
        [SerializeField] private Vector3 historyTextLocalEuler = Vector3.zero;

        [Header("Prototype Lines")]
        [SerializeField] private string[] sampleNpcLines =
        {
            "这是最新的一句台词，它会先落在下面的对话框1号。",
            "再点击一次，上一句会被推送到上方的对话框2号。",
            "这一版先把节奏跑通，等你试听手感后我们再细调错落时机。",
            "如果你喜欢，我们下一轮就可以把这套原型接进真实的 Yarn 文本流。"
        };

        private RuntimeCard currentLatestCard;
        private RuntimeCard currentHistoryCard;
        private int nextLineIndex;
        private bool transitionRunning;
        private bool legacyRootWasActive;
        private bool legacyRootStateCaptured;
        private Sequence activeSequence;

        private void Awake()
        {
            AutoAssignReferences();
            EnsureRuntimeParents();
        }

        private void Reset()
        {
            AutoAssignReferences();
            EnsureRuntimeParents();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            AutoAssignReferences();
            EnsureRuntimeParents();
        }

        private void OnEnable()
        {
            AutoAssignReferences();
            EnsureRuntimeParents();

            if (Application.isPlaying)
            {
                ToggleTemplateMarkers(false);
                ApplyLegacyRootState(false);
            }
        }

        private void OnDisable()
        {
            if (activeSequence != null && activeSequence.IsActive())
            {
                activeSequence.Kill();
            }

            activeSequence = null;
            transitionRunning = false;

            KillAndDisposeCard(ref currentLatestCard);
            KillAndDisposeCard(ref currentHistoryCard);
            RestoreLegacyRootState();

            if (Application.isPlaying)
            {
                ToggleTemplateMarkers(true);
            }
        }

        private void Update()
        {
            if (!Application.isPlaying || transitionRunning)
            {
                return;
            }

            if (allowMouseClick && Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                AdvancePrototype();
                return;
            }

            if (allowKeyboardTrigger && Keyboard.current != null && Keyboard.current[advanceKey].wasPressedThisFrame)
            {
                AdvancePrototype();
            }
        }

        [ContextMenu("Advance Prototype")]
        public void AdvancePrototype()
        {
            if (!ValidateSetup())
            {
                return;
            }

            var incomingLine = GetNextSampleLine();
            if (string.IsNullOrWhiteSpace(incomingLine))
            {
                Debug.LogWarning("[SignDialogueCardTransitionPrototype] 没有可用的示例台词。", this);
                return;
            }

            if (currentLatestCard == null)
            {
                transitionRunning = true;
                currentLatestCard = CreateCard(dialogueSlot1Template, latestRuntimeParent, incomingLine, latestTextPadding, latestTextLocalEuler, latestAlphaOnEntry);
                activeSequence = BuildEntrySequence(currentLatestCard, dialogueSlot1Template, latestEntryDelay);
                activeSequence.OnComplete(HandleTransitionComplete);
                Debug.Log($"[SignDialogueCardTransitionPrototype] 最新卡片入场: {incomingLine}", this);
                return;
            }

            transitionRunning = true;

            var outgoingLatest = currentLatestCard;
            var outgoingHistory = currentHistoryCard;
            var carryLine = outgoingLatest.Text.text;

            var incomingLatest = CreateCard(dialogueSlot1Template, latestRuntimeParent, incomingLine, latestTextPadding, latestTextLocalEuler, latestAlphaOnEntry);
            var incomingHistory = CreateCard(dialogueSlot2Template, historyRuntimeParent, carryLine, historyTextPadding, historyTextLocalEuler, historyAlphaOnEntry);

            currentLatestCard = incomingLatest;
            currentHistoryCard = incomingHistory;

            activeSequence = DOTween.Sequence();
            activeSequence.Join(BuildExitSequence(outgoingLatest, 0f));
            activeSequence.Insert(latestEntryDelay, BuildEntrySequence(incomingLatest, dialogueSlot1Template, 0f));

            var historyPhaseStart = Mathf.Max(0f, exitDuration * historyExitTriggerNormalized);
            activeSequence.Insert(historyPhaseStart + historyExitDelay, BuildEntrySequence(incomingHistory, dialogueSlot2Template, historyEntryDelay));

            if (outgoingHistory != null)
            {
                activeSequence.Insert(historyPhaseStart + historyExitDelay, BuildExitSequence(outgoingHistory, 0f));
            }

            activeSequence.OnComplete(() =>
            {
                DisposeCard(outgoingLatest);
                DisposeCard(outgoingHistory);
                HandleTransitionComplete();
            });

            Debug.Log($"[SignDialogueCardTransitionPrototype] 推进对话，最新: {incomingLine}", this);
        }

        private void HandleTransitionComplete()
        {
            activeSequence = null;
            transitionRunning = false;
        }

        private Sequence BuildEntrySequence(RuntimeCard card, RectTransform targetTemplate, float delay)
        {
            var targetPosition = targetTemplate.position;
            var targetScale = targetTemplate.localScale;
            var targetRotation = targetTemplate.localRotation;

            card.Root.position = dialogueSlot0Marker.position;
            card.Root.localRotation = targetRotation;
            card.Root.localScale = Vector3.Scale(targetScale, entryStartScaleMultiplier);
            card.CanvasGroup.alpha = 0f;
            card.Root.gameObject.SetActive(true);

            var sequence = DOTween.Sequence();
            if (delay > 0f)
            {
                sequence.AppendInterval(delay);
            }

            sequence.Join(card.Root.DOMove(targetPosition, entryDuration).SetEase(entryMoveEase));
            sequence.Join(card.Root.DOScale(targetScale, entryDuration).SetEase(entryScaleEase));
            sequence.Join(card.CanvasGroup.DOFade(card.TargetAlpha, Mathf.Min(entryDuration, 0.14f)).SetEase(Ease.OutQuad));
            sequence.Append(card.Root.DOPunchPosition(new Vector3(0f, settlePunchDistance, 0f), settleDuration, settlePunchVibrato, settlePunchElasticity));
            sequence.Join(card.Root.DOPunchRotation(new Vector3(0f, 0f, settlePunchRotationZ), settleDuration, settlePunchVibrato, settlePunchElasticity));
            return sequence;
        }

        private Sequence BuildExitSequence(RuntimeCard card, float delay)
        {
            var targetEuler = new Vector3(exitRotationX, card.BaseLocalEulerAngles.y, card.BaseLocalEulerAngles.z);
            var sequence = DOTween.Sequence();

            if (delay > 0f)
            {
                sequence.AppendInterval(delay);
            }

            sequence.Join(card.Root.DOLocalRotate(targetEuler, exitDuration, RotateMode.Fast).SetEase(exitEase));
            sequence.Join(card.CanvasGroup.DOFade(0f, exitDuration * 0.72f).SetEase(Ease.InQuad));
            return sequence;
        }

        private RuntimeCard CreateCard(
            RectTransform template,
            RectTransform runtimeParent,
            string line,
            Vector4 textPadding,
            Vector3 textEuler,
            float targetAlpha)
        {
            var clone = Instantiate(template.gameObject, runtimeParent, false);
            clone.name = $"{template.name}_Runtime";
            clone.SetActive(true);

            var root = clone.GetComponent<RectTransform>();
            root.anchorMin = template.anchorMin;
            root.anchorMax = template.anchorMax;
            root.pivot = template.pivot;
            root.sizeDelta = template.sizeDelta;
            root.localScale = template.localScale;
            root.localRotation = template.localRotation;
            root.position = template.position;

            var canvasGroup = clone.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = clone.AddComponent<CanvasGroup>();
            }

            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;

            var graphic = clone.GetComponent<Graphic>();
            if (graphic != null)
            {
                graphic.raycastTarget = false;
            }

            var text = CreateCardText(root, line, textPadding, textEuler);

            return new RuntimeCard(root, canvasGroup, text, targetAlpha);
        }

        private TextMeshProUGUI CreateCardText(RectTransform cardRoot, string line, Vector4 textPadding, Vector3 textEuler)
        {
            var textObject = new GameObject("DialogueText", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            var textRect = textObject.GetComponent<RectTransform>();
            textRect.SetParent(cardRoot, false);
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(textPadding.x, textPadding.y);
            textRect.offsetMax = new Vector2(textPadding.z, textPadding.w);
            textRect.localEulerAngles = textEuler;
            textRect.localScale = Vector3.one;

            var text = textObject.GetComponent<TextMeshProUGUI>();
            ApplyTextStyle(text);
            text.text = line;
            text.ForceMeshUpdate();
            return text;
        }

        private void ApplyTextStyle(TextMeshProUGUI target)
        {
            if (textStyleTemplate != null)
            {
                target.font = textStyleTemplate.font;
                target.fontSharedMaterial = textStyleTemplate.fontSharedMaterial;
                target.fontSize = textStyleTemplate.fontSize;
                target.fontStyle = textStyleTemplate.fontStyle;
                target.color = textStyleTemplate.color;
                target.alignment = textStyleTemplate.alignment;
                target.enableWordWrapping = textStyleTemplate.enableWordWrapping;
                target.overflowMode = textStyleTemplate.overflowMode;
                target.characterSpacing = textStyleTemplate.characterSpacing;
                target.wordSpacing = textStyleTemplate.wordSpacing;
                target.lineSpacing = textStyleTemplate.lineSpacing;
                target.richText = textStyleTemplate.richText;
                target.enableAutoSizing = textStyleTemplate.enableAutoSizing;
                target.fontSizeMin = textStyleTemplate.fontSizeMin;
                target.fontSizeMax = textStyleTemplate.fontSizeMax;
            }

            target.raycastTarget = false;
            target.margin = Vector4.zero;
        }

        private void AutoAssignReferences()
        {
            if (dialogueSlot1Template == null)
            {
                dialogueSlot1Template = FindRectTransformByName("对话框1号");
            }

            if (dialogueSlot2Template == null)
            {
                dialogueSlot2Template = FindRectTransformByName("对话框2号");
            }

            if (dialogueSlot0Marker == null)
            {
                dialogueSlot0Marker = FindRectTransformByName("对话框0号");
            }

            if (latestRuntimeParent == null && dialogueSlot1Template != null)
            {
                latestRuntimeParent = dialogueSlot1Template.parent as RectTransform;
            }

            if (historyRuntimeParent == null && dialogueSlot2Template != null)
            {
                historyRuntimeParent = dialogueSlot2Template.parent as RectTransform;
            }

            if (legacyDialogueSystemRoot == null)
            {
                var legacyRootTransform = FindTransformByName("签约特制对话系统");
                if (legacyRootTransform != null)
                {
                    legacyDialogueSystemRoot = legacyRootTransform.gameObject;
                }
            }

            if (textStyleTemplate == null)
            {
                var allTexts = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                for (var i = 0; i < allTexts.Length; i++)
                {
                    var candidate = allTexts[i];
                    if (candidate == null || candidate.transform.parent == null)
                    {
                        continue;
                    }

                    if (candidate.transform.parent.name == "LineText")
                    {
                        textStyleTemplate = candidate;
                        break;
                    }
                }
            }
        }

        private void EnsureRuntimeParents()
        {
            if (latestRuntimeParent == null && dialogueSlot1Template != null)
            {
                latestRuntimeParent = dialogueSlot1Template.parent as RectTransform;
            }

            if (historyRuntimeParent == null && dialogueSlot2Template != null)
            {
                historyRuntimeParent = dialogueSlot2Template.parent as RectTransform;
            }
        }

        private void ToggleTemplateMarkers(bool visible)
        {
            ToggleGraphic(dialogueSlot1Template, visible);
            ToggleGraphic(dialogueSlot2Template, visible);
            ToggleGraphic(dialogueSlot0Marker, visible);
        }

        private void ToggleGraphic(Component target, bool visible)
        {
            if (target == null || !Application.isPlaying)
            {
                return;
            }

            var graphic = target.GetComponent<Graphic>();
            if (graphic != null)
            {
                graphic.enabled = visible;
            }
        }

        private void ApplyLegacyRootState(bool active)
        {
            if (!Application.isPlaying || !disableLegacyDialogueSystemWhilePlaying || legacyDialogueSystemRoot == null)
            {
                return;
            }

            if (!legacyRootStateCaptured)
            {
                legacyRootWasActive = legacyDialogueSystemRoot.activeSelf;
                legacyRootStateCaptured = true;
            }

            legacyDialogueSystemRoot.SetActive(active);
        }

        private void RestoreLegacyRootState()
        {
            if (!Application.isPlaying || !legacyRootStateCaptured || legacyDialogueSystemRoot == null)
            {
                return;
            }

            legacyDialogueSystemRoot.SetActive(legacyRootWasActive);
        }

        private bool ValidateSetup()
        {
            AutoAssignReferences();
            EnsureRuntimeParents();

            if (dialogueSlot0Marker == null || dialogueSlot1Template == null || dialogueSlot2Template == null)
            {
                Debug.LogError("[SignDialogueCardTransitionPrototype] 缺少对话框0/1/2号引用，请先确认场景节点。", this);
                return false;
            }

            if (latestRuntimeParent == null || historyRuntimeParent == null)
            {
                Debug.LogError("[SignDialogueCardTransitionPrototype] 运行时父节点未准备好。", this);
                return false;
            }

            if (textStyleTemplate == null)
            {
                Debug.LogError("[SignDialogueCardTransitionPrototype] 未找到文本样式模板。", this);
                return false;
            }

            return true;
        }

        private string GetNextSampleLine()
        {
            if (sampleNpcLines == null || sampleNpcLines.Length == 0)
            {
                return string.Empty;
            }

            var line = sampleNpcLines[nextLineIndex % sampleNpcLines.Length];
            nextLineIndex = (nextLineIndex + 1) % sampleNpcLines.Length;
            return line;
        }

        private RectTransform FindRectTransformByName(string targetName)
        {
            var rects = FindObjectsByType<RectTransform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < rects.Length; i++)
            {
                if (rects[i] != null && rects[i].name == targetName)
                {
                    return rects[i];
                }
            }

            return null;
        }

        private Transform FindTransformByName(string targetName)
        {
            var transforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            for (var i = 0; i < transforms.Length; i++)
            {
                if (transforms[i] != null && transforms[i].name == targetName)
                {
                    return transforms[i];
                }
            }

            return null;
        }

        private void KillAndDisposeCard(ref RuntimeCard card)
        {
            if (card == null)
            {
                return;
            }

            if (card.Root != null)
            {
                card.Root.DOKill();
            }

            DisposeCard(card);
            card = null;
        }

        private void DisposeCard(RuntimeCard card)
        {
            if (card == null || card.Root == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(card.Root.gameObject);
            }
            else
            {
                DestroyImmediate(card.Root.gameObject);
            }
        }

        private sealed class RuntimeCard
        {
            public RuntimeCard(RectTransform root, CanvasGroup canvasGroup, TextMeshProUGUI text, float targetAlpha)
            {
                Root = root;
                CanvasGroup = canvasGroup;
                Text = text;
                TargetAlpha = targetAlpha;
                BaseLocalEulerAngles = root.localEulerAngles;
            }

            public RectTransform Root { get; }
            public CanvasGroup CanvasGroup { get; }
            public TextMeshProUGUI Text { get; }
            public float TargetAlpha { get; }
            public Vector3 BaseLocalEulerAngles { get; }
        }
    }
}
