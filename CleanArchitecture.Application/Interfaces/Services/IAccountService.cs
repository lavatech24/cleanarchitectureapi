
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Models.SystemModels;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface IAccountService
    {
        Task<IEnumerable<ApiUserResponseDto>> GetAllUsersAsync(string company);
        Task<ApiUserResponseDto> GetUserByIdAsync(Guid userId);
        Task<ApiUserResponseDto> GetUserByClientIdAsync(string clientId);
        Task<ApiUserResponseDto> CreateUserAsync(string company);
        Task<ApiUserResponseDto> UpdateUserAsync(ApiUserUpdateDto user);
        Task DeleteUserAsync(string clientId);
        Task<bool> UserExistsAsync(string clientId);
        Task<Company> GetCompanyByNameAsync(string company);
        Task<Role> GetRoleByNameAsync(string role);
    }
}
