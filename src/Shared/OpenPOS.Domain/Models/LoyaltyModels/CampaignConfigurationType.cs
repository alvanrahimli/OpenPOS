namespace OpenPOS.Domain.Models.LoyaltyModels
{
    public enum CampaignConfigurationType
    {
        // Shared
        FromDate,
        ToDate,
        
        // CashBack
        // AppliedProduct,
        CashBackPercentage,
        
        // Bonus
        AppliedProduct,
        BonusCount,
        
        // Discount
        DiscountPercentage,
        DiscountPrice,
        
        // Vaucher
        // AppliedProduct,
        BundleSize,
        // DiscountPrice,
        // DiscountPercent,
        GiftProduct,
        
        // Promocode
        AppliedClient,
        // AppliedProduct
        // DiscountPrice
        // DiscountPercent,

        // Birthday
        // DiscountPrice
        // DiscountPercent,
        
        

        // TODO: Continue this
    }
}