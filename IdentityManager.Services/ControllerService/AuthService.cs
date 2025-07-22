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

        public async Task<object> RegisterAsync(RegisterRequestDTO registerRequestDTO)
        {
            var emailExist = await _userRepository.GetAsync(user => user.Email == registerRequestDTO.Email);
            if (emailExist != null)
            {
                throw new ValidationException("Email Already exists");
            }

            return await _userRepository.Register(registerRequestDTO);
        }
    }
}
