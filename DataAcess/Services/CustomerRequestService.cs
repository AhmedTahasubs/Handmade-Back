using Microsoft.EntityFrameworkCore;
using DataAcess.Repos.IRepos;
using Models.Domain;
using Models.DTOs.CustomerReqestDTOs;

namespace DataAcess.Services
{
    public class CustomerRequestService : ICustomerRequestService
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageRepository _imageRepo;

        public CustomerRequestService(ApplicationDbContext context, IImageRepository imageRepo)
        {
            _context = context;
            _imageRepo = imageRepo;
        }
        public async Task<List<CustomerRequestResponse>> GetAllAsync()
        {
            return await _context.CustomerRequests
                .Include(r => r.Buyer)
                .Include(r => r.Seller)
                .Include(r => r.Service)
                .Select(r => new CustomerRequestResponse
                {
                    Id = r.Id,
                    BuyerId = r.BuyerId,
                    SellerId = r.SellerId,
                    BuyerName = r.Buyer.UserName,
                    SellerName = r.Seller.UserName,
                    ServiceId = r.ServiceId,
                    ServiceTitle = r.Service != null ? r.Service.Name : null,
                    Description = r.Description,
                    ReferenceImageUrl = r.ReferenceImageUrl,
                    Status = r.Status,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }
        public async Task<CustomerRequestResponse> CreateAsync(CreateCustomerRequestDto dto, string customerId)
        {
            // Upload image
            var image = new Image
            {
                File = dto.File,
                FileName = Path.GetFileNameWithoutExtension(dto.File.FileName),
                FileExtension = Path.GetExtension(dto.File.FileName)
            };

            var uploadedImage = await _imageRepo.Upload(image);

            var request = new CustomerRequest
            {
                BuyerId = customerId,
                SellerId = dto.SellerId,
                ServiceId = dto.ServiceId,
                Description = dto.Description,
                ReferenceImageUrl = uploadedImage.FilePath,
                CreatedAt = DateTime.UtcNow,
                Status = RequestStatus.Pending
            };

            _context.CustomerRequests.Add(request);
            await _context.SaveChangesAsync();

            return MapToResponse(request);
        }

        public async Task<List<CustomerRequestResponse>> GetBySellerIdAsync(string sellerId)
        {
            return await _context.CustomerRequests
                .Where(r => r.SellerId == sellerId)
                .Select(r => MapToResponse(r))
                .ToListAsync();
        }

        public async Task<List<CustomerRequestResponse>> GetByCustomerIdAsync(string customerId)
        {
            return await _context.CustomerRequests
                .Where(r => r.BuyerId == customerId)
                .Select(r => MapToResponse(r))
                .ToListAsync();
        }

        public async Task<CustomerRequestResponse?> GetByIdAsync(int id)
        {
            var r = await _context.CustomerRequests
                .Include(r => r.Buyer)
                .Include(r => r.Seller)
                .Include(r => r.Service)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (r == null) return null;

            return new CustomerRequestResponse
            {
                Id = r.Id,
                BuyerId = r.BuyerId,
                SellerId = r.SellerId,
                BuyerName = r.Buyer?.UserName,
                SellerName = r.Seller?.UserName,
                ServiceId = r.ServiceId,
                ServiceTitle = r.Service?.Name,
                Description = r.Description,
                ReferenceImageUrl = r.ReferenceImageUrl,
                Status = r.Status,
                CreatedAt = r.CreatedAt
            };
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var request = await _context.CustomerRequests.FindAsync(id);
            if (request == null) return false;

            if (!Enum.TryParse<RequestStatus>(status, true, out var parsedStatus))
                return false;

            request.Status = parsedStatus;
            await _context.SaveChangesAsync();
            return true;
        }

        private static CustomerRequestResponse MapToResponse(CustomerRequest request) => new()
        {
            Id = request.Id,
            BuyerId = request.BuyerId,
            SellerId = request.SellerId,
            ServiceId = request.ServiceId,
            Description = request.Description,
            ReferenceImageUrl = request.ReferenceImageUrl,
            Status = request.Status,
            CreatedAt = request.CreatedAt
        };
    }
}
