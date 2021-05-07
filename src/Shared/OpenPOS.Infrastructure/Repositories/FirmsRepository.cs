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
    public class FirmsRepository : IFirmsRepository
    {
        private readonly PosContext _context;

        public FirmsRepository(PosContext context)
        {
            _context = context;
        }
        
        public async Task<List<Firm>> GetFirms(Guid storeId)
        {
            var firms = await _context.Firms.Where(f => f.StoreId == storeId).ToListAsync();
            return firms;
        }
    }
}