using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Models.SystemModels;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Security.Claims;

namespace CleanArchitecture.Api.Middleware
{
    public class IPWhitelistMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IPWhitelistMiddleware> _logger;

        public IPWhitelistMiddleware(RequestDelegate next, ILogger<IPWhitelistMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IIPWhitelistRepository iPWhitelistRepository)
        {
            if (context.User?.Identity?.IsAuthenticated ?? false)
            {
                Guid.TryParse(context.User.Claims?.FirstOrDefault(x => x.Type == "uid")?.Value, out Guid userId);
                if(userId == Guid.Empty)
				{
					_logger.LogWarning("Forbidden Request: Invalid User ID");
					context.Response.StatusCode = StatusCodes.Status403Forbidden;
					await context.Response.WriteAsync("Access denied: Invalid User ID.");
					return;
				}
				//var company = context.User.Claims?.FirstOrDefault(x => x.Type == "Company")?.Value ?? "";
                var remoteIp = context.Connection.RemoteIpAddress;
                // 🔍 Extract Company ID from route, header, or token
                //var companyIdHeader = context.Request.Headers["X-Company-ID"].FirstOrDefault();
                //var tenantIdHeader = context.Request.Headers["X-Tenant-ID"].FirstOrDefault();
                var allowedIPs = await iPWhitelistRepository.GetWhitelistedIPsAsync(userId);

                // Handle IPv4-mapped IPv6 addresses
                if (remoteIp.IsIPv4MappedToIPv6)
                {
                    remoteIp = remoteIp.MapToIPv4();
                }
                if (!allowedIPs.Any(ip => ip.IPAddress.Equals(remoteIp.ToString())))
                {
                    _logger.LogWarning("Forbidden Request from Remote IP address: {RemoteIp}", remoteIp);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync($"Access denied: IP Address: {remoteIp} not allowed.");
                    return;
                }
            }

            await _next(context);
        }
    }
}
