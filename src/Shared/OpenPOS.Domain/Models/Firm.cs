using System;
using System.Collections.Generic;

namespace OpenPOS.Domain.Models
{
    public class Firm
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Contact { get; set; }
        public string[] Tags { get; set; }
        public Guid StoreId { get; set; }
        public Store Store { get; set; }

        public List<Product> Products { get; set; }
    }
}