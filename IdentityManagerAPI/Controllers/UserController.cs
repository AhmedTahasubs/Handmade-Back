using DataAcess;
using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;
using Models.Domain;
using Models.DTOs.image;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityManagerAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{

		private readonly IUserService userService;
		private readonly ApplicationDbContext _context;

		public UserController(IUserService userService, ApplicationDbContext context)
		{
			this.userService = userService;
			_context = context;
		}

		public IUserRepository UserRepo { get; }

		[HttpPost]
		[Authorize]
		[Route("uploadUserImage")]
		public async Task<IActionResult> UploadUserImage([FromForm] ImageUploadRequestDto request)
		{
			var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
			var result = await userService.UploadUserImageAsync(userId, request);
			return Ok(result);
		}
		[HttpGet("{id}")]
		public async Task<IActionResult> GetUser([FromRoute] string id)
		{
			var user = await userService.GetById(id);
			return Ok(user);
		}

		//get all users
		[HttpGet]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetAllUsers()
		{
			var users = await userService.GetAllUsers();
			return Ok(users);
		}

		[HttpDelete("{id}")]
		//[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteUser([FromRoute] string id)
		{
			await userService.DeleteUser(id);
			return NoContent();
		}
	}
}