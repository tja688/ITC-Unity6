using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class FixOptionPrefab
{
    [MenuItem("Tools/Fix Option Item Prefab")]
    public static void Run()
    {
        string prefabPath = "Assets/Prefabs/UI/OptionButton.prefab";
        using (var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var root = editingScope.prefabContentsRoot;

            // Add an invisible Image to serve as the hit target and the Target Graphic for the Selectable
            var image = root.GetComponent<Image>();
            if (image == null)
            {
                image = root.AddComponent<Image>();
            }
            image.color = new Color(1, 1, 1, 0); // completely transparent
            image.raycastTarget = true;

            var optItem = root.GetComponent<OptionItem>();
            optItem.targetGraphic = image; // Hook up the Target Graphic to mute the warning and enable color transitions!

            // Ensure no LayoutElement restricts it too small
            var le = root.GetComponent<LayoutElement>();
            if (le == null)
            {
                le = root.AddComponent<LayoutElement>();
            }
            le.minHeight = 50f;

            // Fix text wrapping if needed, but horizontal wrapping shouldn't be vertical unless width is tiny. 
            // In cases of zero width, let's allow it to not wrap, or wrap if we want.
            var tmp = root.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.enableWordWrapping = false; // Disable word wrapping to prevent vertical stacking if width gets weirdly squished
                tmp.overflowMode = TextOverflowModes.Overflow;
            }

            // Also check the OptionsPanel in the main DialogueSystem prefab to ensure layout is safe
            string mainPrefabPath = "Assets/Prefabs/UI/ITC DialogueSystem.prefab";
            using (var mainEditingScope = new PrefabUtility.EditPrefabContentsScope(mainPrefabPath))
            {
                var mainRoot = mainEditingScope.prefabContentsRoot;
                var optionsPanel = mainRoot.transform.Find("DialogueCanvas/DialoguePanel/OptionsPanel");
                if (optionsPanel != null)
                {
                    var vlg = optionsPanel.GetComponent<VerticalLayoutGroup>();
                    if (vlg != null)
                    {
                        // Some layout groups squish child rects if childControlWidth/Height is true and sizes are not well defined
                        vlg.childControlWidth = true;
                        vlg.childControlHeight = false;
                        vlg.childForceExpandHeight = false;
                        vlg.childForceExpandWidth = false;
                        vlg.spacing = 20;
                    }
                }
            }

            Debug.Log("Option item and panel layout fixed!");
        }
    }
}
