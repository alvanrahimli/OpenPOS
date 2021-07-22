using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Enums;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Contexts;
using OpenPOS.Infrastructure.Interfaces;
using OpenPOS.Infrastructure.Utils;

namespace OpenPOS.Infrastructure.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly PosContext _context;
        private readonly IStoresRepository _storesRepository;
        private readonly UserManager<PosUser> _userManager;
        private readonly ILogger<TransactionsRepository> _logger;
        private readonly IMapper _mapper;

        public TransactionsRepository(PosContext context,
            IStoresRepository storesRepository,
            UserManager<PosUser> userManager,
            ILogger<TransactionsRepository> logger,
            IMapper mapper)
        {
            _context = context;
            _storesRepository = storesRepository;
            _userManager = userManager;
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
        
        public async Task<PaginatedList<TransactionDto>> GetTransactionsFilter(string userId, TransactionFilterContext filterContext,
            TransactionType? type = null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var transactionsQuery = _context.Transactions.AsNoTracking()
                .Include(t => t.Client)
                .Include(t => t.Firm)
                .Where(t => t.StoreId == user.SelectedStoreId);

            if (type != null)
            {
                transactionsQuery = transactionsQuery.Where(t => t.Type == type);
            }

            if (!string.IsNullOrEmpty(filterContext.PaymentMethod))
            {
                var parsed = Enum.TryParse(filterContext.PaymentMethod, true, out PaymentMethod method);
                if (parsed)
                {
                    transactionsQuery = transactionsQuery.Where(t => t.PaymentMethod == method);
                }
            }

            if (!string.IsNullOrEmpty(filterContext.SearchBy) && !string.IsNullOrEmpty(filterContext.SearchTerm))
            {
                var _ = Enum.TryParse(filterContext.SearchTerm, out PaymentMethod method);

                transactionsQuery = filterContext.SearchBy switch
                {
                    "Method" => transactionsQuery.Where(t => t.PaymentMethod == method),
                    "Client" => transactionsQuery.Where(t => t.Client.Name.Contains(filterContext.SearchTerm)),
                    "Firm" => transactionsQuery.Where(t => t.Firm.Name.Contains(filterContext.SearchTerm)),
                    "Notes" => transactionsQuery.Where(t => t.Notes.Contains(filterContext.SearchTerm)),
                    _ => transactionsQuery
                };
            }

            // Apply client filter
            if (filterContext.ClientId != null)
            {
                transactionsQuery = transactionsQuery.Where(t => t.ClientId == filterContext.ClientId);
            }
            
            // Apply firm filter
            if (filterContext.FirmId != null)
            {
                transactionsQuery = transactionsQuery.Where(t => t.FirmId == filterContext.FirmId);
            }

            // Order by parameter if is argument has passed
            if (!string.IsNullOrEmpty(filterContext.OrderBy))
            {
                if (filterContext.IsDescending)
                {
                    transactionsQuery = filterContext.OrderBy switch
                    {
                        "TotalPrice" => transactionsQuery.OrderByDescending(t => t.TotalPrice),
                        "Timestamp" => transactionsQuery.OrderByDescending(t => t.Timestamp),
                        _ => transactionsQuery.OrderByDescending(t => t.Timestamp)
                    };
                }
                else
                {
                    transactionsQuery = filterContext.OrderBy switch
                    {
                        "TotalPrice" => transactionsQuery.OrderBy(t => t.TotalPrice),
                        "Timestamp" => transactionsQuery.OrderBy(t => t.Timestamp),
                        _ => transactionsQuery.OrderBy(t => t.Timestamp)
                    };
                }
            }
            else
            {
                transactionsQuery = transactionsQuery.OrderByDescending(t => t.Timestamp);
            }

            if (!string.IsNullOrEmpty(filterContext.LimitBy) && (filterContext.FromPrice > 0 || filterContext.ToPrice > 0))
            {
                transactionsQuery = filterContext.LimitBy switch
                {
                    "TotalPrice" => transactionsQuery.Where(t => t.TotalPrice > filterContext.FromPrice
                                                                 && t.TotalPrice < filterContext.ToPrice),
                    _ => transactionsQuery
                };
            }

            transactionsQuery = transactionsQuery.Skip(filterContext.Offset).Take(filterContext.Limit);

            var transactions = await transactionsQuery.ToListAsync();
            var mapped = _mapper.Map<List<TransactionDto>>(transactions);

            var totalTransactionOfStore = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.StoreId == user.SelectedStoreId)
                .CountAsync();
            var totalPages = totalTransactionOfStore / filterContext.Limit;
            var currentPage = filterContext.Offset / filterContext.Limit + 1;

            var paginatedList = new PaginatedList<TransactionDto>(mapped, totalPages, currentPage);
            return paginatedList;
        }

        public async Task<TransactionDto> GetTransactionById(string userId, Guid id)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var transaction = await _context.Transactions.AsNoTracking()
                .Include(t => t.Client)
                .Include(t => t.Firm)
                .Include(t => t.IncludedProducts)
                .FirstOrDefaultAsync(t => t.Id == id && t.StoreId == user.SelectedStoreId);
            return _mapper.Map<TransactionDto>(transaction);
        }
    }
}