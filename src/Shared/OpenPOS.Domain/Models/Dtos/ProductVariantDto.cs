using System;

namespace OpenPOS.Domain.Models.Dtos
{
    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        
        public string UnitName { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalPrice => Quantity * SalePrice;
        
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
    }
}