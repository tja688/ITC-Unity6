using QFramework;
using UnityEngine;

public sealed class MainMenuPanel : UIPanel
{
    private const string StartButtonKeyword = "开始新游戏";

    protected override void OnInit(IUIData uiData = null)
    {
        BindStartButtons();
    }

    protected override void OnOpen(IUIData uiData = null)
    {
        BindStartButtons();
    }

    protected override void OnClose()
    {
    }

    private void BindStartButtons()
    {
        if (Transform == null)
        {
            LogKit.W("[MainMenuPanel] Transform is null, skip start-button binding.");
            return;
        }

        var colliders = Transform.GetComponentsInChildren<Collider2D>(true);
        var boundCount = 0;

        foreach (var collider in colliders)
        {
            if (collider == null)
            {
                continue;
            }

            var target = collider.gameObject;
            if (!target.name.Contains(StartButtonKeyword))
            {
                continue;
            }

            if (target.GetComponent<MainMenuStartButton>() == null)
            {
                target.AddComponent<MainMenuStartButton>();
                boundCount++;
            }
        }

        LogKit.I($"[MainMenuPanel] Start-button binding finished. Added {boundCount} component(s).");
    }
}
