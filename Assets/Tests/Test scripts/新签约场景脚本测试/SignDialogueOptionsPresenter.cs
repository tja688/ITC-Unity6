using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using UnityEngine;
using Yarn.Unity;

namespace ITC.Dialogue
{
    [DisallowMultipleComponent]
    public sealed class SignDialogueOptionsPresenter : DialoguePresenterBase
    {
        [SerializeField] private OptionItem optionViewPrefab;
        [SerializeField] private bool showUnavailableOptions = false;

        private SignDialogueRuntimeFacade runtimeFacade;
        private readonly List<OptionItem> activeOptions = new();
        private SignDialoguePanelInstance activePanel;

        public bool HasActiveOptions => activeOptions.Count > 0;

        public void Bind(SignDialogueRuntimeFacade facade)
        {
            runtimeFacade = facade;
            TryCopyLegacyPrefab();
        }

        private void Awake()
        {
            if (runtimeFacade == null)
            {
                runtimeFacade = FindFirstObjectByType<SignDialogueRuntimeFacade>();
            }

            TryCopyLegacyPrefab();
        }

        public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token)
        {
            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueStartedAsync()
        {
            ClearActiveOptions(immediate: true);
            return YarnTask.CompletedTask;
        }

        public override YarnTask OnDialogueCompleteAsync()
        {
            ClearActiveOptions(immediate: false);
            return YarnTask.CompletedTask;
        }

        public override async YarnTask<DialogueOption?> RunOptionsAsync(DialogueOption[] dialogueOptions, LineCancellationToken cancellationToken)
        {
            if (runtimeFacade == null || optionViewPrefab == null)
            {
                return await DialogueRunner.NoOptionSelected;
            }

            var hasAvailableOption = false;
            foreach (var option in dialogueOptions)
            {
                if (option.IsAvailable)
                {
                    hasAvailableOption = true;
                    break;
                }
            }

            if (!hasAvailableOption)
            {
                return null;
            }

            ClearActiveOptions(immediate: true);

            activePanel = runtimeFacade.ShowOptionsPanel();
            var optionRoot = activePanel.PrepareForOptions();

            var selectedOptionSource = new YarnTaskCompletionSource<DialogueOption?>();
            var completionCancellationSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken.NextContentToken);

            async YarnTask CancelSelectionWhenDialogueCancelled()
            {
                await YarnTask.WaitUntilCanceled(completionCancellationSource.Token);
                if (cancellationToken.IsNextContentRequested)
                {
                    selectedOptionSource.TrySetResult(null);
                }
            }

            CancelSelectionWhenDialogueCancelled().Forget();

            foreach (var option in dialogueOptions)
            {
                if (!option.IsAvailable && !showUnavailableOptions)
                {
                    continue;
                }

                var optionView = Object.Instantiate(optionViewPrefab, optionRoot, false);
                optionView.gameObject.SetActive(true);
                optionView.Option = option;
                optionView.OnOptionSelected = selectedOptionSource;
                optionView.completionToken = completionCancellationSource.Token;
                activeOptions.Add(optionView);
            }

            var selectedOption = await selectedOptionSource.Task;
            completionCancellationSource.Cancel();
            completionCancellationSource.Dispose();

            ClearActiveOptions(immediate: false);

            if (cancellationToken.NextContentToken.IsCancellationRequested)
            {
                return await DialogueRunner.NoOptionSelected;
            }

            return selectedOption;
        }

        private void ClearActiveOptions(bool immediate)
        {
            foreach (var option in activeOptions)
            {
                if (option != null)
                {
                    Object.Destroy(option.gameObject);
                }
            }

            activeOptions.Clear();

            if (activePanel != null)
            {
                if (immediate)
                {
                    activePanel.DestroyImmediateSafe();
                }
                else
                {
                    activePanel.PlayHideAndDestroy();
                }

                activePanel = null;
            }
        }

        private void TryCopyLegacyPrefab()
        {
            if (optionViewPrefab != null)
            {
                return;
            }

            var legacyPresenter = GetComponent<OptionsPresenter>();
            if (legacyPresenter == null)
            {
                return;
            }

            var prefabField = typeof(OptionsPresenter).GetField("optionViewPrefab", BindingFlags.Instance | BindingFlags.NonPublic);
            if (prefabField != null)
            {
                optionViewPrefab = prefabField.GetValue(legacyPresenter) as OptionItem;
            }
        }
    }
}
