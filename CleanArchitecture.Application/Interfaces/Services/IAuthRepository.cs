using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface IAuthRepository
    {
        Task<IEnumerable<ApiUser>> GetAllApiUsersAsync();
        Task<ApiUser> GetApiUserByClientIdAsync(LoginDto user);
        //Task<string> GetClientSecretAsync(LoginDto user);
        Task<bool> ClientExistsAsync(string clientId);
        Task<ApiUser> UpdateClientSecretAsync(ApiUserUpdateDto user);
        Task<ApiUser> UpdateRefreshTokenAsync(ApiUserUpdateDto user);
        Task<bool> DeleteClientAsync(LoginDto user);
    }
}
