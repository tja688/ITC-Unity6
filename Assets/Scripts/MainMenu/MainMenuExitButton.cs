using QFramework;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class MainMenuExitButton : MonoBehaviour, IController
{
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float hoverLerpSpeed = 12f;
    [SerializeField] private bool enableHoverFeedback = true;
    [SerializeField] private bool quitOnClick = true;
    [SerializeField] private string exitNameKeyword = "退出";
    [SerializeField] private string startGameNameKeyword = "开始新游戏";
    [SerializeField] private bool openDialogueOnStartClick = true;

    private Vector3 baseScale;
    private bool isHovered;
    private bool canQuitOnClick;
    private bool canOpenDialogueOnClick;

    private void Awake()
    {
        baseScale = transform.localScale;
        canQuitOnClick = quitOnClick &&
                         (string.IsNullOrEmpty(exitNameKeyword) || gameObject.name.Contains(exitNameKeyword));
        canOpenDialogueOnClick = openDialogueOnStartClick &&
                                 !string.IsNullOrEmpty(startGameNameKeyword) &&
                                 gameObject.name.Contains(startGameNameKeyword);
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
        if (canQuitOnClick)
        {
            Quit();
            return;
        }

        if (canOpenDialogueOnClick)
        {
            this.SendCommand<RequestOpenDialoguePanelCommand>();
        }
    }

    public IArchitecture GetArchitecture()
    {
        return MainMenuApp.Interface;
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
