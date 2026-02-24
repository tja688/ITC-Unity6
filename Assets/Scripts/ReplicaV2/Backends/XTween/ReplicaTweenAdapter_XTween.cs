using System;
using SevenStrikeModules.XTween;
using UnityEngine;
using UnityEngine.UI;

public sealed class ReplicaTweenAdapter_XTween : IReplicaTweenAdapter
{
    public void Kill(UnityEngine.Object owner, bool complete = false)
    {
        ReplicaTweens_XTween.Kill(owner, complete);
    }

    public IReplicaTweenHandle AnchoredPosTo(RectTransform target, Vector2 endValue, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_XTween(ReplicaTweens_XTween.AnchoredPosTo(target, endValue, duration, owner));
    }

    public IReplicaTweenHandle AnchoredPosXTo(RectTransform target, float endX, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_XTween(ReplicaTweens_XTween.AnchoredPosXTo(target, endX, duration, owner));
    }

    public IReplicaTweenHandle AnchoredPosYTo(RectTransform target, float endY, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_XTween(ReplicaTweens_XTween.AnchoredPosYTo(target, endY, duration, owner));
    }

    public IReplicaTweenHandle ScaleTo(RectTransform target, Vector3 endValue, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_XTween(ReplicaTweens_XTween.ScaleTo(target, endValue, duration, owner));
    }

    public IReplicaTweenHandle LocalRotateTo(RectTransform target, Vector3 endEuler, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_XTween(ReplicaTweens_XTween.LocalRotateTo(target, endEuler, duration, RotationMode.Shortest, owner));
    }

    public IReplicaTweenHandle FadeImageTo(Image target, float endAlpha, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_XTween(ReplicaTweens_XTween.FadeImageTo(target, endAlpha, duration, owner));
    }

    public IReplicaTweenHandle FadeTextTo(Text target, float endAlpha, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_XTween(ReplicaTweens_XTween.FadeTextTo(target, endAlpha, duration, owner));
    }

    public IReplicaTweenHandle ColorImageTo(Image target, Color endValue, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_XTween(ReplicaTweens_XTween.ColorImageTo(target, endValue, duration, owner));
    }

    public IReplicaTweenHandle ToFloat(Func<float> getter, Action<float> setter, float endValue, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_XTween(ReplicaTweens_XTween.ToFloat(getter, setter, endValue, duration, owner));
    }

    public IReplicaTweenHandle DelayedCall(float delay, Action callback, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_XTween(ReplicaTweens_XTween.DelayedCall(delay, callback, owner));
    }

    public IReplicaTweenHandle PunchScale(RectTransform target, Vector3 punch, float duration, int vibrato, float elasticity, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_XTween(ReplicaTweens_XTween.PunchScale(target, punch, duration, vibrato, elasticity, owner));
    }
}
