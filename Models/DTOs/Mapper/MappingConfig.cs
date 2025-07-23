using AutoMapper;
using Models.Domain;
using Models.DTOs;
using Models.DTOs.User;
using Models.DTOs.CustomRequestDTO;
using Models.DTOs.OrderDTO;
using Models.DTOs.OrderItemDTO;
using System;
using Models.DTOs.Categories;
//using Models.DTOs.Category;

namespace Models.DTOs.Mapper
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // User
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();

			//Category
			CreateMap<CreateCategoryDto, Category>().ReverseMap();
			// Product
			CreateMap<Product, ProductDisplayDTO>();
            CreateMap<ProductUpdateDTO, Product>();
            CreateMap<ProductCreateDTO, Product>();

            // Order
            CreateMap<Order, OrderReadDto>();
            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderUpdateDto, Order>();

            // OrderItem
            CreateMap<OrderItem, OrderItemReadDto>();
            CreateMap<OrderItemCreateDto, OrderItem>();

            // CustomRequest
            CreateMap<CustomRequest, CustomRequestReadDto>();
            CreateMap<CustomRequestCreateDto, CustomRequest>();
            CreateMap<CustomRequestUpdateDto, CustomRequest>();
        }
    }
}
