using AutoMapper;
using Models.Domain;
using Models.DTOs;
using Models.DTOs.User;
using Models.DTOs.CustomRequestDTO;
using Models.DTOs.OrderDTO;
using Models.DTOs.OrderItemDTO;
using System;
using Models.DTOs.Categories;
using Models.Const;

namespace Models.DTOs.Mapper
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            // User
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserProfileDto>()
                .AfterMap((src, dest) =>
				{
					dest.Imageurl = src?.Image?.FilePath ?? "https://localhost:7047/images/avatar.png";
				});
			CreateMap<ApplicationUser, UserMangementDto>().ReverseMap();
			//Category
			CreateMap<CreateCategoryDto, Category>().ReverseMap();

            // Product
            CreateMap<Product, ProductDisplayDTO>()

                .AfterMap((src, dest) =>
                {
					dest.ImageUrl = src?.Image?.FilePath;
					dest.Category = src?.Service?.Name ?? string.Empty;
					dest.SellerName = src?.User?.FullName;
					
				});
            CreateMap<ProductDisplayDTO, Product>(); 


            // ProductUpdateDTO <-> Product
            CreateMap<ProductUpdateDTO, Product>()
                .AfterMap((src, dest) => dest.Status = ProductStatus.Pending);

            CreateMap<Product, ProductUpdateDTO>();


            // ProductCreateDTO -> Product
            CreateMap<ProductCreateDTO, Product>()
                .AfterMap((src, dest) => dest.Status = ProductStatus.Pending);

            

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
