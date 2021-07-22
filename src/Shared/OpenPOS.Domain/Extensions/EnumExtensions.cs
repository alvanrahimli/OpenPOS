using System;
using OpenPOS.Domain.Enums;

namespace OpenPOS.Domain.Extensions
{
    public static class EnumExtensions
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

        public static string Stringify(this TransactionType type)
        {
            var str = type switch
            {
                TransactionType.Income => "Alış",
                TransactionType.Return => "Geri qaytarma",
                TransactionType.Sale => "Satış",
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };

            return str;
        }
    }
}