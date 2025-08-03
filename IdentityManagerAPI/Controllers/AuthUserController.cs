using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs.Auth;
using Models;
using System.Net;
using DataAcess.Repos.IRepos;
using Models.Domain;
using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Identity.UI.Services;
using IdentityManager.Services.ControllerService;

namespace IdentityManagerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMailingService _mailingService;
		public AuthUserController(IAuthService authService, IMailingService mailingService)
		{
			_authService = authService;
			this._mailingService = mailingService;
		}
		[HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
			var result = await _authService.LoginAsync(loginRequestDTO);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequestDTO)
        {
            var result = await _authService.RegisterAdminAsync(registerRequestDTO);
            return Ok(result);
        }

        [HttpPost("register/seller")]
        public async Task<IActionResult> RegisterSeller([FromBody] SellerRegisterDto sellerRegisterDto)
        {
            var result = await _authService.RegisterSellerAsync(sellerRegisterDto);
            return Ok(result);
        }

        [HttpPost("register/customer")]
        public async Task<IActionResult> RegisterCustomer([FromBody] CustomerRegisterDto customerRegisterDto)
        {
            var result = await _authService.RegisterCustomerAsync(customerRegisterDto);
            return Ok(result);
        }


		[HttpPost("Forgot-Password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordRequestDto)
		{
			var result = await _authService.ForgotPasswordAsync(forgotPasswordRequestDto);
			
			return Ok(result);
		}

		[HttpPost("Reset-Password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
		{
			var result = await _authService.ResetPasswordAsync(resetPasswordRequestDto);
			return Ok(result);
		}
	}
}
