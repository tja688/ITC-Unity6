using System.Collections;
using ITC.Dialogue;
using QFramework;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class DialogueSceneFlowLauncher : MonoBehaviour
{
    [Header("Dialogue Startup")]
    [SerializeField] private bool autoOpenDialogueOnStart = true;
    [SerializeField] private float startupDelaySeconds = 0.2f;
    [SerializeField] private string startNode = "ITC_Start";
    [SerializeField] private string dialoguePanelAssetBundleName = "dialogue_ui";
    [SerializeField] private string dialoguePanelPrefabName = "ITC DialogueSystem";

    private bool bootstrapped;
    private bool dialogueOpenRequested;

    private void Awake()
    {
        UIKit.Config = new MainMenuUIKitConfig();
    }

    private IEnumerator Start()
    {
        if (bootstrapped)
        {
            yield break;
        }

        bootstrapped = true;
        yield return ResKit.InitAsync();

        if (!autoOpenDialogueOnStart)
        {
            yield break;
        }

        yield return new WaitForSecondsRealtime(Mathf.Max(0f, startupDelaySeconds));
        if (dialogueOpenRequested)
        {
            yield break;
        }

        dialogueOpenRequested = true;

        UIKit.OpenPanelAsync<ITCDialoguePanel>(
                UILevel.Common,
                new ITCDialoguePanelData
                {
                    StartNode = startNode,
                    AutoStartOnOpen = true
                },
                assetBundleName: dialoguePanelAssetBundleName,
                prefabName: dialoguePanelPrefabName)
            .ToAction()
            .StartGlobal();
    }
}
