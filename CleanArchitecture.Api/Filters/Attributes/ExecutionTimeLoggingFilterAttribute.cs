using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace CleanArchitecture.Api.Filters.Attributes
{
    public class ExecutionTimeLoggingFilterAttribute : ActionFilterAttribute
    {
        private Stopwatch? stopwatch;
        private ILogger<ExecutionTimeLoggingFilterAttribute>? logger;
        public ExecutionTimeLoggingFilterAttribute(ILogger<ExecutionTimeLoggingFilterAttribute> logger)
        {
            this.logger = logger;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            stopwatch = Stopwatch.StartNew();
            await next();
            stopwatch.Stop();
            logger?.LogInformation("Action {ActionName} completed in {ElapsedMilliseconds} ms", context.ActionDescriptor.DisplayName, stopwatch.ElapsedMilliseconds);
        }
    }
}
