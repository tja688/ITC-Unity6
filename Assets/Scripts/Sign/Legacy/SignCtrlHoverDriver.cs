using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ITC.Dialogue
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public sealed class SignCtrlHoverDriver : MonoBehaviour
    {
        private const string HitboxObjectName = "__CtrlHitbox";

        private enum HoverVisualState
        {
            Idle = 0,
            Entering = 1,
            Hovered = 2,
            Exiting = 3
        }

        [Header("Hover Scale")]
        [SerializeField, Min(1f)] private float hoverScaleMultiplier = 1.06f;
        [SerializeField, Min(0f)] private float hoverEnterDuration = 0.08f;
        [SerializeField, Min(0f)] private float hoverExitDuration = 0.12f;
        [SerializeField] private AnimationCurve hoverCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [SerializeField] private bool useUnscaledTime = true;

        [Header("Hitbox")]
        [SerializeField] private bool autoBuildHitbox = true;
        [SerializeField] private bool autoFitWhenHitboxCreated = true;
        [SerializeField] private bool includeInactiveGraphics = true;
        [SerializeField] private Vector2 hitboxPadding = new(8f, 8f);
        [SerializeField] private Vector2 hitboxOffset = Vector2.zero;
        [SerializeField] private Vector2 hitboxSizeScale = Vector2.one;
        [SerializeField] private bool refreshHitboxInEditor = false;

        [Header("Click Placeholder")]
        [SerializeField] private bool enableClickDetection = false;
        [SerializeField] private UnityEvent onCtrlClicked;

        [Header("Runtime State (Read Only)")]
        [SerializeField] private HoverVisualState hoverState = HoverVisualState.Idle;

        [SerializeField, HideInInspector] private RectTransform hitboxRect;
        [SerializeField, HideInInspector] private SignCtrlHoverHitbox hitboxForwarder;
        [SerializeField, HideInInspector] private bool hitboxInitialized;

        private readonly List<RaycastResult> mRaycastResults = new();
        private readonly Vector3[] mWorldRectCorners = new Vector3[4];
        private EventSystem mCachedEventSystem;
        private PointerEventData mPointerEventData;
        private Vector3 mBaseLocalScale = Vector3.one;
        private float mHoverProgress;
        private bool mIsPointerInside;
        private bool mScaleInitialized;

        public bool IsHovering => hoverState == HoverVisualState.Entering || hoverState == HoverVisualState.Hovered;

        private void Awake()
        {
            CacheBaseScaleIfNeeded();
            EnsureHitbox();
            ApplyScaleImmediate(mHoverProgress);
        }

        private void OnEnable()
        {
            CacheBaseScaleIfNeeded();
            EnsureHitbox();
            ApplyScaleImmediate(mHoverProgress);
        }

        private void Reset()
        {
            CacheBaseScaleIfNeeded();
            EnsureHitbox();
            ApplyScaleImmediate(0f);
        }

        private void OnValidate()
        {
            hoverScaleMultiplier = Mathf.Max(1f, hoverScaleMultiplier);
            hoverEnterDuration = Mathf.Max(0f, hoverEnterDuration);
            hoverExitDuration = Mathf.Max(0f, hoverExitDuration);
            hitboxSizeScale.x = Mathf.Max(0.01f, hitboxSizeScale.x);
            hitboxSizeScale.y = Mathf.Max(0.01f, hitboxSizeScale.y);

            if (!Application.isPlaying && refreshHitboxInEditor)
            {
                mScaleInitialized = false;
                CacheBaseScaleIfNeeded();
                EnsureHitbox(forceRefit: true);
                ApplyScaleImmediate(mIsPointerInside ? 1f : 0f);
            }
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            if (mIsPointerInside && !IsPointerStillOverThisCtrl())
            {
                SetPointerInside(false);
            }

            float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            UpdateScaleAnimation(deltaTime);
        }

        private void OnDisable()
        {
            mIsPointerInside = false;
            mHoverProgress = 0f;
            hoverState = HoverVisualState.Idle;
            ApplyScaleImmediate(0f);
        }

        internal void NotifyPointerEnter()
        {
            SetPointerInside(true);
        }

        internal void NotifyPointerExit()
        {
            SetPointerInside(false);
        }

        internal void NotifyPointerClick(PointerEventData _)
        {
            if (!enableClickDetection)
            {
                return;
            }

            onCtrlClicked?.Invoke();
        }

        [ContextMenu("Rebuild Ctrl Hitbox")]
        public void RebuildHitbox()
        {
            EnsureHitbox(forceRefit: true);
        }

        private void SetPointerInside(bool isInside)
        {
            if (mIsPointerInside == isInside)
            {
                return;
            }

            mIsPointerInside = isInside;
            if (isInside)
            {
                hoverState = HoverVisualState.Entering;
            }
            else if (mHoverProgress > 0f)
            {
                hoverState = HoverVisualState.Exiting;
            }
            else
            {
                hoverState = HoverVisualState.Idle;
            }
        }

        private void UpdateScaleAnimation(float deltaTime)
        {
            float target = mIsPointerInside ? 1f : 0f;
            float duration = mIsPointerInside ? hoverEnterDuration : hoverExitDuration;
            float step = duration <= 0f ? 1f : deltaTime / duration;

            mHoverProgress = Mathf.MoveTowards(mHoverProgress, target, step);

            float sampled = hoverCurve != null ? hoverCurve.Evaluate(mHoverProgress) : mHoverProgress;
            ApplyScaleImmediate(sampled);

            if (mHoverProgress <= 0.0001f)
            {
                hoverState = HoverVisualState.Idle;
            }
            else if (mHoverProgress >= 0.9999f)
            {
                hoverState = HoverVisualState.Hovered;
            }
            else if (mIsPointerInside)
            {
                hoverState = HoverVisualState.Entering;
            }
            else
            {
                hoverState = HoverVisualState.Exiting;
            }
        }

        private void CacheBaseScaleIfNeeded()
        {
            if (mScaleInitialized)
            {
                return;
            }

            mBaseLocalScale = transform.localScale;
            mScaleInitialized = true;
        }

        private void ApplyScaleImmediate(float hover01)
        {
            CacheBaseScaleIfNeeded();
            float factor = Mathf.LerpUnclamped(1f, hoverScaleMultiplier, hover01);
            transform.localScale = mBaseLocalScale * factor;
        }

        private void EnsureHitbox(bool forceRefit = false)
        {
            if (!autoBuildHitbox)
            {
                return;
            }

            if (!TryGetComponent<RectTransform>(out _))
            {
                return;
            }

            var hitboxTransform = transform.Find(HitboxObjectName);
            bool createdThisCall = false;
            if (hitboxTransform == null)
            {
                var hitboxObject = new GameObject(HitboxObjectName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(SignCtrlHoverHitbox));
                hitboxTransform = hitboxObject.transform;
                hitboxTransform.SetParent(transform, false);
                createdThisCall = true;
            }

            if (!hitboxTransform.TryGetComponent(out hitboxRect))
            {
                hitboxRect = hitboxTransform.gameObject.AddComponent<RectTransform>();
            }

            Image hitboxImage;
            if (!hitboxTransform.TryGetComponent(out hitboxImage))
            {
                hitboxImage = hitboxTransform.gameObject.AddComponent<Image>();
            }

            hitboxImage.color = new Color(1f, 1f, 1f, 0f);
            hitboxImage.raycastTarget = true;

            if (!hitboxTransform.TryGetComponent(out hitboxForwarder))
            {
                hitboxForwarder = hitboxTransform.gameObject.AddComponent<SignCtrlHoverHitbox>();
            }

            hitboxForwarder.Bind(this);

            if (!createdThisCall && !hitboxInitialized)
            {
                // Existing hitbox means it was already authored before this script version.
                // Mark initialized to avoid overwriting manual adjustments.
                hitboxInitialized = true;
            }

            if (forceRefit || (createdThisCall && autoFitWhenHitboxCreated))
            {
                FitHitboxToGraphics();
                hitboxInitialized = true;
            }
        }

        private void FitHitboxToGraphics()
        {
            if (hitboxRect == null)
            {
                return;
            }

            hitboxRect.anchorMin = new Vector2(0.5f, 0.5f);
            hitboxRect.anchorMax = new Vector2(0.5f, 0.5f);
            hitboxRect.pivot = new Vector2(0.5f, 0.5f);
            hitboxRect.localRotation = Quaternion.identity;
            hitboxRect.localScale = Vector3.one;

            Bounds localBounds;
            if (!TryBuildLocalBounds(out localBounds))
            {
                localBounds = new Bounds(Vector3.zero, new Vector3(100f, 100f, 0f));
            }

            var scaledSize = Vector2.Scale(new Vector2(localBounds.size.x, localBounds.size.y), hitboxSizeScale);
            scaledSize += hitboxPadding * 2f;
            scaledSize.x = Mathf.Max(4f, scaledSize.x);
            scaledSize.y = Mathf.Max(4f, scaledSize.y);

            hitboxRect.anchoredPosition = new Vector2(localBounds.center.x, localBounds.center.y) + hitboxOffset;
            hitboxRect.sizeDelta = scaledSize;
            hitboxRect.SetAsLastSibling();
        }

        private bool TryBuildLocalBounds(out Bounds localBounds)
        {
            bool hasBounds = false;
            localBounds = new Bounds(Vector3.zero, Vector3.zero);

            var graphics = GetComponentsInChildren<Graphic>(includeInactiveGraphics);
            foreach (var graphic in graphics)
            {
                if (graphic == null || !graphic.gameObject.activeInHierarchy && !includeInactiveGraphics)
                {
                    continue;
                }

                if (graphic.transform == hitboxRect)
                {
                    continue;
                }

                var rect = graphic.rectTransform;
                rect.GetWorldCorners(mWorldRectCorners);

                for (int i = 0; i < mWorldRectCorners.Length; i++)
                {
                    Vector3 local = transform.InverseTransformPoint(mWorldRectCorners[i]);
                    if (!hasBounds)
                    {
                        localBounds = new Bounds(local, Vector3.zero);
                        hasBounds = true;
                    }
                    else
                    {
                        localBounds.Encapsulate(local);
                    }
                }
            }

            if (hasBounds)
            {
                return true;
            }

            RectTransform rectTransform;
            if (!TryGetComponent(out rectTransform))
            {
                return false;
            }

            rectTransform.GetWorldCorners(mWorldRectCorners);
            for (int i = 0; i < mWorldRectCorners.Length; i++)
            {
                Vector3 local = transform.InverseTransformPoint(mWorldRectCorners[i]);
                if (!hasBounds)
                {
                    localBounds = new Bounds(local, Vector3.zero);
                    hasBounds = true;
                }
                else
                {
                    localBounds.Encapsulate(local);
                }
            }

            return hasBounds;
        }

        private bool IsPointerStillOverThisCtrl()
        {
            if (EventSystem.current == null || hitboxRect == null)
            {
                return false;
            }

            if (mPointerEventData == null || mCachedEventSystem != EventSystem.current)
            {
                mCachedEventSystem = EventSystem.current;
                mPointerEventData = new PointerEventData(mCachedEventSystem);
            }

            mPointerEventData.Reset();
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current == null)
            {
                return false;
            }

            mPointerEventData.position = Mouse.current.position.ReadValue();
#else
            mPointerEventData.position = Input.mousePosition;
#endif
            mRaycastResults.Clear();
            EventSystem.current.RaycastAll(mPointerEventData, mRaycastResults);

            for (int i = 0; i < mRaycastResults.Count; i++)
            {
                var hitObject = mRaycastResults[i].gameObject;
                if (hitObject != null && hitObject.transform.IsChildOf(transform))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
