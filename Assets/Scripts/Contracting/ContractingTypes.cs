using System;
using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace ITC.Contracting
{
    public enum DocumentReviewDecision
    {
        Pass = 0,
        Reject = 1
    }

    public enum DocumentReviewRejectReason
    {
        None = 0,
        ImageMismatch = 1,
        DateMismatch = 2,
        ApplicationMismatch = 3,
        PaperForgery = 4,
        PaperDamage = 5
    }

    [Serializable]
    public sealed class DocumentReviewHotspotConfig
    {
        public string Id = string.Empty;
        public string DisplayName = string.Empty;
    }

    [Serializable]
    public sealed class DocumentReviewConfig
    {
        public int MinInspectCount = 2;
        public Vector2 ZoomRange = new(1f, 1.5f);
        public int HoverExpandPx = 10;
        public int SubmitDebounceMs = 300;
        public int WrongPenaltySatisfaction = 1;
        public bool WrongPenaltyMistakeFlag = true;
        public bool TutorialForceAllHotspots = true;
        public List<DocumentReviewHotspotConfig> Hotspots = new()
        {
            new DocumentReviewHotspotConfig { Id = "seal", DisplayName = "Seal" },
            new DocumentReviewHotspotConfig { Id = "ink", DisplayName = "Ink" },
            new DocumentReviewHotspotConfig { Id = "date", DisplayName = "Date" },
            new DocumentReviewHotspotConfig { Id = "content", DisplayName = "Content" }
        };
    }

    [Serializable]
    public sealed class DocumentReviewClientConfig
    {
        public int ClientId;
        public string ClientDisplayName = string.Empty;
        public string DateDisplayText = string.Empty;
        public string InkType = string.Empty;
        public bool SealIntegrity = true;
        public bool PhotoMatchesClient = true;
        public DocumentReviewDecision CorrectDecision = DocumentReviewDecision.Pass;
        public string[] AllowedRejectReasons =
        {
            "ImageMismatch",
            "DateMismatch",
            "ApplicationMismatch",
            "PaperForgery",
            "PaperDamage"
        };
    }

    public struct DocumentReviewResultPayload
    {
        public int ClientId;
        public DocumentReviewDecision FinalAction;
        public DocumentReviewRejectReason RejectReason;
        public int InspectedHotspotCount;
        public bool TutorialMode;
        public bool ForceConfirmed;
        public bool WasFallback;
    }

    public enum RuneVerifyResultType
    {
        Skipped = 0,
        Success = 1,
        Failed = 2
    }

    [Serializable]
    public sealed class RuneVerifyConfig
    {
        [Range(0f, 1f)] public float TriggerProbability = 0.3f;
        [Range(2, 8)] public int GridWidth = 4;
        [Range(2, 8)] public int GridHeight = 5;
        [Range(1, 10)] public int DistortedCount = 3;
        [Range(1f, 15f)] public float TimeLimitSeconds = 5f;
        [Range(0.05f, 1f)] public float FinalSecondPulseRate = 0.25f;
        public bool MisclickPenaltyVisualOnly = true;
        [Range(0f, 2f)] public float StampDebuffShakeAmp = 0.35f;
        public int FixedRandomSeed = -1;
        [Range(0f, 2f)] public float CountdownLeadSeconds = 0.45f;

        public RuneVerifyConfig Clone()
        {
            return new RuneVerifyConfig
            {
                TriggerProbability = TriggerProbability,
                GridWidth = GridWidth,
                GridHeight = GridHeight,
                DistortedCount = DistortedCount,
                TimeLimitSeconds = TimeLimitSeconds,
                FinalSecondPulseRate = FinalSecondPulseRate,
                MisclickPenaltyVisualOnly = MisclickPenaltyVisualOnly,
                StampDebuffShakeAmp = StampDebuffShakeAmp,
                FixedRandomSeed = FixedRandomSeed,
                CountdownLeadSeconds = CountdownLeadSeconds
            };
        }
    }

    public struct RuneVerifyResultPayload
    {
        public int ClientId;
        public RuneVerifyResultType Result;
        public int FoundCount;
        public int DistortedCount;
        public bool Triggered;
        public bool WasTimeoutFallback;
        public int RandomSeed;
    }

    [Serializable]
    public sealed class DocumentReviewPanelData : UIPanelData
    {
        public int ClientId;
        public bool TutorialMode;
        public DocumentReviewDecision ExpectedAction;
        public DocumentReviewConfig RuntimeConfig;
        public DocumentReviewClientConfig ClientConfig;
        public string[] AllowedRejectReasons;

        [NonSerialized] public Action<DocumentReviewResultPayload> OnCompleted;
        [NonSerialized] public Action<string, Transform, float> OnFxCue;
    }

    [Serializable]
    public sealed class RuneVerifyPanelData : UIPanelData
    {
        public int ClientId;
        public RuneVerifyConfig RuntimeConfig;
        public int RuntimeSeed = -1;

        [NonSerialized] public Action<RuneVerifyResultPayload> OnCompleted;
        [NonSerialized] public Action<string, Transform, float> OnFxCue;
    }

    public readonly struct ContractFxCueEvent
    {
        public readonly string CueId;
        public readonly Transform Anchor;
        public readonly float Intensity;

        public ContractFxCueEvent(string cueId, Transform anchor, float intensity)
        {
            CueId = cueId;
            Anchor = anchor;
            Intensity = intensity;
        }
    }
}

