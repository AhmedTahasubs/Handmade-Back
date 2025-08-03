using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.DTOs.CustomerReqestDTOs;

namespace DataAcess.Repos.IRepos
{
    public interface ICustomerRequestService
    {
        Task<CustomerRequestResponse> CreateAsync(CreateCustomerRequestDto dto, string customerId);
        Task<List<CustomerRequestResponse>> GetBySellerIdAsync(string sellerId);
        Task<List<CustomerRequestResponse>> GetByCustomerIdAsync(string customerId);
        Task<CustomerRequestResponse?> GetByIdAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<List<CustomerRequestResponse>> GetAllAsync();
    }
}
