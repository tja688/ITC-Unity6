public static class ReplicaTweenAdapterFactory
{
    public static IReplicaTweenAdapter Create(ReplicaTweenBackend backend)
    {
        // Target project only uses DOTween
        return new ReplicaTweenAdapter_DOTween();
    }
}
