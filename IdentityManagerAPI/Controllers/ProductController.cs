using AutoMapper;
using DataAcess.Repos;
using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Models.Const;
using Models.Domain;
using Models.DTOs;
using Models.DTOs.Product;
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
        private readonly ISearchService searchService;

        public ProductController(
            IProductService _productService, 
            IProductRepository _repo,
            ISearchService _searchService)
        {
            productService = _productService;
            repo = _repo;
            searchService = _searchService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var isAdmin = User.IsInRole(AppRoles.Admin);
            if (isAdmin)
                return Ok(await productService.GetAllDisplayDTOs());
            return Ok((await productService.GetAllDisplayDTOs()).Where(dto => dto.Status.ToLower() == ProductStatus.Approved));

        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute]int id)
        {
            var product = await productService.GetById(id);
            var isAdmin = User.IsInRole(AppRoles.Admin);
            if (isAdmin && product != null)
                return Ok(product);
            if (product?.Status == ProductStatus.Approved)
                return Ok(product);
            return NotFound();


        }
        [HttpGet("get-by-serviceid/{id}")]
        public async Task<IActionResult>GetByServiceId([FromRoute] int id)
        {
            var isAdmin = User.IsInRole(AppRoles.Admin);
            if (isAdmin)
                return Ok(await productService.GetAllProductsBySeriviceId(id));
            return Ok((await productService.GetAllProductsBySeriviceId(id))
                .Where(dto => dto.Status.ToLower() == ProductStatus.Approved));
        }

        [HttpGet("get-by-sellerid/{id}")]
		public async Task<IActionResult> GetBySellerId([FromRoute] string? id)
		{
            if(id is null)
            {
				var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
				return Ok(await productService.GetAllProductsBySellerId(userId!));
			}
                
			return Ok(await productService.GetAllProductsBySellerId(id));
		}



		[HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
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
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();
            var productDTO = await productService.Create(productCreateDTO, userId);
            return CreatedAtAction(nameof(GetById), new { id = productDTO.Id }, productDTO);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute]int id,[FromForm] ProductUpdateDTO productUpdateDTO)
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

            if (!isAdmin && userId != existingProduct.SellerId)
                return Forbid();
            ProductDisplayDTO productDisplayDTO = await productService.Update(productUpdateDTO);

            return Ok(productDisplayDTO);
        }
        [HttpPatch("{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> PutStatus([FromRoute] int id, [FromForm] UpdateProductStatusDTO dto)
        {
            var prod = await productService.UpdateProductStatusAsync(id, dto);
            if (prod == null)
                return NotFound();
            return NoContent();
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] SearchRequestDto searchRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var results = await searchService.SearchProductsAsync(searchRequest.Query, searchRequest.MaxResults);
                return Ok(results);
            }
            catch (Exception ex)
            {
                // In production, you'd want proper logging here
                return StatusCode(500, "An error occurred while processing your search request.");
            }
        }

        [HttpPost("update-embeddings/{id}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> UpdateProductEmbeddings([FromRoute] int id)
        {
            try
            {
                await searchService.UpdateProductEmbeddingsAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating product embeddings.");
            }
        }

        [HttpPost("update-all-embeddings")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> UpdateAllProductEmbeddings()
        {
            try
            {
                await searchService.UpdateAllProductEmbeddingsAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating all product embeddings.");
            }
        }
    }
}
