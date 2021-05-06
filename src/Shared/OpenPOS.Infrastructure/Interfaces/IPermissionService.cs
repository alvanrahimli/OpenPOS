using System;
using System.Threading.Tasks;
using OpenPOS.Domain.Enums;

namespace OpenPOS.Infrastructure.Interfaces
{
    public interface IPermissionService
    {
        Task<PermissionResult> UserHasPermission(string userId, Guid storeId);
    }
}