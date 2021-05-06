using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Interfaces;
using OpenPOS.Web.Extensions;

namespace OpenPOS.Web.Pages.Product
{
    public class AddProduct : PageModel
    {
        private readonly IProductsRepository _productsRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IUnitsRepository _unitsRepository;

        public Guid StoreId { get; set; }
        
        [BindProperty]
        public ProductDto NewProduct { get; set; }
        [BindProperty]
        public string SearchBarcode { get; set; }

        public ProductDto SelectedProduct { get; set; }
        public List<Category> Categories { get; set; }
        public List<Unit> Units { get; set; }
        public string Message { get; set; }

        public AddProduct(IProductsRepository productsRepository,
            ICategoriesRepository categoriesRepository,
            IUnitsRepository unitsRepository)
        {
            _productsRepository = productsRepository;
            _categoriesRepository = categoriesRepository;
            _unitsRepository = unitsRepository;
        }
        
        public async Task OnGet(string barcode)
        {
            await InitializePage();
        }

        public async Task<ActionResult> OnPostSearchBarcodeAsync()
        {
            if (string.IsNullOrEmpty(SearchBarcode))
            {
                Message = "Barkod daxil edin";
                await InitializePage();
                return Page();
            }

            SelectedProduct = await _productsRepository.GetProductByBarcode(SearchBarcode);
            await InitializePage();
            return Page();
        }

        public async Task<ActionResult> OnPostCreateProductAsync()
        {
            var storeId = Request.GetStoreId();
            if (storeId == Guid.Empty)
            {
                return LocalRedirect("/");
            }

            NewProduct.StoreId = storeId;
            var response = _productsRepository.CreateProduct(NewProduct);
            if (response == null)
            {
                return Page();
            }

            Message = "Məhsul yaradıldı";
            await InitializePage();
            return Page();
        }

        private async Task InitializePage()
        {
            StoreId = Guid.Parse(Request.Cookies[".o.p.s"] ?? Guid.Empty.ToString());
            Categories = await _categoriesRepository.GetCategories(StoreId);
            Units = await _unitsRepository.GetUnits();
        }
    }
}