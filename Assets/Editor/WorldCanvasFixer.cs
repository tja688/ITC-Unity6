using UnityEngine;
using UnityEditor;

public class WorldCanvasFixer : EditorWindow
{
    [MenuItem("Tools/Fix Current World Canvas Scale")]
    [MenuItem("Tools/Fix Current World Canvas Scale")]
    public static void FixSelection()
    {
        GameObject canvasObj = GameObject.Find("世界BG_WorldCanvas");
        if (canvasObj == null)
        {
            Debug.LogWarning("Could not find 世界BG_WorldCanvas in scene.");
            return;
        }

        Undo.RegisterFullObjectHierarchyUndo(canvasObj, "Fix World Canvas Scale");

        // 1. Set Canvas Scale to 0.01
        canvasObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        // 2. Adjust all children recursively
        RescaleChildren(canvasObj.transform, 100f);

        Debug.Log("World Canvas fixed! Scale set to 0.01 and children adjusted by 100x.");
    }

    private static void RescaleChildren(Transform parent, float factor)
    {
        foreach (Transform child in parent)
        {
            RectTransform rt = child as RectTransform;
            if (rt != null)
            {
                rt.localPosition *= factor;
                rt.sizeDelta *= factor;
            }
            
            if (child.childCount > 0)
            {
                RescaleChildren(child, factor);
            }
        }
    }
}
