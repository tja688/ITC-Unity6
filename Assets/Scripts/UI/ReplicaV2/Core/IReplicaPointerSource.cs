using UnityEngine;

public interface IReplicaPointerSource
{
    Vector2 ScreenPosition { get; }
    bool IsPointerValid { get; }

    bool TryGetLocalPoint(RectTransform target, out Vector2 localPoint, Camera eventCamera = null);
}
