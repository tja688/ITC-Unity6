using QFramework;

namespace ITC.Dialogue
{
    public sealed class DialogueSceneApp : Architecture<DialogueSceneApp>
    {
        protected override void Init()
        {
            RegisterModel(new DialogueSceneStateModel());
        }
    }

    public sealed class DialogueSceneStateModel : AbstractModel
    {
        public readonly BindableProperty<bool> ResKitReady = new(false);
        public readonly BindableProperty<bool> DialogueSystemSpawned = new(false);
        public readonly BindableProperty<bool> DialogueRunning = new(false);
        public readonly BindableProperty<bool> VisualLayerReady = new(false);
        public readonly BindableProperty<bool> SpawnFallbackUsed = new(false);
        public readonly BindableProperty<string> ActiveBackgroundKey = new(string.Empty);
        public readonly BindableProperty<string> ActivePortraitKey = new(string.Empty);

        protected override void OnInit()
        {
        }
    }

    public sealed class MarkDialogueResReadyCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<DialogueSceneStateModel>().ResKitReady.Value = true;
        }
    }

    public sealed class MarkDialogueSystemSpawnedCommand : AbstractCommand
    {
        public readonly bool UsedFallback;

        public MarkDialogueSystemSpawnedCommand(bool usedFallback)
        {
            UsedFallback = usedFallback;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<DialogueSceneStateModel>();
            model.DialogueSystemSpawned.Value = true;
            model.SpawnFallbackUsed.Value = UsedFallback;
        }
    }

    public sealed class MarkDialogueVisualLayerReadyCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<DialogueSceneStateModel>().VisualLayerReady.Value = true;
        }
    }

    public sealed class MarkDialogueRunningCommand : AbstractCommand
    {
        protected override void OnExecute()
        {
            this.GetModel<DialogueSceneStateModel>().DialogueRunning.Value = true;
        }
    }

    public sealed class UpdateDialogueVisualStateCommand : AbstractCommand
    {
        private readonly string backgroundKey;
        private readonly string portraitKey;

        public UpdateDialogueVisualStateCommand(string backgroundKey, string portraitKey)
        {
            this.backgroundKey = backgroundKey;
            this.portraitKey = portraitKey;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<DialogueSceneStateModel>();

            if (!string.IsNullOrEmpty(backgroundKey))
            {
                model.ActiveBackgroundKey.Value = backgroundKey;
            }

            if (portraitKey != null)
            {
                model.ActivePortraitKey.Value = portraitKey;
            }
        }
    }
}
