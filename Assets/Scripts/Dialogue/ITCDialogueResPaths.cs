using QFramework;

namespace ITC.Dialogue
{
    public static class ITCDialogueResPaths
    {
        public const string MainMenuSceneName = "MainMenu";
        public const string DialogueSceneName = "DialogueScene";

        public const string DialogueSceneBundle = "dialogue_flow";
        public const string DialogueSystemBundle = "dialogue_core";
        public const string DialogueSystemPrefabName = "ITC DialogueSystem";
        public const string DialogueBackgroundBundle = "dialogue_art_bg";
        public const string DialoguePortraitBundle = "dialogue_art_portrait";
        public const string DialogueTextBundle = "dialogue_text";

        public static bool HasAssetEntry(string bundleName, string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                return false;
            }

            var searchKeys = string.IsNullOrEmpty(bundleName)
                ? ResSearchKeys.Allocate(assetName)
                : ResSearchKeys.Allocate(assetName, bundleName);

            var assetData = AssetBundleSettings.AssetBundleConfigFile.GetAssetData(searchKeys);
            searchKeys.Recycle2Cache();
            return assetData != null;
        }

        public static bool HasSceneAssetEntry(string bundleName, string sceneName)
        {
            if (!HasAssetEntry(bundleName, sceneName))
            {
                return false;
            }

            var searchKeys = string.IsNullOrEmpty(bundleName)
                ? ResSearchKeys.Allocate(sceneName)
                : ResSearchKeys.Allocate(sceneName, bundleName);

            var assetData = AssetBundleSettings.AssetBundleConfigFile.GetAssetData(searchKeys);
            searchKeys.Recycle2Cache();
            return assetData != null && assetData.AssetType == ResLoadType.ABScene;
        }
    }
}
