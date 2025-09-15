using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Models.SystemModels;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Repositories
{
	public class AccountRepository : IAccountRepository
    {
        private readonly CleanArchitectureContext dbContext;

        public AccountRepository(CleanArchitectureContext dbContext)
        {
            this.dbContext = dbContext;
        }
        private async Task<IEnumerable<ApiUser>> GetApiUsersByCompanyAsync(int? companyId)
        {
            if (companyId == null || companyId <= 0)
                throw new ArgumentException("Company is required");

            return await dbContext.ApiUsers.Include(r => r.Role).Include( c=> c.Company)
                .Where(u => u.CompanyId == companyId).ToListAsync();
        }
        private async Task<ApiUser> GetApiUserByCompanyAndClientIdAsync(int? companyId, string clientId = "")
        {
            if (String.IsNullOrEmpty(clientId))
                throw new ArgumentException("ClientId is required");

            var users = await GetApiUsersByCompanyAsync(companyId); 
            return users.FirstOrDefault(u => u.ClientId == clientId);
        }
        public async Task<IEnumerable<ApiUser>> GetAllUsersAsync(int companyId)
        {
            return await GetApiUsersByCompanyAsync(companyId);
        }
        public async Task<ApiUser> GetUserByClientIdAsync(string clientId)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            var user = await dbContext.ApiUsers.Include(r => r.Role).Include(c => c.Company).Include(s => s.ApiSubscription).Include(i => i.ApiWhitelistedIPs)
                .FirstOrDefaultAsync(u => u.ClientId == clientId) ?? null;

            return user;
		}
        public async Task<ApiUser> GetUserByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(userId));
            }
            return await dbContext.ApiUsers.FindAsync(userId);
        }

        public async Task<ApiUser> CreateUserAsync(ApiUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var existingUser = await dbContext.ApiUsers.FirstOrDefaultAsync(u => u.ClientId == user.ClientId && u.CompanyId == user.CompanyId);
            if (existingUser != null)
            {
                throw new InvalidOperationException($"User with ClientId {user.ClientId} already exists for Company {(user.Company?.CompanyName ?? "")}.");
            }
            dbContext.ApiUsers.Add(user);
            await dbContext.SaveChangesAsync();
            user = await GetUserByClientIdAsync(user.ClientId);
            return user;
        }
        public async Task<ApiUser> UpdateUserAsync(ApiUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var existingUser = await GetUserByClientIdAsync(user.ClientId);
            if (existingUser == null)
            {
                throw new KeyNotFoundException($"Invalid ClientId: {user.ClientId}.");
            }
            // Update properties
            //existingUser.ClientId = user.ClientId;
            //existingUser.ClientSecret = user.ClientSecret;
            //existingUser.Salt = user.Salt;
            //existingUser.ClientSecretHash = user.ClientSecretHash;
            existingUser.IsActive = user.IsActive;
            existingUser.UpdatedAt = DateTime.UtcNow;
            dbContext.ApiUsers.Update(existingUser);
            await dbContext.SaveChangesAsync();
            return existingUser;
        }
        public async Task DeleteUserAsync(string clientId)
        {
            var user = await GetUserByClientIdAsync(clientId);
            if (user == null)
            {
                throw new KeyNotFoundException($"Invalid ClientId: {clientId}.");
            }
            dbContext.ApiUsers.Remove(user);
            await dbContext.SaveChangesAsync();
        }
        public async Task<bool> UserExistsAsync(string clientId)
        {
            if (String.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            var user = await GetUserByClientIdAsync(clientId);
            if (user == null)
                return false;
            else
                return true;
        }
		public async Task<bool> SubscriptionAsync(string clientId)
		{
			if (String.IsNullOrEmpty(clientId))
			{
				throw new ArgumentNullException(nameof(clientId));
			}
			var subscription = await dbContext.ApiSubscription.Include(s => s.ApiUser).FirstOrDefaultAsync(u => u.ApiUser.ClientId == clientId);
			if (subscription == null)
			{
				throw new UnauthorizedAccessException($"Invalid Client Id");
			}
            else if (subscription.EndDate?.Date >= DateTime.UtcNow.Date)
            {
                return false;
            }
			return false;
		}

		public async Task<Company> GetCompanyByNameAsync(string cmpCode)
        {
            var cmp =  await dbContext.Companies.FirstOrDefaultAsync(c => c.CompanyCode.ToLower() == cmpCode.ToLower()
                || c.CompanyName.ToLower() == cmpCode.ToLower());
            if (cmp == null)
            {
                throw new UnauthorizedAccessException($"Invalid Company");
            }

            return cmp;
        }
        
        public async Task<Role> GetRoleByNameAsync(string role)
        {
            var cmp =  await dbContext.Roles.FirstOrDefaultAsync(c => c.RoleCode.ToLower() == role.ToLower()
                || c.RoleName.ToLower() == role.ToLower());
            if (cmp == null)
            {
                throw new UnauthorizedAccessException($"Invalid Role");
            }

            return cmp;
        }

	}
}
