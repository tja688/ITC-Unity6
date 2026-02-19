using System.Collections;
using System.Collections.Generic;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ITC.Contracting
{
    public sealed class DocumentReviewPanel : UIPanel, IController
    {
        private enum ReviewState
        {
            Entering,
            Inspecting,
            DecisionPending,
            RejectReasonSelecting,
            Resolving,
            Completed
        }

        [Header("Roots")]
        [SerializeField] private RectTransform panelRoot;
        [SerializeField] private RectTransform reviewRoot;
        [SerializeField] private RectTransform documentRoot;
        [SerializeField] private RectTransform documentZoomTarget;
        [SerializeField] private Button documentBlankButton;
        [SerializeField] private GameObject rejectReasonRoot;

        [Header("Text")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text zoomButtonLabel;
        [SerializeField] private TMP_Text guideToggleLabel;

        [Header("Decision Buttons")]
        [SerializeField] private Button passButton;
        [SerializeField] private Button rejectButton;
        [SerializeField] private Button zoomButton;
        [SerializeField] private Button guideToggleButton;
        [SerializeField] private Button cancelRejectButton;

        [Header("Guide")]
        [SerializeField] private bool showGuideOnOpen = true;

        [Header("Hotspots")]
        [SerializeField] private List<Button> hotspotButtons = new();

        [Header("Reject Reason Buttons")]
        [SerializeField] private Button reasonImageMismatchButton;
        [SerializeField] private Button reasonDateMismatchButton;
        [SerializeField] private Button reasonApplicationMismatchButton;
        [SerializeField] private Button reasonPaperForgeryButton;
        [SerializeField] private Button reasonPaperDamageButton;

        [Header("Tuning")]
        [SerializeField] private float inspectPulseDuration = 0.16f;
        [SerializeField] private float wrongTapShakeDuration = 0.08f;
        [SerializeField] private float resolveFeedbackDuration = 0.45f;

        private readonly Dictionary<Button, UnityAction> hotspotClickListeners = new();
        private readonly Dictionary<Button, UnityAction> rejectReasonClickListeners = new();
        private readonly Dictionary<Button, HotspotHoverExpander> hotspotHoverExpanders = new();
        private readonly HashSet<Button> inspectedHotspots = new();
        private readonly List<DocumentReviewGuideTag> guideTags = new();

        private DocumentReviewPanelData panelData = new();
        private ReviewState state;
        private bool unsafeDecisionArmed;
        private DocumentReviewDecision armedDecision;
        private float armedDecisionTime;
        private bool zoomed;
        private bool guideVisible;
        private Coroutine resolveCoroutine;

        private void Awake()
        {
            CacheReferences();
        }

        protected override void OnInit(IUIData uiData = null)
        {
            panelData = uiData as DocumentReviewPanelData ?? new DocumentReviewPanelData();
            CacheReferences();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            panelData = uiData as DocumentReviewPanelData ?? panelData ?? new DocumentReviewPanelData();
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

            inspectedHotspots.Clear();
            unsafeDecisionArmed = false;
            zoomed = false;
        }

        private void EnsurePanelConfig()
        {
            if (panelData.RuntimeConfig == null)
            {
                panelData.RuntimeConfig = this.GetModel<ContractClientConfigModel>().DocumentReviewRuleConfig;
            }

            if (panelData.ClientConfig == null)
            {
                panelData.ClientConfig =
                    this.GetModel<ContractClientConfigModel>().GetDocumentReviewClientConfig(panelData.ClientId);
            }
        }

        private void CacheReferences()
        {
            panelRoot ??= transform as RectTransform;

            var canvasRoot = transform.Find("Canvas");
            reviewRoot ??= canvasRoot?.Find("ReviewRoot") as RectTransform;
            documentRoot ??= reviewRoot?.Find("DocumentArea") as RectTransform;
            documentZoomTarget ??= documentRoot;
            documentBlankButton ??= documentRoot?.GetComponent<Button>();

            rejectReasonRoot ??= reviewRoot?.Find("RejectReasons")?.gameObject;

            titleText ??= reviewRoot?.Find("HeaderText")?.GetComponent<TMP_Text>();
            hintText ??= reviewRoot?.Find("HintText")?.GetComponent<TMP_Text>();
            statusText ??= reviewRoot?.Find("StatusText")?.GetComponent<TMP_Text>();

            passButton ??= reviewRoot?.Find("Controls/PassButton")?.GetComponent<Button>();
            rejectButton ??= reviewRoot?.Find("Controls/RejectButton")?.GetComponent<Button>();
            zoomButton ??= reviewRoot?.Find("Controls/ZoomButton")?.GetComponent<Button>();
            zoomButtonLabel ??= reviewRoot?.Find("Controls/ZoomButton/Label")?.GetComponent<TMP_Text>();
            guideToggleButton ??= reviewRoot?.Find("GuideToggleButton")?.GetComponent<Button>();
            guideToggleLabel ??= reviewRoot?.Find("GuideToggleButton/Label")?.GetComponent<TMP_Text>();

            cancelRejectButton ??= rejectReasonRoot?.transform.Find("CancelButton")?.GetComponent<Button>();
            reasonImageMismatchButton ??= rejectReasonRoot?.transform.Find("Reason_ImageMismatch")?.GetComponent<Button>();
            reasonDateMismatchButton ??= rejectReasonRoot?.transform.Find("Reason_DateMismatch")?.GetComponent<Button>();
            reasonApplicationMismatchButton ??=
                rejectReasonRoot?.transform.Find("Reason_ApplicationMismatch")?.GetComponent<Button>();
            reasonPaperForgeryButton ??= rejectReasonRoot?.transform.Find("Reason_PaperForgery")?.GetComponent<Button>();
            reasonPaperDamageButton ??= rejectReasonRoot?.transform.Find("Reason_PaperDamage")?.GetComponent<Button>();

            if (hotspotButtons.Count == 0 && documentRoot != null)
            {
                TryAddHotspotByName("Hotspot_Seal");
                TryAddHotspotByName("Hotspot_Ink");
                TryAddHotspotByName("Hotspot_Date");
                TryAddHotspotByName("Hotspot_Content");
            }

            RebuildGuideTags();
        }

        private void TryAddHotspotByName(string hotspotName)
        {
            var button = documentRoot?.Find(hotspotName)?.GetComponent<Button>();
            if (button != null)
            {
                hotspotButtons.Add(button);
            }
        }

        private void RebuildGuideTags()
        {
            guideTags.Clear();
            if (reviewRoot == null)
            {
                return;
            }

            var found = reviewRoot.GetComponentsInChildren<DocumentReviewGuideTag>(true);
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

            if (passButton != null)
            {
                passButton.onClick.AddListener(OnPassClicked);
            }

            if (rejectButton != null)
            {
                rejectButton.onClick.AddListener(OnRejectClicked);
            }

            if (zoomButton != null)
            {
                zoomButton.onClick.AddListener(OnZoomClicked);
            }

            if (guideToggleButton != null)
            {
                guideToggleButton.onClick.AddListener(OnGuideToggleClicked);
            }

            if (documentBlankButton != null)
            {
                documentBlankButton.onClick.AddListener(OnBlankAreaClicked);
            }

            if (cancelRejectButton != null)
            {
                cancelRejectButton.onClick.AddListener(HideRejectReasonSelection);
            }

            RegisterRejectReasonButton(reasonImageMismatchButton, DocumentReviewRejectReason.ImageMismatch);
            RegisterRejectReasonButton(reasonDateMismatchButton, DocumentReviewRejectReason.DateMismatch);
            RegisterRejectReasonButton(reasonApplicationMismatchButton, DocumentReviewRejectReason.ApplicationMismatch);
            RegisterRejectReasonButton(reasonPaperForgeryButton, DocumentReviewRejectReason.PaperForgery);
            RegisterRejectReasonButton(reasonPaperDamageButton, DocumentReviewRejectReason.PaperDamage);

            hotspotClickListeners.Clear();
            hotspotHoverExpanders.Clear();
            foreach (var hotspotButton in hotspotButtons)
            {
                if (hotspotButton == null)
                {
                    continue;
                }

                UnityAction clickAction = () => OnHotspotClicked(hotspotButton);
                hotspotClickListeners[hotspotButton] = clickAction;
                hotspotButton.onClick.AddListener(clickAction);

                var expander = hotspotButton.GetComponent<HotspotHoverExpander>();
                if (expander == null)
                {
                    expander = hotspotButton.gameObject.AddComponent<HotspotHoverExpander>();
                }

                expander.Configure(panelData.RuntimeConfig.HoverExpandPx);
                hotspotHoverExpanders[hotspotButton] = expander;
            }
        }

        private void RegisterRejectReasonButton(Button button, DocumentReviewRejectReason reason)
        {
            if (button == null)
            {
                return;
            }

            UnityAction action = () => OnRejectReasonPicked(reason);
            rejectReasonClickListeners[button] = action;
            button.onClick.AddListener(action);
        }

        private void UnregisterRuntimeListeners()
        {
            if (passButton != null)
            {
                passButton.onClick.RemoveListener(OnPassClicked);
            }

            if (rejectButton != null)
            {
                rejectButton.onClick.RemoveListener(OnRejectClicked);
            }

            if (zoomButton != null)
            {
                zoomButton.onClick.RemoveListener(OnZoomClicked);
            }

            if (guideToggleButton != null)
            {
                guideToggleButton.onClick.RemoveListener(OnGuideToggleClicked);
            }

            if (documentBlankButton != null)
            {
                documentBlankButton.onClick.RemoveListener(OnBlankAreaClicked);
            }

            if (cancelRejectButton != null)
            {
                cancelRejectButton.onClick.RemoveListener(HideRejectReasonSelection);
            }

            foreach (var pair in hotspotClickListeners)
            {
                if (pair.Key != null)
                {
                    pair.Key.onClick.RemoveListener(pair.Value);
                }
            }

            foreach (var pair in rejectReasonClickListeners)
            {
                if (pair.Key != null)
                {
                    pair.Key.onClick.RemoveListener(pair.Value);
                }
            }

            hotspotClickListeners.Clear();
            rejectReasonClickListeners.Clear();
            hotspotHoverExpanders.Clear();
        }

        private void ResetRound()
        {
            state = ReviewState.Entering;
            inspectedHotspots.Clear();
            unsafeDecisionArmed = false;
            armedDecisionTime = 0f;
            zoomed = false;
            SetRejectReasonRootVisible(false);
            ApplyZoom();
            SetInteractable(true);
            RefreshTitleAndHint();
            RefreshHotspotVisuals();
            RefreshStatusText();
            SetGuideVisible(showGuideOnOpen);
            state = ReviewState.Inspecting;
        }

        private void RefreshTitleAndHint()
        {
            if (titleText != null)
            {
                var clientName = panelData.ClientConfig != null
                    ? panelData.ClientConfig.ClientDisplayName
                    : $"客户{panelData.ClientId}";
                titleText.text = $"文书审核 - {clientName}";
            }

            if (hintText == null)
            {
                return;
            }

            if (panelData.TutorialMode && panelData.RuntimeConfig.TutorialForceAllHotspots)
            {
                hintText.text = "教学模式：请先检查全部热点，再提交判定。";
            }
            else
            {
                hintText.text = "请先检查文书，再选择“通过”或“退回”。";
            }
        }

        private void RefreshStatusText()
        {
            if (statusText == null)
            {
                return;
            }

            var current = inspectedHotspots.Count;
            var required = RequiredInspectCount();
            var total = Mathf.Max(1, hotspotButtons.Count);
            statusText.text = $"已检查 {current}/{total}（建议至少 {required} 项）";
        }

        private int RequiredInspectCount()
        {
            return Mathf.Clamp(panelData.RuntimeConfig.MinInspectCount, 1, Mathf.Max(1, hotspotButtons.Count));
        }

        private bool IsTutorialBlocked()
        {
            if (!panelData.TutorialMode || !panelData.RuntimeConfig.TutorialForceAllHotspots)
            {
                return false;
            }

            return inspectedHotspots.Count < hotspotButtons.Count;
        }

        private void OnHotspotClicked(Button hotspotButton)
        {
            if (state is ReviewState.Resolving or ReviewState.Completed || hotspotButton == null)
            {
                return;
            }

            var added = inspectedHotspots.Add(hotspotButton);
            if (added)
            {
                EmitCue("sfx.contract.doc.inspect", hotspotButton.transform, 1f);
                EmitCue("vfx.contract.doc.hotspot_ping", hotspotButton.transform, 1f);
                StartCoroutine(PulseHotspot(hotspotButton.transform as RectTransform));
            }

            unsafeDecisionArmed = false;
            RefreshHotspotVisuals();
            state = ReviewState.DecisionPending;
            RefreshStatusText();
        }

        private void RefreshHotspotVisuals()
        {
            foreach (var hotspot in hotspotButtons)
            {
                if (hotspot == null)
                {
                    continue;
                }

                var image = hotspot.image;
                if (image == null)
                {
                    continue;
                }

                image.color = inspectedHotspots.Contains(hotspot)
                    ? new Color32(84, 214, 120, 175)
                    : new Color32(250, 152, 74, 110);
            }
        }

        private void OnPassClicked()
        {
            if (state is ReviewState.Resolving or ReviewState.Completed)
            {
                return;
            }

            if (!TryValidateSubmissionGate(DocumentReviewDecision.Pass))
            {
                return;
            }

            BeginResolve(DocumentReviewDecision.Pass, DocumentReviewRejectReason.None);
        }

        private void OnRejectClicked()
        {
            if (state is ReviewState.Resolving or ReviewState.Completed)
            {
                return;
            }

            if (!TryValidateSubmissionGate(DocumentReviewDecision.Reject))
            {
                return;
            }

            ShowRejectReasonSelection();
        }

        private bool TryValidateSubmissionGate(DocumentReviewDecision decision)
        {
            if (IsTutorialBlocked())
            {
                if (statusText != null)
                {
                    statusText.text = "教学模式要求：必须完成全部热点检查。";
                }

                EmitCue("vfx.contract.doc.wrong_shake", transform, 0.8f);
                return false;
            }

            if (inspectedHotspots.Count >= RequiredInspectCount())
            {
                unsafeDecisionArmed = false;
                return true;
            }

            return ArmUnsafeDecision(decision);
        }

        private bool ArmUnsafeDecision(DocumentReviewDecision decision)
        {
            var now = Time.unscaledTime;
            var minInterval = Mathf.Max(0.05f, panelData.RuntimeConfig.SubmitDebounceMs * 0.001f);

            if (!unsafeDecisionArmed || armedDecision != decision)
            {
                unsafeDecisionArmed = true;
                armedDecision = decision;
                armedDecisionTime = now;
                if (statusText != null)
                {
                    statusText.text = "检查项不足。再次点击可确认本次判定。";
                }

                return false;
            }

            if (now - armedDecisionTime < minInterval)
            {
                if (statusText != null)
                {
                    statusText.text = "请稍候后再次点击确认。";
                }

                return false;
            }

            unsafeDecisionArmed = false;
            return true;
        }

        private void ShowRejectReasonSelection()
        {
            state = ReviewState.RejectReasonSelecting;
            SetRejectReasonRootVisible(true);
            if (statusText != null)
            {
                statusText.text = "请选择一个退回理由。";
            }
        }

        private void HideRejectReasonSelection()
        {
            if (state != ReviewState.RejectReasonSelecting)
            {
                return;
            }

            SetRejectReasonRootVisible(false);
            state = ReviewState.DecisionPending;
            RefreshStatusText();
        }

        private void SetRejectReasonRootVisible(bool visible)
        {
            if (rejectReasonRoot == null)
            {
                return;
            }

            rejectReasonRoot.SetActive(visible);
        }

        private void OnRejectReasonPicked(DocumentReviewRejectReason reason)
        {
            if (state != ReviewState.RejectReasonSelecting)
            {
                return;
            }

            SetRejectReasonRootVisible(false);
            BeginResolve(DocumentReviewDecision.Reject, reason);
        }

        private void BeginResolve(DocumentReviewDecision decision, DocumentReviewRejectReason reason)
        {
            if (resolveCoroutine != null)
            {
                StopCoroutine(resolveCoroutine);
            }

            resolveCoroutine = StartCoroutine(ResolveRoutine(decision, reason));
        }

        private IEnumerator ResolveRoutine(DocumentReviewDecision decision, DocumentReviewRejectReason reason)
        {
            state = ReviewState.Resolving;
            SetInteractable(false);

            if (decision == DocumentReviewDecision.Pass)
            {
                EmitCue("sfx.contract.doc.pass", transform, 1f);
            }
            else
            {
                EmitCue("sfx.contract.doc.reject", transform, 1f);
            }

            if (statusText != null)
            {
                statusText.text = "正在结算判定结果...";
            }

            var duration = Mathf.Max(0.05f, resolveFeedbackDuration);
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            state = ReviewState.Completed;

            var result = new DocumentReviewResultPayload
            {
                ClientId = Mathf.Max(1, panelData.ClientId),
                FinalAction = decision,
                RejectReason = reason,
                InspectedHotspotCount = inspectedHotspots.Count,
                TutorialMode = panelData.TutorialMode,
                ForceConfirmed = inspectedHotspots.Count < RequiredInspectCount(),
                WasFallback = false
            };

            panelData.OnCompleted?.Invoke(result);
        }

        private void OnBlankAreaClicked()
        {
            if (state is ReviewState.Resolving or ReviewState.Completed)
            {
                return;
            }

            EmitCue("sfx.contract.doc.hover", documentRoot != null ? documentRoot : transform, 0.6f);
            EmitCue("vfx.contract.doc.wrong_shake", documentRoot != null ? documentRoot : transform, 0.8f);
            StartCoroutine(ShakeDocumentBriefly());
        }

        private void OnZoomClicked()
        {
            zoomed = !zoomed;
            ApplyZoom();
            if (zoomButtonLabel != null)
            {
                zoomButtonLabel.text = zoomed ? "缩放 x1.0" : "缩放 x1.5";
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

        private void ApplyZoom()
        {
            if (documentZoomTarget == null)
            {
                return;
            }

            var zoomRange = panelData.RuntimeConfig.ZoomRange;
            var minZoom = Mathf.Clamp(zoomRange.x, 0.75f, 2f);
            var maxZoom = Mathf.Clamp(zoomRange.y, minZoom, 3f);
            var targetScale = zoomed ? maxZoom : minZoom;
            documentZoomTarget.localScale = Vector3.one * targetScale;
        }

        private void SetInteractable(bool interactable)
        {
            SetButtonInteractable(passButton, interactable);
            SetButtonInteractable(rejectButton, interactable);
            SetButtonInteractable(zoomButton, interactable);
            SetButtonInteractable(guideToggleButton, interactable);
            SetButtonInteractable(cancelRejectButton, interactable);

            foreach (var hotspot in hotspotButtons)
            {
                SetButtonInteractable(hotspot, interactable);
            }

            foreach (var button in rejectReasonClickListeners.Keys)
            {
                SetButtonInteractable(button, interactable);
            }
        }

        private static void SetButtonInteractable(Button button, bool interactable)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
        }

        private IEnumerator PulseHotspot(RectTransform target)
        {
            if (target == null)
            {
                yield break;
            }

            var duration = Mathf.Max(0.05f, inspectPulseDuration);
            var half = duration * 0.5f;
            var originalScale = target.localScale;
            var boostedScale = originalScale * 1.08f;

            var elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / half);
                target.localScale = Vector3.Lerp(originalScale, boostedScale, t);
                yield return null;
            }

            elapsed = 0f;
            while (elapsed < half)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / half);
                target.localScale = Vector3.Lerp(boostedScale, originalScale, t);
                yield return null;
            }

            target.localScale = originalScale;
        }

        private IEnumerator ShakeDocumentBriefly()
        {
            if (documentRoot == null)
            {
                yield break;
            }

            var basePos = documentRoot.anchoredPosition;
            var duration = Mathf.Max(0.03f, wrongTapShakeDuration);
            var elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var strength = 1f - Mathf.Clamp01(elapsed / duration);
                var offset = Mathf.Sin(elapsed * 80f) * 5f * strength;
                documentRoot.anchoredPosition = basePos + new Vector2(offset, 0f);
                yield return null;
            }

            documentRoot.anchoredPosition = basePos;
        }

        private void EmitCue(string cueId, Transform anchor, float intensity)
        {
            panelData.OnFxCue?.Invoke(cueId, anchor, intensity);
        }

        public IArchitecture GetArchitecture()
        {
            return MainMenuApp.Interface;
        }

        [DisallowMultipleComponent]
        private sealed class HotspotHoverExpander : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
        {
            private RectTransform cachedRectTransform;
            private Vector2 baseSize;
            private bool configured;
            private float expandPixels = 10f;

            public void Configure(int hoverExpandPx)
            {
                expandPixels = Mathf.Clamp(hoverExpandPx, 8f, 12f);
                if (cachedRectTransform == null)
                {
                    cachedRectTransform = transform as RectTransform;
                }

                if (cachedRectTransform != null)
                {
                    baseSize = cachedRectTransform.sizeDelta;
                    configured = true;
                }
            }

            public void OnPointerEnter(PointerEventData eventData)
            {
                if (!configured || cachedRectTransform == null)
                {
                    return;
                }

                cachedRectTransform.sizeDelta = baseSize + new Vector2(expandPixels * 2f, expandPixels * 2f);
            }

            public void OnPointerExit(PointerEventData eventData)
            {
                if (!configured || cachedRectTransform == null)
                {
                    return;
                }

                cachedRectTransform.sizeDelta = baseSize;
            }
        }
    }
}
