using System;
using System.ComponentModel.DataAnnotations;

namespace OpenPOS.Infrastructure.Contexts
{
    public class ProductVariantContext
    {
        public Guid ProductId { get; set; }
        [Range(1, 10000)]
        public decimal Quantity { get; set; }

        public string ProductName { get; set; }
        public string UnitName { get; set; }
        [Range(0, int.MaxValue)]
        public decimal Price { get; set; }

        public decimal TotalPrice => Price * Quantity;
    }
}