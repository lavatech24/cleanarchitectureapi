using CleanArchitecture.Domain.Models.SystemModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface IIPWhitelistRepository
    {
        /// <summary>
        /// Gets all whitelisted IP addresses.
        /// </summary>
        /// <returns>A list of whitelisted IP addresses.</returns>
        Task<IEnumerable<ApiWhitelistedIP>> GetWhitelistedIPsAsync(Guid userId);
        /// <summary>
        /// Checks if the given IP address is whitelisted.
        /// </summary>
        /// <param name="ipAddress">The IP address to check.</param>
        /// <returns>True if the IP address is whitelisted, otherwise false.</returns>
        Task<bool> IsWhitelistedAsync(Guid userId, string ipAddress);
        /// <summary>
        /// Adds an IP address to the whitelist.
        /// </summary>
        /// <param name="ipAddress">The IP address to add.</param>
        Task<bool> AddWhitelistedAsync(Guid userId, string ipAddress);
        /// <summary>
        /// Removes an IP address from the whitelist.
        /// </summary>
        /// <param name="ipAddress">The IP address to remove.</param>
        Task<bool> DeleteWhitelistedAsync(Guid userId, string ipAddress);
    }
}
