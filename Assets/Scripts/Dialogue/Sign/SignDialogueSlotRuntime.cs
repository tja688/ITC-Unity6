using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using DG.Tweening;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Yarn.Unity;

namespace ITC.Dialogue
{
    public enum SignDialogueRole
    {
        Auto = 0,
        Npc = 1,
        Player = 2,
        Thought = 3
    }

    [DisallowMultipleComponent]
    public sealed class SignDialogueSlotRuntime : MonoBehaviour
    {
        [Serializable]
        private struct NpcLineRecord
        {
            public string NpcId;
            public string Text;
            public string Timestamp;
        }

        [Serializable]
        private struct TraceRecord
        {
            public string Role;
            public string Speaker;
            public string Text;
            public string Timestamp;
        }

        private const string NpcEnterCommand = "itc_sign_npc_enter";
        private const string NpcExitCommand = "itc_sign_npc_exit";
        private const string RoleCommand = "itc_sign_role";
        private const string PlaceholderMinigameCommand = "itc_sign_minigame";

        private static readonly Regex AngleTagRegex = new("<.*?>", RegexOptions.Compiled);

        [Header("Feature Toggle")]
        [SerializeField] private bool enableSignSlotRouting = true;
        [SerializeField] private bool suppressLegacyPresenterVisuals = true;

        [Header("Scene Bindings")]
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private Canvas rootCanvas;
        [SerializeField] private OptionsPresenter optionsPresenter;
        [SerializeField] private RectTransform panelRect;
        [SerializeField] private RectTransform textRoot;
        [SerializeField] private RectTransform optionsPanelRect;

        [Header("Startup")]
        [SerializeField] private bool autoStartDialogueIfIdle = true;
        [SerializeField] private string startupNodeName = "Start";

        [Header("Frame Anchors")]
        [SerializeField] private RectTransform npcSlot1Frame;
        [SerializeField] private RectTransform npcSlot2Frame;
        [SerializeField] private RectTransform playerSlot1Frame;
        [SerializeField] private RectTransform npcHistoryScrollArea;
        [SerializeField] private Vector2 optionsPanelPadding = new(140f, 110f);
        [SerializeField] private Vector2 optionsPanelOffset = Vector2.zero;
        [SerializeField] private bool normalizeOptionsPanelAnchorToCenter = true;

        [Header("Text Template")]
        [SerializeField] private TMP_Text textTemplate;
        [SerializeField] private Vector2 npcSlotPadding = new(120f, 72f);
        [SerializeField] private Vector2 playerSlotPadding = new(120f, 72f);

        [Header("Runtime Text Nodes")]
        [SerializeField] private RectTransform npcSlot1Container;
        [SerializeField] private RectTransform npcSlot2Container;
        [SerializeField] private RectTransform playerSlot1Container;
        [SerializeField] private TMP_Text npcSlot1Text;
        [SerializeField] private TMP_Text npcSlot2Text;
        [SerializeField] private TMP_Text playerSlot1Text;

        [Header("Input Rules")]
        [SerializeField] private bool forceResetSlot2WhenNewNpcLineArrives = true;
        [SerializeField] private string[] playerSpeakerKeywords =
        {
            "玩家",
            "主角",
            "我",
            "内心",
            "心声",
            "player",
            "narrator"
        };
        [SerializeField] private string[] thoughtKeywords = { "内心", "心声", "thought", "mind" };

        [Header("Animation")]
        [SerializeField] private float textFadeDuration = 0.18f;
        [SerializeField] private float textSlideDistance = 20f;
        [SerializeField] private Ease textEase = Ease.OutCubic;
        [SerializeField] private bool animateFrameShells = true;
        [SerializeField] private float frameMoveDuration = 0.46f;
        [SerializeField] private float frameFlipAngle = 72f;
        [SerializeField] private Ease frameShellEase = Ease.OutCubic;
        [SerializeField] private bool animateHistoryScrollFrameFlip = true;
        [SerializeField] private float historyScrollFlipAngle = 22f;
        [SerializeField] private float historyScrollFlipDuration = 0.14f;
        [SerializeField] private bool freezeHistoryHitAreaWhileBrowsing = true;
        [SerializeField, Range(0f, 1f)] private float npcSlot1RefreshDelayRatio = 0.42f;
        [SerializeField] private float slot2CarryFadeFrom = 0.55f;
        [SerializeField] private float slot2CarryTextNudge = 16f;

        [Header("History Limits")]
        [SerializeField] private int maxNpcHistoryRecords = 128;
        [SerializeField] private int maxTraceRecords = 256;
        [SerializeField] private bool verboseTraceLog = false;

        [Header("Placeholder Minigame")]
        [SerializeField] private Key placeholderCompleteKey = Key.Y;
        [SerializeField] private float placeholderFadeDuration = 0.18f;
        [SerializeField] private CanvasGroup placeholderOverlayGroup;
        [SerializeField] private Image placeholderOverlayImage;
        [SerializeField] private TMP_Text placeholderOverlayText;

        [Header("Trace Preview (Debug)")]
        [SerializeField, TextArea(3, 10)] private string latestTracePreview;

        private readonly List<NpcLineRecord> npcHistory = new();
        private readonly List<TraceRecord> traceRecords = new();
        private readonly HashSet<string> playerSpeakerSet = new(StringComparer.OrdinalIgnoreCase);
        private readonly Vector3[] frameWorldCorners = new Vector3[4];
        private static readonly BindingFlags RunnerBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private DialogueRunner commandRunner;
        private bool commandsRegistered;
        private bool pointerInsideHistoryArea;
        private bool browsingHistory;
        private bool placeholderRunning;
        private int slot2DefaultIndex = -1;
        private int slot2BrowseIndex = -1;
        private string activeNpcId = string.Empty;
        private SignDialogueRole roleOverride = SignDialogueRole.Auto;

        private CanvasGroup npcSlot1Group;
        private CanvasGroup npcSlot2Group;
        private CanvasGroup playerSlot1Group;
        private CanvasGroup npcSlot1FrameGroup;
        private CanvasGroup npcSlot2FrameGroup;
        private CanvasGroup playerSlot1FrameGroup;

        private bool frameDefaultsCaptured;
        private Vector2 npcSlot1FrameDefaultAnchoredPosition;
        private Vector2 npcSlot2FrameDefaultAnchoredPosition;
        private Vector2 playerSlot1FrameDefaultAnchoredPosition;
        private Vector3 npcSlot1FrameDefaultScale;
        private Vector3 npcSlot2FrameDefaultScale;
        private Vector3 playerSlot1FrameDefaultScale;
        private Vector3 npcSlot1FrameDefaultEuler;
        private Vector3 npcSlot2FrameDefaultEuler;
        private Vector3 playerSlot1FrameDefaultEuler;

        public bool IsRoutingEnabled => enableSignSlotRouting;
        public bool ShouldSuppressLegacyPresenterVisuals => suppressLegacyPresenterVisuals;
        public bool IsContinueInputBlocked => placeholderRunning || IsOptionsBlockingContinue();

        private void Awake()
        {
            TryFindSceneReferences();
            EnsureDialogueRunnerPresenters();
            BuildPlayerSpeakerLookup();
            EnsureTextHierarchy();
            EnsureFrameShellBindings();
            EnsureOverlayHierarchy();
            HideAllSlotsImmediately();
        }

        private void OnEnable()
        {
            RegisterCommands();
        }

        private void Start()
        {
            TryFindSceneReferences();
            EnsureDialogueRunnerPresenters();
            RegisterCommands();
            EnsureFrameShellBindings();
            SyncAllContainersToFrames();
            if (placeholderOverlayGroup != null)
            {
                placeholderOverlayGroup.alpha = 0f;
                placeholderOverlayGroup.gameObject.SetActive(false);
            }

            if (Application.isPlaying && autoStartDialogueIfIdle)
            {
                StartCoroutine(EnsureDialogueStartedNextFrame());
            }
        }

        private void Update()
        {
            if (!enableSignSlotRouting)
            {
                return;
            }

            UpdateHistoryScrollInput();
            UpdatePlaceholderCompletionInput();
        }

        private void LateUpdate()
        {
            if (!enableSignSlotRouting)
            {
                return;
            }

            SyncAllContainersToFrames();
        }

        private void OnDisable()
        {
            UnregisterCommands();
            placeholderRunning = false;
            HidePlaceholderImmediately();
        }

        public void RouteLine(string speaker, string content)
        {
            if (!enableSignSlotRouting)
            {
                return;
            }

            var visibleText = ToVisibleText(content);
            if (string.IsNullOrWhiteSpace(visibleText))
            {
                return;
            }

            var resolvedRole = ResolveRole(speaker, visibleText);
            switch (resolvedRole)
            {
                case SignDialogueRole.Npc:
                    PresentNpcLine(speaker, visibleText);
                    break;
                case SignDialogueRole.Player:
                case SignDialogueRole.Thought:
                    PresentPlayerLine(visibleText);
                    break;
                default:
                    PresentPlayerLine(visibleText);
                    break;
            }

            AppendTrace(resolvedRole, speaker, visibleText);
        }

        public void ResetNpcCycle(string npcId, bool keepVisible)
        {
            activeNpcId = string.IsNullOrWhiteSpace(npcId) ? string.Empty : npcId.Trim();
            npcHistory.Clear();
            slot2DefaultIndex = -1;
            slot2BrowseIndex = -1;
            browsingHistory = false;
            pointerInsideHistoryArea = false;

            if (npcSlot1Text != null)
            {
                npcSlot1Text.text = string.Empty;
            }

            if (npcSlot2Text != null)
            {
                npcSlot2Text.text = string.Empty;
            }

            SetSlotVisible(npcSlot1Group, keepVisible);
            SetSlotVisible(npcSlot2Group, false);
            SetShellVisible(npcSlot1FrameGroup, keepVisible);
            SetShellVisible(npcSlot2FrameGroup, false);
            ResetFrameShellToDefault(npcSlot1Frame, npcSlot1FrameDefaultAnchoredPosition, npcSlot1FrameDefaultScale, npcSlot1FrameDefaultEuler);
            ResetFrameShellToDefault(npcSlot2Frame, npcSlot2FrameDefaultAnchoredPosition, npcSlot2FrameDefaultScale, npcSlot2FrameDefaultEuler);
        }

        public void HideNpcCycle()
        {
            activeNpcId = string.Empty;
            npcHistory.Clear();
            slot2DefaultIndex = -1;
            slot2BrowseIndex = -1;
            browsingHistory = false;
            pointerInsideHistoryArea = false;
            SetSlotVisible(npcSlot1Group, false);
            SetSlotVisible(npcSlot2Group, false);
            SetShellVisible(npcSlot1FrameGroup, false);
            SetShellVisible(npcSlot2FrameGroup, false);
            ResetFrameShellToDefault(npcSlot1Frame, npcSlot1FrameDefaultAnchoredPosition, npcSlot1FrameDefaultScale, npcSlot1FrameDefaultEuler);
            ResetFrameShellToDefault(npcSlot2Frame, npcSlot2FrameDefaultAnchoredPosition, npcSlot2FrameDefaultScale, npcSlot2FrameDefaultEuler);
        }

        private void TryFindSceneReferences()
        {
            if (panelRect == null)
            {
                panelRect = transform as RectTransform;
            }

            if (rootCanvas == null)
            {
                rootCanvas = GetComponentInParent<Canvas>();
            }

            if (dialogueRunner == null)
            {
                dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            }

            if (optionsPresenter == null)
            {
                optionsPresenter = GetComponentInChildren<OptionsPresenter>(true);
            }

            if (optionsPanelRect == null && optionsPresenter != null)
            {
                optionsPanelRect = optionsPresenter.transform as RectTransform;
            }

            if (textTemplate == null)
            {
                textTemplate = transform.Find("LineText/Text (TMP)")?.GetComponent<TMP_Text>();
            }

            // Use the scene hierarchy root so we can reach sibling branches like "纯背景".
            var root = rootCanvas != null ? rootCanvas.transform.root : transform.root;
            if (root == null)
            {
                return;
            }

            if (npcSlot1Frame == null)
            {
                npcSlot1Frame = root.Find("纯背景/对话框遮罩/对话框1号") as RectTransform;
            }

            if (npcSlot2Frame == null)
            {
                npcSlot2Frame = root.Find("纯背景/对话框遮罩/对话框2号") as RectTransform;
            }

            if (playerSlot1Frame == null)
            {
                playerSlot1Frame = root.Find("纯背景/玩家框遮罩/玩家框1号") as RectTransform;
            }
        }

        private void BuildPlayerSpeakerLookup()
        {
            playerSpeakerSet.Clear();

            if (playerSpeakerKeywords == null)
            {
                return;
            }

            foreach (var keyword in playerSpeakerKeywords)
            {
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    playerSpeakerSet.Add(keyword.Trim());
                }
            }
        }

        private void EnsureTextHierarchy()
        {
            if (panelRect == null)
            {
                return;
            }

            if (textRoot == null)
            {
                var existing = panelRect.Find("SignSlotTextRoot") as RectTransform;
                if (existing == null)
                {
                    var rootObject = new GameObject("SignSlotTextRoot", typeof(RectTransform));
                    existing = rootObject.GetComponent<RectTransform>();
                    existing.SetParent(panelRect, false);
                }

                textRoot = existing;
            }

            ConfigureStretchRect(textRoot);

            npcSlot1Container = EnsureContainer(npcSlot1Container, "NPCSlot1Container");
            npcSlot2Container = EnsureContainer(npcSlot2Container, "NPCSlot2Container");
            playerSlot1Container = EnsureContainer(playerSlot1Container, "PlayerSlot1Container");

            npcSlot1Text = EnsureSlotText(npcSlot1Text, npcSlot1Container, "NPCSlot1Text");
            npcSlot2Text = EnsureSlotText(npcSlot2Text, npcSlot2Container, "NPCSlot2Text");
            playerSlot1Text = EnsureSlotText(playerSlot1Text, playerSlot1Container, "PlayerSlot1Text");

            npcSlot1Group = EnsureCanvasGroup(npcSlot1Container);
            npcSlot2Group = EnsureCanvasGroup(npcSlot2Container);
            playerSlot1Group = EnsureCanvasGroup(playerSlot1Container);

            if (npcHistoryScrollArea == null && npcSlot2Frame != null)
            {
                var scrollArea = new GameObject("NPCSlot2ScrollArea", typeof(RectTransform), typeof(Image));
                scrollArea.transform.SetParent(textRoot, false);
                npcHistoryScrollArea = scrollArea.GetComponent<RectTransform>();
                var image = scrollArea.GetComponent<Image>();
                image.color = new Color(0f, 0f, 0f, 0f);
                image.raycastTarget = false;
            }

            if (optionsPresenter != null && textRoot.GetSiblingIndex() >= optionsPresenter.transform.GetSiblingIndex())
            {
                textRoot.SetSiblingIndex(Mathf.Max(0, optionsPresenter.transform.GetSiblingIndex() - 1));
            }

            if (optionsPanelRect == null && optionsPresenter != null)
            {
                optionsPanelRect = optionsPresenter.transform as RectTransform;
            }
        }

        private void EnsureOverlayHierarchy()
        {
            if (panelRect == null)
            {
                return;
            }

            if (placeholderOverlayGroup == null)
            {
                var existing = panelRect.Find("SignPlaceholderMinigameOverlay") as RectTransform;
                if (existing == null)
                {
                    var overlayObject = new GameObject(
                        "SignPlaceholderMinigameOverlay",
                        typeof(RectTransform),
                        typeof(CanvasGroup),
                        typeof(Image));
                    existing = overlayObject.GetComponent<RectTransform>();
                    existing.SetParent(panelRect, false);
                }

                placeholderOverlayGroup = existing.GetComponent<CanvasGroup>();
                placeholderOverlayImage = existing.GetComponent<Image>();
            }

            var overlayRect = placeholderOverlayGroup.transform as RectTransform;
            ConfigureStretchRect(overlayRect);
            overlayRect.SetAsLastSibling();

            if (placeholderOverlayImage != null)
            {
                placeholderOverlayImage.raycastTarget = true;
                placeholderOverlayImage.color = new Color(0.12f, 0.18f, 0.24f, 0.86f);
            }

            if (placeholderOverlayText == null)
            {
                placeholderOverlayText = overlayRect.Find("Label")?.GetComponent<TMP_Text>();
                if (placeholderOverlayText == null)
                {
                    placeholderOverlayText = CreateTextClone(overlayRect, "Label");
                }

                if (placeholderOverlayText != null)
                {
                    placeholderOverlayText.alignment = TextAlignmentOptions.Center;
                    placeholderOverlayText.textWrappingMode = TextWrappingModes.Normal;
                    placeholderOverlayText.raycastTarget = false;
                    placeholderOverlayText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    placeholderOverlayText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    placeholderOverlayText.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    placeholderOverlayText.rectTransform.sizeDelta = new Vector2(900f, 320f);
                    placeholderOverlayText.rectTransform.anchoredPosition = Vector2.zero;
                }
            }
        }

        private RectTransform EnsureContainer(RectTransform container, string objectName)
        {
            if (container == null && textRoot != null)
            {
                container = textRoot.Find(objectName) as RectTransform;
            }

            if (container == null && textRoot != null)
            {
                var go = new GameObject(objectName, typeof(RectTransform), typeof(CanvasGroup));
                container = go.GetComponent<RectTransform>();
                container.SetParent(textRoot, false);
            }

            if (container != null)
            {
                container.anchorMin = new Vector2(0.5f, 0.5f);
                container.anchorMax = new Vector2(0.5f, 0.5f);
                container.pivot = new Vector2(0.5f, 0.5f);
                container.localScale = Vector3.one;
            }

            return container;
        }

        private TMP_Text EnsureSlotText(TMP_Text slotText, RectTransform container, string objectName)
        {
            if (slotText == null && container != null)
            {
                slotText = container.Find(objectName)?.GetComponent<TMP_Text>();
            }

            if (slotText == null && container != null)
            {
                slotText = CreateTextClone(container, objectName);
            }

            if (slotText != null)
            {
                slotText.raycastTarget = false;
                slotText.textWrappingMode = TextWrappingModes.Normal;
                slotText.text = string.Empty;
                var rect = slotText.rectTransform;
                ConfigureStretchRect(rect);
            }

            return slotText;
        }

        private TMP_Text CreateTextClone(RectTransform parent, string objectName)
        {
            var go = new GameObject(objectName, typeof(RectTransform), typeof(TextMeshProUGUI));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            ConfigureStretchRect(rect);

            var text = go.GetComponent<TextMeshProUGUI>();
            if (textTemplate != null)
            {
                text.font = textTemplate.font;
                text.fontSharedMaterial = textTemplate.fontSharedMaterial;
                text.fontSize = textTemplate.fontSize;
                text.fontStyle = textTemplate.fontStyle;
                text.color = textTemplate.color;
                text.alignment = textTemplate.alignment;
                text.richText = textTemplate.richText;
                text.enableAutoSizing = false;
                text.overflowMode = textTemplate.overflowMode;
                text.lineSpacing = textTemplate.lineSpacing;
                text.characterSpacing = textTemplate.characterSpacing;
                text.wordSpacing = textTemplate.wordSpacing;
                text.paragraphSpacing = textTemplate.paragraphSpacing;
                text.margin = textTemplate.margin;
                text.extraPadding = textTemplate.extraPadding;
            }
            else
            {
                text.font = TMP_Settings.defaultFontAsset;
                text.fontSize = 30f;
                text.color = Color.white;
                text.alignment = TextAlignmentOptions.TopLeft;
                text.overflowMode = TextOverflowModes.Overflow;
            }

            text.textWrappingMode = TextWrappingModes.Normal;
            text.raycastTarget = false;
            return text;
        }

        private static CanvasGroup EnsureCanvasGroup(Component target)
        {
            if (target == null)
            {
                return null;
            }

            if (target.TryGetComponent<CanvasGroup>(out var existing))
            {
                return existing;
            }

            return target.gameObject.AddComponent<CanvasGroup>();
        }

        private static void ConfigureStretchRect(RectTransform rect)
        {
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.localScale = Vector3.one;
        }

        private void EnsureFrameShellBindings()
        {
            npcSlot1FrameGroup = EnsureCanvasGroup(npcSlot1Frame);
            npcSlot2FrameGroup = EnsureCanvasGroup(npcSlot2Frame);
            playerSlot1FrameGroup = EnsureCanvasGroup(playerSlot1Frame);
            CaptureFrameDefaults();
        }

        private void CaptureFrameDefaults()
        {
            if (frameDefaultsCaptured)
            {
                return;
            }

            CaptureFrameDefault(
                npcSlot1Frame,
                ref npcSlot1FrameDefaultAnchoredPosition,
                ref npcSlot1FrameDefaultScale,
                ref npcSlot1FrameDefaultEuler);
            CaptureFrameDefault(
                npcSlot2Frame,
                ref npcSlot2FrameDefaultAnchoredPosition,
                ref npcSlot2FrameDefaultScale,
                ref npcSlot2FrameDefaultEuler);
            CaptureFrameDefault(
                playerSlot1Frame,
                ref playerSlot1FrameDefaultAnchoredPosition,
                ref playerSlot1FrameDefaultScale,
                ref playerSlot1FrameDefaultEuler);

            frameDefaultsCaptured = npcSlot1Frame != null || npcSlot2Frame != null || playerSlot1Frame != null;
        }

        private static void CaptureFrameDefault(
            RectTransform frame,
            ref Vector2 anchoredPosition,
            ref Vector3 scale,
            ref Vector3 euler)
        {
            if (frame == null)
            {
                return;
            }

            anchoredPosition = frame.anchoredPosition;
            scale = frame.localScale;
            euler = frame.localEulerAngles;
        }

        private void RegisterCommands()
        {
            if (commandsRegistered)
            {
                return;
            }

            if (dialogueRunner == null)
            {
                dialogueRunner = FindFirstObjectByType<DialogueRunner>();
            }

            if (dialogueRunner == null)
            {
                return;
            }

            dialogueRunner.AddCommandHandler<string>(NpcEnterCommand, HandleNpcEnterCommand);
            dialogueRunner.AddCommandHandler(NpcExitCommand, HandleNpcExitCommand);
            dialogueRunner.AddCommandHandler<string>(RoleCommand, HandleRoleCommand);
            dialogueRunner.AddCommandHandler<string>(PlaceholderMinigameCommand, RunPlaceholderMinigameCommand);

            commandRunner = dialogueRunner;
            commandsRegistered = true;
        }

        private IEnumerator EnsureDialogueStartedNextFrame()
        {
            var node = string.IsNullOrWhiteSpace(startupNodeName) ? "Start" : startupNodeName.Trim();
            for (var attempt = 0; attempt < 4; attempt++)
            {
                yield return null;

                if (dialogueRunner == null)
                {
                    yield break;
                }

                if (dialogueRunner.IsDialogueRunning)
                {
                    yield break;
                }

                _ = dialogueRunner.StartDialogue(node);
            }

            if (dialogueRunner != null && !dialogueRunner.IsDialogueRunning)
            {
                LogKit.W($"[SignDialogueSlotRuntime] Failed to auto-start node '{node}'.");
            }
        }

        private void EnsureDialogueRunnerPresenters()
        {
            if (dialogueRunner == null)
            {
                return;
            }

            var linePresenter = GetComponent<TALinePresenter>();
            if (linePresenter == null)
            {
                return;
            }

            var presenterList = ResolvePresenterList(dialogueRunner);
            if (presenterList == null)
            {
                return;
            }

            var mutated = false;
            for (var i = presenterList.Count - 1; i >= 0; i--)
            {
                if (presenterList[i] == null)
                {
                    presenterList.RemoveAt(i);
                    mutated = true;
                }
            }

            var lineIndex = presenterList.IndexOf(linePresenter);
            if (lineIndex < 0)
            {
                presenterList.Insert(0, linePresenter);
                mutated = true;
            }
            else if (lineIndex > 0)
            {
                presenterList.RemoveAt(lineIndex);
                presenterList.Insert(0, linePresenter);
                mutated = true;
            }

            if (optionsPresenter != null && !presenterList.Contains(optionsPresenter))
            {
                presenterList.Add(optionsPresenter);
                mutated = true;
            }

            if (TrySetRunnerOptionFallthrough(dialogueRunner, false))
            {
                mutated = true;
            }

            if (mutated && Application.isEditor && !Application.isPlaying)
            {
                // Keep this scene fix persisted after save.
#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(dialogueRunner);
#endif
            }
        }

        private static List<DialoguePresenterBase> ResolvePresenterList(DialogueRunner runner)
        {
            if (runner == null)
            {
                return null;
            }

            var runnerType = runner.GetType();
            var field = runnerType.GetField("dialoguePresenters", RunnerBindingFlags) ??
                        runnerType.GetField("dialogueViews", RunnerBindingFlags);
            if (field == null)
            {
                return null;
            }

            if (field.GetValue(runner) is List<DialoguePresenterBase> list)
            {
                return list;
            }

            return null;
        }

        private static bool TrySetRunnerOptionFallthrough(DialogueRunner runner, bool value)
        {
            if (runner == null)
            {
                return false;
            }

            var runnerType = runner.GetType();
            var field = runnerType.GetField("allowOptionFallthrough", RunnerBindingFlags);
            if (field != null && field.FieldType == typeof(bool))
            {
                var current = (bool)field.GetValue(runner);
                if (current == value)
                {
                    return false;
                }

                field.SetValue(runner, value);
                return true;
            }

            var property = runnerType.GetProperty("AllowOptionFallthrough", RunnerBindingFlags);
            if (property != null && property.PropertyType == typeof(bool) && property.CanRead && property.CanWrite)
            {
                var current = (bool)property.GetValue(runner);
                if (current == value)
                {
                    return false;
                }

                property.SetValue(runner, value);
                return true;
            }

            return false;
        }

        private void UnregisterCommands()
        {
            if (!commandsRegistered || commandRunner == null)
            {
                return;
            }

            commandRunner.RemoveCommandHandler(NpcEnterCommand);
            commandRunner.RemoveCommandHandler(NpcExitCommand);
            commandRunner.RemoveCommandHandler(RoleCommand);
            commandRunner.RemoveCommandHandler(PlaceholderMinigameCommand);
            commandRunner = null;
            commandsRegistered = false;
        }

        private void HandleNpcEnterCommand(string npcId)
        {
            ResetNpcCycle(npcId, false);
        }

        private void HandleNpcExitCommand()
        {
            HideNpcCycle();
        }

        private void HandleRoleCommand(string roleToken)
        {
            roleOverride = ParseRoleToken(roleToken);
        }

        private IEnumerator RunPlaceholderMinigameCommand(string token)
        {
            yield return ShowPlaceholderMinigame(token);
        }

        private IEnumerator ShowPlaceholderMinigame(string token)
        {
            EnsureOverlayHierarchy();

            if (placeholderOverlayGroup == null || placeholderOverlayImage == null)
            {
                yield break;
            }

            if (placeholderRunning)
            {
                while (placeholderRunning)
                {
                    yield return null;
                }

                yield break;
            }

            placeholderRunning = true;
            placeholderOverlayGroup.gameObject.SetActive(true);
            placeholderOverlayGroup.blocksRaycasts = true;
            placeholderOverlayGroup.interactable = true;
            placeholderOverlayImage.color = ResolvePlaceholderColor(token);
            if (placeholderOverlayText != null)
            {
                placeholderOverlayText.text = BuildPlaceholderText(token);
            }

            placeholderOverlayGroup.DOKill();
            placeholderOverlayGroup.alpha = 0f;
            placeholderOverlayGroup
                .DOFade(1f, placeholderFadeDuration)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic);

            while (placeholderRunning)
            {
                yield return null;
            }

            placeholderOverlayGroup.DOKill();
            yield return placeholderOverlayGroup
                .DOFade(0f, placeholderFadeDuration)
                .SetUpdate(true)
                .SetEase(Ease.InCubic)
                .WaitForCompletion();

            placeholderOverlayGroup.blocksRaycasts = false;
            placeholderOverlayGroup.interactable = false;
            placeholderOverlayGroup.gameObject.SetActive(false);
        }

        private void HidePlaceholderImmediately()
        {
            if (placeholderOverlayGroup == null)
            {
                return;
            }

            placeholderOverlayGroup.DOKill();
            placeholderOverlayGroup.alpha = 0f;
            placeholderOverlayGroup.blocksRaycasts = false;
            placeholderOverlayGroup.interactable = false;
            placeholderOverlayGroup.gameObject.SetActive(false);
        }

        private static Color ResolvePlaceholderColor(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return new Color(0.12f, 0.18f, 0.24f, 0.86f);
            }

            var normalized = token.Trim().ToLowerInvariant();
            return normalized switch
            {
                "doc" or "document" or "review" => new Color(0.11f, 0.25f, 0.35f, 0.88f),
                "rune" or "typing" => new Color(0.32f, 0.18f, 0.08f, 0.88f),
                "stamp" => new Color(0.31f, 0.13f, 0.13f, 0.88f),
                "soul" => new Color(0.12f, 0.30f, 0.24f, 0.88f),
                _ => new Color(0.16f, 0.20f, 0.26f, 0.88f)
            };
        }

        private string BuildPlaceholderText(string token)
        {
            var label = string.IsNullOrWhiteSpace(token) ? "placeholder_minigame" : token.Trim();
            return $"占位小游戏: {label}\n按 {placeholderCompleteKey} 键完成";
        }

        private void UpdatePlaceholderCompletionInput()
        {
            if (!placeholderRunning || Keyboard.current == null)
            {
                return;
            }

            if (Keyboard.current[placeholderCompleteKey].wasPressedThisFrame)
            {
                placeholderRunning = false;
            }
        }

        private SignDialogueRole ParseRoleToken(string roleToken)
        {
            if (string.IsNullOrWhiteSpace(roleToken))
            {
                return SignDialogueRole.Auto;
            }

            var normalized = roleToken.Trim().ToLowerInvariant();
            return normalized switch
            {
                "npc" => SignDialogueRole.Npc,
                "player" => SignDialogueRole.Player,
                "thought" => SignDialogueRole.Thought,
                "inner" => SignDialogueRole.Thought,
                "auto" => SignDialogueRole.Auto,
                _ => SignDialogueRole.Auto
            };
        }

        private SignDialogueRole ResolveRole(string speaker, string content)
        {
            if (roleOverride != SignDialogueRole.Auto)
            {
                return roleOverride;
            }

            if (!string.IsNullOrWhiteSpace(speaker))
            {
                if (IsPlayerSpeaker(speaker))
                {
                    return SignDialogueRole.Player;
                }

                return SignDialogueRole.Npc;
            }

            return ContainsThoughtKeyword(content) ? SignDialogueRole.Thought : SignDialogueRole.Player;
        }

        private bool IsPlayerSpeaker(string speaker)
        {
            if (string.IsNullOrWhiteSpace(speaker) || playerSpeakerSet.Count == 0)
            {
                return false;
            }

            foreach (var keyword in playerSpeakerSet)
            {
                if (speaker.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool ContainsThoughtKeyword(string content)
        {
            if (string.IsNullOrWhiteSpace(content) || thoughtKeywords == null)
            {
                return false;
            }

            foreach (var keyword in thoughtKeywords)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    continue;
                }

                if (content.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void PresentNpcLine(string speaker, string text)
        {
            var npcId = string.IsNullOrWhiteSpace(speaker) ? activeNpcId : speaker.Trim();
            if (string.IsNullOrWhiteSpace(npcId))
            {
                npcId = "NPC";
            }

            if (!string.IsNullOrWhiteSpace(activeNpcId) &&
                !string.Equals(activeNpcId, npcId, StringComparison.OrdinalIgnoreCase))
            {
                ResetNpcCycle(npcId, false);
            }
            else if (string.IsNullOrWhiteSpace(activeNpcId))
            {
                activeNpcId = npcId;
            }

            var hasPreviousLatest = npcHistory.Count > 0;

            npcHistory.Add(new NpcLineRecord
            {
                NpcId = npcId,
                Text = text,
                Timestamp = DateTime.Now.ToString("HH:mm:ss.fff")
            });

            if (maxNpcHistoryRecords > 0 && npcHistory.Count > maxNpcHistoryRecords)
            {
                npcHistory.RemoveAt(0);
            }

            if (npcSlot1Text != null)
            {
                npcSlot1Text.text = text;
            }

            slot2DefaultIndex = Mathf.Clamp(npcHistory.Count - 2, -1, npcHistory.Count - 1);
            if (slot2DefaultIndex >= 0)
            {
                if (forceResetSlot2WhenNewNpcLineArrives || !browsingHistory)
                {
                    browsingHistory = false;
                    slot2BrowseIndex = slot2DefaultIndex;
                }
                else
                {
                    slot2BrowseIndex = Mathf.Clamp(slot2BrowseIndex, 0, slot2DefaultIndex);
                }

                RefreshSlot2TextByIndex(slot2BrowseIndex);
            }
            else
            {
                slot2BrowseIndex = -1;
                if (npcSlot2Text != null)
                {
                    npcSlot2Text.text = string.Empty;
                }
            }

            SetSlotVisible(npcSlot1Group, true);
            SetSlotVisible(npcSlot2Group, slot2DefaultIndex >= 0);
            SetShellVisible(npcSlot1FrameGroup, true);
            SetShellVisible(npcSlot2FrameGroup, slot2DefaultIndex >= 0);
            PlayNpcUpdateAnimation(hasPreviousLatest, slot2DefaultIndex >= 0);
        }

        private void PresentPlayerLine(string text)
        {
            if (playerSlot1Text != null)
            {
                playerSlot1Text.text = text;
            }

            SetSlotVisible(playerSlot1Group, true);
            SetShellVisible(playerSlot1FrameGroup, true);
            PlayTextTransition(playerSlot1Group, playerSlot1Text, 1f);
        }

        private void RefreshSlot2TextByIndex(int index)
        {
            if (npcSlot2Text == null || index < 0 || index >= npcHistory.Count)
            {
                return;
            }

            npcSlot2Text.text = npcHistory[index].Text;
        }

        private void PlayNpcUpdateAnimation(bool hasPreviousLatest, bool hasSlot2)
        {
            if (hasPreviousLatest && hasSlot2)
            {
                var slot1Delay = Mathf.Max(0f, frameMoveDuration * Mathf.Clamp01(npcSlot1RefreshDelayRatio));
                PlaySlot2CarryOverAnimation();
                PlayTextTransition(npcSlot1Group, npcSlot1Text, 1f, slot1Delay);
            }
            else
            {
                PlayTextTransition(npcSlot1Group, npcSlot1Text, 1f);
                if (hasSlot2)
                {
                    PlayTextTransition(npcSlot2Group, npcSlot2Text, -1f);
                }
            }

            PlayFrameShellShiftAnimation(hasPreviousLatest, hasSlot2);
        }

        private void PlaySlot2CarryOverAnimation()
        {
            if (npcSlot2Group == null || npcSlot2Text == null)
            {
                return;
            }

            npcSlot2Group.DOKill();
            npcSlot2Text.rectTransform.DOKill();

            var duration = Mathf.Max(textFadeDuration, frameMoveDuration * 0.85f);
            npcSlot2Group.alpha = Mathf.Clamp01(slot2CarryFadeFrom);
            npcSlot2Text.rectTransform.anchoredPosition = new Vector2(0f, -Mathf.Abs(slot2CarryTextNudge));

            npcSlot2Group
                .DOFade(1f, duration)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic);

            npcSlot2Text.rectTransform
                .DOAnchorPos(Vector2.zero, duration)
                .SetUpdate(true)
                .SetEase(textEase);
        }

        private void PlayTextTransition(CanvasGroup group, TMP_Text text, float direction, float delay = 0f)
        {
            if (group == null || text == null)
            {
                return;
            }

            group.DOKill();
            text.rectTransform.DOKill();

            void Animate()
            {
                if (group == null || text == null)
                {
                    return;
                }

                text.rectTransform.anchoredPosition = new Vector2(0f, direction * textSlideDistance);
                group.alpha = 0f;

                group
                    .DOFade(1f, textFadeDuration)
                    .SetUpdate(true)
                    .SetEase(textEase);

                text.rectTransform
                    .DOAnchorPos(Vector2.zero, textFadeDuration)
                    .SetUpdate(true)
                    .SetEase(textEase);
            }

            if (delay <= 0.001f)
            {
                Animate();
                return;
            }

            group.alpha = 0f;
            text.rectTransform.anchoredPosition = new Vector2(0f, direction * textSlideDistance);
            DOVirtual.DelayedCall(delay, Animate, true).SetUpdate(true);
        }

        private void UpdateHistoryScrollInput()
        {
            if (slot2DefaultIndex < 0 || npcHistory.Count == 0)
            {
                browsingHistory = false;
                pointerInsideHistoryArea = false;
                return;
            }

            if (Mouse.current == null)
            {
                return;
            }

            var scrollRect = npcHistoryScrollArea != null ? npcHistoryScrollArea : npcSlot2Frame;
            if (scrollRect == null)
            {
                return;
            }

            var pointerPosition = Mouse.current.position.ReadValue();
            var eventCamera = rootCanvas != null ? rootCanvas.worldCamera : null;
            var inside = RectTransformUtility.RectangleContainsScreenPoint(scrollRect, pointerPosition, eventCamera);

            if (inside)
            {
                var scrollValue = Mouse.current.scroll.ReadValue().y;
                if (Mathf.Abs(scrollValue) > 0.01f)
                {
                    browsingHistory = true;
                    var delta = scrollValue > 0f ? -1 : 1;
                    var nextIndex = Mathf.Clamp(slot2BrowseIndex + delta, 0, slot2DefaultIndex);
                    if (nextIndex != slot2BrowseIndex)
                    {
                        slot2BrowseIndex = nextIndex;
                        RefreshSlot2TextByIndex(slot2BrowseIndex);
                        PlayTextTransition(npcSlot2Group, npcSlot2Text, -Mathf.Sign(delta));
                        PlayFrameShellFlipAnimation(-Mathf.Sign(delta));
                    }
                }
            }
            else if (pointerInsideHistoryArea && browsingHistory)
            {
                browsingHistory = false;
                if (slot2DefaultIndex >= 0 && slot2BrowseIndex != slot2DefaultIndex)
                {
                    var direction = slot2BrowseIndex > slot2DefaultIndex ? 1f : -1f;
                    slot2BrowseIndex = slot2DefaultIndex;
                    RefreshSlot2TextByIndex(slot2BrowseIndex);
                    PlayTextTransition(npcSlot2Group, npcSlot2Text, direction);
                    PlayFrameShellFlipAnimation(direction);
                }
            }

            pointerInsideHistoryArea = inside;
        }

        private bool IsOptionsBlockingContinue()
        {
            if (optionsPresenter == null || !optionsPresenter.isActiveAndEnabled)
            {
                return false;
            }

            if (!optionsPresenter.gameObject.activeInHierarchy)
            {
                return false;
            }

            var optionItems = optionsPresenter.GetComponentsInChildren<OptionItem>(true);
            var hasActiveInteractableOption = false;
            foreach (var item in optionItems)
            {
                if (item == null || !item.gameObject.activeInHierarchy)
                {
                    continue;
                }

                if (item.interactable)
                {
                    hasActiveInteractableOption = true;
                    break;
                }
            }

            if (!hasActiveInteractableOption)
            {
                for (var i = 0; i < optionsPresenter.transform.childCount; i++)
                {
                    if (optionsPresenter.transform.GetChild(i).gameObject.activeInHierarchy)
                    {
                        hasActiveInteractableOption = true;
                        break;
                    }
                }
            }

            if (!hasActiveInteractableOption)
            {
                return false;
            }

            if (optionsPresenter.TryGetComponent<CanvasGroup>(out var group))
            {
                return group.alpha > 0.001f && group.interactable;
            }

            return true;
        }

        private void SyncAllContainersToFrames()
        {
            if (panelRect == null)
            {
                return;
            }

            SyncContainerToFrame(npcSlot1Frame, npcSlot1Container, npcSlotPadding);
            SyncContainerToFrame(npcSlot2Frame, npcSlot2Container, npcSlotPadding);
            SyncContainerToFrame(playerSlot1Frame, playerSlot1Container, playerSlotPadding);
            if (!freezeHistoryHitAreaWhileBrowsing || !browsingHistory)
            {
                SyncContainerToFrame(npcSlot2Frame, npcHistoryScrollArea, Vector2.zero);
            }

            if (optionsPanelRect == null && optionsPresenter != null)
            {
                optionsPanelRect = optionsPresenter.transform as RectTransform;
            }

            SyncOptionsPanelToFrame(playerSlot1Frame, optionsPanelRect);
        }

        private void SyncContainerToFrame(RectTransform frame, RectTransform container, Vector2 padding)
        {
            if (frame == null || container == null || panelRect == null)
            {
                return;
            }

            var eventCamera = rootCanvas != null ? rootCanvas.worldCamera : null;
            var frameCenterWorld = frame.TransformPoint(frame.rect.center);
            var frameCenterScreen = RectTransformUtility.WorldToScreenPoint(eventCamera, frameCenterWorld);

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    panelRect,
                    frameCenterScreen,
                    eventCamera,
                    out var frameCenterLocal))
            {
                container.anchoredPosition = frameCenterLocal;
            }

            frame.GetWorldCorners(frameWorldCorners);
            var worldWidth = Vector3.Distance(frameWorldCorners[0], frameWorldCorners[3]);
            var worldHeight = Vector3.Distance(frameWorldCorners[0], frameWorldCorners[1]);
            var panelScale = panelRect.lossyScale;
            var localWidth = worldWidth / Mathf.Max(0.0001f, panelScale.x);
            var localHeight = worldHeight / Mathf.Max(0.0001f, panelScale.y);

            container.sizeDelta = new Vector2(
                Mathf.Max(16f, localWidth - padding.x),
                Mathf.Max(16f, localHeight - padding.y));
        }

        private void SyncOptionsPanelToFrame(RectTransform frame, RectTransform optionsRect)
        {
            if (frame == null || optionsRect == null || panelRect == null)
            {
                return;
            }

            if (normalizeOptionsPanelAnchorToCenter)
            {
                optionsRect.anchorMin = new Vector2(0.5f, 0.5f);
                optionsRect.anchorMax = new Vector2(0.5f, 0.5f);
                optionsRect.pivot = new Vector2(0.5f, 0.5f);
            }

            var eventCamera = rootCanvas != null ? rootCanvas.worldCamera : null;
            var frameCenterWorld = frame.TransformPoint(frame.rect.center);
            var frameCenterScreen = RectTransformUtility.WorldToScreenPoint(eventCamera, frameCenterWorld);
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    panelRect,
                    frameCenterScreen,
                    eventCamera,
                    out var frameCenterLocal))
            {
                optionsRect.anchoredPosition = frameCenterLocal + optionsPanelOffset;
            }

            frame.GetWorldCorners(frameWorldCorners);
            var worldWidth = Vector3.Distance(frameWorldCorners[0], frameWorldCorners[3]);
            var worldHeight = Vector3.Distance(frameWorldCorners[0], frameWorldCorners[1]);
            var panelScale = panelRect.lossyScale;
            var localWidth = worldWidth / Mathf.Max(0.0001f, panelScale.x);
            var localHeight = worldHeight / Mathf.Max(0.0001f, panelScale.y);
            optionsRect.sizeDelta = new Vector2(
                Mathf.Max(16f, localWidth - optionsPanelPadding.x),
                Mathf.Max(16f, localHeight - optionsPanelPadding.y));
        }

        private void HideAllSlotsImmediately()
        {
            SetSlotVisible(npcSlot1Group, false);
            SetSlotVisible(npcSlot2Group, false);
            SetSlotVisible(playerSlot1Group, false);
            SetShellVisible(npcSlot1FrameGroup, false);
            SetShellVisible(npcSlot2FrameGroup, false);
            SetShellVisible(playerSlot1FrameGroup, false);
            ResetFrameShellToDefault(npcSlot1Frame, npcSlot1FrameDefaultAnchoredPosition, npcSlot1FrameDefaultScale, npcSlot1FrameDefaultEuler);
            ResetFrameShellToDefault(npcSlot2Frame, npcSlot2FrameDefaultAnchoredPosition, npcSlot2FrameDefaultScale, npcSlot2FrameDefaultEuler);
            ResetFrameShellToDefault(playerSlot1Frame, playerSlot1FrameDefaultAnchoredPosition, playerSlot1FrameDefaultScale, playerSlot1FrameDefaultEuler);
        }

        private static void SetSlotVisible(CanvasGroup group, bool visible)
        {
            if (group == null)
            {
                return;
            }

            group.alpha = visible ? 1f : 0f;
        }

        private static void SetShellVisible(CanvasGroup group, bool visible)
        {
            if (group == null)
            {
                return;
            }

            group.DOKill();
            group.alpha = visible ? 1f : 0f;
        }

        private static void ResetFrameShellToDefault(
            RectTransform frame,
            Vector2 defaultAnchoredPosition,
            Vector3 defaultScale,
            Vector3 defaultEuler)
        {
            if (frame == null)
            {
                return;
            }

            frame.DOKill();
            frame.anchoredPosition = defaultAnchoredPosition;
            frame.localScale = defaultScale == Vector3.zero ? Vector3.one : defaultScale;
            frame.localEulerAngles = defaultEuler;
        }

        private void PlayFrameShellShiftAnimation(bool hasPreviousLatest, bool hasSlot2)
        {
            if (!animateFrameShells || !frameDefaultsCaptured)
            {
                return;
            }

            if (npcSlot1Frame != null)
            {
                npcSlot1Frame.DOKill();
                npcSlot1Frame.anchoredPosition = npcSlot1FrameDefaultAnchoredPosition + new Vector2(0f, -26f);
                npcSlot1Frame.localScale = npcSlot1FrameDefaultScale * 0.90f;
                npcSlot1Frame.localEulerAngles = npcSlot1FrameDefaultEuler;

                npcSlot1Frame
                    .DOAnchorPos(npcSlot1FrameDefaultAnchoredPosition, frameMoveDuration)
                    .SetUpdate(true)
                    .SetEase(frameShellEase);
                npcSlot1Frame
                    .DOScale(npcSlot1FrameDefaultScale, frameMoveDuration)
                    .SetUpdate(true)
                    .SetEase(Ease.OutBack);
            }

            if (hasPreviousLatest && hasSlot2 && npcSlot2Frame != null)
            {
                npcSlot2Frame.DOKill();
                var slot2Start = npcSlot1Frame != null
                    ? npcSlot1FrameDefaultAnchoredPosition
                    : npcSlot2FrameDefaultAnchoredPosition + new Vector2(0f, -12f);
                npcSlot2Frame.anchoredPosition = slot2Start;
                npcSlot2Frame.localScale = npcSlot1Frame != null ? npcSlot1FrameDefaultScale : npcSlot2FrameDefaultScale * 0.97f;
                npcSlot2Frame.localEulerAngles = npcSlot2FrameDefaultEuler + new Vector3(frameFlipAngle * 0.65f, 0f, 0f);

                var seq = DOTween.Sequence().SetUpdate(true);
                seq.Join(npcSlot2Frame
                    .DOAnchorPos(npcSlot2FrameDefaultAnchoredPosition, frameMoveDuration)
                    .SetEase(frameShellEase));
                seq.Join(npcSlot2Frame
                    .DOScale(npcSlot2FrameDefaultScale, frameMoveDuration)
                    .SetEase(Ease.OutBack));
                seq.Join(npcSlot2Frame
                    .DOLocalRotate(npcSlot2FrameDefaultEuler, frameMoveDuration)
                    .SetEase(Ease.OutCubic));
            }
        }

        private void PlayFrameShellFlipAnimation(float direction)
        {
            if (!animateFrameShells || !animateHistoryScrollFrameFlip || !frameDefaultsCaptured || npcSlot2Frame == null)
            {
                return;
            }

            npcSlot2Frame.DOKill();
            var signedDirection = Mathf.Abs(direction) < 0.01f ? -1f : Mathf.Sign(direction);
            var flipAngle = Mathf.Max(0f, historyScrollFlipAngle);
            var flipDuration = Mathf.Max(0.01f, historyScrollFlipDuration);
            npcSlot2Frame.localEulerAngles = npcSlot2FrameDefaultEuler + new Vector3(flipAngle * signedDirection, 0f, 0f);
            npcSlot2Frame
                .DOLocalRotate(npcSlot2FrameDefaultEuler, flipDuration)
                .SetUpdate(true)
                .SetEase(Ease.OutCubic);
        }

        private void AppendTrace(SignDialogueRole role, string speaker, string text)
        {
            traceRecords.Add(new TraceRecord
            {
                Role = role.ToString(),
                Speaker = string.IsNullOrWhiteSpace(speaker) ? "-" : speaker.Trim(),
                Text = text,
                Timestamp = DateTime.Now.ToString("HH:mm:ss.fff")
            });

            if (maxTraceRecords > 0 && traceRecords.Count > maxTraceRecords)
            {
                traceRecords.RemoveAt(0);
            }

            RefreshTracePreview();

            if (verboseTraceLog)
            {
                LogKit.I($"[SignDialogue] {role} | {speaker} | {text}");
            }
        }

        private void RefreshTracePreview()
        {
            const int previewCount = 6;
            if (traceRecords.Count == 0)
            {
                latestTracePreview = string.Empty;
                return;
            }

            var start = Mathf.Max(0, traceRecords.Count - previewCount);
            var lines = new List<string>(previewCount);
            for (var i = start; i < traceRecords.Count; i++)
            {
                var record = traceRecords[i];
                lines.Add($"{record.Timestamp} [{record.Role}] {record.Speaker}: {record.Text}");
            }

            latestTracePreview = string.Join("\n", lines);
        }

        private static string ToVisibleText(string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return string.Empty;
            }

            var sanitized = AngleTagRegex.Replace(rawText, string.Empty);
            sanitized = sanitized.Replace("\\n", "\n");
            return sanitized.Trim();
        }
    }
}
