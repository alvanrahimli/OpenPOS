using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using OpenPOS.Domain.Data;

namespace OpenPOS.Sale.Extensions
{
    public static class CacheExtensions
    {
        public static async Task<Guid> GetSelectedStore(this IDistributedCache cache, PosContext context, string userId)
        {
            var storeId = await cache.GetStringAsync($"selected_store:{userId}");
            if (storeId == null)
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    throw new ArgumentNullException(nameof(user));
                }

                if (user.SelectedStoreId == null)
                {
                    return Guid.Empty;
                }

                await cache.SetStringAsync($"selected_store:{user.Id}", user.SelectedStoreId.ToString());
                return (Guid) user.SelectedStoreId;
            }

            var exactPart = storeId.Split(":")[0];
            return Guid.Parse(exactPart);
        }
    }
}