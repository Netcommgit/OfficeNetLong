
using Microsoft.EntityFrameworkCore;
using OfficeNet.Infrastructure.Context;

namespace OfficeNet.Service.Permission
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;

        public PermissionService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<bool> HasPermissionAsync(string userId, string permission)
        {
            if (!long.TryParse(userId, out var userIdLong))
                return false; // Invalid user ID format

            var hasUserPermission = await _context.UserClaims
                .AnyAsync(c =>
                    c.UserId == userIdLong &&
                    c.ClaimType == "Permission" &&
                    c.ClaimValue == permission);

            if (hasUserPermission)
                return true;

            var roleIds = await _context.UserRoles
                .Where(ur => ur.UserId == userIdLong)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (!roleIds.Any())
                return false;

            var hasRolePermission = await _context.RoleClaims
                .AnyAsync(rc =>
                    roleIds.Contains(rc.RoleId) &&
                    rc.ClaimType == "Permission" &&
                    rc.ClaimValue == permission);

            return hasRolePermission;
        }
    }
}
