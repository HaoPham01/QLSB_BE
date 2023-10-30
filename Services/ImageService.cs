using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace QLSB_APIs.Services
{
    public interface IImageService
    {
        ImageUploadResult UploadPhotoAsync(IFormFile photo);
        ImageUploadResult UploadPhotoNews(IFormFile photo);
        DeletionResult DeletePhotoAsync(string publicId);
    }
    
    public class ImageService : IImageService
    {
        private readonly Cloudinary cloudinary;
        public ImageService(IConfiguration config)
        {
            Account account = new Account(
               config.GetSection("CloudinarySettings:CloudName").Value,
               config.GetSection("CloudinarySettings:ApiKey").Value,
               config.GetSection("CloudinarySettings:ApiSecret").Value
                );
            cloudinary = new Cloudinary(account);
        }
        public ImageUploadResult UploadPhotoAsync(IFormFile photo)
        {
            var uploadResult = new ImageUploadResult();
            if(photo.Length > 0)
            {
                using var stream = photo.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(photo.FileName, stream),
                    Transformation = new Transformation()
                    .Height(500).Width(800)
                };
                uploadResult = cloudinary.Upload(uploadParams);
                
            }
            return uploadResult;
        }

        public ImageUploadResult UploadPhotoNews(IFormFile photo)
        {
            var uploadResult = new ImageUploadResult();
            if (photo.Length > 0)
            {
                using var stream = photo.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(photo.FileName, stream),
                    Transformation = new Transformation()
                    .Height(500).Width(1000)
                };
                uploadResult = cloudinary.Upload(uploadParams);

            }
            return uploadResult;
        }

        public DeletionResult DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = cloudinary.Destroy(deleteParams);
            return result;

        }
    }
}
