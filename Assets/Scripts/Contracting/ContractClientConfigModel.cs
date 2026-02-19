using System.Collections.Generic;
using QFramework;

namespace ITC.Contracting
{
    public sealed class ContractClientConfigModel : AbstractModel
    {
        private readonly Dictionary<int, DocumentReviewClientConfig> documentReviewClientConfigs = new();
        private DocumentReviewConfig documentReviewRuleConfig = new();

        public DocumentReviewConfig DocumentReviewRuleConfig => documentReviewRuleConfig;

        protected override void OnInit()
        {
            documentReviewRuleConfig = new DocumentReviewConfig();
            documentReviewClientConfigs.Clear();

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
        }

        public DocumentReviewClientConfig GetDocumentReviewClientConfig(int clientId)
        {
            if (documentReviewClientConfigs.TryGetValue(clientId, out var config))
            {
                return config;
            }

            return documentReviewClientConfigs[1];
        }
    }
}

