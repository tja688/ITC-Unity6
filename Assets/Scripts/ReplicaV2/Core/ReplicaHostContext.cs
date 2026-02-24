using UnityEngine;

public sealed class ReplicaHostContext
{
    public RectTransform MountRoot { get; private set; }
    public Camera UICamera { get; private set; }
    public IReplicaPointerSource Pointer { get; private set; }
    public IReplicaTweenAdapter Tweens { get; private set; }
    public bool UseUnscaledTime { get; private set; }

    public ReplicaHostContext(RectTransform mountRoot, Camera uiCamera, IReplicaPointerSource pointer, IReplicaTweenAdapter tweens, bool useUnscaledTime)
    {
        MountRoot = mountRoot;
        UICamera = uiCamera;
        Pointer = pointer;
        Tweens = tweens;
        UseUnscaledTime = useUnscaledTime;
    }
}
