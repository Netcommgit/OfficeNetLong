using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using OfficeNet.Domain.Contracts;
using OfficeNet.Domain.Entities;
using OfficeNet.Service.TokenService;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace OfficeNet.Service.UserService
{
    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _configuration;

        public UserService(ITokenService tokenService, ICurrentUserService currentUserService, UserManager<ApplicationUser> userManager, IMapper mapper, ILogger<UserService> logger,IConfiguration configuration) 
        {
            _tokenService = tokenService;
            _currentUserService = currentUserService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<UserResponse> RegisterAsync(UserRegisterRequest request)
        {
            try { 
            _logger.LogInformation("Registering User");
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null) 
            {
                _logger.LogError("Email already exists.");
                throw new Exception("Email alreday exists.");
            }
            var newUser = _mapper.Map<ApplicationUser>(request);
            //Generate a unique username
            newUser.UserName = GenerateUserName(request.FirstName,request.LastName);
            newUser.EmpCode = await GenerateEmployeeCodeAsync();
            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded) 
            {
                var errors = string.Join(",", result.Errors.Select(e => e.Description));
                _logger.LogError($"Failed to create user:{errors}",errors);
                throw new Exception($"Failed to create user:{errors}");
            }
            _logger.LogInformation($"User created sucessfully");
            await _userManager.AddToRoleAsync(newUser, "User");
            await _tokenService.GenerateToken(newUser); 
            return _mapper.Map<UserResponse>(newUser);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Failed to register user: {ex.Message}");
                throw new Exception($"Failed to register user: {ex.Message}");
            }
            //return null;
        }

        private async Task<string> GenerateEmployeeCodeAsync()
        {
            // Get the highest EmpCode from the database
            var lastEmpCode = await _userManager.Users
                .OrderByDescending(e => e.EmpCode)
                .Select(e => e.EmpCode)
                .FirstOrDefaultAsync();

            int lastNumber = 0;

            if (!string.IsNullOrEmpty(lastEmpCode))
            {
                // Extract the numeric part after "EM"
                if (lastEmpCode.Length >= 8 && int.TryParse(lastEmpCode.Substring(2), out int num))
                {
                    lastNumber = num;
                }
            }

            int newNumber = lastNumber + 1;

            // Format to maintain exactly 6 digits after 'EM'
            string newEmpCode = $"EM{newNumber.ToString("D6")}";

            // Double check to avoid duplicates (optional if you trust ordering)
            bool exists = await _userManager.Users.AnyAsync(e => e.EmpCode == newEmpCode);
            if (exists)
            {
                throw new Exception("Generated Employee Code already exists. Please try again.");
            }

            return newEmpCode;
        }

        private string GenerateUserName(string firstName, string lastName)
        {
            var cleanFirstName = firstName?.Trim().Replace(" ", "") ?? string.Empty;
            var cleanLastName = lastName?.Trim().Replace(" ", "") ?? string.Empty;

            var baseUserName =$"{cleanFirstName}{cleanLastName}".ToLower();
            //check if the username already exists
            var userName = baseUserName;
            var count = 1;
            while (_userManager.Users.Any(u => u.UserName == userName)) 
            {
                userName = $"{baseUserName}{count}";
                count++;
            }
            return userName;
        }
        public async Task<UserResponse> LoginAsync(UserLoginRequest loginRequest)
        {
            if (loginRequest == null) 
            {
                _logger.LogError("Logiin request is null");
                throw new ArgumentNullException(nameof(loginRequest));
            }
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                _logger.LogWarning("Invalid email or password");
                throw  new UnauthorizedAccessException("Invalid email or password");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Invalid email or password");
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var token =  await _tokenService.GenerateToken(user);
            var refreshToken =  _tokenService.GenerateRefreshToken();
            using var sha256 = SHA256.Create();
            var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
            user.RefreshToken = Convert.ToBase64String(refreshTokenHash);
            var jwtSetting = _configuration.GetSection("JwtSettings").Get<JwtSetting>();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddSeconds(jwtSetting.Expires);
           
           
            //user.RefreshTokenExpiryTime =  DateTime.Now.AddMinutes(2);
            //var exp = DateTime.Now.AddMinutes(2);
            //user.RefreshToken = refreshToken;
            var result =  await _userManager.UpdateAsync(user);
            if (!result.Succeeded) {
                var errors = string.Join(", ",result.Errors.Select(e => e.Description));
                 _logger.LogError($"Failed to update user:{errors}",errors);
                throw new Exception($"Failed to update user:{errors}");
            }
            var userResponse =  _mapper.Map<ApplicationUser,UserResponse>(user);
            userResponse.AccessToken = token;
            userResponse.RefreshToken = refreshToken;

            return userResponse;
        }
        public async Task<UserResponse> GetByIdAsync(long id)
        {
            _logger.LogInformation("Getting user by id");
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogError($"User not found");
                throw new Exception($"User not found");
            }
            _logger.LogInformation("User found");
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<CurrentUserResponse> GetCurrentUserAsync()
        {
            var user =   await _userManager.FindByIdAsync(_currentUserService.GetUserId().ToString());
            if(user == null)
            {
                _logger.LogError($"User not found.");
                throw new Exception("User not found");
            }
            return _mapper.Map<CurrentUserResponse>(user);
        }
        public async Task<CurrentUserResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            _logger.LogInformation($"Refresh Token");
            using var sha256  = SHA256.Create();
            var  refreshToeknHash =  sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshTokenRequest.RefreshToken));
            var hasehedRefreshToken = Convert.ToBase64String(refreshToeknHash);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == hasehedRefreshToken);
            if (user == null)
            {
                _logger.LogError($"Invalid refresh token");
                throw new Exception($"Invalid refresh token");
            }
            if (user.RefreshTokenExpiryTime <DateTime.UtcNow)
            {
                _logger.LogWarning("Refresh token expired for User ID:{UserId}",user.Id);
                throw new Exception($"Refresh token expired");
            }
            try
            {
                var newAccessToken = await _tokenService.GenerateToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                var newRefreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(newRefreshToken));
                user.RefreshToken = Convert.ToBase64String(newRefreshTokenHash);
                var jwtSetting = _configuration.GetSection("JwtSettings").Get<JwtSetting>();
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddSeconds(jwtSetting.Expires);
                //user.RefreshTokenExpiryTime = DateTime.UtcNow.AddSeconds(3600);
                //user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(2);
                //var exp = DateTime.Now.AddMinutes(2);
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError($"Failed to update user:{errors}", errors);
                    throw new Exception($"Failed to update user:{errors}");
                }

                _logger.LogInformation($"Access token generated successfully");
                //var currentUserResponse = _mapper.Map<CurrentUserResponse>(newAccessToken);
                var currentUserResponse = _mapper.Map<CurrentUserResponse>(user);
                currentUserResponse.AccessToken = newAccessToken;
                currentUserResponse.RefreshToken = newRefreshToken;
                //currentUserResponse.Id = user.Id;
                return currentUserResponse;
            }
            catch (Exception ex) {
                _logger.LogError("Failed to revoke refresh token:{ex}", ex.Message);
                throw new Exception("Failed to revoke refresh token");
            }
           

        }
        public async Task<RevokeRefreshTokenResponse> RevokeRefreshTokenAsync(RefreshTokenRequest refreshTokenRemoveRequest)
        {
            _logger.LogInformation("Revoking refresh token");
            try
            {
                using var sha256 = SHA256.Create();
                var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshTokenRemoveRequest.RefreshToken));
                var hashedRefreshToken = Convert.ToBase64String(refreshTokenHash);
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == hashedRefreshToken);
                if (user == null) 
                {
                    _logger.LogError("Invalid refresh token");
                    throw new Exception("Invalid refresh token");
                }
                if (user.RefreshTokenExpiryTime < DateTime.Now)
                {
                    _logger.LogWarning("Refresh toekn expired for user ID:{UserId}", user.Id);
                    throw new Exception("Refresh toekn expired");
                }
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null; 
                var result =  await _userManager.UpdateAsync(user);
                if (!result.Succeeded) 
                {
                    return new RevokeRefreshTokenResponse
                    {
                        Message = "Failed to revoke refresh token",
                    };
                    //var errors = string.Join(",",result.Errors.Select(u => u.Description));
                    //_logger.LogError("Failed to update user:{errors}", errors);
                    //throw new Exception($"Failed to update user:{errors}");
                }
                _logger.LogInformation("Refresh token revoked successfully");
                return new RevokeRefreshTokenResponse
                {
                    Message = "Refresh token revoked successfully"
                };
            }
            catch (Exception ex) 
            {
                _logger.LogError("Failed to revoke refresh token:{ex}",ex.Message);
                throw new Exception("Failed to revoke refresh token");
            }
        }
        public async Task<UserResponse> UpdateAsync(long id, UpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) 
            {
                _logger.LogError("User not found");
                throw new Exception("User not found");
            }
            user.FirstName =  request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            await _userManager.UpdateAsync(user);
            //user.UserName = GenerateUserName(request.FirstName, request.LastName);
            return _mapper.Map<UserResponse>(user);
        }

        public async Task DeleteAsync(long id)
        {
            var user  = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                _logger.LogError("User not found");
                throw new Exception("User not found");
            }
            await _userManager.DeleteAsync(user);
        }

        public async Task<List<UserResponse>> GetUserListAsync()
        {
            var userList = new List<UserResponse>();
            _logger.LogInformation("Get all user list");
            try
            {
                var users = await _userManager.Users.ToListAsync();
                if (userList == null || !users.Any())
                {
                    _logger.LogInformation("No user found");
                }
                else
                {
                    userList = users.Select(u => new UserResponse
                    {
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Gender = u.Gender,
                        EmpCode = u.EmpCode,
                        Email = u.Email,
                        IsActive = u.IsActive,
                        Id = u.Id,
                    }).ToList();
                }

            }
            catch (Exception exx)
            {
                _logger.LogError(exx, "Error while getting user list");
                throw new Exception("Error while getting user list");
            }
            return userList;
        }

        public async Task<List<UserResponse>> GetUserListByPlantDept(int plantId, int departmentId)
        {
            var userList = new List<UserResponse>();
            var user = await _userManager.Users.Where(u => u.PlantID == plantId && (departmentId == null || departmentId == 0 || u.DepartmentID == departmentId)).ToListAsync();
            if(user != null)
            {
                userList = user.Select(u => new UserResponse
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                }).ToList();
            }
            return userList;
        }

        //public Task<UserResponse> UpdateAsync(long id, UpdateUserRequest request)
        //{
        //    throw new NotImplementedException();
        //}

        //Task IUserService.GetByIdAsync(long id)
        //{
        //    return GetByIdAsync(id);
        //}
    }
}
