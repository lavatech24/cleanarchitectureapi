using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanArchitecture.Api.Filters.Attributes
{
    public class ValidationTenantKeyResourceFilterAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("X-Tenant-ID", out var tenantId) || string.IsNullOrEmpty(tenantId))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult("Tenant ID header is missing or empty.");
            }
        }
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
