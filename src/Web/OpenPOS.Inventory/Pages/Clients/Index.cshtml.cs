using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenPOS.Domain.Extensions;
using OpenPOS.Domain.Models;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Inventory.Pages.Clients
{
    public class ClientsIndex : PageModel
    {
        public List<Client> Clients { get; set; }
        
        private readonly IClientsRepository _clientsRepository;
        public ClientsIndex(IClientsRepository clientsRepository)
        {
            _clientsRepository = clientsRepository;
        }
        
        public async Task OnGet()
        {
            await InitializePage();
        }

        private async Task InitializePage()
        {
            Clients = await _clientsRepository.GetClientsForUser(User.GetUserId());
        }
    }
}