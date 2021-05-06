using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OpenPOS.Web.Pages.Sale
{
    public class SalePageModel : PageModel
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public Guid SelectedStoreId { get; set; }

        public SalePageModel(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        
        public async Task OnGet()
        {
            await InitializePage();
        }

        private async Task InitializePage()
        {
            SelectedStoreId = GetStoreId();
        }

        private Guid GetStoreId()
        {
            var storeId = _contextAccessor.HttpContext?.Request.Cookies[".o.p.s"];
            if (string.IsNullOrEmpty(storeId))
            {
                return Guid.Empty;
            }

            return Guid.Parse(storeId);
        }
    }
}