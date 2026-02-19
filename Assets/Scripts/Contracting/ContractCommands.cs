using QFramework;
using UnityEngine;

namespace ITC.Contracting
{
    public sealed class BeginDocumentReviewCommand : AbstractCommand
    {
        private readonly int clientId;
        private readonly float currentSatisfaction;
        private readonly int currentSignMistake;

        public BeginDocumentReviewCommand(int clientId, float currentSatisfaction, int currentSignMistake)
        {
            this.clientId = Mathf.Max(1, clientId);
            this.currentSatisfaction = Mathf.Max(0f, currentSatisfaction);
            this.currentSignMistake = Mathf.Max(0, currentSignMistake);
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<ContractFlowStateModel>();
            model.CurrentClientId.Value = clientId;
            model.BaselineSatisfaction.Value = currentSatisfaction;
            model.BaselineSignMistake.Value = currentSignMistake;
            model.Satisfaction.Value = currentSatisfaction;
            model.SignMistake.Value = currentSignMistake;
            model.RouteDocReviewResult.Value = "passed";
            model.LastInspectCount.Value = 0;
            model.DocumentReviewCompleted.Value = false;
            model.DocumentReviewRunning.Value = true;
        }
    }

    public sealed class SubmitDocumentReviewResultCommand : AbstractCommand
    {
        private readonly DocumentReviewResultPayload payload;

        public SubmitDocumentReviewResultCommand(DocumentReviewResultPayload payload)
        {
            this.payload = payload;
        }

        protected override void OnExecute()
        {
            var flowState = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var clientConfig = configModel.GetDocumentReviewClientConfig(payload.ClientId);
            var ruleConfig = configModel.DocumentReviewRuleConfig;

            var isCorrect = payload.FinalAction == clientConfig.CorrectDecision;
            var routeResult = ResolveRouteResult(payload.FinalAction, isCorrect);

            var satisfaction = flowState.BaselineSatisfaction.Value;
            var signMistake = flowState.BaselineSignMistake.Value;

            if (!isCorrect)
            {
                if (ruleConfig.WrongPenaltyMistakeFlag)
                {
                    signMistake = 1;
                }

                var penalty = Mathf.Max(0, ruleConfig.WrongPenaltySatisfaction);
                satisfaction = Mathf.Max(0f, satisfaction - penalty);
            }

            if (payload.WasFallback)
            {
                LogKit.W($"[DocumentReview] Fallback result used for client {payload.ClientId}.");
            }

            flowState.RouteDocReviewResult.Value = routeResult;
            flowState.LastInspectCount.Value = Mathf.Max(0, payload.InspectedHotspotCount);
            flowState.Satisfaction.Value = satisfaction;
            flowState.SignMistake.Value = signMistake;
            flowState.DocumentReviewRunning.Value = false;
            flowState.DocumentReviewCompleted.Value = true;
        }

        private static string ResolveRouteResult(DocumentReviewDecision finalAction, bool isCorrect)
        {
            if (isCorrect && finalAction == DocumentReviewDecision.Pass)
            {
                return "passed";
            }

            if (isCorrect && finalAction == DocumentReviewDecision.Reject)
            {
                return "rejected_correct";
            }

            return "rejected_wrong";
        }
    }
}

