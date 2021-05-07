using System;
using System.Linq;

namespace OpenPOS.Infrastructure
{
    public static class Helper
    {
        public static string GenerateRandomNumericString(int length = 10)
        {
            const string chars = "0123456789";
 
            var random = new Random();
            var randomString = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return randomString;
        }
    }
}