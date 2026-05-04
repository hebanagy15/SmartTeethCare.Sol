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

        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var response = await _paymentService.CreatePaymentIntent(request);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"],
                    _config["Stripe:WebhookSecret"]
                );
            }
            catch (Exception ex)
            {
                return BadRequest($"Webhook Error: {ex.Message}");
            }

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;

                    if (paymentIntent?.Metadata?.TryGetValue("appointmentId", out var idStr) == true &&
                        int.TryParse(idStr, out var appointmentId))
                    {
                        await _paymentService.HandlePaymentSuccess(appointmentId);
                    }
                    break;

                case "payment_intent.payment_failed":
                  
                    break;
            }

            return Ok();
        }
    }
}