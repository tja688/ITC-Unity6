using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ITC.SignMiniGame
{
    public sealed class RuneVerifyPanel : UIPanel, IController
    {
        private enum VerifyState
        {
            SpawnGrid,
            Countdown,
            Hunting,
            ResultSuccess,
            ResultFail,
            Completed
        }

        [Header("Roots")]
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private RectTransform verifyRoot;
        [SerializeField] private RectTransform gridRoot;
        [SerializeField] private GridLayoutGroup gridLayout;

        [Header("Top HUD")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private TMP_Text countdownText;
        [SerializeField] private Image timerFillImage;

        [Header("Bottom HUD")]
        [SerializeField] private TMP_Text foundText;
        [SerializeField] private TMP_Text statusText;

        [Header("Guide")]
        [SerializeField] private Button guideToggleButton;
        [SerializeField] private TMP_Text guideToggleLabel;
        [SerializeField] private bool showGuideOnOpen = true;

        [Header("Tuning")]
        [SerializeField] private float foundFlashDuration = 0.2f;
        [SerializeField] private float wrongFlashDuration = 0.12f;
        [SerializeField] private float resultStaySeconds = 0.4f;

        private readonly Dictionary<Button, UnityAction> cellClickListeners = new();
        private readonly Dictionary<Button, RuneCellView> cellLookup = new();
        private readonly List<RuneCellView> cellPool = new();
        private readonly List<RuneVerifyGuideTag> guideTags = new();
        private readonly HashSet<int> distortedIndices = new();
        private readonly HashSet<int> foundIndices = new();
        private readonly string[] runeGlyphs =
        {
            "A", "B", "C", "D", "E",
            "F", "G", "H", "I", "J",
            "K", "L", "M", "N", "P",
            "R", "S", "T", "V", "W"
        };

        private RuneVerifyPanelData panelData = new();
        private RuneVerifyConfig runtimeConfig = new();
        private VerifyState state;
        private Coroutine roundCoroutine;
        private Coroutine countdownPulseCoroutine;
        private bool completionSent;
        private bool resolveRequested;
        private bool resolveSuccess;
        private bool guideVisible;
        private int activeSeed;
        private float timeRemaining;
        private int targetDistortedCount;
        private int activeCellCount;

        private sealed class RuneCellView
        {
            public int Index;
            public Button Button;
            public Image FrameImage;
            public Image GlyphImage;
            public TMP_Text Label;
            public RectTransform Rect;
            public bool IsDistorted;
            public bool IsFound;
        }

        private void Awake()
        {
            CacheReferences();
        }

        protected override void OnInit(IUIData uiData = null)
        {
            panelData = uiData as RuneVerifyPanelData ?? new RuneVerifyPanelData();
            CacheReferences();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            panelData = uiData as RuneVerifyPanelData ?? panelData ?? new RuneVerifyPanelData();
            EnsurePanelConfig();
            CacheReferences();
            RegisterRuntimeListeners();
            ResetRound();
        }

        protected override void OnClose()
        {
            UnregisterRuntimeListeners();

            if (roundCoroutine != null)
            {
                StopCoroutine(roundCoroutine);
                roundCoroutine = null;
            }

            if (countdownPulseCoroutine != null)
            {
                StopCoroutine(countdownPulseCoroutine);
                countdownPulseCoroutine = null;
            }

            SetCellsInteractable(false);
            foundIndices.Clear();
            distortedIndices.Clear();
        }

        private void EnsurePanelConfig()
        {
            var defaultConfig = this.GetModel<SignMiniGameClientConfigModel>().RuneVerifyRuleConfig;
            runtimeConfig = (panelData.RuntimeConfig ?? defaultConfig ?? new RuneVerifyConfig()).Clone();

            runtimeConfig.GridWidth = Mathf.Clamp(runtimeConfig.GridWidth, 2, 8);
            runtimeConfig.GridHeight = Mathf.Clamp(runtimeConfig.GridHeight, 2, 8);
            runtimeConfig.DistortedCount = Mathf.Clamp(runtimeConfig.DistortedCount, 1, runtimeConfig.GridWidth * runtimeConfig.GridHeight);
            runtimeConfig.TimeLimitSeconds = Mathf.Clamp(runtimeConfig.TimeLimitSeconds, 1f, 15f);
            runtimeConfig.FinalSecondPulseRate = Mathf.Clamp(runtimeConfig.FinalSecondPulseRate, 0.05f, 1f);
            runtimeConfig.CountdownLeadSeconds = Mathf.Clamp(runtimeConfig.CountdownLeadSeconds, 0f, 2f);
        }

        private void CacheReferences()
        {
            if (panelRoot == null)
            {
                panelRoot = transform as RectTransform;
            }

            var canvasRoot = transform.Find("Canvas");
            if (verifyRoot == null)
            {
                verifyRoot = canvasRoot != null ? canvasRoot.Find("VerifyRoot") as RectTransform : null;
            }

            if (gridRoot == null && verifyRoot != null)
            {
                gridRoot = verifyRoot.Find("GridRoot") as RectTransform;
            }

            if (gridLayout == null && gridRoot != null)
            {
                gridLayout = gridRoot.GetComponent<GridLayoutGroup>();
            }

            if (titleText == null && verifyRoot != null)
            {
                titleText = verifyRoot.Find("Top/TitleText")?.GetComponent<TMP_Text>();
            }

            if (hintText == null && verifyRoot != null)
            {
                hintText = verifyRoot.Find("Top/HintText")?.GetComponent<TMP_Text>();
            }

            if (countdownText == null && verifyRoot != null)
            {
                countdownText = verifyRoot.Find("Top/CountdownText")?.GetComponent<TMP_Text>();
            }

            if (timerFillImage == null && verifyRoot != null)
            {
                timerFillImage = verifyRoot.Find("Top/TimerBar/Fill")?.GetComponent<Image>();
            }

            if (foundText == null && verifyRoot != null)
            {
                foundText = verifyRoot.Find("Bottom/FoundText")?.GetComponent<TMP_Text>();
            }

            if (statusText == null && verifyRoot != null)
            {
                statusText = verifyRoot.Find("Bottom/StatusText")?.GetComponent<TMP_Text>();
            }

            if (guideToggleButton == null && verifyRoot != null)
            {
                guideToggleButton = verifyRoot.Find("GuideToggleButton")?.GetComponent<Button>();
            }

            if (guideToggleLabel == null && verifyRoot != null)
            {
                guideToggleLabel = verifyRoot.Find("GuideToggleButton/Label")?.GetComponent<TMP_Text>();
            }

            RebuildCellPoolFromHierarchy();
            RebuildGuideTags();
        }

        private void RebuildCellPoolFromHierarchy()
        {
            cellPool.Clear();
            cellLookup.Clear();
            if (gridRoot == null)
            {
                return;
            }

            for (var i = 0; i < gridRoot.childCount; i++)
            {
                var cellTransform = gridRoot.GetChild(i);
                var button = cellTransform.GetComponent<Button>();
                if (button == null)
                {
                    continue;
                }

                var view = BuildCellView(button, i);
                cellPool.Add(view);
                cellLookup[button] = view;
            }
        }

        private RuneCellView BuildCellView(Button button, int index)
        {
            var frameImage = button.GetComponent<Image>();
            var glyph = button.transform.Find("Glyph")?.GetComponent<Image>();
            var label = button.transform.Find("Label")?.GetComponent<TMP_Text>();
            return new RuneCellView
            {
                Index = index,
                Button = button,
                FrameImage = frameImage,
                GlyphImage = glyph,
                Label = label,
                Rect = button.transform as RectTransform
            };
        }

        private void RebuildGuideTags()
        {
            guideTags.Clear();
            if (verifyRoot == null)
            {
                return;
            }

            var found = verifyRoot.GetComponentsInChildren<RuneVerifyGuideTag>(true);
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

            if (guideToggleButton != null)
            {
                guideToggleButton.onClick.AddListener(OnGuideToggleClicked);
            }

            cellClickListeners.Clear();
            foreach (var view in cellPool)
            {
                if (view?.Button == null)
                {
                    continue;
                }

                var localView = view;
                UnityAction action = () => OnRuneCellClicked(localView);
                cellClickListeners[view.Button] = action;
                view.Button.onClick.AddListener(action);
            }
        }

        private void UnregisterRuntimeListeners()
        {
            if (guideToggleButton != null)
            {
                guideToggleButton.onClick.RemoveListener(OnGuideToggleClicked);
            }

            foreach (var pair in cellClickListeners)
            {
                if (pair.Key != null)
                {
                    pair.Key.onClick.RemoveListener(pair.Value);
                }
            }

            cellClickListeners.Clear();
        }

        private void ResetRound()
        {
            state = VerifyState.SpawnGrid;
            completionSent = false;
            resolveRequested = false;
            resolveSuccess = false;
            activeSeed = panelData.RuntimeSeed >= 0 ? panelData.RuntimeSeed : Environment.TickCount;
            foundIndices.Clear();
            distortedIndices.Clear();

            ConfigureHeaderText();
            BuildGridContent();
            SetGuideVisible(showGuideOnOpen);
            SetCellsInteractable(false);
            UpdateFoundText();
            UpdateStatusText("准备开始核验...");

            if (roundCoroutine != null)
            {
                StopCoroutine(roundCoroutine);
            }

            roundCoroutine = StartCoroutine(RoundRoutine());
        }

        private void ConfigureHeaderText()
        {
            if (titleText != null)
            {
                titleText.text = $"符文核验 - 客户 {Mathf.Max(1, panelData.ClientId)}";
            }

            if (hintText != null)
            {
                hintText.text = "在限时内找出全部扭曲符文（误点只触发视觉反馈）。";
            }
        }

        private void BuildGridContent()
        {
            var width = runtimeConfig.GridWidth;
            var height = runtimeConfig.GridHeight;
            var total = Mathf.Max(1, width * height);
            activeCellCount = total;
            targetDistortedCount = Mathf.Clamp(runtimeConfig.DistortedCount, 1, total);

            EnsureCellPool(total);
            ConfigureGridLayout(width);

            var random = new System.Random(activeSeed);
            distortedIndices.Clear();
            while (distortedIndices.Count < targetDistortedCount)
            {
                distortedIndices.Add(random.Next(0, total));
            }

            for (var i = 0; i < cellPool.Count; i++)
            {
                var view = cellPool[i];
                if (view?.Button == null)
                {
                    continue;
                }

                var active = i < total;
                view.Button.gameObject.SetActive(active);
                view.Index = i;
                view.IsFound = false;
                view.IsDistorted = active && distortedIndices.Contains(i);

                if (!active)
                {
                    continue;
                }

                var glyph = runeGlyphs[random.Next(0, runeGlyphs.Length)];
                if (view.Label != null)
                {
                    view.Label.text = glyph;
                }

                ApplyCellVisual(view, found: false);
            }
        }

        private void ConfigureGridLayout(int width)
        {
            if (gridLayout == null)
            {
                return;
            }

            gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            gridLayout.constraintCount = Mathf.Max(1, width);
        }

        private void EnsureCellPool(int requiredCount)
        {
            if (gridRoot == null)
            {
                return;
            }

            while (cellPool.Count < requiredCount)
            {
                var index = cellPool.Count;
                var button = CreateCell(index);
                var view = BuildCellView(button, index);
                cellPool.Add(view);
                cellLookup[button] = view;

                var localView = view;
                UnityAction action = () => OnRuneCellClicked(localView);
                cellClickListeners[button] = action;
                button.onClick.AddListener(action);
            }
        }

        private Button CreateCell(int index)
        {
            var go = new GameObject($"RuneCell_{index:00}", typeof(RectTransform), typeof(Image), typeof(Button));
            go.transform.SetParent(gridRoot, false);

            var rect = go.transform as RectTransform;
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(132f, 132f);
                rect.localScale = Vector3.one;
            }

            var frame = go.GetComponent<Image>();
            frame.color = new Color32(42, 54, 66, 255);

            var button = go.GetComponent<Button>();
            button.targetGraphic = frame;

            var glyphGo = new GameObject("Glyph", typeof(RectTransform), typeof(Image));
            glyphGo.transform.SetParent(go.transform, false);
            var glyphRect = glyphGo.transform as RectTransform;
            if (glyphRect != null)
            {
                glyphRect.anchorMin = new Vector2(0.5f, 0.5f);
                glyphRect.anchorMax = new Vector2(0.5f, 0.5f);
                glyphRect.pivot = new Vector2(0.5f, 0.5f);
                glyphRect.anchoredPosition = Vector2.zero;
                glyphRect.sizeDelta = new Vector2(120f, 120f);
            }

            var glyphImage = glyphGo.GetComponent<Image>();
            glyphImage.color = new Color32(68, 88, 106, 255);
            glyphImage.raycastTarget = false;

            var labelGo = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.transform as RectTransform;
            if (labelRect != null)
            {
                labelRect.anchorMin = Vector2.zero;
                labelRect.anchorMax = Vector2.one;
                labelRect.pivot = new Vector2(0.5f, 0.5f);
                labelRect.offsetMin = Vector2.zero;
                labelRect.offsetMax = Vector2.zero;
            }

            var label = labelGo.GetComponent<TextMeshProUGUI>();
            label.text = "A";
            label.fontSize = 42f;
            label.alignment = TextAlignmentOptions.Center;
            label.color = new Color32(197, 230, 255, 255);
            label.raycastTarget = false;

            return button;
        }

        private IEnumerator RoundRoutine()
        {
            var leadTime = runtimeConfig.CountdownLeadSeconds;
            timeRemaining = runtimeConfig.TimeLimitSeconds;
            state = VerifyState.Countdown;

            while (leadTime > 0f)
            {
                leadTime -= Time.unscaledDeltaTime;
                var remain = Mathf.Max(0f, leadTime);
                if (countdownText != null)
                {
                    countdownText.SetText("准备 {0:0.0}s", remain);
                }

                if (timerFillImage != null)
                {
                    timerFillImage.fillAmount = 1f;
                }

                yield return null;
            }

            state = VerifyState.Hunting;
            SetCellsInteractable(true);
            EmitCue("vfx.contract.verify.distort_ping", gridRoot != null ? gridRoot : transform, 0.9f);

            var pulseRate = Mathf.Max(0.05f, runtimeConfig.FinalSecondPulseRate);
            var pulseAccumulator = 0f;

            while (!resolveRequested && timeRemaining > 0f)
            {
                timeRemaining = Mathf.Max(0f, timeRemaining - Time.unscaledDeltaTime);
                UpdateTimerHud();

                if (timeRemaining <= 1f)
                {
                    pulseAccumulator += Time.unscaledDeltaTime;
                    if (pulseAccumulator >= pulseRate)
                    {
                        pulseAccumulator = 0f;
                        EmitCue("sfx.contract.verify.tick", transform, 1f);
                        TriggerCountdownPulse();
                    }
                }

                yield return null;
            }

            SetCellsInteractable(false);
            if (!resolveRequested)
            {
                resolveRequested = true;
                resolveSuccess = false;
            }

            if (resolveSuccess)
            {
                state = VerifyState.ResultSuccess;
                UpdateStatusText("核验成功：已找齐全部扭曲符文。");
                EmitCue("sfx.contract.verify.correct", transform, 1f);
                EmitCue("vfx.contract.verify.success_burst", transform, 1f);
            }
            else
            {
                state = VerifyState.ResultFail;
                UpdateStatusText("核验失败：时间耗尽或未找齐目标。");
                EmitCue("sfx.contract.verify.timeout", transform, 1f);
                EmitCue("vfx.contract.verify.fail_noise", transform, 1f);
            }

            yield return new WaitForSecondsRealtime(Mathf.Max(0.05f, resultStaySeconds));
            CompleteRound(resolveSuccess);
        }

        private void TriggerCountdownPulse()
        {
            if (countdownPulseCoroutine != null)
            {
                StopCoroutine(countdownPulseCoroutine);
            }

            countdownPulseCoroutine = StartCoroutine(PulseCountdownText());
        }

        private IEnumerator PulseCountdownText()
        {
            if (countdownText == null)
            {
                yield break;
            }

            var targetTransform = countdownText.rectTransform;
            var baseScale = targetTransform.localScale;
            var boostedScale = baseScale * 1.08f;
            var halfDuration = 0.06f;

            var elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / halfDuration);
                targetTransform.localScale = Vector3.Lerp(baseScale, boostedScale, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / halfDuration);
                targetTransform.localScale = Vector3.Lerp(boostedScale, baseScale, t);
                yield return null;
            }

            targetTransform.localScale = baseScale;
            countdownPulseCoroutine = null;
        }

        private void UpdateTimerHud()
        {
            if (countdownText != null)
            {
                countdownText.SetText("{0:0.0}s", timeRemaining);
            }

            if (timerFillImage != null)
            {
                var total = Mathf.Max(0.001f, runtimeConfig.TimeLimitSeconds);
                timerFillImage.fillAmount = Mathf.Clamp01(timeRemaining / total);
            }
        }

        private void OnRuneCellClicked(RuneCellView view)
        {
            if (view == null || state != VerifyState.Hunting || resolveRequested)
            {
                return;
            }

            if (view.IsDistorted && !view.IsFound)
            {
                view.IsFound = true;
                foundIndices.Add(view.Index);
                ApplyCellVisual(view, found: true);
                UpdateFoundText();
                UpdateStatusText($"命中 {foundIndices.Count}/{targetDistortedCount}");
                EmitCue("sfx.contract.verify.correct", view.Button.transform, 1f);
                StartCoroutine(FlashCell(view, true));

                if (foundIndices.Count >= targetDistortedCount)
                {
                    resolveRequested = true;
                    resolveSuccess = true;
                }

                return;
            }

            UpdateStatusText(runtimeConfig.MisclickPenaltyVisualOnly
                ? "误点：该符文并未扭曲。"
                : "误点：请重新核验。");

            EmitCue("sfx.contract.verify.wrong", view.Button.transform, 1f);
            StartCoroutine(FlashCell(view, false));
        }

        private IEnumerator FlashCell(RuneCellView view, bool correct)
        {
            if (view == null || view.Rect == null)
            {
                yield break;
            }

            var duration = correct
                ? Mathf.Max(0.05f, foundFlashDuration)
                : Mathf.Max(0.05f, wrongFlashDuration);
            var baseScale = view.Rect.localScale;
            var boostScale = correct ? baseScale * 1.07f : baseScale * 0.94f;

            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                view.Rect.localScale = Vector3.Lerp(baseScale, boostScale, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                view.Rect.localScale = Vector3.Lerp(boostScale, baseScale, t);
                yield return null;
            }

            view.Rect.localScale = baseScale;
        }

        private void ApplyCellVisual(RuneCellView view, bool found)
        {
            if (view == null)
            {
                return;
            }

            if (view.FrameImage != null)
            {
                if (found)
                {
                    view.FrameImage.color = new Color32(58, 123, 77, 255);
                }
                else if (view.IsDistorted)
                {
                    view.FrameImage.color = new Color32(122, 34, 46, 255);
                }
                else
                {
                    view.FrameImage.color = new Color32(42, 54, 66, 255);
                }
            }

            if (view.GlyphImage != null)
            {
                if (found)
                {
                    view.GlyphImage.color = new Color32(93, 168, 110, 255);
                }
                else if (view.IsDistorted)
                {
                    view.GlyphImage.color = new Color32(180, 58, 76, 255);
                }
                else
                {
                    view.GlyphImage.color = new Color32(68, 88, 106, 255);
                }
            }

            if (view.Label != null)
            {
                if (found)
                {
                    view.Label.color = new Color32(229, 255, 219, 255);
                    view.Label.fontStyle = FontStyles.Bold;
                    view.Label.rectTransform.localRotation = Quaternion.identity;
                }
                else if (view.IsDistorted)
                {
                    view.Label.color = new Color32(255, 182, 174, 255);
                    view.Label.fontStyle = FontStyles.Bold | FontStyles.Italic;
                    view.Label.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 8f);
                }
                else
                {
                    view.Label.color = new Color32(197, 230, 255, 255);
                    view.Label.fontStyle = FontStyles.Bold;
                    view.Label.rectTransform.localRotation = Quaternion.identity;
                }
            }
        }

        private void UpdateFoundText()
        {
            if (foundText != null)
            {
                foundText.SetText("Found {0}/{1}", foundIndices.Count, targetDistortedCount);
            }
        }

        private void UpdateStatusText(string text)
        {
            if (statusText != null)
            {
                statusText.text = text ?? string.Empty;
            }
        }

        private void SetCellsInteractable(bool interactable)
        {
            for (var i = 0; i < cellPool.Count; i++)
            {
                var view = cellPool[i];
                if (view?.Button == null || !view.Button.gameObject.activeInHierarchy)
                {
                    continue;
                }

                view.Button.interactable = interactable;
            }
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

        private void CompleteRound(bool success)
        {
            if (completionSent)
            {
                return;
            }

            completionSent = true;
            state = VerifyState.Completed;

            var payload = new RuneVerifyResultPayload
            {
                ClientId = Mathf.Max(1, panelData.ClientId),
                Result = success ? RuneVerifyResultType.Success : RuneVerifyResultType.Failed,
                FoundCount = Mathf.Clamp(foundIndices.Count, 0, targetDistortedCount),
                DistortedCount = Mathf.Max(1, targetDistortedCount),
                Triggered = true,
                WasTimeoutFallback = false,
                RandomSeed = activeSeed
            };

            panelData.OnCompleted?.Invoke(payload);
        }

        private void EmitCue(string cueId, Transform anchor, float intensity)
        {
            panelData.OnFxCue?.Invoke(cueId, anchor, intensity);
        }

        public IArchitecture GetArchitecture()
        {
            return MainMenuApp.Interface;
        }
    }
}

