using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Models;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Infrastructure.Repositories
{
    public class FirmsRepository : IFirmsRepository
    {
        private readonly PosContext _context;
        private readonly UserManager<PosUser> _userManager;

        public FirmsRepository(PosContext context,
            UserManager<PosUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        public async Task<List<Firm>> GetFirms(Guid storeId)
        {
            var firms = await _context.Firms.Where(f => f.StoreId == storeId).ToListAsync();
            return firms;
        }

        public async Task<List<Firm>> GetFirmsForUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var firms = await _context.Firms.AsNoTracking()
                .Where(c => c.StoreId == user.SelectedStoreId)
                .ToListAsync();
            return firms;
        }
    }
}