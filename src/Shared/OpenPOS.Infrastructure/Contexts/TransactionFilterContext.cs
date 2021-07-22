using System;

namespace OpenPOS.Infrastructure.Contexts
{
    public class TransactionFilterContext : FilterContext
    {
        // public bool FilterFirm { get; set; }
        public Guid? FirmId { get; set; }

        // public bool FilterClients { get; set; }
        public Guid? ClientId { get; set; }
    }
}