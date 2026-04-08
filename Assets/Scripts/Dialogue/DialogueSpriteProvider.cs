using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QFramework;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ITC.Dialogue
{
    public sealed class DialogueSpriteProvider
    {
        private readonly ResLoader resLoader;
        private readonly Dictionary<string, Sprite> spriteCache = new(StringComparer.OrdinalIgnoreCase);

        public DialogueSpriteProvider(ResLoader loader)
        {
            resLoader = loader;
        }

        public IEnumerator LoadSpriteAsync(
            DialogueSpriteRef mapping,
            string defaultBundle,
            Action<Sprite> onCompleted)
        {
            if (mapping == null)
            {
                onCompleted?.Invoke(null);
                yield break;
            }

            if (mapping.directSprite != null)
            {
                onCompleted?.Invoke(mapping.directSprite);
                yield break;
            }

            var bundle = string.IsNullOrWhiteSpace(mapping.assetBundleName)
                ? defaultBundle
                : mapping.assetBundleName.Trim();

            yield return LoadRawSpriteAsync(
                bundle,
                mapping.assetName,
                mapping.subSpriteName,
#if UNITY_EDITOR
                mapping.editorAssetPath,
#else
                null,
#endif
                onCompleted);
        }

        public IEnumerator LoadRawSpriteAsync(
            string bundleName,
            string assetName,
            string subSpriteName,
            string editorAssetPath,
            Action<Sprite> onCompleted)
        {
            if (string.IsNullOrWhiteSpace(assetName))
            {
                onCompleted?.Invoke(null);
                yield break;
            }

            var cacheKey = $"{bundleName}|{assetName}|{subSpriteName}";
            if (spriteCache.TryGetValue(cacheKey, out var cached) && cached != null)
            {
                onCompleted?.Invoke(cached);
                yield break;
            }

#if UNITY_EDITOR
            if (AssetBundlePathHelper.SimulationMode &&
                !string.IsNullOrWhiteSpace(editorAssetPath))
            {
                var editorSprite = LoadSpriteFromEditorAsset(editorAssetPath, subSpriteName);
                if (editorSprite != null)
                {
                    spriteCache[cacheKey] = editorSprite;
                }

                onCompleted?.Invoke(editorSprite);
                yield break;
            }
#endif

            if (string.IsNullOrWhiteSpace(bundleName))
            {
                onCompleted?.Invoke(null);
                yield break;
            }

            AssetBundle loadedBundle = null;
            var done = false;
            var success = false;

            void OnBundleLoaded(bool ok, IRes res)
            {
                success = ok;
                loadedBundle = res?.Asset as AssetBundle;
                done = true;
            }

            resLoader.Add2Load<AssetBundle>(bundleName, OnBundleLoaded);
            resLoader.LoadAsync();

            while (!done)
            {
                yield return null;
            }

            if (!success || loadedBundle == null)
            {
                onCompleted?.Invoke(null);
                yield break;
            }

            var sprite = LoadSpriteFromBundle(loadedBundle, assetName, subSpriteName);
            if (sprite != null)
            {
                spriteCache[cacheKey] = sprite;
            }

            onCompleted?.Invoke(sprite);
        }

        public void ClearCache()
        {
            spriteCache.Clear();
        }

        private static Sprite LoadSpriteFromBundle(
            AssetBundle bundle,
            string assetName,
            string subSpriteName)
        {
            if (bundle == null || string.IsNullOrWhiteSpace(assetName))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(subSpriteName))
            {
                var subSprites = bundle.LoadAssetWithSubAssets<Sprite>(assetName);
                var matchedSub = FindByName(subSprites, subSpriteName);
                if (matchedSub != null)
                {
                    return matchedSub;
                }

                var directSub = bundle.LoadAsset<Sprite>(subSpriteName);
                if (directSub != null)
                {
                    return directSub;
                }

                if (subSprites != null && subSprites.Length > 0)
                {
                    return subSprites[0];
                }
            }

            var sprite = bundle.LoadAsset<Sprite>(assetName);
            if (sprite != null)
            {
                return sprite;
            }

            var allSprites = bundle.LoadAssetWithSubAssets<Sprite>(assetName);
            return allSprites != null && allSprites.Length > 0 ? allSprites[0] : null;
        }

#if UNITY_EDITOR
        private static Sprite LoadSpriteFromEditorAsset(string assetPath, string subSpriteName)
        {
            var allAssets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
            var sprites = allAssets.OfType<Sprite>().ToArray();

            if (!string.IsNullOrWhiteSpace(subSpriteName))
            {
                var match = FindByName(sprites, subSpriteName);
                if (match != null)
                {
                    return match;
                }
            }

            if (sprites.Length == 1)
            {
                return sprites[0];
            }

            if (sprites.Length > 1)
            {
                var defaultName = Path.GetFileNameWithoutExtension(assetPath);
                return FindByName(sprites, defaultName) ?? sprites[0];
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
        }
#endif

        private static Sprite FindByName(IEnumerable<Sprite> sprites, string name)
        {
            if (sprites == null || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            foreach (var sprite in sprites)
            {
                if (sprite != null &&
                    string.Equals(sprite.name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return sprite;
                }
            }

            return null;
        }
    }
}
