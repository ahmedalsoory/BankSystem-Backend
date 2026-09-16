using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Attributes
{
    public class ValidateByDelegateAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => true;

        // One single pointer to the validation logic. No Dictionary!
        public readonly Func<ActionExecutingContext, bool> Validator;

        public ValidateByDelegateAttribute(Func<ActionExecutingContext, bool> validator)
        {
            Validator = validator;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider) =>
            serviceProvider.GetRequiredService<DelegateValidationFilter>();
    }
    public class DelegateValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var attribute = context.ActionDescriptor.EndpointMetadata
                                   .OfType<ValidateByDelegateAttribute>()
                                   .FirstOrDefault();

            // Direct execution of the logic. No loops, no dictionary lookups.
            if (attribute?.Validator != null && !attribute.Validator(context))
            {
                context.Result = new BadRequestObjectResult("Validation failed.");
                return;
            }

            await next();
        }
    }


}
