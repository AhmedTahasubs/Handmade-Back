using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Models.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManager.Services.ControllerService
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;

        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
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
    }
}
