using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace ITC.Contracting
{
    public sealed class RuneTypingPanel : UIPanel, IController
    {
        private enum RuneTypingState
        {
            Countdown,
            Playing,
            FeedbackCorrect,
            FeedbackError,
            ComboResolve,
            Completed
        }

        [Header("Roots")]
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private RectTransform gameRoot;
        [SerializeField] private RectTransform targetSequenceRoot;
        [SerializeField] private RectTransform gridRoot;
        [SerializeField] private RectTransform cursorFrame;
        [SerializeField] private GameObject onScreenButtonsRoot;

        [Header("Text")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text errorCountText;
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private TMP_Text guideToggleLabel;
        [SerializeField] private TMP_Text currentTargetHighlight;

        [Header("Buttons")]
        [SerializeField] private Button guideToggleButton;
        [SerializeField] private Button upButton;
        [SerializeField] private Button downButton;
        [SerializeField] private Button leftButton;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button confirmButton;
        [SerializeField] private List<Button> gridCellButtons = new();

        [Header("Guide")]
        [SerializeField] private bool showGuideOnOpen = true;

        [Header("Visuals")]
        [SerializeField] private Color cellDefaultColor = new(0.19f, 0.20f, 0.23f, 1f);
        [SerializeField] private Color cellCursorColor = new(0.95f, 0.80f, 0.24f, 1f);
        [SerializeField] private Color cellCorrectColor = new(0.25f, 0.64f, 0.33f, 1f);
        [SerializeField] private Color cellErrorColor = new(0.74f, 0.25f, 0.23f, 1f);
        [SerializeField] private Color targetPendingColor = new(0.95f, 0.93f, 0.82f, 1f);
        [SerializeField] private Color targetDoneColor = new(0.46f, 0.88f, 0.49f, 1f);

        private readonly Dictionary<Button, UnityAction> cellClickActions = new();
        private readonly Dictionary<Button, TMP_Text> cellLabelLookup = new();
        private readonly Dictionary<Button, UnityAction> buttonActionLookup = new();
        private readonly List<TMP_Text> targetRuneTexts = new();
        private readonly List<RuneTypingGuideTag> guideTags = new();
        private readonly List<RuneInputDirection> runtimeGridLayout = new();
        private readonly List<RuneInputDirection> runtimeTargetSequence = new();

        private RuneTypingPanelData panelData = new();
        private RuneTypingState state;
        private int gridSize;
        private int totalGridCells;
        private int cursorCellIndex;
        private int targetSequenceIndex;
        private int errorCount;
        private bool usedOnScreenButtons;
        private bool guideVisible;
        private float nextInputAllowedAt;
        private float nextConfirmAllowedAt;

        private Coroutine countdownRoutine;
        private Coroutine feedbackRoutine;
        private Coroutine cursorMoveRoutine;
        private Coroutine completeRoutine;

        private RuneTypingConfig RuntimeConfig => panelData.RuntimeConfig ??= new RuneTypingConfig();

        private void Awake()
        {
            CacheReferences();
        }

        protected override void OnInit(IUIData uiData = null)
        {
            panelData = uiData as RuneTypingPanelData ?? new RuneTypingPanelData();
            CacheReferences();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            panelData = uiData as RuneTypingPanelData ?? panelData ?? new RuneTypingPanelData();
            EnsurePanelConfig();
            CacheReferences();
            RegisterRuntimeListeners();
            ResetRound();
        }

        protected override void OnClose()
        {
            UnregisterRuntimeListeners();
            StopAllPanelCoroutines();
            runtimeGridLayout.Clear();
            runtimeTargetSequence.Clear();
        }

        private void Update()
        {
            if (state != RuneTypingState.Playing)
            {
                return;
            }

            var now = Time.unscaledTime;

            if (now >= nextInputAllowedAt)
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    HandleMove(0, -1, false);
                    return;
                }

                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    HandleMove(0, 1, false);
                    return;
                }

                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    HandleMove(-1, 0, false);
                    return;
                }

                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    HandleMove(1, 0, false);
                    return;
                }
            }

            if (now >= nextConfirmAllowedAt &&
                (Input.GetKeyDown(KeyCode.Space) ||
                 Input.GetKeyDown(KeyCode.Return) ||
                 Input.GetKeyDown(KeyCode.KeypadEnter)))
            {
                HandleConfirm(false);
            }
        }

        private void EnsurePanelConfig()
        {
            var configModel = this.GetModel<ContractClientConfigModel>();

            if (panelData.RuntimeConfig == null)
            {
                panelData.RuntimeConfig = configModel.RuneTypingRuleConfig;
            }

            panelData.RuntimeConfig ??= new RuneTypingConfig();

            var safeGridSize = Mathf.Clamp(panelData.GridSize, 4, 5);
            if (panelData.RoundConfig == null)
            {
                panelData.RoundConfig = configModel.BuildRuneTypingRoundConfig(panelData.ClientId, safeGridSize);
            }
            else if (panelData.RoundConfig.GridSize < 4 || panelData.RoundConfig.GridSize > 5)
            {
                panelData.RoundConfig.GridSize = safeGridSize;
            }
        }

        private void CacheReferences()
        {
            if (panelRoot == null)
            {
                panelRoot = transform as RectTransform;
            }

            var canvasRoot = transform.Find("Canvas");
            if (gameRoot == null && canvasRoot != null)
            {
                gameRoot = canvasRoot.Find("GameRoot") as RectTransform;
            }

            if (targetSequenceRoot == null && gameRoot != null)
            {
                targetSequenceRoot = gameRoot.Find("TargetSequenceRoot") as RectTransform;
            }

            if (gridRoot == null && gameRoot != null)
            {
                gridRoot = gameRoot.Find("GridRoot") as RectTransform;
            }

            if (cursorFrame == null && gridRoot != null)
            {
                cursorFrame = gridRoot.Find("CursorFrame") as RectTransform;
            }

            if (onScreenButtonsRoot == null && gameRoot != null)
            {
                onScreenButtonsRoot = gameRoot.Find("OnScreenButtons")?.gameObject;
            }

            if (titleText == null && gameRoot != null)
            {
                titleText = gameRoot.Find("HeaderText")?.GetComponent<TMP_Text>();
            }

            if (hintText == null && gameRoot != null)
            {
                hintText = gameRoot.Find("HintText")?.GetComponent<TMP_Text>();
            }

            if (statusText == null && gameRoot != null)
            {
                statusText = gameRoot.Find("StatusText")?.GetComponent<TMP_Text>();
            }

            if (errorCountText == null && gameRoot != null)
            {
                errorCountText = gameRoot.Find("ErrorCountText")?.GetComponent<TMP_Text>();
            }

            if (countdownText == null && gameRoot != null)
            {
                countdownText = gameRoot.Find("CountdownText")?.GetComponent<TMP_Text>();
            }

            if (guideToggleButton == null && gameRoot != null)
            {
                guideToggleButton = gameRoot.Find("GuideToggleButton")?.GetComponent<Button>();
            }

            if (guideToggleLabel == null && gameRoot != null)
            {
                guideToggleLabel = gameRoot.Find("GuideToggleButton/Label")?.GetComponent<TMP_Text>();
            }

            if (currentTargetHighlight == null && gameRoot != null)
            {
                currentTargetHighlight = gameRoot.Find("CurrentTargetHighlight")?.GetComponent<TMP_Text>();
            }

            if (onScreenButtonsRoot != null)
            {
                var btnRoot = onScreenButtonsRoot.transform;
                if (upButton == null)
                {
                    upButton = btnRoot.Find("UpButton")?.GetComponent<Button>();
                }

                if (downButton == null)
                {
                    downButton = btnRoot.Find("DownButton")?.GetComponent<Button>();
                }

                if (leftButton == null)
                {
                    leftButton = btnRoot.Find("LeftButton")?.GetComponent<Button>();
                }

                if (rightButton == null)
                {
                    rightButton = btnRoot.Find("RightButton")?.GetComponent<Button>();
                }

                if (confirmButton == null)
                {
                    confirmButton = btnRoot.Find("ConfirmButton")?.GetComponent<Button>();
                }
            }

            CacheGridCells();
            CacheTargetTexts();
            RebuildGuideTags();
        }

        private void CacheGridCells()
        {
            if (gridRoot == null)
            {
                return;
            }

            var found = new List<Button>();
            var gridButtons = gridRoot.GetComponentsInChildren<Button>(true);
            foreach (var button in gridButtons)
            {
                if (button != null && button.name.StartsWith("Cell_", StringComparison.Ordinal))
                {
                    found.Add(button);
                }
            }

            found.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
            gridCellButtons.Clear();
            gridCellButtons.AddRange(found);

            cellLabelLookup.Clear();
            foreach (var button in gridCellButtons)
            {
                if (button == null)
                {
                    continue;
                }

                var label = button.transform.Find("Label")?.GetComponent<TMP_Text>();
                if (label != null)
                {
                    cellLabelLookup[button] = label;
                }
            }
        }

        private void CacheTargetTexts()
        {
            targetRuneTexts.Clear();
            if (targetSequenceRoot == null)
            {
                return;
            }

            var texts = targetSequenceRoot.GetComponentsInChildren<TMP_Text>(true);
            foreach (var text in texts)
            {
                if (text != null &&
                    text.transform.parent == targetSequenceRoot &&
                    text.name.StartsWith("Target_", StringComparison.Ordinal))
                {
                    targetRuneTexts.Add(text);
                }
            }

            targetRuneTexts.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
        }

        private void RebuildGuideTags()
        {
            guideTags.Clear();
            if (gameRoot == null)
            {
                return;
            }

            var found = gameRoot.GetComponentsInChildren<RuneTypingGuideTag>(true);
            foreach (var tag in found)
            {
                if (tag != null)
                {
                    guideTags.Add(tag);
                }
            }
        }

        private void RegisterRuntimeListeners()
        {
            UnregisterRuntimeListeners();

            RegisterButton(guideToggleButton, OnGuideToggleClicked);
            RegisterButton(upButton, () => HandleMove(0, -1, true));
            RegisterButton(downButton, () => HandleMove(0, 1, true));
            RegisterButton(leftButton, () => HandleMove(-1, 0, true));
            RegisterButton(rightButton, () => HandleMove(1, 0, true));
            RegisterButton(confirmButton, () => HandleConfirm(true));

            cellClickActions.Clear();
            for (var i = 0; i < gridCellButtons.Count; i++)
            {
                var button = gridCellButtons[i];
                var index = i;
                if (button == null)
                {
                    continue;
                }

                UnityAction action = () => OnGridCellClicked(index);
                cellClickActions[button] = action;
                button.onClick.AddListener(action);
            }
        }

        private void UnregisterRuntimeListeners()
        {
            UnregisterButton(guideToggleButton);
            UnregisterButton(upButton);
            UnregisterButton(downButton);
            UnregisterButton(leftButton);
            UnregisterButton(rightButton);
            UnregisterButton(confirmButton);

            foreach (var pair in cellClickActions)
            {
                if (pair.Key != null)
                {
                    pair.Key.onClick.RemoveListener(pair.Value);
                }
            }

            cellClickActions.Clear();
            buttonActionLookup.Clear();
        }

        private void RegisterButton(Button button, UnityAction action)
        {
            if (button == null || action == null)
            {
                return;
            }

            buttonActionLookup[button] = action;
            button.onClick.AddListener(action);
        }

        private void UnregisterButton(Button button)
        {
            if (button == null)
            {
                return;
            }

            if (buttonActionLookup.TryGetValue(button, out var action))
            {
                button.onClick.RemoveListener(action);
            }
        }

        private void ResetRound()
        {
            StopAllPanelCoroutines();

            state = RuneTypingState.Countdown;
            usedOnScreenButtons = false;
            targetSequenceIndex = 0;
            cursorCellIndex = 0;
            errorCount = 0;
            nextInputAllowedAt = 0f;
            nextConfirmAllowedAt = 0f;

            var safeGridSize = Mathf.Clamp(panelData.RoundConfig?.GridSize ?? panelData.GridSize, 4, 5);
            gridSize = safeGridSize;
            panelData.GridSize = safeGridSize;
            totalGridCells = gridSize * gridSize;

            runtimeTargetSequence.Clear();
            if (panelData.RoundConfig?.TargetSequence != null)
            {
                runtimeTargetSequence.AddRange(panelData.RoundConfig.TargetSequence);
            }

            if (runtimeTargetSequence.Count == 0)
            {
                runtimeTargetSequence.AddRange(BuildFallbackSequence(gridSize));
            }

            runtimeGridLayout.Clear();
            if (panelData.RoundConfig?.RuneGridLayout != null)
            {
                runtimeGridLayout.AddRange(panelData.RoundConfig.RuneGridLayout);
            }

            if (runtimeGridLayout.Count < totalGridCells)
            {
                runtimeGridLayout.Clear();
                runtimeGridLayout.AddRange(BuildFallbackLayout(gridSize, totalGridCells));
            }

            SetInteractable(true);
            RefreshHeaderTexts();
            ApplyGridLayout();
            ConfigureTargetSequenceView();
            ConfigureGridCells();
            PositionCursorImmediately();
            RefreshGridVisuals();
            RefreshStatusText();
            RefreshErrorText();
            RefreshCurrentTargetHighlight();

            var showOnScreenButtons = RuntimeConfig.ShowOnScreenButtonsInWebGL ||
                                      Application.platform == RuntimePlatform.WebGLPlayer;
            if (onScreenButtonsRoot != null)
            {
                onScreenButtonsRoot.SetActive(showOnScreenButtons);
            }

            SetGuideVisible(showGuideOnOpen);

            countdownRoutine = StartCoroutine(CountdownRoutine());
        }

        private void RefreshHeaderTexts()
        {
            if (titleText != null)
            {
                titleText.text = $"符文控制板 QTE - 客户 {Mathf.Max(1, panelData.ClientId)}";
            }

            if (hintText != null)
            {
                hintText.text = "WASD/方向键移动光标，空格或回车确认当前符文。";
            }
        }

        private void ConfigureTargetSequenceView()
        {
            // Also add a HorizontalLayoutGroup to target sequence root for consistent spacing
            if (targetSequenceRoot != null)
            {
                var hlg = targetSequenceRoot.GetComponent<HorizontalLayoutGroup>();
                if (hlg == null)
                {
                    hlg = targetSequenceRoot.gameObject.AddComponent<HorizontalLayoutGroup>();
                }
                hlg.childAlignment = TextAnchor.MiddleCenter;
                hlg.spacing = 16f;
                hlg.childControlWidth = true;
                hlg.childControlHeight = true;
                hlg.childForceExpandWidth = true;
                hlg.childForceExpandHeight = true;
                hlg.padding = new RectOffset(24, 24, 8, 8);
            }

            for (var i = 0; i < targetRuneTexts.Count; i++)
            {
                var text = targetRuneTexts[i];
                if (text == null)
                {
                    continue;
                }

                if (i < runtimeTargetSequence.Count)
                {
                    text.gameObject.SetActive(true);
                    text.text = DirectionToGlyph(runtimeTargetSequence[i]);
                    text.color = targetPendingColor;
                    text.fontSize = 44;
                    text.fontStyle = FontStyles.Bold;
                    text.alignment = TextAlignmentOptions.Center;
                }
                else
                {
                    text.gameObject.SetActive(false);
                }
            }
        }

        private void ConfigureGridCells()
        {
            for (var i = 0; i < gridCellButtons.Count; i++)
            {
                var button = gridCellButtons[i];
                if (button == null)
                {
                    continue;
                }

                var active = i < totalGridCells;
                button.gameObject.SetActive(active);
                if (!active)
                {
                    continue;
                }

                if (cellLabelLookup.TryGetValue(button, out var label))
                {
                    label.text = DirectionToGlyph(runtimeGridLayout[i]);
                }

                var image = button.image;
                if (image != null)
                {
                    image.color = cellDefaultColor;
                }
            }
        }

        private IEnumerator CountdownRoutine()
        {
            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(true);
            }

            yield return SetCountdownNumber("3", 0.22f);
            yield return SetCountdownNumber("2", 0.22f);
            yield return SetCountdownNumber("1", 0.22f);
            yield return SetCountdownNumber("开始", 0.14f);

            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(false);
            }

            state = RuneTypingState.Playing;
            if (statusText != null)
            {
                statusText.text = "开始输入符文序列。";
            }
            countdownRoutine = null;
        }

        private IEnumerator SetCountdownNumber(string numberText, float duration)
        {
            if (countdownText != null)
            {
                countdownText.text = numberText;
            }

            var elapsed = 0f;
            var wait = Mathf.Max(0.06f, duration);
            while (elapsed < wait)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        private void OnGridCellClicked(int index)
        {
            if (state != RuneTypingState.Playing || index < 0 || index >= totalGridCells)
            {
                return;
            }

            cursorCellIndex = index;
            AnimateCursorToSelection();
            RefreshGridVisuals();
            nextInputAllowedAt = Time.unscaledTime + RuntimeConfig.InputBufferMs * 0.001f;
            nextConfirmAllowedAt = Time.unscaledTime + RuntimeConfig.MinConfirmIntervalMs * 0.001f;
            if (statusText != null)
            {
                statusText.text = "已定位符文，可直接确认。";
            }
        }

        private void HandleMove(int dx, int dy, bool fromOnScreenButtons)
        {
            if (state != RuneTypingState.Playing)
            {
                return;
            }

            usedOnScreenButtons |= fromOnScreenButtons;

            var row = cursorCellIndex / gridSize;
            var col = cursorCellIndex % gridSize;
            var nextRow = row + dy;
            var nextCol = col + dx;

            if (nextRow < 0 || nextRow >= gridSize || nextCol < 0 || nextCol >= gridSize)
            {
                OnBoundaryInputBlocked();
                return;
            }

            cursorCellIndex = nextRow * gridSize + nextCol;
            nextInputAllowedAt = Time.unscaledTime + RuntimeConfig.InputBufferMs * 0.001f;
            EmitCue("sfx.contract.rune.move", transform, 0.8f);
            AnimateCursorToSelection();
            RefreshGridVisuals();
        }

        private void OnBoundaryInputBlocked()
        {
            if (statusText != null)
            {
                statusText.text = "边界外输入已忽略。";
            }
            EmitCue("vfx.contract.rune.error_blink", transform, 0.45f);
            nextInputAllowedAt = Time.unscaledTime + RuntimeConfig.InputBufferMs * 0.001f;
        }

        private void HandleConfirm(bool fromOnScreenButtons)
        {
            if (state != RuneTypingState.Playing)
            {
                return;
            }

            if (targetSequenceIndex >= runtimeTargetSequence.Count)
            {
                return;
            }

            usedOnScreenButtons |= fromOnScreenButtons;
            nextInputAllowedAt = Time.unscaledTime + RuntimeConfig.InputBufferMs * 0.001f;
            nextConfirmAllowedAt = Time.unscaledTime + RuntimeConfig.MinConfirmIntervalMs * 0.001f;

            EmitCue("sfx.contract.rune.confirm", transform, 1f);

            var selectedRune = runtimeGridLayout[cursorCellIndex];
            var expectedRune = runtimeTargetSequence[targetSequenceIndex];

            if (selectedRune == expectedRune)
            {
                targetSequenceIndex++;
                EmitCue("sfx.contract.rune.correct", transform, 1f);
                EmitCue("vfx.contract.rune.correct_glow", transform, 1f);
                if (feedbackRoutine != null)
                {
                    StopCoroutine(feedbackRoutine);
                }

                feedbackRoutine = StartCoroutine(CorrectFeedbackRoutine(cursorCellIndex));
                RefreshCurrentTargetHighlight();
                return;
            }

            errorCount++;
            EmitCue("sfx.contract.rune.error", transform, 1f);
            EmitCue("vfx.contract.rune.error_blink", transform, 1f);
            if (feedbackRoutine != null)
            {
                StopCoroutine(feedbackRoutine);
            }

            feedbackRoutine = StartCoroutine(ErrorFeedbackRoutine(cursorCellIndex));
        }

        private IEnumerator CorrectFeedbackRoutine(int cellIndex)
        {
            state = RuneTypingState.FeedbackCorrect;
            RefreshTargetSequenceProgress();
            RefreshErrorText();
            if (statusText != null)
            {
                statusText.text = "命中正确符文。";
            }

            yield return FlashCell(cellIndex, cellCorrectColor, RuntimeConfig.CorrectFlashDuration);

            if (targetSequenceIndex >= runtimeTargetSequence.Count)
            {
                state = RuneTypingState.ComboResolve;
                BeginComplete();
                yield break;
            }

            state = RuneTypingState.Playing;
            RefreshGridVisuals();
            RefreshStatusText();
            feedbackRoutine = null;
        }

        private IEnumerator ErrorFeedbackRoutine(int cellIndex)
        {
            state = RuneTypingState.FeedbackError;
            RefreshErrorText();
            if (statusText != null)
            {
                statusText.text = "符文错误，继续输入。";
            }

            var duration = Mathf.Max(0.05f, RuntimeConfig.ErrorFlashDuration);
            yield return FlashCell(cellIndex, cellErrorColor, duration);
            yield return FlashCell(cellIndex, cellDefaultColor, duration);
            yield return FlashCell(cellIndex, cellErrorColor, duration);
            yield return FlashCell(cellIndex, cellDefaultColor, duration);

            state = RuneTypingState.Playing;
            RefreshGridVisuals();
            RefreshStatusText();
            feedbackRoutine = null;
        }

        private void BeginComplete()
        {
            if (completeRoutine != null)
            {
                StopCoroutine(completeRoutine);
            }

            completeRoutine = StartCoroutine(CompleteRoutine());
        }

        private IEnumerator CompleteRoutine()
        {
            SetInteractable(false);
            RefreshTargetSequenceProgress();
            if (statusText != null)
            {
                statusText.text = "符文序列完成，正在提交结果...";
            }
            EmitCue("sfx.contract.rune.finish", transform, 1f);
            EmitCue("vfx.contract.rune.energy_flow", transform, 1f);

            var elapsed = 0f;
            var duration = Mathf.Max(0.2f, RuntimeConfig.CompleteResolveDuration);
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            state = RuneTypingState.Completed;
            panelData.OnCompleted?.Invoke(new RuneTypingResultPayload
            {
                ClientId = Mathf.Max(1, panelData.ClientId),
                GridSize = Mathf.Clamp(gridSize, 4, 5),
                ErrorCount = Mathf.Max(0, errorCount),
                SequenceLength = runtimeTargetSequence.Count,
                UsedOnScreenButtons = usedOnScreenButtons,
                WasFallback = false
            });
            completeRoutine = null;
        }

        private IEnumerator FlashCell(int cellIndex, Color flashColor, float duration)
        {
            if (!TryGetCellImage(cellIndex, out var image))
            {
                yield break;
            }

            var originalColor = image.color;
            image.color = flashColor;
            var elapsed = 0f;
            var wait = Mathf.Max(0.02f, duration);
            while (elapsed < wait)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            image.color = originalColor;
        }

        private bool TryGetCellImage(int cellIndex, out Image image)
        {
            image = null;
            if (cellIndex < 0 || cellIndex >= gridCellButtons.Count)
            {
                return false;
            }

            var button = gridCellButtons[cellIndex];
            if (button == null || !button.gameObject.activeSelf)
            {
                return false;
            }

            image = button.image;
            return image != null;
        }

        private void RefreshGridVisuals()
        {
            for (var i = 0; i < gridCellButtons.Count; i++)
            {
                var button = gridCellButtons[i];
                if (button == null || !button.gameObject.activeSelf || button.image == null)
                {
                    continue;
                }

                button.image.color = i == cursorCellIndex ? cellCursorColor : cellDefaultColor;
            }
        }

        private void RefreshTargetSequenceProgress()
        {
            for (var i = 0; i < targetRuneTexts.Count; i++)
            {
                var text = targetRuneTexts[i];
                if (text == null || !text.gameObject.activeSelf)
                {
                    continue;
                }

                text.color = i < targetSequenceIndex ? targetDoneColor : targetPendingColor;
            }
        }

        private void RefreshStatusText()
        {
            if (statusText == null)
            {
                return;
            }

            var finished = Mathf.Clamp(targetSequenceIndex, 0, runtimeTargetSequence.Count);
            statusText.text = $"序列进度：{finished}/{runtimeTargetSequence.Count}";
        }

        private void RefreshErrorText()
        {
            if (errorCountText != null)
            {
                errorCountText.text = $"失误次数：{errorCount}";
            }
        }

        private void AnimateCursorToSelection()
        {
            if (cursorFrame == null)
            {
                return;
            }

            if (cursorMoveRoutine != null)
            {
                StopCoroutine(cursorMoveRoutine);
            }

            cursorMoveRoutine = StartCoroutine(CursorMoveRoutine());
        }

        private void PositionCursorImmediately()
        {
            if (cursorFrame == null || !TryGetCellRect(cursorCellIndex, out var cellRect))
            {
                return;
            }

            // Force layout rebuild so GridLayoutGroup has updated child positions
            if (gridRoot != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(gridRoot);
            }

            cursorFrame.anchoredPosition = cellRect.anchoredPosition;
            cursorFrame.sizeDelta = cellRect.sizeDelta + new Vector2(12f, 12f);
        }

        private IEnumerator CursorMoveRoutine()
        {
            if (!TryGetCellRect(cursorCellIndex, out var cellRect))
            {
                cursorMoveRoutine = null;
                yield break;
            }

            var fromPos = cursorFrame.anchoredPosition;
            var toPos = cellRect.anchoredPosition;
            var toSize = cellRect.sizeDelta + new Vector2(12f, 12f);
            var duration = Mathf.Max(0.02f, RuntimeConfig.CursorMoveDuration);
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var eased = 1f - Mathf.Pow(1f - t, 2f);
                cursorFrame.anchoredPosition = Vector2.Lerp(fromPos, toPos, eased);
                cursorFrame.sizeDelta = toSize;
                yield return null;
            }

            cursorFrame.anchoredPosition = toPos;
            cursorFrame.sizeDelta = toSize;
            cursorMoveRoutine = null;
        }

        private bool TryGetCellRect(int cellIndex, out RectTransform rectTransform)
        {
            rectTransform = null;
            if (cellIndex < 0 || cellIndex >= gridCellButtons.Count)
            {
                return false;
            }

            var button = gridCellButtons[cellIndex];
            if (button == null)
            {
                return false;
            }

            rectTransform = button.transform as RectTransform;
            return rectTransform != null;
        }

        private void OnGuideToggleClicked()
        {
            SetGuideVisible(!guideVisible);
        }

        private void SetGuideVisible(bool visible)
        {
            guideVisible = visible;
            foreach (var tag in guideTags)
            {
                if (tag != null)
                {
                    tag.gameObject.SetActive(visible);
                }
            }

            if (guideToggleLabel != null)
            {
                guideToggleLabel.text = visible ? "说明：开" : "说明：关";
            }
        }

        private void SetInteractable(bool interactable)
        {
            SetButtonInteractable(guideToggleButton, interactable);
            SetButtonInteractable(upButton, interactable);
            SetButtonInteractable(downButton, interactable);
            SetButtonInteractable(leftButton, interactable);
            SetButtonInteractable(rightButton, interactable);
            SetButtonInteractable(confirmButton, interactable);

            foreach (var cellButton in gridCellButtons)
            {
                if (cellButton != null)
                {
                    cellButton.interactable = interactable && cellButton.gameObject.activeSelf;
                }
            }
        }

        private static void SetButtonInteractable(Button button, bool interactable)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
        }

        private void StopAllPanelCoroutines()
        {
            if (countdownRoutine != null)
            {
                StopCoroutine(countdownRoutine);
                countdownRoutine = null;
            }

            if (feedbackRoutine != null)
            {
                StopCoroutine(feedbackRoutine);
                feedbackRoutine = null;
            }

            if (cursorMoveRoutine != null)
            {
                StopCoroutine(cursorMoveRoutine);
                cursorMoveRoutine = null;
            }

            if (completeRoutine != null)
            {
                StopCoroutine(completeRoutine);
                completeRoutine = null;
            }
        }

        private void EmitCue(string cueId, Transform anchor, float intensity)
        {
            panelData.OnFxCue?.Invoke(cueId, anchor, intensity);
        }

        private static List<RuneInputDirection> BuildFallbackSequence(int safeGridSize)
        {
            var length = safeGridSize == 4 ? 4 : 5;
            var result = new List<RuneInputDirection>(length)
            {
                RuneInputDirection.Up,
                RuneInputDirection.Right,
                RuneInputDirection.Down,
                RuneInputDirection.Left,
                RuneInputDirection.Right
            };

            if (result.Count > length)
            {
                result.RemoveRange(length, result.Count - length);
            }

            return result;
        }

        private static List<RuneInputDirection> BuildFallbackLayout(int safeGridSize, int totalCells)
        {
            var layout = new List<RuneInputDirection>(totalCells);
            for (var i = 0; i < totalCells; i++)
            {
                var value = Mathf.Abs(i * 5 + i / safeGridSize) % 4;
                layout.Add((RuneInputDirection)value);
            }

            return layout;
        }

        private static string DirectionToGlyph(RuneInputDirection direction)
        {
            return direction switch
            {
                RuneInputDirection.Up => "▲",
                RuneInputDirection.Down => "▼",
                RuneInputDirection.Left => "◀",
                RuneInputDirection.Right => "▶",
                _ => "?"
            };
        }

        /// <summary>
        /// Applies a GridLayoutGroup to gridRoot so cells are uniformly arranged and
        /// dynamically adapt to 4x4 / 5x5 without manual absolute positioning.
        /// </summary>
        private void ApplyGridLayout()
        {
            if (gridRoot == null)
            {
                return;
            }

            var glg = gridRoot.GetComponent<GridLayoutGroup>();
            if (glg == null)
            {
                glg = gridRoot.gameObject.AddComponent<GridLayoutGroup>();
            }

            // Determine cell size from available space
            var gridRect = gridRoot.rect;
            var availableWidth = gridRect.width > 0f ? gridRect.width : 760f;
            var availableHeight = gridRect.height > 0f ? gridRect.height : 760f;
            var spacing = 12f;
            var totalSpacingW = spacing * (gridSize - 1);
            var totalSpacingH = spacing * (gridSize - 1);
            var cellW = (availableWidth - totalSpacingW - 32f) / gridSize;  // 32 padding
            var cellH = (availableHeight - totalSpacingH - 32f) / gridSize;
            var cellSize = Mathf.Min(cellW, cellH);
            cellSize = Mathf.Max(cellSize, 60f); // minimum cell size

            glg.cellSize = new Vector2(cellSize, cellSize);
            glg.spacing = new Vector2(spacing, spacing);
            glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            glg.constraintCount = gridSize;
            glg.childAlignment = TextAnchor.MiddleCenter;
            glg.startCorner = GridLayoutGroup.Corner.UpperLeft;
            glg.startAxis = GridLayoutGroup.Axis.Horizontal;
            glg.padding = new RectOffset(16, 16, 16, 16);

            // Ensure CursorFrame is excluded from GridLayoutGroup
            if (cursorFrame != null)
            {
                var cursorLayout = cursorFrame.GetComponent<LayoutElement>();
                if (cursorLayout == null)
                {
                    cursorLayout = cursorFrame.gameObject.AddComponent<LayoutElement>();
                }
                cursorLayout.ignoreLayout = true;
            }
        }

        /// <summary>
        /// Shows a prominent highlight for the current target rune the player needs to find.
        /// This makes it impossible to "miss" what to look for.
        /// </summary>
        private void RefreshCurrentTargetHighlight()
        {
            if (currentTargetHighlight == null)
            {
                return;
            }

            if (targetSequenceIndex >= runtimeTargetSequence.Count)
            {
                currentTargetHighlight.text = "✔ 序列完成";
                currentTargetHighlight.color = targetDoneColor;
                return;
            }

            var glyph = DirectionToGlyph(runtimeTargetSequence[targetSequenceIndex]);
            currentTargetHighlight.text = $"当前目标: {glyph}  ({targetSequenceIndex + 1}/{runtimeTargetSequence.Count})";
            currentTargetHighlight.color = new Color(1f, 0.92f, 0.2f, 1f);
        }

        public IArchitecture GetArchitecture()
        {
            return MainMenuApp.Interface;
        }
    }
}
