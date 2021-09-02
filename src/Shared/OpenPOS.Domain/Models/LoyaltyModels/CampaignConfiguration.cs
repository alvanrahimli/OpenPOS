using System;

namespace OpenPOS.Domain.Models.LoyaltyModels
{
    public class CampaignConfiguration
    {
        public Guid Id { get; set; }
        public CampaignConfigurationType Type { get; set; }
        public string Data { get; set; }
        public string AdditionalData { get; set; }

        public int CampaignId { get; set; }
        public Campaign Campaign { get; set; }
    }
}