using System;

namespace OpenPOS.Domain.Models
{
    public class Client
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }

        public Guid StoreId { get; set; }
        public Store Store { get; set; }
    }
}