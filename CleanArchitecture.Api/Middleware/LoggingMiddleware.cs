using CleanArchitecture.Infrastructure.Data;
using System;

namespace CleanArchitecture.Api.Middleware
{
	public class AuditLogMiddleware
	{
		private readonly RequestDelegate _next;

		public AuditLogMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context, CleanArchitectureContext dbContext)
		{
			await _next(context);

			var endpoint = context.GetEndpoint();
			var routePattern = endpoint?.Metadata.GetMetadata<Microsoft.AspNetCore.Routing.RouteNameMetadata>()?.RouteName;

			var controller = context.Request.RouteValues["controller"]?.ToString();
			var action = context.Request.RouteValues["action"]?.ToString();
			var clientId = context.Request.Headers["ClientID"].FirstOrDefault();

			if (controller != null && action != null)
			{
				//dbContext.AuditLogs.Add(new AuditLog
				//{
				//	ClientId = clientId,
				//	ControllerName = controller,
				//	ActionName = action,
				//	Timestamp = DateTime.UtcNow
				//});
				await dbContext.SaveChangesAsync();
			}
		}
	}
}
