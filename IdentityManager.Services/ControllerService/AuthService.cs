using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Domain;
using Models.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManager.Services.ControllerService
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMailingService _mailingService;
        private readonly UserManager<ApplicationUser> _userManager;

		public AuthService(IUserRepository userRepository, IMailingService mailingService, UserManager<ApplicationUser> userManager)
		{
			_userRepository = userRepository;
			_mailingService = mailingService;
			_userManager = userManager;
		}

		public async Task<object> LoginAsync(LoginRequestDTO loginRequestDTO)
        {
            return await _userRepository.Login(loginRequestDTO);
        }

        public async Task ValidateUserNameAndEmail(string Email, string userName)
        {
			var emailExist = await _userRepository.GetAsync(user => user.Email == Email);
			var usernameExist = await _userRepository.IsUniqueUserName(userName);
			if (!usernameExist)
			{
				throw new ValidationException("Username Already exists");
			}
			if (emailExist != null)
			{
				throw new ValidationException("Email Already exists");
			}
		}

        public async Task<object> RegisterAdminAsync(RegisterRequestDTO registerRequestDTO)
        {
            await ValidateUserNameAndEmail(registerRequestDTO.Email, registerRequestDTO.UserName);
			return await _userRepository.RegisterAdmin(registerRequestDTO);
        }
        public async Task<object> RegisterSellerAsync(SellerRegisterDto sellerRegisterDto)
        {
			await ValidateUserNameAndEmail(sellerRegisterDto.Email, sellerRegisterDto.UserName);
            var nationalIdExist = await _userRepository.GetAsync(user => user.NationalId == sellerRegisterDto.NationalId);
			if (nationalIdExist != null)
            {
				throw new ValidationException("National ID Already exists");
			}

				return await _userRepository.RegisterSeller(sellerRegisterDto);
        }
        public async Task<object> RegisterCustomerAsync(CustomerRegisterDto customerRegisterDto)
        {
			await ValidateUserNameAndEmail(customerRegisterDto.Email, customerRegisterDto.UserName);
			return await _userRepository.RegisterCustomer(customerRegisterDto);
        }

		public async Task<object> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto)
		{
			var user = await _userRepository.GetAsync(u => u.Email == forgotPasswordRequestDto.Email);
			if (user == null)
			{
				throw new ValidationException("User with this email does not exist.");
			}
			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

			if (string.IsNullOrEmpty(token))
			{
				throw new ValidationException("Some thing went wrong!");
			}

			var callBackUrl = $"http://localhost:4200/reset-password?token={WebUtility.UrlEncode(token)}&email={user.Email}";

			var filePath = $"{Directory.GetCurrentDirectory()}\\Templates\\Email.html";
			var str = new StreamReader(filePath);

			var mailText = str.ReadToEnd();
			str.Close();

			mailText = mailText.Replace("[header]", $"Hey {user.FullName}")
				.Replace("[body]", "Please click the below button to reset your password")
				.Replace("[imageUrl]", "https://res.cloudinary.com/gradbookify/image/upload/v1754135477/icon-positive-vote-2_jcxdww_mo1gkb.svg")
				.Replace("[linkTitle]", "Reset Paswword")
				.Replace("[url]", callBackUrl);

			await _mailingService.SendEmailAsync(forgotPasswordRequestDto.Email, "Reset Password", mailText);

			return new
			{
				token = token,
				email = user.Email,
			};
		}

		public Task<object> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto)
		{
			var user = _userRepository.GetAsync(u => u.Email == resetPasswordRequestDto.Email);
			if (user == null)
			{
				throw new ValidationException("User with this email does not exist.");
			}
			var result = _userManager.ResetPasswordAsync(user.Result, resetPasswordRequestDto.Token, resetPasswordRequestDto.NewPassword);
			if (!result.Result.Succeeded)
			{
				throw new ValidationException("Reset password failed.");
			}
			return Task.FromResult<object>(new { message = "Password reset successfully." });
		}
	}
}
