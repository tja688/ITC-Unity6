using UnityEngine;

namespace ITC.Dialogue
{
    public sealed class SignDialoguePanelFactory
    {
        private readonly SignDialoguePanelInstance npcFrontTemplate;
        private readonly SignDialoguePanelInstance npcBackTemplate;
        private readonly SignDialoguePanelInstance playerTemplate;

        public SignDialoguePanelFactory(
            SignDialoguePanelInstance npcFrontTemplate,
            SignDialoguePanelInstance npcBackTemplate,
            SignDialoguePanelInstance playerTemplate)
        {
            this.npcFrontTemplate = npcFrontTemplate;
            this.npcBackTemplate = npcBackTemplate;
            this.playerTemplate = playerTemplate;
        }

        public SignDialoguePanelInstance CreateNpcFront(string text)
        {
            return CreatePanel(npcFrontTemplate, text, true);
        }

        public SignDialoguePanelInstance CreateNpcBack(string text, bool showImmediately)
        {
            return CreatePanel(npcBackTemplate, text, showImmediately);
        }

        public SignDialoguePanelInstance CreatePlayer(string text, bool showImmediately)
        {
            return CreatePanel(playerTemplate, text, showImmediately);
        }

        private static SignDialoguePanelInstance CreatePanel(
            SignDialoguePanelInstance template,
            string text,
            bool showImmediately)
        {
            var clone = Object.Instantiate(template.gameObject, template.transform.parent, false);
            clone.name = $"{template.gameObject.name}_Runtime";
            clone.SetActive(false);

            var panel = clone.GetComponent<SignDialoguePanelInstance>();
            if (panel == null)
            {
                panel = clone.AddComponent<SignDialoguePanelInstance>();
            }

            panel.ResolveReferences();
            panel.ConfigureAnimationsForManualControl();
            panel.PrepareForText(text);

            if (showImmediately)
            {
                clone.SetActive(true);
                panel.PlayShow();
            }

            return panel;
        }
    }
}
