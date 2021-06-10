using System;
using System.Collections.Generic;
using System.Linq;
using OpenPOS.Domain.Enums;

namespace OpenPOS.Infrastructure.Contexts
{
    public class CreateTransactionContext
    {
        public PaymentMethod PaymentMethod { get; set; }
        public TransactionType Type { get; set; }
        public string Notes { get; set; }

        public decimal PayedAmount { get; set; }
        public decimal TotalAmount => IncludedProducts.Sum(p => p.TotalPrice);
        public decimal ReturnAmount => PayedAmount - TotalAmount;

        public string ClientName { get; set; }
        public string ClientNotes { get; set; }
        public Guid? FirmId { get; set; }

        public List<ProductVariantContext> IncludedProducts { get; set; }
    }
}