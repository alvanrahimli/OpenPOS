using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenPOS.Domain.Data;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;
using OpenPOS.Infrastructure.Interfaces;

namespace OpenPOS.Infrastructure.Repositories
{
    public class CategoriesRepository : ICategoriesRepository
    {
        private readonly PosContext _context;
        private readonly UserManager<PosUser> _userManager;
        private readonly IMapper _mapper;

        public CategoriesRepository(PosContext context,
            UserManager<PosUser> userManager,
            IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<List<CategoryDto>> GetCategories(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user?.SelectedStoreId == null)
            {
                return new List<CategoryDto>();
            }

            var categories = await _context.Categories.Where(c => c.StoreId == user.SelectedStoreId).ToListAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> AddCategory(string userId, AddCategoryDto categoryDto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user?.SelectedStoreId == null)
            {
                return null;
            }

            var category = _mapper.Map<Category>(categoryDto);
            category.StoreId = (Guid)user.SelectedStoreId;

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
            return _mapper.Map<CategoryDto>(category);
        }
    }
}