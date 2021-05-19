using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Enums;
using OpenPOS.Domain.Extensions;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Contexts;
using OpenPOS.Infrastructure.Interfaces;
using OpenPOS.Infrastructure.Utils;

namespace OpenPOS.Infrastructure.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly PosContext _context;
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductsRepository> _logger;

        public ProductsRepository(PosContext context,
            ITransactionsRepository transactionsRepository,
            ILogger<ProductsRepository> logger,
            IMapper mapper)
        {
            _context = context;
            _transactionsRepository = transactionsRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PaginatedList<ProductDto>> GetProducts(Guid storeId, ProductFilterContext filterContext,
            int offset, int limit)
        {
            var productsQuery = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Firm)
                .Include(p => p.Unit)
                .Where(p => p.StoreId == storeId);
            
            // Search functionality. Do it first, so it won't be overrided by other filter
            if (!string.IsNullOrEmpty(filterContext.SearchBy) && !string.IsNullOrEmpty(filterContext.SearchTerm))
            {
                productsQuery = filterContext.SearchBy switch
                {
                    "Name" => productsQuery.Where(p => p.Name.Contains(filterContext.SearchTerm)),
                    "Barcode" => productsQuery.Where(p => p.Barcode == filterContext.SearchTerm),
                    _ => productsQuery
                };
            }

            // Price boundaries accoring given parameter
            if (filterContext.LimitBy != null)
            {
                productsQuery = filterContext.LimitBy switch
                {
                    "SalePrice" => productsQuery.Where(p =>
                        p.SalePrice > filterContext.FromPrice && p.SalePrice < filterContext.ToPrice),
                    "PurchasePrice" => productsQuery.Where(p =>
                        p.PurchasePrice > filterContext.FromPrice && p.PurchasePrice < filterContext.ToPrice),
                    "SecondSalePrice" => productsQuery.Where(p =>
                        p.SecondSalePrice > filterContext.FromPrice && p.SecondSalePrice < filterContext.ToPrice),
                    _ => productsQuery
                };
            }

            // Orders products by given parameter
            if (filterContext.OrderBy != null)
            {
                if (filterContext.IsDescending)
                {
                    productsQuery = filterContext.OrderBy switch
                    {
                        "Name" => productsQuery.OrderByDescending(p => p.Name),
                        "StockCount" => productsQuery.OrderByDescending(p => p.StockCount),
                        "PurchasePrice" => productsQuery.OrderByDescending(p => p.PurchasePrice),
                        "SalePrice" => productsQuery.OrderByDescending(p => p.SalePrice),
                        "SecondSalePrice" => productsQuery.OrderByDescending(p => p.SecondSalePrice),
                        _ => productsQuery
                    };
                }
                else
                {
                    productsQuery = filterContext.OrderBy switch
                    {
                        "Name" => productsQuery.OrderBy(p => p.Name),
                        "StockCount" => productsQuery.OrderBy(p => p.StockCount),
                        "PurchasePrice" => productsQuery.OrderBy(p => p.PurchasePrice),
                        "SalePrice" => productsQuery.OrderBy(p => p.SalePrice),
                        "SecondSalePrice" => productsQuery.OrderBy(p => p.SecondSalePrice),
                        _ => productsQuery
                    };
                }
            }
            else
            {
                // If no parameter is provided, order by name
                productsQuery = productsQuery.OrderBy(p => p.Name);
            }

            // Pagination
            productsQuery = productsQuery
                .Skip(offset)
                .Take(limit);

            var products = await productsQuery.ToListAsync();
            var mapped = _mapper.Map<List<ProductDto>>(products);

            var totalProductsInStore = await _context.Products.AsNoTracking().Where(p => p.StoreId == storeId).CountAsync();
            var totalPages = totalProductsInStore / limit;
            var currentPage = offset / limit + 1;

            var paginatedList = new PaginatedList<ProductDto>(mapped, totalPages, currentPage);
            return paginatedList;
        }

        public async Task<ProductDto> GetProduct(Guid id)
        {
            var product = await _context.Products
                .AsNoTracking()
                .Include(p => p.Unit)
                .Include(p => p.Category)
                .Include(p => p.Firm)
                .FirstOrDefaultAsync(p => p.Id == id);
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
                .Include(p => p.Unit)
                .Include(p => p.Category)
                .Include(p => p.Firm)
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

        public async Task<ProductDto> IncreaseProductCount(string userId, ProductIncomeContext incomeContext)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == incomeContext.ProductId);
            if (product == null)
            {
                return null;
            }

            product.StockCount += incomeContext.Quantity;

            var isParsed = Enum.TryParse(incomeContext.PaymentMethod, true, out PaymentMethod selectedMethod);
            if (!isParsed)
            {
                selectedMethod = PaymentMethod.Unknown;
            }

            var transactionContext = new CreateTransactionContext
            {
                Type = TransactionType.Income,
                Notes = incomeContext.Notes,
                PaymentMethod = selectedMethod,
                IncludedProductVariantContexts = new List<ProductVariantContext>()
                {
                    new()
                    {
                        ProductId = incomeContext.ProductId,
                        Quantity = incomeContext.Quantity
                    }
                },
                FirmId = incomeContext.FirmId
            };

            var transaction = await _transactionsRepository.CreateTransaction(userId, transactionContext);
            if (transaction != null)
            {
                _logger.LogInformation("Transaction '{TransactionId}' succeeded", transaction.Id);
            }
            else
            {
                _logger.LogWarning("Transaction failed. Could not increase stock count for product '{ProductId}'",
                    product.Id);
            }

            return _mapper.Map<ProductDto>(product);
        }
    }
}