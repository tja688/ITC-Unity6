using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class SpriteToUGUIConverter : EditorWindow
{
    [MenuItem("GameObject/Convert to World Canvas", false, 10)]
    [MenuItem("GameObject/Convert to World Canvas", false, 10)]
    [MenuItem("GameObject/Convert to World Canvas", false, 10)]
    public static void ConvertSelectionToWorldCanvas()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            Debug.LogWarning("Please select a parent GameObject containing sprites.");
            return;
        }

        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("Convert Sprite Hierarchy to World Canvas");

        // 1. Create the root Canvas
        GameObject canvasObj = new GameObject(selected.name + "_WorldCanvas");
        Undo.RegisterCreatedObjectUndo(canvasObj, "Create World Canvas");
        
        // The Canvas represents the spatial container (the selected object)
        canvasObj.transform.position = selected.transform.position;
        canvasObj.transform.rotation = selected.transform.rotation;
        canvasObj.transform.localScale = selected.transform.localScale;

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. Convert all children of the selected object
        ConvertRecursive(selected.transform, canvasObj.transform);

        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        
        Selection.activeGameObject = canvasObj;
        Debug.Log($"Successfully converted {selected.name} children to UGUI World Canvas.");
    }

    private static void ConvertRecursive(Transform source, Transform parentInCanvas)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in source) children.Add(child);

        children.Sort((a, b) => {
            SpriteRenderer srA = a.GetComponent<SpriteRenderer>();
            SpriteRenderer srB = b.GetComponent<SpriteRenderer>();
            int orderA = srA != null ? srA.sortingOrder : 0;
            int orderB = srB != null ? srB.sortingOrder : 0;
            return orderA.CompareTo(orderB);
        });

        foreach (Transform child in children)
        {
            GameObject newUIObj = CreateUGUIObject(child.gameObject, parentInCanvas);
            
            if (child.childCount > 0)
            {
                ConvertRecursive(child, newUIObj.transform);
            }
        }
    }


    private static GameObject CreateUGUIObject(GameObject source, Transform parent)
    {
        GameObject newObj = new GameObject(source.name);
        Undo.RegisterCreatedObjectUndo(newObj, "Create UGUI Object");
        
        RectTransform rectTransform = newObj.AddComponent<RectTransform>();
        newObj.transform.SetParent(parent);
        
        // Local mapping relative to parent (which matches source's parent structure)
        rectTransform.localPosition = source.transform.localPosition;
        rectTransform.localRotation = source.transform.localRotation;
        rectTransform.localScale = source.transform.localScale;

        SpriteRenderer sr = source.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            Image img = newObj.AddComponent<Image>();
            img.sprite = sr.sprite;
            img.color = sr.color;
            img.raycastTarget = false;

            float width = sr.sprite.rect.width / sr.sprite.pixelsPerUnit;
            float height = sr.sprite.rect.height / sr.sprite.pixelsPerUnit;
            rectTransform.sizeDelta = new Vector2(width, height);
            
            Vector2 pivot = sr.sprite.pivot;
            pivot.x /= sr.sprite.rect.width;
            pivot.y /= sr.sprite.rect.height;
            rectTransform.pivot = pivot;
            
            rectTransform.localPosition = source.transform.localPosition;

            // Handle flip
            Vector3 localScale = rectTransform.localScale;
            if (sr.flipX) localScale.x *= -1;
            if (sr.flipY) localScale.y *= -1;
            rectTransform.localScale = localScale;
        }
        else
        {
            rectTransform.sizeDelta = Vector2.zero;
        }

        return newObj;
    }
}
