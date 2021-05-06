using System;
using System.Linq;
using System.Security.Claims;

namespace OpenPOS.Domain.Extensions
{
    public static class UserExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            if (user?.Identity == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.Identity.IsAuthenticated)
            {
                var userId = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    return Guid.Empty;
                }

                return Guid.Parse(userId);
            }
            
            return Guid.Empty;
        }
    }
}