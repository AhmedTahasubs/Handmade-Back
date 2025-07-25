﻿using AutoMapper;
using Models.Domain;
using Models.DTOs;
using Models.DTOs.User;
using Models.DTOs.CustomRequestDTO;
using Models.DTOs.OrderDTO;
using Models.DTOs.OrderItemDTO;
using System;
using Models.DTOs.Categories;
using Models.Const;
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
            // ProductDisplayDTO <-> Product
            CreateMap<Product, ProductDisplayDTO>()
                .AfterMap((src, dest) => dest.ImageUrl = src?.Image?.FilePath);
            CreateMap<ProductDisplayDTO, Product>(); 


            // ProductUpdateDTO <-> Product
            CreateMap<ProductUpdateDTO, Product>()
                .AfterMap((src, dest) => dest.Status = ProductStatus.Pending);

            CreateMap<Product, ProductUpdateDTO>();


            // ProductCreateDTO -> Product
            CreateMap<ProductCreateDTO, Product>()
                .AfterMap((src, dest) => dest.Status = ProductStatus.Pending);

            ;

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
