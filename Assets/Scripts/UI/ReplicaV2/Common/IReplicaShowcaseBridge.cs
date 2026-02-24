public interface IReplicaShowcaseBridge
{
    void SetBackend(ReplicaTweenBackend backend, bool rebuild = true);
    void PlayIn(ReplicaEnterDirection direction);
    void PlayOut(ReplicaExitDirection direction);
}
