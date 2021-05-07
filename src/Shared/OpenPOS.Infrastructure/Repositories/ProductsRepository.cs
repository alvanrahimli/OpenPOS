using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Infrastructure.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly PosContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsRepository> _logger;

        public ProductsRepository(PosContext context, IMapper mapper, ILogger<ProductsRepository> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        
        public async Task<ProductDto> GetProduct(Guid id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProduct(ProductDto input)
        {
            var product = _mapper.Map<Product>(input);
            await _context.Products.AddAsync(product);
            var dbRes = await _context.SaveChangesAsync();
            if (dbRes > 0)
            {
                return _mapper.Map<ProductDto>(product);
            }

            return null;
        }

        public async Task<ProductDto> GetProductByBarcode(Guid storeId, string barcode)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Barcode == barcode && p.StoreId == storeId);
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<string> GenerateBarcode(Guid storeId)
        {
            var barcode = Helper.GenerateRandomNumericString(12);
            var barcodeExists = await _context.Products
                .AsNoTracking()
                .Where(p => p.StoreId == storeId)
                .AnyAsync(p => p.Barcode == barcode);
            while (barcodeExists)
            {
                barcode = Helper.GenerateRandomNumericString(12);
                barcodeExists = await _context.Products
                    .AsNoTracking()
                    .Where(p => p.StoreId == storeId)
                    .AnyAsync(p => p.Barcode == barcode);
            }

            return barcode;
        }

        public async Task<ProductDto> UpdateProduct(ProductDto newProduct)
        {
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Barcode == newProduct.Barcode && p.StoreId == newProduct.StoreId);

            _mapper.Map(newProduct, existingProduct);
            var dbRes = await _context.SaveChangesAsync();
            if (dbRes > 0)
            {
                return _mapper.Map<ProductDto>(existingProduct);
            }

            return null;
        }
    }
}