using System;
using SevenStrikeModules.XTween;

public sealed class ReplicaTweenHandle_XTween
{
    private readonly XTween_Interface mTween;
    private object mId;

    public ReplicaTweenHandle_XTween(XTween_Interface tween, object id)
    {
        mTween = tween;
        mId = id;
    }

    public bool IsActive => mTween != null && mTween.IsActive;
    public bool IsPlaying => mTween != null && mTween.IsPlaying;
    public float Duration => mTween != null ? mTween.Duration : 0f;

    public void SetDelay(float delay)
    {
        if (mTween == null)
        {
            return;
        }

        ReplicaTweens_XTween.ApplyStartDelay(mTween, delay);
    }

    public void SetEase(EaseMode easeMode, float overshootOrAmplitude = 0f)
    {
        mTween?.SetEase(easeMode);
    }

    public void SetUpdate(bool independentUpdate)
    {
    }

    public void OnComplete(Action callback)
    {
        mTween?.OnComplete(_ => callback?.Invoke());
    }

    public void SetId(object id)
    {
        mId = id;
    }

    public void Kill(bool complete = false)
    {
        if (mTween == null)
        {
            return;
        }

        ReplicaTweens_XTween.CancelPendingPlay(mTween);
        ReplicaTweens_XTween.Unregister(mId, mTween);
        mTween.Kill(complete);
    }
}
