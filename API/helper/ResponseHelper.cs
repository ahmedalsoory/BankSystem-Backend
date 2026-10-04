using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace API.helper
{
    public static class ResponseHelper
    {
       public static bool IsSuccessResponse(ActionExecutedContext context)
        {
            // 1. If an unhandled C# exception occurred -> FAILURE
            if (context.Exception != null && !context.ExceptionHandled)
            {
                return false;
            }

            // 2. Check if the result is an ObjectResult (e.g. OkObjectResult, BadRequestObjectResult)
            if (context.Result is ObjectResult objectResult)
            {
                // Fail immediately if HTTP Status Code indicates error (400, 422, 500, etc.)
                if (objectResult.StatusCode.HasValue && objectResult.StatusCode.Value >= 400)
                {
                    return false;
                }

                // Pattern match against your base class (handles both OperationResult and OperationResult<T>)
                if (objectResult.Value is OperationResult operationResult)
                {
                    // Fails if Success is false OR if Errors contains items
                    if (!operationResult.Success || (operationResult.Errors != null && operationResult.Errors.Count > 0))
                    {
                        return false;
                    }
                }
            }
            else if (context.Result is StatusCodeResult statusCodeResult && statusCodeResult.StatusCode >= 400)
            {
                return false;
            }

            return true;
        }
    }
}