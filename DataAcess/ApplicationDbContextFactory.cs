using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DataAcess
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // 1️⃣ حمّل الإعدادات من الـ appsettings.json
            var basePath = Directory.GetCurrentDirectory();

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true) // لو فيه إعدادات خاصة بالتطوير
                .Build();

            // 2️⃣ هات الـ ConnectionString
            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("⚠️ Connection string 'DefaultConnection' not found in appsettings.json!");
            }

            // 3️⃣ جهز الـ options بتاعة الـ DbContext
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // 4️⃣ رجّع نسخة جديدة من الـ DbContext
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
