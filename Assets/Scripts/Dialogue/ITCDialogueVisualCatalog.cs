using System;
using System.Collections.Generic;

namespace ITC.Dialogue
{
    public readonly struct DialogueSpriteAddress
    {
        public readonly string BundleName;
        public readonly string AssetName;

        public DialogueSpriteAddress(string bundleName, string assetName)
        {
            BundleName = bundleName;
            AssetName = assetName;
        }
    }

    public static class ITCDialogueVisualCatalog
    {
        private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

        private static readonly Dictionary<string, DialogueSpriteAddress> Backgrounds =
            new Dictionary<string, DialogueSpriteAddress>(Comparer)
            {
                ["main_menu"] = new(ITCDialogueResPaths.DialogueBackgroundBundle, "主菜单1_0"),
                ["hotel_room"] = new(ITCDialogueResPaths.DialogueBackgroundBundle, "2_0"),
                ["hotel_bathroom"] = new(ITCDialogueResPaths.DialogueBackgroundBundle, "地狱旅馆-浴室镜前_0"),
                ["flame_gate"] = new(ITCDialogueResPaths.DialogueBackgroundBundle, "火焰之门_0"),
                ["city_center"] = new(ITCDialogueResPaths.DialogueBackgroundBundle, "圣纽约市-中心全景_0"),
                ["itc_building"] = new(ITCDialogueResPaths.DialogueBackgroundBundle, "ITC大楼-外观 1_0"),
                ["itc_guard"] = new(ITCDialogueResPaths.DialogueBackgroundBundle, "ITC门卫亭_0"),
                ["itc_lobby"] = new(ITCDialogueResPaths.DialogueBackgroundBundle, "ITC大厅_0"),
                ["itc_elevator"] = new(ITCDialogueResPaths.DialogueBackgroundBundle, "ITC电梯内部_0"),
                ["henet_office"] = new(ITCDialogueResPaths.DialogueBackgroundBundle, "Henet办公室_0"),
                ["itc_office"] = new(ITCDialogueResPaths.DialogueBackgroundBundle, "ITC办公区_0"),
                ["window13"] = new(ITCDialogueResPaths.DialogueBackgroundBundle, "13号窗口 (外部反打)_0"),
            };

        private static readonly Dictionary<string, DialogueSpriteAddress> Portraits =
            new Dictionary<string, DialogueSpriteAddress>(Comparer)
            {
                ["barks"] = new(ITCDialogueResPaths.DialoguePortraitBundle, "通用标准屏幕中心立绘"),
                ["skeleton_bellboy"] = new(ITCDialogueResPaths.DialoguePortraitBundle, "通用标准屏幕中心立绘"),
                ["oldguard"] = new(ITCDialogueResPaths.DialoguePortraitBundle, "通用标准屏幕中心立绘"),
                ["receptionist"] = new(ITCDialogueResPaths.DialoguePortraitBundle, "通用标准屏幕中心立绘"),
                ["oldtom"] = new(ITCDialogueResPaths.DialoguePortraitBundle, "通用标准屏幕中心立绘"),
                ["henet"] = new(ITCDialogueResPaths.DialoguePortraitBundle, "通用标准屏幕中心立绘"),
                ["oldclerk13"] = new(ITCDialogueResPaths.DialoguePortraitBundle, "通用标准屏幕中心立绘"),
                ["veer"] = new(ITCDialogueResPaths.DialoguePortraitBundle, "通用标准屏幕中心立绘"),
                ["emmett"] = new(ITCDialogueResPaths.DialoguePortraitBundle, "通用标准屏幕中心立绘"),
                ["thomas"] = new(ITCDialogueResPaths.DialoguePortraitBundle, "通用标准屏幕中心立绘"),
                ["narrator"] = new(ITCDialogueResPaths.DialoguePortraitBundle, "通用标准屏幕中心立绘"),
            };

        public static bool TryGetBackground(string key, out DialogueSpriteAddress address)
        {
            return Backgrounds.TryGetValue(key, out address);
        }

        public static bool TryGetPortrait(string key, out DialogueSpriteAddress address)
        {
            return Portraits.TryGetValue(key, out address);
        }
    }
}
