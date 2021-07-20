using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenPOS.Domain.Enums;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Contexts;

namespace OpenPOS.Infrastructure.Interfaces
{
    public interface ITransactionsRepository
    {
        Task<TransactionDto> CreateTransaction(string userId, CreateTransactionContext transactionContext);
        Task<List<TransactionDto>> GetTransactions(string userId, TransactionType? type = null);
        Task<TransactionDto> GetTransactionById(string userId, Guid id);
    }
}