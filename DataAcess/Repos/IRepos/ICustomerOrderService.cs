﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Domain;

namespace DataAcess.Repos.IRepos
{
    public interface ICustomerOrderService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
        Task<List<OrderResponse>> GetAllOrdersAsync();
        Task<List<OrderResponse>> GetOrdersByCustomerAsync(string customerId);
        Task<List<SellerOrderItemResponse>> GetOrdersBySellerAsync(string sellerId);
        Task<bool> UpdateOrderItemStatusAsync(int orderItemId, string newStatus);
    }
}
