using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTOs
{
    public class ApiUserDto
    {
        public Guid Id { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Salt { get; set; }
        public string ClientSecretHash { get; set; }
        public bool IsActive { get; set; }
        public bool IsSuperuser { get; set; }
        public int? RoleId { get; set; }

        public int? CompanyId { get; set; }
        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        // Additional properties can be added as needed
    }
    
    public class ApiUserCreateDto
    {
        public required string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Salt { get; set; }
        public string ClientSecretHash { get; set; }
        public bool IsActive { get; set; }
        public int? CompanyId { get; set; }
        public int? RoleId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; }

    }
    public class ApiUserUpdateDto
    {
        public required string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Salt { get; set; }
        public string ClientSecretHash { get; set; }
        public bool IsActive { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class ApiUserDeleteDto
    {
        public required Guid Id { get; set; }
    }
    public class ApiUserStatusDto
    {
        public required Guid Id { get; set; }
        public required bool IsActive { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class ApiUserResponseDto
    {
        public required Guid Id { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public required bool IsActive { get; set; }
        public string? Role { get; set; }
        public string? Company { get; set; }
        //public DateTime? RefreshTokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class ApiUserRefreshTokenDto
    {
        public required string ClientId { get; set; }
        public required string RefreshToken { get; set; }
        //public DateTime? RefreshTokenExpiry { get; set; }
    }
    public class ApiUserListDto
    {
        public IEnumerable<ApiUserResponseDto> Users { get; set; }
    }
    public class ApiUserDetailDto
    {
        public ApiUserResponseDto User { get; set; }
    }
}
