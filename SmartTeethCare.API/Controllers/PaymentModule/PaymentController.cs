// SmartTeethCare.API/Controllers/PaymentModule/PaymentController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.Stripe;
using SmartTeethCare.Core.Entities;
using SmartTeethCare.Core.Interfaces.Services.Stripe;
using SmartTeethCare.Core.Interfaces.UnitOfWork;
using System.Security.Claims;

namespace SmartTeethCare.API.Controllers.PaymentModule
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentController(IPaymentService paymentService, IUnitOfWork unitOfWork)
        {
            _paymentService = paymentService;
            _unitOfWork = unitOfWork;
        }

        // ✅ STEP 1: المريض بيبعت بيانات الموعد → بنرجعله ClientSecret
        [Authorize(Roles = "Patient")]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            // ✅ جيب PatientId من الـ Token مش من الـ Body
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var patients = await _unitOfWork.Repository<Patient>()
                .FindAsync(p => p.UserId == userId);
            var patient = patients.FirstOrDefault();
            if (patient == null) return Unauthorized("Patient not found");

            var response = await _paymentService.CreatePaymentIntent(request, patient.Id);
            return Ok(response);
        }

        // ✅ STEP 2: Stripe بتبعت Webhook بعد الدفع
        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            // ✅ ارجع للأول
            HttpContext.Request.Body.Position = 0;

            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();

            if (string.IsNullOrEmpty(signature))
                return BadRequest("Missing Stripe Signature");

            try
            {
                await _paymentService.HandleWebhook(json, signature);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, details = ex.InnerException?.Message });
            }
        }

        // ✅ STEP 3: الفرونت بينده على ده بعد ما confirmCardPayment ينجح
        [Authorize(Roles = "Patient")]
        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var patients = await _unitOfWork.Repository<Patient>()
                .FindAsync(p => p.UserId == userId);
            var patient = patients.FirstOrDefault();
            if (patient == null) return Unauthorized("Patient not found");

            try
            {
                var result = await _paymentService.ConfirmPayment(request.PaymentIntentId, patient.Id);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}