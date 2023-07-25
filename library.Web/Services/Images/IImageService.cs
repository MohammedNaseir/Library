using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace library.Core.Services.Images
{
    public interface IImageService
    {
        Task<(bool isUploaded, string? errorMessage)> UploadAsync(IFormFile image, string imageName, string folderPath, bool hasThumbnail);
		void Delete(string imagePath, string? imageThumbnailPath = null);
	}
}
