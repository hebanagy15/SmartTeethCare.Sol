using Hangfire;
using Microsoft.AspNetCore.Identity;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Repositories;
using SmartTeethCare.Core.Interfaces.Services;
using SmartTeethCare.Core.Interfaces.Services.AdminModule;
using SmartTeethCare.Core.Interfaces.Services.AiService;
using SmartTeethCare.Core.Interfaces.Services.DoctorModule;
using SmartTeethCare.Core.Interfaces.Services.Lookup;
using SmartTeethCare.Core.Interfaces.Services.NotificationService;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;
using SmartTeethCare.Core.Interfaces.Services.SecurityModule;
using SmartTeethCare.Core.Interfaces.Services.Stripe;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using SmartTeethCare.Repository.Data;
using SmartTeethCare.Repository.Implementation;
using SmartTeethCare.Service.AdminModule;
using SmartTeethCare.Service.AiService;
using SmartTeethCare.Service.DoctorModule;
using SmartTeethCare.Service.Implementation;
using SmartTeethCare.Service.Lookup;
using SmartTeethCare.Service.NotificationService;
using SmartTeethCare.Service.PatientModule;
using SmartTeethCare.Service.SecurityModule;
using SmartTeethCare.Service.Services.Stripe;
using SmartTeethCare.Services.AppointmentModule;

namespace SmartTeethCare.API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddHttpClient();

            services.AddHangfireServer();

            services.AddIdentity<User, IdentityRole>()
                   .AddEntityFrameworkStores<ApplicationDbContext>()
                   .AddDefaultTokenProviders();


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped(typeof(IAuthService), typeof(AuthService));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPatientAppointmentService, PatientAppointmentService>();
            services.AddScoped<IPatientMedicalHistoryService, PatientMedicalHistoryService>();
            services.AddScoped<IPatientPrescriptionService, PatientPrescriptionService>();
            services.AddScoped<IPatientReviewService, PatientReviewService>();
            services.AddScoped<IAdminPatientService, AdminPatientService>();
            services.AddScoped<IAdminAppointmentService, AdminAppointmentService>();
            services.AddScoped<IAdminSpecialityService, AdminSpecialityService>();
            services.AddScoped<ILookupService, LookupService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<IDoctorAppointmentService, DoctorAppointmentService>();
            services.AddScoped<IAdminDoctorService, AdminDoctorService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAiService, AiService>();

            services.AddScoped<IDoctorScheduleService, DoctorScheduleService>();
            services.AddScoped<IAppointmentBookingService, AppointmentBookingService>();
            services.AddScoped<IPaymentService, PaymentService>();
            return services;
        }
    }
}
