using Microsoft.AspNetCore.Http;
using Shared.Image;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    /*
    internal class Class1
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
                    // **Advanced Security Check:** 
                    // Open a stream and attempt to fully decode the image pixels. 
                    // This stops attackers using fake extensions, hex spoofing, or corrupted data payloads.
                    using (var imageStream = file.OpenReadStream())
                    {
                        try
                        {

                            using var image = SixLabors.ImageSharp.Image.Load(imageStream);
                        }
                        catch
                        {
                            return OperationResult<string>.Failure("The uploaded file is not a valid, decodable image.");
                        }
                    }

                    var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", folderName);
                    Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    var savedPath = $"/images/{folderName}/{uniqueFileName}";
                    _imageTracker.TrackNewImage(savedPath);

                    return OperationResult<string>.Ok(savedPath);
                }
                catch (Exception ex)
                {
                    return OperationResult<string>.Failure($"An unexpected error occurred while saving the image: {ex.Message}");
                }
            }
        }
    }
    */
}
