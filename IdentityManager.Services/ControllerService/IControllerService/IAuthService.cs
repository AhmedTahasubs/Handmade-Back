using Models.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManager.Services.ControllerService.IControllerService
{
    public interface IAuthService
    {
        Task<object> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<object> RegisterAdminAsync(RegisterRequestDTO registerRequestDTO);
        Task<object> RegisterSellerAsync(SellerRegisterDto sellerRegistertDTO);
        Task<object> RegisterCustomerAsync(CustomerRegisterDto customerRegistertDTO);
        Task<object> ForgotPasswordAsync(ForgotPasswordRequestDto forgotPasswordRequestDto);
        Task<object> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto);

	}
}
