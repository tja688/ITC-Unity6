using UnityEngine;

public enum ReplicaEnterDirection
{
    FromBottom,
    FromTop,
    FromLeft,
    FromRight,
    None
}

public enum ReplicaExitDirection
{
    ToBottom,
    ToTop,
    ToLeft,
    ToRight,
    FadeOnly
}

[System.Serializable]
public struct ReplicaTransition
{
    public ReplicaEnterDirection EnterDirection;
    public ReplicaExitDirection ExitDirection;
    public float Duration;
    public ReplicaEase Ease;

    public static ReplicaTransition Default
    {
        get
        {
            return new ReplicaTransition
            {
                EnterDirection = ReplicaEnterDirection.FromBottom,
                ExitDirection = ReplicaExitDirection.FadeOnly,
                Duration = 0.32f,
                Ease = ReplicaEase.OutCubic
            };
        }
    }
}
