using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using SevenStrikeModules.XTween;
using UnityEngine;
using UnityEngine.UI;

public static class ReplicaTweens_XTween
{
    private sealed class ReplicaTweenDelayRunner : MonoBehaviour
    {
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

        public new bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return obj == null ? 0 : RuntimeHelpers.GetHashCode(obj);
        }
    }

    private static readonly Dictionary<object, List<XTween_Interface>> sTweensByOwner = new Dictionary<object, List<XTween_Interface>>(ReferenceEqualityComparer.Instance);
    private static readonly Dictionary<object, Coroutine> sPendingPlayByTween = new Dictionary<object, Coroutine>(ReferenceEqualityComparer.Instance);

    private static bool sManagerEnsured;
    private static ReplicaTweenDelayRunner sDelayRunner;

    private static void EnsureManager()
    {
        if (sManagerEnsured)
        {
            return;
        }

        if (!Application.isPlaying)
        {
            return;
        }

        // Try to find if XTween_Manager exists in scene
        var existingManager = UnityEngine.Object.FindFirstObjectByType<XTween_Manager>();
        if (existingManager != null)
        {
            sManagerEnsured = true;
            return;
        }

        var go = new GameObject("XTween_Manager (ReplicaV2)");
        go.AddComponent<XTween_Manager>();
        sManagerEnsured = true;
    }

    public static void Kill(UnityEngine.Object owner, bool complete = false)
    {
        if (owner == null)
        {
            return;
        }

        if (!sTweensByOwner.TryGetValue(owner, out var list) || list == null)
        {
            return;
        }

        var snapshot = list.ToArray();
        for (var i = 0; i < snapshot.Length; i++)
        {
            var tween = snapshot[i];
            CancelPendingPlay(tween);
            tween?.Kill(complete);
        }

        list.Clear();
        sTweensByOwner.Remove(owner);
    }

    internal static void CancelPendingPlay(XTween_Interface tween)
    {
        if (tween == null)
        {
            return;
        }

        var key = (object)tween;
        if (!sPendingPlayByTween.TryGetValue(key, out var coroutine))
        {
            return;
        }

        if (sDelayRunner != null && coroutine != null)
        {
            sDelayRunner.StopCoroutine(coroutine);
        }

        sPendingPlayByTween.Remove(key);
    }

    internal static void ApplyStartDelay(XTween_Interface tween, float delay)
    {
        if (tween == null || tween.IsKilled)
        {
            return;
        }

        CancelPendingPlay(tween);
        tween.SetDelay(0f);
        tween.Rewind(false);
        SchedulePlay(tween, Mathf.Max(0f, delay));
    }

    private static void SchedulePlay(XTween_Interface tween, float delay)
    {
        if (tween == null || tween.IsKilled)
        {
            return;
        }

        var clampedDelay = Mathf.Max(0f, delay);
        if (clampedDelay <= 0f)
        {
            tween.Play();
            return;
        }

        var runner = EnsureDelayRunner();
        if (runner == null)
        {
            tween.Play();
            return;
        }

        var key = (object)tween;
        sPendingPlayByTween[key] = runner.StartCoroutine(PlayAfterDelay(key, tween, clampedDelay));
    }

    private static ReplicaTweenDelayRunner EnsureDelayRunner()
    {
        EnsureManager();
        if (sDelayRunner != null)
        {
            return sDelayRunner;
        }

        sDelayRunner = UnityEngine.Object.FindFirstObjectByType<ReplicaTweenDelayRunner>();
        if (sDelayRunner != null)
        {
            return sDelayRunner;
        }

        var manager = UnityEngine.Object.FindFirstObjectByType<XTween_Manager>();
        if (manager != null)
        {
            sDelayRunner = manager.GetComponent<ReplicaTweenDelayRunner>();
            if (sDelayRunner == null)
            {
                sDelayRunner = manager.gameObject.AddComponent<ReplicaTweenDelayRunner>();
            }
        }

        return sDelayRunner;
    }

    private static IEnumerator PlayAfterDelay(object key, XTween_Interface tween, float delay)
    {
        yield return new WaitForSeconds(delay);
        sPendingPlayByTween.Remove(key);

        if (tween == null || tween.IsKilled || !tween.IsActive)
        {
            yield break;
        }

        tween.Play();
    }

    internal static void Unregister(object owner, XTween_Interface tween)
    {
        CancelPendingPlay(tween);

        if (owner == null || tween == null)
        {
            return;
        }

        if (!sTweensByOwner.TryGetValue(owner, out var list) || list == null)
        {
            return;
        }

        list.Remove(tween);
        if (list.Count <= 0)
        {
            sTweensByOwner.Remove(owner);
        }
    }

    private static void Register(object owner, XTween_Interface tween)
    {
        if (owner == null || tween == null)
        {
            return;
        }

        EnsureManager();

        if (!sTweensByOwner.TryGetValue(owner, out var list) || list == null)
        {
            list = new List<XTween_Interface>(4);
            sTweensByOwner[owner] = list;
        }

        list.Add(tween);
        tween.OnKill(() => Unregister(owner, tween));
    }

    public static ReplicaTweenHandle_XTween AnchoredPosTo(RectTransform target, Vector2 endValue, float duration, UnityEngine.Object owner = null)
    {
        EnsureManager();
        var id = owner != null ? owner : target;
        var tween = XTween.To(() => target.anchoredPosition, v => target.anchoredPosition = v, endValue, duration, true);
        tween.Play();
        Register(id, tween);
        return new ReplicaTweenHandle_XTween(tween, id);
    }

    public static ReplicaTweenHandle_XTween AnchoredPosXTo(RectTransform target, float endX, float duration, UnityEngine.Object owner = null)
    {
        EnsureManager();
        var id = owner != null ? owner : target;
        var tween = XTween.To(
            () => target.anchoredPosition.x,
            x =>
            {
                var p = target.anchoredPosition;
                p.x = x;
                target.anchoredPosition = p;
            },
            endX,
            duration,
            true);
        tween.Play();
        Register(id, tween);
        return new ReplicaTweenHandle_XTween(tween, id);
    }

    public static ReplicaTweenHandle_XTween AnchoredPosYTo(RectTransform target, float endY, float duration, UnityEngine.Object owner = null)
    {
        EnsureManager();
        var id = owner != null ? owner : target;
        var tween = XTween.To(
            () => target.anchoredPosition.y,
            y =>
            {
                var p = target.anchoredPosition;
                p.y = y;
                target.anchoredPosition = p;
            },
            endY,
            duration,
            true);
        tween.Play();
        Register(id, tween);
        return new ReplicaTweenHandle_XTween(tween, id);
    }

    public static ReplicaTweenHandle_XTween ScaleTo(RectTransform target, Vector3 endValue, float duration, UnityEngine.Object owner = null)
    {
        EnsureManager();
        var id = owner != null ? owner : target;
        var tween = XTween.To(() => target.localScale, v => target.localScale = v, endValue, duration, true);
        tween.Play();
        Register(id, tween);
        return new ReplicaTweenHandle_XTween(tween, id);
    }

    public static ReplicaTweenHandle_XTween LocalRotateTo(RectTransform target, Vector3 endEuler, float duration, RotationMode mode, UnityEngine.Object owner = null)
    {
        EnsureManager();
        var id = owner != null ? owner : target;
        var tween = XTween.To(() => target.localEulerAngles, v => target.localEulerAngles = v, endEuler, duration, true);
        tween.Play();
        Register(id, tween);
        return new ReplicaTweenHandle_XTween(tween, id);
    }

    public static ReplicaTweenHandle_XTween FadeImageTo(Image target, float endAlpha, float duration, UnityEngine.Object owner = null)
    {
        EnsureManager();
        var id = owner != null ? owner : target;
        var tween = XTween.To(
            () => target.color.a,
            a =>
            {
                var c = target.color;
                c.a = a;
                target.color = c;
            },
            endAlpha,
            duration,
            true);
        tween.Play();
        Register(id, tween);
        return new ReplicaTweenHandle_XTween(tween, id);
    }

    public static ReplicaTweenHandle_XTween FadeTextTo(Text target, float endAlpha, float duration, UnityEngine.Object owner = null)
    {
        EnsureManager();
        var id = owner != null ? owner : target;
        var tween = XTween.To(
            () => target.color.a,
            a =>
            {
                var c = target.color;
                c.a = a;
                target.color = c;
            },
            endAlpha,
            duration,
            true);
        tween.Play();
        Register(id, tween);
        return new ReplicaTweenHandle_XTween(tween, id);
    }

    public static ReplicaTweenHandle_XTween ColorImageTo(Image target, Color endValue, float duration, UnityEngine.Object owner = null)
    {
        EnsureManager();
        var id = owner != null ? owner : target;
        var tween = XTween.To(() => target.color, c => target.color = c, endValue, duration, true);
        tween.Play();
        Register(id, tween);
        return new ReplicaTweenHandle_XTween(tween, id);
    }

    public static ReplicaTweenHandle_XTween ToFloat(Func<float> getter, Action<float> setter, float endValue, float duration, UnityEngine.Object owner = null)
    {
        EnsureManager();
        object id = owner != null ? owner : getter;
        var tween = XTween.To(() => getter(), v => setter(v), endValue, duration, true);
        tween.Play();
        Register(id, tween);
        return new ReplicaTweenHandle_XTween(tween, id);
    }

    public static ReplicaTweenHandle_XTween DelayedCall(float delay, Action callback, UnityEngine.Object owner = null)
    {
        EnsureManager();
        object id = owner != null ? owner : callback;
        var tween = XTween.To(() => 0f, _ => { }, 1f, 0.0001f, true).SetDelay(0f).OnComplete(_ => callback?.Invoke());
        Register(id, tween);
        SchedulePlay(tween, Mathf.Max(0f, delay));
        return new ReplicaTweenHandle_XTween(tween, id);
    }

    public static ReplicaTweenHandle_XTween PunchScale(RectTransform target, Vector3 punch, float duration, int vibrato, float elasticity, UnityEngine.Object owner = null)
    {
        EnsureManager();
        var id = owner != null ? owner : target;
        var baseScale = target.localScale;
        var vib = Mathf.Max(1, vibrato);
        var el = Mathf.Clamp01(elasticity);

        var tween = XTween.To(
            () => 0f,
            t =>
            {
                var omega = vib * Mathf.PI * 2f;
                var damper = Mathf.Exp(-t * vib * (1f - el) * 2f);
                var s = Mathf.Sin(t * omega) * damper;
                target.localScale = baseScale + punch * s;
            },
            1f,
            duration,
            true).OnComplete(_ => target.localScale = baseScale);
        tween.Play();
        Register(id, tween);
        return new ReplicaTweenHandle_XTween(tween, id);
    }
}
