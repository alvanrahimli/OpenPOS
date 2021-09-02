using System;
using System.Collections.Generic;

namespace OpenPOS.Domain.Models.LoyaltyModels
{
    public class Campaign
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DueDate { get; set; }
        public CampaignType Type { get; set; }

        // public int CampaignTypeId { get; set; }
        // public CampaignType CampaignType { get; set; }

        public Guid StoreId { get; set; }
        public Store Store { get; set; }
        
        public List<CampaignConfiguration> Configurations { get; set; }
    }
}
