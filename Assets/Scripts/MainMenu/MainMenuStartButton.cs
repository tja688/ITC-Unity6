using QFramework;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class MainMenuStartButton : MonoBehaviour, IController
{
    [SerializeField] private string startNameKeyword = "开始新游戏";
    [SerializeField] private bool disableColliderAfterClick = true;

    private bool isEnabledByName;
    private bool clickRequested;

    private void Awake()
    {
        isEnabledByName = string.IsNullOrEmpty(startNameKeyword) || gameObject.name.Contains(startNameKeyword);
        LogKit.I($"[MainMenuStartButton] Awake on '{gameObject.name}', enabledByName={isEnabledByName}.");
    }

    private void OnMouseUpAsButton()
    {
        if (!isEnabledByName)
        {
            return;
        }

        if (clickRequested)
        {
            return;
        }

        clickRequested = true;

        if (disableColliderAfterClick)
        {
            var collider2D = GetComponent<Collider2D>();
            if (collider2D != null)
            {
                collider2D.enabled = false;
            }
        }

        LogKit.I($"[MainMenuStartButton] Click accepted on '{gameObject.name}', requesting DialogueScene load.");
        this.SendCommand<RequestEnterDialogueSceneCommand>();
    }

    public IArchitecture GetArchitecture()
    {
        return MainMenuApp.Interface;
    }
}
