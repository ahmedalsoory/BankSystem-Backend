using Microsoft.AspNetCore.Http;
using Shared;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Shared.Image
{

    public interface IImageHandler
    {
        Task<OperationResult<string>> ProcessAsync(IFormFile? file, string? existingPath);
        void CleanupNewImages(string webRootPath);
        void CleanupOldImages(string webRootPath);
    }


    public class ImageHandler : IImageHandler
    {
        private readonly IImageService _imageService;
        private readonly IImageTracker _imageTracker;
        private readonly Context _context;
        public ImageHandler(IImageService imageService, IImageTracker imageTracker, Context context)
        {
            _imageService = imageService;
            _imageTracker = imageTracker;
            _context = context; 
        }

        public async Task<OperationResult<string>> ProcessAsync(IFormFile? file, string? existingPath)
        {
            if (file == null || file.Length == 0)
                return OperationResult<string>.Ok(existingPath ?? string.Empty);


            var result = await _imageService.SaveImageAsync(file, _context?.ImageFolderName??"");
            if (!result.Success) return OperationResult<string>.Failure(result.Errors);

            if (!string.IsNullOrEmpty(existingPath))
                _imageTracker.TrackOldImageForDeletion(existingPath);

            return OperationResult<string>.Ok(result.Data);
        }

        public void CleanupNewImages(string webRootPath)
        {
            var trackedImages = _imageTracker.GetNewImages();
            foreach (var imagePath in trackedImages)
            {
                try
                {
                    var fullPath = Path.Combine(webRootPath, imagePath.TrimStart('/'));
                    if (File.Exists(fullPath)) File.Delete(fullPath);
                }
                catch { }
            }
            _imageTracker.Clear();
        }

        public void CleanupOldImages(string webRootPath)
        {
            var oldImages = _imageTracker.GetOldImagesForDeletion();
            foreach (var imagePath in oldImages)
            {
                try
                {
                    var fullPath = Path.Combine(webRootPath, imagePath.TrimStart('/'));
                    if (File.Exists(fullPath)) File.Delete(fullPath);
                }
                catch { }
            }
            _imageTracker.Clear();
        }

    
    }
}