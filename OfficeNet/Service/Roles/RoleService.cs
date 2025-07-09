using Microsoft.AspNetCore.Identity;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using System.Runtime.Intrinsics.X86;

namespace OfficeNet.Service.Roles
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService ;
        public RoleService(RoleManager<ApplicationRole> roleManager , UserManager<ApplicationUser> userManager, ICurrentUserService currentUserService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _currentUserService = currentUserService;
        }
        public async Task<ApplicationRole> AddRoleAsync(string roleName)
        {
            var role = new ApplicationRole
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                CreatedBy = _currentUserService.GetUserId(), // Assuming you have a way to get the current user's ID
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                throw new Exception(string.Join("; ", result.Errors.Select(e => e.Description)));

            return role;
        }

       

        public async Task<UserRole> AssingRoleToUserAync(UserRole userRole)
        {
            var user = await _userManager.FindByIdAsync(userRole.UserId.ToString());
            if (user == null)
                throw new Exception("User not found");

            var resultrole = await _roleManager.RoleExistsAsync(userRole.RoleId);
               /// throw new Exception("Role does not exist");

            if (!await _roleManager.RoleExistsAsync(userRole.RoleId))
                throw new Exception("Role does not exist");

            var result = await _userManager.AddToRoleAsync(user, userRole.RoleId);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to assign role: {errors}");
            }

            return userRole;
        }
    }
}
