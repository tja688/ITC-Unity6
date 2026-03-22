using System;
using System.Collections.Generic;
using UnityEngine;

namespace ITC.Dialogue
{
    public sealed class SignDialogueNpcTrack
    {
        private readonly SignDialoguePanelFactory panelFactory;
        private readonly List<SignDialoguePanelInstance> historyPanels = new();

        private SignDialoguePanelInstance currentFrontPanel;
        private SignDialoguePanelInstance currentVisibleHistoryPanel;
        private RectTransform historyBrowseRect;
        private int currentHistoryIndex = -1;
        private string currentNpcId = string.Empty;
        private bool hasPinnedNpcCycle;

        public SignDialogueNpcTrack(SignDialoguePanelFactory panelFactory)
        {
            this.panelFactory = panelFactory;
        }

        public bool HasHistory => historyPanels.Count > 0;
        public RectTransform HistoryBrowseRect => historyBrowseRect;
        public int DefaultHistoryIndex => historyPanels.Count - 1;

        public void SetHistoryBrowseRect(RectTransform browseRect)
        {
            historyBrowseRect = browseRect;
        }

        public void ResetForNpc(string npcId)
        {
            hasPinnedNpcCycle = !string.IsNullOrWhiteSpace(npcId);
            currentNpcId = hasPinnedNpcCycle ? npcId.Trim() : string.Empty;
            ClearAllPanels();
        }

        public void HideAll()
        {
            hasPinnedNpcCycle = false;
            currentNpcId = string.Empty;
            ClearAllPanels();
        }

        public void PresentNpcLine(string speaker, string text)
        {
            var npcId = ResolveNpcId(speaker);
            if (string.IsNullOrWhiteSpace(npcId))
            {
                npcId = "NPC";
            }

            if (!hasPinnedNpcCycle &&
                !string.IsNullOrWhiteSpace(currentNpcId) &&
                !string.Equals(currentNpcId, npcId, StringComparison.OrdinalIgnoreCase))
            {
                ResetForAutoDetectedNpc(npcId);
            }
            else if (string.IsNullOrWhiteSpace(currentNpcId))
            {
                currentNpcId = npcId;
            }

            var previousFrontText = currentFrontPanel != null ? currentFrontPanel.CurrentText : null;

            if (currentFrontPanel != null)
            {
                currentFrontPanel.PlayHideAndDestroy();
                currentFrontPanel = null;
            }

            if (!string.IsNullOrWhiteSpace(previousFrontText))
            {
                var historyPanel = panelFactory.CreateNpcBack(previousFrontText, false);
                historyPanels.Add(historyPanel);
            }

            currentFrontPanel = panelFactory.CreateNpcFront(text);
            ShowHistoryIndex(DefaultHistoryIndex, true);
        }

        public void ScrollHistory(int delta)
        {
            if (!HasHistory)
            {
                return;
            }

            var baseIndex = currentHistoryIndex >= 0 ? currentHistoryIndex : DefaultHistoryIndex;
            var nextIndex = Mathf.Clamp(baseIndex + delta, 0, DefaultHistoryIndex);
            ShowHistoryIndex(nextIndex, true);
        }

        public void ResetVisibleHistoryToDefault(bool animate)
        {
            ShowHistoryIndex(DefaultHistoryIndex, animate);
        }

        private void ShowHistoryIndex(int index, bool animate)
        {
            if (!HasHistory || index < 0 || index >= historyPanels.Count)
            {
                if (currentVisibleHistoryPanel != null)
                {
                    currentVisibleHistoryPanel.HideRetained();
                    currentVisibleHistoryPanel = null;
                }

                currentHistoryIndex = -1;
                return;
            }

            var nextPanel = historyPanels[index];
            if (currentVisibleHistoryPanel == nextPanel)
            {
                return;
            }

            if (currentVisibleHistoryPanel != null)
            {
                currentVisibleHistoryPanel.HideRetained();
            }

            currentVisibleHistoryPanel = nextPanel;
            currentHistoryIndex = index;
            currentVisibleHistoryPanel.ShowRetained(animate);
        }

        private string ResolveNpcId(string speaker)
        {
            if (hasPinnedNpcCycle && !string.IsNullOrWhiteSpace(currentNpcId))
            {
                return currentNpcId;
            }

            return string.IsNullOrWhiteSpace(speaker)
                ? currentNpcId
                : speaker.Trim();
        }

        private void ResetForAutoDetectedNpc(string npcId)
        {
            hasPinnedNpcCycle = false;
            currentNpcId = string.IsNullOrWhiteSpace(npcId) ? string.Empty : npcId.Trim();
            ClearAllPanels();
        }

        private void ClearAllPanels()
        {
            if (currentFrontPanel != null)
            {
                currentFrontPanel.DestroyImmediateSafe();
                currentFrontPanel = null;
            }

            if (currentVisibleHistoryPanel != null)
            {
                currentVisibleHistoryPanel.HideRetained();
                currentVisibleHistoryPanel = null;
            }

            foreach (var panel in historyPanels)
            {
                if (panel != null)
                {
                    panel.DestroyImmediateSafe();
                }
            }

            historyPanels.Clear();
            currentHistoryIndex = -1;
        }
    }
}
