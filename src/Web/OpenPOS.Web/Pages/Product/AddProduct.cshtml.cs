using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
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
        private readonly IFirmsRepository _firmsRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Selected Store's Id. Will be read form Cookie
        /// </summary>
        public Guid StoreId { get; set; }
        /// <summary>
        /// [BindProperty] New Product Model which will be created
        /// </summary>
        [BindProperty]
        public ProductDto NewProduct { get; set; }
        /// <summary>
        /// [BindProperty] Barcode search input property.
        /// </summary>
        [BindProperty]
        public string SearchBarcode { get; set; }

        /// <summary>
        /// Product returned after search by barcode
        /// </summary>
        public ProductDto SelectedProduct { get; set; }

        /// <summary>
        /// If product exists and is returned from search result
        /// </summary>
        [BindProperty]
        public bool IsExistingProduct { get; set; }
        /// <summary>
        /// NewProduct's barcode. Will be set after search or new barcode generation.
        /// </summary>
        public string SelectedBarcode { get; set; }
        /// <summary>
        /// Categories retrieved from db
        /// </summary>
        public List<Category> Categories { get; set; }
        /// <summary>
        /// Units retrieved from db
        /// </summary>
        public List<Unit> Units { get; set; }
        /// <summary>
        /// Available firms for store
        /// </summary>
        public List<Firm> Firms { get; set; }
        /// <summary>
        /// Message which will be passed to UI as alert
        /// </summary>
        public string Message { get; set; }

        public AddProduct(IProductsRepository productsRepository,
            ICategoriesRepository categoriesRepository,
            IUnitsRepository unitsRepository,
            IFirmsRepository firmsRepository,
            IMapper mapper)
        {
            _productsRepository = productsRepository;
            _categoriesRepository = categoriesRepository;
            _unitsRepository = unitsRepository;
            _firmsRepository = firmsRepository;
            _mapper = mapper;
        }
        
        public async Task OnGet(string barcode)
        {
            await InitializePage();
        }

        public async Task<ActionResult> OnPostGenerateBarcodeAsync()
        {
            var storeId = Request.GetStoreId();
            if (storeId == Guid.Empty)
            {
                Message = "Barkod daxil edin";
                await InitializePage();
                return Page();
            }

            var newBarcode = await _productsRepository.GenerateBarcode(storeId);
            SelectedBarcode = newBarcode;

            await InitializePage();
            return Page();
        }

        public async Task<ActionResult> OnPostSearchBarcodeAsync()
        {
            if (string.IsNullOrEmpty(SearchBarcode))
            {
                Message = "Barkod daxil edin";
                await InitializePage();
                return Page();
            }

            var searchResult = await _productsRepository.GetProductByBarcode(Request.GetStoreId(), SearchBarcode);
            if (searchResult == null)
            {
                SelectedBarcode = SearchBarcode;
                Message = $"Məhsul tapılmadı. '{SearchBarcode}' ilə yenisini yaradın.";
            }
            else
            {
                SelectedBarcode = searchResult.Barcode;
                SelectedProduct = searchResult;
                NewProduct = _mapper.Map<ProductDto>(searchResult);
                Message = "Məhsul tapıldı. Düzəlişlərinizi edin.";
                IsExistingProduct = true;
            }
            
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

            if (IsExistingProduct)
            {
                var response = await _productsRepository.UpdateProduct(NewProduct);
                Message = response == null ? "Səhv baş verdi. Yenidən cəhd edin." : "Məhsul yeniləndi!";
            }
            else
            {
                var response = await _productsRepository.CreateProduct(NewProduct);
                Message = response == null ? "Səhv baş verdi. Yenidən cəhd edin." : "Məhsul yaradıldı!";
            }
            
            await InitializePage();
            return Page();
        }

        private async Task InitializePage()
        {
            StoreId = Guid.Parse(Request.Cookies[".o.p.s"] ?? Guid.Empty.ToString());
            Categories = await _categoriesRepository.GetCategories(StoreId);
            Firms = await _firmsRepository.GetFirms(StoreId);
            Units = await _unitsRepository.GetUnits();
        }
    }
}