using System;
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

        public async Task<ProductDto> GetProductByBarcode(string barcode)
        {
            throw new NotImplementedException();
        }
    }
}