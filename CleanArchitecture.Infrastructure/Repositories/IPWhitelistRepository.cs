using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Models.SystemModels;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class CachedIPWhitelistRepository : IIPWhitelistRepository
    {
        private readonly IMemoryCache cache;
        private readonly CleanArchitectureContext dbContext;
        private readonly IConfiguration config;
        private int? tenantId;

        public CachedIPWhitelistRepository(IMemoryCache cache, CleanArchitectureContext dbContext, IConfiguration config)
        {
            this.cache = cache;
            this.dbContext = dbContext;
            this.config = config;
        }
        private async Task<IEnumerable<ApiWhitelistedIP>> GetIPsByIdAsync(Guid userId)
        {
            //if (companyId == null || companyId <= 0)
            //    throw new ArgumentException("Company/Tenant is required");

            return await dbContext.ApiWhitelistedIPs.Where(w => w.ApiUserId == userId && w.IsActive).ToListAsync();
        }
        public async Task<IEnumerable<ApiWhitelistedIP>> GetWhitelistedIPsAsync(Guid userId)
        {
			cache.Remove($"WhitelistedIPs_Cmp_{userId}");
            return await cache.GetOrCreateAsync($"WhitelistedIPs_{userId}", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(Convert.ToInt32(config["ApiSettings:IPWhitelistRefreshMinutes"]));
                return await GetIPsByIdAsync(userId);
            });
        }

        public async Task<bool> AddWhitelistedAsync(Guid userId, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }
            if (await IsWhitelistedAsync(userId, ipAddress))
            {
                throw new InvalidOperationException($"IP address {ipAddress} is already whitelisted.");
            }
            var newIp = new ApiWhitelistedIP
            {
                ApiUserId = userId,
                IPAddress = ipAddress,
            };
            dbContext.ApiWhitelistedIPs.Add(newIp);
            await dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<bool> IsWhitelistedAsync(Guid userId, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }
            return await dbContext.ApiWhitelistedIPs.AnyAsync(ip => ip.ApiUserId == userId && ip.IPAddress == ipAddress && ip.IsActive);
        }

        public async Task<bool> DeleteWhitelistedAsync(Guid userId, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }
            var ipToRemove = await dbContext.ApiWhitelistedIPs.FirstOrDefaultAsync(ip => ip.ApiUserId == userId && ip.IPAddress == ipAddress);
            if (ipToRemove == null)
            {
                throw new InvalidOperationException($"IP address {ipAddress} is not whitelisted.");
            }
            dbContext.ApiWhitelistedIPs.Remove(ipToRemove);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
