using Microsoft.Extensions.Configuration;
using SmartTeethCare.Core.DTOs.Payment;
using SmartTeethCare.Core.DTOs.Stripe;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Enums;
using SmartTeethCare.Core.Interfaces.Services.Stripe;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using Stripe;

namespace SmartTeethCare.Service.Services.Stripe
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public PaymentService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;

            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public async Task<PaymentResponse> CreatePaymentIntent(CreatePaymentRequest request)
        {
            var repo = _unitOfWork.Repository<Appointment>();

            var appointment = await repo.GetByIdAsync(request.AppointmentId);

            if (appointment == null)
                throw new Exception("Appointment not found");

            if (appointment.PaymentStatus == AppointmentPaymentStatus.Paid)
                throw new Exception("Already paid");

            var options = new PaymentIntentCreateOptions
            {
                Amount = appointment.Amount * 100,
                Currency = "usd",
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
        }
    }
}