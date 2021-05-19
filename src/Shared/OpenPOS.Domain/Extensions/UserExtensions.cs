using System;
using System.Linq;
using System.Security.Claims;

namespace OpenPOS.Domain.Extensions
{
    public static class UserExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            if (user?.Identity == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (!user.Identity.IsAuthenticated)
                return null;
            
            var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }
    }
}