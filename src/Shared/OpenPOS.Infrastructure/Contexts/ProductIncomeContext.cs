using System;
using System.ComponentModel.DataAnnotations;

namespace OpenPOS.Infrastructure.Contexts
{
    public class ProductIncomeContext
    {
        public Guid ProductId { get; set; }
        [Range(0, 10000)]
        public decimal Quantity { get; set; }
        public string Notes { get; set; }
        public string PaymentMethod { get; set; }
        public Guid? FirmId { get; set; }
    }
}