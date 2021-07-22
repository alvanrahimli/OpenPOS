using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenPOS.Domain.Extensions;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Inventory.Pages.Transactions
{
    public class TransactionDetails : PageModel
    {
        private readonly ITransactionsRepository _transactionsRepository;
        public TransactionDto Transaction { get; set; }
        
        // TODO: Place a button to revert transaction

        public TransactionDetails(ITransactionsRepository transactionsRepository)
        {
            _transactionsRepository = transactionsRepository;
        }
        
        public async Task OnGet(Guid id)
        {
            await InitializePage(id);
        }

        private async Task InitializePage(Guid id)
        {
            Transaction = await _transactionsRepository.GetTransactionById(User.GetUserId(), id);
        }
    }
}