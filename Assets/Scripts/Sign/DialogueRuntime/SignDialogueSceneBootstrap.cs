using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Reflection;
using Febucci.TextAnimatorForUnity;
using Febucci.TextAnimatorForUnity.TextMeshPro;
using QFramework;
using UnityEngine;
using Yarn.Unity;

namespace ITC.Dialogue
{
    [DefaultExecutionOrder(-1000)]
    [DisallowMultipleComponent]
    public sealed class SignDialogueSceneBootstrap : MonoBehaviour
    {
        private const string DialogueSystemRootName = "签约特制对话系统";
        private const string BackgroundRootName = "纯背景";
        private const string FrontMaskName = "对话框前置遮罩";
        private const string BackMaskName = "对话框后置遮罩";
        private const string PlayerMaskName = "玩家框遮罩";
        private const string FrontTemplateName = "对话框前置";
        private const string BackTemplateName = "对话框后置";
        private const string PlayerTemplateName = "玩家框";
        private const string DialoguePanelPath = "DialogueCanvas/DialoguePanel";
        private const string OptionsPanelPath = "DialogueCanvas/DialoguePanel/OptionsPanel";

        [Header("Play Mode Fallback")]
        [SerializeField] private bool ensureDialogueStarts = true;
        [SerializeField] private float startKickDelaySeconds = 0.35f;
        [SerializeField] private string fallbackStartNode = "Start";

        private void Awake()
        {
            var dialogueRoot = FindByName(transform, DialogueSystemRootName);
            if (dialogueRoot != null && !dialogueRoot.gameObject.activeSelf)
            {
                dialogueRoot.gameObject.SetActive(true);
            }

            var backgroundRoot = FindByName(transform, BackgroundRootName);
            var frontMask = FindByName(backgroundRoot, FrontMaskName) as RectTransform;
            var backMask = FindByName(backgroundRoot, BackMaskName) as RectTransform;
            var playerMask = FindByName(backgroundRoot, PlayerMaskName) as RectTransform;

            if (frontMask == null || backMask == null || playerMask == null || dialogueRoot == null)
            {
                LogKit.E("[SignDialogueSceneBootstrap] Failed to resolve required scene anchors.");
                enabled = false;
                return;
            }

            var frontTemplate = PrepareTemplate(frontMask, FrontTemplateName);
            var backTemplate = PrepareTemplate(backMask, BackTemplateName);
            var playerTemplate = PrepareTemplate(playerMask, PlayerTemplateName);

            if (frontTemplate == null || backTemplate == null || playerTemplate == null)
            {
                LogKit.E("[SignDialogueSceneBootstrap] Failed to resolve dialogue panel templates.");
                enabled = false;
                return;
            }

            var dialogueRunner = dialogueRoot.GetComponentInChildren<DialogueRunner>(true);
            var dialoguePanel = dialogueRoot.Find(DialoguePanelPath);
            var optionsPanel = dialogueRoot.Find(OptionsPanelPath);
            var linePresenter = dialogueRoot.GetComponentInChildren<TALinePresenter>(true);

            if (dialogueRunner == null || dialoguePanel == null || optionsPanel == null || linePresenter == null)
            {
                LogKit.E("[SignDialogueSceneBootstrap] Failed to resolve dialogue bridge components.");
                enabled = false;
                return;
            }

            var runtimeFacade = GetComponent<SignDialogueRuntimeFacade>();
            if (runtimeFacade == null)
            {
                runtimeFacade = gameObject.AddComponent<SignDialogueRuntimeFacade>();
            }

            var newOptionsPresenter = optionsPanel.GetComponent<SignDialogueOptionsPresenter>();
            if (newOptionsPresenter == null)
            {
                newOptionsPresenter = optionsPanel.gameObject.AddComponent<SignDialogueOptionsPresenter>();
            }

            var legacyOptionsPresenter = optionsPanel.GetComponent<OptionsPresenter>();
            if (legacyOptionsPresenter != null)
            {
                legacyOptionsPresenter.enabled = false;
            }

            // SignScene routes dialogue into the sign runtime slots, so the scene-local legacy
            // Text Animator stack should stay disabled from the first frame. Leaving it enabled
            // lets the hidden fallback text object tick independently and pollute both runtime
            // behavior and editor validation with irrelevant exceptions.
            var legacyTextAnimator = dialoguePanel.GetComponentInChildren<TextAnimator_TMP>(true);
            if (legacyTextAnimator != null)
            {
                legacyTextAnimator.enabled = false;
            }

            var legacyTypewriter = dialoguePanel.GetComponentInChildren<TypewriterComponent>(true);
            if (legacyTypewriter != null)
            {
                legacyTypewriter.enabled = false;
            }

            ConfigureDialogueRunner(dialogueRunner, linePresenter, newOptionsPresenter);
            SetAllowOptionFallthrough(dialogueRunner, false);

            var panelFactory = new SignDialoguePanelFactory(frontTemplate, backTemplate, playerTemplate);
            var rootCanvas = GetComponent<Canvas>();
            runtimeFacade.Configure(
                dialogueRunner,
                rootCanvas,
                panelFactory,
                backMask,
                newOptionsPresenter);

            if (Application.isPlaying && ensureDialogueStarts)
            {
                StartCoroutine(EnsureDialogueStarted(dialogueRunner));
            }
        }

        private static SignDialoguePanelInstance PrepareTemplate(Transform parent, string templateName)
        {
            var templateTransform = FindByName(parent, templateName);
            if (templateTransform == null)
            {
                return null;
            }

            var template = templateTransform.GetComponent<SignDialoguePanelInstance>();
            if (template == null)
            {
                template = templateTransform.gameObject.AddComponent<SignDialoguePanelInstance>();
            }

            template.ResolveReferences();
            template.ConfigureAnimationsForManualControl();
            templateTransform.gameObject.SetActive(false);
            return template;
        }

        private static void ConfigureDialogueRunner(
            DialogueRunner dialogueRunner,
            TALinePresenter linePresenter,
            SignDialogueOptionsPresenter optionsPresenter)
        {
            var presenters = new List<DialoguePresenterBase?>();
            foreach (var presenter in dialogueRunner.DialoguePresenters)
            {
                if (presenter == null)
                {
                    continue;
                }

                if (presenter is OptionsPresenter)
                {
                    continue;
                }

                if (!presenters.Contains(presenter))
                {
                    presenters.Add(presenter);
                }
            }

            if (!presenters.Contains(linePresenter))
            {
                presenters.Insert(0, linePresenter);
            }

            if (!presenters.Contains(optionsPresenter))
            {
                presenters.Add(optionsPresenter);
            }

            dialogueRunner.DialoguePresenters = presenters.Where(presenter => presenter != null);
        }

        private static void SetAllowOptionFallthrough(DialogueRunner dialogueRunner, bool value)
        {
            var field = typeof(DialogueRunner).GetField("allowOptionFallthrough", BindingFlags.Instance | BindingFlags.NonPublic);
            field?.SetValue(dialogueRunner, value);
        }

        private static Transform FindByName(Transform root, string targetName)
        {
            if (root == null)
            {
                return null;
            }

            var transforms = root.GetComponentsInChildren<Transform>(true);
            foreach (var candidate in transforms)
            {
                if (candidate != null && candidate.name == targetName)
                {
                    return candidate;
                }
            }

            return null;
        }

        private IEnumerator EnsureDialogueStarted(DialogueRunner dialogueRunner)
        {
            if (dialogueRunner == null)
            {
                yield break;
            }

            if (startKickDelaySeconds > 0f)
            {
                yield return new WaitForSecondsRealtime(startKickDelaySeconds);
            }
            else
            {
                yield return null;
            }

            if (dialogueRunner == null || dialogueRunner.IsDialogueRunning)
            {
                yield break;
            }

            var nodeName = string.IsNullOrWhiteSpace(fallbackStartNode) ? "Start" : fallbackStartNode.Trim();
            _ = dialogueRunner.StartDialogue(nodeName);
        }
    }
}
