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
        }
    }
}