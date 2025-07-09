using Microsoft.AspNetCore.Mvc;
using OfficeNet.Infrastructure.Mapping;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace OfficeNet.Domain.Contracts
{
    public class UserRegisterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string? MobileNum { get; set; }
        //[JsonConverter(typeof(IsoDateOnlyConverter))]
        [Column(TypeName = "date")]
        public DateOnly DOB { get; set; }
        
    }


    public class UserResponse
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string EmpCode { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool? IsActive { get; set; }


    }

    public class UserLoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class CurrentUserResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

    }


    public class UpdateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
    }


    public class RevokeRefreshTokenResponse
    {
        public string Token {  get; set; }
        public string Message { get; set; }
    }


    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    
}
