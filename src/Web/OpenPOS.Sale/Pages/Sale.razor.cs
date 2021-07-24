using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
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
        private string _uiMessage;
        private CreateTransactionContext NewTransaction { get; set; }
        private List<string> _customers;
        
        [Inject] public IDbContextFactory<PosContext> DbContextFactory { get; set; }
        [Inject] public IStoresRepository StoresRepository { get; set; }
        [Inject] public IHttpContextAccessor HttpContextAccessor { get; set; }
        [Inject] public ILogger<Sale> Logger { get; set; }
        [Inject] public IDistributedCache Cache { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public IJSRuntime JsRuntime { get; set; }

        private bool IsBusy { get; set; }

        protected override async Task OnInitializedAsync()
        {
            NewTransaction = new CreateTransactionContext();

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
                _uiMessage = "Yenidən cəhd edin";
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
                    _uiMessage = $"Məhsul '{_barcode}' tapılmadı.";
                    return;
                }
                
                if (selectedProduct.StockCount == 0)
                {
                    _uiMessage = $"{selectedProduct.Name} stokda yoxdur";
                }

                var existingProductInCart = NewTransaction.IncludedProducts
                    .FirstOrDefault(p => p.ProductId == selectedProduct.Id);
                if (existingProductInCart == null)
                {
                    NewTransaction.IncludedProducts.Add(new ProductVariantContext
                    {
                        ProductId = selectedProduct.Id,
                        Quantity = 1,
                        SalePrice = selectedProduct.SalePrice,
                        ProductName = selectedProduct.Name,
                        UnitName = selectedProduct.Unit.DisplayName
                    });
                }
                else
                {
                    existingProductInCart.Quantity++;
                }

                _barcode = string.Empty;
                _uiMessage = null;
            }
        }

        private async Task SubmitTransaction()
        {
            if (NewTransaction.IncludedProducts.Count == 0)
            {
                _uiMessage = "Satmaq üçün məhsul əlavə edin";
                return;
            }
            
            if (IsBusy)
            {
                return;
            }

            try
            {
                IsBusy = true;

                await using var context = DbContextFactory.CreateDbContext();
                var userId = HttpContextAccessor.HttpContext?.User.GetUserId();
                var storeId = await Cache.GetSelectedStore(context, userId);

                var transaction = new Transaction
                {
                    Timestamp = DateTime.UtcNow,
                    Type = TransactionType.Sale,
                    PaymentMethod = NewTransaction.PaymentMethod,
                    StoreId = storeId,
                    TotalPrice = NewTransaction.TotalAmount,
                    PayedAmount = NewTransaction.PayedAmount,
                    ReturnAmount = NewTransaction.ReturnAmount,
                    IncludedProducts = new List<ProductVariant>(),
                    Notes = NewTransaction.Notes
                };
                
                // TODO: Report failed products to user 
                var unfoundProducts = new List<Guid>();
                foreach (var soldProduct in NewTransaction.IncludedProducts)
                {
                    var product = await context.Products
                        .Include(p => p.Unit)
                        .FirstOrDefaultAsync(p => p.Id == soldProduct.ProductId);
                    if (product == null)
                    {
                        unfoundProducts.Add(soldProduct.ProductId);
                        continue;
                    }

                    // Change product stock count according transaction type
                    switch (transaction.Type)
                    {
                        case TransactionType.Sale:
                            product.StockCount -= soldProduct.Quantity;
                            break;
                        case TransactionType.Return:
                            product.StockCount += soldProduct.Quantity;
                            break;
                        case TransactionType.Income:
                            product.StockCount += soldProduct.Quantity;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(transaction.Type));
                    }
                    product.LastModifiedDate = DateTime.UtcNow;
                
                    var productVariant = new ProductVariant
                    {
                        Quantity = soldProduct.Quantity,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        UnitName = product.Unit.DisplayName,
                        SalePrice = product.SalePrice
                    };
                    transaction.IncludedProducts.Add(productVariant);
                }

                // Add client if name was given
                if (!string.IsNullOrEmpty(NewTransaction.ClientName))
                {
                    var client = await context.Clients.FirstOrDefaultAsync(c => c.Name == NewTransaction.ClientName
                        && c.StoreId == storeId);
                    if (client == null)
                    {
                        var newClient = new Client
                        {
                            Name = NewTransaction.ClientName,
                            StoreId = storeId,
                            Notes = NewTransaction.ClientNotes,
                            FirstSaleDate = DateTime.UtcNow,
                            LastSaleDate = DateTime.UtcNow,
                            Debt = transaction.PaymentMethod == PaymentMethod.Loan ? transaction.TotalPrice : 0
                        };

                        await context.Clients.AddAsync(newClient);
                        await context.SaveChangesAsync();
                        transaction.ClientId = newClient.Id;
                    }
                    else
                    {
                        if (transaction.PaymentMethod == PaymentMethod.Loan)
                        {
                            client.Debt += transaction.TotalPrice;
                        }
                        client.LastSaleDate = DateTime.UtcNow;
                        transaction.ClientId = client.Id;
                    }
                }

                await context.Transactions.AddAsync(transaction);
                var dbRes = await context.SaveChangesAsync();
                if (dbRes <= 0)
                {
                    await AlertifyError("Satış həyata keçmədi");
                    _uiMessage = "Satış həyata keçmədi";
                }

                // Successfull
                await AlertifySuccess("Satış həyata keçirildi");
                await JsRuntime.InvokeVoidAsync("eval", "document.getElementById('barcode-input').focus()");
                ResetForm();
            }
            catch (Exception e)
            {
                Logger.LogError("Error occured: {Error}", e.Message);
                await AlertifyError("Yenidən cəhd edin");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void ResetForm()
        {
            NewTransaction = new CreateTransactionContext();
            _uiMessage = null;
        }

        private async Task AlertifySuccess(string text)
        {
            await JsRuntime.InvokeVoidAsync("alertify.success", text);
        }
        
        private async Task AlertifyWarning(string text)
        {
            await JsRuntime.InvokeVoidAsync("alertify.warning", text);
        }
        
        private async Task AlertifyError(string text)
        {
            await JsRuntime.InvokeVoidAsync("alertify.error", text);
        }
    }
}