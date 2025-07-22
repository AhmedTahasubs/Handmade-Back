using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Models.DTOs.image;
using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManager.Services.ControllerService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepo;
        private readonly IImageRepository _imageRepo;

        public ProductService(IProductRepository productRepository, IImageRepository imageRepository)
        {
            productRepo = productRepository;
            _imageRepo = imageRepository;
        }
        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            if (request.File == null)
            {
                throw new Exception("File is required");
            }
            if (request.File.Length == 0)
            {
                throw new Exception("File is empty");
            }
            if (request.File.Length > 10 * 1024 * 1024)
            {
                throw new Exception("File is too large");
            }
            if (request.File.ContentType != "image/jpeg" && request.File.ContentType != "image/png")
            {
                throw new Exception("File is not an image");
            }
        }
        public async Task<int> UploadProductImageAsync(ImageUploadRequestDto request)
        {

            ValidateFileUpload(request);

            var image = new Image
            {
                File = request.File,
                FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                FileExtension = Path.GetExtension(request.File.FileName),
                FileSize = request.File.Length
            };

            await _imageRepo.Upload(image);

            return image.Id;
        }
    }
}
