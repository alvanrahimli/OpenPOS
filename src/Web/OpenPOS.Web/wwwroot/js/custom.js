const salePriceInput = document.getElementById("sale-price-input");
const purchasePriceInput = document.getElementById("purchase-price-input");
const profitInput = document.getElementById("profit-rate-input");

function calcProfit (e) {
    let salePrice = parseInt(salePriceInput.value);
    let purchasePrice = parseInt(purchasePriceInput.value);
    
    let profit = (salePrice - purchasePrice) / purchasePrice * 100;
    profit = Math.round(profit * 100) / 100;
    profitInput.value = profit;
}