using System;
using ITC.SignMiniGame;
using QFramework;
using UnityEngine;
using Yarn.Unity;

namespace ITC.Dialogue
{
    [DisallowMultipleComponent]
    public sealed class SignDialogueRuntimeFacade : MonoBehaviour
    {
        [Header("Feature Toggle")]
        [SerializeField] private bool enableSignRouting = true;
        [SerializeField] private bool suppressLegacyPresenterVisuals = true;

        [Header("Role Rules")]
        [SerializeField] private string[] playerSpeakerKeywords =
        {
            "玩家",
            "主角",
            "我",
            "内心",
            "心声",
            "player",
            "narrator"
        };

        [SerializeField] private string[] thoughtKeywords =
        {
            "内心",
            "心声",
            "thought",
            "mind"
        };

        private static readonly string[] BuiltInPlayerSpeakerKeywords =
        {
            "barks",
            "巴克斯"
        };

        private DialogueRunner dialogueRunner;
        private Canvas rootCanvas;
        private SignDialogueNpcTrack npcTrack;
        private SignDialoguePlayerTrack playerTrack;
        private SignDialogueHistoryBrowser historyBrowser;
        private SignDialogueCommandBridge commandBridge;
        private SignDialogueOptionsPresenter optionsPresenter;
        private SignMiniGameFlowStateModel flowStateModel;
        private SignDialogueRole roleOverride = SignDialogueRole.Auto;
        private bool isConfigured;

        public bool IsRoutingEnabled => enableSignRouting && isConfigured;
        public bool ShouldSuppressLegacyPresenterVisuals => suppressLegacyPresenterVisuals;
        public bool IsContinueInputBlocked =>
            IsAnyMiniGameRunning() ||
            (optionsPresenter != null && optionsPresenter.HasActiveOptions);

        public void Configure(
            DialogueRunner runner,
            Canvas canvas,
            SignDialoguePanelFactory factory,
            RectTransform historyBrowseRect,
            SignDialogueOptionsPresenter presenter)
        {
            dialogueRunner = runner;
            rootCanvas = canvas;
            optionsPresenter = presenter;
            flowStateModel ??= MainMenuApp.Interface.GetModel<SignMiniGameFlowStateModel>();

            npcTrack ??= new SignDialogueNpcTrack(factory);
            playerTrack ??= new SignDialoguePlayerTrack(factory);
            historyBrowser ??= new SignDialogueHistoryBrowser();
            commandBridge ??= new SignDialogueCommandBridge(this);

            npcTrack.SetHistoryBrowseRect(historyBrowseRect);
            historyBrowser.Configure(npcTrack, rootCanvas);
            optionsPresenter.Bind(this);

            isConfigured = dialogueRunner != null && npcTrack != null && playerTrack != null && optionsPresenter != null;

            if (isConfigured && isActiveAndEnabled)
            {
                commandBridge.Register(dialogueRunner);
            }
        }

        private void OnEnable()
        {
            if (isConfigured)
            {
                commandBridge?.Register(dialogueRunner);
            }
        }

        private void OnDisable()
        {
            commandBridge?.Unregister();
        }

        private void Update()
        {
            if (!IsRoutingEnabled)
            {
                return;
            }

            historyBrowser?.Tick();
        }

        public void RouteLine(string speaker, string content)
        {
            if (!IsRoutingEnabled)
            {
                return;
            }

            var visibleText = NormalizeVisibleText(content);
            if (string.IsNullOrWhiteSpace(visibleText))
            {
                return;
            }

            switch (ResolveRole(speaker, visibleText))
            {
                case SignDialogueRole.Npc:
                    playerTrack.HideCurrent();
                    npcTrack.PresentNpcLine(speaker, visibleText);
                    break;
                case SignDialogueRole.Player:
                case SignDialogueRole.Thought:
                    playerTrack.ShowText(visibleText);
                    break;
                default:
                    playerTrack.ShowText(visibleText);
                    break;
            }
        }

        public void EnterNpcCycle(string npcId)
        {
            npcTrack.ResetForNpc(npcId);
        }

        public void ExitNpcCycle()
        {
            npcTrack.HideAll();
        }

        public void SetRoleOverride(SignDialogueRole role)
        {
            roleOverride = role;
        }

        public SignDialoguePanelInstance ShowOptionsPanel()
        {
            return playerTrack.ShowOptionsPanel();
        }

        public void HidePlayerPanel()
        {
            playerTrack.HideCurrent();
        }

        public static SignDialogueRole ParseRoleToken(string roleToken)
        {
            if (string.IsNullOrWhiteSpace(roleToken))
            {
                return SignDialogueRole.Auto;
            }

            return roleToken.Trim().ToLowerInvariant() switch
            {
                "npc" => SignDialogueRole.Npc,
                "player" => SignDialogueRole.Player,
                "thought" => SignDialogueRole.Thought,
                "inner" => SignDialogueRole.Thought,
                "auto" => SignDialogueRole.Auto,
                _ => SignDialogueRole.Auto
            };
        }

        private SignDialogueRole ResolveRole(string speaker, string content)
        {
            if (roleOverride != SignDialogueRole.Auto)
            {
                return roleOverride;
            }

            if (!string.IsNullOrWhiteSpace(speaker))
            {
                if (ContainsKeyword(speaker, playerSpeakerKeywords))
                {
                    return SignDialogueRole.Player;
                }

                if (ContainsKeyword(speaker, BuiltInPlayerSpeakerKeywords))
                {
                    return SignDialogueRole.Player;
                }

                return SignDialogueRole.Npc;
            }

            return ContainsKeyword(content, thoughtKeywords)
                ? SignDialogueRole.Thought
                : SignDialogueRole.Player;
        }

        private bool IsAnyMiniGameRunning()
        {
            flowStateModel ??= MainMenuApp.Interface.GetModel<SignMiniGameFlowStateModel>();
            if (flowStateModel == null)
            {
                return false;
            }

            return flowStateModel.DocumentReviewRunning.Value ||
                   flowStateModel.RuneTypingRunning.Value ||
                   flowStateModel.RuneVerifyRunning.Value ||
                   flowStateModel.StampRunning.Value ||
                   flowStateModel.SoulCollectRunning.Value ||
                   flowStateModel.BeanSellRunning.Value ||
                   flowStateModel.SettlementRunning.Value;
        }

        private static bool ContainsKeyword(string source, string[] keywords)
        {
            if (string.IsNullOrWhiteSpace(source) || keywords == null)
            {
                return false;
            }

            foreach (var keyword in keywords)
            {
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    continue;
                }

                if (source.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private static string NormalizeVisibleText(string rawText)
        {
            if (string.IsNullOrWhiteSpace(rawText))
            {
                return string.Empty;
            }

            return rawText.Replace("\\n", "\n").Trim();
        }
    }
}
