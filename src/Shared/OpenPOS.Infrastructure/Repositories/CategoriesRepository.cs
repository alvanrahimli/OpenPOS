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
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly PosContext _context;

        public CategoriesRepository(PosContext context)
        {
            _context = context;
        }
        
        public async Task<List<Category>> GetCategories(Guid storeId)
        {
            var categories = await _context.Categories.Where(c => c.StoreId == storeId).ToListAsync();
            return categories;
        }
    }
}