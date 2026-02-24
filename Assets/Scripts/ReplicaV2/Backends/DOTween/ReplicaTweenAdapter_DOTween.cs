using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public sealed class ReplicaTweenAdapter_DOTween : IReplicaTweenAdapter
{
    public void Kill(UnityEngine.Object owner, bool complete = false)
    {
        ReplicaTweens_DOTween.Kill(owner, complete);
    }

    public IReplicaTweenHandle AnchoredPosTo(RectTransform target, Vector2 endValue, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_DOTween(ReplicaTweens_DOTween.AnchoredPosTo(target, endValue, duration, owner));
    }

    public IReplicaTweenHandle AnchoredPosXTo(RectTransform target, float endX, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_DOTween(ReplicaTweens_DOTween.AnchoredPosXTo(target, endX, duration, owner));
    }

    public IReplicaTweenHandle AnchoredPosYTo(RectTransform target, float endY, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_DOTween(ReplicaTweens_DOTween.AnchoredPosYTo(target, endY, duration, owner));
    }

    public IReplicaTweenHandle ScaleTo(RectTransform target, Vector3 endValue, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_DOTween(ReplicaTweens_DOTween.ScaleTo(target, endValue, duration, owner));
    }

    public IReplicaTweenHandle LocalRotateTo(RectTransform target, Vector3 endEuler, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_DOTween(ReplicaTweens_DOTween.LocalRotateTo(target, endEuler, duration, RotateMode.Fast, owner));
    }

    public IReplicaTweenHandle FadeImageTo(Image target, float endAlpha, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_DOTween(ReplicaTweens_DOTween.FadeImageTo(target, endAlpha, duration, owner));
    }

    public IReplicaTweenHandle FadeTextTo(Text target, float endAlpha, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_DOTween(ReplicaTweens_DOTween.FadeTextTo(target, endAlpha, duration, owner));
    }

    public IReplicaTweenHandle ColorImageTo(Image target, Color endValue, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_DOTween(ReplicaTweens_DOTween.ColorImageTo(target, endValue, duration, owner));
    }

    public IReplicaTweenHandle ToFloat(Func<float> getter, Action<float> setter, float endValue, float duration, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_DOTween(ReplicaTweens_DOTween.ToFloat(getter, setter, endValue, duration, owner));
    }

    public IReplicaTweenHandle DelayedCall(float delay, Action callback, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_DOTween(ReplicaTweens_DOTween.DelayedCall(delay, callback, owner));
    }

    public IReplicaTweenHandle PunchScale(RectTransform target, Vector3 punch, float duration, int vibrato, float elasticity, UnityEngine.Object owner = null)
    {
        return new ReplicaTweenHandleAdapter_DOTween(ReplicaTweens_DOTween.PunchScale(target, punch, duration, vibrato, elasticity, owner));
    }
}
