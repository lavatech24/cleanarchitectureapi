using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Models.SystemModels;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly CleanArchitectureContext dbContext;

        public AuthRepository(CleanArchitectureContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<bool> ClientExistsAsync(string clientId)
        {
            if (String.IsNullOrEmpty(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }
            return await dbContext.ApiUsers.AnyAsync(u => u.ClientId == clientId);
        }

        public async Task<bool> DeleteClientAsync(LoginDto request)
        {
            if (await ClientExistsAsync(request.ClientId))
            {
                var user = await dbContext.ApiUsers.FirstOrDefaultAsync(u => u.ClientId == request.ClientId);
                if (user != null)
                {
                    dbContext.ApiUsers.Remove(user);
                    await dbContext.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        /*
        public async Task<string> GetClientSecretAsync(LoginDto request)
        {
            if (await ClientExistsAsync(request.ClientId))
            {
                var user = await dbContext.ApiUsers.FirstOrDefaultAsync(u => u.ClientId == request.ClientId);
                if (user != null)
                {
                    return user.ClientSecret;
                }
            }
            throw new UnauthorizedAccessException($"Invalid Credentials");
        }
        */

        public async Task<ApiUser> UpdateClientSecretAsync(ApiUserUpdateDto request)
        {
            if (await ClientExistsAsync(request.ClientId))
            {
                var user = await dbContext.ApiUsers.FirstOrDefaultAsync(u => u.ClientId == request.ClientId);
                if (user != null)
                {
                    //user.ClientSecret = request.ClientSecret;
                    user.Salt = request.Salt;
                    user.ClientSecretHash = request.ClientSecretHash;
                    user.UpdatedAt = DateTime.UtcNow;
                    dbContext.ApiUsers.Update(user);
                    await dbContext.SaveChangesAsync();
                    return user;
                }
                throw new KeyNotFoundException($"Client with ID {request.ClientId} not found.");
            }
            throw new ArgumentException($"Client with ID {request.ClientId} does not exist.", nameof(request.ClientId));
        }

        public async Task<ApiUser> UpdateRefreshTokenAsync(ApiUserUpdateDto user)
        {
            if (user == null || String.IsNullOrEmpty(user.ClientId) || String.IsNullOrEmpty(user.RefreshToken))
            {
                throw new ArgumentNullException("ClientId and refresh token cannot be null or empty.");
            }
            var apiUser = await dbContext.ApiUsers.FirstOrDefaultAsync(u => u.ClientId == user.ClientId);
            if (apiUser == null)
            {
                throw new KeyNotFoundException($"Client with ID {user.ClientId} not found.");
            }
            apiUser.RefreshToken = user.RefreshToken;
            apiUser.RefreshTokenExpiry = user.RefreshTokenExpiry;
            dbContext.ApiUsers.Update(apiUser);
            await dbContext.SaveChangesAsync();
            return apiUser;
        }

        public async Task<ApiUser> GetApiUserByClientIdAsync(LoginDto request)
        {
            if (String.IsNullOrEmpty(request.ClientId))
            {
                throw new ArgumentNullException(nameof(request.ClientId));
            }
            var user = await dbContext.ApiUsers.Include(r => r.Role).Include(c => c.Company).FirstOrDefaultAsync(u => u.ClientId == request.ClientId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid client credentials.");
            }
            return user;
        }
        public async Task<IEnumerable<ApiUser>> GetAllApiUsersAsync()
        {
            return await dbContext.ApiUsers.Include(r => r.Role).Include(c => c.Company).ToListAsync();
        }
    }
}
