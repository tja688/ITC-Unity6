using UnityEngine;

public class MoveToOriginFixed : MonoBehaviour
{
    private void FixedUpdate()
    {
        transform.position = Vector3.zero;
    }
}
