using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Repository.Data;
using Microsoft.AspNetCore.Identity;
using SmartTeethCare.Repository.DataSeed;

namespace SmartTeethCare.API.Extensions
{
    public static class SeedDataExtension
    {
        public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var userManager = services.GetRequiredService<UserManager<User>>();

                context.Database.Migrate();
                await SeedUsers.SeedAsync(services);
                await DoctorSeed.SeedDoctorsAsy(userManager, context);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                throw;
            }

            return app;
        }
    }
}