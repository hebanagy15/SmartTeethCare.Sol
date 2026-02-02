using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartTeethCare.Core.DTOs.PatientModule;
using SmartTeethCare.Core.Interfaces.Services.PatientModule;

namespace SmartTeethCare.API.Controllers.PatientModule
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Patient")]
    public class PatientPrescriptionsController : ControllerBase
    {
        private readonly IPatientPrescriptionService _prescriptionService;

        public PatientPrescriptionsController(IPatientPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }
        [HttpGet]
        public async Task<ActionResult<List<PrescriptionDetailsDTO>>> GetMyPrescriptions()
        {
            var user = HttpContext.User;
            var prescriptions = await _prescriptionService.GetMyPrescriptionsAsync(user);
            return Ok(prescriptions);
        }
        [HttpGet("appointment/{appointmentId}")]
        public async Task<ActionResult<PrescriptionDetailsDTO>> GetByAppointment(int appointmentId)
        {
            var user = HttpContext.User;

            try
            {
                var prescription = await _prescriptionService.GetByAppointmentAsync(appointmentId, user);
                return Ok(prescription);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid(); 
            }
        }
    }
}
