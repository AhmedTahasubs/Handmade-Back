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
        private readonly IProductRepository repo;

        public ProductController(IProductService _productService, IProductRepository _repo)
        {
            productService = _productService;
            repo = _repo;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return Ok(await productService.GetAllDisplayDTOs());
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await productService.GetById(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await repo.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            var isAdmin = User.IsInRole(AppRoles.Admin);
            if (!isAdmin && product.SellerId.ToString() != userId)
                return Forbid("You are not allowed to delete this product.");
            await productService.Delete(product);
            return NoContent();
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ProductCreateDTO productCreateDTO)
        {
            var productDTO = await productService.Create(productCreateDTO);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();
            return CreatedAtAction(nameof(GetById), new { id = productDTO.Id }, productDTO);
        }
            [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id,[FromForm] ProductUpdateDTO productUpdateDTO)
        {
            if (id != productUpdateDTO.Id)
                return BadRequest("ID mismatch between route and payload.");

            var existingProduct = await productService.GetById(id);
            if (existingProduct == null)
                return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole(AppRoles.Admin);

            if (userId == null)
                return Unauthorized();
            Console.WriteLine($"userId: {userId}");
            Console.WriteLine($"SellerId: {existingProduct.SellerId}");

            if (!isAdmin && userId != existingProduct.SellerId)
                return Forbid();

            ProductDisplayDTO productDisplayDTO = await productService.Update(productUpdateDTO);

            return CreatedAtAction("GetById", new { id = productDisplayDTO.Id}, productDisplayDTO);
        }
    }
}
