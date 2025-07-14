using OfficeNet.Domain.Contracts;
using System.Security.Claims;

namespace OfficeNet.Service.RoleClaim
{
    public interface IRoleClaimService
    {
        Task<IEnumerable<Claim>> GetRoleClaimsAsync(long roleId);
        Task<bool> AddClaimToRoleAsync(RoleClaimDto dto);
        Task<bool> RemoveClaimFromRoleAsync(RoleClaimDto dto);
        Task<bool> CreateRoleWithClaimsAsync(string roleName, List<string> permissions);
        Task<bool> AddPermissionsToRoleAsync(long roleId, List<string> permissions);
    }
}
