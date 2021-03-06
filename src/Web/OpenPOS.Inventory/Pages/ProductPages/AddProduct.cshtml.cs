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
        public List<CategoryDto> Categories { get; set; }
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
                    Message = $"M??hsul tap??lmad??. '{id}' il?? yenisini yarad??n.";
                }
                else
                {
                    SelectedBarcode = product.Barcode;
                    SelectedProduct = product;
                    NewProduct = _mapper.Map<ProductDto>(product);
                    Message = "D??z??li??l??rinizi edin.";
                    IsExistingProduct = true;
                }
            }
            else
            {
                // Always generate new barcode for new product page.
                var storeId = (await _storesRepository.GetSelectedStore(User.GetUserId())).Id;
                var newBarcode = await _productsRepository.GenerateBarcode(storeId);
                SelectedBarcode = newBarcode;
                Message = "Yeni barkod yarad??ld??";
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
                Message = $"M??hsul tap??lmad??. '{SearchBarcode}' il?? yenisini yarad??n.";
            }
            else
            {
                SelectedBarcode = searchResult.Barcode;
                SelectedProduct = searchResult;
                NewProduct = _mapper.Map<ProductDto>(searchResult);
                Message = "M??hsul tap??ld??. D??z??li??l??rinizi edin.";
                IsExistingProduct = true;
            }
            
            await InitializePage();
            return Page();
        }

        public async Task<ActionResult> OnPostCreateProductAsync()
        {
            if (string.IsNullOrEmpty(NewProduct.Barcode))
            {
                Message = "Barkod daxil edin";
                await InitializePage();
                return Page();
            }

            var storeId = (await _storesRepository.GetSelectedStore(User.GetUserId())).Id;
            NewProduct.StoreId = storeId;
            if (IsExistingProduct)
            {
                var response = await _productsRepository.UpdateProduct(NewProduct);
                Message = response == null ? "S??hv ba?? verdi. Yenid??n c??hd edin." : "M??hsul yenil??ndi!";
            }
            else
            {
                var response = await _productsRepository.CreateProduct(NewProduct);
                Message = response == null ? "S??hv ba?? verdi. Yenid??n c??hd edin." : "M??hsul yarad??ld??!";
            }
            
            await InitializePage();
            return Page();
        }

        private async Task InitializePage()
        {
            var store = await _storesRepository.GetSelectedStore(User.GetUserId());
            StoreId = store.Id;
            Categories = await _categoriesRepository.GetCategories(User.GetUserId());
            Firms = await _firmsRepository.GetFirms(StoreId);
            Units = await _unitsRepository.GetUnits();
        }
    }
}