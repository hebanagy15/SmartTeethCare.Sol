
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SmartTeethCare.API.Errors;
using SmartTeethCare.API.Middlewares;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Repositories;
using SmartTeethCare.Core.Interfaces.Services;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;
using SmartTeethCare.Core.Interfaces.Services.DoctorModule;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using SmartTeethCare.Repository.Data;
using SmartTeethCare.Repository.Implementation;
using SmartTeethCare.Service;
using SmartTeethCare.Service.AdminModule;
using SmartTeethCare.Service.DoctorModule;
using SmartTeethCare.Service.Implementation;
using SmartTeethCare.Service.PatientModule;
using SmartTeethCare.Services.AppointmentModule;
using SmartTeethCare.Web.Areas.Patient.Controllers;
using System.Text;

namespace SmartTeethCare.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            // CORS (Deployment)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy
                        .WithOrigins(
                            "http://localhost:8081"         // menna
                            
                        // áãÇ ÇáÝÑæäÊ íÊÚãáå deploy ÖíÝí ÇáÏæãíä åäÇ
                        // "https://smart-teeth-care.vercel.app"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });



            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");      //from appsettings.json


			builder.Services.AddDbContext<ApplicationDbContext>(options =>
					options.UseSqlServer(connectionString));
            
            builder.Services.AddIdentity<User, IdentityRole>()
                   .AddEntityFrameworkStores<ApplicationDbContext>()
                   .AddDefaultTokenProviders();

            
            builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));
            builder.Services.AddScoped<IPatientAppointmentService, PatientAppointmentService>();
            builder.Services.AddScoped<IPatientMedicalHistoryService, PatientMedicalHistoryService>();
            builder.Services.AddScoped<IPatientPrescriptionService, PatientPrescriptionService>();
            builder.Services.AddScoped<IAdminPatientService, AdminPatientService>();
            builder.Services.AddScoped<IAdminAppointmentService, AdminAppointmentService>();
            builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();
            builder.Services.AddScoped<IDoctorAppointmentService, DoctorAppointmentService>();



            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer("Bearer", options =>
           {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])
           )
        };
    });

            builder.Services.AddAuthorization();
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    var errors = actionContext.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage).ToArray();
                    var errorResponse = new ApiValidationErrorResponse
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartTeethCare API V1");
            });

            app.UseStatusCodePagesWithRedirects("/errors/{0}"); // Handle Status Codes (404)
            app.UseHttpsRedirection();
            app.UseRouting();          

            app.UseCors("AllowFrontend"); // CORS
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var db = services.GetRequiredService<ApplicationDbContext>();
                    db.Database.Migrate();
                    await SmartTeethCare.Repository.DataSeed.SeedUsers.SeedAsync(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while migrating or seeding the database.");
                    throw;
                }
            }

            app.Run();
        }
    }
}
