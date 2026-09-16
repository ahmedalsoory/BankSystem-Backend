using API.Validation;

using DTOs.Client;
using DTOs.interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContract;
using ServiceContract.Client;
using Shared.Interfaces;

namespace API
{
    /*
    public class GlobalValidationFilter<T, TContract> : IAsyncActionFilter
     where T : class, TContract
    {
        private readonly IValidationService<T, TContract> _validator; // Use the 2-parameter interface

        public GlobalValidationFilter(IValidationService<T, TContract> validator)
        {
            _validator = validator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var dto = context.ActionArguments.Values.OfType<T>().FirstOrDefault();

            if (dto != null)
            {

                var errors = await _validator.ValidateAsync(dto);
                if (errors.Any())
                {
                    context.Result = new BadRequestObjectResult(new { Errors = errors });
                    return;
                }
            }
            await next();
        }
    }
    */
}
