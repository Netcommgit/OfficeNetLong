using Microsoft.AspNetCore.Authorization;
using OfficeNet.Infrastructure.Context;
using OfficeNet.Service.RoleAuthHandler;

namespace OfficeNet.Permissons
{
    public static class DynamicPolicyRegister
    {
        public static void RegisterPermissionsFromDatabase(AuthorizationOptions options, IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var permissionClaims = dbContext.RoleClaims
                .Where(rc => rc.ClaimType == "Permission")
                .Select(rc => rc.ClaimValue)
                .Distinct()
                .ToList();

            foreach (var permission in permissionClaims)
            {
                //options.AddPolicy(permission, policy =>
                //    policy.RequireClaim("Permission", permission));
                options.AddPolicy(permission, policy =>
                    policy.Requirements.Add(new PermissionRequirement(permission)));
            }
        }
    }
}
