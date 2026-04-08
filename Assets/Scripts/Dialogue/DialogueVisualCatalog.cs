using System;
using System.Collections.Generic;
using UnityEngine;

namespace ITC.Dialogue
{
    public enum DialogueVisualSlot
    {
        Background = 0,
        NpcMain = 1,
        NpcAvatar = 2,
        PcAvatar = 3
    }

    [Serializable]
    public sealed class DialogueSpriteRef
    {
        public string key;
        public DialogueVisualSlot slot = DialogueVisualSlot.Background;
        public Sprite directSprite;
        public string assetName;
        public string subSpriteName;
        public string assetBundleName;
        public string fallbackKey;
#if UNITY_EDITOR
        public string editorAssetPath;
#endif
    }

    [CreateAssetMenu(
        fileName = "DialogueVisualCatalog",
        menuName = "ITC/Dialogue/Visual Catalog")]
    public sealed class DialogueVisualCatalog : ScriptableObject
    {
        [Header("Default Bundles")]
        [SerializeField] private string defaultBackgroundBundle = "dialogue_bg";
        [SerializeField] private string defaultPortraitBundle = "dialogue_portrait";

        [Header("Slot Default Keys")]
        [SerializeField] private string npcMainPortraitDefaultKey = "barks_default";
        [SerializeField] private string npcAvatarDefaultKey = "barks_default";
        [SerializeField] private string pcAvatarDefaultKey = "barks_default";

        [Header("Missing Sprite Fallback")]
        [SerializeField] private Sprite missingSprite;
        [SerializeField] private string missingSpriteBundle = "dialogue_portrait";
        [SerializeField] private string missingSpriteAssetName = "image missing";
        [SerializeField] private string missingSpriteSubSpriteName = "image missing_0";
#if UNITY_EDITOR
        [SerializeField] private string missingSpriteEditorAssetPath = "Assets/Arts/Texture2d图片/image missing.png";
#endif

        [Header("Visual Mappings")]
        [SerializeField] private List<DialogueSpriteRef> mappings = new();

        public string DefaultBackgroundBundle => defaultBackgroundBundle;
        public string DefaultPortraitBundle => defaultPortraitBundle;
        public string NpcMainPortraitDefaultKey => npcMainPortraitDefaultKey;
        public string NpcAvatarDefaultKey => npcAvatarDefaultKey;
        public string PcAvatarDefaultKey => pcAvatarDefaultKey;
        public Sprite MissingSprite => missingSprite;
        public string MissingSpriteBundle => missingSpriteBundle;
        public string MissingSpriteAssetName => missingSpriteAssetName;
        public string MissingSpriteSubSpriteName => missingSpriteSubSpriteName;
#if UNITY_EDITOR
        public string MissingSpriteEditorAssetPath => missingSpriteEditorAssetPath;
#endif
        public IReadOnlyList<DialogueSpriteRef> Mappings => mappings;
#if UNITY_EDITOR
        public List<DialogueSpriteRef> MutableMappings => mappings;
#endif

        public string GetDefaultBundle(DialogueVisualSlot slot)
        {
            return slot == DialogueVisualSlot.Background ? defaultBackgroundBundle : defaultPortraitBundle;
        }

        public string GetDefaultKey(DialogueVisualSlot slot)
        {
            return slot switch
            {
                DialogueVisualSlot.NpcMain => npcMainPortraitDefaultKey,
                DialogueVisualSlot.NpcAvatar => npcAvatarDefaultKey,
                DialogueVisualSlot.PcAvatar => pcAvatarDefaultKey,
                _ => null
            };
        }

        public bool TryGetMapping(DialogueVisualSlot slot, string key, out DialogueSpriteRef mapping)
        {
            mapping = null;
            if (string.IsNullOrWhiteSpace(key) || mappings == null)
            {
                return false;
            }

            var trimmedKey = key.Trim();
            for (var i = 0; i < mappings.Count; i++)
            {
                var candidate = mappings[i];
                if (candidate == null || candidate.slot != slot)
                {
                    continue;
                }

                if (string.Equals(candidate.key, trimmedKey, StringComparison.OrdinalIgnoreCase))
                {
                    mapping = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}
