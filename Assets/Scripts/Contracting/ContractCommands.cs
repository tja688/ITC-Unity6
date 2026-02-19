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

    public sealed class BeginSoulCollectCommand : AbstractCommand
    {
        private readonly int clientId;
        private readonly float currentSatisfaction;
        private readonly int currentSignMistake;
        private readonly int targetPercent;
        private readonly int minPercent;
        private readonly int maxPercent;

        public BeginSoulCollectCommand(
            int clientId,
            float currentSatisfaction,
            int currentSignMistake,
            int targetPercent,
            int minPercent,
            int maxPercent)
        {
            this.clientId = Mathf.Max(1, clientId);
            this.currentSatisfaction = Mathf.Max(0f, currentSatisfaction);
            this.currentSignMistake = Mathf.Max(0, currentSignMistake);
            this.minPercent = Mathf.Clamp(minPercent, 0, 100);
            this.maxPercent = Mathf.Clamp(maxPercent, this.minPercent, 100);
            this.targetPercent = Mathf.Clamp(targetPercent, this.minPercent, this.maxPercent);
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<ContractFlowStateModel>();
            model.CurrentClientId.Value = clientId;
            model.BaselineSatisfaction.Value = currentSatisfaction;
            model.BaselineSignMistake.Value = currentSignMistake;
            model.Satisfaction.Value = currentSatisfaction;
            model.SignMistake.Value = currentSignMistake;
            model.RouteSoulCollectPercent.Value = targetPercent;
            model.RouteSoulMin.Value = minPercent;
            model.RouteSoulMax.Value = maxPercent;
            model.SoulCollectActualRawPercent.Value = targetPercent;
            model.SoulCollectCompleted.Value = false;
            model.SoulCollectRunning.Value = true;
        }
    }

    public sealed class SubmitSoulCollectResultCommand : AbstractCommand
    {
        private readonly SoulCollectResultPayload payload;

        public SubmitSoulCollectResultCommand(SoulCollectResultPayload payload)
        {
            this.payload = payload;
        }

        protected override void OnExecute()
        {
            var flowState = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var ruleConfig = configModel.SoulCollectRuleConfig ?? new SoulCollectConfig();

            var minPercent = Mathf.Clamp(payload.MinPercent, 0, 100);
            var maxPercent = Mathf.Clamp(payload.MaxPercent, minPercent, 100);
            var actualPercent = Mathf.Clamp(payload.ActualPercent, 0, 100);
            var actualRaw = Mathf.Clamp(payload.ActualRawFloat, 0f, 100f);

            var satisfaction = flowState.BaselineSatisfaction.Value;
            var signMistake = flowState.BaselineSignMistake.Value;

            if (actualPercent < minPercent)
            {
                if (ruleConfig.TooLowAsMistake)
                {
                    signMistake = 1;
                }
            }
            else if (actualPercent > maxPercent)
            {
                var penalty = Mathf.Max(0, ruleConfig.TooHighSatisfactionPenalty);
                satisfaction = Mathf.Max(0f, satisfaction - penalty);
            }

            if (payload.WasFallback)
            {
                LogKit.W($"[SoulCollect] Fallback result used for client {payload.ClientId}.");
            }

            flowState.RouteSoulCollectPercent.Value = actualPercent;
            flowState.RouteSoulMin.Value = minPercent;
            flowState.RouteSoulMax.Value = maxPercent;
            flowState.SoulCollectActualRawPercent.Value = actualRaw;
            flowState.Satisfaction.Value = satisfaction;
            flowState.SignMistake.Value = signMistake;
            flowState.SoulCollectRunning.Value = false;
            flowState.SoulCollectCompleted.Value = true;
        }
    }

    public sealed class BeginBeanSellCommand : AbstractCommand
    {
        private readonly int clientId;
        private readonly int day;
        private readonly float currentSatisfaction;
        private readonly int currentSignMistake;
        private readonly int currentSoldCount;

        public BeginBeanSellCommand(
            int clientId,
            int day,
            float currentSatisfaction,
            int currentSignMistake,
            int currentSoldCount)
        {
            this.clientId = Mathf.Max(1, clientId);
            this.day = Mathf.Max(1, day);
            this.currentSatisfaction = Mathf.Max(0f, currentSatisfaction);
            this.currentSignMistake = Mathf.Max(0, currentSignMistake);
            this.currentSoldCount = Mathf.Max(0, currentSoldCount);
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var ruleConfig = configModel.BeanSellRuleConfig ?? new BeanSellConfig();

            var soldCount = currentSoldCount;
            if (ruleConfig.DailyResetOnDayChange && model.BeanSellCurrentDay.Value != day)
            {
                soldCount = 0;
            }

            model.CurrentClientId.Value = clientId;
            model.BaselineSatisfaction.Value = currentSatisfaction;
            model.BaselineSignMistake.Value = currentSignMistake;
            model.Satisfaction.Value = currentSatisfaction;
            model.SignMistake.Value = currentSignMistake;
            model.BeanSellCurrentDay.Value = day;
            model.RouteBeanSoldCount.Value = soldCount;
            model.RouteBeanSellResult.Value = "skipped";
            model.RouteBeanPitchType.Value = "none";
            model.BeanSellCompleted.Value = false;
            model.BeanSellRunning.Value = true;
        }
    }

    public sealed class SubmitBeanSellResultCommand : AbstractCommand
    {
        private readonly BeanSellResultPayload payload;

        public SubmitBeanSellResultCommand(BeanSellResultPayload payload)
        {
            this.payload = payload;
        }

        protected override void OnExecute()
        {
            var flowState = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var ruleConfig = configModel.BeanSellRuleConfig ?? new BeanSellConfig();

            var soldCount = Mathf.Max(0, flowState.RouteBeanSoldCount.Value);
            var satisfaction = flowState.BaselineSatisfaction.Value;
            var signMistake = flowState.BaselineSignMistake.Value;

            var result = "skipped";
            if (!payload.IsSkipped)
            {
                if (payload.IsSuccess)
                {
                    soldCount += 1;
                    result = "success";
                }
                else
                {
                    var penalty = Mathf.Max(0, ruleConfig.FailSatisfactionPenalty);
                    satisfaction = Mathf.Max(0f, satisfaction - penalty);
                    result = "failed";
                }
            }

            if (payload.WasFallback)
            {
                LogKit.W($"[BeanSell] Fallback result used for client {payload.ClientId}.");
            }

            flowState.BeanSellCurrentDay.Value = Mathf.Max(1, payload.Day);
            flowState.RouteBeanSellResult.Value = result;
            flowState.RouteBeanPitchType.Value = ToYarnBeanPitch(payload.PitchType);
            flowState.RouteBeanSoldCount.Value = soldCount;
            flowState.Satisfaction.Value = satisfaction;
            flowState.SignMistake.Value = signMistake;
            flowState.BeanSellRunning.Value = false;
            flowState.BeanSellCompleted.Value = true;
        }

        private static string ToYarnBeanPitch(BeanPitchType pitchType)
        {
            return pitchType switch
            {
                BeanPitchType.StrongPush => "strong",
                BeanPitchType.Empathy => "empathy",
                BeanPitchType.Benefit => "benefit",
                _ => "none"
            };
        }
    }

    public sealed class BeginSettlementCommand : AbstractCommand
    {
        private readonly int clientId;
        private readonly int day;
        private readonly int currentSatisfaction;
        private readonly int currentSignMistake;
        private readonly int currentMoney;
        private readonly int currentNumberOfSignMistake;
        private readonly int currentGlobalSignMistake;

        public BeginSettlementCommand(
            int clientId,
            int day,
            int currentSatisfaction,
            int currentSignMistake,
            int currentMoney,
            int currentNumberOfSignMistake,
            int currentGlobalSignMistake)
        {
            this.clientId = Mathf.Max(1, clientId);
            this.day = Mathf.Max(1, day);
            this.currentSatisfaction = Mathf.Max(0, currentSatisfaction);
            this.currentSignMistake = Mathf.Max(0, currentSignMistake);
            this.currentMoney = Mathf.Max(0, currentMoney);
            this.currentNumberOfSignMistake = Mathf.Max(0, currentNumberOfSignMistake);
            this.currentGlobalSignMistake = Mathf.Max(0, currentGlobalSignMistake);
        }

        protected override void OnExecute()
        {
            var model = this.GetModel<ContractFlowStateModel>();
            model.CurrentClientId.Value = clientId;
            model.BaselineSatisfaction.Value = currentSatisfaction;
            model.BaselineSignMistake.Value = currentSignMistake;
            model.Satisfaction.Value = currentSatisfaction;
            model.SignMistake.Value = currentSignMistake;
            model.BeanSellCurrentDay.Value = day;
            model.Money.Value = currentMoney;
            model.NumberOfSignMistake.Value = currentNumberOfSignMistake;
            model.GlobalSignMistake.Value = currentGlobalSignMistake;
            model.RouteSettlementTip.Value = 0;
            model.RouteSettlementTier.Value = "neutral";
            model.RouteSettlementFinalSatisfaction.Value = currentSatisfaction;
            model.SettlementCompleted.Value = false;
            model.SettlementRunning.Value = true;
        }
    }

    public sealed class FinalizeClientContractCommand : AbstractCommand
    {
        private readonly int day;

        public FinalizeClientContractCommand(int day)
        {
            this.day = Mathf.Max(1, day);
        }

        protected override void OnExecute()
        {
            var flowState = this.GetModel<ContractFlowStateModel>();
            var configModel = this.GetModel<ContractClientConfigModel>();
            var ruleConfig = configModel.BuildSettlementRuntimeConfig();
            var clientConfig = configModel.GetSettlementClientConfig(flowState.CurrentClientId.Value);

            var minSatisfaction = Mathf.Clamp(ruleConfig.SatisfactionClampMin, 0, 5);
            var maxSatisfaction = Mathf.Clamp(ruleConfig.SatisfactionClampMax, minSatisfaction, 5);
            var finalSatisfaction = Mathf.Clamp(Mathf.RoundToInt(flowState.Satisfaction.Value), minSatisfaction, maxSatisfaction);
            var signMistake = Mathf.Clamp(flowState.SignMistake.Value, 0, 1);

            var tipAmount = ResolveTipAmount(
                ruleConfig,
                clientConfig != null ? clientConfig.SocialClass : ContractSocialClass.Worker,
                day,
                flowState.CurrentClientId.Value,
                finalSatisfaction);

            var numberOfSignMistake = Mathf.Max(0, flowState.NumberOfSignMistake.Value);
            var globalSignMistake = Mathf.Max(0, flowState.GlobalSignMistake.Value);
            if (signMistake > 0)
            {
                numberOfSignMistake += 1;
                globalSignMistake += 1;
            }

            var tier = ResolveTier(ruleConfig, finalSatisfaction, signMistake, tipAmount);
            var totalMoney = Mathf.Max(0, flowState.Money.Value + tipAmount);

            flowState.Satisfaction.Value = finalSatisfaction;
            flowState.RouteSettlementFinalSatisfaction.Value = finalSatisfaction;
            flowState.RouteSettlementTip.Value = tipAmount;
            flowState.RouteSettlementTier.Value = ToYarnTier(tier);
            flowState.Money.Value = totalMoney;
            flowState.NumberOfSignMistake.Value = numberOfSignMistake;
            flowState.GlobalSignMistake.Value = globalSignMistake;
            flowState.SettlementRunning.Value = false;
            flowState.SettlementCompleted.Value = true;
        }

        private static int ResolveTipAmount(
            ContractSettlementConfig ruleConfig,
            ContractSocialClass socialClass,
            int day,
            int clientId,
            int satisfaction)
        {
            if (satisfaction < ruleConfig.SatisfactionTipThreshold)
            {
                return 0;
            }

            var range = ruleConfig.GetTipRange(socialClass) ?? new SettlementTipRange();
            var min = Mathf.Max(0, range.Min);
            var max = Mathf.Max(min, range.Max);
            var span = max - min + 1;
            if (span <= 0)
            {
                return min;
            }

            var seed = Mathf.Abs(day * 37 + clientId * 53 + satisfaction * 11);
            return min + seed % span;
        }

        private static SettlementTier ResolveTier(
            ContractSettlementConfig ruleConfig,
            int satisfaction,
            int signMistake,
            int tipAmount)
        {
            if (signMistake > 0)
            {
                return SettlementTier.Bad;
            }

            if (tipAmount > 0 && satisfaction >= ruleConfig.SatisfactionTipThreshold + 1)
            {
                return SettlementTier.Good;
            }

            if (satisfaction < ruleConfig.SatisfactionTipThreshold)
            {
                return SettlementTier.Bad;
            }

            return SettlementTier.Neutral;
        }

        private static string ToYarnTier(SettlementTier tier)
        {
            return tier switch
            {
                SettlementTier.Good => "good",
                SettlementTier.Bad => "bad",
                _ => "neutral"
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
