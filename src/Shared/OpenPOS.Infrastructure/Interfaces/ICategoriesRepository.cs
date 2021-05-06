using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenPOS.Domain.Models;

namespace OpenPOS.Infrastructure.Interfaces
{
    public interface ICategoriesRepository
    {
        Task<List<Category>> GetCategories(Guid storeId);
    }
}