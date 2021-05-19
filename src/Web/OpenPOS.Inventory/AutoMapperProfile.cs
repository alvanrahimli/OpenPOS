using System;
using System.Linq;
using AutoMapper;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Contexts;

namespace OpenPOS.Inventory
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Store, StoreDto>();
            CreateMap<ProductDto, Product>()
                .ForMember(b => b.FirmId, x => x.MapFrom(a => a.FirmId == Guid.Empty ? null : a.FirmId))
                .ForMember(b => b.CreationDate, x => x.MapFrom(a => DateTime.UtcNow))
                .ForMember(b => b.LastModifiedDate, x => x.MapFrom(a => DateTime.UtcNow));
            CreateMap<Product, ProductDto>()
                .ForMember(b => b.CategoryName, x => x.MapFrom(a => a.Category == null ? null : a.Category.Name))
                .ForMember(b => b.FirmName, x => x.MapFrom(a => a.Firm == null ? null : a.Firm.Name));

            CreateMap<CreateTransactionContext, Transaction>()
                .ForMember(b => b.Timestamp, x => x.MapFrom(a => DateTime.UtcNow))
                .ForMember(b => b.IncludedProducts, x => x.Ignore());
            CreateMap<Transaction, TransactionDto>();
        }
    }
}