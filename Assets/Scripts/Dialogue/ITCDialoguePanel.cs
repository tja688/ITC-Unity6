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
        [Header("Dialogue References")]
        [SerializeField] private DialogueRunner dialogueRunner;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image npcPortraitImage;
        [SerializeField] private Image npcAvatarImage;
        [SerializeField] private Image pcPortraitImage;

        [Header("Visual Catalog")]
        [SerializeField] private DialogueVisualCatalog visualCatalog;
        [SerializeField] private string resourcesCatalogPath = "Dialogue/DialogueVisualCatalog";

        [Header("Playback")]
        [SerializeField] private float visualFadeDuration = 0.2f;
        [SerializeField] private bool hidePortraitWhenMissing = true;

        private readonly Dictionary<string, DialogueSpriteRef> visualLookup =
            new(StringComparer.OrdinalIgnoreCase);

        private readonly Dictionary<Image, CanvasGroup> imageCanvasGroups = new();
        private readonly HashSet<string> loggedVisualErrors = new(StringComparer.OrdinalIgnoreCase);

        private ITCDialoguePanelData panelData = new();
        private ResLoader resLoader;
        private DialogueSpriteProvider spriteProvider;
        private bool commandsRegistered;

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
            EnsureCatalog();
            BuildLookups();
            EnsureResLoader();
            EnsureSpriteProvider();
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

        private void EnsureCatalog()
        {
            if (visualCatalog != null || string.IsNullOrWhiteSpace(resourcesCatalogPath))
            {
                return;
            }

            visualCatalog = Resources.Load<DialogueVisualCatalog>(resourcesCatalogPath.Trim());
        }

        private void BuildLookups()
        {
            visualLookup.Clear();

            if (visualCatalog == null || visualCatalog.Mappings == null)
            {
                return;
            }

            foreach (var mapping in visualCatalog.Mappings)
            {
                if (mapping == null || string.IsNullOrWhiteSpace(mapping.key))
                {
                    continue;
                }

                var lookupKey = ComposeLookupKey(mapping.slot, mapping.key);
                if (visualLookup.ContainsKey(lookupKey))
                {
                    LogKit.W($"[ITCDialoguePanel] Duplicate mapping overridden: {mapping.slot}:{mapping.key}");
                }

                visualLookup[lookupKey] = mapping;
            }
        }

        private void EnsureResLoader()
        {
            if (resLoader == null)
            {
                resLoader = ResLoader.Allocate();
            }
        }

        private void EnsureSpriteProvider()
        {
            if (spriteProvider == null && resLoader != null)
            {
                spriteProvider = new DialogueSpriteProvider(resLoader);
            }
        }

        private void RegisterCommands()
        {
            if (commandsRegistered || dialogueRunner == null)
            {
                return;
            }

            dialogueRunner.AddCommandHandler<string>("itc_bg", SwitchBackgroundCommand);
            dialogueRunner.AddCommandHandler<string>("itc_npc_main", SwitchNpcMainPortraitCommand);
            dialogueRunner.AddCommandHandler<string>("itc_npc_avatar", SwitchNpcAvatarCommand);
            dialogueRunner.AddCommandHandler<string>("itc_pc_avatar", SwitchPcAvatarCommand);
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
            dialogueRunner.RemoveCommandHandler("itc_npc_main");
            dialogueRunner.RemoveCommandHandler("itc_npc_avatar");
            dialogueRunner.RemoveCommandHandler("itc_pc_avatar");
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
                DialogueVisualSlot.Background,
                preserveAspect: false);
        }

        private IEnumerator SwitchNpcMainPortraitCommand(string key)
        {
            yield return SwapPortraitByKey(
                npcPortraitImage,
                key,
                DialogueVisualSlot.NpcMain);
        }

        private IEnumerator SwitchNpcAvatarCommand(string key)
        {
            yield return SwapPortraitByKey(
                npcAvatarImage,
                key,
                DialogueVisualSlot.NpcAvatar);
        }

        private IEnumerator SwitchPcAvatarCommand(string key)
        {
            yield return SwapPortraitByKey(
                pcPortraitImage,
                key,
                DialogueVisualSlot.PcAvatar);
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

        private IEnumerator SwapPortraitByKey(Image target, string key, DialogueVisualSlot slot)
        {
            yield return SwapImageByKey(
                target,
                key,
                slot,
                preserveAspect: true);
        }

        private IEnumerator SwapImageByKey(
            Image target,
            string key,
            DialogueVisualSlot slot,
            bool preserveAspect)
        {
            if (target == null)
            {
                yield break;
            }

            if (visualCatalog == null || spriteProvider == null)
            {
                LogOnceError($"catalog-missing:{slot}",
                    $"[ITCDialoguePanel] Visual catalog/provider unavailable. Slot={slot}");
                if (hidePortraitWhenMissing)
                {
                    HideImage(target);
                }

                yield break;
            }

            Sprite sprite = null;
            yield return ResolveSpriteByKeyWithFallbackAsync(
                slot,
                key,
                visualCatalog.GetDefaultKey(slot),
                s => sprite = s);

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

        private IEnumerator ResolveSpriteByKeyWithFallbackAsync(
            DialogueVisualSlot slot,
            string requestedKey,
            string slotDefaultKey,
            Action<Sprite> onCompleted)
        {
            var queue = new Queue<string>();
            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            EnqueueIfNotEmpty(queue, requestedKey);
            EnqueueIfNotEmpty(queue, slotDefaultKey);

            while (queue.Count > 0)
            {
                var currentKey = queue.Dequeue();
                if (!visited.Add(currentKey))
                {
                    continue;
                }

                if (!TryGetMapping(slot, currentKey, out var mapping))
                {
                    continue;
                }

                Sprite loaded = null;
                yield return LoadSpriteForMappingAsync(mapping, s => loaded = s);
                if (loaded != null)
                {
                    onCompleted?.Invoke(loaded);
                    yield break;
                }

                LogOnceError(
                    $"load-failed:{slot}:{currentKey}",
                    $"[ITCDialoguePanel] Sprite load failed. slot={slot}, key={currentKey}, bundle={mapping.assetBundleName}, asset={mapping.assetName}, subSprite={mapping.subSpriteName}");

                EnqueueIfNotEmpty(queue, mapping.fallbackKey);
            }

            Sprite missing = null;
            yield return LoadMissingSpriteAsync(s => missing = s);
            onCompleted?.Invoke(missing);
        }

        private IEnumerator LoadSpriteForMappingAsync(DialogueSpriteRef mapping, Action<Sprite> onCompleted)
        {
            var defaultBundle = visualCatalog.GetDefaultBundle(mapping.slot);
            yield return spriteProvider.LoadSpriteAsync(mapping, defaultBundle, onCompleted);
        }

        private IEnumerator LoadMissingSpriteAsync(Action<Sprite> onCompleted)
        {
            var missingBundle = string.IsNullOrWhiteSpace(visualCatalog.MissingSpriteBundle)
                ? visualCatalog.DefaultPortraitBundle
                : visualCatalog.MissingSpriteBundle.Trim();

            if (string.IsNullOrWhiteSpace(visualCatalog.MissingSpriteAssetName))
            {
                onCompleted?.Invoke(null);
                yield break;
            }

            yield return spriteProvider.LoadRawSpriteAsync(
                missingBundle,
                visualCatalog.MissingSpriteAssetName,
                visualCatalog.MissingSpriteSubSpriteName,
#if UNITY_EDITOR
                visualCatalog.MissingSpriteEditorAssetPath,
#else
                null,
#endif
                onCompleted);
        }

        private bool TryGetMapping(DialogueVisualSlot slot, string key, out DialogueSpriteRef mapping)
        {
            mapping = null;
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }

            return visualLookup.TryGetValue(ComposeLookupKey(slot, key), out mapping);
        }

        private static string ComposeLookupKey(DialogueVisualSlot slot, string key)
        {
            return $"{slot}:{key.Trim()}";
        }

        private static void EnqueueIfNotEmpty(Queue<string> queue, string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                queue.Enqueue(key.Trim());
            }
        }

        private void LogOnceError(string hash, string message)
        {
            if (loggedVisualErrors.Add(hash))
            {
                LogKit.E(message);
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
            spriteProvider?.ClearCache();
            spriteProvider = null;

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
