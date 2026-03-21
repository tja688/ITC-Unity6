using UnityEngine;

public sealed class MouseRaycastNameDisplay : MonoBehaviour
{
    [SerializeField] private Camera mCamera;
    [SerializeField] private float mMaxDistance = 100f;
    [SerializeField] private LayerMask mLayerMask = ~0;

    private string mDisplayText = "Hit: None";

    private void Awake()
    {
        if (mCamera == null)
        {
            mCamera = Camera.main;
        }

        if (mCamera == null)
        {
            Debug.LogError("MouseRaycastNameDisplay needs a Camera reference.", this);
        }
    }

    private void Update()
    {
        if (mCamera == null)
        {
            return;
        }

        Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, mMaxDistance, mLayerMask, QueryTriggerInteraction.Ignore))
        {
            mDisplayText = $"Hit: {hit.collider.gameObject.name}";
        }
        else
        {
            mDisplayText = "Hit: None";
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(8f, 8f, 400f, 24f), mDisplayText);
    }
}
