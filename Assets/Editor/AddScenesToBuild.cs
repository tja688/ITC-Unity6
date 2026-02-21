using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class AddScenesToBuild
{
    [MenuItem("ITC/Build/Add Dialogue Scenes")]
    public static void Execute()
    {
        string[] scenesToAdd = new[]
        {
            "Assets/Scenes/DialogueScene.unity",
            "Assets/Scenes/SignScene.unity",
            "Assets/Scenes/OffworkScene.unity"
        };

        var currentScenes = EditorBuildSettings.scenes.ToList();
        bool changed = false;

        foreach (var scenePath in scenesToAdd)
        {
            if (!currentScenes.Any(s => s.path == scenePath))
            {
                currentScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                changed = true;
            }
        }

        if (changed)
        {
            EditorBuildSettings.scenes = currentScenes.ToArray();
            Debug.Log("Added scenes to Build Settings.");
        }
        else
        {
            Debug.Log("Scenes already in Build Settings.");
        }
    }
}
