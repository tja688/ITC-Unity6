using System;
using UnityEngine;
using UnityEngine.UI;

public interface IReplicaTweenAdapter
{
    void Kill(UnityEngine.Object owner, bool complete = false);

    IReplicaTweenHandle AnchoredPosTo(RectTransform target, Vector2 endValue, float duration, UnityEngine.Object owner = null);
    IReplicaTweenHandle AnchoredPosXTo(RectTransform target, float endX, float duration, UnityEngine.Object owner = null);
    IReplicaTweenHandle AnchoredPosYTo(RectTransform target, float endY, float duration, UnityEngine.Object owner = null);
    IReplicaTweenHandle ScaleTo(RectTransform target, Vector3 endValue, float duration, UnityEngine.Object owner = null);
    IReplicaTweenHandle LocalRotateTo(RectTransform target, Vector3 endEuler, float duration, UnityEngine.Object owner = null);
    IReplicaTweenHandle FadeImageTo(Image target, float endAlpha, float duration, UnityEngine.Object owner = null);
    IReplicaTweenHandle FadeTextTo(Text target, float endAlpha, float duration, UnityEngine.Object owner = null);
    IReplicaTweenHandle ColorImageTo(Image target, Color endValue, float duration, UnityEngine.Object owner = null);
    IReplicaTweenHandle ToFloat(Func<float> getter, Action<float> setter, float endValue, float duration, UnityEngine.Object owner = null);
    IReplicaTweenHandle DelayedCall(float delay, Action callback, UnityEngine.Object owner = null);
    IReplicaTweenHandle PunchScale(RectTransform target, Vector3 punch, float duration, int vibrato, float elasticity, UnityEngine.Object owner = null);
}
