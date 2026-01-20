
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Repositories;
using SmartTeethCare.Core.Interfaces.Services;
using SmartTeethCare.Repository.Data;
using SmartTeethCare.Repository.Implementation;
using SmartTeethCare.Service;

namespace SmartTeethCare.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");      //from appsettings.json


			builder.Services.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlServer(connectionString));
            builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));
            builder.Services.AddIdentity<User, IdentityRole>()
                   .AddEntityFrameworkStores<ApplicationDbContext>()
                   .AddDefaultTokenProviders();
            builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));

            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    try
            //    {
            //        var db = services.GetRequiredService<ApplicationDbContext>();
            //        db.Database.Migrate(); 
            //        await SmartTeethCare.Repository.DataSeed.SeedUsers.SeedAsync(services); 
            //    }
            //    catch (Exception ex)
            //    {
            //        var logger = services.GetRequiredService<ILogger<Program>>();
            //        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            //        throw;
            //    }
            //}

            app.Run();
        }
    }
}
