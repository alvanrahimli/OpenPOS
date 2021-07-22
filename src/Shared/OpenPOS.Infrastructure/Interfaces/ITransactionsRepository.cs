using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenPOS.Domain.Enums;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Contexts;
using OpenPOS.Infrastructure.Utils;

namespace OpenPOS.Infrastructure.Interfaces
{
    public interface ITransactionsRepository
    {
        Task<TransactionDto> CreateTransaction(string userId, CreateTransactionContext transactionContext);
        Task<PaginatedList<TransactionDto>> GetTransactionsFilter(string userId, TransactionFilterContext filterContext,
            TransactionType? type = null);
        Task<TransactionDto> GetTransactionById(string userId, Guid id);
    }
}