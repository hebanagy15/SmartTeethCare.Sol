using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.DTOs.Payment;
using SmartTeethCare.Core.DTOs.Stripe;

namespace SmartTeethCare.Core.Interfaces.Services.Stripe
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentIntent(CreatePaymentRequest request, int patientId);
        Task HandleWebhook(string json, string stripeSignature);
        Task<AppointmentDetailsDTO> ConfirmPayment(string paymentIntentId, int patientId);
    }
}


 