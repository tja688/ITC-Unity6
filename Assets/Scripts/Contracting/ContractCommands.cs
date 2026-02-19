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

    public sealed class BeginStampSelectionCommand : AbstractCommand
    {
        private readonly int clientId;
        private readonly float currentSatisfaction;
        private readonly int currentSignMistake;

        public BeginStampSelectionCommand(int clientId, float currentSatisfaction, int currentSignMistake)
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
            model.RouteStampType.Value = "事件";
            model.RouteStampTimingResult.Value = "normal";
            model.LastStampHitNormalized.Value = 0f;
            model.StampCompleted.Value = false;
            model.StampRunning.Value = true;
        }
    }

    public sealed class SubmitStampSelectionResultCommand : AbstractCommand
    {
        private readonly StampResultPayload payload;

        public SubmitStampSelectionResultCommand(StampResultPayload payload)
        {
            this.payload = payload;
        }

        protected override void OnExecute()
        {
            var flowState = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var clientConfig = configModel.GetStampClientConfig(payload.ClientId);
            var ruleConfig = configModel.StampRuleConfig;

            var selectedType = payload.SelectedStampType;
            var timingResult = payload.TimingResult;
            var typeCorrect = selectedType == clientConfig.CorrectStampType;

            var satisfaction = flowState.BaselineSatisfaction.Value;
            var signMistake = flowState.BaselineSignMistake.Value;

            if (!typeCorrect)
            {
                signMistake = 1;
                timingResult = StampTimingResult.Failed;
            }
            else
            {
                switch (timingResult)
                {
                    case StampTimingResult.Perfect:
                        satisfaction += 1f;
                        break;
                    case StampTimingResult.Failed:
                    {
                        var penalty = Mathf.Max(0, ruleConfig.TimingFailPenalty);
                        satisfaction = Mathf.Max(0f, satisfaction - penalty);
                        break;
                    }
                }
            }

            if (payload.WasFallback)
            {
                LogKit.W($"[Stamp] Fallback result used for client {payload.ClientId}.");
            }

            flowState.RouteStampType.Value = ToYarnStampType(selectedType);
            flowState.RouteStampTimingResult.Value = ToYarnTimingResult(timingResult);
            flowState.LastStampHitNormalized.Value = Mathf.Clamp01(payload.HitNormalizedTime);
            flowState.Satisfaction.Value = satisfaction;
            flowState.SignMistake.Value = signMistake;
            flowState.StampRunning.Value = false;
            flowState.StampCompleted.Value = true;
        }

        private static string ToYarnStampType(StampType stampType)
        {
            return stampType switch
            {
                StampType.Money => "金钱",
                StampType.Fame => "名利",
                StampType.Skill => "特技",
                StampType.Event => "事件",
                _ => "事件"
            };
        }

        private static string ToYarnTimingResult(StampTimingResult timingResult)
        {
            return timingResult switch
            {
                StampTimingResult.Perfect => "perfect",
                StampTimingResult.Normal => "normal",
                _ => "failed"
            };
        }
    }

    public sealed class BeginRuneTypingCommand : AbstractCommand
    {
        private readonly int clientId;
        private readonly float currentSatisfaction;
        private readonly int currentSignMistake;
        private readonly int gridSize;

        public BeginRuneTypingCommand(int clientId, float currentSatisfaction, int currentSignMistake, int gridSize)
        {
            this.clientId = Mathf.Max(1, clientId);
            this.currentSatisfaction = Mathf.Max(0f, currentSatisfaction);
            this.currentSignMistake = Mathf.Max(0, currentSignMistake);
            this.gridSize = Mathf.Clamp(gridSize, 4, 5);
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<ContractFlowStateModel>();
            model.CurrentClientId.Value = clientId;
            model.BaselineSatisfaction.Value = currentSatisfaction;
            model.BaselineSignMistake.Value = currentSignMistake;
            model.Satisfaction.Value = currentSatisfaction;
            model.SignMistake.Value = currentSignMistake;
            model.RouteQteErrorCount.Value = 0;
            model.RuneTypingGridSize.Value = gridSize;
            model.RuneTypingCompleted.Value = false;
            model.RuneTypingRunning.Value = true;
        }
    }

    public sealed class SubmitRuneTypingResultCommand : AbstractCommand
    {
        private readonly RuneTypingResultPayload payload;

        public SubmitRuneTypingResultCommand(RuneTypingResultPayload payload)
        {
            this.payload = payload;
        }

        protected override void OnExecute()
        {
            var flowState = this.GetModel<ContractFlowStateModel>();
            flowState.RouteQteErrorCount.Value = Mathf.Max(0, payload.ErrorCount);
            flowState.RuneTypingGridSize.Value = Mathf.Clamp(payload.GridSize, 4, 5);
            flowState.RuneTypingRunning.Value = false;
            flowState.RuneTypingCompleted.Value = true;

            if (payload.WasFallback)
            {
                LogKit.W($"[RuneTyping] Fallback result used for client {payload.ClientId}.");
            }
        }
    }

    public sealed class BeginRuneVerifyCommand : AbstractCommand
    {
        private readonly int clientId;
        private readonly bool triggered;
        private readonly int randomSeed;

        public BeginRuneVerifyCommand(int clientId, bool triggered, int randomSeed)
        {
            this.clientId = Mathf.Max(1, clientId);
            this.triggered = triggered;
            this.randomSeed = randomSeed;
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<ContractFlowStateModel>();
            model.CurrentClientId.Value = clientId;
            model.RuneVerifySeed.Value = randomSeed;
            model.RuneVerifyFoundCount.Value = 0;
            model.RuneVerifyDistortedCount.Value = 0;
            model.RouteRuneVerifyDebuff.Value = 0;

            if (triggered)
            {
                model.RouteRuneVerifyResult.Value = "failed";
                model.RuneVerifyRunning.Value = true;
                model.RuneVerifyCompleted.Value = false;
                return;
            }

            model.RouteRuneVerifyResult.Value = "skipped";
            model.RuneVerifyRunning.Value = false;
            model.RuneVerifyCompleted.Value = true;
        }
    }

    public sealed class SubmitRuneVerifyResultCommand : AbstractCommand
    {
        private readonly RuneVerifyResultPayload payload;

        public SubmitRuneVerifyResultCommand(RuneVerifyResultPayload payload)
        {
            this.payload = payload;
        }

        protected override void OnExecute()
        {
            var flowState = this.GetModel<ContractFlowStateModel>();

            if (payload.WasTimeoutFallback)
            {
                LogKit.W($"[RuneVerify] Timeout fallback result used for client {payload.ClientId}.");
            }

            var resultString = ResolveRouteResult(payload.Result);
            flowState.RouteRuneVerifyResult.Value = resultString;
            flowState.RouteRuneVerifyDebuff.Value = payload.Result == RuneVerifyResultType.Failed ? 1 : 0;
            flowState.RuneVerifyFoundCount.Value = Mathf.Max(0, payload.FoundCount);
            flowState.RuneVerifyDistortedCount.Value = Mathf.Max(0, payload.DistortedCount);
            flowState.RuneVerifySeed.Value = payload.RandomSeed;
            flowState.RuneVerifyRunning.Value = false;
            flowState.RuneVerifyCompleted.Value = true;
        }

        private static string ResolveRouteResult(RuneVerifyResultType result)
        {
            return result switch
            {
                RuneVerifyResultType.Success => "success",
                RuneVerifyResultType.Failed => "failed",
                _ => "skipped"
            };
        }
    }
}
