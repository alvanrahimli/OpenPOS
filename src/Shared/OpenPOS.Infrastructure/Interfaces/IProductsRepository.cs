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
        Task<ProductDto> GetProductByBarcode(string barcode);
    }
}