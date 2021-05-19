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

        public Guid? ClientId { get; set; }
        public Guid? FirmId { get; set; }

        public List<ProductVariantContext> IncludedProductVariantContexts { get; set; }
    }
}