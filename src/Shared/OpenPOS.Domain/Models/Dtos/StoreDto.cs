using System;

namespace OpenPOS.Domain.Models.Dtos
{
    public class StoreDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
        public string UserId { get; set; }
    }
}