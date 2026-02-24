using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public sealed class UGUIReactBitsReplicaShowcase : MonoBehaviour
{
    private void Awake()
    {
        EnsureV2Component();
        enabled = false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!Application.isPlaying)
        {
            EnsureV2Component();
        }
    }
#endif

    private void EnsureV2Component()
    {
        if (!TryGetComponent<UGUIReactBitsReplicaShowcase_V2>(out _))
        {
            gameObject.AddComponent<UGUIReactBitsReplicaShowcase_V2>();
        }
    }
}
