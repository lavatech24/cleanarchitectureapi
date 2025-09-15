using CleanArchitecture.Domain.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface IAccountRepository
    {
        Task<IEnumerable<ApiUser>> GetAllUsersAsync(int companyId);
        Task<ApiUser> GetUserByIdAsync(Guid userId);
        Task<ApiUser> GetUserByClientIdAsync(string clientId);
        Task<ApiUser> CreateUserAsync(ApiUser user);
        Task<ApiUser> UpdateUserAsync(ApiUser user);
        Task DeleteUserAsync(string clientId);
        Task<bool> UserExistsAsync(string clientId);
        Task<Company> GetCompanyByNameAsync(string cmpCode);
        Task<Role> GetRoleByNameAsync(string role);
    }
}
