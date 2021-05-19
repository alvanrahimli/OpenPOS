using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Contexts;
using OpenPOS.Infrastructure.Utils;

namespace OpenPOS.Infrastructure.Interfaces
{
    public interface IProductsRepository
    {
        Task<PaginatedList<ProductDto>> GetProducts(Guid storeId, ProductFilterContext filterContext,
            int offset, int limit);
        Task<ProductDto> GetProduct(Guid id);
        Task<ProductDto> CreateProduct(ProductDto input);
        Task<ProductDto> GetProductByBarcode(Guid storeId, string barcode);
        Task<string> GenerateBarcode(Guid storeId);
        Task<ProductDto> UpdateProduct(ProductDto newProduct);
        Task<ProductDto> IncreaseProductCount(string userId, ProductIncomeContext incomeContext);
    }
}