using System.Threading.Tasks;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Contexts;

namespace OpenPOS.Infrastructure.Interfaces
{
    public interface ITransactionsRepository
    {
        Task<TransactionDto> CreateTransaction(string userId, CreateTransactionContext transactionContext);
    }
}