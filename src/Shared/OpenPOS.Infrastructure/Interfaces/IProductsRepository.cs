using System;
using System.Threading.Tasks;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;

namespace OpenPOS.Infrastructure.Interfaces
{
    public interface IProductsRepository
    {
        Task<ProductDto> GetProduct(Guid id);
        Task<ProductDto> CreateProduct(ProductDto input);
        Task<ProductDto> GetProductByBarcode(Guid storeId, string barcode);
        Task<string> GenerateBarcode(Guid storeId);
        Task<ProductDto> UpdateProduct(ProductDto newProduct);
    }
}