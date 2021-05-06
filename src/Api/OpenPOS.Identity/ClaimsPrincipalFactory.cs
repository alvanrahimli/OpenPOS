using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using OpenPOS.Domain.Models;

namespace OpenPOS.Identity
{
    public class ClaimsPrincipalFactory : UserClaimsPrincipalFactory<PosUser>
    {
        private readonly UserManager<PosUser> _userManager;
        private readonly IOptions<IdentityOptions> _optionsAccessor;

        public ClaimsPrincipalFactory(UserManager<PosUser> userManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
            _userManager = userManager;
            _optionsAccessor = optionsAccessor;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(PosUser user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var identity = await base.GenerateClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            if (roles is {Count: > 0}) identity.AddClaim(new Claim("Role", string.Join(',', roles)));

            identity.AddClaim(new Claim("Name", user.FullName ?? user.UserName));

            return identity;
        }
    }
}