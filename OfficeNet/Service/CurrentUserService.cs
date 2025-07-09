using System.Security.Claims;

namespace OfficeNet.Service
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public CurrentUserService() { }

        public CurrentUserService(IHttpContextAccessor contextAccessor) 
        {
            _contextAccessor = contextAccessor;
        }
        public long GetUserId()
        {
            var userIdString = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdString, out var userId))
            {
                return userId;
            }

            return 0;
        }
    }
}
