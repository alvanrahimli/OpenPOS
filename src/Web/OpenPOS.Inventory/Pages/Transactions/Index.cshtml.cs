using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenPOS.Domain.Enums;
using OpenPOS.Domain.Extensions;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Inventory.Pages.Transactions
{
    public class TransactionsIndex : PageModel
    {
        private readonly ITransactionsRepository _transactionsRepository;
        public List<TransactionDto> Transactions { get; set; }

        public TransactionsIndex(ITransactionsRepository transactionsRepository)
        {
            _transactionsRepository = transactionsRepository;
        }
        
        public async Task OnGetAsync()
        {
            await InitializePage();
        }

        private async Task InitializePage()
        {
            Transactions = await _transactionsRepository.GetTransactions(User.GetUserId(), TransactionType.Sale);
        }
    }
}