using Models.Domain;
using Models.DTOs.Auth;
using Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAcess.Repos.IRepos
{
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<bool> IsUniqueUserName(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
        Task<UserDTO> RegisterAdmin(RegisterRequestDTO registerRequestDTO);
        Task<UserDTO> RegisterSeller(SellerRegisterDto sellerRegisterDto);
        Task<UserDTO> RegisterCustomer(CustomerRegisterDto customerRegisterDto);
        Task<ApplicationUser> GetUserByID(string userID);
        Task<bool> UpdateAsync(ApplicationUser user);
        Task UpdateUser(ApplicationUser user);

	}
}
