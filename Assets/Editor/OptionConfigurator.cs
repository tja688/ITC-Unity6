using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class OptionConfigurator
{
    [MenuItem("Tools/Configure Dialogue Options")]
    public static void Configure()
    {
        string prefabPath = "Assets/Prefabs/UI/ITC DialogueSystem.prefab";
        using (var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
        {
            var prefabRoot = editingScope.prefabContentsRoot;

            var dialoguePanel = prefabRoot.transform.Find("DialogueCanvas/DialoguePanel");
            if (dialoguePanel == null)
            {
                Debug.LogError("Could not find DialogueCanvas/DialoguePanel in prefab.");
                return;
            }

            var runnerPath = prefabRoot.transform.Find("DialogueRunner");
            if (runnerPath == null)
            {
                Debug.LogError("Could not find DialogueRunner.");
                return;
            }
            var runner = runnerPath.GetComponent<DialogueRunner>();

            var existingOptions = dialoguePanel.Find("OptionsPanel");
            if (existingOptions != null)
            {
                Object.DestroyImmediate(existingOptions.gameObject);
            }

            // Create Options Panel
            var optionsPanel = new GameObject("OptionsPanel");
            optionsPanel.transform.SetParent(dialoguePanel, false);

            var rt = optionsPanel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.3f, 0.1f);
            rt.anchorMax = new Vector2(0.7f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.0f);
            rt.anchoredPosition = new Vector2(0, 0);
            rt.sizeDelta = new Vector2(0, 0);

            var cg = optionsPanel.AddComponent<CanvasGroup>();
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            var vlg = optionsPanel.AddComponent<VerticalLayoutGroup>();
            vlg.childAlignment = TextAnchor.LowerCenter;
            vlg.childControlHeight = true;
            vlg.childControlWidth = true;
            vlg.childForceExpandHeight = false;
            vlg.childForceExpandWidth = false;
            vlg.spacing = 15;

            var fontAssetToUse = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>(true)?.font;

            // Create OptionButton prefab inside OptionsPanel temporarily 
            var btnObj = new GameObject("OptionItemPrefab", typeof(RectTransform));
            var btnRt = btnObj.GetComponent<RectTransform>();
            btnRt.sizeDelta = new Vector2(600, 50);

            var textObj = new GameObject("Text", typeof(RectTransform));
            textObj.transform.SetParent(btnObj.transform, false);
            var textRt = textObj.GetComponent<RectTransform>();
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.offsetMin = Vector2.zero;
            textRt.offsetMax = Vector2.zero;

            var tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = "Try to run away";
            tmp.fontSize = 28;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            if (fontAssetToUse != null) tmp.font = fontAssetToUse;
            tmp.fontStyle = FontStyles.Bold;
            // outline maybe? Or just keep it simple.

            // Add OptionItem instead of Button! 
            // OptionItem IS a Selectable already!
            var optView = btnObj.AddComponent<OptionItem>();
            var soBtn = new SerializedObject(optView);
            soBtn.Update();
            soBtn.FindProperty("text").objectReferenceValue = tmp;

            // Set Appearance colors
            var normalProp = soBtn.FindProperty("normal");
            normalProp.FindPropertyRelative("colour").colorValue = new Color(1f, 1f, 1f, 0.9f);
            var selectedProp = soBtn.FindProperty("selected");
            selectedProp.FindPropertyRelative("colour").colorValue = new Color(1f, 0.82f, 0.35f, 1f); // yellowish/golden
            var disabledProp = soBtn.FindProperty("disabled");
            disabledProp.FindPropertyRelative("colour").colorValue = new Color(0.5f, 0.5f, 0.5f, 0.5f);

            soBtn.ApplyModifiedPropertiesWithoutUndo();

            // Save Prefab
            string btnPrefabPath = "Assets/Prefabs/UI/OptionButton.prefab";
            var optionPrefab = PrefabUtility.SaveAsPrefabAsset(btnObj, btnPrefabPath);
            Object.DestroyImmediate(btnObj);

            // Configure OptionsPresenter
            var optListView = optionsPanel.AddComponent<OptionsPresenter>();
            var soListView = new SerializedObject(optListView);
            soListView.Update();
            soListView.FindProperty("optionViewPrefab").objectReferenceValue = optionPrefab.GetComponent<OptionItem>();
            soListView.FindProperty("canvasGroup").objectReferenceValue = cg;
            soListView.FindProperty("useFadeEffect").boolValue = true;
            soListView.FindProperty("fadeUpDuration").floatValue = 0.25f;
            soListView.FindProperty("fadeDownDuration").floatValue = 0.2f;
            soListView.ApplyModifiedPropertiesWithoutUndo();

            // Add OptionsPresenter to DialogueRunner's dialoguePresenters
            if (runner != null)
            {
                var soRunner = new SerializedObject(runner);
                soRunner.Update();
                var viewsProp = soRunner.FindProperty("dialoguePresenters");

                bool found = false;
                for (int i = 0; i < viewsProp.arraySize; i++)
                {
                    if (viewsProp.GetArrayElementAtIndex(i).objectReferenceValue == optListView)
                    {
                        found = true; break;
                    }
                }
                if (!found)
                {
                    viewsProp.arraySize++;
                    viewsProp.GetArrayElementAtIndex(viewsProp.arraySize - 1).objectReferenceValue = optListView;
                }
                soRunner.ApplyModifiedPropertiesWithoutUndo();
            }

            Debug.Log("Option Dialogue successfully configured!");
        }
    }
}
