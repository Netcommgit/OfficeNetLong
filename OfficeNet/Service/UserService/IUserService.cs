using Microsoft.AspNetCore.Identity;
using OfficeNet.Domain.Contracts;

namespace OfficeNet.Service.UserService
{
    public interface IUserService
    {
        Task<UserResponse> RegisterAsync(UserRegisterRequest request);
        Task<CurrentUserResponse> GetCurrentUserAsync();
        Task<UserResponse> GetByIdAsync(long id);
        Task<UserResponse> UpdateAsync(long id, UpdateUserRequest request);
        Task DeleteAsync(long id);
        Task<RevokeRefreshTokenResponse> RevokeRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
        Task<CurrentUserResponse> RefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);

        Task<UserResponse> LoginAsync(UserLoginRequest loginRequest);

        Task<List<UserResponse>> GetUserListAsync();
        Task<List<UserResponse>> GetUserListByPlantDept(int plantId, int departmentId);
        //Task GetByIdAsync(long id);
    }   
}
