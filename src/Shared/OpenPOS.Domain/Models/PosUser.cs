using Microsoft.AspNetCore.Identity;

namespace OpenPOS.Domain.Models
{
    public class PosUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}