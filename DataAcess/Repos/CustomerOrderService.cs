﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAcess.Repos.IRepos;
using Microsoft.EntityFrameworkCore;
using Models.Domain;

namespace DataAcess.Repos
{
    public enum OrderItemStatus
    {
        Pending,
        Shipped,
        Delivered,
        Rejected
    }
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly ApplicationDbContext _context;

        public CustomerOrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            var cart = await _context.ShoppingCarts
                .Include(c => c.Items)
                .ThenInclude(ci => ci.Product)
                .FirstOrDefaultAsync(c => c.CustomerId == request.CustomerId);

            if (cart == null || !cart.Items.Any())
                throw new Exception("Cart is empty");

            var order = new CustomerOrder
            {
                CustomerId = request.CustomerId,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                PaymentMethod = request.PaymentMethod,
                CreatedAt = DateTime.UtcNow,
                Items = cart.Items.Select(item => new CustomerOrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SellerId = item.Product.SellerId!
                }).ToList()
            };

            _context.CustomerOrders.Add(order);
            _context.ShoppingCarts.Remove(cart);
            await _context.SaveChangesAsync();
            var savedOrder = await _context.CustomerOrders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Image)
                .FirstOrDefaultAsync(o => o.Id == order.Id);
            return MapOrderToResponse(savedOrder!);
        }

        public async Task<List<OrderResponse>> GetAllOrdersAsync()
        {
            var orders = await _context.CustomerOrders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Image)
                .ToListAsync();

            return orders.Select(MapOrderToResponse).ToList();
        }

        public async Task<List<OrderResponse>> GetOrdersByCustomerAsync(string customerId)
        {
            var orders = await _context.CustomerOrders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .ThenInclude(p => p.Image)
                .ToListAsync();

            return orders.Select(MapOrderToResponse).ToList();
        }

        public async Task<List<SellerOrderItemResponse>> GetOrdersBySellerAsync(string sellerId)
        {
            return await _context.CustomerOrderItems
                .Where(i => i.SellerId == sellerId)
                .Include(i => i.CustomerOrder)
                .Include(i => i.Product)
                .Select(i => new SellerOrderItemResponse
                {
                    OrderId = i.CustomerOrderId,
                    CreatedAt = i.CustomerOrder.CreatedAt,
                    CustomerName = i.CustomerOrder.CustomerId,
                    CustomerPhone = i.CustomerOrder.PhoneNumber,
                    ProductTitle = i.Product.Title,
                    ProductImageUrl = i.Product.Image!.FilePath,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Status = i.Status
                }).ToListAsync();
        }

        private OrderResponse MapOrderToResponse(CustomerOrder order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                TotalPrice = order.Items.Sum(i => i.Quantity * i.UnitPrice),
                Items = order.Items.Select(i => new OrderItemResponse
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductTitle = i.Product?.Title ?? "Unknown",
                    ProductImageUrl = i.Product?.Image?.FilePath ?? "Image Not Loaded",
                    SellerId = i.SellerId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Status = i.Status
                }).ToList()
            };
        }
        public async Task<bool> UpdateOrderItemStatusAsync(int orderItemId, string newStatus)
        {
            if (!Enum.TryParse<OrderItemStatus>(newStatus, true, out var parsedStatus))
            {
                Console.WriteLine($"Invalid status: {newStatus}");
                throw new ArgumentException("Invalid order item status.");
            }

            var orderItem = await _context.CustomerOrderItems.FindAsync(orderItemId);
            if (orderItem == null)
            {
                Console.WriteLine($"Order item with ID {orderItemId} not found.");
                return false;
            }

            orderItem.Status = parsedStatus.ToString();
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
