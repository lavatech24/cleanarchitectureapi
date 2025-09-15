using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Api.Filters
{
    public class ValidationFilter: IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments)
            {
                if (argument.Value != null)
                {
                    var validatorType = typeof(IValidator<>).MakeGenericType(argument.Value.GetType());
                    var validator = context.HttpContext.RequestServices.GetService(validatorType) as IValidator;
                    if (validator != null)
                    {
                        var validationResult = await validator.ValidateAsync(new ValidationContext<object>(argument.Value));
                        if (!validationResult.IsValid)
                        {
                            //foreach (var error in validationResult.Errors)
                            //{
                            //    context.ModelState.AddModelError(argument.Key, error.ErrorMessage);
                            //}
                            //context.Result = new BadRequestObjectResult(context.ModelState);
                            var errors = validationResult.Errors.Select(e => new{ e.PropertyName, e.ErrorMessage}).ToList();
                            context.Result = new BadRequestObjectResult(errors);
                            return;
                        }
                    }
                }
            }

            await next();
        }
    }
}
