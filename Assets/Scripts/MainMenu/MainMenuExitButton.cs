using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class MainMenuExitButton : MonoBehaviour
{
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float hoverLerpSpeed = 12f;
    [SerializeField] private bool enableHoverFeedback = true;

    private Vector3 baseScale;
    private bool isHovered;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void Update()
    {
        if (!enableHoverFeedback)
        {
            return;
        }

        var targetScale = isHovered ? baseScale * hoverScale : baseScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * hoverLerpSpeed);
    }

    private void OnMouseEnter()
    {
        isHovered = true;
    }

    private void OnMouseExit()
    {
        isHovered = false;
    }

    private void OnMouseUpAsButton()
    {
        Quit();
    }

    private static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
