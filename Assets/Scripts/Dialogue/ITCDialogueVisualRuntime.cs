using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace ITC.Dialogue
{
    [DisallowMultipleComponent]
    public sealed class ITCDialogueVisualRuntime : MonoBehaviour, IController
    {
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private Canvas targetCanvas;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image portraitImage;
        [SerializeField] private bool emitVerboseLogs = true;

        private readonly Dictionary<string, Sprite> backgroundCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Sprite> portraitCache = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Color> portraitTintMap = new(StringComparer.OrdinalIgnoreCase)
        {
            ["barks"] = new Color(1f, 1f, 1f, 0.92f),
            ["skeleton_bellboy"] = new Color(0.78f, 1f, 0.8f, 0.92f),
            ["oldguard"] = new Color(0.85f, 0.9f, 1f, 0.92f),
            ["receptionist"] = new Color(1f, 0.9f, 0.85f, 0.92f),
            ["oldtom"] = new Color(0.92f, 0.95f, 0.85f, 0.92f),
            ["henet"] = new Color(1f, 0.82f, 0.9f, 0.92f),
            ["oldclerk13"] = new Color(0.94f, 0.94f, 0.8f, 0.92f),
            ["veer"] = new Color(0.86f, 0.82f, 1f, 0.92f),
            ["emmett"] = new Color(0.95f, 0.88f, 0.74f, 0.92f),
            ["thomas"] = new Color(0.8f, 0.86f, 0.9f, 0.92f),
            ["narrator"] = new Color(1f, 1f, 1f, 0.5f),
        };

        private ResLoader resLoader;

        public void Initialize(DialogueRunner runner)
        {
            dialogueRunner = runner;
            EnsureVisualLayer();
        }

        private void Awake()
        {
            if (dialogueRunner == null)
            {
                dialogueRunner = GetComponentInChildren<DialogueRunner>(true);
            }

            if (targetCanvas == null)
            {
                targetCanvas = GetComponentInChildren<Canvas>(true);
            }

            resLoader = ResLoader.Allocate();
            EnsureVisualLayer();
        }

        private void OnDestroy()
        {
            resLoader?.ReleaseAllRes();
            resLoader?.Recycle2Cache();
            resLoader = null;
        }

        [YarnCommand("itc_bg")]
        public void YarnSetBackground(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                LogKit.W("[ITCDialogueVisualRuntime] itc_bg called with empty key.");
                return;
            }

            if (!EnsureVisualLayer())
            {
                return;
            }

            if (!ITCDialogueVisualCatalog.TryGetBackground(key, out var address))
            {
                LogKit.W($"[ITCDialogueVisualRuntime] Unknown background key '{key}'.");
                return;
            }

            var sprite = GetOrLoadSprite(backgroundCache, key, address);
            if (sprite == null)
            {
                LogKit.E(
                    $"[ITCDialogueVisualRuntime] Failed to load background sprite, key='{key}', asset='{address.AssetName}'.");
                return;
            }

            backgroundImage.sprite = sprite;
            backgroundImage.enabled = true;
            this.SendCommand(new UpdateDialogueVisualStateCommand(key, null));

            if (emitVerboseLogs)
            {
                LogKit.I(
                    $"[ITCDialogueVisualRuntime] Background switched -> key='{key}', bundle='{address.BundleName}', asset='{address.AssetName}'.");
            }
        }

        [YarnCommand("itc_portrait")]
        public void YarnSetPortrait(string key)
        {
            if (string.IsNullOrWhiteSpace(key) || key.Equals("none", StringComparison.OrdinalIgnoreCase))
            {
                YarnHidePortrait();
                return;
            }

            if (!EnsureVisualLayer())
            {
                return;
            }

            if (!ITCDialogueVisualCatalog.TryGetPortrait(key, out var address))
            {
                LogKit.W($"[ITCDialogueVisualRuntime] Unknown portrait key '{key}'.");
                return;
            }

            var sprite = GetOrLoadSprite(portraitCache, key, address);
            if (sprite == null)
            {
                LogKit.E(
                    $"[ITCDialogueVisualRuntime] Failed to load portrait sprite, key='{key}', asset='{address.AssetName}'.");
                return;
            }

            portraitImage.sprite = sprite;
            portraitImage.enabled = true;
            portraitImage.color = portraitTintMap.TryGetValue(key, out var tint)
                ? tint
                : new Color(1f, 1f, 1f, 0.9f);

            this.SendCommand(new UpdateDialogueVisualStateCommand(null, key));

            if (emitVerboseLogs)
            {
                LogKit.I(
                    $"[ITCDialogueVisualRuntime] Portrait switched -> key='{key}', bundle='{address.BundleName}', asset='{address.AssetName}'.");
            }
        }

        [YarnCommand("itc_hide_portrait")]
        public void YarnHidePortrait()
        {
            if (!EnsureVisualLayer())
            {
                return;
            }

            portraitImage.enabled = false;
            portraitImage.sprite = null;
            this.SendCommand(new UpdateDialogueVisualStateCommand(null, string.Empty));

            if (emitVerboseLogs)
            {
                LogKit.I("[ITCDialogueVisualRuntime] Portrait hidden.");
            }
        }

        [YarnCommand("itc_debug")]
        public void YarnDebugMarker(string marker)
        {
            LogKit.I($"[ITCDialogueVisualRuntime] Yarn marker: {marker}");
        }

        public IArchitecture GetArchitecture()
        {
            return DialogueSceneApp.Interface;
        }

        private bool EnsureVisualLayer()
        {
            if (targetCanvas == null)
            {
                targetCanvas = GetComponentInChildren<Canvas>(true);
            }

            if (targetCanvas == null)
            {
                LogKit.E("[ITCDialogueVisualRuntime] Target canvas is missing.");
                return false;
            }

            if (backgroundImage != null && portraitImage != null)
            {
                return true;
            }

            var visualRoot = targetCanvas.transform.Find("ITCVisualLayer");
            if (visualRoot == null)
            {
                var rootGo = new GameObject("ITCVisualLayer", typeof(RectTransform));
                visualRoot = rootGo.transform;
                visualRoot.SetParent(targetCanvas.transform, false);

                var rootRect = (RectTransform)visualRoot;
                rootRect.anchorMin = Vector2.zero;
                rootRect.anchorMax = Vector2.one;
                rootRect.offsetMin = Vector2.zero;
                rootRect.offsetMax = Vector2.zero;
                visualRoot.SetSiblingIndex(0);
            }

            if (backgroundImage == null)
            {
                var bgGo = new GameObject("Background", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                bgGo.transform.SetParent(visualRoot, false);

                var rect = (RectTransform)bgGo.transform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                backgroundImage = bgGo.GetComponent<Image>();
                backgroundImage.preserveAspect = false;
                backgroundImage.raycastTarget = false;
            }

            if (portraitImage == null)
            {
                var portraitGo = new GameObject("Portrait", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                portraitGo.transform.SetParent(visualRoot, false);

                var rect = (RectTransform)portraitGo.transform;
                rect.anchorMin = new Vector2(0.5f, 0f);
                rect.anchorMax = new Vector2(0.5f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(0f, 16f);
                rect.sizeDelta = new Vector2(980f, 980f);

                portraitImage = portraitGo.GetComponent<Image>();
                portraitImage.preserveAspect = true;
                portraitImage.raycastTarget = false;
                portraitImage.enabled = false;
            }

            this.SendCommand<MarkDialogueVisualLayerReadyCommand>();
            return true;
        }

        private Sprite GetOrLoadSprite(
            IDictionary<string, Sprite> cache,
            string key,
            DialogueSpriteAddress address)
        {
            if (cache.TryGetValue(key, out var cachedSprite) && cachedSprite != null)
            {
                return cachedSprite;
            }

            var sprite = TryLoadSprite(address.BundleName, address.AssetName);
            if (sprite == null)
            {
                return null;
            }

            cache[key] = sprite;
            return sprite;
        }

        private Sprite TryLoadSprite(string bundleName, string assetName)
        {
            if (resLoader == null)
            {
                resLoader = ResLoader.Allocate();
            }

            Sprite sprite = null;

            if (!string.IsNullOrEmpty(bundleName) && ITCDialogueResPaths.HasAssetEntry(bundleName, assetName))
            {
                sprite = resLoader.LoadSync<Sprite>(bundleName, assetName);
            }

            if (sprite == null && ITCDialogueResPaths.HasAssetEntry(string.Empty, assetName))
            {
                sprite = resLoader.LoadSync<Sprite>(assetName);
            }

            return sprite;
        }
    }
}
