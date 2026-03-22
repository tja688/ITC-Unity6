using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ITC.SignMiniGame
{
    public sealed class SettlementPanel : UIPanel, IController
    {
        private enum SettlementState
        {
            Collect,
            Evaluate,
            Reward,
            Narrate,
            RouteNext,
            Completed
        }

        [Header("Roots")]
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private RectTransform settlementRoot;

        [Header("Controls")]
        [SerializeField] private Button skipButton;
        [SerializeField] private Button guideToggleButton;

        [Header("Text")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text summaryText;
        [SerializeField] private TMP_Text detailText;
        [SerializeField] private TMP_Text tipText;
        [SerializeField] private TMP_Text moneyText;
        [SerializeField] private TMP_Text tierText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text skipButtonLabel;
        [SerializeField] private TMP_Text guideToggleLabel;

        [Header("Visual")]
        [SerializeField] private Image pulseImage;
        [SerializeField] private bool showGuideOnOpen = true;

        private readonly List<SettlementGuideTag> guideTags = new();
        private SettlementPanelData panelData = new();
        private SettlementState state;
        private bool guideVisible;
        private bool completed;
        private Coroutine flowCoroutine;

        private void Awake()
        {
            CacheReferences();
        }

        protected override void OnInit(IUIData uiData = null)
        {
            panelData = uiData as SettlementPanelData ?? new SettlementPanelData();
            CacheReferences();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            panelData = uiData as SettlementPanelData ?? panelData ?? new SettlementPanelData();
            CacheReferences();
            RegisterRuntimeListeners();
            ResetRound();
        }

        protected override void OnClose()
        {
            UnregisterRuntimeListeners();
            if (flowCoroutine != null)
            {
                StopCoroutine(flowCoroutine);
                flowCoroutine = null;
            }
        }

        private void CacheReferences()
        {
            if (panelRoot == null)
            {
                panelRoot = transform as RectTransform;
            }

            var canvasRoot = transform.Find("Canvas");
            if (settlementRoot == null)
            {
                settlementRoot = canvasRoot != null ? canvasRoot.Find("SettlementRoot") as RectTransform : null;
            }

            if (titleText == null && settlementRoot != null)
            {
                titleText = settlementRoot.Find("HeaderText")?.GetComponent<TMP_Text>();
            }

            if (summaryText == null && settlementRoot != null)
            {
                summaryText = settlementRoot.Find("SummaryText")?.GetComponent<TMP_Text>();
            }

            if (detailText == null && settlementRoot != null)
            {
                detailText = settlementRoot.Find("DetailText")?.GetComponent<TMP_Text>();
            }

            if (tipText == null && settlementRoot != null)
            {
                tipText = settlementRoot.Find("TipText")?.GetComponent<TMP_Text>();
            }

            if (moneyText == null && settlementRoot != null)
            {
                moneyText = settlementRoot.Find("MoneyText")?.GetComponent<TMP_Text>();
            }

            if (tierText == null && settlementRoot != null)
            {
                tierText = settlementRoot.Find("TierText")?.GetComponent<TMP_Text>();
            }

            if (statusText == null && settlementRoot != null)
            {
                statusText = settlementRoot.Find("StatusText")?.GetComponent<TMP_Text>();
            }

            if (skipButton == null && settlementRoot != null)
            {
                skipButton = settlementRoot.Find("Controls/SkipButton")?.GetComponent<Button>();
            }

            if (skipButtonLabel == null && settlementRoot != null)
            {
                skipButtonLabel = settlementRoot.Find("Controls/SkipButton/Label")?.GetComponent<TMP_Text>();
            }

            if (guideToggleButton == null && settlementRoot != null)
            {
                guideToggleButton = settlementRoot.Find("GuideToggleButton")?.GetComponent<Button>();
            }

            if (guideToggleLabel == null && settlementRoot != null)
            {
                guideToggleLabel = settlementRoot.Find("GuideToggleButton/Label")?.GetComponent<TMP_Text>();
            }

            if (pulseImage == null && settlementRoot != null)
            {
                pulseImage = settlementRoot.Find("Pulse")?.GetComponent<Image>();
            }

            RebuildGuideTags();
        }

        private void RebuildGuideTags()
        {
            guideTags.Clear();
            if (settlementRoot == null)
            {
                return;
            }

            var found = settlementRoot.GetComponentsInChildren<SettlementGuideTag>(true);
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

            if (skipButton != null)
            {
                skipButton.onClick.AddListener(OnSkipClicked);
            }

            if (guideToggleButton != null)
            {
                guideToggleButton.onClick.AddListener(OnGuideToggleClicked);
            }
        }

        private void UnregisterRuntimeListeners()
        {
            if (skipButton != null)
            {
                skipButton.onClick.RemoveListener(OnSkipClicked);
            }

            if (guideToggleButton != null)
            {
                guideToggleButton.onClick.RemoveListener(OnGuideToggleClicked);
            }
        }

        private void ResetRound()
        {
            completed = false;
            state = SettlementState.Collect;
            SetGuideVisible(showGuideOnOpen);
            ApplySummaryText();
            ApplyTierVisual();
            SetStatus("收口中...");

            if (skipButtonLabel != null)
            {
                skipButtonLabel.text = "跳过展示";
            }

            if (flowCoroutine != null)
            {
                StopCoroutine(flowCoroutine);
            }

            flowCoroutine = StartCoroutine(FlowRoutine());
        }

        private void ApplySummaryText()
        {
            if (titleText != null)
            {
                var safeClientName = string.IsNullOrWhiteSpace(panelData.ClientDisplayName)
                    ? $"客户{Mathf.Max(1, panelData.ClientId)}"
                    : panelData.ClientDisplayName;
                titleText.text = $"结算反馈 - {safeClientName}";
            }

            if (summaryText != null)
            {
                summaryText.text = $"满意度 {panelData.FinalSatisfaction} | 小费 +{panelData.TipAmount} | 余额 {panelData.TotalMoney}";
            }

            if (detailText != null)
            {
                detailText.text =
                    $"文书:{panelData.DocReviewResult}  QTE错:{panelData.QteErrorCount}  印章:{panelData.StampType}/{panelData.StampTimingResult}  灵魂:{panelData.SoulPercent}%  豆罐:{panelData.BeanSellResult}";
            }

            if (tipText != null)
            {
                tipText.SetText("本次小费：+{0}", panelData.TipAmount);
            }

            if (moneyText != null)
            {
                moneyText.SetText("当前收入：{0}", panelData.TotalMoney);
            }

            if (tierText != null)
            {
                tierText.text = panelData.Tier switch
                {
                    SettlementTier.Good => "评价：优秀",
                    SettlementTier.Bad => "评价：警告",
                    _ => "评价：普通"
                };
            }
        }

        private void ApplyTierVisual()
        {
            if (pulseImage == null)
            {
                return;
            }

            pulseImage.color = panelData.Tier switch
            {
                SettlementTier.Good => new Color(0.17f, 0.62f, 0.27f, 0.28f),
                SettlementTier.Bad => new Color(0.78f, 0.16f, 0.16f, 0.28f),
                _ => new Color(0.64f, 0.56f, 0.18f, 0.24f)
            };
        }

        private IEnumerator FlowRoutine()
        {
            var totalDuration = Mathf.Clamp(panelData.FeedbackDuration, 1.0f, 2.0f);
            var stepDuration = totalDuration / 5f;

            state = SettlementState.Collect;
            SetStatus("Collect：聚合本客户结果。");
            yield return new WaitForSecondsRealtime(stepDuration);

            state = SettlementState.Evaluate;
            SetStatus("Evaluate：计算满意度与失误标签。");
            yield return new WaitForSecondsRealtime(stepDuration);

            state = SettlementState.Reward;
            if (panelData.TipAmount > 0)
            {
                EmitCue("sfx.contract.settle.tip", transform, 1f);
            }
            SetStatus("Reward：结算小费与收入。");
            yield return new WaitForSecondsRealtime(stepDuration);

            state = SettlementState.Narrate;
            EmitCue(panelData.Tier == SettlementTier.Bad ? "sfx.contract.settle.bad" : "sfx.contract.settle.good", transform, 1f);
            EmitCue(panelData.Tier == SettlementTier.Bad ? "vfx.contract.settle.red_pulse" : "vfx.contract.settle.green_pulse", transform, 1f);
            SetStatus("Narrate：反馈结果并准备下一步。");
            yield return new WaitForSecondsRealtime(stepDuration);

            state = SettlementState.RouteNext;
            SetStatus("RouteNext：跳转下一客户或日终节点。");
            yield return new WaitForSecondsRealtime(stepDuration);

            Complete();
        }

        private void OnSkipClicked()
        {
            if (state == SettlementState.Completed)
            {
                return;
            }

            Complete();
        }

        private void Complete()
        {
            if (completed)
            {
                return;
            }

            completed = true;
            state = SettlementState.Completed;
            SetStatus("结算完成。");
            panelData.OnCompleted?.Invoke();
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

        private void SetStatus(string text)
        {
            if (statusText != null)
            {
                statusText.text = text ?? string.Empty;
            }
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
