using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;

public class SpriteToUGUIConverter : EditorWindow
{
    [MenuItem("GameObject/Convert to World Canvas", false, 10)]
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
        
        canvasObj.transform.position = selected.transform.position;
        canvasObj.transform.rotation = selected.transform.rotation;
        
        // Standard World Space Canvas Scale: 0.01 (1 unit = 100 pixels)
        canvasObj.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.referencePixelsPerUnit = 100;
        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. Convert all children
        ConvertRecursive(selected.transform, canvasObj.transform);

        Undo.CollapseUndoOperations(Undo.GetCurrentGroup());
        
        Selection.activeGameObject = canvasObj;
        Debug.Log($"Successfully converted {selected.name} to UGUI World Canvas (Standard 0.01 Scale).");
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
        
        // Factors for scale adjustment: since canvas is 0.01, local units are 100x pixel units
        float unitToPixel = 100f;

        // Local mapping relative to parent
        rectTransform.localPosition = source.transform.localPosition * unitToPixel;
        rectTransform.localRotation = source.transform.localRotation;
        rectTransform.localScale = source.transform.localScale;

        SpriteRenderer sr = source.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
        {
            Image img = newObj.AddComponent<Image>();
            img.sprite = sr.sprite;
            img.color = sr.color;
            img.raycastTarget = false;

            // Size in pixels
            float width = sr.sprite.rect.width;
            float height = sr.sprite.rect.height;
            rectTransform.sizeDelta = new Vector2(width, height);
            
            Vector2 pivot = sr.sprite.pivot;
            pivot.x /= sr.sprite.rect.width;
            pivot.y /= sr.sprite.rect.height;
            rectTransform.pivot = pivot;
            
            // Recalculate localPosition because pivot change on RectTransform might shift it
            rectTransform.localPosition = source.transform.localPosition * unitToPixel;

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
