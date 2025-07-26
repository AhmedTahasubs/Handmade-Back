using Microsoft.AspNetCore.Http;
using Models.Domain;
using Models.DTOs;
using Models.DTOs.image;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityManager.Services.ControllerService.IControllerService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDisplayDTO>> GetAllDisplayDTOs();
        Task<ProductDisplayDTO> GetById(int id);
        Task<ProductDisplayDTO> Create(ProductCreateDTO dto);
        Task<ProductDisplayDTO> Update(ProductUpdateDTO dto);
        Task Delete(Product p);
        Task<int> UploadProductImageAsync(IFormFile File);
    }
}
