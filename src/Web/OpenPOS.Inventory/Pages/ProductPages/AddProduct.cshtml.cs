using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenPOS.Domain.Extensions;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Inventory.Pages.ProductPages
{
    public class AddProduct : PageModel
    {
        private readonly IProductsRepository _productsRepository;
        private readonly ICategoriesRepository _categoriesRepository;
        private readonly IUnitsRepository _unitsRepository;
        private readonly IFirmsRepository _firmsRepository;
        private readonly IStoresRepository _storesRepository;
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
            IStoresRepository storesRepository,
            IMapper mapper)
        {
            _productsRepository = productsRepository;
            _categoriesRepository = categoriesRepository;
            _unitsRepository = unitsRepository;
            _firmsRepository = firmsRepository;
            _storesRepository = storesRepository;
            _mapper = mapper;
        }
        
        public async Task OnGet(Guid? id)
        {
            if (id != null)
            {
                var product = await _productsRepository.GetProduct((Guid) id);
                if (product == null)
                {
                    Message = $"Məhsul tapılmadı. '{id}' ilə yenisini yaradın.";
                }
                else
                {
                    SelectedBarcode = product.Barcode;
                    SelectedProduct = product;
                    NewProduct = _mapper.Map<ProductDto>(product);
                    Message = "Düzəlişlərinizi edin.";
                    IsExistingProduct = true;
                }
            }
            await InitializePage();
        }

        public async Task<ActionResult> OnPostGenerateBarcodeAsync()
        {
            var storeId = (await _storesRepository.GetSelectedStore(User.GetUserId())).Id;

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

            var storeId = (await _storesRepository.GetSelectedStore(User.GetUserId())).Id;
            var searchResult = await _productsRepository.GetProductByBarcode(storeId, SearchBarcode);
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
            var storeId = (await _storesRepository.GetSelectedStore(User.GetUserId())).Id;
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
            var store = await _storesRepository.GetSelectedStore(User.GetUserId());
            StoreId = store.Id;
            Categories = await _categoriesRepository.GetCategories(StoreId);
            Firms = await _firmsRepository.GetFirms(StoreId);
            Units = await _unitsRepository.GetUnits();
        }
    }
}