namespace ITC.Dialogue
{
    public sealed class SignDialoguePlayerTrack
    {
        private readonly SignDialoguePanelFactory panelFactory;
        private SignDialoguePanelInstance currentPanel;

        public SignDialoguePlayerTrack(SignDialoguePanelFactory panelFactory)
        {
            this.panelFactory = panelFactory;
        }

        public void ShowText(string text)
        {
            ReplaceCurrent(panelFactory.CreatePlayer(text, true));
        }

        public SignDialoguePanelInstance ShowOptionsPanel()
        {
            var panel = panelFactory.CreatePlayer(string.Empty, true);
            ReplaceCurrent(panel);
            panel.PrepareForOptions();
            return panel;
        }

        public void HideCurrent()
        {
            if (currentPanel == null)
            {
                return;
            }

            currentPanel.PlayHideAndDestroy();
            currentPanel = null;
        }

        private void ReplaceCurrent(SignDialoguePanelInstance nextPanel)
        {
            if (currentPanel != null && currentPanel != nextPanel)
            {
                currentPanel.PlayHideAndDestroy();
            }

            currentPanel = nextPanel;
        }
    }
}
