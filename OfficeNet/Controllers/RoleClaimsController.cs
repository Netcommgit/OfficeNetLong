using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeNet.Domain.Contracts;
using OfficeNet.Service.RoleClaim;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleClaimsController : ControllerBase
    {
        private readonly IRoleClaimService _roleClaimService;

        public RoleClaimsController(IRoleClaimService roleClaimService)
        {
            _roleClaimService = roleClaimService;
        }

        
        [HttpGet("{roleId:long}")]
        [Authorize]
        public async Task<IActionResult> GetClaims(long roleId)
        {
            try
            {
                var claims = await _roleClaimService.GetRoleClaimsAsync(roleId);
                return Ok(claims.Select(c => new { c.Type, c.Value }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        
        [HttpPost]
        [Authorize(Policy ="Claim.Create")]
        public async Task<IActionResult> AddClaim([FromBody] RoleClaimDto dto)
        {
            try
            {
                var result = await _roleClaimService.AddClaimToRoleAsync(dto);
                if (!result)
                    return BadRequest(new { message = "Claim already exists or could not be added." });

                return Ok(new { message = "Claim added successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        
        [HttpDelete]
        [Authorize(Policy = "Claim.Delete")]
        public async Task<IActionResult> RemoveClaim(RoleClaimDto dto)
        {
            try
            {
                var result = await _roleClaimService.RemoveClaimFromRoleAsync(dto);
                if (!result)
                    return BadRequest(new { message = "Failed to remove claim." });

                return Ok(new { message = "Claim removed successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new role and assigns permissions to it.
        /// </summary>
        [HttpPost("CreateRoleWithClaims")]
        [Authorize(Policy = "Claim.Create")]
        public async Task<IActionResult> CreateRoleWithClaims(RoleWithClaimsDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RoleName) || dto.Permissions == null || !dto.Permissions.Any())
                return BadRequest("Role name and permissions are required.");

            var result = await _roleClaimService.CreateRoleWithClaimsAsync(dto.RoleName, dto.Permissions);

            if (result)
                return Ok("Role and permissions created successfully.");
            else
                return BadRequest("Role creation or permission assignment failed or already exists.");
        }

        /// <summary>
        /// Adds permissions to an existing role.
        /// </summary>
        [HttpPost("AddPermissionsToRole")]
        [Authorize(Policy = "Claim.Create")]
        public async Task<IActionResult> AddPermissionsToRole(AddRolePermissionsDto dto)
        {
            if (dto.RoleId <= 0 || dto.Permissions == null || !dto.Permissions.Any())
                return BadRequest("Role ID and permissions are required.");

            var result = await _roleClaimService.AddPermissionsToRoleAsync(dto.RoleId, dto.Permissions);

            if (result)
                return Ok("Permissions added to role successfully.");
            else
                return BadRequest("Permissions were already assigned or failed to add.");
        }
    }
}
