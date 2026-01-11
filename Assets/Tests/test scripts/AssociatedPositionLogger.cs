using UnityEngine;

namespace ITC.Unity.Tests
{
    /// <summary>
    /// Logs the world position of the associated transform each frame to help inspect motion during play mode.
    /// </summary>
    public class AssociatedPositionLogger : MonoBehaviour
    {
        [SerializeField]
        private Transform associatedTransform;

        private void Reset()
        {
            associatedTransform = transform;
        }

        private void Update()
        {
            if (associatedTransform == null)
            {
                Debug.LogWarning("[AssociatedPositionLogger] No transform assigned to log.", gameObject);
                return;
            }

            Debug.Log(
                $"[AssociatedPositionLogger] {associatedTransform.name} position: {associatedTransform.position}",
                associatedTransform);
        }
    }
}
