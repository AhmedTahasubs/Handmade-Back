using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Domain;
using System.Reflection.Emit;

namespace DataAcess
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CustomRequest> CustomRequests { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Category> Categories { get; set; }
        
        public DbSet<ServiceReview> ServiceReviews { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        // new cart and cart item 
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        // new order and order item
        public DbSet<CustomerOrder> CustomerOrders { get; set; }
        public DbSet<CustomerOrderItem> CustomerOrderItems { get; set; }
        // new custom request
        public DbSet<CustomerRequest> CustomerRequests { get; set; }
		public DbSet<Payment> Payments { get; set; }
		protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // ✅ Category
            builder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            // ✅ Service → Category
            builder.Entity<Service>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
               .HasOne(s => s.Service)
               .WithMany(c => c.Products)
               .HasForeignKey(s => s.ServiceId)
               .OnDelete(DeleteBehavior.Cascade);

            // ✅ Service → Seller (User)
            builder.Entity<Service>()
                .HasOne(s => s.Seller)
                .WithMany(u => u.Services)
                .HasForeignKey(s => s.SellerId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ Service → Image
            builder.Entity<Service>()
                .HasOne(s => s.Image)
                .WithMany()
                .HasForeignKey(s => s.ImageId)
                .OnDelete(DeleteBehavior.SetNull);

            // ✅ ServiceReview → Service
            builder.Entity<ServiceReview>()
                .HasOne(r => r.Service)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // ✅ ServiceReview → Reviewer
            builder.Entity<ServiceReview>()
                .HasOne(r => r.Reviewer)
                .WithMany(u => u.ServiceReviews)
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ChatMessage>()
            .HasOne(m => m.Sender)
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict); // don't auto-delete

            builder.Entity<ChatMessage>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerOrder>()
                   .HasMany(o => o.Items)
                   .WithOne(i => i.CustomerOrder)
                   .HasForeignKey(i => i.CustomerOrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CustomerRequest>()
                .HasOne(r => r.Buyer)
                .WithMany() // or .WithMany(u => u.CustomerRequestsSent) if you define navigation in user
                .HasForeignKey(r => r.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerRequest>()
                .HasOne(r => r.Seller)
                .WithMany() // or .WithMany(u => u.CustomerRequestsReceived)
                .HasForeignKey(r => r.SellerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<CustomerRequest>()
                .HasOne(r => r.Service)
                .WithMany() // or .WithMany(s => s.CustomerRequests)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Entity<CustomerOrder>()
                .HasOne(o => o.Customer)
                .WithMany() // or .WithMany(u => u.Orders) if you add a collection on ApplicationUser
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

