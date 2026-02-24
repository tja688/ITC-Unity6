using System;
using DG.Tweening;

public sealed class ReplicaTweenHandle_DOTween
{
    private readonly Tween mTween;

    public ReplicaTweenHandle_DOTween(Tween tween)
    {
        mTween = tween;
    }

    public bool IsActive => mTween != null && mTween.active;
    public bool IsPlaying => mTween != null && mTween.IsPlaying();
    public float Duration => mTween != null ? mTween.Duration(false) : 0f;

    public void SetDelay(float delay)
    {
        if (mTween == null)
        {
            return;
        }

        mTween.SetDelay(delay);
    }

    public void SetEase(Ease ease, float overshootOrAmplitude = 0f)
    {
        if (mTween == null)
        {
            return;
        }

        if (Math.Abs(overshootOrAmplitude) > 0.0001f)
        {
            mTween.SetEase(ease, overshootOrAmplitude);
        }
        else
        {
            mTween.SetEase(ease);
        }
    }

    public void SetUpdate(bool independentUpdate)
    {
        if (mTween == null)
        {
            return;
        }

        mTween.SetUpdate(independentUpdate);
    }

    public void OnComplete(Action callback)
    {
        if (mTween == null)
        {
            return;
        }

        mTween.OnComplete(() => callback?.Invoke());
    }

    public void SetId(object id)
    {
        if (mTween == null)
        {
            return;
        }

        mTween.SetId(id);
    }

    public void Kill(bool complete = false)
    {
        mTween?.Kill(complete);
    }
}
