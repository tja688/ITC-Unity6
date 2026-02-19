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
        public readonly BindableProperty<int> LastInspectCount = new(0);
        public readonly BindableProperty<bool> DocumentReviewRunning = new(false);
        public readonly BindableProperty<bool> DocumentReviewCompleted = new(false);

        protected override void OnInit()
        {
        }
    }
}

