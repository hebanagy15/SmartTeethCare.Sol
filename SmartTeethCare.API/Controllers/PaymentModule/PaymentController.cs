using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.Stripe;
using SmartTeethCare.Core.Interfaces.Services.Stripe;
using Stripe;

namespace SmartTeethCare.API.Controllers.PaymentModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _config;

        public PaymentController(IPaymentService paymentService, IConfiguration config)
        {
            _paymentService = paymentService;
            _config = config;
        }

        // =========================
        // CREATE PAYMENT INTENT
        // =========================
        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            if (request == null || request.AppointmentId <= 0)
                return BadRequest("Invalid request");

            var response = await _paymentService.CreatePaymentIntent(request);
            return Ok(response);
        }

        // =========================
        // STRIPE WEBHOOK
        // =========================
        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();

            if (string.IsNullOrEmpty(signature))
                return BadRequest("Missing Stripe Signature");

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    signature,
                    _config["Stripe:WebhookSecret"]
                );
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook Error: {ex.Message}");
            }

            // =========================
            // PAYMENT SUCCESS
            // =========================
            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                if (paymentIntent == null)
                    return BadRequest("PaymentIntent is null");

                if (paymentIntent.Metadata != null &&
                    paymentIntent.Metadata.TryGetValue("appointmentId", out var idStr) &&
                    int.TryParse(idStr, out var appointmentId))
                {
                    await _paymentService.HandlePaymentSuccess(appointmentId);
                }
            }

            // =========================
            // PAYMENT FAILED
            // =========================
            else if (stripeEvent.Type == "payment_intent.payment_failed")
            {
                var failedIntent = stripeEvent.Data.Object as PaymentIntent;

                if (failedIntent != null)
                {
                    Console.WriteLine($"Payment Failed: {failedIntent.Id}");
                }
            }

            return Ok();
        }
    }
}