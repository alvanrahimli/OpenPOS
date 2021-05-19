using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenPOS.Domain.Extensions;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Inventory.Pages
{
    public class Index : PageModel
    {
        private readonly IStoresRepository _storesRepository;
        public List<StoreDto> Stores { get; set; }
        
        [BindProperty]
        public Guid SelectedStore { get; set; }

        public string ErrorMessage { get; set; }

        public Index(IStoresRepository storesRepository)
        {
            _storesRepository = storesRepository;
        }
        
        public async Task OnGet()
        {
            await InitializePage();
        }

        public async Task<ActionResult> OnPostSelectStoreAsync()
        {
            var store = await _storesRepository.SelectStore(User.GetUserId(), SelectedStore);
            SelectedStore = store.Id;
            await InitializePage();
            return Page();
        }

        private async Task InitializePage()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            Stores = await _storesRepository.GetStoresForUser(userId);
        }
    }
}