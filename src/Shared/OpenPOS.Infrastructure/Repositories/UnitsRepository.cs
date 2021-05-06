using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Models;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Infrastructure.Repositories
{
    public class UnitsRepository : IUnitsRepository
    {
        private readonly PosContext _context;

        public UnitsRepository(PosContext context)
        {
            _context = context;
        }
        
        public async Task<List<Unit>> GetUnits()
        {
            var units = await _context.Units.ToListAsync();
            return units;
        }
    }
}