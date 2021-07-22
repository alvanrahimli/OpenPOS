using System;
using OpenPOS.Domain.Enums;

namespace OpenPOS.Infrastructure.Contexts
{
    public class TransactionFilterContext : FilterContext
    {
        public Guid? FirmId { get; set; }

        public Guid? ClientId { get; set; }

        public string PaymentMethod { get; set; }
    }
}