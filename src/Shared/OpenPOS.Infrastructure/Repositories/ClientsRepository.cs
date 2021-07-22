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
    public class ClientsRepository : IClientsRepository
    {
        private readonly PosContext _context;
        private readonly UserManager<PosUser> _userManager;

        public ClientsRepository(PosContext context, 
            UserManager<PosUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<List<Client>> GetClientsForUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var clients = await _context.Clients.AsNoTracking()
                .Where(c => c.StoreId == user.SelectedStoreId)
                .ToListAsync();
            return clients;
        }
    }
}