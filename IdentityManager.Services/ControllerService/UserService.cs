using AutoMapper;
using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
using Microsoft.AspNetCore.Identity;
using Models.Const;
using Models.Domain;
using Models.DTOs.image;
using Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManager.Services.ControllerService
{
	public class UserService : IUserService
	{
		private readonly IImageRepository _imageRepo;
		private readonly IUserRepository _userRepo;
		private readonly IMapper _mapper;
		private readonly UserManager<ApplicationUser> _userManager;

		public UserService(IImageRepository imageRepo, IUserRepository userRepo, IMapper mapper, UserManager<ApplicationUser> userManager)
		{
			_imageRepo = imageRepo;
			_userRepo = userRepo;
			_mapper = mapper;
			_userManager = userManager;
		}

		public async Task<UserProfileDto> GetById(string userId)
		{
			var user = await _userRepo.GetUserByID(userId);
			var userDto = _mapper.Map<UserProfileDto>(user);
			return userDto;
		}
		public async Task<IEnumerable<UserMangementDto>> GetAllUsers()
		{
			var users = await _userRepo.GetAllAsync();
			var usersDto = new List<UserMangementDto>();
			foreach (var user in users)
			{
				var roles = await _userManager.GetRolesAsync(user);

				var userDto = _mapper.Map<UserMangementDto>(user);
				userDto.Roles = roles.ToList();

				usersDto.Add(userDto);
			}
			return usersDto;
		}

		public async Task<IEnumerable<UserMangementDto>> GetAllUnVerifiedSellers()
		{
			var users = await _userRepo.GetAllAsync(includes: "Image,IdCardImage");
			var sellers = new List<UserMangementDto>();
			foreach (var user in users)
			{
				if (await _userManager.IsInRoleAsync(user, AppRoles.Seller) && user.Status == VerificationStatus.Unverified)
				{
					sellers.Add(_mapper.Map<UserMangementDto>(user));
				}
			}
			return sellers;
		}

		public async Task<object> UploadUserImageAsync(string userId, ImageUploadRequestDto request)
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new Exception("User not found");
			}

			var user = await _userRepo.GetUserByID(userId);
			ValidateFileUpload(request);

			if (request.ProfileImage != null)
			{
				var image = new Image
				{
					File = request.ProfileImage,
					FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
					FileExtension = Path.GetExtension(request.ProfileImage.FileName),
					FileSize = request.ProfileImage.Length
				};

				await _imageRepo.Upload(image);
				user.ImageId = image.Id;
			}
			if (request.IdCardImage != null)
			{
				var idCardImage = new Image
				{
					File = request.IdCardImage,
					FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
					FileExtension = Path.GetExtension(request.IdCardImage.FileName),
					FileSize = request.IdCardImage.Length
				};
				await _imageRepo.Upload(idCardImage);
				user.IdCardImageId = idCardImage.Id;
			}

			await _userRepo.UpdateAsync(user);
			return user;
		}

		private void ValidateFileUpload(ImageUploadRequestDto request)
		{
			if (request.ProfileImage == null || request.IdCardImage == null)
			{
				throw new Exception("File is required");
			}
			if (request.ProfileImage.Length == 0 || request.IdCardImage.Length == 0)
            {
                throw new Exception("File is empty");
            }
            if (request.ProfileImage.Length > 10 * 1024 * 1024 || request.IdCardImage.Length > 10 * 1024 * 1024)
            {
                throw new Exception("File is too large");
            }
            if ((request.ProfileImage.ContentType != "image/jpeg" && request.ProfileImage.ContentType != "image/png")
				|| (request.IdCardImage.ContentType != "image/jpeg" && request.IdCardImage.ContentType != "image/png"))
            {
                throw new Exception("File is not an image");
            }
        }

		public async Task DeleteUser(string userId)
		{
			var user = await _userRepo.GetUserByID(userId);
            user.IsDeleted = !user.IsDeleted;
            user.LastUpdatedOn = DateTime.Now;
			await _userRepo.UpdateUser(user);
		}

		public async Task ChangeSellerStatus(string userId, string newStatus)
		{
			var user = await _userRepo.GetUserByID(userId);
			if (!VerificationStatus.TryParse(newStatus, out var parsedStatus))
			{
				Console.WriteLine($"Invalid status: {newStatus}");
				throw new ArgumentException("Invalid verification status.");
			}
			user.Status = parsedStatus;
			await _userRepo.UpdateUser(user);
		}

		public async Task<string> GetSellerStatus(string userId)
		{
			var user = await _userRepo.GetUserByID(userId);
			return user.Status;
		}
	}
}
