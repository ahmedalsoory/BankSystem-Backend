using API.helper;
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
            // 1. Execute the controller action
            var resultContext = await next();

            // 2. Check if the operation was successful using ResponseHelper
            if (ResponseHelper.IsSuccessResponse(resultContext))
            {
                // 🎉 SUCCESS: Safely delete old replaced images from disk
                try
                {
                    _imageHandler.CleanupOldImages(_env.WebRootPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while cleaning up old replaced images.");
                }
            }
            else
            {
                // ⚠️ FAILURE: Delete newly uploaded image so it doesn't become orphaned on disk
                try
                {
                    _logger.LogWarning("Action failed or returned failure status. Cleaning up newly uploaded images.");
                    _imageHandler.CleanupNewImages(_env.WebRootPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while cleaning up new uploaded images.");
                }
            }
        }
    }
}