using System;
using System.ComponentModel.DataAnnotations;

namespace OpenPOS.Domain.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Sku { get; set; }
        [Required]
        public string Barcode { get; set; }
        
        [Range(0, int.MaxValue)]
        public int StockCount { get; set; }
        [Range(0, int.MaxValue)]
        public int AlertStockCount { get; set; }

        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        
        [Range(0, int.MaxValue)]
        public decimal SalePrice { get; set; }
        [Range(0, int.MaxValue)]
        public decimal SecondSalePrice { get; set; }
        [Range(0, int.MaxValue)]
        public decimal PurchasePrice { get; set; }
        [Range(0, 100)]
        public decimal ProfitRate { get; set; }
        [Range(0, 100)]
        public decimal Vat { get; set; }
        
        public string Notes { get; set; }
        public string[] Tags { get; set; }
        
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
        
        public Guid StoreId { get; set; }
        public Store Store { get; set; }

        public Guid? FirmId { get; set; }
        public Firm Firm { get; set; }
    }
}