using CleanArchitecture.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Api.Filters
{
    public class RateLimitResourceFilter: IAsyncResourceFilter
    {
        private readonly IMemoryCache cache;
		private readonly CleanArchitectureContext dbContext;

		//private readonly IConfiguration config;
		private readonly int maxRequestPerMinute;

        public RateLimitResourceFilter(IMemoryCache cache, IConfiguration config, CleanArchitectureContext dbContext)
        {
            this.cache = cache;
			this.dbContext = dbContext;
			//this.config = config;
			maxRequestPerMinute = Convert.ToInt32(config["ApiSettings:MaxRequestsPerMinute"] ?? "5");
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
			if (remoteIp?.IsIPv4MappedToIPv6 ?? false)
				remoteIp = remoteIp.MapToIPv4();
			
            var cacheKey = $"RateLimit_{remoteIp?.ToString() ?? "unknown"}";
            if (!cache.TryGetValue(cacheKey, out int requestCount))
            {
                // If the cache entry does not exist, create it with a sliding expiration
                requestCount = 0;
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                cache.Set(cacheKey, requestCount, cacheEntryOptions);
            }
            requestCount++;
            if (requestCount > maxRequestPerMinute)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.StatusCodeResult(429); // Too Many Requests
                //Write database log when the request threshold limt reached 
                return;
            }
            // Update the request count in the cache
            cache.Set(cacheKey, requestCount);

            await next();
        }
    }  
}
