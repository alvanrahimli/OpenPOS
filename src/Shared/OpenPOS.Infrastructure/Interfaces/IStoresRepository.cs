using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenPOS.Domain.Models.Dtos;

namespace OpenPOS.Infrastructure.Interfaces
{
    public interface IStoresRepository
    {
        Task<StoreDto> GetSelectedStore(string userId);
        Task<StoreDto> SelectStore(string userId, Guid storeId);
        Task<List<StoreDto>> GetStoresForUser(string userId);
        Task<StoreDto> GetStore(Guid storeId);
        Task<StoreDto> CreateStore(StoreDto input);
        Task<StoreDto> UpdateStore(StoreDto input);
        Task<bool> DeleteStore(Guid storeId);
    }
}