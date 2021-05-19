using System;

namespace OpenPOS.Infrastructure.Contexts
{
    public class ProductIncomeContext
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
        public string Notes { get; set; }
        public string PaymentMethod { get; set; }
        public Guid? FirmId { get; set; }
    }
}