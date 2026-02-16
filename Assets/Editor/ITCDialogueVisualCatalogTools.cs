using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ITC.Dialogue;
using UnityEditor;
using UnityEngine;

namespace ITC.Editor
{
    public static class ITCDialogueVisualCatalogTools
    {
        private const string DefaultCatalogPath = "Assets/Resources/Dialogue/DialogueVisualCatalog.asset";
        private const string YarnPath = "Assets/Doc/ITC Doc/dialogue/ITC_YARN_DATA_TEXT_ANIM.yarn";

        private static readonly Regex YarnKeyCommandRegex = new(
            @"<<\s*(itc_bg|itc_npc_main|itc_npc_avatar|itc_pc_avatar)\s+([^\s>]+)\s*>>",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        [MenuItem("Tools/ITC/Dialogue/Validate Visual Catalog")]
        public static void ValidateVisualCatalogMenu()
        {
            var passed = ValidateVisualCatalog(true);
            if (passed)
            {
                Debug.Log("[ITCDialogueVisualCatalogTools] Visual catalog validation passed.");
            }
        }

        [MenuItem("Tools/ITC/Dialogue/Sync Keys From Yarn")]
        public static void SyncKeysFromYarnMenu()
        {
            var catalog = LoadCatalogAsset(out _);
            if (catalog == null)
            {
                Debug.LogError("[ITCDialogueVisualCatalogTools] Cannot sync keys because DialogueVisualCatalog was not found.");
                return;
            }

            var yarnRefs = CollectYarnReferences(YarnPath);
            var changed = false;
            foreach (var pair in yarnRefs)
            {
                var slot = pair.Key;
                foreach (var key in pair.Value)
                {
                    if (catalog.TryGetMapping(slot, key, out _))
                    {
                        continue;
                    }

                    catalog.MutableMappings.Add(CreatePlaceholder(catalog, slot, key));
                    Debug.Log($"[ITCDialogueVisualCatalogTools] Added placeholder mapping: slot={slot}, key={key}");
                    changed = true;
                }
            }

            if (!changed)
            {
                Debug.Log("[ITCDialogueVisualCatalogTools] Sync complete. No missing keys.");
                return;
            }

            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ITCDialogueVisualCatalogTools] Sync complete. Catalog updated.");
        }

        public static bool ApplyCatalogAssetBundleLabels()
        {
            var catalog = LoadCatalogAsset(out _);
            if (catalog == null)
            {
                Debug.LogError("[ITCDialogueVisualCatalogTools] DialogueVisualCatalog not found. Cannot apply labels from catalog.");
                return false;
            }

            var changed = false;
            if (catalog.Mappings != null)
            {
                foreach (var mapping in catalog.Mappings)
                {
                    if (mapping == null || string.IsNullOrWhiteSpace(mapping.editorAssetPath))
                    {
                        continue;
                    }

                    var bundleName = string.IsNullOrWhiteSpace(mapping.assetBundleName)
                        ? catalog.GetDefaultBundle(mapping.slot)
                        : mapping.assetBundleName.Trim();

                    if (string.IsNullOrWhiteSpace(bundleName))
                    {
                        continue;
                    }

                    changed |= SetBundleName(mapping.editorAssetPath, bundleName);
                }
            }

            if (!string.IsNullOrWhiteSpace(catalog.MissingSpriteEditorAssetPath))
            {
                var missingBundle = string.IsNullOrWhiteSpace(catalog.MissingSpriteBundle)
                    ? catalog.DefaultPortraitBundle
                    : catalog.MissingSpriteBundle.Trim();
                changed |= SetBundleName(catalog.MissingSpriteEditorAssetPath, missingBundle);
            }

            return changed;
        }

        public static bool ValidateVisualCatalog(bool logResults)
        {
            var hasErrors = false;
            var catalog = LoadCatalogAsset(out var catalogPath);
            if (catalog == null)
            {
                if (logResults)
                {
                    Debug.LogError("[ITCDialogueVisualCatalogTools] DialogueVisualCatalog asset not found.");
                }

                return false;
            }

            var mappings = catalog.Mappings ?? Array.Empty<DialogueSpriteRef>();
            var uniqueCheck = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var lookup = new Dictionary<string, DialogueSpriteRef>(StringComparer.OrdinalIgnoreCase);

            foreach (var mapping in mappings)
            {
                if (mapping == null)
                {
                    hasErrors |= LogError(logResults, "[ITCDialogueVisualCatalogTools] Null mapping entry found.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(mapping.key))
                {
                    hasErrors |= LogError(logResults, "[ITCDialogueVisualCatalogTools] Mapping has empty key.");
                    continue;
                }

                if (string.IsNullOrWhiteSpace(mapping.assetName))
                {
                    hasErrors |= LogError(logResults, $"[ITCDialogueVisualCatalogTools] Mapping '{mapping.slot}:{mapping.key}' has empty assetName.");
                }

                var compositeKey = ComposeKey(mapping.slot, mapping.key);
                if (!uniqueCheck.Add(compositeKey))
                {
                    hasErrors |= LogError(logResults, $"[ITCDialogueVisualCatalogTools] Duplicate mapping key+slot: {mapping.slot}:{mapping.key}");
                }

                lookup[compositeKey] = mapping;
            }

            ValidateFallbackChains(catalog, lookup, ref hasErrors, logResults);
            ValidateAssetReferences(catalog, ref hasErrors, logResults);
            ValidateYarnCoverage(catalog, lookup, ref hasErrors, logResults);
            ValidateMissingSprite(catalog, ref hasErrors, logResults);

            if (!hasErrors && logResults)
            {
                Debug.Log($"[ITCDialogueVisualCatalogTools] Validation passed: {catalogPath}");
            }

            return !hasErrors;
        }

        private static void ValidateFallbackChains(
            DialogueVisualCatalog catalog,
            IReadOnlyDictionary<string, DialogueSpriteRef> lookup,
            ref bool hasErrors,
            bool logResults)
        {
            foreach (var mapping in catalog.Mappings)
            {
                if (mapping == null || string.IsNullOrWhiteSpace(mapping.key))
                {
                    continue;
                }

                var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var cursor = mapping.fallbackKey;
                while (!string.IsNullOrWhiteSpace(cursor))
                {
                    cursor = cursor.Trim();
                    if (!visited.Add(cursor))
                    {
                        hasErrors |= LogError(logResults,
                            $"[ITCDialogueVisualCatalogTools] Fallback cycle detected at {mapping.slot}:{mapping.key} -> {cursor}");
                        break;
                    }

                    if (!lookup.TryGetValue(ComposeKey(mapping.slot, cursor), out var fallbackMapping))
                    {
                        hasErrors |= LogError(logResults,
                            $"[ITCDialogueVisualCatalogTools] Missing fallback target: {mapping.slot}:{mapping.key} -> {cursor}");
                        break;
                    }

                    cursor = fallbackMapping.fallbackKey;
                }
            }
        }

        private static void ValidateAssetReferences(
            DialogueVisualCatalog catalog,
            ref bool hasErrors,
            bool logResults)
        {
            foreach (var mapping in catalog.Mappings)
            {
                if (mapping == null || string.IsNullOrWhiteSpace(mapping.assetName))
                {
                    continue;
                }

                var resolvedBundle = string.IsNullOrWhiteSpace(mapping.assetBundleName)
                    ? catalog.GetDefaultBundle(mapping.slot)
                    : mapping.assetBundleName.Trim();

                var assetPath = ResolveAssetPath(mapping, resolvedBundle);
                if (string.IsNullOrWhiteSpace(assetPath))
                {
                    hasErrors |= LogError(logResults,
                        $"[ITCDialogueVisualCatalogTools] Asset not found for mapping {mapping.slot}:{mapping.key} (bundle={resolvedBundle}, asset={mapping.assetName}).");
                    continue;
                }

                var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                if (textureImporter == null || textureImporter.textureType != TextureImporterType.Sprite)
                {
                    hasErrors |= LogError(logResults,
                        $"[ITCDialogueVisualCatalogTools] Asset is not Sprite texture: {assetPath} (mapping {mapping.slot}:{mapping.key}).");
                    continue;
                }

                var allSprites = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();
                var hasMultiple = allSprites.Length > 1;
                if (hasMultiple && string.IsNullOrWhiteSpace(mapping.subSpriteName))
                {
                    hasErrors |= LogError(logResults,
                        $"[ITCDialogueVisualCatalogTools] Multiple Sprite requires subSpriteName: {mapping.slot}:{mapping.key} @ {assetPath}");
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(mapping.subSpriteName) &&
                    allSprites.All(s => !string.Equals(s.name, mapping.subSpriteName.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    hasErrors |= LogError(logResults,
                        $"[ITCDialogueVisualCatalogTools] subSpriteName not found: {mapping.slot}:{mapping.key} -> {mapping.subSpriteName} @ {assetPath}");
                }
            }
        }

        private static void ValidateYarnCoverage(
            DialogueVisualCatalog catalog,
            IReadOnlyDictionary<string, DialogueSpriteRef> lookup,
            ref bool hasErrors,
            bool logResults)
        {
            var yarnRefs = CollectYarnReferences(YarnPath);
            foreach (var pair in yarnRefs)
            {
                var slot = pair.Key;
                foreach (var key in pair.Value)
                {
                    if (!lookup.ContainsKey(ComposeKey(slot, key)))
                    {
                        hasErrors |= LogError(logResults,
                            $"[ITCDialogueVisualCatalogTools] Yarn key missing in catalog: commandSlot={slot}, key={key}");
                    }
                }
            }

            foreach (var slot in new[] { DialogueVisualSlot.NpcMain, DialogueVisualSlot.NpcAvatar, DialogueVisualSlot.PcAvatar })
            {
                var defaultKey = catalog.GetDefaultKey(slot);
                if (string.IsNullOrWhiteSpace(defaultKey))
                {
                    hasErrors |= LogError(logResults, $"[ITCDialogueVisualCatalogTools] Default key is empty for slot {slot}.");
                    continue;
                }

                if (!lookup.ContainsKey(ComposeKey(slot, defaultKey)))
                {
                    hasErrors |= LogError(logResults,
                        $"[ITCDialogueVisualCatalogTools] Default key missing mapping for slot {slot}: {defaultKey}");
                }
            }
        }

        private static void ValidateMissingSprite(
            DialogueVisualCatalog catalog,
            ref bool hasErrors,
            bool logResults)
        {
            if (string.IsNullOrWhiteSpace(catalog.MissingSpriteAssetName))
            {
                hasErrors |= LogError(logResults, "[ITCDialogueVisualCatalogTools] MissingSpriteAssetName is empty.");
                return;
            }

            var pseudoMapping = new DialogueSpriteRef
            {
                slot = DialogueVisualSlot.NpcAvatar,
                assetName = catalog.MissingSpriteAssetName,
                subSpriteName = catalog.MissingSpriteSubSpriteName,
                assetBundleName = catalog.MissingSpriteBundle,
#if UNITY_EDITOR
                editorAssetPath = catalog.MissingSpriteEditorAssetPath
#endif
            };

            var resolvedBundle = string.IsNullOrWhiteSpace(catalog.MissingSpriteBundle)
                ? catalog.DefaultPortraitBundle
                : catalog.MissingSpriteBundle.Trim();

            var assetPath = ResolveAssetPath(pseudoMapping, resolvedBundle);
            if (string.IsNullOrWhiteSpace(assetPath))
            {
                hasErrors |= LogError(logResults,
                    $"[ITCDialogueVisualCatalogTools] Missing sprite asset not found (bundle={resolvedBundle}, asset={catalog.MissingSpriteAssetName}).");
                return;
            }

            var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();
            if (!string.IsNullOrWhiteSpace(catalog.MissingSpriteSubSpriteName) &&
                sprites.All(s => !string.Equals(s.name, catalog.MissingSpriteSubSpriteName.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                hasErrors |= LogError(logResults,
                    $"[ITCDialogueVisualCatalogTools] Missing sprite subSpriteName not found: {catalog.MissingSpriteSubSpriteName} @ {assetPath}");
            }
        }

        private static DialogueVisualCatalog LoadCatalogAsset(out string assetPath)
        {
            assetPath = null;

            if (AssetDatabase.LoadAssetAtPath<DialogueVisualCatalog>(DefaultCatalogPath) is { } direct)
            {
                assetPath = DefaultCatalogPath;
                return direct;
            }

            var guids = AssetDatabase.FindAssets("t:DialogueVisualCatalog");
            if (guids == null || guids.Length == 0)
            {
                return null;
            }

            assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<DialogueVisualCatalog>(assetPath);
        }

        private static string ResolveAssetPath(DialogueSpriteRef mapping, string resolvedBundle)
        {
            if (mapping == null)
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(mapping.editorAssetPath) &&
                AssetDatabase.LoadMainAssetAtPath(mapping.editorAssetPath) != null)
            {
                return mapping.editorAssetPath;
            }

            if (string.IsNullOrWhiteSpace(resolvedBundle) || string.IsNullOrWhiteSpace(mapping.assetName))
            {
                return null;
            }

            var paths = AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(resolvedBundle, mapping.assetName);
            return paths != null && paths.Length > 0 ? paths[0] : null;
        }

        private static Dictionary<DialogueVisualSlot, HashSet<string>> CollectYarnReferences(string yarnPath)
        {
            var result = new Dictionary<DialogueVisualSlot, HashSet<string>>
            {
                { DialogueVisualSlot.Background, new HashSet<string>(StringComparer.OrdinalIgnoreCase) },
                { DialogueVisualSlot.NpcMain, new HashSet<string>(StringComparer.OrdinalIgnoreCase) },
                { DialogueVisualSlot.NpcAvatar, new HashSet<string>(StringComparer.OrdinalIgnoreCase) },
                { DialogueVisualSlot.PcAvatar, new HashSet<string>(StringComparer.OrdinalIgnoreCase) }
            };

            var textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(yarnPath);
            if (textAsset == null || string.IsNullOrWhiteSpace(textAsset.text))
            {
                return result;
            }

            var matches = YarnKeyCommandRegex.Matches(textAsset.text);
            foreach (Match match in matches)
            {
                if (!match.Success || match.Groups.Count < 3)
                {
                    continue;
                }

                var command = match.Groups[1].Value.Trim().ToLowerInvariant();
                var key = match.Groups[2].Value.Trim();
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                switch (command)
                {
                    case "itc_bg":
                        result[DialogueVisualSlot.Background].Add(key);
                        break;
                    case "itc_npc_main":
                        result[DialogueVisualSlot.NpcMain].Add(key);
                        break;
                    case "itc_npc_avatar":
                        result[DialogueVisualSlot.NpcAvatar].Add(key);
                        break;
                    case "itc_pc_avatar":
                        result[DialogueVisualSlot.PcAvatar].Add(key);
                        break;
                }
            }

            return result;
        }

        private static DialogueSpriteRef CreatePlaceholder(
            DialogueVisualCatalog catalog,
            DialogueVisualSlot slot,
            string key)
        {
            var placeholder = new DialogueSpriteRef
            {
                key = key,
                slot = slot,
                fallbackKey = catalog.GetDefaultKey(slot)
            };

            if (catalog.TryGetMapping(slot, placeholder.fallbackKey, out var fallback))
            {
                placeholder.assetName = fallback.assetName;
                placeholder.subSpriteName = fallback.subSpriteName;
                placeholder.assetBundleName = fallback.assetBundleName;
#if UNITY_EDITOR
                placeholder.editorAssetPath = fallback.editorAssetPath;
#endif
            }

            return placeholder;
        }

        private static bool SetBundleName(string assetPath, string bundleName)
        {
            if (string.IsNullOrWhiteSpace(assetPath) || string.IsNullOrWhiteSpace(bundleName))
            {
                return false;
            }

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

        private static string ComposeKey(DialogueVisualSlot slot, string key)
        {
            return $"{slot}:{key.Trim()}";
        }

        private static bool LogError(bool enabled, string message)
        {
            if (enabled)
            {
                Debug.LogError(message);
            }

            return true;
        }
    }
}
