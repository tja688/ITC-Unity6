using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ITC.Contracting
{
    public sealed class StampPanel : UIPanel, IController
    {
        private enum StampState
        {
            ChooseType,
            PrepareStamp,
            ChargeLoop,
            HitWindow,
            Resolve,
            Completed
        }

        [Header("Roots")]
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private RectTransform stampRoot;
        [SerializeField] private RectTransform chargeVisualRoot;
        [SerializeField] private RectTransform chargeTrack;
        [SerializeField] private RectTransform perfectWindow;
        [SerializeField] private RectTransform normalWindow;
        [SerializeField] private RectTransform marker;
        [SerializeField] private Transform stampTargetAnchor;

        [Header("Buttons")]
        [SerializeField] private Button moneyButton;
        [SerializeField] private Button fameButton;
        [SerializeField] private Button skillButton;
        [SerializeField] private Button eventButton;
        [SerializeField] private Button stampButton;
        [SerializeField] private Button reselectButton;
        [SerializeField] private Button guideToggleButton;

        [Header("Text")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text stampButtonLabel;
        [SerializeField] private TMP_Text reselectButtonLabel;
        [SerializeField] private TMP_Text guideToggleLabel;

        [Header("Visual")]
        [SerializeField] private Image chargeFill;
        [SerializeField] private bool showGuideOnOpen = true;

        [Header("Tuning")]
        [SerializeField] private float chargeWarmupSeconds = 0.12f;
        [SerializeField] private float hitSnapSeconds = 0.04f;
        [SerializeField] private float resolveFeedbackSeconds = 0.45f;
        [SerializeField] private float lightFailThreshold = 0.08f;

        private readonly Dictionary<Button, UnityAction> typeButtonListeners = new();
        private readonly Dictionary<Button, StampType> typeButtonMap = new();
        private readonly Dictionary<Button, Color> baseTypeButtonColors = new();
        private readonly List<StampGuideTag> guideTags = new();

        private StampPanelData panelData = new();
        private StampState state;
        private StampType selectedType;
        private bool hasSelectedType;
        private bool tutorialReselectAvailable;
        private bool guideVisible;
        private float chargeStartTime;
        private float chargeWarmupEndTime;
        private float currentChargeNormalized;
        private float debuffShakeEndTime;
        private float nextDebuffShakeTime;
        private int debuffShakeTriggeredCount;
        private Vector2 chargeVisualBasePosition;
        private Coroutine resolveCoroutine;

        private void Awake()
        {
            CacheReferences();
        }

        protected override void OnInit(IUIData uiData = null)
        {
            panelData = uiData as StampPanelData ?? new StampPanelData();
            CacheReferences();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            panelData = uiData as StampPanelData ?? panelData ?? new StampPanelData();
            EnsurePanelConfig();
            CacheReferences();
            RegisterRuntimeListeners();
            ResetRound();
        }

        protected override void OnClose()
        {
            UnregisterRuntimeListeners();
            if (resolveCoroutine != null)
            {
                StopCoroutine(resolveCoroutine);
                resolveCoroutine = null;
            }

            RestoreChargeRootPosition();
        }

        private void Update()
        {
            if (state != StampState.ChargeLoop && state != StampState.HitWindow)
            {
                return;
            }

            if (state == StampState.ChargeLoop && Time.unscaledTime >= chargeWarmupEndTime)
            {
                state = StampState.HitWindow;
                SetStatus("命中窗口已开启，按下“盖印”完成判定。");
                RefreshStampButtonLabel();
            }

            UpdateChargeProgress();
            ApplyDebuffShakeIfNeeded();
        }

        private void EnsurePanelConfig()
        {
            var configModel = this.GetModel<ContractClientConfigModel>();

            if (panelData.ClientConfig == null)
            {
                panelData.ClientConfig = configModel.GetStampClientConfig(panelData.ClientId);
            }

            if (panelData.RuntimeConfig == null)
            {
                panelData.RuntimeConfig = configModel.GetStampRuntimeConfig(panelData.ClientId);
            }

            panelData.RuntimeConfig.ChargeDuration = Mathf.Max(0.2f, panelData.RuntimeConfig.ChargeDuration);
            panelData.RuntimeConfig.PerfectWindowStart = Mathf.Clamp01(panelData.RuntimeConfig.PerfectWindowStart);
            panelData.RuntimeConfig.PerfectWindowEnd = Mathf.Clamp01(panelData.RuntimeConfig.PerfectWindowEnd);
            if (panelData.RuntimeConfig.PerfectWindowEnd < panelData.RuntimeConfig.PerfectWindowStart)
            {
                (panelData.RuntimeConfig.PerfectWindowStart, panelData.RuntimeConfig.PerfectWindowEnd) =
                    (panelData.RuntimeConfig.PerfectWindowEnd, panelData.RuntimeConfig.PerfectWindowStart);
            }
        }

        private void CacheReferences()
        {
            if (panelRoot == null)
            {
                panelRoot = transform as RectTransform;
            }

            var canvasRoot = transform.Find("Canvas");

            if (stampRoot == null)
            {
                stampRoot = canvasRoot != null ? canvasRoot.Find("StampRoot") as RectTransform : null;
            }

            if (titleText == null && stampRoot != null)
            {
                titleText = stampRoot.Find("HeaderText")?.GetComponent<TMP_Text>();
            }

            if (hintText == null && stampRoot != null)
            {
                hintText = stampRoot.Find("HintText")?.GetComponent<TMP_Text>();
            }

            if (statusText == null && stampRoot != null)
            {
                statusText = stampRoot.Find("StatusText")?.GetComponent<TMP_Text>();
            }

            if (guideToggleButton == null && stampRoot != null)
            {
                guideToggleButton = stampRoot.Find("GuideToggleButton")?.GetComponent<Button>();
            }

            if (guideToggleLabel == null && stampRoot != null)
            {
                guideToggleLabel = stampRoot.Find("GuideToggleButton/Label")?.GetComponent<TMP_Text>();
            }

            if (moneyButton == null && stampRoot != null)
            {
                moneyButton = stampRoot.Find("TypeButtons/MoneyButton")?.GetComponent<Button>();
            }

            if (fameButton == null && stampRoot != null)
            {
                fameButton = stampRoot.Find("TypeButtons/FameButton")?.GetComponent<Button>();
            }

            if (skillButton == null && stampRoot != null)
            {
                skillButton = stampRoot.Find("TypeButtons/SkillButton")?.GetComponent<Button>();
            }

            if (eventButton == null && stampRoot != null)
            {
                eventButton = stampRoot.Find("TypeButtons/EventButton")?.GetComponent<Button>();
            }

            if (stampButton == null && stampRoot != null)
            {
                stampButton = stampRoot.Find("Controls/StampButton")?.GetComponent<Button>();
            }

            if (stampButtonLabel == null && stampRoot != null)
            {
                stampButtonLabel = stampRoot.Find("Controls/StampButton/Label")?.GetComponent<TMP_Text>();
            }

            if (reselectButton == null && stampRoot != null)
            {
                reselectButton = stampRoot.Find("Controls/ReselectButton")?.GetComponent<Button>();
            }

            if (reselectButtonLabel == null && stampRoot != null)
            {
                reselectButtonLabel = stampRoot.Find("Controls/ReselectButton/Label")?.GetComponent<TMP_Text>();
            }

            if (chargeVisualRoot == null && stampRoot != null)
            {
                chargeVisualRoot = stampRoot.Find("ChargeArea") as RectTransform;
            }

            if (chargeTrack == null && chargeVisualRoot != null)
            {
                chargeTrack = chargeVisualRoot.Find("ChargeTrack") as RectTransform;
            }

            if (chargeFill == null && chargeTrack != null)
            {
                chargeFill = chargeTrack.Find("ChargeFill")?.GetComponent<Image>();
            }

            if (perfectWindow == null && chargeTrack != null)
            {
                perfectWindow = chargeTrack.Find("PerfectWindow") as RectTransform;
            }

            if (normalWindow == null && chargeTrack != null)
            {
                normalWindow = chargeTrack.Find("NormalWindow") as RectTransform;
            }

            if (marker == null && chargeTrack != null)
            {
                marker = chargeTrack.Find("Marker") as RectTransform;
            }

            if (stampTargetAnchor == null && chargeVisualRoot != null)
            {
                stampTargetAnchor = chargeVisualRoot.Find("TargetArea");
            }

            if (chargeVisualRoot != null)
            {
                chargeVisualBasePosition = chargeVisualRoot.anchoredPosition;
            }

            RebuildGuideTags();
        }

        private void RebuildGuideTags()
        {
            guideTags.Clear();
            if (stampRoot == null)
            {
                return;
            }

            var found = stampRoot.GetComponentsInChildren<StampGuideTag>(true);
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

            RegisterTypeButton(moneyButton, StampType.Money);
            RegisterTypeButton(fameButton, StampType.Fame);
            RegisterTypeButton(skillButton, StampType.Skill);
            RegisterTypeButton(eventButton, StampType.Event);

            if (stampButton != null)
            {
                stampButton.onClick.AddListener(OnStampButtonClicked);
            }

            if (reselectButton != null)
            {
                reselectButton.onClick.AddListener(OnReselectClicked);
            }

            if (guideToggleButton != null)
            {
                guideToggleButton.onClick.AddListener(OnGuideToggleClicked);
            }
        }

        private void RegisterTypeButton(Button button, StampType stampType)
        {
            if (button == null)
            {
                return;
            }

            typeButtonMap[button] = stampType;
            if (button.image != null)
            {
                baseTypeButtonColors[button] = button.image.color;
            }

            UnityAction listener = () => OnTypeButtonClicked(button);
            typeButtonListeners[button] = listener;
            button.onClick.AddListener(listener);
        }

        private void UnregisterRuntimeListeners()
        {
            foreach (var pair in typeButtonListeners)
            {
                if (pair.Key != null)
                {
                    pair.Key.onClick.RemoveListener(pair.Value);
                }
            }

            typeButtonListeners.Clear();
            typeButtonMap.Clear();
            baseTypeButtonColors.Clear();

            if (stampButton != null)
            {
                stampButton.onClick.RemoveListener(OnStampButtonClicked);
            }

            if (reselectButton != null)
            {
                reselectButton.onClick.RemoveListener(OnReselectClicked);
            }

            if (guideToggleButton != null)
            {
                guideToggleButton.onClick.RemoveListener(OnGuideToggleClicked);
            }
        }

        private void ResetRound()
        {
            state = StampState.ChooseType;
            hasSelectedType = false;
            selectedType = StampType.Event;
            tutorialReselectAvailable = panelData.TutorialMode;
            chargeStartTime = 0f;
            chargeWarmupEndTime = 0f;
            currentChargeNormalized = 0f;
            debuffShakeEndTime = 0f;
            nextDebuffShakeTime = 0f;
            debuffShakeTriggeredCount = 0;

            ApplyTimingWindowVisuals();
            UpdateChargeVisual(0f);
            RestoreChargeRootPosition();
            RefreshTitleAndHint();
            RefreshTypeButtonVisuals();
            RefreshButtonInteractable();
            RefreshStampButtonLabel();
            SetStatus("请选择印章类型。");
            SetGuideVisible(showGuideOnOpen);
        }

        private void RefreshTitleAndHint()
        {
            if (titleText != null)
            {
                var clientName = panelData.ClientConfig != null && !string.IsNullOrWhiteSpace(panelData.ClientConfig.ClientDisplayName)
                    ? panelData.ClientConfig.ClientDisplayName
                    : $"客户{Mathf.Max(1, panelData.ClientId)}";
                titleText.text = $"印章盖印 - {clientName}";
            }

            if (hintText != null)
            {
                var debuffHint = panelData.HasVerifyDebuff ? "核验干扰：已启用。 " : string.Empty;
                hintText.text = debuffHint + "先选印章，再在蓄力循环中按下“盖印”。";
            }
        }

        private void ApplyTimingWindowVisuals()
        {
            var perfectStart = panelData.RuntimeConfig.PerfectWindowStart;
            var perfectEnd = panelData.RuntimeConfig.PerfectWindowEnd;
            var normalPadding = Mathf.Max(0f, panelData.RuntimeConfig.NormalWindowPadding);
            var normalStart = Mathf.Clamp01(perfectStart - normalPadding);
            var normalEnd = Mathf.Clamp01(perfectEnd + normalPadding);

            SetWindowRect(normalWindow, normalStart, normalEnd);
            SetWindowRect(perfectWindow, perfectStart, perfectEnd);
        }

        private static void SetWindowRect(RectTransform windowRect, float start, float end)
        {
            if (windowRect == null)
            {
                return;
            }

            windowRect.anchorMin = new Vector2(start, 0f);
            windowRect.anchorMax = new Vector2(Mathf.Max(start + 0.001f, end), 1f);
            windowRect.offsetMin = Vector2.zero;
            windowRect.offsetMax = Vector2.zero;
        }

        private void OnTypeButtonClicked(Button button)
        {
            if (state != StampState.ChooseType || button == null || !typeButtonMap.TryGetValue(button, out var stampType))
            {
                return;
            }

            selectedType = stampType;
            hasSelectedType = true;
            state = StampState.PrepareStamp;
            EmitCue("sfx.contract.stamp.select", button.transform, 1f);
            SetStatus($"已选择：{GetTypeDisplayName(stampType)}。点击“开始盖印”进入时机判定。");
            RefreshTypeButtonVisuals();
            RefreshButtonInteractable();
            RefreshStampButtonLabel();
        }

        private void OnReselectClicked()
        {
            if (state != StampState.PrepareStamp || !panelData.TutorialMode || !tutorialReselectAvailable)
            {
                return;
            }

            tutorialReselectAvailable = false;
            hasSelectedType = false;
            state = StampState.ChooseType;
            SetStatus("已返回重选。请重新选择一个印章类型。");
            RefreshTypeButtonVisuals();
            RefreshButtonInteractable();
            RefreshStampButtonLabel();
        }

        private void OnStampButtonClicked()
        {
            if (state == StampState.Resolve || state == StampState.Completed)
            {
                return;
            }

            if (state == StampState.ChooseType || !hasSelectedType)
            {
                SetStatus("请先选择印章类型。");
                return;
            }

            if (state == StampState.PrepareStamp)
            {
                // Always enter the charge loop regardless of type correctness.
                // Type correctness is evaluated after the timing phase completes.
                StartChargeLoop();
                return;
            }

            if (state == StampState.ChargeLoop || state == StampState.HitWindow)
            {
                var typeCorrect = selectedType == panelData.RuntimeConfig.CorrectStampType;
                var timingResult = EvaluateTiming(currentChargeNormalized, out var lightFail);
                StartResolve(selectedType, timingResult, currentChargeNormalized, typeCorrect, lightFail);
            }
        }

        private void StartChargeLoop()
        {
            state = StampState.ChargeLoop;
            chargeStartTime = Time.unscaledTime;
            chargeWarmupEndTime = chargeStartTime + Mathf.Max(0f, chargeWarmupSeconds);
            currentChargeNormalized = 0f;
            SetStatus("蓄力中，请观察命中窗口并按下“盖印”。");
            EmitCue("sfx.contract.stamp.charge", chargeVisualRoot != null ? chargeVisualRoot : transform, 1f);
            EmitCue("vfx.contract.stamp.charge_glow", chargeVisualRoot != null ? chargeVisualRoot : transform, 0.9f);

            if (panelData.HasVerifyDebuff)
            {
                debuffShakeTriggeredCount = 0;
                nextDebuffShakeTime = Time.unscaledTime + 0.24f;
                debuffShakeEndTime = 0f;
            }

            RefreshButtonInteractable();
            RefreshStampButtonLabel();
        }

        private void StartResolve(
            StampType stampType,
            StampTimingResult timingResult,
            float hitNormalized,
            bool typeCorrect,
            bool lightFail)
        {
            if (resolveCoroutine != null)
            {
                StopCoroutine(resolveCoroutine);
            }

            resolveCoroutine = StartCoroutine(ResolveRoutine(stampType, timingResult, hitNormalized, typeCorrect, lightFail));
        }

        private IEnumerator ResolveRoutine(
            StampType stampType,
            StampTimingResult timingResult,
            float hitNormalized,
            bool typeCorrect,
            bool lightFail)
        {
            state = StampState.Resolve;
            RefreshButtonInteractable();
            RefreshStampButtonLabel();
            RestoreChargeRootPosition();

            var anchor = stampTargetAnchor != null ? stampTargetAnchor : transform;

            // Always show timing feedback first
            if (timingResult != StampTimingResult.Failed)
            {
                yield return new WaitForSecondsRealtime(Mathf.Max(0f, hitSnapSeconds));
            }

            // Determine composite result message
            if (!typeCorrect)
            {
                // Wrong type — show type error regardless of timing
                switch (timingResult)
                {
                    case StampTimingResult.Perfect:
                        SetStatus("时机完美，但印章类型错误！本轮记为签约失误。");
                        break;
                    case StampTimingResult.Normal:
                        SetStatus("时机一般，且印章类型错误。本轮记为签约失误。");
                        break;
                    default:
                        SetStatus("时机与印章类型均有误。本轮记为签约失误。");
                        break;
                }
                EmitCue("sfx.contract.stamp.hit_fail", anchor, 1f);
                EmitCue("vfx.contract.stamp.paper_burn", anchor, 0.9f);
            }
            else
            {
                switch (timingResult)
                {
                    case StampTimingResult.Perfect:
                        SetStatus("完美命中，客户满意度提升。");
                        EmitCue("sfx.contract.stamp.hit_perfect", anchor, 1f);
                        EmitCue("vfx.contract.stamp.blood_bloom", anchor, 1f);
                        break;
                    case StampTimingResult.Normal:
                        SetStatus("普通命中，流程继续。");
                        EmitCue("sfx.contract.stamp.hit_perfect", anchor, 0.65f);
                        EmitCue("vfx.contract.stamp.blood_bloom", anchor, 0.6f);
                        break;
                    default:
                        SetStatus(lightFail ? "轻微偏差，判定失败。" : "重偏差，判定失败。");
                        EmitCue("sfx.contract.stamp.hit_fail", anchor, lightFail ? 0.65f : 1.2f);
                        EmitCue("vfx.contract.stamp.paper_burn", anchor, lightFail ? 0.55f : 1f);
                        break;
                }
            }

            // Use extended feedback time for wrong-type so player clearly sees the result
            var baseDuration = Mathf.Max(0.05f, resolveFeedbackSeconds);
            var duration = typeCorrect ? baseDuration : Mathf.Max(baseDuration, 1.2f);
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            state = StampState.Completed;
            RefreshButtonInteractable();
            RefreshStampButtonLabel();

            panelData.OnCompleted?.Invoke(new StampResultPayload
            {
                ClientId = Mathf.Max(1, panelData.ClientId),
                SelectedStampType = stampType,
                TimingResult = timingResult,
                HitNormalizedTime = Mathf.Clamp01(hitNormalized),
                TypeCorrect = typeCorrect,
                HasVerifyDebuff = panelData.HasVerifyDebuff,
                WasFallback = false
            });
        }

        private StampTimingResult EvaluateTiming(float normalized, out bool lightFail)
        {
            var perfectStart = panelData.RuntimeConfig.PerfectWindowStart;
            var perfectEnd = panelData.RuntimeConfig.PerfectWindowEnd;
            var normalPadding = Mathf.Max(0f, panelData.RuntimeConfig.NormalWindowPadding);
            var normalStart = Mathf.Clamp01(perfectStart - normalPadding);
            var normalEnd = Mathf.Clamp01(perfectEnd + normalPadding);

            if (normalized >= perfectStart && normalized <= perfectEnd)
            {
                lightFail = false;
                return StampTimingResult.Perfect;
            }

            if (normalized >= normalStart && normalized <= normalEnd)
            {
                lightFail = false;
                return StampTimingResult.Normal;
            }

            var distance = normalized < normalStart
                ? normalStart - normalized
                : normalized - normalEnd;

            lightFail = distance <= Mathf.Max(0.01f, lightFailThreshold);
            return StampTimingResult.Failed;
        }

        private void UpdateChargeProgress()
        {
            var duration = Mathf.Max(0.2f, panelData.RuntimeConfig.ChargeDuration);
            var elapsed = Mathf.Max(0f, Time.unscaledTime - chargeStartTime);
            currentChargeNormalized = Mathf.Repeat(elapsed, duration) / duration;
            UpdateChargeVisual(currentChargeNormalized);
        }

        private void UpdateChargeVisual(float normalized)
        {
            if (chargeFill != null)
            {
                chargeFill.fillAmount = Mathf.Clamp01(normalized);
            }

            if (marker != null)
            {
                marker.anchorMin = new Vector2(normalized, 0.5f);
                marker.anchorMax = new Vector2(normalized, 0.5f);
                marker.anchoredPosition = Vector2.zero;
            }
        }

        private void ApplyDebuffShakeIfNeeded()
        {
            if (!panelData.HasVerifyDebuff || chargeVisualRoot == null)
            {
                return;
            }

            var maxShakeCount = Mathf.Clamp(panelData.RuntimeConfig.VerifyDebuffShakeCount, 0, 6);
            if (maxShakeCount <= 0)
            {
                RestoreChargeRootPosition();
                return;
            }

            var now = Time.unscaledTime;
            if (debuffShakeTriggeredCount < maxShakeCount && now >= nextDebuffShakeTime)
            {
                debuffShakeTriggeredCount++;
                debuffShakeEndTime = now + 0.12f;
                nextDebuffShakeTime = now + Random.Range(0.22f, 0.46f);
            }

            if (now <= debuffShakeEndTime)
            {
                var normalized = Mathf.Clamp01((debuffShakeEndTime - now) / 0.12f);
                var amplitude = Mathf.Clamp(panelData.RuntimeConfig.VerifyDebuffShakeAmp, 2f, 20f);
                var x = Mathf.Sin(now * 102f) * amplitude * normalized;
                var y = Mathf.Cos(now * 88f) * amplitude * 0.45f * normalized;
                chargeVisualRoot.anchoredPosition = chargeVisualBasePosition + new Vector2(x, y);
            }
            else
            {
                RestoreChargeRootPosition();
            }
        }

        private void RestoreChargeRootPosition()
        {
            if (chargeVisualRoot != null)
            {
                chargeVisualRoot.anchoredPosition = chargeVisualBasePosition;
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

        private void RefreshTypeButtonVisuals()
        {
            foreach (var pair in typeButtonMap)
            {
                var button = pair.Key;
                if (button == null || button.image == null)
                {
                    continue;
                }

                var isSelected = hasSelectedType && selectedType == pair.Value;
                if (isSelected)
                {
                    button.image.color = new Color32(92, 166, 103, 255);
                }
                else if (baseTypeButtonColors.TryGetValue(button, out var baseColor))
                {
                    button.image.color = baseColor;
                }
            }
        }

        private void RefreshButtonInteractable()
        {
            var allowTypePick = state == StampState.ChooseType;
            foreach (var pair in typeButtonMap)
            {
                if (pair.Key != null)
                {
                    pair.Key.interactable = allowTypePick;
                }
            }

            if (stampButton != null)
            {
                stampButton.interactable = state != StampState.Resolve && state != StampState.Completed;
            }

            if (reselectButton != null)
            {
                reselectButton.interactable =
                    state == StampState.PrepareStamp &&
                    panelData.TutorialMode &&
                    tutorialReselectAvailable;
            }

            if (guideToggleButton != null)
            {
                guideToggleButton.interactable = true;
            }

            if (reselectButtonLabel != null)
            {
                reselectButtonLabel.text = tutorialReselectAvailable ? "重选印章" : "重选已用";
            }
        }

        private void RefreshStampButtonLabel()
        {
            if (stampButtonLabel == null)
            {
                return;
            }

            stampButtonLabel.text = state switch
            {
                StampState.ChooseType => "请先选印章",
                StampState.PrepareStamp => "开始盖印",
                StampState.ChargeLoop => "等待命中",
                StampState.HitWindow => "按下盖印",
                StampState.Resolve => "结算中...",
                _ => "已完成"
            };
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }

        private static string GetTypeDisplayName(StampType stampType)
        {
            return stampType switch
            {
                StampType.Money => "金钱",
                StampType.Fame => "名利",
                StampType.Skill => "特技",
                StampType.Event => "事件",
                _ => "未知"
            };
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
