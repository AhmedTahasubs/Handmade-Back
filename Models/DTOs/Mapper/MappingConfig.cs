using AutoMapper;
using Models.Domain;
using Models.DTOs.CustomRequestDTO;
using Models.DTOs.OrderDTO;
using Models.DTOs.OrderItemDTO;
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
