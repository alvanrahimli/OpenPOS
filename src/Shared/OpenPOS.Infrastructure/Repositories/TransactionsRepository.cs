using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Enums;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Contexts;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Infrastructure.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly PosContext _context;
        private readonly IStoresRepository _storesRepository;
        private readonly ILogger<TransactionsRepository> _logger;
        private readonly IMapper _mapper;

        public TransactionsRepository(PosContext context,
            IStoresRepository storesRepository,
            ILogger<TransactionsRepository> logger,
            IMapper mapper)
        {
            _context = context;
            _storesRepository = storesRepository;
            _logger = logger;
            _mapper = mapper;
        }
        
        public async Task<TransactionDto> CreateTransaction(string userId,  CreateTransactionContext transactionContext)
        {
            var transaction = _mapper.Map<Transaction>(transactionContext);
            transaction.IncludedProducts = new List<ProductVariant>();
            var usersStore = await _storesRepository.GetSelectedStore(userId);
            transaction.StoreId = usersStore.Id;
            
            // Load sold products and create product variants
            var unfoundProducts = new List<Guid>();
            foreach (var soldProduct in transactionContext.IncludedProducts)
            {
                var product = await _context.Products
                    .Include(p => p.Unit)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == soldProduct.ProductId);
                if (product == null)
                {
                    unfoundProducts.Add(soldProduct.ProductId);
                    continue;
                }

                // Change product stock count according transaction type
                switch (transaction.Type)
                {
                    case TransactionType.Sale:
                        product.StockCount -= soldProduct.Quantity;
                        break;
                    case TransactionType.Return:
                        product.StockCount += soldProduct.Quantity;
                        break;
                    case TransactionType.Income:
                        product.StockCount += soldProduct.Quantity;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(transaction.Type));
                }
                product.LastModifiedDate = DateTime.UtcNow;
                
                var productVariant = new ProductVariant
                {
                    Quantity = soldProduct.Quantity,
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitName = product.Unit.DisplayName,
                    SalePrice = product.SalePrice
                };
                transaction.IncludedProducts.Add(productVariant);
            }
            transaction.TotalPrice = transaction.IncludedProducts.Sum(p => p.TotalPrice);

            if (unfoundProducts.Count > 0)
            {
                _logger.LogWarning("Some products was not found to sell. Ids: '{Ids}'", 
                    string.Join(',', unfoundProducts));
            }

            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
            return _mapper.Map<TransactionDto>(transaction);
        }
    }
}