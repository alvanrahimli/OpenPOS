using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace OpenPOS.Domain.Models
{
    public class ProductVariant
    {
        public Guid Id { get; set; }

        public string UnitName { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalePrice { get; set; }
        [NotMapped]
        public decimal TotalPrice => Quantity * SalePrice;
        
        public Guid? ProductId { get; set; }
        public string ProductName { get; set; }
        public Product Product { get; set; }

        public Guid TransactionId { get; set; }
        public Transaction Transaction { get; set; }
    }
}