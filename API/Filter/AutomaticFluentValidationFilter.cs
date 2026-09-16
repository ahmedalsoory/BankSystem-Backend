using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using DTOs.interfaces;

namespace API.Filter
{
    public class AutomaticFluentValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public AutomaticFluentValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Grab the request argument if it implements our marker interface
            var dto = context.ActionArguments.Values.OfType<IValidatableDto>().FirstOrDefault();

            if (dto != null)
            {
                var dtoType = dto.GetType();
                var validatorType = typeof(IValidator<>).MakeGenericType(dtoType);
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(dto);
                    var result = await validator.ValidateAsync(validationContext);

                    if (!result.IsValid)
                    {
                        var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
                        context.Result = new BadRequestObjectResult(errors);
                        return; // 🛑 Short-circuit instantly!
                    }
                }
            }

            await next();
        }
    }
}
