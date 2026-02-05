using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.DoctorModule;
using SmartTeethCare.Core.Interfaces.Services;
using System.Security.Claims;

namespace SmartTeethCare.API.Controllers
{
    [Authorize(Roles = "Doctor")]
    [ApiController]
    [Route("api/[controller]")]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePrescription(CreatePrescriptionDto dto)
        {
            var doctorUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _prescriptionService.CreatePrescriptionAsync(dto, doctorUserId);

            return Ok(new { message = "Prescription created successfully" });
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPatientPrescriptions(int patientId)
        {
            var result = await _prescriptionService
                .GetPrescriptionsByPatientIdAsync(patientId);

            return Ok(result);
        }
    }
}
