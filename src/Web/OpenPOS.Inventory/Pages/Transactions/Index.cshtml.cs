using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenPOS.Domain.Enums;
using OpenPOS.Domain.Extensions;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Contexts;
using OpenPOS.Infrastructure.Interfaces;
using OpenPOS.Infrastructure.Utils;

namespace OpenPOS.Inventory.Pages.Transactions
{
    public class TransactionsIndex : PageModel
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IClientsRepository _clientsRepository;
        private readonly IFirmsRepository _firmsRepository;

        public TransactionsIndex(ITransactionsRepository transactionsRepository,
            IClientsRepository clientsRepository,
            IFirmsRepository firmsRepository)
        {
            _transactionsRepository = transactionsRepository;
            _clientsRepository = clientsRepository;
            _firmsRepository = firmsRepository;
        }
        
        [BindProperty(SupportsGet = true)]
        public TransactionFilterContext FilterContext { get; set; }
        
        public PaginatedList<TransactionDto> Transactions { get; set; }
        public List<Client> Clients { get; set; }
        public List<Firm> Firms { get; set; }

        public async Task OnGetAsync(int pageNum = 1)
        {
            await InitializePage(pageNum);
        }

        private async Task InitializePage(int pageNum)
        {
            FilterContext.Limit = FilterContext.Limit == 0 ? 50 : FilterContext.Limit;
            FilterContext.Offset = (pageNum - 1) * FilterContext.Limit;

            Transactions = await _transactionsRepository.GetTransactionsFilter(User.GetUserId(), FilterContext, 
                TransactionType.Sale);
            Clients = await _clientsRepository.GetClientsForUser(User.GetUserId());
            Firms = await _firmsRepository.GetFirmsForUser(User.GetUserId());
        }
    }
}