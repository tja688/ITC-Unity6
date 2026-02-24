using System;

public interface IReplicaTweenHandle
{
    bool IsActive { get; }
    bool IsPlaying { get; }
    float Duration { get; }

    IReplicaTweenHandle SetDelay(float delay);
    IReplicaTweenHandle SetEase(ReplicaEase ease, float overshootOrAmplitude = 0f);
    IReplicaTweenHandle SetUpdate(bool independentUpdate);
    IReplicaTweenHandle OnComplete(Action callback);
    IReplicaTweenHandle SetId(object id);
    void Kill(bool complete = false);
}
