using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OfficeNet.Service.UserClaim;

namespace OfficeNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserClaimController : ControllerBase
    {
        private readonly IUserClaimService _userClaimService;
        public UserClaimController(IUserClaimService userClaimService)
        {
            _userClaimService = userClaimService;
        }
        [HttpPost("AddPermission")]
        public async Task<IActionResult> AddPermissionAsync(string userId, string permission)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(permission))
            {
                return BadRequest("User ID and permission cannot be null or empty.");
            }
            var result = await _userClaimService.AddPermissionAsync(userId, permission);
            if (result)
            {
                return Ok("Permission added successfully.");
            }
            else
            {
                return BadRequest("Failed to add permission.");
            }
        }

        [HttpGet("GetUserClaims")]
        public async Task<IActionResult> GetUserClaims(long userId)
        {
            try
            {
                var claims = await _userClaimService.GetUserClaimsAsync(userId);
                return Ok(claims.Select(c => new { c.Type, c.Value }));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
