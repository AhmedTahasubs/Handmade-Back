using AutoMapper;
using DataAcess.Repos;
using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Models.Domain;
using Models.DTOs;
using Models.DTOs.image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IdentityManager.Services.ControllerService
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository productRepo;
        private readonly IImageRepository _imageRepo;
        private readonly IServiceRepository _serviceRepository;
        private readonly IMapper mapper;
        private readonly string _cohereApiKey;
        public ProductService(
     IConfiguration config,
     IServiceRepository serviceRepository,
     IProductRepository productRepository,
     IImageRepository imageRepository,
     IMapper _mapper)
        {
            productRepo = productRepository;
            _imageRepo = imageRepository;
            _serviceRepository = serviceRepository;
            mapper = _mapper;

            _cohereApiKey = config["OpenAI:ApiKey"];
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

        public async Task<bool> ValidateProductMatchesServiceAsync(string productDescription, int serviceId)
        {
            var service = _serviceRepository.Getbyid(serviceId);
            if (service == null) return false;

            string serviceDescription = service.Description;
            string prompt = $"I have a service described as: \"{serviceDescription}\". " +
                            $"I want to add a product with the following description: \"{productDescription}\". " +
                            $"Does this product clearly match the purpose of the service? Answer with only one word: Yes or No.";

            using var http = new HttpClient();
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _cohereApiKey);

            var requestBody = new
            {
                model = "command",
                prompt = prompt,
                max_tokens = 10,
                temperature = 0.2
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await http.PostAsync("https://api.cohere.ai/v1/generate", jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Cohere request failed. Status: {response.StatusCode}, Details: {error}");
            }

            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonDocument.Parse(responseString);

            string reply = result.RootElement
                .GetProperty("generations")[0]
                .GetProperty("text")
                .GetString()
                .Trim()
                .ToLower();

            return reply.StartsWith("yes");
        }


    }
}
