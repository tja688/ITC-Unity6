using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ITC.Contracting
{
    public sealed class SoulCollectPanel : UIPanel, IController
    {
        private enum SoulCollectState
        {
            SoulReveal,
            Aiming,
            PreviewPercent,
            ConfirmSplit,
            Resolve,
            Completed
        }

        [Header("Roots")]
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private RectTransform soulRoot;
        [SerializeField] private RectTransform soulArea;
        [SerializeField] private RectTransform splitLine;
        [SerializeField] private RectTransform targetBand;

        [Header("Buttons")]
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button guideToggleButton;

        [Header("Text")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private TMP_Text sideCompanyText;
        [SerializeField] private TMP_Text sideClientText;
        [SerializeField] private TMP_Text targetHintText;
        [SerializeField] private TMP_Text percentText;
        [SerializeField] private TMP_Text rangeText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text confirmButtonLabel;
        [SerializeField] private TMP_Text resetButtonLabel;
        [SerializeField] private TMP_Text guideToggleLabel;

        [Header("Visual")]
        [SerializeField] private Image soulCoreImage;
        [SerializeField] private Image splitLineImage;
        [SerializeField] private Image overdrawHintImage;
        [SerializeField] private Image underdrawHintImage;
        [SerializeField] private bool showGuideOnOpen = true;

        [Header("Tuning")]
        [SerializeField] private float revealDurationSeconds = 0.35f;

        private readonly List<SoulCollectGuideTag> guideTags = new();

        private SoulCollectPanelData panelData = new();
        private SoulCollectConfig runtimeConfig = new();
        private SoulCollectState state;
        private bool guideVisible;
        private bool inputLocked;
        private bool completionSent;
        private bool hasUserAimed;
        private float targetNormalized;
        private float currentNormalized;
        private float previewTickTimer;
        private float nextDragCueTime;
        private int previewPercentInt;
        private Coroutine revealCoroutine;
        private Coroutine resolveCoroutine;

        private void Awake()
        {
            CacheReferences();
        }

        protected override void OnInit(IUIData uiData = null)
        {
            panelData = uiData as SoulCollectPanelData ?? new SoulCollectPanelData();
            CacheReferences();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            panelData = uiData as SoulCollectPanelData ?? panelData ?? new SoulCollectPanelData();
            EnsurePanelConfig();
            CacheReferences();
            RegisterRuntimeListeners();
            ResetRound();
        }

        protected override void OnClose()
        {
            UnregisterRuntimeListeners();

            if (revealCoroutine != null)
            {
                StopCoroutine(revealCoroutine);
                revealCoroutine = null;
            }

            if (resolveCoroutine != null)
            {
                StopCoroutine(resolveCoroutine);
                resolveCoroutine = null;
            }
        }

        private void Update()
        {
            if (inputLocked || (state != SoulCollectState.Aiming && state != SoulCollectState.PreviewPercent))
            {
                return;
            }

            ProcessKeyboardInput();
            ProcessPointerInput();
            ApplyAimAssist();
            UpdateAimingSmoothing();
            UpdateSplitLinePosition();

            previewTickTimer += Time.unscaledDeltaTime;
            var tickInterval = 1f / Mathf.Max(5f, runtimeConfig.PreviewRefreshRateHz);
            if (previewTickTimer >= tickInterval)
            {
                previewTickTimer = 0f;
                RefreshPreview(force: false);
            }
        }

        private void EnsurePanelConfig()
        {
            var configModel = this.GetModel<ContractClientConfigModel>();

            if (panelData.ClientConfig == null)
            {
                panelData.ClientConfig = configModel.GetSoulCollectClientConfig(panelData.ClientId);
            }

            var defaultRuntimeConfig = configModel.BuildSoulCollectRuntimeConfig(panelData.ClientId);
            runtimeConfig = (panelData.RuntimeConfig ?? defaultRuntimeConfig ?? new SoulCollectConfig()).Clone();

            runtimeConfig.MinPercent = Mathf.Clamp(runtimeConfig.MinPercent, 0, 100);
            runtimeConfig.MaxPercent = Mathf.Clamp(runtimeConfig.MaxPercent, runtimeConfig.MinPercent, 100);
            runtimeConfig.TargetPercent = Mathf.Clamp(runtimeConfig.TargetPercent, runtimeConfig.MinPercent, runtimeConfig.MaxPercent);
            runtimeConfig.AimAssistRadius = Mathf.Clamp(runtimeConfig.AimAssistRadius, 0f, 20f);
            runtimeConfig.DragDamping = Mathf.Clamp(runtimeConfig.DragDamping, 0.01f, 0.5f);
            runtimeConfig.SplitAnimDuration = Mathf.Clamp(runtimeConfig.SplitAnimDuration, 0.1f, 1.5f);
            runtimeConfig.PreviewRefreshRateHz = Mathf.Clamp(runtimeConfig.PreviewRefreshRateHz, 5f, 60f);
            runtimeConfig.ResultStaySeconds = Mathf.Clamp(runtimeConfig.ResultStaySeconds, 0.1f, 2f);
            runtimeConfig.KeyboardAdjustPerSecond = Mathf.Clamp(runtimeConfig.KeyboardAdjustPerSecond, 0.05f, 1f);

            panelData.RuntimeConfig = runtimeConfig;
        }

        private void CacheReferences()
        {
            if (panelRoot == null)
            {
                panelRoot = transform as RectTransform;
            }

            var canvasRoot = transform.Find("Canvas");
            if (soulRoot == null)
            {
                soulRoot = canvasRoot != null ? canvasRoot.Find("SoulRoot") as RectTransform : null;
            }

            if (titleText == null && soulRoot != null)
            {
                titleText = soulRoot.Find("HeaderText")?.GetComponent<TMP_Text>();
            }

            if (hintText == null && soulRoot != null)
            {
                hintText = soulRoot.Find("HintText")?.GetComponent<TMP_Text>();
            }

            if (guideToggleButton == null && soulRoot != null)
            {
                guideToggleButton = soulRoot.Find("GuideToggleButton")?.GetComponent<Button>();
            }

            if (guideToggleLabel == null && soulRoot != null)
            {
                guideToggleLabel = soulRoot.Find("GuideToggleButton/Label")?.GetComponent<TMP_Text>();
            }

            if (soulArea == null && soulRoot != null)
            {
                soulArea = soulRoot.Find("SoulArea") as RectTransform;
            }

            if (soulCoreImage == null && soulArea != null)
            {
                soulCoreImage = soulArea.Find("SoulCore")?.GetComponent<Image>();
            }

            if (splitLine == null && soulArea != null)
            {
                splitLine = soulArea.Find("SplitLine") as RectTransform;
            }

            if (splitLineImage == null && splitLine != null)
            {
                splitLineImage = splitLine.GetComponent<Image>();
            }

            if (targetBand == null && soulArea != null)
            {
                targetBand = soulArea.Find("TargetBand") as RectTransform;
            }

            if (overdrawHintImage == null && soulArea != null)
            {
                overdrawHintImage = soulArea.Find("OverdrawHint")?.GetComponent<Image>();
            }

            if (underdrawHintImage == null && soulArea != null)
            {
                underdrawHintImage = soulArea.Find("UnderdrawHint")?.GetComponent<Image>();
            }

            if (targetHintText == null && soulArea != null)
            {
                targetHintText = soulArea.Find("TargetHintText")?.GetComponent<TMP_Text>();
            }

            if (sideCompanyText == null && soulArea != null)
            {
                sideCompanyText = soulArea.Find("CompanyLabel")?.GetComponent<TMP_Text>();
            }

            if (sideClientText == null && soulArea != null)
            {
                sideClientText = soulArea.Find("ClientLabel")?.GetComponent<TMP_Text>();
            }

            var bottomRoot = soulRoot != null ? soulRoot.Find("Bottom") : null;
            if (percentText == null && bottomRoot != null)
            {
                percentText = bottomRoot.Find("PercentText")?.GetComponent<TMP_Text>();
            }

            if (rangeText == null && bottomRoot != null)
            {
                rangeText = bottomRoot.Find("RangeText")?.GetComponent<TMP_Text>();
            }

            if (statusText == null && bottomRoot != null)
            {
                statusText = bottomRoot.Find("StatusText")?.GetComponent<TMP_Text>();
            }

            var controlsRoot = bottomRoot != null ? bottomRoot.Find("Controls") : null;
            if (confirmButton == null && controlsRoot != null)
            {
                confirmButton = controlsRoot.Find("ConfirmButton")?.GetComponent<Button>();
            }

            if (confirmButtonLabel == null && controlsRoot != null)
            {
                confirmButtonLabel = controlsRoot.Find("ConfirmButton/Label")?.GetComponent<TMP_Text>();
            }

            if (resetButton == null && controlsRoot != null)
            {
                resetButton = controlsRoot.Find("ResetButton")?.GetComponent<Button>();
            }

            if (resetButtonLabel == null && controlsRoot != null)
            {
                resetButtonLabel = controlsRoot.Find("ResetButton/Label")?.GetComponent<TMP_Text>();
            }

            RebuildGuideTags();
        }

        private void RebuildGuideTags()
        {
            guideTags.Clear();
            if (soulRoot == null)
            {
                return;
            }

            var found = soulRoot.GetComponentsInChildren<SoulCollectGuideTag>(true);
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

            if (confirmButton != null)
            {
                confirmButton.onClick.AddListener(OnConfirmClicked);
            }

            if (resetButton != null)
            {
                resetButton.onClick.AddListener(OnResetClicked);
            }

            if (guideToggleButton != null)
            {
                guideToggleButton.onClick.AddListener(OnGuideToggleClicked);
            }
        }

        private void UnregisterRuntimeListeners()
        {
            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveListener(OnConfirmClicked);
            }

            if (resetButton != null)
            {
                resetButton.onClick.RemoveListener(OnResetClicked);
            }

            if (guideToggleButton != null)
            {
                guideToggleButton.onClick.RemoveListener(OnGuideToggleClicked);
            }
        }

        private void ResetRound()
        {
            state = SoulCollectState.SoulReveal;
            inputLocked = true;
            completionSent = false;
            hasUserAimed = false;
            previewTickTimer = 0f;
            nextDragCueTime = 0f;

            targetNormalized = PercentToNormalized(runtimeConfig.TargetPercent);
            currentNormalized = targetNormalized;
            previewPercentInt = runtimeConfig.TargetPercent;

            ConfigureTexts();
            ApplyTargetBandVisual();
            UpdateSplitLinePosition();
            RefreshPreview(force: true);
            SetGuideVisible(showGuideOnOpen);
            SetStatus("灵魂显现中...");
            RefreshButtons();

            EmitCue("sfx.contract.soul.reveal", soulArea != null ? soulArea : transform, 1f);
            EmitCue(panelData.ClientConfig != null && !string.IsNullOrWhiteSpace(panelData.ClientConfig.RevealFxKey)
                ? panelData.ClientConfig.RevealFxKey
                : "vfx.contract.soul.transfer", soulArea != null ? soulArea : transform, 1f);

            if (revealCoroutine != null)
            {
                StopCoroutine(revealCoroutine);
            }

            revealCoroutine = StartCoroutine(BeginAimingRoutine());
        }

        private IEnumerator BeginAimingRoutine()
        {
            yield return new WaitForSecondsRealtime(Mathf.Max(0.1f, revealDurationSeconds));

            state = SoulCollectState.Aiming;
            inputLocked = false;
            SetStatus("拖动分割线调整收取比例，确认后立即结算。");
            RefreshButtons();
        }

        private void ConfigureTexts()
        {
            var clientName = panelData.ClientConfig != null && !string.IsNullOrWhiteSpace(panelData.ClientConfig.ClientDisplayName)
                ? panelData.ClientConfig.ClientDisplayName
                : $"客户{Mathf.Max(1, panelData.ClientId)}";

            if (titleText != null)
            {
                titleText.text = $"灵魂收取分割 - {clientName}";
            }

            if (hintText != null)
            {
                hintText.text = "先瞄准收取比例，再点击“确认分割”锁定结果。";
            }

            if (sideCompanyText != null)
            {
                sideCompanyText.text = "Company";
            }

            if (sideClientText != null)
            {
                sideClientText.text = "Client";
            }

            if (targetHintText != null)
            {
                targetHintText.text = panelData.TutorialMode && runtimeConfig.ShowTargetHintInTutorial
                    ? $"目标 {runtimeConfig.TargetPercent}%"
                    : "按区间完成收取";
            }

            if (rangeText != null)
            {
                rangeText.SetText("有效区间 {0}% - {1}%", runtimeConfig.MinPercent, runtimeConfig.MaxPercent);
            }
        }

        private void ApplyTargetBandVisual()
        {
            if (targetBand == null)
            {
                return;
            }

            var min = PercentToNormalized(runtimeConfig.MinPercent);
            var max = PercentToNormalized(runtimeConfig.MaxPercent);
            targetBand.anchorMin = new Vector2(min, 0f);
            targetBand.anchorMax = new Vector2(Mathf.Max(min + 0.001f, max), 1f);
            targetBand.offsetMin = Vector2.zero;
            targetBand.offsetMax = Vector2.zero;
        }

        private void ProcessKeyboardInput()
        {
            var direction = 0f;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                direction -= 1f;
            }

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                direction += 1f;
            }

            if (Mathf.Approximately(direction, 0f))
            {
                return;
            }

            targetNormalized = Mathf.Clamp01(targetNormalized + direction * runtimeConfig.KeyboardAdjustPerSecond * Time.unscaledDeltaTime);
            MarkAimingActive();
        }

        private void ProcessPointerInput()
        {
            if (!Input.GetMouseButton(0) || soulArea == null)
            {
                return;
            }

            if (!RectTransformUtility.RectangleContainsScreenPoint(soulArea, Input.mousePosition, null))
            {
                return;
            }

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(soulArea, Input.mousePosition, null, out var localPoint))
            {
                return;
            }

            var width = Mathf.Max(1f, soulArea.rect.width);
            var normalized = Mathf.InverseLerp(-width * 0.5f, width * 0.5f, localPoint.x);
            targetNormalized = Mathf.Clamp01(normalized);
            MarkAimingActive();
            if (Time.unscaledTime >= nextDragCueTime)
            {
                nextDragCueTime = Time.unscaledTime + 0.08f;
                EmitCue("sfx.contract.soul.drag", splitLine != null ? splitLine : transform, 0.5f);
            }
        }

        private void MarkAimingActive()
        {
            if (hasUserAimed)
            {
                return;
            }

            hasUserAimed = true;
            state = SoulCollectState.PreviewPercent;
        }

        private void ApplyAimAssist()
        {
            if (runtimeConfig.AimAssistRadius <= 0f)
            {
                return;
            }

            var target = PercentToNormalized(runtimeConfig.TargetPercent);
            var radius = runtimeConfig.AimAssistRadius / 100f;
            var distance = Mathf.Abs(targetNormalized - target);
            if (distance > radius || radius <= 0.0001f)
            {
                return;
            }

            var assist = 1f - Mathf.Clamp01(distance / radius);
            targetNormalized = Mathf.Lerp(targetNormalized, target, assist * 0.25f);
        }

        private void UpdateAimingSmoothing()
        {
            var lerpT = 1f - Mathf.Exp(-runtimeConfig.DragDamping * 60f * Time.unscaledDeltaTime);
            currentNormalized = Mathf.Lerp(currentNormalized, targetNormalized, lerpT);
        }

        private void UpdateSplitLinePosition()
        {
            if (splitLine == null)
            {
                return;
            }

            splitLine.anchorMin = new Vector2(currentNormalized, 0.5f);
            splitLine.anchorMax = new Vector2(currentNormalized, 0.5f);
            splitLine.anchoredPosition = Vector2.zero;
        }

        private void RefreshPreview(bool force)
        {
            var actualRaw = Mathf.Clamp(currentNormalized * 100f, 0f, 100f);
            var actualPercent = Mathf.RoundToInt(actualRaw);
            if (!force && actualPercent == previewPercentInt)
            {
                ApplyRangeVisuals(actualRaw);
                return;
            }

            previewPercentInt = actualPercent;
            if (percentText != null)
            {
                percentText.SetText("{0}%", previewPercentInt);
            }

            ApplyRangeVisuals(actualRaw);
        }

        private void ApplyRangeVisuals(float actualRaw)
        {
            var tooLow = actualRaw < runtimeConfig.MinPercent;
            var tooHigh = actualRaw > runtimeConfig.MaxPercent;
            var baseColor = panelData.ClientConfig != null ? panelData.ClientConfig.SoulColor : new Color(0.49f, 0.83f, 1f, 1f);

            if (splitLineImage != null)
            {
                if (tooLow)
                {
                    splitLineImage.color = Color.Lerp(baseColor, new Color(0.53f, 0.53f, 0.53f, 1f), 0.75f);
                }
                else if (tooHigh)
                {
                    splitLineImage.color = Color.Lerp(baseColor, new Color(0.93f, 0.20f, 0.20f, 1f), 0.8f);
                }
                else
                {
                    var target = runtimeConfig.TargetPercent;
                    var distanceToTarget = Mathf.Abs(actualRaw - target);
                    var glow = 1f - Mathf.Clamp01(distanceToTarget / Mathf.Max(1f, runtimeConfig.AimAssistRadius));
                    splitLineImage.color = Color.Lerp(baseColor, Color.white, glow * 0.45f);
                }
            }

            if (overdrawHintImage != null)
            {
                var color = overdrawHintImage.color;
                color.a = tooHigh ? 0.55f : 0f;
                overdrawHintImage.color = color;
            }

            if (underdrawHintImage != null)
            {
                var color = underdrawHintImage.color;
                color.a = tooLow ? 0.45f : 0f;
                underdrawHintImage.color = color;
            }
        }

        private void OnConfirmClicked()
        {
            if (inputLocked || (state != SoulCollectState.Aiming && state != SoulCollectState.PreviewPercent))
            {
                return;
            }

            inputLocked = true;
            state = SoulCollectState.ConfirmSplit;
            RefreshButtons();

            if (resolveCoroutine != null)
            {
                StopCoroutine(resolveCoroutine);
            }

            resolveCoroutine = StartCoroutine(ResolveRoutine());
        }

        private IEnumerator ResolveRoutine()
        {
            EmitCue("sfx.contract.soul.split", splitLine != null ? splitLine : transform, 1f);
            EmitCue("vfx.contract.soul.split_arc", splitLine != null ? splitLine : transform, 1f);

            var lineBaseScale = splitLine != null ? splitLine.localScale : Vector3.one;
            var splitAnimDuration = Mathf.Max(0.1f, runtimeConfig.SplitAnimDuration);
            var elapsed = 0f;
            while (elapsed < splitAnimDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / splitAnimDuration);
                var pulse = Mathf.Sin(t * Mathf.PI);

                if (splitLine != null)
                {
                    splitLine.localScale = new Vector3(lineBaseScale.x * (1f + pulse * 0.24f), lineBaseScale.y, 1f);
                }

                if (soulCoreImage != null)
                {
                    var color = soulCoreImage.color;
                    color.a = Mathf.Lerp(0.75f, 1f, pulse);
                    soulCoreImage.color = color;
                }

                yield return null;
            }

            if (splitLine != null)
            {
                splitLine.localScale = lineBaseScale;
            }

            var actualRaw = Mathf.Clamp(currentNormalized * 100f, 0f, 100f);
            var actualPercent = Mathf.RoundToInt(actualRaw);
            var resolveType = ResolveResultType(actualPercent, runtimeConfig.MinPercent, runtimeConfig.MaxPercent);

            state = SoulCollectState.Resolve;
            ApplyRangeVisuals(actualRaw);
            EmitResultCue(resolveType);

            switch (resolveType)
            {
                case SoulCollectResolveType.TooLow:
                    SetStatus("收取过少：记为签约失误。");
                    break;
                case SoulCollectResolveType.TooHigh:
                    SetStatus("收取过多：客户满意度下降。");
                    break;
                default:
                    SetStatus("收取比例合规，流程继续。");
                    break;
            }

            RefreshButtons();
            yield return new WaitForSecondsRealtime(runtimeConfig.ResultStaySeconds);

            if (completionSent)
            {
                yield break;
            }

            completionSent = true;
            state = SoulCollectState.Completed;
            RefreshButtons();

            panelData.OnCompleted?.Invoke(new SoulCollectResultPayload
            {
                ClientId = Mathf.Max(1, panelData.ClientId),
                TargetPercent = runtimeConfig.TargetPercent,
                MinPercent = runtimeConfig.MinPercent,
                MaxPercent = runtimeConfig.MaxPercent,
                ActualPercent = actualPercent,
                ActualRawFloat = actualRaw,
                ResolveType = resolveType,
                WasFallback = false
            });
        }

        private void EmitResultCue(SoulCollectResolveType resolveType)
        {
            switch (resolveType)
            {
                case SoulCollectResolveType.TooLow:
                    EmitCue("sfx.contract.soul.result", soulArea != null ? soulArea : transform, 0.8f);
                    break;
                case SoulCollectResolveType.TooHigh:
                    EmitCue("sfx.contract.soul.result", soulArea != null ? soulArea : transform, 1.1f);
                    EmitCue("vfx.contract.soul.overdraw_warning", soulArea != null ? soulArea : transform, 1f);
                    break;
                default:
                    EmitCue("sfx.contract.soul.result", soulArea != null ? soulArea : transform, 1f);
                    break;
            }
        }

        private static SoulCollectResolveType ResolveResultType(int actualPercent, int minPercent, int maxPercent)
        {
            if (actualPercent < minPercent)
            {
                return SoulCollectResolveType.TooLow;
            }

            if (actualPercent > maxPercent)
            {
                return SoulCollectResolveType.TooHigh;
            }

            return SoulCollectResolveType.Normal;
        }

        private void OnResetClicked()
        {
            if (inputLocked || (state != SoulCollectState.Aiming && state != SoulCollectState.PreviewPercent))
            {
                return;
            }

            targetNormalized = PercentToNormalized(runtimeConfig.TargetPercent);
            currentNormalized = targetNormalized;
            state = SoulCollectState.Aiming;
            hasUserAimed = false;
            previewTickTimer = 0f;
            nextDragCueTime = 0f;
            UpdateSplitLinePosition();
            RefreshPreview(force: true);
            SetStatus("已重置到目标附近，可继续调整。");
            RefreshButtons();
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

        private void RefreshButtons()
        {
            var canOperate = !inputLocked &&
                             (state == SoulCollectState.Aiming || state == SoulCollectState.PreviewPercent);

            if (confirmButton != null)
            {
                confirmButton.interactable = canOperate;
            }

            if (resetButton != null)
            {
                resetButton.interactable = canOperate;
            }

            if (guideToggleButton != null)
            {
                guideToggleButton.interactable = true;
            }

            if (confirmButtonLabel != null)
            {
                confirmButtonLabel.text = state switch
                {
                    SoulCollectState.SoulReveal => "显现中...",
                    SoulCollectState.Aiming => "确认分割",
                    SoulCollectState.PreviewPercent => "确认分割",
                    SoulCollectState.ConfirmSplit => "分割中...",
                    SoulCollectState.Resolve => "结算中...",
                    _ => "已完成"
                };
            }

            if (resetButtonLabel != null)
            {
                resetButtonLabel.text = "重置位置";
            }
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        private void EmitCue(string cueId, Transform anchor, float intensity)
        {
            panelData.OnFxCue?.Invoke(cueId, anchor, intensity);
        }

        private static float PercentToNormalized(int percent)
        {
            return Mathf.Clamp01(percent / 100f);
        }

        public IArchitecture GetArchitecture()
        {
            return MainMenuApp.Interface;
        }
    }
}
