using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace ITC.Dialogue.Testing
{
    [RequireComponent(typeof(Button))]
    public class YarnDialogueFastForward : MonoBehaviour
    {
        [Header("References")]
        [Tooltip("The DialogueRunner to control. If empty, will FindObjectOfType.")]
        public DialogueRunner dialogueRunner;

        [Header("Yarn Test Catalog")]
        [Tooltip("The auto-maintained catalog of Yarn nodes.")]
        public YarnTestCatalog testCatalog;

        private List<string> jumpableNodes = new List<string>();
        private int currentIndex = -1;

        private void Start()
        {
            if (dialogueRunner == null)
            {
                dialogueRunner = Object.FindFirstObjectByType<DialogueRunner>();
            }

            var btn = GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(OnFastForwardClicked);
            }

            if (testCatalog != null)
            {
                jumpableNodes = testCatalog.GetAllJumpableNodes();
            }
        }

        private void OnFastForwardClicked()
        {
            if (dialogueRunner == null)
            {
                Debug.LogError("[YarnDialogueFastForward] DialogueRunner is missing.");
                return;
            }

            // Lazy load if not initialized
            if (jumpableNodes == null || jumpableNodes.Count == 0)
            {
                if (testCatalog != null)
                {
                    jumpableNodes = testCatalog.GetAllJumpableNodes();
                }
            }

            if (jumpableNodes == null || jumpableNodes.Count == 0)
            {
                Debug.LogWarning("[YarnDialogueFastForward] No jumpable nodes configured or Catalog is missing.");
                return;
            }

            if (dialogueRunner.IsDialogueRunning)
            {
                dialogueRunner.Stop();
            }

            currentIndex = (currentIndex + 1) % jumpableNodes.Count;
            string targetNode = jumpableNodes[currentIndex];

            Debug.Log($"[YarnDialogueFastForward] Fast-Forwarding to Yarn Node: {targetNode}");
            dialogueRunner.StartDialogue(targetNode);
        }
    }
}
