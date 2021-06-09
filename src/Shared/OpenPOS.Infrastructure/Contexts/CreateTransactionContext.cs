using System;
using System.Collections.Generic;
using OpenPOS.Domain.Enums;

namespace OpenPOS.Infrastructure.Contexts
{
    public class CreateTransactionContext
    {
        public PaymentMethod PaymentMethod { get; set; }
        public TransactionType Type { get; set; }
        public string Notes { get; set; }

        public decimal PayedAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReturnAmount { get; set; }

        public string ClientName { get; set; }
        public Guid? FirmId { get; set; }

        public List<ProductVariantContext> IncludedProducts { get; set; }
    }
}