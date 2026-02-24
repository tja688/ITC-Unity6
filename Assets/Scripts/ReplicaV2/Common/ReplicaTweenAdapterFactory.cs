public static class ReplicaTweenAdapterFactory
{
    public static IReplicaTweenAdapter Create(ReplicaTweenBackend backend)
    {
        return backend == ReplicaTweenBackend.DOTween
            ? (IReplicaTweenAdapter)new ReplicaTweenAdapter_DOTween()
            : new ReplicaTweenAdapter_XTween();
    }
}
