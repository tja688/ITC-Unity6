using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace ITC.Contracting
{
    public sealed class ContractClientConfigModel : AbstractModel
    {
        private readonly Dictionary<int, DocumentReviewClientConfig> documentReviewClientConfigs = new();
        private readonly Dictionary<int, StampClientConfig> stampClientConfigs = new();
        private readonly Dictionary<int, RuneTypingRoundConfig> runeTypingClientConfigs = new();
        private DocumentReviewConfig documentReviewRuleConfig = new();
        private StampConfig stampRuleConfig = new();
        private RuneTypingConfig runeTypingRuleConfig = new();
        private RuneVerifyConfig runeVerifyRuleConfig = new();

        public DocumentReviewConfig DocumentReviewRuleConfig => documentReviewRuleConfig;
        public StampConfig StampRuleConfig => stampRuleConfig;
        public RuneTypingConfig RuneTypingRuleConfig => runeTypingRuleConfig;
        public RuneVerifyConfig RuneVerifyRuleConfig => runeVerifyRuleConfig;

        protected override void OnInit()
        {
            documentReviewRuleConfig = new DocumentReviewConfig();
            stampRuleConfig = new StampConfig();
            runeTypingRuleConfig = new RuneTypingConfig();
            runeVerifyRuleConfig = new RuneVerifyConfig();
            documentReviewClientConfigs.Clear();
            stampClientConfigs.Clear();
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
