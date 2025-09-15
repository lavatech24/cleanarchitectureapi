using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace CleanArchitecture.Api.Filters
{
    public class RequestResponseLoggingFilter : IActionFilter
    {
        private readonly ILogger<RequestResponseLoggingFilter> logger;

        public RequestResponseLoggingFilter(ILogger<RequestResponseLoggingFilter> logger)
        {
            this.logger = logger;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var httpContext = context.HttpContext;
            var response = httpContext.Response;
            logger.LogInformation($"Response Code: {response.StatusCode}");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var request = httpContext.Request;
            var query = JsonSerializer.Serialize(request.QueryString.HasValue ? request.QueryString.Value : string.Empty);
            var routeValues = JsonSerializer.Serialize(request.RouteValues);
            var headers = JsonSerializer.Serialize(request.Headers);
            logger.LogInformation($"Request: {request.Method} {request.Path} {query}");
            
        }
    }
}
