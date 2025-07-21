using OfficeNet.Domain.Contracts;
using System.Security.Claims;

namespace OfficeNet.Service.UserClaim
{
    public interface IUserClaimService
    {
        Task<bool> AddPermissionAsync(string userId, string permission);
        Task<IEnumerable<Claim>> GetUserClaimsAsync(long userId);
        Task<bool> RemoveClaimFromUserAsync(UserCalimDto dto);
        //Task<bool> RemoveClaimFromRoleAsync(RoleClaimDto dto);
        //Task<bool> CreateRoleWithClaimsAsync(string roleName, List<string> permissions);
        //Task<bool> AddPermissionsToUserAsync(long userid , List<string> permissions);
    }
}
