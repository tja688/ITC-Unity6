using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public static class ReplicaTweens_DOTween
{
    public static void Kill(UnityEngine.Object owner, bool complete = false)
    {
        if (owner == null)
        {
            return;
        }

        DOTween.Kill(owner, complete);
    }

    public static ReplicaTweenHandle_DOTween AnchoredPosTo(RectTransform target, Vector2 endValue, float duration, UnityEngine.Object owner = null)
    {
        var tween = target.DOAnchorPos(endValue, duration);
        tween.SetId(owner != null ? owner : target);
        return new ReplicaTweenHandle_DOTween(tween);
    }

    public static ReplicaTweenHandle_DOTween AnchoredPosXTo(RectTransform target, float endX, float duration, UnityEngine.Object owner = null)
    {
        var tween = target.DOAnchorPosX(endX, duration);
        tween.SetId(owner != null ? owner : target);
        return new ReplicaTweenHandle_DOTween(tween);
    }

    public static ReplicaTweenHandle_DOTween AnchoredPosYTo(RectTransform target, float endY, float duration, UnityEngine.Object owner = null)
    {
        var tween = target.DOAnchorPosY(endY, duration);
        tween.SetId(owner != null ? owner : target);
        return new ReplicaTweenHandle_DOTween(tween);
    }

    public static ReplicaTweenHandle_DOTween ScaleTo(RectTransform target, Vector3 endValue, float duration, UnityEngine.Object owner = null)
    {
        var tween = target.DOScale(endValue, duration);
        tween.SetId(owner != null ? owner : target);
        return new ReplicaTweenHandle_DOTween(tween);
    }

    public static ReplicaTweenHandle_DOTween LocalRotateTo(RectTransform target, Vector3 endEuler, float duration, RotateMode mode, UnityEngine.Object owner = null)
    {
        var tween = target.DOLocalRotate(endEuler, duration, mode);
        tween.SetId(owner != null ? owner : target);
        return new ReplicaTweenHandle_DOTween(tween);
    }

    public static ReplicaTweenHandle_DOTween FadeImageTo(Image target, float endAlpha, float duration, UnityEngine.Object owner = null)
    {
        var tween = target.DOFade(endAlpha, duration);
        tween.SetId(owner != null ? owner : target);
        return new ReplicaTweenHandle_DOTween(tween);
    }

    public static ReplicaTweenHandle_DOTween FadeTextTo(Text target, float endAlpha, float duration, UnityEngine.Object owner = null)
    {
        var tween = target.DOFade(endAlpha, duration);
        tween.SetId(owner != null ? owner : target);
        return new ReplicaTweenHandle_DOTween(tween);
    }

    public static ReplicaTweenHandle_DOTween ColorImageTo(Image target, Color endValue, float duration, UnityEngine.Object owner = null)
    {
        var tween = target.DOColor(endValue, duration);
        tween.SetId(owner != null ? owner : target);
        return new ReplicaTweenHandle_DOTween(tween);
    }

    public static ReplicaTweenHandle_DOTween ToFloat(Func<float> getter, Action<float> setter, float endValue, float duration, UnityEngine.Object owner = null)
    {
        var tween = DOTween.To(() => getter(), value => setter(value), endValue, duration);
        tween.SetId(owner);
        return new ReplicaTweenHandle_DOTween(tween);
    }

    public static ReplicaTweenHandle_DOTween DelayedCall(float delay, Action callback, UnityEngine.Object owner = null)
    {
        var tween = DOVirtual.DelayedCall(delay, () => callback?.Invoke());
        tween.SetId(owner);
        return new ReplicaTweenHandle_DOTween(tween);
    }

    public static ReplicaTweenHandle_DOTween PunchScale(RectTransform target, Vector3 punch, float duration, int vibrato, float elasticity, UnityEngine.Object owner = null)
    {
        var tween = target.DOPunchScale(punch, duration, vibrato, elasticity);
        tween.SetId(owner != null ? owner : target);
        return new ReplicaTweenHandle_DOTween(tween);
    }
}
