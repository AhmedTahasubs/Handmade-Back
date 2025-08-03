using DataAcess.Repos.IRepos;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.DTOs.Auth;
using Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Models.Domain;
using Models.Const;
using Microsoft.EntityFrameworkCore;


namespace DataAcess.Repos
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        private readonly ApplicationDbContext db;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration configuration;
        private string securityKey;

        public UserRepository(ApplicationDbContext db, IConfiguration configuration, UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager) : base(db)
        {
            this.db = db;
            this.configuration = configuration;
            this.userManager = userManager;
            this.mapper = mapper;
            this.roleManager = roleManager;
            //Just install `Microsoft.Extensions.Configuration.Binder` and the method `GetValue` will be available
            securityKey = configuration.GetValue<string>("ApiSettings:Secret") ?? throw new InvalidOperationException("ApiSettings:Secret is not configured.");
        }

		public async Task DeleteUser(ApplicationUser user)
		{
			  await userManager.UpdateAsync(user);
		}

		public async Task<ApplicationUser> GetUserByID(string userID)
        {
            var user = await db.ApplicationUser.Include(u => u.Image).FirstOrDefaultAsync(u => u.Id == userID);
			return user ?? throw new InvalidOperationException("User not found.");
        }

        public async Task<bool> IsUniqueUserName(string username)
        {
            var matchUsername = await userManager.FindByNameAsync(username);
            return matchUsername == null;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await userManager.FindByNameAsync(loginRequestDTO.UserName) ??
                await userManager.FindByEmailAsync(loginRequestDTO.UserName);
            if (user == null || !await userManager.CheckPasswordAsync(user, loginRequestDTO.Password)|| user.IsDeleted)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null,
                };
            }
            var userRoles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            claims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds);

            return new LoginResponseDTO()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                User = mapper.Map<UserDTO>(user),
            };
        }

        public async Task<UserDTO> RegisterAdmin(RegisterRequestDTO registerRequestDTO)
        {
            var user = new ApplicationUser
            {
                UserName = registerRequestDTO.UserName,
                FullName = registerRequestDTO.Name,
                Email = registerRequestDTO.Email,
                NormalizedEmail = registerRequestDTO.Email.ToUpper()
            };

            var userDTO = new UserDTO();

            try
            {
                var result = await userManager.CreateAsync(user, registerRequestDTO.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user,AppRoles.Admin);
                    userDTO = mapper.Map<UserDTO>(user);
                }
                else
                {
                    userDTO.ErrorMessages = result.Errors.Select(e => e.Description).ToList();
                }
            }
            catch (Exception)
            {
                userDTO.ErrorMessages = new List<string> { "An unexpected error occurred while registering the user." };
            }

            return userDTO;
        }

		public async Task<UserDTO> RegisterCustomer(CustomerRegisterDto customerRegisterDto)
		{
			var user = new ApplicationUser
			{
				UserName = customerRegisterDto.UserName,
				FullName = customerRegisterDto.Name,
				Email = customerRegisterDto.Email,
				NormalizedEmail = customerRegisterDto.Email.ToUpper(),
				HasWhatsApp = customerRegisterDto.HasWhatsApp,
				Address = customerRegisterDto.Address,
			};

			var userDTO = new UserDTO();

			try
			{
				var result = await userManager.CreateAsync(user, customerRegisterDto.Password);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(user, AppRoles.Customer);
					userDTO = mapper.Map<UserDTO>(user);
				}
				else
				{
					userDTO.ErrorMessages = result.Errors.Select(e => e.Description).ToList();
				}
			}
			catch (Exception)
			{
				userDTO.ErrorMessages = new List<string> { "An unexpected error occurred while registering the user." };
			}

			return userDTO;
		}

		public async Task<UserDTO> RegisterSeller(SellerRegisterDto sellerRegisterDto)
        {
            var user = new ApplicationUser
            {
                UserName = sellerRegisterDto.UserName,
                FullName = sellerRegisterDto.Name,
                Email = sellerRegisterDto.Email,
                NormalizedEmail = sellerRegisterDto.Email.ToUpper(),
                NationalId = sellerRegisterDto.NationalId,
                Bio = sellerRegisterDto.Bio,
            };

            var userDTO = new UserDTO();

            try
            {
                var result = await userManager.CreateAsync(user, sellerRegisterDto.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user,AppRoles.Seller);
                    userDTO = mapper.Map<UserDTO>(user);
                }
                else
                {
                    userDTO.ErrorMessages = result.Errors.Select(e => e.Description).ToList();
                }
            }
            catch (Exception)
            {
                userDTO.ErrorMessages = new List<string> { "An unexpected error occurred while registering the user." };
            }

            return userDTO;
        }



        public async Task<bool> UpdateAsync(ApplicationUser user)
        {
            var existingUser = await db.ApplicationUser.FindAsync(user.Id);
            if (existingUser == null)
            {
                return false;
            }

            if (user.ImageId != 0 || user.ImageId != null)
            {
                existingUser.ImageId = user.ImageId;
            }

            var result = await db.SaveChangesAsync();
            return result > 0;
        }

    }
}