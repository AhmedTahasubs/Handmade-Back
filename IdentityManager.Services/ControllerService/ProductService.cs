using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Models.DTOs.image;
using Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace IdentityManager.Services.ControllerService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepo;
        private readonly IImageRepository _imageRepo;
        private readonly IMapper mapper;

        public ProductService(IProductRepository productRepository, IImageRepository imageRepository, IMapper _mapper)
        {
            productRepo = productRepository;
            _imageRepo = imageRepository;
            mapper = _mapper;
        }
        private void ValidateFileUpload(IFormFile File)
        {
            if (File == null)
            {
                throw new Exception("File is required");
            }
            if (File.Length == 0)
            {
                throw new Exception("File is empty");
            }
            if (File.Length > 10 * 1024 * 1024)
            {
                throw new Exception("File is too large");
            }
            if (File.ContentType != "image/jpeg" && File.ContentType != "image/png")
            {
                throw new Exception("File is not an image");
            }
        }
        public async Task<int> UploadProductImageAsync(IFormFile File)
        {

            ValidateFileUpload(File);

            var image = new Image
            {
                File = File,
                FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                FileExtension = Path.GetExtension(File.FileName),
                FileSize = File.Length
            };

            await _imageRepo.Upload(image);

            return image.Id;
        }

        public async Task<IEnumerable<ProductDisplayDTO>> GetAllDisplayDTOs()
        {
            return mapper.Map<IEnumerable<ProductDisplayDTO>>(await productRepo.GetAllProducts());
        }

        public async Task<ProductDisplayDTO?> GetById(int id)
        {
            Product? p = await productRepo.GetProductByIdAsync(id);
            if (p == null)
                return null;
            return mapper.Map<ProductDisplayDTO>(p);

        }

        public async Task<ProductDisplayDTO> Create(ProductCreateDTO dto, string sellerId)
        {
            Product p = mapper.Map<Product>(dto);
            p.ImageId = await UploadProductImageAsync(dto.File);
            p.SellerId = sellerId;
            await productRepo.CreateProductAsync(p);
            await productRepo.SaveAsync();
            return mapper.Map<ProductDisplayDTO>(p);
        }

        public async Task<ProductDisplayDTO> Update(ProductUpdateDTO dto)
        {
            Product? existing = await productRepo.GetProductByIdAsync(dto.Id);
            if (existing == null)
            {
                return null;
            }
            mapper.Map(dto, existing);
            if(dto.File != null)
                existing.ImageId = await UploadProductImageAsync(dto.File);
            await productRepo.UpdateProductAsync(existing);
            await productRepo.SaveAsync();
            return mapper.Map<ProductDisplayDTO>(existing);
        }

        public async Task Delete(Product p)
        {
            await productRepo.DeleteProductAsync(p);
            await productRepo.SaveAsync();


        }

        public async Task<IEnumerable<ProductDisplayDTO>> GetAllProductsBySeriviceId(int seriviceId)
        {
            return mapper.Map<IEnumerable<ProductDisplayDTO>>(await productRepo.GetAllProductsBySeriviceId(seriviceId));
        }
        public async Task<IEnumerable<ProductDisplayDTO>> GetAllProductsBySellerId(string sellerId)
        {
            return mapper.Map<IEnumerable<ProductDisplayDTO>>(await productRepo.GetAllProductsBySellerId(sellerId));
        }
        public async Task<Product?> UpdateProductStatusAsync(int id, UpdateProductStatusDTO dto)
        {
            var prod = await productRepo.UpdateProductStatusAsync(id, dto.Status);
            await productRepo.SaveAsync();
            return prod;
        }
    }
}
