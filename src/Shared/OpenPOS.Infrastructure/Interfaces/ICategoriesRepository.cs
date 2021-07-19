using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenPOS.Domain.Models;
using OpenPOS.Domain.Models.Dtos;

namespace OpenPOS.Infrastructure.Interfaces
{
    public interface ICategoriesRepository
    {
        Task<List<CategoryDto>> GetCategories(string userId);
        Task<CategoryDto> AddCategory(string userId, AddCategoryDto categoryDto);
    }
}