using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Shared.Image;

namespace API.Filter
{
    public class ImageCommitActionFilter : IAsyncActionFilter
    {
        private readonly IImageHandler _imageHandler;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ImageCommitActionFilter> _logger;

        public ImageCommitActionFilter(
            IImageHandler imageHandler,
            IWebHostEnvironment env,
            ILogger<ImageCommitActionFilter> logger)
        {
            _imageHandler = imageHandler;
            _env = env;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // 1. Execute the action and underlying services/database transaction
            var resultContext = await next();

            // 2. If the action itself failed, do nothing (ExceptionFilter handles the rollback)
            if (resultContext.Exception != null)
            {
                return;
            }

            // 3. 🎉 SUCCESS! Database has committed. Safely delegate old image cleanup to ImageHandler.
            try
            {
                _imageHandler.CleanupOldImages(_env.WebRootPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while cleaning up old replaced images.");
            }
        }
    }
}