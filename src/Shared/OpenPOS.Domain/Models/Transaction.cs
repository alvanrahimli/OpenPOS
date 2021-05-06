using System;
using OpenPOS.Domain.Enums;

namespace OpenPOS.Domain.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string[] Tags { get; set; }
        public string Notes { get; set; }
        public decimal TotalPrice { get; set; }
        public TransactionType Type { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        public Guid? ClientId { get; set; }
        public Client Client { get; set; }

        public Guid? FirmId { get; set; }
        public Firm Firm { get; set; }
    }
}