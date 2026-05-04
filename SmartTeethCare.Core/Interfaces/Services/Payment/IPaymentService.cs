using SmartTeethCare.Core.DTOs.Payment;
using SmartTeethCare.Core.DTOs.Stripe;

namespace SmartTeethCare.Core.Interfaces.Services.Stripe
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentIntent(CreatePaymentRequest request);
        Task HandlePaymentSuccess(int appointmentId);
    }
}