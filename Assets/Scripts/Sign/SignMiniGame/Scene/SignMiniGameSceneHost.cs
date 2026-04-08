using System;
using System.Collections.Generic;
using System.Reflection;
using QFramework;
using UnityEngine;

namespace ITC.SignMiniGame
{
    [DisallowMultipleComponent]
    public sealed class SignMiniGameSceneHost : MonoBehaviour
    {
        public static SignMiniGameSceneHost Instance { get; private set; }

        [Header("Scene Hosted Panels")]
        [SerializeField] private DocumentReviewPanel documentReviewPanel;
        [SerializeField] private RuneTypingPanel runeTypingPanel;
        [SerializeField] private StampPanel stampPanel;
        [SerializeField] private SoulCollectPanel soulCollectPanel;
        [SerializeField] private BeanSellPanel beanSellPanel;
        [SerializeField] private SettlementPanel settlementPanel;
        [SerializeField] private RuneVerifyPanel runeVerifyPanel;

        private readonly HashSet<UIPanel> openedPanels = new();
        private static readonly Dictionary<string, MethodInfo> MethodCache = new(StringComparer.Ordinal);

        private void Awake()
        {
            Instance = this;
            CachePanelReferences();
            HideAllPanelsImmediate();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public bool OpenDocumentReview(DocumentReviewPanelData panelData)
        {
            return TryOpenPanel(documentReviewPanel, panelData);
        }

        public void CloseDocumentReview()
        {
            TryClosePanel(documentReviewPanel);
        }

        public bool OpenRuneTyping(RuneTypingPanelData panelData)
        {
            return TryOpenPanel(runeTypingPanel, panelData);
        }

        public void CloseRuneTyping()
        {
            TryClosePanel(runeTypingPanel);
        }

        public bool OpenStamp(StampPanelData panelData)
        {
            return TryOpenPanel(stampPanel, panelData);
        }

        public void CloseStamp()
        {
            TryClosePanel(stampPanel);
        }

        public bool OpenSoulCollect(SoulCollectPanelData panelData)
        {
            return TryOpenPanel(soulCollectPanel, panelData);
        }

        public void CloseSoulCollect()
        {
            TryClosePanel(soulCollectPanel);
        }

        public bool OpenBeanSell(BeanSellPanelData panelData)
        {
            return TryOpenPanel(beanSellPanel, panelData);
        }

        public void CloseBeanSell()
        {
            TryClosePanel(beanSellPanel);
        }

        public bool OpenSettlement(SettlementPanelData panelData)
        {
            return TryOpenPanel(settlementPanel, panelData);
        }

        public void CloseSettlement()
        {
            TryClosePanel(settlementPanel);
        }

        public bool OpenRuneVerify(RuneVerifyPanelData panelData)
        {
            return TryOpenPanel(runeVerifyPanel, panelData);
        }

        public void CloseRuneVerify()
        {
            TryClosePanel(runeVerifyPanel);
        }

        private void CachePanelReferences()
        {
            documentReviewPanel ??= GetComponentInChildren<DocumentReviewPanel>(true);
            runeTypingPanel ??= GetComponentInChildren<RuneTypingPanel>(true);
            stampPanel ??= GetComponentInChildren<StampPanel>(true);
            soulCollectPanel ??= GetComponentInChildren<SoulCollectPanel>(true);
            beanSellPanel ??= GetComponentInChildren<BeanSellPanel>(true);
            settlementPanel ??= GetComponentInChildren<SettlementPanel>(true);
            runeVerifyPanel ??= GetComponentInChildren<RuneVerifyPanel>(true);
        }

        private void HideAllPanelsImmediate()
        {
            ForceDeactivate(documentReviewPanel);
            ForceDeactivate(runeTypingPanel);
            ForceDeactivate(stampPanel);
            ForceDeactivate(soulCollectPanel);
            ForceDeactivate(beanSellPanel);
            ForceDeactivate(settlementPanel);
            ForceDeactivate(runeVerifyPanel);
        }

        private bool TryOpenPanel<TPanelData>(UIPanel panel, TPanelData panelData)
            where TPanelData : IUIData
        {
            if (panel == null)
            {
                return false;
            }

            if (openedPanels.Contains(panel))
            {
                TryClosePanel(panel);
            }

            panel.gameObject.SetActive(true);
            InvokeLifecycle(panel, "OnOpen", panelData);
            openedPanels.Add(panel);
            return true;
        }

        private void TryClosePanel(UIPanel panel)
        {
            if (panel == null)
            {
                return;
            }

            if (openedPanels.Remove(panel))
            {
                InvokeLifecycle(panel, "OnClose");
            }

            panel.gameObject.SetActive(false);
        }

        private void ForceDeactivate(UIPanel panel)
        {
            if (panel == null)
            {
                return;
            }

            openedPanels.Remove(panel);
            panel.gameObject.SetActive(false);
        }

        private static void InvokeLifecycle(UIPanel panel, string methodName, params object[] args)
        {
            if (panel == null)
            {
                return;
            }

            var key = $"{panel.GetType().FullName}:{methodName}";
            if (!MethodCache.TryGetValue(key, out var method))
            {
                method = panel.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
                MethodCache[key] = method;
            }

            method?.Invoke(panel, args);
        }
    }
}
