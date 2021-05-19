using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using OpenPOS.Domain.Extensions;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Contexts;
using OpenPOS.Infrastructure.Interfaces;
using OpenPOS.Infrastructure.Utils;

namespace OpenPOS.Inventory.Pages.ProductPages
{
    public class ProductsPageModel : PageModel
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IStoresRepository _storesRepository;
        private readonly IFirmsRepository _firmsRepository;
        private readonly IUnitsRepository _unitsRepository;
        private readonly IConfiguration _configuration;

        public ProductsPageModel(IProductsRepository productsRepository,
            IStoresRepository storesRepository,
            IFirmsRepository firmsRepository,
            IUnitsRepository unitsRepository,
            IConfiguration configuration)
        {
            _productsRepository = productsRepository;
            _storesRepository = storesRepository;
            _firmsRepository = firmsRepository;
            _unitsRepository = unitsRepository;
            _configuration = configuration;
        }

        [BindProperty(SupportsGet = true)]
        public ProductFilterContext FilterContext { get; set; }
        [BindProperty]
        public ProductIncomeContext IncomeContext { get; set; }

        public PaginatedList<ProductDto> Products { get; set; }
        public List<StoreDto> Stores { get; set; }
        public List<Firm> Firms { get; set; }
        public List<Unit> Units { get; set; }
        public Guid SelectedStore { get; set; }
        public string Message { get; set; }
        
        public async Task<ActionResult> OnGet(int pageNum = 1)
        {
            await InitializePage(pageNum);
            return Page();
        }

        public ActionResult OnGetClearFilterAsync()
        {
            return LocalRedirect("/products/");
        }

        public async Task<ActionResult> OnPostProductIncomeAsync()
        {
            var result = await _productsRepository.IncreaseProductCount(User.GetUserId(), IncomeContext);
            Message = result != null ? $"Məhsul ({result.Name}) sayı artırıldı" : "Səhv baş verdi";
            await InitializePage();
            return Page();
        }

        private async Task InitializePage(int pageNum = 1)
        {
            var store = await _storesRepository.GetSelectedStore(User.GetUserId());
            if (store == null)
            {
                return;
            }

            FilterContext ??= new ProductFilterContext
            {
                LimitBy = "SalePrice",
                FromPrice = 0,
                ToPrice = decimal.MaxValue,
                OrderBy = "Name",
                Offset = 0,
                Limit = 50
            };
            FilterContext.Limit = FilterContext.Limit == 0 ? 50 : FilterContext.Limit;
            FilterContext.Offset = (pageNum - 1) * FilterContext.Limit;
            
            Products = await _productsRepository.GetProducts(store.Id, FilterContext,
                FilterContext.Offset, FilterContext.Limit);
            
            // var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userId = User.GetUserId();
            Stores = await _storesRepository.GetStoresForUser(userId);
            Firms = await _firmsRepository.GetFirms(store.Id);
            Units = await _unitsRepository.GetUnits();
        }
    }
}