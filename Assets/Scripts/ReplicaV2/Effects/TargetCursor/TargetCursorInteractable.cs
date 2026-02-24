using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach this component to any RectTransform to make it automatically discoverable by the TargetCursor.
/// </summary>
[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class TargetCursorInteractable : MonoBehaviour
{
    private static readonly HashSet<RectTransform> s_ActiveTargets = new HashSet<RectTransform>();

    public static IReadOnlyCollection<RectTransform> ActiveTargets => s_ActiveTargets;

    private RectTransform mRect;

    private void Awake()
    {
        mRect = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (mRect != null)
        {
            s_ActiveTargets.Add(mRect);
        }
    }

    private void OnDisable()
    {
        if (mRect != null)
        {
            s_ActiveTargets.Remove(mRect);
        }
    }
}
