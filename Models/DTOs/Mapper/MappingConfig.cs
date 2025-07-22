using AutoMapper;
using Models.Domain;
using Models.DTOs;
using Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs.Mapper
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<Product, ProductDisplayDTO>();
            CreateMap<ProductUpdateDTO, Product>();
            CreateMap<ProductCreateDTO, Product>();
        }
    }
}
