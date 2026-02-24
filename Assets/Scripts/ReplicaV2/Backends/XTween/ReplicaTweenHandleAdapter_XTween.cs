using System;
using SevenStrikeModules.XTween;

public sealed class ReplicaTweenHandleAdapter_XTween : IReplicaTweenHandle
{
    private readonly ReplicaTweenHandle_XTween mHandle;

    public ReplicaTweenHandleAdapter_XTween(ReplicaTweenHandle_XTween handle)
    {
        mHandle = handle;
    }

    public bool IsActive => mHandle != null && mHandle.IsActive;
    public bool IsPlaying => mHandle != null && mHandle.IsPlaying;
    public float Duration => mHandle != null ? mHandle.Duration : 0f;

    public IReplicaTweenHandle SetDelay(float delay)
    {
        mHandle?.SetDelay(delay);
        return this;
    }

    public IReplicaTweenHandle SetEase(ReplicaEase ease, float overshootOrAmplitude = 0f)
    {
        var mapped = ReplicaEaseMapper.ToXTween(ease);
        mHandle?.SetEase(mapped, overshootOrAmplitude);
        return this;
    }

    public IReplicaTweenHandle SetUpdate(bool independentUpdate)
    {
        mHandle?.SetUpdate(independentUpdate);
        return this;
    }

    public IReplicaTweenHandle OnComplete(Action callback)
    {
        mHandle?.OnComplete(callback);
        return this;
    }

    public IReplicaTweenHandle SetId(object id)
    {
        mHandle?.SetId(id);
        return this;
    }

    public void Kill(bool complete = false)
    {
        mHandle?.Kill(complete);
    }
}
