using Microsoft.AspNetCore.Authorization;
using OfficeNet.Service.Permission;
using System.Security.Claims;

namespace OfficeNet.Service.RoleAuthHandler
{
    public class CentralAuthorizationHandler : IAuthorizationHandler
    {
        private readonly IPermissionService _permissionService;

        public CentralAuthorizationHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        public async Task HandleAsync(AuthorizationHandlerContext context)
        {
            foreach (var requirement in context.Requirements)
            {
                switch (requirement)
                {
                    case PermissionRequirement roleRequirement:
                        await HandleRolePermissionAsync(context, roleRequirement);
                        break;
                }
            }
        }

        private async Task HandleRolePermissionAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? context.User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId))
                return;

            var hasPermission = await _permissionService.HasPermissionAsync(userId, requirement.Permission);
            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
}
