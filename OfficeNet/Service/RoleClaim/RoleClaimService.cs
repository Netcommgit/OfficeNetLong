using Microsoft.AspNetCore.Identity;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using System.Security.Claims;

namespace OfficeNet.Service.RoleClaim
{
    public class RoleClaimService : IRoleClaimService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        public RoleClaimService(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<bool> AddClaimToRoleAsync(RoleClaimDto dto)
        {
            var role = await _roleManager.FindByIdAsync(dto.RoleId.ToString());
            if (role == null)
                throw new KeyNotFoundException("Role not found");

            var existingClaims = await _roleManager.GetClaimsAsync(role);
            if (existingClaims.Any(c => c.Type == dto.ClaimType && c.Value == dto.ClaimValue))
                return false;

            var result = await _roleManager.AddClaimAsync(role, new Claim(dto.ClaimType, dto.ClaimValue));
            return result.Succeeded;
        }

        public  async Task<IEnumerable<Claim>> GetRoleClaimsAsync(long roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                throw new KeyNotFoundException("Role not found");

            return await _roleManager.GetClaimsAsync(role);
        }

        public async Task<bool> RemoveClaimFromRoleAsync(RoleClaimDto dto)
        {
            var role = await _roleManager.FindByIdAsync(dto.RoleId.ToString());
            if (role == null)
                throw new KeyNotFoundException("Role not found");

            var result = await _roleManager.RemoveClaimAsync(role, new Claim(dto.ClaimType, dto.ClaimValue));
            return result.Succeeded;
        }
        public async Task<bool> AddPermissionsToRoleAsync(long roleId, List<string> permissions)
        {
            var role = await _roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                throw new KeyNotFoundException("Role not found");

            var existingClaims = await _roleManager.GetClaimsAsync(role);
            bool addedAny = false;

            foreach (var permission in permissions)
            {
                bool alreadyExists = existingClaims.Any(c =>
                    c.Type == "Permission" && c.Value.Equals(permission, StringComparison.OrdinalIgnoreCase));

                if (!alreadyExists)
                {
                    var result = await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                    if (result.Succeeded)
                        addedAny = true;
                }
            }

            return addedAny;
        }

        public async Task<bool> CreateRoleWithClaimsAsync(string roleName, List<string> permissions)
        {
            var role = await _roleManager.FindByNameAsync(roleName);

            if (role == null)
            {
                var createResult = await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
                if (!createResult.Succeeded)
                    throw new Exception("Failed to create role.");

                // Fetch the newly created role
                role = await _roleManager.FindByNameAsync(roleName);
            }

            var existingClaims = await _roleManager.GetClaimsAsync(role);
            bool addedAny = false;

            foreach (var permission in permissions)
            {
                bool alreadyExists = existingClaims.Any(c =>
                    c.Type == "Permission" && c.Value.Equals(permission, StringComparison.OrdinalIgnoreCase));

                if (!alreadyExists)
                {
                    var result = await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
                    if (result.Succeeded)
                        addedAny = true;
                }
            }

            return addedAny;
        }

    }
}
