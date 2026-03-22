using System.Collections.Generic;
using Spine.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SignUiSpineWrapperPatcher
{
    private const string MenuPath = "Tools/ITC/SignMiniGame/Patch SignUI Spine Wrappers";
    private const string ValidateMenuPath = "Tools/ITC/SignMiniGame/Validate SignUI Wrapper Stability";
    private const string TargetRootPath = "SignUI/签约场景元素";
    private const string WrapperSuffix = "_Ctrl";

    [MenuItem(MenuPath)]
    public static void Patch()
    {
        var root = GameObject.Find(TargetRootPath)?.transform as RectTransform;
        if (!root)
        {
            Debug.LogError($"[SignUiSpineWrapperPatcher] Target root not found: {TargetRootPath}");
            return;
        }

        var targets = CollectTargets(root);
        if (targets.Count == 0)
        {
            Debug.LogWarning("[SignUiSpineWrapperPatcher] No eligible SkeletonGraphic nodes found.");
            return;
        }

        Undo.IncrementCurrentGroup();
        var undoGroup = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Patch SignUI Spine Wrappers");

        var patchedCount = 0;
        var skippedCount = 0;

        foreach (var target in targets)
        {
            if (target.parent is not RectTransform parent)
            {
                skippedCount++;
                Debug.LogWarning($"[SignUiSpineWrapperPatcher] Skip '{target.name}': parent is not RectTransform.");
                continue;
            }

            if (target.anchorMin != target.anchorMax)
            {
                skippedCount++;
                Debug.LogWarning(
                    $"[SignUiSpineWrapperPatcher] Skip '{target.name}': stretch anchors not supported safely.");
                continue;
            }

            var localCenter = GetVisualCenterLocal(target);
            var centerOffset = GetCenterOffsetInParentSpace(target, localCenter);

            if (parent.name.EndsWith(WrapperSuffix))
            {
                Undo.RecordObject(parent, "Recenter Existing Spine Wrapper");
                Undo.RecordObject(target, "Recenter Existing Spine Target");

                parent.anchoredPosition3D += target.anchoredPosition3D + centerOffset;
                target.anchoredPosition3D = -centerOffset;
                patchedCount++;
                continue;
            }

            var wrapper = CreateWrapper(parent, target);

            wrapper.anchoredPosition3D = target.anchoredPosition3D + centerOffset;

            Undo.SetTransformParent(target, wrapper, "Reparent Spine Target");
            target.anchoredPosition3D = -centerOffset;

            patchedCount++;
        }

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        Debug.Log(
            $"[SignUiSpineWrapperPatcher] Done. patched={patchedCount}, skipped={skippedCount}, scene='{SceneManager.GetActiveScene().name}'.");
    }

    private static List<RectTransform> CollectTargets(RectTransform root)
    {
        var allRects = root.GetComponentsInChildren<RectTransform>(true);
        var result = new List<RectTransform>(allRects.Length);

        foreach (var rect in allRects)
        {
            if (!rect || rect == root)
            {
                continue;
            }

            if (!rect.GetComponent<SkeletonGraphic>())
            {
                continue;
            }

            if (rect.name.EndsWith(WrapperSuffix))
            {
                continue;
            }

            result.Add(rect);
        }

        return result;
    }

    private static RectTransform CreateWrapper(RectTransform parent, RectTransform target)
    {
        var wrapperObject = new GameObject(target.name + WrapperSuffix, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(wrapperObject, "Create Spine Wrapper");

        var wrapper = wrapperObject.GetComponent<RectTransform>();
        wrapper.SetParent(parent, false);
        wrapper.SetSiblingIndex(target.GetSiblingIndex());

        wrapper.anchorMin = target.anchorMin;
        wrapper.anchorMax = target.anchorMax;
        wrapper.pivot = new Vector2(0.5f, 0.5f);
        wrapper.sizeDelta = Vector2.zero;
        wrapper.localRotation = Quaternion.identity;
        wrapper.localScale = Vector3.one;

        return wrapper;
    }

    private static Vector3 GetCenterOffsetInParentSpace(RectTransform rectTransform, Vector3 localCenterOffset)
    {
        var scaledOffset = Vector3.Scale(localCenterOffset, rectTransform.localScale);
        return rectTransform.localRotation * scaledOffset;
    }

    private static Vector3 GetVisualCenterLocal(RectTransform rectTransform)
    {
        if (TryGetVisualCenterLocal(rectTransform, out var center))
        {
            return center;
        }

        return new Vector3(
            (0.5f - rectTransform.pivot.x) * rectTransform.rect.width,
            (0.5f - rectTransform.pivot.y) * rectTransform.rect.height,
            0f);
    }

    private static bool TryGetVisualCenterLocal(RectTransform rectTransform, out Vector3 center)
    {
        center = Vector3.zero;

        var skeletonGraphic = rectTransform.GetComponent<SkeletonGraphic>();
        if (!skeletonGraphic)
        {
            return false;
        }

        if (!skeletonGraphic.IsValid)
        {
            skeletonGraphic.Initialize(false);
        }

        skeletonGraphic.UpdateMesh();
        var mesh = skeletonGraphic.GetLastMesh();
        if (!mesh || mesh.vertexCount == 0)
        {
            return false;
        }

        mesh.RecalculateBounds();
        center = GetVisibleMeshCenter(mesh);
        return true;
    }

    private static Vector3 GetVisibleMeshCenter(Mesh mesh)
    {
        var vertices = mesh.vertices;
        if (vertices == null || vertices.Length == 0)
        {
            return mesh.bounds.center;
        }

        var colors = mesh.colors32;
        var canFilterByAlpha = colors != null && colors.Length == vertices.Length;

        var hasVisibleVertex = false;
        var visibleBounds = new Bounds();
        var visibleVertexSum = Vector3.zero;
        var visibleVertexCount = 0;

        for (var i = 0; i < vertices.Length; i++)
        {
            if (canFilterByAlpha && colors[i].a == 0)
            {
                continue;
            }

            if (!hasVisibleVertex)
            {
                visibleBounds = new Bounds(vertices[i], Vector3.zero);
                hasVisibleVertex = true;
            }
            else
            {
                visibleBounds.Encapsulate(vertices[i]);
            }

            visibleVertexSum += vertices[i];
            visibleVertexCount++;
        }

        if (visibleVertexCount > 0)
        {
            return visibleVertexSum / visibleVertexCount;
        }

        return hasVisibleVertex ? visibleBounds.center : mesh.bounds.center;
    }

    [MenuItem(ValidateMenuPath)]
    public static void ValidateWrapperStability()
    {
        var root = GameObject.Find(TargetRootPath)?.transform as RectTransform;
        if (!root)
        {
            Debug.LogError($"[SignUiSpineWrapperPatcher] Target root not found: {TargetRootPath}");
            return;
        }

        var wrappers = root.GetComponentsInChildren<RectTransform>(true);
        var checkedCount = 0;
        var maxDrift = 0f;

        foreach (var wrapper in wrappers)
        {
            if (!wrapper || !wrapper.name.EndsWith(WrapperSuffix))
            {
                continue;
            }

            if (wrapper.childCount != 1)
            {
                continue;
            }

            var child = wrapper.GetChild(0) as RectTransform;
            if (!child || !child.GetComponent<SkeletonGraphic>())
            {
                continue;
            }

            var originalScale = wrapper.localScale;
            var originalRotation = wrapper.localRotation;
            var centerBefore = GetVisualCenterWorld(child);

            wrapper.localScale = Vector3.Scale(originalScale, new Vector3(1.1f, 1.1f, 1f));
            var centerAfterScale = GetVisualCenterWorld(child);
            wrapper.localScale = originalScale;

            wrapper.localRotation = originalRotation * Quaternion.Euler(0f, 0f, 15f);
            var centerAfterRotate = GetVisualCenterWorld(child);
            wrapper.localRotation = originalRotation;

            var scaleDrift = Vector3.Distance(centerBefore, centerAfterScale);
            var rotateDrift = Vector3.Distance(centerBefore, centerAfterRotate);
            var drift = Mathf.Max(scaleDrift, rotateDrift);

            if (scaleDrift > maxDrift)
            {
                maxDrift = scaleDrift;
            }

            if (rotateDrift > maxDrift)
            {
                maxDrift = rotateDrift;
            }

            checkedCount++;
            Debug.Log(
                $"[SignUiSpineWrapperPatcher] Stability '{wrapper.name}': scaleDrift={scaleDrift:F6}, rotateDrift={rotateDrift:F6}, maxDrift={drift:F6}");
        }

        Debug.Log($"[SignUiSpineWrapperPatcher] Stability check done. checked={checkedCount}, maxDrift={maxDrift:F6}");
    }

    private static Vector3 GetVisualCenterWorld(RectTransform rectTransform)
    {
        var localCenterOffset = GetVisualCenterLocal(rectTransform);
        return rectTransform.TransformPoint(localCenterOffset);
    }
}
