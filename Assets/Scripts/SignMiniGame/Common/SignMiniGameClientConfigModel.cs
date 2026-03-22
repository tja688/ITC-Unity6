using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace ITC.SignMiniGame
{
    public sealed class SignMiniGameClientConfigModel : AbstractModel
    {
        private readonly Dictionary<int, DocumentReviewClientConfig> documentReviewClientConfigs = new();
        private readonly Dictionary<int, StampClientConfig> stampClientConfigs = new();
        private readonly Dictionary<int, SoulCollectClientConfig> soulCollectClientConfigs = new();
        private readonly Dictionary<int, BeanSellClientConfig> beanSellClientConfigs = new();
        private readonly Dictionary<int, SettlementClientConfig> settlementClientConfigs = new();
        private readonly Dictionary<int, RuneTypingRoundConfig> runeTypingClientConfigs = new();
        private DocumentReviewConfig documentReviewRuleConfig = new();
        private StampConfig stampRuleConfig = new();
        private SoulCollectConfig soulCollectRuleConfig = new();
        private BeanSellConfig beanSellRuleConfig = new();
        private SignMiniGameSettlementConfig settlementRuleConfig = new();
        private RuneTypingConfig runeTypingRuleConfig = new();
        private RuneVerifyConfig runeVerifyRuleConfig = new();

        public DocumentReviewConfig DocumentReviewRuleConfig => documentReviewRuleConfig;
        public StampConfig StampRuleConfig => stampRuleConfig;
        public SoulCollectConfig SoulCollectRuleConfig => soulCollectRuleConfig;
        public BeanSellConfig BeanSellRuleConfig => beanSellRuleConfig;
        public SignMiniGameSettlementConfig SettlementRuleConfig => settlementRuleConfig;
        public RuneTypingConfig RuneTypingRuleConfig => runeTypingRuleConfig;
        public RuneVerifyConfig RuneVerifyRuleConfig => runeVerifyRuleConfig;

        protected override void OnInit()
        {
            documentReviewRuleConfig = new DocumentReviewConfig();
            stampRuleConfig = new StampConfig();
            soulCollectRuleConfig = new SoulCollectConfig();
            beanSellRuleConfig = new BeanSellConfig
            {
                EnabledFromDay = 2,
                DailyResetOnDayChange = true,
                SuccessThreshold = 3,
                PitchTypeWeights = new BeanSellPitchWeights
                {
                    StrongPush = 0,
                    Empathy = 0,
                    Benefit = 0
                },
                FailSatisfactionPenalty = 1,
                ResolveStaySeconds = 0.75f,
                ShowPreferenceHint = true
            };
            settlementRuleConfig = new SignMiniGameSettlementConfig
            {
                SatisfactionTipThreshold = 3,
                SatisfactionClampMin = 0,
                SatisfactionClampMax = 5,
                PoorTipRange = new SettlementTipRange { Min = 0, Max = 1 },
                WorkerTipRange = new SettlementTipRange { Min = 1, Max = 3 },
                BourgeoisTipRange = new SettlementTipRange { Min = 3, Max = 5 },
                FeedbackDuration = 1.4f
            };
            runeTypingRuleConfig = new RuneTypingConfig();
            runeVerifyRuleConfig = new RuneVerifyConfig();
            documentReviewClientConfigs.Clear();
            stampClientConfigs.Clear();
            soulCollectClientConfigs.Clear();
            beanSellClientConfigs.Clear();
            settlementClientConfigs.Clear();
            runeTypingClientConfigs.Clear();

            documentReviewClientConfigs[1] = new DocumentReviewClientConfig
            {
                ClientId = 1,
                ClientDisplayName = "Emmett",
                DateDisplayText = "Day 1 / Window 13",
                InkType = "Black",
                SealIntegrity = true,
                PhotoMatchesClient = true,
                CorrectDecision = DocumentReviewDecision.Pass
            };

            documentReviewClientConfigs[2] = new DocumentReviewClientConfig
            {
                ClientId = 2,
                ClientDisplayName = "Thomas",
                DateDisplayText = "Day 1 / Window 13",
                InkType = "Black",
                SealIntegrity = true,
                PhotoMatchesClient = true,
                CorrectDecision = DocumentReviewDecision.Pass
            };

            documentReviewClientConfigs[3] = new DocumentReviewClientConfig
            {
                ClientId = 3,
                ClientDisplayName = "Bartholomew",
                DateDisplayText = "Day 1 / Window 13",
                InkType = "Red",
                SealIntegrity = true,
                PhotoMatchesClient = false,
                CorrectDecision = DocumentReviewDecision.Reject
            };

            documentReviewClientConfigs[4] = new DocumentReviewClientConfig
            {
                ClientId = 4,
                ClientDisplayName = "Reggie",
                DateDisplayText = "Day 1 / Window 13",
                InkType = "Black",
                SealIntegrity = true,
                PhotoMatchesClient = true,
                CorrectDecision = DocumentReviewDecision.Pass
            };

            stampClientConfigs[1] = new StampClientConfig
            {
                ClientId = 1,
                ClientDisplayName = "Emmett",
                CorrectStampType = StampType.Event
            };

            stampClientConfigs[2] = new StampClientConfig
            {
                ClientId = 2,
                ClientDisplayName = "Thomas",
                CorrectStampType = StampType.Skill
            };

            stampClientConfigs[3] = new StampClientConfig
            {
                ClientId = 3,
                ClientDisplayName = "Bartholomew",
                CorrectStampType = StampType.Skill
            };

            stampClientConfigs[4] = new StampClientConfig
            {
                ClientId = 4,
                ClientDisplayName = "Reggie",
                CorrectStampType = StampType.Event
            };

            soulCollectClientConfigs[1] = new SoulCollectClientConfig
            {
                ClientId = 1,
                ClientDisplayName = "Emmett",
                MinPercent = 40,
                MaxPercent = 40,
                DefaultTargetPercent = 40,
                SoulColor = new Color(0.47f, 0.84f, 1f, 1f),
                RevealFxKey = "vfx.contract.soul.transfer",
                VoiceReactionKey = "voice.contract.soul.emmett"
            };

            soulCollectClientConfigs[2] = new SoulCollectClientConfig
            {
                ClientId = 2,
                ClientDisplayName = "Thomas",
                MinPercent = 50,
                MaxPercent = 50,
                DefaultTargetPercent = 50,
                SoulColor = new Color(0.76f, 0.88f, 1f, 1f),
                RevealFxKey = "vfx.contract.soul.transfer",
                VoiceReactionKey = "voice.contract.soul.thomas"
            };

            soulCollectClientConfigs[3] = new SoulCollectClientConfig
            {
                ClientId = 3,
                ClientDisplayName = "Bartholomew",
                MinPercent = 50,
                MaxPercent = 50,
                DefaultTargetPercent = 50,
                SoulColor = new Color(0.85f, 0.82f, 1f, 1f),
                RevealFxKey = "vfx.contract.soul.transfer",
                VoiceReactionKey = "voice.contract.soul.sullivan"
            };

            soulCollectClientConfigs[4] = new SoulCollectClientConfig
            {
                ClientId = 4,
                ClientDisplayName = "Reggie",
                MinPercent = 45,
                MaxPercent = 45,
                DefaultTargetPercent = 45,
                SoulColor = new Color(0.82f, 0.91f, 1f, 1f),
                RevealFxKey = "vfx.contract.soul.transfer",
                VoiceReactionKey = "voice.contract.soul.reggie"
            };

            beanSellClientConfigs[1] = new BeanSellClientConfig
            {
                ClientId = 1,
                ClientDisplayName = "Emmett",
                BuyWillingness = 3,
                PreferredPitch = BeanPitchType.Benefit,
                SituationalModifier = 1
            };

            beanSellClientConfigs[2] = new BeanSellClientConfig
            {
                ClientId = 2,
                ClientDisplayName = "Thomas",
                BuyWillingness = 2,
                PreferredPitch = BeanPitchType.Empathy,
                SituationalModifier = 0
            };

            beanSellClientConfigs[3] = new BeanSellClientConfig
            {
                ClientId = 3,
                ClientDisplayName = "Bartholomew",
                BuyWillingness = 1,
                PreferredPitch = BeanPitchType.StrongPush,
                SituationalModifier = -1
            };

            settlementClientConfigs[1] = new SettlementClientConfig
            {
                ClientId = 1,
                ClientDisplayName = "Emmett",
                SocialClass = SignMiniGameSocialClass.Worker
            };

            settlementClientConfigs[2] = new SettlementClientConfig
            {
                ClientId = 2,
                ClientDisplayName = "Thomas",
                SocialClass = SignMiniGameSocialClass.Worker
            };

            settlementClientConfigs[3] = new SettlementClientConfig
            {
                ClientId = 3,
                ClientDisplayName = "Bartholomew",
                SocialClass = SignMiniGameSocialClass.Worker
            };

            settlementClientConfigs[4] = new SettlementClientConfig
            {
                ClientId = 4,
                ClientDisplayName = "Reggie",
                SocialClass = SignMiniGameSocialClass.Poor
            };

            runeTypingClientConfigs[1] = new RuneTypingRoundConfig
            {
                ClientId = 1,
                GridSize = 4,
                TargetSequence = new List<RuneInputDirection>
                {
                    RuneInputDirection.Up,
                    RuneInputDirection.Right,
                    RuneInputDirection.Down,
                    RuneInputDirection.Left
                }
            };

            runeTypingClientConfigs[2] = new RuneTypingRoundConfig
            {
                ClientId = 2,
                GridSize = 4,
                TargetSequence = new List<RuneInputDirection>
                {
                    RuneInputDirection.Left,
                    RuneInputDirection.Up,
                    RuneInputDirection.Right,
                    RuneInputDirection.Down,
                    RuneInputDirection.Right
                }
            };

            runeTypingClientConfigs[3] = new RuneTypingRoundConfig
            {
                ClientId = 3,
                GridSize = 4,
                TargetSequence = new List<RuneInputDirection>
                {
                    RuneInputDirection.Up,
                    RuneInputDirection.Up,
                    RuneInputDirection.Right,
                    RuneInputDirection.Down
                }
            };

            runeTypingClientConfigs[4] = new RuneTypingRoundConfig
            {
                ClientId = 4,
                GridSize = 5,
                TargetSequence = new List<RuneInputDirection>
                {
                    RuneInputDirection.Right,
                    RuneInputDirection.Down,
                    RuneInputDirection.Left,
                    RuneInputDirection.Up,
                    RuneInputDirection.Right
                }
            };
        }

        public DocumentReviewClientConfig GetDocumentReviewClientConfig(int clientId)
        {
            if (documentReviewClientConfigs.TryGetValue(clientId, out var config))
            {
                return config;
            }

            return documentReviewClientConfigs[1];
        }

        public StampClientConfig GetStampClientConfig(int clientId)
        {
            if (stampClientConfigs.TryGetValue(clientId, out var config))
            {
                return config;
            }

            return stampClientConfigs[1];
        }

        public StampConfig GetStampRuntimeConfig(int clientId)
        {
            var clientConfig = GetStampClientConfig(clientId);
            return new StampConfig
            {
                CorrectStampType = clientConfig.CorrectStampType,
                ChargeDuration = stampRuleConfig.ChargeDuration,
                PerfectWindowStart = stampRuleConfig.PerfectWindowStart,
                PerfectWindowEnd = stampRuleConfig.PerfectWindowEnd,
                NormalWindowPadding = stampRuleConfig.NormalWindowPadding,
                VerifyDebuffShakeAmp = stampRuleConfig.VerifyDebuffShakeAmp,
                VerifyDebuffShakeCount = stampRuleConfig.VerifyDebuffShakeCount,
                TimingFailPenalty = stampRuleConfig.TimingFailPenalty
            };
        }

        public SoulCollectClientConfig GetSoulCollectClientConfig(int clientId)
        {
            if (soulCollectClientConfigs.TryGetValue(clientId, out var config))
            {
                return config;
            }

            return soulCollectClientConfigs[1];
        }

        public SoulCollectConfig BuildSoulCollectRuntimeConfig(int clientId, int targetPercentOverride = -1)
        {
            var clientConfig = GetSoulCollectClientConfig(clientId);
            var config = soulCollectRuleConfig?.Clone() ?? new SoulCollectConfig();

            config.MinPercent = Mathf.Clamp(clientConfig.MinPercent, 0, 100);
            config.MaxPercent = Mathf.Clamp(clientConfig.MaxPercent, config.MinPercent, 100);

            var target = targetPercentOverride >= 0
                ? targetPercentOverride
                : clientConfig.DefaultTargetPercent;
            config.TargetPercent = Mathf.Clamp(target, config.MinPercent, config.MaxPercent);

            return config;
        }

        public BeanSellClientConfig GetBeanSellClientConfig(int clientId)
        {
            if (beanSellClientConfigs.TryGetValue(clientId, out var config))
            {
                return config;
            }

            return beanSellClientConfigs[1];
        }

        public BeanSellConfig BuildBeanSellRuntimeConfig()
        {
            return beanSellRuleConfig?.Clone() ?? new BeanSellConfig();
        }

        public SettlementClientConfig GetSettlementClientConfig(int clientId)
        {
            if (settlementClientConfigs.TryGetValue(clientId, out var config))
            {
                return config;
            }

            return settlementClientConfigs[1];
        }

        public SignMiniGameSettlementConfig BuildSettlementRuntimeConfig()
        {
            return settlementRuleConfig?.Clone() ?? new SignMiniGameSettlementConfig();
        }

        public RuneTypingRoundConfig BuildRuneTypingRoundConfig(int clientId, int gridSize)
        {
            var safeClientId = Mathf.Max(1, clientId);
            var safeGridSize = Mathf.Clamp(gridSize, 4, 5);
            var totalCells = safeGridSize * safeGridSize;

            runeTypingClientConfigs.TryGetValue(safeClientId, out var configuredRound);

            var targetSequence = new List<RuneInputDirection>();
            if (configuredRound != null && configuredRound.TargetSequence != null && configuredRound.TargetSequence.Count > 0)
            {
                targetSequence.AddRange(configuredRound.TargetSequence);
            }

            if (targetSequence.Count == 0)
            {
                var fallbackLength = Mathf.Clamp(
                    runeTypingRuleConfig.TargetSequenceLength,
                    4,
                    safeGridSize == 4 ? 5 : 6);
                targetSequence.AddRange(BuildGeneratedSequence(safeClientId, safeGridSize, fallbackLength));
            }

            List<RuneInputDirection> layout;
            if (configuredRound != null &&
                configuredRound.GridSize == safeGridSize &&
                configuredRound.RuneGridLayout != null &&
                configuredRound.RuneGridLayout.Count == totalCells)
            {
                layout = new List<RuneInputDirection>(configuredRound.RuneGridLayout);
            }
            else
            {
                layout = BuildGeneratedLayout(safeClientId, safeGridSize, totalCells);
            }

            for (var i = 0; i < targetSequence.Count; i++)
            {
                var targetCellIndex = (safeClientId * 3 + i * 5) % totalCells;
                layout[targetCellIndex] = targetSequence[i];
            }

            return new RuneTypingRoundConfig
            {
                ClientId = safeClientId,
                GridSize = safeGridSize,
                TargetSequence = targetSequence,
                RuneGridLayout = layout
            };
        }

        private static List<RuneInputDirection> BuildGeneratedSequence(int clientId, int gridSize, int length)
        {
            var result = new List<RuneInputDirection>(length);
            var baseSeed = clientId * 97 + gridSize * 53;

            for (var i = 0; i < length; i++)
            {
                var value = Mathf.Abs(baseSeed + i * 7 + i * i) % 4;
                result.Add((RuneInputDirection)value);
            }

            return result;
        }

        private static List<RuneInputDirection> BuildGeneratedLayout(int clientId, int gridSize, int totalCells)
        {
            var result = new List<RuneInputDirection>(totalCells);
            var baseSeed = clientId * 131 + gridSize * 17;

            for (var i = 0; i < totalCells; i++)
            {
                var value = Mathf.Abs(baseSeed + i * 5 + (i / gridSize) * 3) % 4;
                result.Add((RuneInputDirection)value);
            }

            return result;
        }
    }
}
