using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Image
{
    public interface IImageTracker
    {
        void TrackNewImage(string imagePath);
        void TrackOldImageForDeletion(string imagePath);
        List<string> GetNewImages();
        List<string> GetOldImagesForDeletion();
        void Clear();
    }

    public class ImageTracker : IImageTracker
    {
        private readonly List<string> _newImages = new();
        private readonly List<string> _oldImagesForDeletion = new();

        public void TrackNewImage(string imagePath) => _newImages.Add(imagePath);
        public void TrackOldImageForDeletion(string imagePath) => _oldImagesForDeletion.Add(imagePath);

        public List<string> GetNewImages() => _newImages;
        public List<string> GetOldImagesForDeletion() => _oldImagesForDeletion;

        public void Clear()
        {
            _newImages.Clear();
            _oldImagesForDeletion.Clear();
        }
    }
}
