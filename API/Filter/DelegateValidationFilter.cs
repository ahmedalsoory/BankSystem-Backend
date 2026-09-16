using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Shared.Interfaces;

public class DelegateValidationFilter : IAsyncActionFilter
{
    private readonly Func<object, Task<bool>> _validationLogic;

    public DelegateValidationFilter(Func<object, Task<bool>> validationLogic)
    {
        _validationLogic = validationLogic;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var argument = context.ActionArguments.Values.FirstOrDefault();

        if (argument != null)
        {
            // Reuses the warm connection from GlobalConnectionFilter
            bool isValid = await _validationLogic(argument);

            if (!isValid)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    error = "Validation Failed",
                    message = "The provided data is invalid or conflicts with existing records."
                });
                return;
            }
        }

        await next();
    }
}