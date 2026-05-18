using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using SmartTeethCare.Core.DTOs.Payment;
using SmartTeethCare.Core.DTOs.Stripe;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.NotificationService;
using SmartTeethCare.Core.Interfaces.Services.Stripe;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using Stripe;

namespace SmartTeethCare.Service.Services.Stripe
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService;
        private readonly UserManager<User> _userManager;

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration config , INotificationService notificationService , UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _config = config;
            _notificationService = notificationService;
            _userManager = userManager;

            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<PaymentResponse> CreatePaymentIntent(CreatePaymentRequest request)
        {
            var repo = _unitOfWork.Repository<Appointment>();

            var appointment = await repo.GetByIdAsync(request.AppointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found");

            if (appointment.PaymentStatus == AppointmentPaymentStatus.Paid)
                throw new Exception("Appointment already paid");

        
            long depositAmount = 50 * 100;

            var options = new PaymentIntentCreateOptions
            {
                Amount = depositAmount,

            
                Currency = "egp",

                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true
                },

                Metadata = new Dictionary<string, string>
                {
                    { "appointmentId", appointment.Id.ToString() }
                }
            };

            var service = new PaymentIntentService();

            var paymentIntent = await service.CreateAsync(options);

            appointment.PaymentIntentId = paymentIntent.Id;
            appointment.PaymentMethod = AppointmentPaymentMethod.Visa;
            appointment.PaymentStatus = AppointmentPaymentStatus.Pending;

            await repo.UpdateAsync(appointment);
            await _unitOfWork.CompleteAsync();

            return new PaymentResponse
            {
                ClientSecret = paymentIntent.ClientSecret,
                PaymentIntentId = paymentIntent.Id
            };
        }

        public async Task HandlePaymentSuccess(int appointmentId)
        {
            var repo = _unitOfWork.Repository<Appointment>();

            var appointment = await repo.GetByIdAsync(appointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found");

            if (appointment.PaymentStatus == AppointmentPaymentStatus.Paid)
                return;

            appointment.PaymentStatus = AppointmentPaymentStatus.Paid;

            await repo.UpdateAsync(appointment);
            await _unitOfWork.CompleteAsync();

            // 👇 هات الدكتور
            var doctor = await _unitOfWork.Repository<Doctor>()
                .GetByIdAsync(appointment.DoctorID);

            var doctorUser = doctor != null
                ? await _userManager.FindByIdAsync(doctor.UserId)
                : null;

            var doctorName = doctorUser?.DisplayName
                ?? doctorUser?.UserName
                ?? "Doctor";

            var userId = appointment.patient?.UserId
                ?? throw new Exception("Patient not found");

            try
            {
                // 📩 Notification / Email
                await _notificationService.CreateAsync(
                    userId,
                    "Appointment Confirmed",
                    "Your appointment is confirmed after payment",
                    true,
                    new Dictionary<string, string>
                    {
                { "DATE", appointment.Date.ToString("dd/MM/yyyy hh:mm tt") },
                { "DOCTOR", doctorName }
                    }
                );

                // ⏰ Reminder scheduling
                var appointmentDateTime = appointment.Date.Date + appointment.StartTime;
                var reminderTime = appointmentDateTime.ToUniversalTime().AddHours(-3);

                if (reminderTime > DateTime.UtcNow)
                {
                    BackgroundJob.Schedule<INotificationService>(
                        x => x.SendReminderAsync(appointment.Id),
                        reminderTime
                    );
                }
            }
            catch
            {
                // log error only
            }
        }
    }
}