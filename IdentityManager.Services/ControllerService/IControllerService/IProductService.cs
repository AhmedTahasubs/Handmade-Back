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
        Task<IEnumerable<ProductDisplayDTO>> GetAllProductsBySeriviceId(int seriviceId);
        Task<IEnumerable<ProductDisplayDTO>> GetAllProductsBySellerId(string sellerId);

        Task<ProductDisplayDTO> GetById(int id);
        Task<ProductDisplayDTO> Create(ProductCreateDTO dto, string sellerId);
        Task<ProductDisplayDTO> Update(ProductUpdateDTO dto);
        Task Delete(Product p);
        Task<int> UploadProductImageAsync(IFormFile File);
    }
}
