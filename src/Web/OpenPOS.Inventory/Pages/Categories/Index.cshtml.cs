using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenPOS.Domain.Extensions;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Inventory.Pages.Categories
{
    public class CategoriesIndex : PageModel
    {
        public List<CategoryDto> Categories { get; set; }
        [BindProperty]
        public AddCategoryDto NewCategory { get; set; }

        public string Message { get; set; }
        
        private readonly ICategoriesRepository _categoriesRepository;
        
        public CategoriesIndex(ICategoriesRepository categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }
        
        public async Task OnGetAsync()
        {
            await InitializePage();
        }

        public async Task<ActionResult> OnPostCreateAsync()
        {
            var addedCategory = await _categoriesRepository.AddCategory(User.GetUserId(), NewCategory);
            if (addedCategory == null)
            {
                Message = "Səhv baş verdi";
            }

            await InitializePage();
            return Page();
        }

        private async Task InitializePage()
        {
            Categories = await _categoriesRepository.GetCategories(User.GetUserId());
        }
    }
}