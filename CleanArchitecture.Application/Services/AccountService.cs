using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Services
{
	public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepository;
        private readonly IMapper mapper;
        private readonly IAuthService authService;

        public AccountService(IAccountRepository accountRepository, IMapper mapper, IAuthService authService)
        {
            this.accountRepository = accountRepository;
            this.mapper = mapper;
            this.authService = authService;
        }

        public async Task<IEnumerable<ApiUserResponseDto>> GetAllUsersAsync(string company)
        {
            var cmp = await accountRepository.GetCompanyByNameAsync(company);
            return mapper.Map<IEnumerable<ApiUserResponseDto>>(await accountRepository.GetAllUsersAsync(cmp.Id));
        }
        public async Task<ApiUserResponseDto> GetUserByIdAsync(Guid userId)
        {
            return mapper.Map<ApiUserResponseDto>(await accountRepository.GetUserByIdAsync(userId));
        }
        public async Task<ApiUserResponseDto> GetUserByClientIdAsync(string clientId)
        {
            return mapper.Map<ApiUserResponseDto>(await accountRepository.GetUserByClientIdAsync(clientId));
        }
        public async Task<ApiUserResponseDto> CreateUserAsync(string company)
        {
            var cmp = await accountRepository.GetCompanyByNameAsync(company);
            var role = await accountRepository.GetRoleByNameAsync("user");
            var newUser = await authService.CreateUserObj(RoleId: role.Id, CompanyId: cmp.Id);
            //var userEntity = mapper.Map<ApiUser>(user);
            var user = mapper.Map<ApiUserResponseDto>(await accountRepository.CreateUserAsync(newUser));
            user.ClientSecret = newUser.ClientSecret;

            return user;
        }
        public async Task<ApiUserResponseDto> UpdateUserAsync(ApiUserUpdateDto user)
        {
            var userEntity = mapper.Map<ApiUser>(user);
            return mapper.Map<ApiUserResponseDto>(await accountRepository.UpdateUserAsync(userEntity));
        }
        public async Task DeleteUserAsync(string clientId) 
        { 
            await accountRepository.DeleteUserAsync(clientId);
        }
        public async Task<bool> UserExistsAsync(string clientId)
        {
            return await accountRepository.UserExistsAsync(clientId);
        }
        public async Task<Company> GetCompanyByNameAsync(string company)
        {
            if (string.IsNullOrEmpty(company))
            {
                throw new ArgumentNullException(nameof(company), "Invalid company");
            }
            return await accountRepository.GetCompanyByNameAsync(company);
        }
        public async Task<Role> GetRoleByNameAsync(string role)
        {
            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException(nameof(role), "Invalid role");
            }
            return await accountRepository.GetRoleByNameAsync(role);
        }
	
	}
}
