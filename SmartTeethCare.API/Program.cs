using SmartTeethCare.API.Middlewares;
using SmartTeethCare.API.Extensions;

namespace SmartTeethCare.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Configure Service
            // Add services to the container.

            builder.Services.AddControllers();
            
            builder.Services.AddCorsPolicy();      // CORS (Deployment)

            builder.Services.AddSwaggerServices(); //Extension method to add swagger services to container 

            builder.Services.AddDatabase(builder.Configuration);

            builder.Services.AddApplicationServices();  //Extension method to add services to container 

            builder.Services.AddJwtAuthentication(builder.Configuration);

            builder.Services.AddAuthorization();

            builder.Services.AddCustomValidation();

            #endregion

            var app = builder.Build();

            #region Configure Middleware

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSwaggerMiddleware();

            app.UseStatusCodePagesWithRedirects("/errors/{0}"); // Handle Status Codes (404)
            app.UseHttpsRedirection();
            app.UseRouting();          

            app.UseCors("AllowFrontend"); // CORS
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            await app.SeedDatabaseAsync();

            #endregion



            app.Run();
        }
    }
}
