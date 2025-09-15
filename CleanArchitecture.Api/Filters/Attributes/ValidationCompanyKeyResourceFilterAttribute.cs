using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanArchitecture.Api.Filters.Attributes
{
    public class ValidationCompanyKeyResourceFilterAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("X-Company-ID", out var companyId) || string.IsNullOrEmpty(companyId))
            {
                context.Result = new Microsoft.AspNetCore.Mvc.BadRequestObjectResult("Company ID header is missing or empty.");
            }
        }
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
        }
    }
}
