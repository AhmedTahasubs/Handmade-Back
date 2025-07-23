using AutoMapper;
using DataAcess.Repos;
using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Models.Const;
using Models.Domain;
using Models.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly IProductService productService;
        private readonly IMapper mapper;
        private readonly IProductRepository ProductRepo;

        public ProductController(IMapper _mapper, IProductRepository _productRepository, IProductService _productService)
        {
            mapper = _mapper;
            ProductRepo = _productRepository;
            productService = _productService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await ProductRepo.GetAllProducts();
            return Ok(mapper.Map<List<ProductDisplayDTO>>(products));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await ProductRepo.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(mapper.Map<ProductDisplayDTO>(product));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await ProductRepo.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            var isAdmin = User.IsInRole(AppRoles.Admin);
            if (!isAdmin && product.SellerId != userId)
                return Forbid();
            await ProductRepo.DeleteProductAsync(product);
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductCreateDTO productCreateDTO)
        {
            var product = mapper.Map<Product>(productCreateDTO);
            var imageId = await productService.UploadProductImageAsync(productCreateDTO.ImageUploadRequest);
            product.ImageId = imageId;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();
            product.SellerId = userId;
            product.Status = ProductStatus.Pending;
            await ProductRepo.CreateProductAsync(product);
            var displayDto = mapper.Map<ProductDisplayDTO>(product);
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, displayDto);
        }
            [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id,[FromBody] ProductUpdateDTO productUpdateDTO)
        {
            if (id != productUpdateDTO.Id)
                return BadRequest("ID mismatch between route and payload.");

            var existingProduct = await ProductRepo.GetProductByIdAsync(id);
            if (existingProduct == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole(AppRoles.Admin);

            if (userId == null)
                return Unauthorized();

            if (!isAdmin && userId != existingProduct.SellerId)
                return Forbid();

            // Map updates onto the existing product entity
            mapper.Map(productUpdateDTO, existingProduct);

            // Optional image update
            if (productUpdateDTO.ImageUploadRequest != null)
            {
                var imageId = await productService.UploadProductImageAsync(productUpdateDTO.ImageUploadRequest);
                existingProduct.ImageId = imageId;
            }

            existingProduct.SellerId = userId;

            await ProductRepo.UpdateProductAsync(existingProduct);

            return NoContent();
        }
    }
}
