using System;
using System.Collections.Generic;

namespace OpenPOS.Domain.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public Guid? ParentId { get; set; }
        public Category Parent { get; set; }

        public Guid StoreId { get; set; }
        public Store Store { get; set; }
        
        public List<Category> Children { get; set; }
    }
}