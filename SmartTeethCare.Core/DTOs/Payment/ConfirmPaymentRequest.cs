namespace SmartTeethCare.Core.DTOs.Stripe
{
    public class ConfirmPaymentRequest
    {
        public string PaymentIntentId { get; set; } = null!;
    }
}