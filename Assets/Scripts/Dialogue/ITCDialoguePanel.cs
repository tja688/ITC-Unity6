using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace ITC.Dialogue
{
    [Serializable]
    public sealed class ITCDialoguePanelData : UIPanelData
    {
        public string StartNode = "ITC_Start";
        public bool AutoStartOnOpen = true;
    }

    public sealed class ITCDialoguePanel : UIPanel, IController
    {
        [Serializable]
        private sealed class VisualAssetMapping
        {
            public string key;
            public string assetName;
            public string assetBundleName;
#if UNITY_EDITOR
            public string editorAssetPath;
#endif
        }

        [Header("Dialogue References")]
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image npcPortraitImage;
        [SerializeField] private Image npcAvatarImage;
        [SerializeField] private Image pcPortraitImage;

        [Header("ResKit Bundles")]
        [SerializeField] private string defaultBackgroundBundle = "dialogue_bg";
        [SerializeField] private string defaultPortraitBundle = "dialogue_portrait";

        [Header("Portrait Slot Defaults")]
        [SerializeField] private string npcMainPortraitDefaultKey = "barks_default";
        [SerializeField] private string npcAvatarDefaultKey = "barks_default";
        [SerializeField] private string pcAvatarDefaultKey = "barks_default";

        [Header("Playback")]
        [SerializeField] private float visualFadeDuration = 0.2f;
        [SerializeField] private bool hidePortraitWhenMissing = true;
        [SerializeField] private bool trimTransparentPixelsForPortrait = false;
        [SerializeField] [Range(0, 255)] private int portraitAlphaThreshold = 10;

        [Header("Background Mapping")]
        [SerializeField] private List<VisualAssetMapping> backgroundMappings = new()
        {
            new VisualAssetMapping
            {
                key = "main_menu",
                assetName = "主菜单",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位背景原画/主菜单.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "hotel_room",
                assetName = "地狱旅馆-客房",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位背景原画/地狱旅馆-客房.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "mirror_room",
                assetName = "地狱旅馆-浴室镜前",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位背景原画/地狱旅馆-浴室镜前.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "flame_gate",
                assetName = "火焰之门",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位背景原画/火焰之门.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "city_center",
                assetName = "圣纽约市-中心全景",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位背景原画/圣纽约市-中心全景.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "itc_building",
                assetName = "ITC大楼-外观",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位背景原画/ITC大楼-外观.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "itc_guard",
                assetName = "ITC门卫亭",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位背景原画/ITC门卫亭.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "itc_lobby",
                assetName = "ITC大厅",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位背景原画/ITC大厅.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "itc_elevator",
                assetName = "ITC电梯内部",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位背景原画/ITC电梯内部.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "henet_office",
                assetName = "Henet办公室",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位背景原画/Henet办公室.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "itc_office",
                assetName = "ITC办公区",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位背景原画/ITC办公区.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "window13",
                assetName = "13号窗口 (外部反打)",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位背景原画/13号窗口 (外部反打).png"
#endif
            }
        };

        [Header("Portrait Mapping")]
        [SerializeField] private List<VisualAssetMapping> portraitMappings = new()
        {
            new VisualAssetMapping
            {
                key = "barks_default",
                assetName = "通用标准屏幕中心立绘",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位人物立绘/通用标准屏幕中心立绘.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "skeletonbellboy_default",
                assetName = "通用标准屏幕中心立绘",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位人物立绘/通用标准屏幕中心立绘.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "oldguard_default",
                assetName = "通用标准屏幕中心立绘",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位人物立绘/通用标准屏幕中心立绘.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "receptionist_default",
                assetName = "通用标准屏幕中心立绘",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位人物立绘/通用标准屏幕中心立绘.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "oldtom_default",
                assetName = "通用标准屏幕中心立绘",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位人物立绘/通用标准屏幕中心立绘.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "henet_default",
                assetName = "通用标准屏幕中心立绘",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位人物立绘/通用标准屏幕中心立绘.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "oldclerk13_default",
                assetName = "通用标准屏幕中心立绘",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位人物立绘/通用标准屏幕中心立绘.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "emmett_default",
                assetName = "通用标准屏幕中心立绘",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位人物立绘/通用标准屏幕中心立绘.png"
#endif
            },
            new VisualAssetMapping
            {
                key = "thomas_default",
                assetName = "通用标准屏幕中心立绘",
#if UNITY_EDITOR
                editorAssetPath = "Assets/Arts/Texture2d图片/占位人物立绘/通用标准屏幕中心立绘.png"
#endif
            }
        };

        private readonly Dictionary<string, VisualAssetMapping> backgroundLookup =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, VisualAssetMapping> portraitLookup =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<string, Sprite> runtimeSpriteCache =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<Image, CanvasGroup> imageCanvasGroups = new();

        private ITCDialoguePanelData panelData = new();
        private ResLoader resLoader;
        private bool commandsRegistered;
        private bool portraitTrimReadable = true;

        private void Awake()
        {
            BootstrapRuntimeBindings();
        }

        protected override void OnInit(IUIData uiData = null)
        {
            panelData = uiData as ITCDialoguePanelData ?? new ITCDialoguePanelData();
            BootstrapRuntimeBindings();

            if (dialogueRunner != null)
            {
                dialogueRunner.autoStart = false;
            }

            HideImage(npcPortraitImage);
            HideImage(npcAvatarImage);
            HideImage(pcPortraitImage);
        }

        private void BootstrapRuntimeBindings()
        {
            CacheReferences();
            BuildLookups();
            EnsureResLoader();
            RegisterCommands();
        }

        protected override void OnOpen(IUIData uiData = null)
        {
            panelData = uiData as ITCDialoguePanelData ?? panelData ?? new ITCDialoguePanelData();
            if (!panelData.AutoStartOnOpen || dialogueRunner == null || dialogueRunner.IsDialogueRunning)
            {
                return;
            }

            _ = dialogueRunner.StartDialogue(panelData.StartNode);
        }

        protected override void OnClose()
        {
            UnregisterCommands();
            ReleaseResLoader();
            this.SendCommand<MarkDialoguePanelClosedCommand>();
        }

        private void CacheReferences()
        {
            if (dialogueRunner == null)
            {
                dialogueRunner = transform.Find("DialogueRunner")?.GetComponent<DialogueRunner>();
            }

            var panelRoot = transform.Find("DialogueCanvas/DialoguePanel");
            if (panelRoot == null)
            {
                return;
            }

            if (backgroundImage == null)
            {
                backgroundImage = panelRoot.Find("BG")?.GetComponent<Image>();
            }

            if (npcPortraitImage == null)
            {
                npcPortraitImage = panelRoot.Find("NPC main portrait")?.GetComponent<Image>();
            }

            if (npcAvatarImage == null)
            {
                npcAvatarImage = panelRoot.Find("Avatarillustration_NPC")?.GetComponent<Image>();
            }

            if (pcPortraitImage == null)
            {
                pcPortraitImage = panelRoot.Find("Avatarillustration_PC")?.GetComponent<Image>();
            }
        }

        private void BuildLookups()
        {
            backgroundLookup.Clear();
            portraitLookup.Clear();

            BuildLookup(backgroundMappings, backgroundLookup);
            BuildLookup(portraitMappings, portraitLookup);
            NormalizePortraitMappings();
        }

        private void NormalizePortraitMappings()
        {
            if (portraitLookup.TryGetValue("barks_default", out var barksMapping) &&
                barksMapping != null &&
                string.Equals(barksMapping.assetName, "通用标准人物头像", StringComparison.Ordinal))
            {
                barksMapping.assetName = "通用标准屏幕中心立绘";
            }
        }

        private static void BuildLookup(IEnumerable<VisualAssetMapping> source, IDictionary<string, VisualAssetMapping> target)
        {
            if (source == null)
            {
                return;
            }

            foreach (var mapping in source)
            {
                if (mapping == null || string.IsNullOrWhiteSpace(mapping.key))
                {
                    continue;
                }

                target[mapping.key.Trim()] = mapping;
            }
        }

        private void EnsureResLoader()
        {
            if (resLoader == null)
            {
                resLoader = ResLoader.Allocate();
            }
        }

        private void RegisterCommands()
        {
            if (commandsRegistered || dialogueRunner == null)
            {
                return;
            }

            dialogueRunner.AddCommandHandler<string>("itc_bg", SwitchBackgroundCommand);
            dialogueRunner.AddCommandHandler<string>("itc_npc", SwitchNpcPortraitCommand);
            dialogueRunner.AddCommandHandler<string>("itc_pc", SwitchPcPortraitCommand);
            dialogueRunner.AddCommandHandler<string>("itc_npc_main", SwitchNpcMainPortraitCommand);
            dialogueRunner.AddCommandHandler<string>("itc_npc_avatar", SwitchNpcAvatarCommand);
            dialogueRunner.AddCommandHandler<string>("itc_pc_avatar", SwitchPcAvatarCommand);
            dialogueRunner.AddCommandHandler("itc_npc_hide", HideNpcPortraitCommand);
            dialogueRunner.AddCommandHandler("itc_pc_hide", HidePcPortraitCommand);
            dialogueRunner.AddCommandHandler("itc_npc_main_hide", HideNpcMainPortraitCommand);
            dialogueRunner.AddCommandHandler("itc_npc_avatar_hide", HideNpcAvatarCommand);
            dialogueRunner.AddCommandHandler("itc_pc_avatar_hide", HidePcAvatarCommand);
            commandsRegistered = true;
        }

        private void UnregisterCommands()
        {
            if (!commandsRegistered || dialogueRunner == null)
            {
                return;
            }

            dialogueRunner.RemoveCommandHandler("itc_bg");
            dialogueRunner.RemoveCommandHandler("itc_npc");
            dialogueRunner.RemoveCommandHandler("itc_pc");
            dialogueRunner.RemoveCommandHandler("itc_npc_main");
            dialogueRunner.RemoveCommandHandler("itc_npc_avatar");
            dialogueRunner.RemoveCommandHandler("itc_pc_avatar");
            dialogueRunner.RemoveCommandHandler("itc_npc_hide");
            dialogueRunner.RemoveCommandHandler("itc_pc_hide");
            dialogueRunner.RemoveCommandHandler("itc_npc_main_hide");
            dialogueRunner.RemoveCommandHandler("itc_npc_avatar_hide");
            dialogueRunner.RemoveCommandHandler("itc_pc_avatar_hide");
            commandsRegistered = false;
        }

        private IEnumerator SwitchBackgroundCommand(string key)
        {
            yield return SwapImageByKey(
                backgroundImage,
                key,
                backgroundLookup,
                defaultBackgroundBundle,
                preserveAspect: false,
                trimTransparentPixels: false);
        }

        private IEnumerator SwitchNpcPortraitCommand(string key)
        {
            yield return SwitchNpcMainPortraitCommand(key);
            yield return SwitchNpcAvatarCommand(key);
        }

        private IEnumerator SwitchPcPortraitCommand(string key)
        {
            yield return SwitchPcAvatarCommand(key);
        }

        private IEnumerator SwitchNpcMainPortraitCommand(string key)
        {
            yield return SwapPortraitByKey(
                npcPortraitImage,
                key,
                npcMainPortraitDefaultKey);
        }

        private IEnumerator SwitchNpcAvatarCommand(string key)
        {
            yield return SwapPortraitByKey(
                npcAvatarImage,
                key,
                npcAvatarDefaultKey);
        }

        private IEnumerator SwitchPcAvatarCommand(string key)
        {
            yield return SwapPortraitByKey(
                pcPortraitImage,
                key,
                pcAvatarDefaultKey);
        }

        private void HideNpcPortraitCommand()
        {
            HideNpcMainPortraitCommand();
            HideNpcAvatarCommand();
        }

        private void HidePcPortraitCommand()
        {
            HidePcAvatarCommand();
        }

        private void HideNpcMainPortraitCommand()
        {
            HideImage(npcPortraitImage);
        }

        private void HideNpcAvatarCommand()
        {
            HideImage(npcAvatarImage);
        }

        private void HidePcAvatarCommand()
        {
            HideImage(pcPortraitImage);
        }

        private IEnumerator SwapPortraitByKey(Image target, string key, string fallbackKey)
        {
            yield return SwapImageByKey(
                target,
                key,
                portraitLookup,
                defaultPortraitBundle,
                preserveAspect: true,
                trimTransparentPixels: trimTransparentPixelsForPortrait,
                fallbackKey: fallbackKey);
        }

        private IEnumerator SwapImageByKey(
            Image target,
            string key,
            IReadOnlyDictionary<string, VisualAssetMapping> lookup,
            string defaultBundle,
            bool preserveAspect,
            bool trimTransparentPixels,
            string fallbackKey = null)
        {
            if (target == null)
            {
                yield break;
            }

            var resolvedFallbackKey = string.IsNullOrWhiteSpace(fallbackKey) ? null : fallbackKey.Trim();
            var resolvedKey = string.IsNullOrWhiteSpace(key) ? resolvedFallbackKey : key.Trim();

            if (!TryGetMapping(lookup, resolvedKey, out var mapping) &&
                !TryGetMapping(lookup, resolvedFallbackKey, out mapping))
            {
                if (hidePortraitWhenMissing)
                {
                    HideImage(target);
                }
                yield break;
            }

            Sprite sprite = null;
            yield return LoadSpriteForMappingAsync(mapping, defaultBundle, trimTransparentPixels, s => sprite = s);
            if (sprite == null)
            {
                if (hidePortraitWhenMissing)
                {
                    HideImage(target);
                }
                yield break;
            }

            yield return FadeSwapImage(target, sprite, preserveAspect);
        }

        private static bool TryGetMapping(
            IReadOnlyDictionary<string, VisualAssetMapping> lookup,
            string key,
            out VisualAssetMapping mapping)
        {
            mapping = null;

            if (lookup == null || string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            return lookup.TryGetValue(key.Trim(), out mapping);
        }

        private IEnumerator LoadSpriteForMappingAsync(
            VisualAssetMapping mapping,
            string defaultBundle,
            bool trimTransparentPixels,
            Action<Sprite> onCompleted)
        {
            var bundle = string.IsNullOrWhiteSpace(mapping.assetBundleName)
                ? defaultBundle
                : mapping.assetBundleName.Trim();
            var cacheKey = $"{bundle}:{mapping.assetName}:trim={(trimTransparentPixels ? 1 : 0)}";

            if (runtimeSpriteCache.TryGetValue(cacheKey, out var cachedSprite) && cachedSprite != null)
            {
                onCompleted?.Invoke(cachedSprite);
                yield break;
            }

            Texture2D texture = null;
            yield return LoadTextureForMappingAsync(mapping, defaultBundle, t => texture = t);
            if (texture == null)
            {
                onCompleted?.Invoke(null);
                yield break;
            }

            var spriteRect = new Rect(0, 0, texture.width, texture.height);
            if (trimTransparentPixels && TryGetOpaqueRect(texture, out var opaqueRect))
            {
                spriteRect = opaqueRect;
            }

            var sprite = Sprite.Create(
                texture,
                spriteRect,
                new Vector2(0.5f, 0.5f),
                100f);
            sprite.name = $"{mapping.assetName}_runtime";
            runtimeSpriteCache[cacheKey] = sprite;
            onCompleted?.Invoke(sprite);
        }

        private IEnumerator LoadTextureForMappingAsync(
            VisualAssetMapping mapping,
            string defaultBundle,
            Action<Texture2D> onCompleted)
        {
            var bundle = string.IsNullOrWhiteSpace(mapping.assetBundleName)
                ? defaultBundle
                : mapping.assetBundleName.Trim();

            Texture2D texture = null;
            yield return LoadTextureAsync(bundle, mapping.assetName, t => texture = t);

            if (texture == null)
            {
                yield return LoadTextureAsync(string.Empty, mapping.assetName, t => texture = t);
            }

#if UNITY_EDITOR
            if (texture == null &&
                AssetBundlePathHelper.SimulationMode &&
                !string.IsNullOrWhiteSpace(mapping.editorAssetPath))
            {
                texture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(mapping.editorAssetPath);
            }
#endif

            onCompleted?.Invoke(texture);
        }

        private IEnumerator LoadTextureAsync(
            string bundleName,
            string assetName,
            Action<Texture2D> onCompleted)
        {
            if (string.IsNullOrWhiteSpace(assetName))
            {
                onCompleted?.Invoke(null);
                yield break;
            }

            EnsureResLoader();
            var done = false;
            var success = false;
            Texture2D loadedTexture = null;

            void OnLoaded(bool ok, IRes res)
            {
                success = ok;
                loadedTexture = res?.Asset as Texture2D;
                done = true;
            }

            if (string.IsNullOrWhiteSpace(bundleName))
            {
                resLoader.Add2Load<Texture2D>(assetName, OnLoaded);
            }
            else
            {
                resLoader.Add2Load<Texture2D>(bundleName, assetName, OnLoaded);
            }

            resLoader.LoadAsync();
            while (!done)
            {
                yield return null;
            }

            if (!success && loadedTexture == null)
            {
                onCompleted?.Invoke(null);
                yield break;
            }

            if (loadedTexture == null)
            {
                onCompleted?.Invoke(null);
                yield break;
            }

            onCompleted?.Invoke(loadedTexture);
        }

        private bool TryGetOpaqueRect(Texture2D texture, out Rect rect)
        {
            rect = default;

            if (!portraitTrimReadable || texture == null)
            {
                return false;
            }

            try
            {
                var pixels = texture.GetPixels32();
                var width = texture.width;
                var height = texture.height;
                var minX = width;
                var minY = height;
                var maxX = -1;
                var maxY = -1;
                var threshold = (byte)Mathf.Clamp(portraitAlphaThreshold, 0, 255);

                for (var i = 0; i < pixels.Length; i++)
                {
                    if (pixels[i].a <= threshold)
                    {
                        continue;
                    }

                    var x = i % width;
                    var y = i / width;

                    if (x < minX) minX = x;
                    if (y < minY) minY = y;
                    if (x > maxX) maxX = x;
                    if (y > maxY) maxY = y;
                }

                if (maxX < 0 || maxY < 0 || minX > maxX || minY > maxY)
                {
                    return false;
                }

                rect = Rect.MinMaxRect(minX, minY, maxX + 1, maxY + 1);
                return rect.width > 1f && rect.height > 1f;
            }
            catch (Exception)
            {
                portraitTrimReadable = false;
                return false;
            }
        }

        private IEnumerator FadeSwapImage(Image image, Sprite sprite, bool preserveAspect)
        {
            if (image == null || sprite == null)
            {
                yield break;
            }

            var canvasGroup = GetOrAddCanvasGroup(image);
            var halfDuration = Mathf.Max(0.01f, visualFadeDuration * 0.5f);

            if (image.enabled && image.sprite != null)
            {
                yield return FadeCanvasGroup(canvasGroup, canvasGroup.alpha, 0f, halfDuration);
            }

            image.sprite = sprite;
            image.preserveAspect = preserveAspect;
            image.enabled = true;
            canvasGroup.alpha = 0f;

            yield return FadeCanvasGroup(canvasGroup, 0f, 1f, halfDuration);
        }

        private static IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float from, float to, float duration)
        {
            if (canvasGroup == null || duration <= 0f)
            {
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = to;
                }
                yield break;
            }

            var elapsed = 0f;
            canvasGroup.alpha = from;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                canvasGroup.alpha = Mathf.Lerp(from, to, t);
                yield return null;
            }

            canvasGroup.alpha = to;
        }

        private CanvasGroup GetOrAddCanvasGroup(Image image)
        {
            if (imageCanvasGroups.TryGetValue(image, out var existing) && existing != null)
            {
                return existing;
            }

            var canvasGroup = image.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = image.gameObject.AddComponent<CanvasGroup>();
            }

            imageCanvasGroups[image] = canvasGroup;
            return canvasGroup;
        }

        private void HideImage(Image image)
        {
            if (image == null)
            {
                return;
            }

            image.enabled = false;
            image.sprite = null;
            GetOrAddCanvasGroup(image).alpha = 0f;
        }

        private void ReleaseResLoader()
        {
            foreach (var runtimeSprite in runtimeSpriteCache.Values)
            {
                if (runtimeSprite != null)
                {
                    Destroy(runtimeSprite);
                }
            }
            runtimeSpriteCache.Clear();

            if (resLoader == null)
            {
                return;
            }

            resLoader.ReleaseAllRes();
            resLoader.Recycle2Cache();
            resLoader = null;
        }

        public IArchitecture GetArchitecture()
        {
            return MainMenuApp.Interface;
        }
    }
}
