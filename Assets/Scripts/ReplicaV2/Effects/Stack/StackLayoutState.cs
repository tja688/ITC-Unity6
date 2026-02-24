using UnityEngine;

public readonly struct StackLayoutState
{
    public readonly Vector2 Position;
    public readonly float RotationZ;
    public readonly float Scale;
    public readonly int SiblingIndex;

    public StackLayoutState(Vector2 position, float rotationZ, float scale, int siblingIndex)
    {
        Position = position;
        RotationZ = rotationZ;
        Scale = scale;
        SiblingIndex = siblingIndex;
    }
}
