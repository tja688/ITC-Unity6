using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.IMGUI.Controls;
#endif

[ExecuteAlways]
public sealed class LineSideActorToggle : MonoBehaviour
{
    [Header("Area Configuration")]
    [SerializeField, Tooltip("The dimensions of the selection box (Width, Height, Depth).")]
    private Vector3 mBoxSize = new Vector3(10f, 10f, 10f);

    [SerializeField, Tooltip("The center offset of the box relative to this transform.")]
    private Vector3 mBoxOffset = Vector3.zero;

    [SerializeField, Tooltip("Tag to filter objects that should be toggled.")]
    private string mTargetTag = "Actor";

    [Header("Debug Settings")]
    [SerializeField]
    private bool mShowGizmos = true;

    [SerializeField]
    private Color mGizmoColor = new Color(0f, 1f, 1f, 0.3f);

    private List<GameObject> mCachedActors = new List<GameObject>();
    private float mNextCacheRefresh = 0f;

    private void Update()
    {
        // Periodic cache refresh to find new actors (including inactive ones)
        if (Application.isPlaying && Time.time >= mNextCacheRefresh)
        {
            RefreshActorCache();
            mNextCacheRefresh = Time.time + 2.0f; // Refresh every 2 seconds
        }
        else if (!Application.isPlaying)
        {
            // In editor mode, refresh more frequently or every frame for responsiveness
            RefreshActorCache();
        }

        PerformToggle();
    }

    private void RefreshActorCache()
    {
        mCachedActors.Clear();

        // FindObjectsByType with Inactive.Include is efficient in Unity 6 for finding all potential targets.
        // We filter by tag manually because there's no built-in "FindAllByTag(includeInactive)".

        var allTransforms = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var t in allTransforms)
        {
            if (t.gameObject != gameObject && t.CompareTag(mTargetTag))
            {
                mCachedActors.Add(t.gameObject);
            }
        }
    }

    private void PerformToggle()
    {
        if (mCachedActors.Count == 0) return;

        Matrix4x4 worldToLocal = transform.worldToLocalMatrix;
        Vector3 halfSize = mBoxSize * 0.5f;

        foreach (GameObject actor in mCachedActors)
        {
            if (actor == null) continue;

            // Compute local position relative to this transform and the offset
            Vector3 localPos = worldToLocal.MultiplyPoint3x4(actor.transform.position) - mBoxOffset;

            // Check if within bounds
            bool isInside = Mathf.Abs(localPos.x) <= halfSize.x &&
                            Mathf.Abs(localPos.y) <= halfSize.y &&
                            Mathf.Abs(localPos.z) <= halfSize.z;

            // Set activity
            if (isInside)
            {
                if (!actor.activeSelf) actor.SetActive(true);
            }
            else
            {
                if (actor.activeSelf) actor.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!mShowGizmos) return;

        Gizmos.matrix = transform.localToWorldMatrix;

        // Draw the volume

        Gizmos.color = mGizmoColor;
        Gizmos.DrawCube(mBoxOffset, mBoxSize);

        // Draw the wireframe

        Gizmos.color = new Color(mGizmoColor.r, mGizmoColor.g, mGizmoColor.b, 1f);
        Gizmos.DrawWireCube(mBoxOffset, mBoxSize);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(LineSideActorToggle))]
public class LineSideActorToggleEditor : Editor
{
    private BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();

    protected virtual void OnSceneGUI()
    {
        LineSideActorToggle script = (LineSideActorToggle)target;
        if (!script.enabled) return;

        // Retrieve current values
        SerializedProperty sizeProp = serializedObject.FindProperty("mBoxSize");
        SerializedProperty offsetProp = serializedObject.FindProperty("mBoxOffset");

        m_BoundsHandle.center = offsetProp.vector3Value;
        m_BoundsHandle.size = sizeProp.vector3Value;

        // Draw handles in local space
        Matrix4x4 handleMatrix = script.transform.localToWorldMatrix;


        using (new Handles.DrawingScope(handleMatrix))
        {
            EditorGUI.BeginChangeCheck();
            m_BoundsHandle.DrawHandle();
            if (EditorGUI.EndChangeCheck())
            {
                undoHandler(sizeProp, offsetProp, m_BoundsHandle.size, m_BoundsHandle.center);
            }
        }
    }

    private void undoHandler(SerializedProperty sizeProp, SerializedProperty offsetProp, Vector3 newSize, Vector3 newCenter)
    {
        Undo.RecordObject(target, "Change Actor Toggle Bounds");
        sizeProp.vector3Value = newSize;
        offsetProp.vector3Value = newCenter;
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
