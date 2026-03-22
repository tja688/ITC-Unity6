using QFramework;

namespace ITC.SignMiniGame
{
    public sealed class SignMiniGameFlowStateModel : AbstractModel
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
        public readonly BindableProperty<int> RouteSoulCollectPercent = new(0);
        public readonly BindableProperty<int> RouteSoulMin = new(0);
        public readonly BindableProperty<int> RouteSoulMax = new(0);
        public readonly BindableProperty<float> SoulCollectActualRawPercent = new(0f);
        public readonly BindableProperty<bool> SoulCollectRunning = new(false);
        public readonly BindableProperty<bool> SoulCollectCompleted = new(false);
        public readonly BindableProperty<string> RouteBeanSellResult = new("skipped");
        public readonly BindableProperty<int> RouteBeanSoldCount = new(0);
        public readonly BindableProperty<string> RouteBeanPitchType = new("none");
        public readonly BindableProperty<int> BeanSellCurrentDay = new(1);
        public readonly BindableProperty<bool> BeanSellRunning = new(false);
        public readonly BindableProperty<bool> BeanSellCompleted = new(false);
        public readonly BindableProperty<int> Money = new(0);
        public readonly BindableProperty<int> NumberOfSignMistake = new(0);
        public readonly BindableProperty<int> GlobalSignMistake = new(0);
        public readonly BindableProperty<int> RouteSettlementTip = new(0);
        public readonly BindableProperty<string> RouteSettlementTier = new("neutral");
        public readonly BindableProperty<int> RouteSettlementFinalSatisfaction = new(3);
        public readonly BindableProperty<bool> SettlementRunning = new(false);
        public readonly BindableProperty<bool> SettlementCompleted = new(false);
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
