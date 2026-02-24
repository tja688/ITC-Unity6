using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public sealed class ReplicaPointerRelay : UIBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerMoveHandler,
    IDragHandler,
    IInitializePotentialDragHandler,
    IReplicaPointerSource
{
    [SerializeField] private bool mFallbackToMouse = true;

    private Vector2 mScreenPosition;
    private bool mHasPointer;

    public Vector2 ScreenPosition
    {
        get
        {
            if (mHasPointer)
            {
                return mScreenPosition;
            }

            return mFallbackToMouse ? (Vector2)Input.mousePosition : Vector2.zero;
        }
    }

    public bool IsPointerValid => mHasPointer || mFallbackToMouse;

    public void SetFallbackToMouse(bool enabled)
    {
        mFallbackToMouse = enabled;
    }

    protected override void Awake()
    {
        base.Awake();

        var image = ReplicaUIFactoryV2.EnsureComponent<Image>(gameObject);
        image.color = new Color(1f, 1f, 1f, 0.001f);
        image.raycastTarget = true;
    }

    public bool TryGetLocalPoint(RectTransform target, out Vector2 localPoint, Camera eventCamera = null)
    {
        if (target == null)
        {
            localPoint = Vector2.zero;
            return false;
        }

        if (eventCamera == null)
        {
            var canvas = target.GetComponentInParent<Canvas>();
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                eventCamera = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            }
        }

        return RectTransformUtility.ScreenPointToLocalPointInRectangle(target, ScreenPosition, eventCamera, out localPoint);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdatePointer(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData != null)
        {
            mScreenPosition = eventData.position;
        }

        mHasPointer = false;
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        UpdatePointer(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        UpdatePointer(eventData);
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        UpdatePointer(eventData);
    }

    private void UpdatePointer(PointerEventData eventData)
    {
        if (eventData == null)
        {
            return;
        }

        mScreenPosition = eventData.position;
        mHasPointer = true;
    }

    protected override void OnDisable()
    {
        mHasPointer = false;
        base.OnDisable();
    }
}
