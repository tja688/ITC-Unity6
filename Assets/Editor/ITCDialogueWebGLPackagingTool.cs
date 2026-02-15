using System.Collections.Generic;
using QFramework;
using UnityEditor;
using UnityEngine;

public static class ITCDialogueWebGLPackagingTool
{
    [MenuItem("ITC/Dialogue/Apply WebGL Simulation Packaging", priority = 3100)]
    public static void ApplyWebGLSimulationPackaging()
    {
        var assignments = new Dictionary<string, string>
        {
            ["Assets/Scenes/MainMenu.unity"] = "entry_flow",
            ["Assets/Scenes/DialogueScene.unity"] = ITC.Dialogue.ITCDialogueResPaths.DialogueSceneBundle,
            ["Assets/Prefabs/UI/ITC DialogueSystem.prefab"] = ITC.Dialogue.ITCDialogueResPaths.DialogueSystemBundle,
            ["Assets/Doc/ITC Doc/dialogue/TestProject.yarnproject"] = ITC.Dialogue.ITCDialogueResPaths.DialogueTextBundle,
            ["Assets/Doc/ITC Doc/dialogue/ITC_YARN_DATA_TEXT_ANIM.yarn"] = ITC.Dialogue.ITCDialogueResPaths.DialogueTextBundle,
            ["Assets/Arts/Texture2d图片/占位背景原画/13号窗口 (外部反打).png"] = ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
            ["Assets/Arts/Texture2d图片/占位背景原画/地狱旅馆-客房.png"] = ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
            ["Assets/Arts/Texture2d图片/占位背景原画/地狱旅馆-浴室镜前.png"] = ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
            ["Assets/Arts/Texture2d图片/占位背景原画/火焰之门.png"] = ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
            ["Assets/Arts/Texture2d图片/占位背景原画/圣纽约市-中心全景.png"] = ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
            ["Assets/Arts/Texture2d图片/占位背景原画/Henet办公室.png"] = ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
            ["Assets/Arts/Texture2d图片/占位背景原画/ITC办公区.png"] = ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
            ["Assets/Arts/Texture2d图片/占位背景原画/ITC大楼-外观.png"] = ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
            ["Assets/Arts/Texture2d图片/占位背景原画/ITC大厅.png"] = ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
            ["Assets/Arts/Texture2d图片/占位背景原画/ITC电梯内部.png"] = ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
            ["Assets/Arts/Texture2d图片/占位背景原画/ITC门卫亭.png"] = ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
            ["Assets/Arts/Texture2d图片/占位背景原画/主菜单.png"] = ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
            ["Assets/Arts/Texture2d图片/占位人物立绘/通用标准屏幕中心立绘.png"] = ITC.Dialogue.ITCDialogueResPaths.DialoguePortraitBundle,
            ["Assets/Arts/Texture2d图片/占位人物立绘/通用标准人物头像.png"] = ITC.Dialogue.ITCDialogueResPaths.DialoguePortraitBundle,
            ["Assets/Arts/Texture2d图片/占位人物立绘/头像框素材.png"] = ITC.Dialogue.ITCDialogueResPaths.DialoguePortraitBundle,
        };

        foreach (var pair in assignments)
        {
            var importer = AssetImporter.GetAtPath(pair.Key);
            if (importer == null)
            {
                Debug.LogWarning($"[ITCDialogueWebGLPackagingTool] Missing asset: {pair.Key}");
                continue;
            }

            importer.assetBundleName = pair.Value;
            importer.assetBundleVariant = string.Empty;
        }

        ApplyBuildScenes();

        AssetDatabase.RemoveUnusedAssetBundleNames();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        AssetBundleExporter.BuildDataTable(
            new[]
            {
                "menu_core",
                "menu_fx",
                "entry_flow",
                ITC.Dialogue.ITCDialogueResPaths.DialogueSceneBundle,
                ITC.Dialogue.ITCDialogueResPaths.DialogueSystemBundle,
                ITC.Dialogue.ITCDialogueResPaths.DialogueBackgroundBundle,
                ITC.Dialogue.ITCDialogueResPaths.DialoguePortraitBundle,
                ITC.Dialogue.ITCDialogueResPaths.DialogueTextBundle
            });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[ITCDialogueWebGLPackagingTool] WebGL simulation packaging applied.");
    }

    private static void ApplyBuildScenes()
    {
        var scenes = new[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/DialogueScene.unity", true),
            new EditorBuildSettingsScene("Assets/Tests/Test scene/TestScene.unity", true)
        };

        EditorBuildSettings.scenes = scenes;
    }
}
