using System;
using System.Collections;
using System.Collections.Generic;
using ITC.Contracting;
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
            dialogueRunner.AddCommandHandler<string>("itc_doc_review", RunDocumentReviewCommand);
            dialogueRunner.AddCommandHandler<string, string>("itc_rune_typing", RunRuneTypingCommand);
            dialogueRunner.AddCommandHandler<string>("itc_stamp_select", RunStampSelectCommand);
            dialogueRunner.AddCommandHandler<string, string>("itc_soul_collect", RunSoulCollectCommand);
            dialogueRunner.AddCommandHandler<string>("itc_bean_sell", RunBeanSellCommand);
            dialogueRunner.AddCommandHandler<string>("itc_settlement", RunSettlementCommand);
            dialogueRunner.AddCommandHandler<string>("itc_rune_verify", RunRuneVerifyCommand);
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
            dialogueRunner.RemoveCommandHandler("itc_doc_review");
            dialogueRunner.RemoveCommandHandler("itc_rune_typing");
            dialogueRunner.RemoveCommandHandler("itc_stamp_select");
            dialogueRunner.RemoveCommandHandler("itc_soul_collect");
            dialogueRunner.RemoveCommandHandler("itc_bean_sell");
            dialogueRunner.RemoveCommandHandler("itc_settlement");
            dialogueRunner.RemoveCommandHandler("itc_rune_verify");
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

        private IEnumerator RunDocumentReviewCommand(string clientToken)
        {
            if (dialogueRunner == null)
            {
                yield break;
            }

            var flowState = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var variableStorage = dialogueRunner.VariableStorage;

            var clientId = ParseClientId(clientToken, variableStorage);
            var currentSatisfaction = ReadFloatVariable(variableStorage, "$satisfaction", 3f);
            var currentSignMistake = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$Sign_mistake", 0f));

            this.SendCommand(new BeginDocumentReviewCommand(clientId, currentSatisfaction, currentSignMistake));

            var clientConfig = configModel.GetDocumentReviewClientConfig(clientId);
            var resultSubmitted = false;

            var panelData = new DocumentReviewPanelData
            {
                ClientId = clientId,
                TutorialMode = false,
                ExpectedAction = clientConfig.CorrectDecision,
                RuntimeConfig = configModel.DocumentReviewRuleConfig,
                ClientConfig = clientConfig,
                AllowedRejectReasons = clientConfig.AllowedRejectReasons,
                OnFxCue = DispatchContractFxCue,
                OnCompleted = payload =>
                {
                    if (resultSubmitted)
                    {
                        return;
                    }

                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitDocumentReviewResultCommand(payload));
                }
            };

            var panelOpened = false;

            UIKit.OpenPanelAsync<DocumentReviewPanel>(
                    UILevel.PopUI,
                    panelData,
                    assetBundleName: "contracting_ui",
                    prefabName: nameof(DocumentReviewPanel))
                .ToAction()
                .StartGlobal(() => panelOpened = true);

            var openTimeout = 8f;
            var elapsed = 0f;
            while (!panelOpened && elapsed < openTimeout)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!panelOpened)
            {
                LogKit.E("[ITCDialoguePanel] DocumentReviewPanel open timeout. Applying fallback result.");
                if (!resultSubmitted)
                {
                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitDocumentReviewResultCommand(BuildFallbackResult(clientId)));
                }
            }
            else
            {
                var gameplayTimeout = 180f;
                elapsed = 0f;
                while (flowState.DocumentReviewRunning.Value && elapsed < gameplayTimeout)
                {
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }

                if (flowState.DocumentReviewRunning.Value && !resultSubmitted)
                {
                    LogKit.E("[ITCDialoguePanel] DocumentReviewPanel resolve timeout. Applying fallback result.");
                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitDocumentReviewResultCommand(BuildFallbackResult(clientId)));
                }
            }

            var routeResult = string.IsNullOrWhiteSpace(flowState.RouteDocReviewResult.Value)
                ? "passed"
                : flowState.RouteDocReviewResult.Value;

            WriteStringVariable(variableStorage, "$Route_DocReviewResult", routeResult);
            WriteFloatVariable(variableStorage, "$Sign_mistake", flowState.SignMistake.Value);
            WriteFloatVariable(variableStorage, "$satisfaction", flowState.Satisfaction.Value);

            UIKit.ClosePanel<DocumentReviewPanel>();
        }

        private IEnumerator RunRuneTypingCommand(string clientToken, string gridSizeToken)
        {
            if (dialogueRunner == null)
            {
                yield break;
            }

            var flowState = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var variableStorage = dialogueRunner.VariableStorage;

            var clientId = ParseClientId(clientToken, variableStorage);
            var gridSize = ParseRuneTypingGridSize(gridSizeToken, 4);
            var currentSatisfaction = ReadFloatVariable(variableStorage, "$satisfaction", 3f);
            var currentSignMistake = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$Sign_mistake", 0f));

            this.SendCommand(new BeginRuneTypingCommand(clientId, currentSatisfaction, currentSignMistake, gridSize));

            var roundConfig = configModel.BuildRuneTypingRoundConfig(clientId, gridSize);
            var resultSubmitted = false;
            var panelData = new RuneTypingPanelData
            {
                ClientId = clientId,
                GridSize = gridSize,
                RuntimeConfig = configModel.RuneTypingRuleConfig,
                RoundConfig = roundConfig,
                OnFxCue = DispatchContractFxCue,
                OnCompleted = payload =>
                {
                    if (resultSubmitted)
                    {
                        return;
                    }

                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitRuneTypingResultCommand(payload));
                }
            };

            var panelOpened = false;
            UIKit.OpenPanelAsync<RuneTypingPanel>(
                    UILevel.PopUI,
                    panelData,
                    assetBundleName: "contracting_ui",
                    prefabName: nameof(RuneTypingPanel))
                .ToAction()
                .StartGlobal(() => panelOpened = true);

            var openTimeout = 8f;
            var elapsed = 0f;
            while (!panelOpened && elapsed < openTimeout)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!panelOpened)
            {
                LogKit.E("[ITCDialoguePanel] RuneTypingPanel open timeout. Applying fallback result.");
                if (!resultSubmitted)
                {
                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitRuneTypingResultCommand(
                        BuildRuneTypingFallbackResult(clientId, gridSize)));
                }
            }
            else
            {
                var gameplayTimeout = 180f;
                elapsed = 0f;
                while (flowState.RuneTypingRunning.Value && elapsed < gameplayTimeout)
                {
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }

                if (flowState.RuneTypingRunning.Value && !resultSubmitted)
                {
                    LogKit.E("[ITCDialoguePanel] RuneTypingPanel resolve timeout. Applying fallback result.");
                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitRuneTypingResultCommand(
                        BuildRuneTypingFallbackResult(clientId, gridSize)));
                }
            }

            WriteFloatVariable(variableStorage, "$Route_QTEErrorCount", flowState.RouteQteErrorCount.Value);
            UIKit.ClosePanel<RuneTypingPanel>();
        }

        private IEnumerator RunStampSelectCommand(string clientToken)
        {
            if (dialogueRunner == null)
            {
                yield break;
            }

            var flowState = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var variableStorage = dialogueRunner.VariableStorage;

            var clientId = ParseClientId(clientToken, variableStorage);
            var currentSatisfaction = ReadFloatVariable(variableStorage, "$satisfaction", 3f);
            var currentSignMistake = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$Sign_mistake", 0f));
            var hasVerifyDebuff = ReadFloatVariable(variableStorage, "$Route_RuneVerifyDebuff", 0f) > 0.5f
                                  || string.Equals(
                                      ReadStringVariable(variableStorage, "$Route_RuneVerifyResult", "skipped"),
                                      "failed",
                                      StringComparison.OrdinalIgnoreCase);

            this.SendCommand(new BeginStampSelectionCommand(clientId, currentSatisfaction, currentSignMistake));

            var runtimeConfig = configModel.GetStampRuntimeConfig(clientId);
            var clientConfig = configModel.GetStampClientConfig(clientId);
            var resultSubmitted = false;

            var panelData = new StampPanelData
            {
                ClientId = clientId,
                TutorialMode = false,
                HasVerifyDebuff = hasVerifyDebuff,
                RuntimeConfig = runtimeConfig,
                ClientConfig = clientConfig,
                OnFxCue = DispatchContractFxCue,
                OnCompleted = payload =>
                {
                    if (resultSubmitted)
                    {
                        return;
                    }

                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitStampSelectionResultCommand(payload));
                }
            };

            var panelOpened = false;
            UIKit.OpenPanelAsync<StampPanel>(
                    UILevel.PopUI,
                    panelData,
                    assetBundleName: "contracting_ui",
                    prefabName: nameof(StampPanel))
                .ToAction()
                .StartGlobal(() => panelOpened = true);

            var openTimeout = 8f;
            var elapsed = 0f;
            while (!panelOpened && elapsed < openTimeout)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!panelOpened)
            {
                LogKit.E("[ITCDialoguePanel] StampPanel open timeout. Applying fallback result.");
                if (!resultSubmitted)
                {
                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitStampSelectionResultCommand(
                        BuildStampFallbackResult(clientId, runtimeConfig.CorrectStampType, hasVerifyDebuff)));
                }
            }
            else
            {
                var gameplayTimeout = 120f;
                elapsed = 0f;
                while (flowState.StampRunning.Value && elapsed < gameplayTimeout)
                {
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }

                if (flowState.StampRunning.Value && !resultSubmitted)
                {
                    LogKit.E("[ITCDialoguePanel] StampPanel resolve timeout. Applying fallback result.");
                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitStampSelectionResultCommand(
                        BuildStampFallbackResult(clientId, runtimeConfig.CorrectStampType, hasVerifyDebuff)));
                }
            }

            WriteStringVariable(variableStorage, "$Route_StampType", flowState.RouteStampType.Value);
            WriteStringVariable(variableStorage, "$Route_StampTimingResult", flowState.RouteStampTimingResult.Value);
            WriteFloatVariable(variableStorage, "$Sign_mistake", flowState.SignMistake.Value);
            WriteFloatVariable(variableStorage, "$satisfaction", flowState.Satisfaction.Value);

            UIKit.ClosePanel<StampPanel>();
        }

        private IEnumerator RunSoulCollectCommand(string clientToken, string targetPercentToken)
        {
            if (dialogueRunner == null)
            {
                yield break;
            }

            var flowState = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var variableStorage = dialogueRunner.VariableStorage;

            var clientId = ParseClientId(clientToken, variableStorage);
            var clientConfig = configModel.GetSoulCollectClientConfig(clientId);
            var fallbackTarget = clientConfig != null ? clientConfig.DefaultTargetPercent : 50;
            var targetPercent = ParseTargetPercent(targetPercentToken, fallbackTarget);
            var currentSatisfaction = ReadFloatVariable(variableStorage, "$satisfaction", 3f);
            var currentSignMistake = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$Sign_mistake", 0f));

            var runtimeConfig = configModel.BuildSoulCollectRuntimeConfig(clientId, targetPercent);
            this.SendCommand(new BeginSoulCollectCommand(
                clientId,
                currentSatisfaction,
                currentSignMistake,
                runtimeConfig.TargetPercent,
                runtimeConfig.MinPercent,
                runtimeConfig.MaxPercent));

            var resultSubmitted = false;
            var panelData = new SoulCollectPanelData
            {
                ClientId = clientId,
                TutorialMode = false,
                RuntimeConfig = runtimeConfig,
                ClientConfig = clientConfig,
                OnFxCue = DispatchContractFxCue,
                OnCompleted = payload =>
                {
                    if (resultSubmitted)
                    {
                        return;
                    }

                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitSoulCollectResultCommand(payload));
                }
            };

            var panelOpened = false;
            UIKit.OpenPanelAsync<SoulCollectPanel>(
                    UILevel.PopUI,
                    panelData,
                    assetBundleName: "contracting_ui",
                    prefabName: nameof(SoulCollectPanel))
                .ToAction()
                .StartGlobal(() => panelOpened = true);

            var openTimeout = 8f;
            var elapsed = 0f;
            while (!panelOpened && elapsed < openTimeout)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!panelOpened)
            {
                LogKit.E("[ITCDialoguePanel] SoulCollectPanel open timeout. Applying fallback result.");
                if (!resultSubmitted)
                {
                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitSoulCollectResultCommand(
                        BuildSoulCollectFallbackResult(clientId, runtimeConfig)));
                }
            }
            else
            {
                var gameplayTimeout = 180f;
                elapsed = 0f;
                while (flowState.SoulCollectRunning.Value && elapsed < gameplayTimeout)
                {
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }

                if (flowState.SoulCollectRunning.Value && !resultSubmitted)
                {
                    LogKit.E("[ITCDialoguePanel] SoulCollectPanel resolve timeout. Applying fallback result.");
                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitSoulCollectResultCommand(
                        BuildSoulCollectFallbackResult(clientId, runtimeConfig)));
                }
            }

            WriteFloatVariable(variableStorage, "$Route_SoulCollectPercent", flowState.RouteSoulCollectPercent.Value);
            WriteFloatVariable(variableStorage, "$Route_SoulMin", flowState.RouteSoulMin.Value);
            WriteFloatVariable(variableStorage, "$Route_SoulMax", flowState.RouteSoulMax.Value);
            WriteFloatVariable(variableStorage, "$Sign_mistake", flowState.SignMistake.Value);
            WriteFloatVariable(variableStorage, "$satisfaction", flowState.Satisfaction.Value);

            UIKit.ClosePanel<SoulCollectPanel>();
        }

        private IEnumerator RunBeanSellCommand(string clientToken)
        {
            if (dialogueRunner == null)
            {
                yield break;
            }

            var flowState = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var variableStorage = dialogueRunner.VariableStorage;

            var clientId = ParseClientId(clientToken, variableStorage);
            var day = Mathf.Max(1, Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$DAY", 1f)));
            var currentSatisfaction = ReadFloatVariable(variableStorage, "$satisfaction", 3f);
            var currentSignMistake = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$Sign_mistake", 0f));
            var currentSoldCount = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$Route_BeanSoldCount", 0f));

            this.SendCommand(new BeginBeanSellCommand(
                clientId,
                day,
                currentSatisfaction,
                currentSignMistake,
                currentSoldCount));

            var runtimeConfig = configModel.BuildBeanSellRuntimeConfig();
            var clientConfig = configModel.GetBeanSellClientConfig(clientId);
            var resultSubmitted = false;

            if (day < runtimeConfig.EnabledFromDay)
            {
                resultSubmitted = true;
                MainMenuApp.Interface.SendCommand(new SubmitBeanSellResultCommand(
                    BuildBeanSellSkippedResult(clientId, day)));
            }
            else
            {
                var panelData = new BeanSellPanelData
                {
                    ClientId = clientId,
                    Day = day,
                    RuntimeConfig = runtimeConfig,
                    ClientConfig = clientConfig,
                    OnFxCue = DispatchContractFxCue,
                    OnCompleted = payload =>
                    {
                        if (resultSubmitted)
                        {
                            return;
                        }

                        resultSubmitted = true;
                        MainMenuApp.Interface.SendCommand(new SubmitBeanSellResultCommand(payload));
                    }
                };

                var panelOpened = false;
                UIKit.OpenPanelAsync<BeanSellPanel>(
                        UILevel.PopUI,
                        panelData,
                        assetBundleName: "contracting_ui",
                        prefabName: nameof(BeanSellPanel))
                    .ToAction()
                    .StartGlobal(() => panelOpened = true);

                var openTimeout = 8f;
                var elapsed = 0f;
                while (!panelOpened && elapsed < openTimeout)
                {
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }

                if (!panelOpened)
                {
                    LogKit.E("[ITCDialoguePanel] BeanSellPanel open timeout. Applying fallback result.");
                    if (!resultSubmitted)
                    {
                        resultSubmitted = true;
                        MainMenuApp.Interface.SendCommand(new SubmitBeanSellResultCommand(
                            BuildBeanSellFallbackResult(clientId, day)));
                    }
                }
                else
                {
                    var gameplayTimeout = 90f;
                    elapsed = 0f;
                    while (flowState.BeanSellRunning.Value && elapsed < gameplayTimeout)
                    {
                        elapsed += Time.unscaledDeltaTime;
                        yield return null;
                    }

                    if (flowState.BeanSellRunning.Value && !resultSubmitted)
                    {
                        LogKit.E("[ITCDialoguePanel] BeanSellPanel resolve timeout. Applying fallback result.");
                        resultSubmitted = true;
                        MainMenuApp.Interface.SendCommand(new SubmitBeanSellResultCommand(
                            BuildBeanSellFallbackResult(clientId, day)));
                    }
                }

                UIKit.ClosePanel<BeanSellPanel>();
            }

            WriteStringVariable(variableStorage, "$Route_BeanSellResult", flowState.RouteBeanSellResult.Value);
            WriteFloatVariable(variableStorage, "$Route_BeanSoldCount", flowState.RouteBeanSoldCount.Value);
            WriteFloatVariable(variableStorage, "$satisfaction", flowState.Satisfaction.Value);
            WriteFloatVariable(variableStorage, "$Sign_mistake", flowState.SignMistake.Value);
        }

        private IEnumerator RunSettlementCommand(string clientToken)
        {
            if (dialogueRunner == null)
            {
                yield break;
            }

            var flowState = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var variableStorage = dialogueRunner.VariableStorage;

            var clientId = ParseClientId(clientToken, variableStorage);
            var day = Mathf.Max(1, Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$DAY", 1f)));
            var currentSatisfaction = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$satisfaction", 3f));
            var currentSignMistake = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$Sign_mistake", 0f));
            var currentMoney = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$money", 0f));
            var currentNumberOfSignMistake = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$Nmber_of_sign_mistake", 0f));
            var currentGlobalSignMistake = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$Global_sign_mistake", 0f));

            this.SendCommand(new BeginSettlementCommand(
                clientId,
                day,
                currentSatisfaction,
                currentSignMistake,
                currentMoney,
                currentNumberOfSignMistake,
                currentGlobalSignMistake));
            this.SendCommand(new FinalizeClientContractCommand(day));

            var settlementClientConfig = configModel.GetSettlementClientConfig(clientId);
            var settlementRuleConfig = configModel.BuildSettlementRuntimeConfig();
            var panelData = new SettlementPanelData
            {
                ClientId = clientId,
                Day = day,
                ClientDisplayName = settlementClientConfig != null
                    ? settlementClientConfig.ClientDisplayName
                    : $"客户{Mathf.Max(1, clientId)}",
                DocReviewResult = flowState.RouteDocReviewResult.Value,
                QteErrorCount = flowState.RouteQteErrorCount.Value,
                StampType = flowState.RouteStampType.Value,
                StampTimingResult = flowState.RouteStampTimingResult.Value,
                SoulPercent = flowState.RouteSoulCollectPercent.Value,
                BeanSellResult = flowState.RouteBeanSellResult.Value,
                FinalSatisfaction = Mathf.RoundToInt(flowState.Satisfaction.Value),
                TipAmount = flowState.RouteSettlementTip.Value,
                TotalMoney = flowState.Money.Value,
                Tier = ParseSettlementTier(flowState.RouteSettlementTier.Value),
                FeedbackDuration = settlementRuleConfig.FeedbackDuration,
                OnFxCue = DispatchContractFxCue
            };

            var panelOpened = false;
            var panelFinished = false;
            panelData.OnCompleted = () => panelFinished = true;

            UIKit.OpenPanelAsync<SettlementPanel>(
                    UILevel.PopUI,
                    panelData,
                    assetBundleName: "contracting_ui",
                    prefabName: nameof(SettlementPanel))
                .ToAction()
                .StartGlobal(() => panelOpened = true);

            var openTimeout = 8f;
            var elapsed = 0f;
            while (!panelOpened && elapsed < openTimeout)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (panelOpened)
            {
                var feedbackTimeout = 30f;
                elapsed = 0f;
                while (!panelFinished && elapsed < feedbackTimeout)
                {
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }

                if (!panelFinished)
                {
                    LogKit.W("[ITCDialoguePanel] SettlementPanel feedback timeout, continue route.");
                }

                UIKit.ClosePanel<SettlementPanel>();
            }
            else
            {
                LogKit.W("[ITCDialoguePanel] SettlementPanel open timeout, settlement data already finalized.");
            }

            WriteFloatVariable(variableStorage, "$satisfaction", flowState.Satisfaction.Value);
            WriteFloatVariable(variableStorage, "$Sign_mistake", flowState.SignMistake.Value);
            WriteFloatVariable(variableStorage, "$money", flowState.Money.Value);
            WriteFloatVariable(variableStorage, "$Nmber_of_sign_mistake", flowState.NumberOfSignMistake.Value);
            WriteFloatVariable(variableStorage, "$Global_sign_mistake", flowState.GlobalSignMistake.Value);
            WriteFloatVariable(variableStorage, "$Route_SettlementTip", flowState.RouteSettlementTip.Value);
            WriteStringVariable(variableStorage, "$Route_SettlementTier", flowState.RouteSettlementTier.Value);
            WriteFloatVariable(variableStorage, "$Route_SettlementFinalSatisfaction", flowState.RouteSettlementFinalSatisfaction.Value);
        }

        private IEnumerator RunRuneVerifyCommand(string clientToken)
        {
            if (dialogueRunner == null)
            {
                yield break;
            }

            var flowState = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var variableStorage = dialogueRunner.VariableStorage;
            var clientId = ParseClientId(clientToken, variableStorage);
            var config = configModel.RuneVerifyRuleConfig;
            var randomSeed = ResolveRuneVerifySeed(config, clientId, variableStorage);
            var triggered = ShouldTriggerRuneVerify(config, randomSeed);

            this.SendCommand(new BeginRuneVerifyCommand(clientId, triggered, randomSeed));

            if (!triggered)
            {
                MainMenuApp.Interface.SendCommand(new SubmitRuneVerifyResultCommand(new RuneVerifyResultPayload
                {
                    ClientId = clientId,
                    Result = RuneVerifyResultType.Skipped,
                    FoundCount = 0,
                    DistortedCount = Mathf.Max(1, config.DistortedCount),
                    Triggered = false,
                    WasTimeoutFallback = false,
                    RandomSeed = randomSeed
                }));

                WriteStringVariable(variableStorage, "$Route_RuneVerifyResult", flowState.RouteRuneVerifyResult.Value);
                WriteFloatVariable(variableStorage, "$Route_RuneVerifyDebuff", flowState.RouteRuneVerifyDebuff.Value);
                yield break;
            }

            var resultSubmitted = false;
            var panelData = new RuneVerifyPanelData
            {
                ClientId = clientId,
                RuntimeConfig = config,
                RuntimeSeed = randomSeed,
                OnFxCue = DispatchContractFxCue,
                OnCompleted = payload =>
                {
                    if (resultSubmitted)
                    {
                        return;
                    }

                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitRuneVerifyResultCommand(payload));
                }
            };

            var panelOpened = false;
            UIKit.OpenPanelAsync<RuneVerifyPanel>(
                    UILevel.PopUI,
                    panelData,
                    assetBundleName: "contracting_ui",
                    prefabName: nameof(RuneVerifyPanel))
                .ToAction()
                .StartGlobal(() => panelOpened = true);

            var openTimeout = 8f;
            var elapsed = 0f;
            while (!panelOpened && elapsed < openTimeout)
            {
                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!panelOpened)
            {
                LogKit.E("[ITCDialoguePanel] RuneVerifyPanel open timeout. Applying fallback result.");
                if (!resultSubmitted)
                {
                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitRuneVerifyResultCommand(BuildRuneVerifyFallbackResult(
                        clientId,
                        Mathf.Max(1, config.DistortedCount),
                        randomSeed)));
                }
            }
            else
            {
                var gameplayTimeout = Mathf.Max(10f, config.TimeLimitSeconds + config.CountdownLeadSeconds + 8f);
                elapsed = 0f;
                while (flowState.RuneVerifyRunning.Value && elapsed < gameplayTimeout)
                {
                    elapsed += Time.unscaledDeltaTime;
                    yield return null;
                }

                if (flowState.RuneVerifyRunning.Value && !resultSubmitted)
                {
                    LogKit.E("[ITCDialoguePanel] RuneVerifyPanel resolve timeout. Applying fallback result.");
                    resultSubmitted = true;
                    MainMenuApp.Interface.SendCommand(new SubmitRuneVerifyResultCommand(BuildRuneVerifyFallbackResult(
                        clientId,
                        Mathf.Max(1, config.DistortedCount),
                        randomSeed)));
                }
            }

            var routeResult = string.IsNullOrWhiteSpace(flowState.RouteRuneVerifyResult.Value)
                ? "failed"
                : flowState.RouteRuneVerifyResult.Value;
            WriteStringVariable(variableStorage, "$Route_RuneVerifyResult", routeResult);
            WriteFloatVariable(variableStorage, "$Route_RuneVerifyDebuff", flowState.RouteRuneVerifyDebuff.Value);

            UIKit.ClosePanel<RuneVerifyPanel>();
        }

        private static int ParseClientId(string clientToken, VariableStorageBehaviour variableStorage)
        {
            if (int.TryParse(clientToken, out var parsedClientId) && parsedClientId > 0)
            {
                return parsedClientId;
            }

            var fallbackClientId = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$Route_CurrentClient", 1f));
            return Mathf.Max(1, fallbackClientId);
        }

        private static int ParseRuneTypingGridSize(string gridSizeToken, int fallbackGridSize)
        {
            if (int.TryParse(gridSizeToken, out var parsedGridSize))
            {
                return Mathf.Clamp(parsedGridSize, 4, 5);
            }

            return Mathf.Clamp(fallbackGridSize, 4, 5);
        }

        private static int ParseTargetPercent(string targetPercentToken, int fallbackTargetPercent)
        {
            if (int.TryParse(targetPercentToken, out var parsedTarget))
            {
                return Mathf.Clamp(parsedTarget, 0, 100);
            }

            return Mathf.Clamp(fallbackTargetPercent, 0, 100);
        }

        private static SettlementTier ParseSettlementTier(string tier)
        {
            if (string.Equals(tier, "good", StringComparison.OrdinalIgnoreCase))
            {
                return SettlementTier.Good;
            }

            if (string.Equals(tier, "bad", StringComparison.OrdinalIgnoreCase))
            {
                return SettlementTier.Bad;
            }

            return SettlementTier.Neutral;
        }

        private static float ReadFloatVariable(
            VariableStorageBehaviour variableStorage,
            string variableName,
            float defaultValue)
        {
            if (variableStorage == null || string.IsNullOrWhiteSpace(variableName))
            {
                return defaultValue;
            }

            return variableStorage.TryGetValue<float>(variableName, out var value)
                ? value
                : defaultValue;
        }

        private static string ReadStringVariable(
            VariableStorageBehaviour variableStorage,
            string variableName,
            string defaultValue)
        {
            if (variableStorage == null || string.IsNullOrWhiteSpace(variableName))
            {
                return defaultValue;
            }

            return variableStorage.TryGetValue<string>(variableName, out var value)
                ? value
                : defaultValue;
        }

        private static void WriteFloatVariable(
            VariableStorageBehaviour variableStorage,
            string variableName,
            float value)
        {
            if (variableStorage == null || string.IsNullOrWhiteSpace(variableName))
            {
                return;
            }

            variableStorage.SetValue(variableName, value);
        }

        private static void WriteStringVariable(
            VariableStorageBehaviour variableStorage,
            string variableName,
            string value)
        {
            if (variableStorage == null || string.IsNullOrWhiteSpace(variableName))
            {
                return;
            }

            variableStorage.SetValue(variableName, value ?? string.Empty);
        }

        private static void DispatchContractFxCue(string cueId, Transform anchor, float intensity)
        {
            // Reserved for future AudioKit/VFX router wiring.
        }

        private static DocumentReviewResultPayload BuildFallbackResult(int clientId)
        {
            return new DocumentReviewResultPayload
            {
                ClientId = Mathf.Max(1, clientId),
                FinalAction = DocumentReviewDecision.Pass,
                RejectReason = DocumentReviewRejectReason.None,
                InspectedHotspotCount = 0,
                TutorialMode = false,
                ForceConfirmed = false,
                WasFallback = true
            };
        }

        private static RuneTypingResultPayload BuildRuneTypingFallbackResult(int clientId, int gridSize)
        {
            return new RuneTypingResultPayload
            {
                ClientId = Mathf.Max(1, clientId),
                GridSize = Mathf.Clamp(gridSize, 4, 5),
                ErrorCount = 0,
                SequenceLength = 0,
                UsedOnScreenButtons = false,
                WasFallback = true
            };
        }

        private static StampResultPayload BuildStampFallbackResult(
            int clientId,
            StampType correctStampType,
            bool hasVerifyDebuff)
        {
            return new StampResultPayload
            {
                ClientId = Mathf.Max(1, clientId),
                SelectedStampType = correctStampType,
                TimingResult = StampTimingResult.Normal,
                HitNormalizedTime = 0.78f,
                TypeCorrect = true,
                HasVerifyDebuff = hasVerifyDebuff,
                WasFallback = true
            };
        }

        private static SoulCollectResultPayload BuildSoulCollectFallbackResult(int clientId, SoulCollectConfig runtimeConfig)
        {
            var config = runtimeConfig ?? new SoulCollectConfig();
            var minPercent = Mathf.Clamp(config.MinPercent, 0, 100);
            var maxPercent = Mathf.Clamp(config.MaxPercent, minPercent, 100);
            var targetPercent = Mathf.Clamp(config.TargetPercent, minPercent, maxPercent);

            return new SoulCollectResultPayload
            {
                ClientId = Mathf.Max(1, clientId),
                TargetPercent = targetPercent,
                MinPercent = minPercent,
                MaxPercent = maxPercent,
                ActualPercent = targetPercent,
                ActualRawFloat = targetPercent,
                ResolveType = SoulCollectResolveType.Normal,
                WasFallback = true
            };
        }

        private static BeanSellResultPayload BuildBeanSellSkippedResult(int clientId, int day)
        {
            return new BeanSellResultPayload
            {
                ClientId = Mathf.Max(1, clientId),
                Day = Mathf.Max(1, day),
                PitchType = BeanPitchType.Benefit,
                FinalScore = 0,
                SuccessThreshold = 0,
                IsSuccess = false,
                IsSkipped = true,
                WasFallback = false
            };
        }

        private static BeanSellResultPayload BuildBeanSellFallbackResult(int clientId, int day)
        {
            return new BeanSellResultPayload
            {
                ClientId = Mathf.Max(1, clientId),
                Day = Mathf.Max(1, day),
                PitchType = BeanPitchType.Benefit,
                FinalScore = 0,
                SuccessThreshold = 0,
                IsSuccess = false,
                IsSkipped = true,
                WasFallback = true
            };
        }

        private static RuneVerifyResultPayload BuildRuneVerifyFallbackResult(int clientId, int distortedCount, int randomSeed)
        {
            return new RuneVerifyResultPayload
            {
                ClientId = Mathf.Max(1, clientId),
                Result = RuneVerifyResultType.Failed,
                FoundCount = 0,
                DistortedCount = Mathf.Max(1, distortedCount),
                Triggered = true,
                WasTimeoutFallback = true,
                RandomSeed = randomSeed
            };
        }

        private static int ResolveRuneVerifySeed(
            RuneVerifyConfig config,
            int clientId,
            VariableStorageBehaviour variableStorage)
        {
            if (config != null && config.FixedRandomSeed >= 0)
            {
                return config.FixedRandomSeed;
            }

            var day = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$DAY", 1f));
            var currentClient = Mathf.RoundToInt(ReadFloatVariable(variableStorage, "$Route_CurrentClient", clientId));
            var mixA = day * 73856093;
            var mixB = currentClient * 19349663;
            return mixA ^ mixB ^ 83492791;
        }

        private static bool ShouldTriggerRuneVerify(RuneVerifyConfig config, int randomSeed)
        {
            if (config == null)
            {
                return false;
            }

            var probability = Mathf.Clamp01(config.TriggerProbability);
            if (probability <= 0f)
            {
                return false;
            }

            if (probability >= 1f)
            {
                return true;
            }

            var random = new System.Random(randomSeed);
            return random.NextDouble() < probability;
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
