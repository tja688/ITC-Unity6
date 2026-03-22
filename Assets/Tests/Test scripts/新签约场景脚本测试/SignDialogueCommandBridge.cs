using System.Collections;
using Yarn.Unity;

namespace ITC.Dialogue
{
    public sealed class SignDialogueCommandBridge
    {
        private const string NpcEnterCommand = "itc_sign_npc_enter";
        private const string NpcExitCommand = "itc_sign_npc_exit";
        private const string RoleCommand = "itc_sign_role";
        private const string PlaceholderMinigameCommand = "itc_sign_minigame";

        private readonly SignDialogueRuntimeFacade runtimeFacade;
        private readonly SignDialoguePlaceholderMinigameBridge placeholderBridge;
        private DialogueRunner runner;
        private bool isRegistered;

        public SignDialogueCommandBridge(
            SignDialogueRuntimeFacade runtimeFacade,
            SignDialoguePlaceholderMinigameBridge placeholderBridge)
        {
            this.runtimeFacade = runtimeFacade;
            this.placeholderBridge = placeholderBridge;
        }

        public void Register(DialogueRunner dialogueRunner)
        {
            if (isRegistered || dialogueRunner == null)
            {
                return;
            }

            runner = dialogueRunner;
            runner.AddCommandHandler<string>(NpcEnterCommand, HandleNpcEnter);
            runner.AddCommandHandler(NpcExitCommand, HandleNpcExit);
            runner.AddCommandHandler<string>(RoleCommand, HandleRole);
            runner.AddCommandHandler<string>(PlaceholderMinigameCommand, HandlePlaceholderMinigame);
            isRegistered = true;
        }

        public void Unregister()
        {
            if (!isRegistered || runner == null)
            {
                return;
            }

            runner.RemoveCommandHandler(NpcEnterCommand);
            runner.RemoveCommandHandler(NpcExitCommand);
            runner.RemoveCommandHandler(RoleCommand);
            runner.RemoveCommandHandler(PlaceholderMinigameCommand);
            runner = null;
            isRegistered = false;
        }

        private void HandleNpcEnter(string npcId)
        {
            runtimeFacade.EnterNpcCycle(npcId);
        }

        private void HandleNpcExit()
        {
            runtimeFacade.ExitNpcCycle();
        }

        private void HandleRole(string roleToken)
        {
            runtimeFacade.SetRoleOverride(SignDialogueRuntimeFacade.ParseRoleToken(roleToken));
        }

        private IEnumerator HandlePlaceholderMinigame(string token)
        {
            yield return placeholderBridge.RunPlaceholderMinigame(token);
        }
    }
}
