using System;

namespace OpenPOS.Infrastructure.Contexts
{
    public class ProductVariantContext
    {
        public Guid ProductId { get; set; }
        public decimal Quantity { get; set; }
    }
}