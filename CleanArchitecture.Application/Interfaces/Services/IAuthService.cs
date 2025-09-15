using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Models.SystemModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ApiUser> CreateUserObj(int RoleId, int? CompanyId, bool superuser = false);
        Task<TokenResponseDto> GetToken(LoginDto request);
        Task<TokenResponseDto> GenerateRefreshToken(ApiUserRefreshTokenDto request);
        Task<LoginDto> GenerateNewSecret(LoginDto request);
        Task<bool> ValidateClientCredentials(LoginDto request);   
        Task<bool> ValidateRefreshToken(ApiUserRefreshTokenDto request);
        Task<ApiUserResponseDto> GetApiUser(LoginDto request);   
        Task<IEnumerable<ApiUserResponseDto>> GetAllApiUsers();   
    }
}
