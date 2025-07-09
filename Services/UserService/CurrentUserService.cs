using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace Services.UserService
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService() { }

        public CurrentUserService(IHttpContextAccessor httpContextAccessor) { }
        public string? GetUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
