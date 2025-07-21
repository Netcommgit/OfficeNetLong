using Microsoft.AspNetCore.Identity;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using System.Security.Claims;

namespace OfficeNet.Service.UserClaim
{
    public class UserClaimService : IUserClaimService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserClaimService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> AddPermissionAsync(string userId, string permission)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var existingClaims = await _userManager.GetClaimsAsync(user);
            if (existingClaims.Any(c => c.Type == "Permission" && c.Value == permission))
                return true; // Already exists

            var result = await _userManager.AddClaimAsync(user, new Claim("Permission", permission));
            return result.Succeeded;
        }

        public async Task<IEnumerable<Claim>> GetUserClaimsAsync(long userId)
        {
            var role = await _userManager.FindByIdAsync(userId.ToString());
            if (role == null)
                throw new KeyNotFoundException("Role not found");

            return await _userManager.GetClaimsAsync(role);
        }

        public async Task<bool> RemoveClaimFromUserAsync(UserCalimDto dto)
        {
            var role = await _userManager.FindByIdAsync(dto.UserId.ToString());
            if (role == null)
                throw new KeyNotFoundException("Role not found");
            var result = await _userManager.RemoveClaimAsync(role, new Claim(dto.ClaimType, dto.ClaimValue));
            return result.Succeeded;
        }
    }
}
