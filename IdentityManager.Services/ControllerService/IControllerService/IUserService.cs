using Models.DTOs;
using Models.DTOs.image;
using Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManager.Services.ControllerService.IControllerService
{
    public interface IUserService
    {
        Task<object> UploadUserImageAsync(string userId, ImageUploadRequestDto request);
        Task<UserProfileDto> GetById(string userId);
        Task<IEnumerable<UserMangementDto>> GetAllUsers();

    }
}
