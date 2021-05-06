using System;

namespace OpenPOS.Domain.Models
{
    public class ProductVariant
    {
        public Guid Id { get; set; }

        public string ProductName { get; set; }
        public string UnitName { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalePrice { get; set; }
        
        public Guid? ProductId { get; set; }
        public Product Product { get; set; }
    }
}