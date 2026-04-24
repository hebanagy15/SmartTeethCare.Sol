using Hangfire;
using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Repository.Data;

namespace SmartTeethCare.API.Extensions
{
    public static class DatabaseExtension
    {
        public static IServiceCollection AddDatabase(
            this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddHangfire(config =>
                config.UseSqlServerStorage(connectionString));

            return services;
        }
    }
}