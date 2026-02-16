using System.IO;
using QFramework;
using UnityEditor;
using UnityEngine;

namespace ITC.Editor
{
    public static class ITCDialogueResKitBuildTools
    {
        private const string DialoguePrefabPath = "Assets/Prefabs/UI/ITC DialogueSystem.prefab";
        private const string DialogueYarnPath = "Assets/Doc/ITC Doc/dialogue/ITC_YARN_DATA_TEXT_ANIM.yarn";
        private const string DialogueYarnProjectPath = "Assets/Doc/ITC Doc/dialogue/TestProject.yarnproject";
        private const string BackgroundFolderPath = "Assets/Arts/Texture2d图片/占位背景原画";
        private const string PortraitFolderPath = "Assets/Arts/Texture2d图片/占位人物立绘";

        [MenuItem("Tools/ITC/Dialogue/Apply AssetBundle Labels")]
        public static void ApplyDialogueAssetBundleLabels()
        {
            var changed = false;
            changed |= SetBundleName(DialoguePrefabPath, "dialogue_ui");
            changed |= SetBundleName(DialogueYarnPath, "dialogue_script");
            changed |= SetBundleName(DialogueYarnProjectPath, "dialogue_script");
            changed |= SetBundleNameForFolderPng(BackgroundFolderPath, "dialogue_bg");
            changed |= SetBundleNameForFolderPng(PortraitFolderPath, "dialogue_portrait");
            changed |= ITCDialogueVisualCatalogTools.ApplyCatalogAssetBundleLabels();

            if (changed)
            {
                AssetDatabase.RemoveUnusedAssetBundleNames();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Tools/ITC/Dialogue/Build WebGL ResKit Bundles")]
        public static void BuildWebGLResKitBundles()
        {
            ApplyDialogueAssetBundleLabels();

            if (!ITCDialogueVisualCatalogTools.ValidateVisualCatalog(logResults: true))
            {
                Debug.LogError("[ITCDialogueResKitBuildTools] Build canceled because DialogueVisualCatalog validation failed.");
                return;
            }

            EditorPrefs.SetBool(ResKitView.KEY_AUTOGENERATE_CLASS, false);

            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                var switched = EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
                if (!switched)
                {
                    Debug.LogError("[ITCDialogueResKitBuildTools] Failed to switch Build Target to WebGL.");
                    return;
                }
            }

            ResKitEditorAPI.BuildAssetBundles();
            AssetBundlePathHelper.SimulationMode = true;
            AssetDatabase.Refresh();
            Debug.Log("[ITCDialogueResKitBuildTools] WebGL ResKit bundle build finished.");
        }

        private static bool SetBundleName(string assetPath, string bundleName)
        {
            var importer = AssetImporter.GetAtPath(assetPath);
            if (importer == null)
            {
                return false;
            }

            if (importer.assetBundleName == bundleName)
            {
                return false;
            }

            importer.assetBundleName = bundleName;
            return true;
        }

        private static bool SetBundleNameForFolderPng(string folderPath, string bundleName)
        {
            if (!Directory.Exists(folderPath))
            {
                return false;
            }

            var changed = false;
            var pngFiles = Directory.GetFiles(folderPath, "*.png", SearchOption.TopDirectoryOnly);
            foreach (var pngFile in pngFiles)
            {
                var normalized = pngFile.Replace("\\", "/");
                if (!normalized.StartsWith("Assets/"))
                {
                    continue;
                }

                changed |= SetBundleName(normalized, bundleName);
            }

            return changed;
        }
    }
}
