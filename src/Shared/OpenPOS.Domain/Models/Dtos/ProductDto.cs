using System;
using System.ComponentModel.DataAnnotations;

namespace OpenPOS.Domain.Models.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(120, MinimumLength = 2)]
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Sku { get; set; }
        [Required]
        public string Barcode { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModifiedDate { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockCount { get; set; }
        [Range(0, int.MaxValue)]
        public int AlertStockCount { get; set; }

        [Required]
        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        
        [Required]
        public decimal SalePrice { get; set; }
        public decimal SecondSalePrice { get; set; }
        [Required]
        public decimal PurchasePrice { get; set; }
        public decimal ProfitRate { get; set; }
        public decimal SecondProfitRate { get; set; }
        public decimal Vat { get; set; }

        [StringLength(255)]
        public string Notes { get; set; }
        public string[] Tags { get; set; }
        
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid StoreId { get; set; }
        public Guid? FirmId { get; set; }
        public string FirmName { get; set; }
    }
}