using Microsoft.AspNetCore.Identity;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;

namespace OfficeNet.Service.Roles
{
    public interface IRoleService
    {
        Task<ApplicationRole> AddRoleAsync(string roleName);
        Task<UserRole> AssingRoleToUserAync(UserRole user);
    }


}
