﻿using AutoMapper;
using DataAcess.Repos.IRepos;
using IdentityManager.Services.ControllerService.IControllerService;
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

		public UserService(IImageRepository imageRepo, IUserRepository userRepo, IMapper mapper)
		{
			_imageRepo = imageRepo;
			_userRepo = userRepo;
			_mapper = mapper;
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
			var usersDto = _mapper.Map<IEnumerable<UserMangementDto>>(users);
            return usersDto;
		}

		public async Task<object> UploadUserImageAsync(string userId, ImageUploadRequestDto request)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new Exception("User not found");
            }

            var user = await _userRepo.GetUserByID(userId);
            ValidateFileUpload(request);

            var image = new Image
            {
                File = request.File,
                FileName = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                FileExtension = Path.GetExtension(request.File.FileName),
                FileSize = request.File.Length
            };

            await _imageRepo.Upload(image);
            user.ImageId = image.Id;
            await _userRepo.UpdateAsync(user);

            return user;
        }

        private void ValidateFileUpload(ImageUploadRequestDto request)
        {
            if (request.File == null)
            {
                throw new Exception("File is required");
            }
            if (request.File.Length == 0)
            {
                throw new Exception("File is empty");
            }
            if (request.File.Length > 10 * 1024 * 1024)
            {
                throw new Exception("File is too large");
            }
            if (request.File.ContentType != "image/jpeg" && request.File.ContentType != "image/png")
            {
                throw new Exception("File is not an image");
            }
        }
    }
}
