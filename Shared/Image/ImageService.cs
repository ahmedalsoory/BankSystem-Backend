using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.Image
{
    public interface IImageService
    {
        Task<OperationResult<string>> SaveImageAsync(IFormFile file, string folderName);
    }

    public class ImageService : IImageService
    {
        private readonly IImageTracker _imageTracker;
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxFileSize = 2 * 1024 * 1024; // 2MB

        public ImageService(IImageTracker imageTracker)
        {
            _imageTracker = imageTracker;
        }

        public async Task<OperationResult<string>> SaveImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                return OperationResult<string>.Failure("No file provided.");

            if (file.Length > MaxFileSize)
                return OperationResult<string>.Failure("File size exceeds the maximum allowed limit of 2MB.");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedExtensions.Contains(extension))
                return OperationResult<string>.Failure("Invalid file extension. Allowed types: .jpg, .jpeg, .png, .webp.");

            try
            {
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName);
                Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // SINGLE PASS: Validate pixels in RAM and write directly to destination in one stream pass
                using (var imageStream = file.OpenReadStream())
                {
                    using var image = await SixLabors.ImageSharp.Image.LoadAsync(imageStream);

                    // Saves the re-encoded image directly to disk (strips malicious EXIF metadata)
                    await image.SaveAsync(filePath);
                }

                var savedPath = $"/images/{folderName}/{uniqueFileName}";
                _imageTracker.TrackNewImage(savedPath);

                return OperationResult<string>.Ok(savedPath);
            }
            catch (UnknownImageFormatException)
            {
                // Thrown by ImageSharp when the file header or payload isn't a valid image
                return OperationResult<string>.Failure("The uploaded file is not a valid, decodable image.");
            }
            catch (InvalidImageContentException)
            {
                // Thrown by ImageSharp on corrupted or spoofed image streams
                return OperationResult<string>.Failure("The uploaded file contains corrupted or invalid image data.");
            }
            catch (Exception ex)
            {
                return OperationResult<string>.Failure($"An unexpected error occurred while saving the image: {ex.Message}");
            }
        }
    }
}