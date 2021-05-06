using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Infrastructure.Repositories
{
    public class StoresRepository : IStoresRepository
    {
        private readonly PosContext _context;
        private readonly ILogger<StoresRepository> _logger;
        private readonly IMapper _mapper;

        public StoresRepository(PosContext context, ILogger<StoresRepository> logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }
        
        public async Task<List<StoreDto>> GetStoresForUser(string userId)
        {
            var stores = await _context.Stores.AsNoTracking().Where(s => s.UserId == userId).ToListAsync();
            return _mapper.Map<List<StoreDto>>(stores);
        }

        public async Task<StoreDto> GetStore(Guid storeId)
        {
            throw new NotImplementedException();
        }

        public async Task<StoreDto> CreateStore(StoreDto input)
        {
            var store = _mapper.Map<Store>(input);
            await _context.Stores.AddAsync(store);
            var dbRes = await _context.SaveChangesAsync();
            if (dbRes <= 0) return null;
            
            _logger.LogInformation("Created Store {StoreId} for User {UserId}", store.Id, store.UserId);
            return _mapper.Map<StoreDto>(store);
        }

        public async Task<StoreDto> UpdateStore(StoreDto input)
        {
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.Id == input.Id);
            if (store == null) return null;
            
            _mapper.Map(input, store);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated Store {StoreId}", store.Id);
            return _mapper.Map<StoreDto>(store);
        }

        public async Task<bool> DeleteStore(Guid storeId)
        {
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.Id == storeId);
            if (store == null)
            {
                _logger.LogWarning("Could not find Store '{StoreId}' to delete", storeId);
                return false;
            }

            _context.Stores.Remove(store);
            var dbRes = await _context.SaveChangesAsync();
            if (dbRes > 0)
            {
                _logger.LogWarning("Deleted Store '{StoreId}'", storeId);
                return true;
            }

            _logger.LogWarning("Could not delete Store '{StoreId}'", storeId);
            return false;
        }
    }
}