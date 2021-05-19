using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace OpenPOS.Domain.Models
{
    public class PosUser : IdentityUser
    {
        public string FullName { get; set; }
        
        public Guid? SelectedStoreId { get; set; }
        public Store SelectedStore { get; set; }

        public List<Store> Stores { get; set; }
    }
}