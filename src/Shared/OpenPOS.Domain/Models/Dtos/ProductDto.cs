using System;

namespace OpenPOS.Domain.Models.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Sku { get; set; }
        public string Barcode { get; set; }
        
        public int StockCount { get; set; }
        public int AlertStockCount { get; set; }

        public int UnitId { get; set; }
        public Unit Unit { get; set; }
        
        public decimal SalePrice { get; set; }
        public decimal SecondSalePrice { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal ProfitRate { get; set; }
        public decimal SecondProfitRate { get; set; }
        public decimal Vat { get; set; }

        public string Notes { get; set; }
        public string[] Tags { get; set; }
        
        public Guid CategoryId { get; set; }
        public Guid StoreId { get; set; }
        public Guid FirmId { get; set; }
    }
}