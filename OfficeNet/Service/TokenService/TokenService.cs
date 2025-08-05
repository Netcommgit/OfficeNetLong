using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OfficeNet.Service.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey  _secretkey;
        private readonly string? _validIssuer;
        private readonly string? _validAudience;
        private readonly double _expires;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<TokenService> _logger;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager, ILogger<TokenService> logger,RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            var jwtSetting = configuration.GetSection("JwtSettings").Get<JwtSetting>();
            if (jwtSetting == null || string.IsNullOrEmpty(jwtSetting.Key)) 
            {
                throw new InvalidOperationException("JWT secret key is not configured.");
            }
            _secretkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.Key));
            _validIssuer = jwtSetting.ValidIssuer;
            _validAudience = jwtSetting.ValidAudience;
            _expires = jwtSetting.Expires;
            _roleManager = roleManager;
        }

        public async Task<string> GenerateToken(ApplicationUser user)
        {
            var signingCredentials = new SigningCredentials(_secretkey, SecurityAlgorithms.HmacSha256);
            var claims = await GetClaimsasync(user);
            var tokenOptions = GenerateTokenOptions(signingCredentials,claims);
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        private async Task<List<Claim>> GetClaimsasync(ApplicationUser user)
        {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name,user?.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Email,user.Email),
                new Claim("FirstName",user.FirstName),
                new Claim("LastName",user.LastName),
                new Claim("Gender",user.Gender)
                //,
                //new Claim(ClaimTypes.Role,"admin")   // added by ashsih 5 may 2025
            };
            var roles = await _userManager.GetRolesAsync(user);
            //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            //foreach (var roleName in roles)
            //{
            //    var role = await _roleManager.FindByNameAsync(roleName);
            //    if (role != null)
            //    {
            //        var roleClaims = await _roleManager.GetClaimsAsync(role);
            //        claims.AddRange(roleClaims.Where(c => c.Type == "Permission"));
            //    }
            //}

            return claims;
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials , List<Claim> claims) 
        {
            var issuedAt = DateTime.UtcNow;
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat,
               new DateTimeOffset(issuedAt).ToUnixTimeSeconds().ToString(),
               ClaimValueTypes.Integer64)
            );
          
            return new JwtSecurityToken(
                issuer: _validIssuer,
                audience: _validAudience,
                claims: claims,
                notBefore: issuedAt,
                expires: issuedAt.AddSeconds(_expires),
                signingCredentials: signingCredentials
            );
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng =  RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var refreshToken =  Convert.ToBase64String(randomNumber);
            return refreshToken;
        }
       
    }
}
