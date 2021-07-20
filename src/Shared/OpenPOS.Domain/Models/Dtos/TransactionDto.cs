using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using OpenPOS.Domain.Enums;

namespace OpenPOS.Domain.Models.Dtos
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Notes { get; set; }
        public decimal TotalPrice { get; set; }
        public TransactionType Type { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public Guid? ClientId { get; set; }
        public Client Client { get; set; }

        public Guid? FirmId { get; set; }
        public Firm Firm { get; set; }

        public List<ProductVariantDto> IncludedProducts { get; set; }
        
        [JsonIgnore] 
        public string ClientName => Client?.Name ?? "Yoxdur";
    }
}