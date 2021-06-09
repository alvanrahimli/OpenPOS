using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Enums;
using OpenPOS.Domain.Extensions;
using OpenPOS.Domain.Models;
using OpenPOS.Infrastructure.Contexts;
using OpenPOS.Infrastructure.Interfaces;
using OpenPOS.Sale.Extensions;

namespace OpenPOS.Sale.Pages
{
    public partial class Sale
    {
        private string _barcode;
        private string _errorMessage;
        private decimal CartTotalPrice => NewTransaction.IncludedProducts.Sum(p => p.TotalPrice);
        private decimal _incomeMoney;
        public CreateTransactionContext NewTransaction { get; set; }
        private List<string> _customers;
        
        [Inject] public IDbContextFactory<PosContext> DbContextFactory { get; set; }
        [Inject] public IStoresRepository StoresRepository { get; set; }
        [Inject] public IClientsRepository ClientsRepository { get; set; }
        [Inject] public IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject] public ILogger<Sale> Logger { get; set; }
        [Inject] public IDistributedCache Cache { get; set; }

        public bool IsBusy { get; set; }

        protected override async Task OnInitializedAsync()
        {
            NewTransaction = new CreateTransactionContext
            {
                IncludedProducts = new List<ProductVariantContext>()
            };

            await using var context = DbContextFactory.CreateDbContext();
            var userId = HttpContextAccessor.HttpContext?.User.GetUserId();
            var storeId = await Cache.GetSelectedStore(context, userId);
            
            // Load clients of store
            var clients = await context.Clients.Where(c => c.StoreId == storeId).ToListAsync();
            _customers = clients.Select(c => c.Name).ToList();
        }

        private async Task OnBarcodeInputAsync(ChangeEventArgs args)
        {
            if (IsBusy)
            {
                _errorMessage = "Yenidən cəhd edin";
                return;
            }
            
            _barcode = args.Value?.ToString();
            
            if (_barcode?.Length >= 12)
            {
                Product selectedProduct = null;
                try
                {
                    IsBusy = true;
                    
                    await using var context = DbContextFactory.CreateDbContext();
                    var userId = HttpContextAccessor.HttpContext?.User.GetUserId();
                    var storeId = await Cache.GetSelectedStore(context, userId);
                    selectedProduct = await context.Products
                        .Include(p => p.Unit)
                        .FirstOrDefaultAsync(p => p.Barcode == _barcode && p.StoreId == storeId);
                }
                catch (Exception e)
                {
                    Logger.LogError("Error occured: {Error}", e.Message);
                    Logger.LogWarning("Concurrent request");
                }
                finally
                {
                    IsBusy = false;
                }

                if (selectedProduct == null)
                {
                    _errorMessage = $"Məhsul '{_barcode}' tapılmadı.";
                    return;
                }

                if (selectedProduct.StockCount == 0)
                {
                    _errorMessage = $"{selectedProduct.Name} stokda yoxdur";
                }

                var existingProductInCart = NewTransaction.IncludedProducts
                    .FirstOrDefault(p => p.ProductId == selectedProduct.Id);
                if (existingProductInCart == null)
                {
                    NewTransaction.IncludedProducts.Add(new ProductVariantContext
                    {
                        ProductId = selectedProduct.Id,
                        Quantity = 1,
                        Price = selectedProduct.SalePrice,
                        ProductName = selectedProduct.Name,
                        UnitName = selectedProduct.Unit.DisplayName
                    });
                }
                else
                {
                    existingProductInCart.Quantity++;
                }

                _barcode = string.Empty;
            }
        }
    }
}