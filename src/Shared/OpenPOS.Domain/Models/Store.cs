using System;
using System.Collections.Generic;

namespace OpenPOS.Domain.Models
{
    public class Store
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid? ParentId { get; set; }
        public Store Parent { get; set; }

        public string UserId { get; set; }
        public PosUser User { get; set; }

        public List<PosUser> Employees { get; set; }
        public List<Client> Clients { get; set; }
        public List<Category> Categories { get; set; }
        public List<Product> Products { get; set; }
        public List<Store> Children { get; set; }
    }
}