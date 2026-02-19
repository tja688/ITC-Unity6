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

