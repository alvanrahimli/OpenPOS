using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Models;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Infrastructure.Repositories
{
    public class ClientsRepository : IClientsRepository
    {
        private readonly PosContext _context;

        public ClientsRepository(PosContext context)
        {
            _context = context;
        }
        
        public async Task<List<Client>> GetClientsForStore(Guid storeId)
        {
            return await _context.Clients
                .AsNoTracking()
                .Where(c => c.StoreId == storeId)
                .ToListAsync();
        }
    }
}