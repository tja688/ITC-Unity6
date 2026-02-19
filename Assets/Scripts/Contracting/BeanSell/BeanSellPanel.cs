using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ITC.Contracting
{
    public sealed class BeanSellPanel : UIPanel, IController
    {
        private enum BeanSellState
        {
            Intro,
            PitchChoosing,
            Resolving,
            Completed
        }

        [Header("Roots")]
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private RectTransform beanRoot;

        [Header("Buttons")]
        [SerializeField] private Button strongPushButton;
        [SerializeField] private Button empathyButton;
        [SerializeField] private Button benefitButton;
        [SerializeField] private Button guideToggleButton;

        [Header("Text")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private TMP_Text preferenceText;
        [SerializeField] private TMP_Text soldCountText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text guideToggleLabel;

        [Header("Visual")]
        [SerializeField] private Image resultGlowImage;
        [SerializeField] private bool showGuideOnOpen = true;

        private readonly List<BeanSellGuideTag> guideTags = new();
        private BeanSellPanelData panelData = new();
        private BeanSellConfig runtimeConfig = new();
        private BeanSellState state;
        private bool guideVisible;
        private bool completionSent;
        private Coroutine resolveCoroutine;

        private void Awake()
        {
            CacheReferences();
        }

        protected override void OnInit(IUIData uiData = null)
        {
            panelData = uiData as BeanSellPanelData ?? new BeanSellPanelData();
            CacheReferences();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            panelData = uiData as BeanSellPanelData ?? panelData ?? new BeanSellPanelData();
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
        }

        private void EnsurePanelConfig()
        {
            var configModel = this.GetModel<ContractClientConfigModel>();
            runtimeConfig = (panelData.RuntimeConfig ?? configModel.BuildBeanSellRuntimeConfig() ?? new BeanSellConfig()).Clone();
            panelData.RuntimeConfig = runtimeConfig;

            if (panelData.ClientConfig == null)
            {
                panelData.ClientConfig = configModel.GetBeanSellClientConfig(panelData.ClientId);
            }

            runtimeConfig.EnabledFromDay = Mathf.Clamp(runtimeConfig.EnabledFromDay, 1, 30);
            runtimeConfig.SuccessThreshold = Mathf.Clamp(runtimeConfig.SuccessThreshold, -10, 20);
            runtimeConfig.FailSatisfactionPenalty = Mathf.Clamp(runtimeConfig.FailSatisfactionPenalty, 0, 3);
            runtimeConfig.ResolveStaySeconds = Mathf.Clamp(runtimeConfig.ResolveStaySeconds, 0.1f, 2f);
        }

        private void CacheReferences()
        {
            if (panelRoot == null)
            {
                panelRoot = transform as RectTransform;
            }

            var canvasRoot = transform.Find("Canvas");
            if (beanRoot == null)
            {
                beanRoot = canvasRoot != null ? canvasRoot.Find("BeanRoot") as RectTransform : null;
            }

            if (titleText == null && beanRoot != null)
            {
                titleText = beanRoot.Find("HeaderText")?.GetComponent<TMP_Text>();
            }

            if (hintText == null && beanRoot != null)
            {
                hintText = beanRoot.Find("HintText")?.GetComponent<TMP_Text>();
            }

            if (preferenceText == null && beanRoot != null)
            {
                preferenceText = beanRoot.Find("PreferenceText")?.GetComponent<TMP_Text>();
            }

            if (soldCountText == null && beanRoot != null)
            {
                soldCountText = beanRoot.Find("SoldCountText")?.GetComponent<TMP_Text>();
            }

            if (scoreText == null && beanRoot != null)
            {
                scoreText = beanRoot.Find("ScoreText")?.GetComponent<TMP_Text>();
            }

            if (statusText == null && beanRoot != null)
            {
                statusText = beanRoot.Find("StatusText")?.GetComponent<TMP_Text>();
            }

            if (strongPushButton == null && beanRoot != null)
            {
                strongPushButton = beanRoot.Find("PitchButtons/StrongPushButton")?.GetComponent<Button>();
            }

            if (empathyButton == null && beanRoot != null)
            {
                empathyButton = beanRoot.Find("PitchButtons/EmpathyButton")?.GetComponent<Button>();
            }

            if (benefitButton == null && beanRoot != null)
            {
                benefitButton = beanRoot.Find("PitchButtons/BenefitButton")?.GetComponent<Button>();
            }

            if (guideToggleButton == null && beanRoot != null)
            {
                guideToggleButton = beanRoot.Find("GuideToggleButton")?.GetComponent<Button>();
            }

            if (guideToggleLabel == null && beanRoot != null)
            {
                guideToggleLabel = beanRoot.Find("GuideToggleButton/Label")?.GetComponent<TMP_Text>();
            }

            if (resultGlowImage == null && beanRoot != null)
            {
                resultGlowImage = beanRoot.Find("ResultGlow")?.GetComponent<Image>();
            }

            RebuildGuideTags();
        }

        private void RebuildGuideTags()
        {
            guideTags.Clear();
            if (beanRoot == null)
            {
                return;
            }

            var found = beanRoot.GetComponentsInChildren<BeanSellGuideTag>(true);
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

            if (strongPushButton != null)
            {
                strongPushButton.onClick.AddListener(() => OnPitchSelected(BeanPitchType.StrongPush));
            }

            if (empathyButton != null)
            {
                empathyButton.onClick.AddListener(() => OnPitchSelected(BeanPitchType.Empathy));
            }

            if (benefitButton != null)
            {
                benefitButton.onClick.AddListener(() => OnPitchSelected(BeanPitchType.Benefit));
            }

            if (guideToggleButton != null)
            {
                guideToggleButton.onClick.AddListener(OnGuideToggleClicked);
            }
        }

        private void UnregisterRuntimeListeners()
        {
            if (strongPushButton != null)
            {
                strongPushButton.onClick.RemoveAllListeners();
            }

            if (empathyButton != null)
            {
                empathyButton.onClick.RemoveAllListeners();
            }

            if (benefitButton != null)
            {
                benefitButton.onClick.RemoveAllListeners();
            }

            if (guideToggleButton != null)
            {
                guideToggleButton.onClick.RemoveListener(OnGuideToggleClicked);
            }
        }

        private void ResetRound()
        {
            state = BeanSellState.Intro;
            completionSent = false;
            SetGuideVisible(showGuideOnOpen);

            var clientName = panelData.ClientConfig != null && !string.IsNullOrWhiteSpace(panelData.ClientConfig.ClientDisplayName)
                ? panelData.ClientConfig.ClientDisplayName
                : $"客户{Mathf.Max(1, panelData.ClientId)}";

            if (titleText != null)
            {
                titleText.text = $"豆罐头推销 - {clientName}";
            }

            if (hintText != null)
            {
                hintText.text = "请选择一条话术：强推 / 共情 / 利益。";
            }

            if (preferenceText != null)
            {
                var preference = runtimeConfig.ShowPreferenceHint
                    ? GetPitchDisplayName(panelData.ClientConfig != null
                        ? panelData.ClientConfig.PreferredPitch
                        : BeanPitchType.Benefit)
                    : "隐藏";
                preferenceText.text = $"客户偏好：{preference}";
            }

            var soldCount = this.GetModel<ContractFlowStateModel>().RouteBeanSoldCount.Value;
            if (soldCountText != null)
            {
                soldCountText.SetText("今日已售：{0}", Mathf.Max(0, soldCount));
            }

            if (scoreText != null)
            {
                scoreText.text = "分数：待选择";
            }

            SetStatus("请选择话术，完成本次推销判定。");
            SetResultGlow(false);

            state = BeanSellState.PitchChoosing;
            RefreshButtons();
        }

        private void OnPitchSelected(BeanPitchType pitchType)
        {
            if (state != BeanSellState.PitchChoosing)
            {
                return;
            }

            if (resolveCoroutine != null)
            {
                StopCoroutine(resolveCoroutine);
            }

            resolveCoroutine = StartCoroutine(ResolveRoutine(pitchType));
        }

        private IEnumerator ResolveRoutine(BeanPitchType pitchType)
        {
            state = BeanSellState.Resolving;
            RefreshButtons();
            EmitCue("sfx.contract.bean.offer", transform, 1f);

            var clientConfig = panelData.ClientConfig ?? new BeanSellClientConfig();
            var baseWill = clientConfig.BuyWillingness;
            var pitchBonus = pitchType == clientConfig.PreferredPitch ? 2 : -1;
            var weight = runtimeConfig.ResolvePitchWeight(pitchType);
            var situationalModifier = clientConfig.SituationalModifier;
            var finalScore = baseWill + pitchBonus + weight + situationalModifier;
            var threshold = runtimeConfig.SuccessThreshold;
            var success = finalScore >= threshold;

            if (scoreText != null)
            {
                scoreText.SetText("分数：{0}（阈值 {1}）", finalScore, threshold);
            }

            if (success)
            {
                SetStatus($"推销成功：{GetPitchDisplayName(pitchType)} 命中客户偏好。");
                SetResultGlow(true);
                EmitCue("sfx.contract.bean.success", transform, 1f);
                EmitCue("vfx.contract.bean.coin_pop", transform, 0.9f);
            }
            else
            {
                SetStatus($"推销失败：{GetPitchDisplayName(pitchType)} 未说服客户。");
                SetResultGlow(false);
                EmitCue("sfx.contract.bean.fail", transform, 1f);
                EmitCue("vfx.contract.bean.fail_fade", transform, 0.9f);
            }

            yield return new WaitForSecondsRealtime(runtimeConfig.ResolveStaySeconds);

            if (completionSent)
            {
                yield break;
            }

            completionSent = true;
            state = BeanSellState.Completed;
            RefreshButtons();

            panelData.OnCompleted?.Invoke(new BeanSellResultPayload
            {
                ClientId = Mathf.Max(1, panelData.ClientId),
                Day = Mathf.Max(1, panelData.Day),
                PitchType = pitchType,
                FinalScore = finalScore,
                SuccessThreshold = threshold,
                IsSuccess = success,
                IsSkipped = false,
                WasFallback = false
            });
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
            var canSelect = state == BeanSellState.PitchChoosing;

            if (strongPushButton != null)
            {
                strongPushButton.interactable = canSelect;
            }

            if (empathyButton != null)
            {
                empathyButton.interactable = canSelect;
            }

            if (benefitButton != null)
            {
                benefitButton.interactable = canSelect;
            }
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message ?? string.Empty;
            }
        }

        private void SetResultGlow(bool success)
        {
            if (resultGlowImage == null)
            {
                return;
            }

            resultGlowImage.color = success
                ? new Color(0.24f, 0.76f, 0.34f, 0.45f)
                : new Color(0.72f, 0.20f, 0.20f, 0.42f);
        }

        private static string GetPitchDisplayName(BeanPitchType pitchType)
        {
            return pitchType switch
            {
                BeanPitchType.StrongPush => "强推",
                BeanPitchType.Empathy => "共情",
                BeanPitchType.Benefit => "利益",
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
