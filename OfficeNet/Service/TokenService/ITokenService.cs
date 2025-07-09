using OfficeNet.Domain.Entities;

namespace OfficeNet.Service.TokenService
{
    public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);

        string GenerateRefreshToken();
    }
}
