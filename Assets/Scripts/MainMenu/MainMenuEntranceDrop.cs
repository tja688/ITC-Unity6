using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MainMenuEntranceDrop : MonoBehaviour
{
    [SerializeField] private Transform menuRoot;
    [SerializeField] private float startYOffset = 2.0f;
    [SerializeField] private float interval = 0.08f;
    [SerializeField] private float duration = 0.45f;
    [SerializeField] private bool useUnscaledTime = true;
    [SerializeField] private bool playOnEnable = true;
    [SerializeField] private AnimationCurve easing = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 2.2f),
        new Keyframe(0.7f, 1.02f, 1.2f, 0.2f),
        new Keyframe(1f, 1f));

    private readonly List<Item> items = new();
    private Coroutine playRoutine;

    private sealed class Item
    {
        public Transform Transform;
        public SpriteRenderer SpriteRenderer;
        public Vector3 TargetLocalPosition;
        public Color TargetColor;
    }

    private void Reset()
    {
        menuRoot = transform;
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            Play();
        }
    }

    [ContextMenu("Play Entrance")]
    public void Play()
    {
        if (playRoutine != null)
        {
            StopCoroutine(playRoutine);
            playRoutine = null;
        }

        BuildItems();
        PrepareStartState();
        playRoutine = StartCoroutine(PlayRoutine());
    }

    private void BuildItems()
    {
        items.Clear();

        var root = menuRoot != null ? menuRoot : transform;
        for (var i = 0; i < root.childCount; i++)
        {
            var child = root.GetChild(i);
            var spriteRenderer = child.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                continue;
            }

            items.Add(new Item
            {
                Transform = child,
                SpriteRenderer = spriteRenderer,
                TargetLocalPosition = child.localPosition,
                TargetColor = spriteRenderer.color
            });
        }

        // Top-to-bottom by current visual position.
        items.Sort((a, b) =>
        {
            var byY = b.Transform.position.y.CompareTo(a.Transform.position.y);
            if (byY != 0)
            {
                return byY;
            }

            return b.SpriteRenderer.sortingOrder.CompareTo(a.SpriteRenderer.sortingOrder);
        });
    }

    private void PrepareStartState()
    {
        foreach (var item in items)
        {
            item.Transform.localPosition = item.TargetLocalPosition + Vector3.up * startYOffset;

            var color = item.TargetColor;
            color.a = 0f;
            item.SpriteRenderer.color = color;
        }
    }

    private IEnumerator PlayRoutine()
    {
        foreach (var item in items)
        {
            StartCoroutine(AnimateItem(item));
            if (interval > 0f)
            {
                yield return Wait(interval);
            }
        }

        playRoutine = null;
    }

    private IEnumerator AnimateItem(Item item)
    {
        var elapsed = 0f;
        var startPosition = item.TargetLocalPosition + Vector3.up * startYOffset;

        while (elapsed < duration)
        {
            elapsed += DeltaTime();
            var t = duration <= 0f ? 1f : Mathf.Clamp01(elapsed / duration);
            var eased = easing.Evaluate(t);

            item.Transform.localPosition = Vector3.LerpUnclamped(startPosition, item.TargetLocalPosition, eased);

            var color = item.TargetColor;
            color.a = Mathf.LerpUnclamped(0f, item.TargetColor.a, eased);
            item.SpriteRenderer.color = color;

            yield return null;
        }

        item.Transform.localPosition = item.TargetLocalPosition;
        item.SpriteRenderer.color = item.TargetColor;
    }

    private IEnumerator Wait(float seconds)
    {
        if (useUnscaledTime)
        {
            yield return new WaitForSecondsRealtime(seconds);
        }
        else
        {
            yield return new WaitForSeconds(seconds);
        }
    }

    private float DeltaTime()
    {
        return useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
    }
}
