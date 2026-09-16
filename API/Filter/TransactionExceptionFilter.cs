using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Image;
using Shared.Interfaces;

namespace API.Filter
{
    public class TransactionExceptionFilter : IExceptionFilter
    {
        private readonly IDbContextScope _dbContextScope;
        private readonly IImageHandler _imageHandler;
        private readonly IWebHostEnvironment _env;

        public TransactionExceptionFilter(
            IDbContextScope dbContextScope,
            IImageHandler imageHandler,
            IWebHostEnvironment env)
        {
            _dbContextScope = dbContextScope;
            _imageHandler = imageHandler;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            // 1. Rollback the database transaction
            _dbContextScope.Rollback();

            // 2. Cleanup newly uploaded images that failed during this request
            _imageHandler.CleanupNewImages(_env.WebRootPath);

            context.ExceptionHandled = false;
        }
    }
}