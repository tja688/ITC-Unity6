using QFramework;

namespace ITC.Contracting
{
    public sealed class ContractFlowStateModel : AbstractModel
    {
        public readonly BindableProperty<int> CurrentClientId = new(0);
        public readonly BindableProperty<float> BaselineSatisfaction = new(3f);
        public readonly BindableProperty<int> BaselineSignMistake = new(0);
        public readonly BindableProperty<float> Satisfaction = new(3f);
        public readonly BindableProperty<int> SignMistake = new(0);
        public readonly BindableProperty<string> RouteDocReviewResult = new("passed");
        public readonly BindableProperty<string> RouteStampType = new("事件");
        public readonly BindableProperty<string> RouteStampTimingResult = new("normal");
        public readonly BindableProperty<int> LastInspectCount = new(0);
        public readonly BindableProperty<float> LastStampHitNormalized = new(0f);
        public readonly BindableProperty<bool> DocumentReviewRunning = new(false);
        public readonly BindableProperty<bool> DocumentReviewCompleted = new(false);
        public readonly BindableProperty<bool> StampRunning = new(false);
        public readonly BindableProperty<bool> StampCompleted = new(false);
        public readonly BindableProperty<int> RouteQteErrorCount = new(0);
        public readonly BindableProperty<int> RuneTypingGridSize = new(4);
        public readonly BindableProperty<bool> RuneTypingRunning = new(false);
        public readonly BindableProperty<bool> RuneTypingCompleted = new(false);
        public readonly BindableProperty<string> RouteRuneVerifyResult = new("skipped");
        public readonly BindableProperty<int> RouteRuneVerifyDebuff = new(0);
        public readonly BindableProperty<int> RuneVerifyFoundCount = new(0);
        public readonly BindableProperty<int> RuneVerifyDistortedCount = new(0);
        public readonly BindableProperty<int> RuneVerifySeed = new(0);
        public readonly BindableProperty<bool> RuneVerifyRunning = new(false);
        public readonly BindableProperty<bool> RuneVerifyCompleted = new(false);

        protected override void OnInit()
        {
        }
    }
}
