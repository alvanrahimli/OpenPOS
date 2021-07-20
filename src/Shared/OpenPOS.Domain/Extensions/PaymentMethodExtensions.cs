using System;
using OpenPOS.Domain.Enums;

namespace OpenPOS.Domain.Extensions
{
    public static class PaymentMethodExtensions
    {
        public static string Stringify(this PaymentMethod method)
        {
            var str = method switch
            {
                PaymentMethod.Cash => "Nəğd",
                PaymentMethod.Card => "Kartla",
                PaymentMethod.Loan => "Nisyə",
                PaymentMethod.Unknown => "Bilinmir",
                _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
            };

            return str;
        }
    }
}