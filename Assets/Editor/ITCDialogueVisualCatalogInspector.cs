using ITC.Dialogue;
using UnityEditor;
using UnityEngine;

namespace ITC.Editor
{
    [CustomEditor(typeof(DialogueVisualCatalog))]
    public sealed class ITCDialogueVisualCatalogInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "Use the planner editor window for drag-and-drop authoring and workflow buttons. " +
                "All raw fields are still shown below for full manual control.",
                MessageType.Info);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Planner Editor", GUILayout.Height(24f)))
            {
                ITCDialogueVisualCatalogEditorWindow.OpenWindow();
            }

            if (GUILayout.Button("Validate Catalog", GUILayout.Height(24f)))
            {
                ITCDialogueVisualCatalogTools.ValidateVisualCatalogMenu();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4f);
            DrawDefaultInspector();
        }
    }
}
