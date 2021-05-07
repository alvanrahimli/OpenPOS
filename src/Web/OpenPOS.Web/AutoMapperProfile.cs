using System;
using AutoMapper;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;

namespace OpenPOS.Web
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Store, StoreDto>();
            CreateMap<ProductDto, Product>()
                .ForMember(b => b.FirmId, x => x.MapFrom(a => a.FirmId == Guid.Empty ? null : a.FirmId));
            CreateMap<Product, ProductDto>();
        }
    }
}