using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanArchitecture.Api.Filters.Attributes
{
    public class ApiResponseFilterAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if(context.Result is ObjectResult objectResult)
            {
                var response = new
                {
                    Status = objectResult.StatusCode,
                    Message = "Success", //objectResult.StatusCode.GetValueOrDefault(),
                    data = objectResult.Value
                };

                context.Result = new JsonResult(response)
                {
                    StatusCode = objectResult.StatusCode
                };
            }
            return base.OnActionExecutionAsync(context, next);
        }
    }
}
